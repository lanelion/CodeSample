using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//장비숙련도

public class UIItemMasteryPopup : MonoBehaviour
{
    [SerializeField] List<UIItemQulitySlot> QulitySlotList;
    [SerializeField] UIItemMasterySkillPopup SkillInfoPopup;
    Animator animator;
    [System.NonSerialized] public PopupItemGrowth _owner;
    public void StartInitialize()
    {
        List<EquipTalentData> list = GetItemTypeList(ITEM_TYPE.WEAPON);
        animator = GetComponent<Animator>();
        for(int i=0;i< QulitySlotList.Count; i++)
        {
            
            MasteryLevelData info = PopupManager.Instance.MasteryLevelDatas.Where(n => n.itemType == list[i].itemType && n.qualityIndex == list[i].qualityIndex && n.level == 1).FirstOrDefault() ;
            QulitySlotList[i].init(this);
            QulitySlotList[i].SetData(info);
        }
        SkillInfoPopup.StartInitialize();
        SkillInfoPopup.OnClick_Close();
    }
   public void init( PopupItemGrowth owner)
    {
        _owner = owner;
    }
    public void OpenPopup()
    {

        gameObject.SetActive(true);
        UIPopupAniSystem.PlayOpen(animator);
        bool isnotice = false;
        for (int i = 0; i < QulitySlotList.Count; i++)
        {
            QulitySlotList[i].SetUserData();
        }
        for (int i = 0; i < QulitySlotList.Count; i++)
        {
            if (QulitySlotList[i].NoticeObj.activeSelf)
            {
                isnotice = true;
                break;
            }
        }

        _owner.TalentNoitce.SetActive(isnotice);
    }
    
    public void OpenSkillInfoPopup(ITEM_TYPE type, int QuilityIndex)
    {
    }
    public void OpenSkillInfoPopup(ITEM_TYPE type, int QuilityIndex,int skillorder)
    {
        SkillInfoPopup.OpenPopup(type, QuilityIndex,skillorder);
    }
    public void AniEventClose()
    {
        this.gameObject.SetActive(false);
    }
    public void ClosePopup()
    {
        if (animator != null)
        {
            UIPopupAniSystem.PlayClose(animator);
        }
    }
    public void OnClickClose()
    {
        ClosePopup();
    }
    public void OnClickOpen()
    {
        OpenPopup();
    }
    public void UpDataPopup()
    {
        bool isnotice = false;
        for (int i = 0; i < QulitySlotList.Count; i++)
        {
            QulitySlotList[i].SetUserData();
        }
        for (int i = 0; i < QulitySlotList.Count; i++)
        {
           if(QulitySlotList[i].NoticeObj.activeSelf)
            {
                isnotice = true;
                break;
            }
        }

        _owner.TalentNoitce.SetActive(isnotice);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpDataPopup();
    }
    List<EquipTalentData> GetItemTypeList(ITEM_TYPE type)
    {
        List<EquipTalentData> list = new List<EquipTalentData>();
        foreach (EquipTalentData _info in PopupManager.Instance.EquipTalentDatas)
        {
            if (_info.itemType == type)
            {
                list.Add(_info);
            }
        }
        list = list.OrderBy(x => x.Index).ToList();
        return list;
    }
}
