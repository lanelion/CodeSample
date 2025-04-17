using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemMasterySkillPopup : MonoBehaviour
{
    [SerializeField] List<UIItemQuilityInfoPopup> QuilityInfoPopupList;
    [SerializeField] RectTransform arrowrect;
    Canvas canvas;
    Dictionary<int, UIItemQuilityInfoPopup> _dicQuilityInfoPopup;
    public void StartInitialize()
    {
        canvas = UIPopupHandler.Get().UIRootCanvas;
        _dicQuilityInfoPopup = new Dictionary<int, UIItemQuilityInfoPopup>();
        List<EquipTalentData> list = GetItemTypeList(ITEM_TYPE.WEAPON);
        for(int i=0; i < list.Count; i++)
        {
            if (QuilityInfoPopupList.Count <= i)
                break;
            _dicQuilityInfoPopup.Add(list[i].qualityIndex, QuilityInfoPopupList[i]);
        }
    }
    public void OnClick_Close()
    {
        gameObject.SetActive(false);
        for(int i = 0; i < QuilityInfoPopupList.Count; i++)
        {
            QuilityInfoPopupList[i].gameObject.SetActive(false);
        }
    }
    public void OpenPopup(ITEM_TYPE type, int QuilityIndex,int skillorder)
    {
        if (UserGameData.Get()._dicEquipWeaponTalentLevel.ContainsKey(QuilityIndex)) {

            gameObject.SetActive(true);
            MasteryLevelData leveldata = PopupManager.Instance.MasteryLevelDatas.Where(n => n.itemType == type && n.qualityIndex == QuilityIndex
            && n.level == UserGameData.Get()._dicEquipWeaponTalentLevel[QuilityIndex]).FirstOrDefault();

            if (_dicQuilityInfoPopup.ContainsKey(QuilityIndex))
            {
                _dicQuilityInfoPopup[QuilityIndex].SetData(leveldata);
                _dicQuilityInfoPopup[QuilityIndex].ShowPopup(skillorder);
            }

            RectTransform rectTransform = gameObject.transform as RectTransform;
            Vector2 mousePosition = Input.mousePosition; // 마우스 좌표
            Vector2 localPosition = new Vector2(mousePosition.x / 2, mousePosition.y / 2);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, canvas.worldCamera , out localPosition);
            arrowrect.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, localPosition.y);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
