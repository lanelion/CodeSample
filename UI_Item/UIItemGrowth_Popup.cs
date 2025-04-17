using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UIItemGrowth_Popup : MonoBehaviour
{
   [System.NonSerialized] public bool UpdateDirty=false;
    [System.NonSerialized] UIItemSlot selectItemSlot;
    [SerializeField] public UIItemGrowthBase ReinforceTab;
    [SerializeField]public UIItemGrowthBase UpGradeTab;
    [SerializeField] public UIItemGrowthBase AdvancementTab;
    [SerializeField] public UIItemGrowthBase AwakeTab;
    [SerializeField] public Toggle[] toggles;
    [SerializeField] UIItemRefiningResult RefiningResult;
    [SerializeField] UIItemComposeResult ComposeResult;
    [SerializeField] UIItemAwakeResult AwakeResult;
    [SerializeField] public Text ReinforceTabText;
    [SerializeField] public Text UpGardeTabText;
    [SerializeField] public Text RefiningTabText;
    [SerializeField] Animator animator;
    [SerializeField] GameObject BackObj;
    [SerializeField] GameObject ToggleObj;
    [SerializeField] Text Compostext;
    PopupItemGrowth _owner = null;

    public void ClosePopup()
    {
        UIPopupAniSystem.PlayClose(animator, "", Close);
    }
    public void AniEventClose()
    {
        Close();
    }
    public void Close()
    {
        this.gameObject.SetActive(false);
        CloseAllPanel();
        BackObj.SetActive(false);
    }
    public void StartInitialize()
    {
        toggles[0].onValueChanged.AddListener((bool ison)=> {
            if (ison)
                OpenReinfoece();
        }); 
        toggles[1].onValueChanged.AddListener((bool ison) => {
            if (ison)
                OpenRefining();
        });
        ReinforceTabText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ENCHANT");
        toggles[0].isOn = true;
        RefiningResult.Init(_owner);
        RefiningResult.CloseResult();
        AwakeResult.Init(_owner);
        AwakeResult.CloseResult();
        ComposeResult.Init(_owner);
        ComposeResult.CloseResult();
    }
    public void init(PopupItemGrowth owner)
    {
        _owner = owner;
    }
    public void CloseAllPanel()
    {
        ReinforceTab.ClosePanel();
        UpGradeTab.ClosePanel();
        AwakeTab.ClosePanel();
        AdvancementTab.ClosePanel();
        ComposeResult.CloseResult();
        AwakeResult.CloseResult();
        RefiningResult.gameObject.SetActive(false);
        Compostext.gameObject.SetActive(false);
        BackObj.SetActive(false);
    }
    public void OpenPopup(eItemGrowthType type, UIItemSlot _selectItemSlot)
    {
        BackObj.SetActive(true);
        gameObject.SetActive(true);
        selectItemSlot = _selectItemSlot;
        toggles[1].gameObject.SetActive(true);

        switch (_selectItemSlot.EquipDataInfo.Grade)
        {
            case ITEM_GRADE.MYTH:
                {
                    UpGardeTabText.text = LocalizeManager.Instance.GetTXT("STR_UI_AWAKE");
                }
                break;
            case ITEM_GRADE.ANCIENT:
                {
                }
                break;
            default:
                {
                    UpGardeTabText.text = LocalizeManager.Instance.GetTXT("STR_UI_GRADEUP");
                }
                break;

        }
        switch (_selectItemSlot.EquipDataInfo.itemType)
        {
            case ITEM_TYPE.ACCESSARY:
                {
                    RefiningTabText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_JEWELRY");
                }
                break;
            default:
                {
                    RefiningTabText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
                }
                break;
        }
        EnableItem selectitem = UserGameData.Get().GetEnableItem(_selectItemSlot.EquipDataInfo.ItemId);
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == _selectItemSlot.EquipDataInfo.itemType && x.grade == _selectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == _selectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                            where v.equipType == _selectItemSlot.EquipDataInfo.itemType && v.grade == _selectItemSlot.EquipDataInfo.Grade
                                            && v.gradeIndex == _selectItemSlot.EquipDataInfo.gradeIndex
                                            orderby v.maxLevel descending
                                            select v).FirstOrDefault();
        switch (type)
        {
            case eItemGrowthType.REINFORCE:
                {
                    ToggleObj.gameObject.SetActive(true);
                    Compostext.gameObject.SetActive(false);
                    toggles[0].isOn = true;
                    OpenReinfoece();
                }
                break;
            case eItemGrowthType.REFINING:
                {
                    ToggleObj.gameObject.SetActive(false);
                    Compostext.gameObject.SetActive(false);
                    toggles[1].isOn = true; 
                    OpenRefining();
                }
                break;
            case eItemGrowthType.COMPOSE:
                {
                    ToggleObj.gameObject.SetActive(false);
                    Compostext.gameObject.SetActive(true);
                    OpenGradeUp();
                }
                break;
            case eItemGrowthType.ADVANCEMENT:
                {
                    ToggleObj.gameObject.SetActive(false);
                    Compostext.gameObject.SetActive(true);
                    OpenAdvancement();
                }
                break;
        }

        UIPopupAniSystem.PlayOpen(animator);
    }
    public void OpenReinfoece()
    {
        toggles[1].isOn = false;
        CloseAllPanel();
        BackObj.SetActive(true);
    }
    public void OpenRefining()
    {
        toggles[0].isOn = false;
        CloseAllPanel();
        BackObj.SetActive(true);
        Compostext.gameObject.SetActive(true);
        if (selectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
        {
            Compostext.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_JEWELRY");
        }
        else
            Compostext.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
    }
    public void OpenGradeUp()
    {
        toggles[0].isOn = false;
        CloseAllPanel();
        Compostext.gameObject.SetActive(true);
        Compostext.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_COMPOSE");
        BackObj.SetActive(true);
        UpGradeTab.OpenPanel(selectItemSlot);
    }
    public void OpenAdvancement()
    {
        toggles[0].isOn = false;
        CloseAllPanel();
        Compostext.gameObject.SetActive(true);
        Compostext.text = LocalizeManager.Instance.GetTXT("STR_UI_GROW_PROMOTION");
        BackObj.SetActive(true);
        AdvancementTab.OpenPanel(selectItemSlot);
    }
    public void OpenAwake()
    {
        toggles[0].isOn = false;
        CloseAllPanel();
        BackObj.SetActive(true);
        Compostext.gameObject.SetActive(true);
        Compostext.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_FORGE");
        AwakeTab.OpenPanel(selectItemSlot);
    }
    // Start is called before the first frame update
    public void OpenRefiningResult()
    {
        RefiningResult.OpenResult(selectItemSlot, this._owner);
    }
    public void OpenTierUpResult()
    {
        RefiningResult.OpenAdvancementResult(selectItemSlot, this._owner);
    }
    public void OpenComposeResult(int count)
    {
        ComposeResult.OpenResult(selectItemSlot, count);
    }
    public void OpenAwakeResult(EquipInfoData _info)
    {
        AwakeResult.OpenResult(_info);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void RefreshUI(UIItemSlot _selectitem)
    {
        if (!UpdateDirty)
        {
            return;
        }
        else
        {
            UpdateDirty = false;
        }

        
    }
}
