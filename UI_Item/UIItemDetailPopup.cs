using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
public class AbilityClass
{
    public eStatType stattype;
    public float value;
}
public class UIItemDetailPopup : UIItemGrowthBase
{
    [SerializeField] ScrollRect SetItemScroll;
    [SerializeField] UIEffectText MainItemEffect;//무기는 초당공격력 그외는 주스텟예정
    [SerializeField] Text TalentEffectText;
    [SerializeField] UIEffectText ItemTalentEffect;
    [SerializeField] GameObject ItemTalentObj;
    [SerializeField] Text ItemTypeEffectTitle;
    [SerializeField] Text ItemGradeText;

    [SerializeField] Text EquipItemLevelText;
    [SerializeField] Text ItemLevelText;
    [SerializeField] Text MaxLevelText;
    [SerializeField] Text EquipItemSkillTypeText;
    [SerializeField] List<UIEffectText> ItemEffectList;
    [SerializeField] List<UIEffectText> ItemEquipEffectList;
    [SerializeField] List<Text> ItemEquipEffectLockObjs;
    [SerializeField] UIItemSlot Detailslot;
    [SerializeField] Image GradeIcon;
    [SerializeField] Text ItemnameText;
    [SerializeField] List<ContentSizeFitter> _sizefitterlist;
    [SerializeField] Text EnchantBtnCoverText;
    [SerializeField] GameObject EquipBtnCover;
    [SerializeField] GameObject EquipBtn;
    [SerializeField] Text EnchantBtnText;
    [SerializeField] GameObject EnchantBtn;
    [SerializeField] GameObject EnchantBtnCover;
    [SerializeField] Text UpGardeBtnText;                   //합성
    [SerializeField] Text UpGardeBtnCoverText;
    [SerializeField] GameObject UpGardeBtnCover;
    [SerializeField] GameObject UpGardeBtn;
    [SerializeField] Text RefiningBtnCoverText;
    [SerializeField] Text RefiningBtnText;
    [SerializeField] GameObject RefiningBtn;
    [SerializeField] GameObject RefiningNoftiy;
    [SerializeField] GameObject RefiningBtnCover;
    [SerializeField] GameObject SkillinfoObj;
    [SerializeField] GameObject AdvancementBtn;
    [SerializeField] GameObject AdvancementBtnCover;             
    [SerializeField] UIItemAwakeSkillInfo AwakeSkillinfo;
    [SerializeField] private Image costImage = null;
    [SerializeField] Text CostText;
    [SerializeField] Text UserCostText;
    [SerializeField] Color CantRefiningTextColor;
    [SerializeField] Color CanRefiningTextColor;
    [SerializeField] Button ReinforceBtn;
    [SerializeField] Transform EffectPos;
    [SerializeField] GameObject UserCostObj;
    [SerializeField] float sequenceTime = 0.5f;
    [SerializeField] float delayTime = 0.2f;
    [SerializeField] private GameObject upgradeBadge = null;
    [SerializeField] private GameObject refiningBadge = null;
    private float startTime;
    private float clickTime;
    private bool clicked;
    int levelupcheck = -1;

    [Header("해금 시스템용 아이콘 필드")]
    [SerializeField] private GameObject enchantLockBtn;
    [SerializeField] private GameObject refiningLockBtn;
    [SerializeField] private GameObject advancementLockBtn;
    [SerializeField] private GameObject composeLockBtn;

    List<UIItemSetSlot> SetSlotList;
    Animator animator;
    public override EquipInfoData SelectItemSlotInfo => SelectItemSlot.EquipDataInfo;
    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.DETAIL; }
    public override void StartInitialize()
    {
        base.StartInitialize();
        SetSlotList = new List<UIItemSetSlot>();
        AwakeSkillinfo.gameObject.SetActive(false); 
        if (AwakeSkillinfo.SkillPopupObj.activeSelf)
            AwakeSkillinfo.CloseSkillinfoPopup();
        SkillinfoObj.SetActive(false);
        animator = GetComponent<Animator>();
    }
    public override void OpenPanel(UIItemSlot _selectItemSlot = null)
    {
     
        base.OpenPanel(_selectItemSlot);
        if (_selectItemSlot==null)
            return;
        ReSetSetSlot();
        RefleshUI();
        ReinforceBtn.enabled = true;
        UIPopupAniSystem.PlayOpen(animator);
    }
    //장비갱신
    void RefleshUI()
    {
        if (SelectItemSlot == null)
            return;
        ReSetSetSlot();
        Detailslot.SetData(SelectItemSlot.EquipDataInfo, null);
        Detailslot.ReInit();
        GradeIcon.sprite = ItemSystem.GetGradeiconSprite(Detailslot.EquipDataInfo.Grade);
        ItemnameText.text =string.Format(ItemSystem.GetItemGradeTextAndColorKey(SelectItemSlot.EquipDataInfo.Grade, SelectItemSlot.EquipDataInfo.gradeIndex), LocalizeManager.Instance.GetTXT(Detailslot.EquipDataInfo.nameTextID)) ;
        string SubTypeText = ItemSystem.GetItemSubTypeTitelText(SelectItemSlot.EquipDataInfo.itemType, SelectItemSlot.EquipDataInfo.qualityIndex);
        ItemGradeText.text = SubTypeText;
        ItemTypeEffectTitle.text = ItemSystem.GetItemSubTypeEffectTitelText(SelectItemSlot.EquipDataInfo.itemType, SelectItemSlot.EquipDataInfo.qualityIndex);
        EquipAbilityData equipstatdata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index == SelectItemSlot.EquipDataInfo.ItemId);
        AdvancementBtn.SetActive(false);
        UserCostObj.SetActive(true);
        switch (SelectItemSlot.EquipDataInfo.itemType)
        {   //무기류
            case ITEM_TYPE.WEAPON:
                {
                    ItemEffectList[0].gameObject.SetActive(true);
                    ItemEffectList[1].gameObject.SetActive(false);

                    EquipTalentData talentdata = PopupManager.Instance.EquipTalentDatas.Where(x => x.itemType == SelectItemSlot.EquipDataInfo.itemType && x.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex).FirstOrDefault();
                    
                    if (ItemSystem.CheckEnable(SelectItemSlot.EquipDataInfo.ItemId, out EnableItem enableItem))
                    {
                        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Find(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
                            && x.reforge == enableItem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex);

                        if(refiningdata.maxLevel > enableItem.level)
                        {
                            MainItemEffect.SetStatTypeAddValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level)
                                , equipstatdata.MainabilityList[0].LevelValue);

                            if (equipstatdata.MainabilityList.Count > 1)
                            {
                                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                                {
                                    ItemEffectList[i - 1].gameObject.SetActive(true);

                                    if (equipstatdata.MainabilityList[i].LevelValue > 0)
                                    {
                                        ItemEffectList[i - 1].SetStatTypeAddValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)
                                            , equipstatdata.MainabilityList[i].LevelValue);
                                    }

                                    if (equipstatdata.MainabilityList[i].LevelValue <= 0)
                                    {
                                        if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                            ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                                        else
                                        {
                                            string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                                            ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level));

                            if (equipstatdata.MainabilityList.Count > 1)
                            {
                                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                                {
                                    ItemEffectList[i - 1].gameObject.SetActive(true);

                                    if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                        ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                                    else
                                    {
                                        string value = (equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString();
                                        ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value);
                        if (equipstatdata.MainabilityList.Count > 1)
                        {
                            for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                            {
                                ItemEffectList[i - 1].gameObject.SetActive(true);

                                if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                    ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue));
                                else
                                {
                                    string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                                    ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                }
                            }
                        }
                    }
                    EquipItemSkillTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_SKILL");
                }
                break;
            //악세사리류
            case ITEM_TYPE.ACCESSARY:
                {
                    ItemEffectList[1].gameObject.SetActive(false);
                    ItemEffectList[0].gameObject.SetActive(false);
                    EquipItemSkillTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_SKILL");

                    if (ItemSystem.CheckEnable(SelectItemSlot.EquipDataInfo.ItemId, out EnableItem enableItem))
                    {
                        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
            && x.reforge == enableItem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
                        
                        if (refiningdata.maxLevel > enableItem.level)
                        {
                            MainItemEffect.SetStatTypeAddValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level)
                                , equipstatdata.MainabilityList[0].LevelValue);
                            if (equipstatdata.MainabilityList.Count > 1)
                            {
                                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                                {
                                    ItemEffectList[i - 1].gameObject.SetActive(true);

                                    if (equipstatdata.MainabilityList[i].LevelValue > 0)
                                    {
                                        ItemEffectList[i - 1].SetStatTypeAddValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)
                                            , equipstatdata.MainabilityList[i].LevelValue);
                                    }

                                    if (equipstatdata.MainabilityList[i].LevelValue <= 0)
                                    {
                                        if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                            ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                                        else
                                        {
                                            string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                                            ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level));

                            if (equipstatdata.MainabilityList.Count > 1)
                            {
                                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                                {
                                    ItemEffectList[i - 1].gameObject.SetActive(true);

                                    if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                        ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                                    else
                                    {
                                        string value = (equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString();
                                        ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value);
                        if (equipstatdata.MainabilityList.Count > 1)
                        {
                            for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                            {
                                ItemEffectList[i - 1].gameObject.SetActive(true);

                                if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                    ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + equipstatdata.MainabilityList[i].LevelValue);
                                else
                                {
                                    string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                                    ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                }
                            }
                        }
                        MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value);
                    }
                }
                break;
            //방패류
            case ITEM_TYPE.SHIELD:
                {
                    ItemEffectList[1].gameObject.SetActive(false);
                    ItemEffectList[0].gameObject.SetActive(false);
                    EquipItemSkillTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_SHIELD_SKILL");

                    EquipTalentData talentdata = PopupManager.Instance.EquipTalentDatas.Where(x => x.itemType == SelectItemSlot.EquipDataInfo.itemType && x.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex).FirstOrDefault();
                    
                    if (ItemSystem.CheckEnable(SelectItemSlot.EquipDataInfo.ItemId, out EnableItem enableItem))
                    {
                        MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level));
                        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
                         && x.reforge == enableItem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
                        
                        if (refiningdata.maxLevel > enableItem.level)
                        {
                            MainItemEffect.SetStatTypeAddValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level)
                                , equipstatdata.MainabilityList[0].LevelValue);
                            if (equipstatdata.MainabilityList.Count > 1)
                            {
                                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                                {
                                    ItemEffectList[i - 1].gameObject.SetActive(true);

                                    if (equipstatdata.MainabilityList[i].LevelValue > 0)
                                    {
                                        ItemEffectList[i - 1].SetStatTypeAddValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)
                                            , equipstatdata.MainabilityList[i].LevelValue);
                                    }

                                    if (equipstatdata.MainabilityList[i].LevelValue <= 0)
                                    {
                                        if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                            ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                                        else
                                        {
                                            string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                                            ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level));
                            if (equipstatdata.MainabilityList.Count > 1)
                            {
                                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                                {
                                    ItemEffectList[i - 1].gameObject.SetActive(true);

                                    if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                        ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                                    else
                                    {
                                        string value = (equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString();
                                        ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (equipstatdata.MainabilityList.Count > 1)
                        {
                            for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                            {
                                ItemEffectList[i - 1].gameObject.SetActive(true);

                                if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                                    ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + equipstatdata.MainabilityList[i].LevelValue);
                                else
                                {
                                    string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                                    ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                                }
                            }
                        }
                        MainItemEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value);
                    }
                }
                break;
        }
        EnableItem myitem = UserGameData.Get().GetEnableItem(Detailslot.EquipDataInfo.ItemId);

        if (myitem != null)
        {
            if (myitem.equipped)
            {
                EquipBtnCover.SetActive(true);
                EquipBtn.SetActive(false);
            }
            else
            {
                EquipBtnCover.SetActive(false);
                EquipBtn.SetActive(true);
            }

            EnchantBtn.SetActive(true);
            UpGardeBtn.SetActive(true);
            EnchantBtnCover.SetActive(false);
            AdvancementBtnCover.SetActive(false);
            UpGardeBtnCover.SetActive(false);
            RefiningBtn.SetActive(true);
            RefiningBtnCover.SetActive(false);
            EquipItemLevelText.gameObject.SetActive(true);
            UserCostObj.SetActive(true);
            ItemLevelText.text=myitem.level.ToString();
            EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
                && x.reforge == myitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
            
            EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                                where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade
                                                && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                                orderby v.maxLevel descending
                                                select v).FirstOrDefault();

            EquipLevelUpCostData CostData = PopupManager.Instance.EquipLevelUpCostDatas.Where(n => n.equipID == SelectItemSlot.EquipDataInfo.ItemId && n.level == myitem.level).FirstOrDefault();
            ItemClass _refiningCostItem = UserGameData.Get().GetItemClass(refiningdata.reforgeItem[0].itemID);
            if (CostData != null && CostData.itemTypes.Count > 0)
            {
                if (CostData.itemTypes[0].ID > 0)
                {
                    ItemInfoData itemdata = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == CostData.itemTypes[0].ID).FirstOrDefault();
                    ItemClass usecostitem = UserGameData.Get().GetItemClass(CostData.itemTypes[0].ID);
                    double costvalue = CostData.itemTypes[0].value;
                    if (CharacterManager.Instance.MyActor != null)
                    {
                        int downstat = (int)(CostData.itemTypes[0].value * CharacterManager.Instance.MyActor.stat.equipEncCostDown);
                        int downvalue = (int)CharacterManager.Instance.MyActor.buff.GetEquipenccostdown(SKILL_VALUE_TYPE.INTERGER);
                        int downpervalue = (int)(CostData.itemTypes[0].value * CharacterManager.Instance.MyActor.buff.GetEquipenccostdown(SKILL_VALUE_TYPE.PERCENT));
                        costvalue -= (downvalue + downpervalue+ downstat);
                    }

                    costImage.sprite = ItemSystem.GetItemSprite(itemdata.icon);
                    if (usecostitem == null || usecostitem.num < costvalue)
                    {
                        if (usecostitem == null)
                        {
                            UserCostText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + 0 + "</color>", costvalue);
                        }
                        else
                        {
                            UserCostText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + usecostitem.num + "</color>", costvalue);
                        }
                    }
                    else
                    {
                        UserCostText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), usecostitem.num, costvalue);
                        UserCostText.color = CanRefiningTextColor;
                    }
                    CostText.text = costvalue.ToString();
                }
            }
            else
            {
                UserCostObj.SetActive(false);
            }
            MaxLevelText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MAXLV"), refiningdata.maxLevel);

            if (refiningdata.Index > 0)
            {
                if (maxrefiningdata.maxLevel > myitem.level && refiningdata.maxLevel <= myitem.level)
                {//재련
                    if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPREF))
                    {
                        if (maxrefiningdata.reforge == myitem.refining)
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(false);
                            EnchantBtn.SetActive(true);
                            EnchantBtnCover.SetActive(false);
                            enchantLockBtn.SetActive(false);
                            refiningLockBtn.SetActive(false);
                        }
                        else
                        {
                            if (_refiningCostItem == null || _refiningCostItem.num < refiningdata.reforgeItem[0].itemValue)
                            {
                                RefiningBtn.SetActive(false);
                                RefiningBtnCover.SetActive(true);
                                EnchantBtn.SetActive(false);
                                EnchantBtnCover.SetActive(false);
                                enchantLockBtn.SetActive(false);
                                refiningLockBtn.SetActive(false);
                            }
                            else
                            {
                                RefiningBtn.SetActive(true);
                                RefiningBtnCover.SetActive(false);
                                EnchantBtn.SetActive(false);
                                EnchantBtnCover.SetActive(false);
                                enchantLockBtn.SetActive(false);
                                refiningLockBtn.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        RefiningBtn.SetActive(false);
                        RefiningBtnCover.SetActive(false);
                        EnchantBtn.SetActive(false);
                        EnchantBtnCover.SetActive(false);
                        enchantLockBtn.SetActive(false);
                        refiningLockBtn.SetActive(true);
                    }

                    costImage.sprite = ItemSystem.GetItemSprite(UserManager.Instance.ItemInfoDatas.Find(x => x.Index == refiningdata.reforgeItem[0].itemID).icon);

                    if (_refiningCostItem == null || _refiningCostItem.num < refiningdata.reforgeItem[0].itemValue)
                    {
                        if (_refiningCostItem == null)
                        {
                            UserCostText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + 0 + "</color>", refiningdata.reforgeItem[0].itemValue);
                        }
                        else
                        {
                            UserCostText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + _refiningCostItem.num + "</color>", refiningdata.reforgeItem[0].itemValue);
                        }

                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPREF))
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(true);
                            refiningLockBtn.SetActive(false);
                            EnchantBtn.SetActive(false);
                            EnchantBtnCover.SetActive(false);
                            enchantLockBtn.SetActive(false);
                        }
                        else
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(false);
                            refiningLockBtn.SetActive(true);
                            EnchantBtn.SetActive(false);
                            EnchantBtnCover.SetActive(false);
                            enchantLockBtn.SetActive(false);
                        }
                    }
                    else
                    {
                        UserCostText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), _refiningCostItem.num, refiningdata.reforgeItem[0].itemValue);
                        UserCostText.color = CanRefiningTextColor;
                    }

                    EnchantBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");

                    refiningBadge.SetActive(myitem.isRefining);
                    upgradeBadge.SetActive(myitem.isUpgrade);
                    SelectItemSlot.ReInit();
                    if (myitem.refining < 5) // 임시로 승급을 없애기 위해서 만듦, 필준
                    {
                        Detailslot.ReInit(myitem.refining + 1);
                        EnchantBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_BTN_ENCHANT");
                        RefiningBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
                    }
                    else
                    {
                        UserCostObj.SetActive(false);
                        EnchantBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_MAX");
                        RefiningBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_MAX");
                    }

                    if(refiningdata.needSelfValue<= myitem.num)
                    {
                        for(int i=0;i< refiningdata.reforgeItem.Count; i++)
                        {
                            ItemInfoData iteminfo = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==refiningdata.reforgeItem[i].itemID).FirstOrDefault();
                            if (iteminfo.itemType != ITEM_TYPE.GOLD)
                            {
                                ItemClass item = UserGameData.Get().GetItemClass(refiningdata.reforgeItem[i].itemID);
                                if (item != null)
                                {
                                    if (item.num >= refiningdata.reforgeItem[i].itemValue)
                                    {
                                        //RefiningNoftiy.SetActive(true);
                                    }
                                    else
                                    {
                                        //RefiningNoftiy.SetActive(false);
                                        break;
                                    }
                                }
                                else
                                {
                                    //RefiningNoftiy.SetActive(false);
                                    break;
                                }
                            }
                            else
                            {
                                if (UserGameData.Get().AssetGold>= refiningdata.reforgeItem[i].itemValue)
                                {
                                    //RefiningNoftiy.SetActive(true);
                                }
                                else
                                {
                                    //RefiningNoftiy.SetActive(false); 
                                    break;
                                }
                            }
                        }
                    }

                    UIManager.Instance.SetBadgeActive("ItemGrowthRefining", true);
                }
                else if (maxrefiningdata.maxLevel == myitem.level)
                {//max
                    EnchantBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_MAX");
                    EnchantBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_MAX");
                    RefiningBtn.SetActive(false);
                    RefiningBtnCover.SetActive(false);
                    EnchantBtn.SetActive(false);
                    EnchantBtnCover.SetActive(true);
                    UserCostObj.SetActive(false);
                    enchantLockBtn.SetActive(false);
                    refiningLockBtn.SetActive(false);
                    MaxLevelText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MAXLV"), myitem.level);
                }
                else
                {//강화
                    ItemClass usecostitem = UserGameData.Get().GetItemClass(CostData.itemTypes[0].ID);
                    double costvalue = CostData.itemTypes[0].value;

                    if (usecostitem == null || usecostitem.num < costvalue)
                    {
                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPENC))
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(false);
                            EnchantBtn.SetActive(false);
                            EnchantBtnCover.SetActive(true);
                            enchantLockBtn.SetActive(false);
                            refiningLockBtn.SetActive(false);
                        }
                        else
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(false);
                            EnchantBtn.SetActive(false);
                            EnchantBtnCover.SetActive(false);
                            enchantLockBtn.SetActive(true);
                            refiningLockBtn.SetActive(false);
                        }
                    }
                    else
                    {
                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPENC))
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(false);
                            EnchantBtn.SetActive(true);
                            EnchantBtnCover.SetActive(false);
                            enchantLockBtn.SetActive(false);
                            refiningLockBtn.SetActive(false);
                        }
                        else
                        {
                            RefiningBtn.SetActive(false);
                            RefiningBtnCover.SetActive(false);
                            EnchantBtn.SetActive(false);
                            EnchantBtnCover.SetActive(false);
                            enchantLockBtn.SetActive(true);
                            refiningLockBtn.SetActive(false);
                        }
                    }

                    SelectItemSlot.ReInit();

                    EnchantBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ENCHANT");
                    EnchantBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ENCHANT");
                }
            }
            
            if (myitem.isnew)
            {
                myitem.isnew = false;
                UserGameData.Get().UpdateEquipItme(myitem);
                _owner.SetNotify = false;
            }

            if (Detailslot.EquipDataInfo.Grade == ITEM_GRADE.ANCIENT)
            {
                if (myitem.refining >= 5) 
                {
                    if (myitem.level >= 50)
                    {
                        RefiningBtn.SetActive(false);
                        RefiningBtnCover.SetActive(false);
                        refiningLockBtn.SetActive(false);
                        EnchantBtn.SetActive(false);
                        EnchantBtnCover.SetActive(true);
                        enchantLockBtn.SetActive(false);
                    }
                    else
                    {
                        RefiningBtn.SetActive(false);
                        RefiningBtnCover.SetActive(false);
                        refiningLockBtn.SetActive(false);
                        EnchantBtn.SetActive(true);
                        EnchantBtnCover.SetActive(false);
                        enchantLockBtn.SetActive(false);
                    }
                }

                UpGardeBtn.SetActive(false);
                UpGardeBtnCover.SetActive(false);
                composeLockBtn.SetActive(false);
                AdvancementBtn.SetActive(false);
                AdvancementBtnCover.SetActive(false);
                advancementLockBtn.SetActive(false);

                
            }
            else
            {
                EquipComposeData composedata = PopupManager.Instance.EquipComposeDatas.Find(x=>x.Index==myitem.Index);

                if (Detailslot.EquipDataInfo.Grade == ITEM_GRADE.MYTH)
                {
                    if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPAWK))
                    {
                        UpGardeBtn.SetActive(true);
                        UpGardeBtnCover.SetActive(false);
                        composeLockBtn.SetActive(false);
                    }
                    else
                    {
                        UpGardeBtn.SetActive(false);
                        UpGardeBtnCover.SetActive(false);
                        composeLockBtn.SetActive(true);
                    }

                    UpGardeBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_FORGE");
                    UpGardeBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_FORGE");
                }
                
                if (Detailslot.EquipDataInfo.Grade != ITEM_GRADE.MYTH)
                {
                    if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPCOM))
                    {
                        if (composedata.ResourceCount > myitem.num)
                        {
                            UpGardeBtn.SetActive(false);
                            UpGardeBtnCover.SetActive(true);
                            composeLockBtn.SetActive(false);
                        }
                        else
                        {
                            UpGardeBtn.SetActive(true);
                            UpGardeBtnCover.SetActive(false);
                            composeLockBtn.SetActive(false);
                        }
                    }
                    else
                    {
                        UpGardeBtn.SetActive(false);
                        UpGardeBtnCover.SetActive(false);
                        composeLockBtn.SetActive(true);
                    }

                    UpGardeBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_COMPOSE");
                    UpGardeBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_COMPOSE");
                }

                AdvancementBtn.SetActive(false);
                AdvancementBtnCover.SetActive(false);
                advancementLockBtn.SetActive(false);
            }
        }
        else
        {
            EnchantBtn.SetActive(false);
            EnchantBtnCover.SetActive(false);
            enchantLockBtn.SetActive(false);
            UpGardeBtnCover.SetActive(false);
            UpGardeBtn.SetActive(false);
            composeLockBtn.SetActive(false);
            EquipBtnCover.SetActive(false); 
            RefiningBtn.SetActive(false);
            RefiningBtnCover.SetActive(false);
            refiningLockBtn.SetActive(false);
            EquipBtn.SetActive(false);
            AdvancementBtn.SetActive(false);
            AdvancementBtnCover.SetActive(false);
            EquipItemLevelText.gameObject.SetActive(false);
            UserCostObj.SetActive(false);
            //강화
            
            Detailslot.ReInit();

            
        }
        for (int i = 0; i < ItemEquipEffectList.Count; i++)
        {
            ItemEquipEffectList[i].gameObject.SetActive(false);
        }
        
        Dictionary<eStatType, AbilityType> _dicstat = new Dictionary<eStatType, AbilityType>();

        for (int i = 0; i < _sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_sizefitterlist[i].transform);
        }
        
        AwakeSkillinfo.SetAwakeSkillicon(false);
        SkillinfoObj.SetActive(false);
        

        switch (SelectItemSlot.EquipDataInfo.Grade)
        {
            case ITEM_GRADE.MYTH:
                {
                    AwakeSkillinfo.CloseSkillinfoPopup();
                }
                break;
            case ITEM_GRADE.ANCIENT:
                {
                    if (SelectItemSlot.EquipDataInfo.skillinfoID > 0)
                    {
                        SkillinfoObj.SetActive(true);
                        AwakeSkillinfo.SetData(SkillManager.Instance.GetSkillData(SelectItemSlot.EquipDataInfo.skillinfoID), SelectItemSlot.EquipDataInfo);
                        AwakeSkillinfo.SetAwakeSkillicon(true);
                    }
                }
                break;
            default:
                {
                    AwakeSkillinfo.CloseSkillinfoPopup();

                    SkillinfoObj.SetActive(false);
                }
                break;
        }
        if (equipstatdata.AbilityTypes.Count > 0)
        {
            ItemTypeEffectTitle.gameObject.SetActive(true);
            List<float> AddEffectList = new List<float>();
            List<int> AddEffectindexList = new List<int>();
            for (int v = 0; v < equipstatdata.AbilityTypes.Count; v++)
            {
                ItemEquipEffectList[v].gameObject.SetActive(true);
                ItemEquipEffectList[v].SetStatTypeValue(equipstatdata.AbilityTypes[v].Type, equipstatdata.AbilityTypes[v].Value);
                AddEffectList.Add(equipstatdata.AbilityTypes[v].Value);
                {
                    ItemEquipEffectList[v].LockObj.SetActive(false);
                }
            }
            if ( myitem!=null)
            {
                for (int i = 0; i < AddEffectindexList.Count; i++)
                {
                    AddEffectList[AddEffectindexList[i]] += equipstatdata.AbilityTypes[AddEffectindexList[i]].LevelValue * (myitem.refining - 1);
                }
                for (int i = 0; i < equipstatdata.AbilityTypes.Count; i++)
                {
                    ItemEquipEffectList[i].SetStatTypeValue(equipstatdata.AbilityTypes[i].Type, AddEffectList[i] + equipstatdata.AbilityTypes[i].LevelValue * (myitem.refining - 1));
                }
            }
        }
        else
        {
            ItemTypeEffectTitle.gameObject.SetActive(false);
        }
    }
    public void OnClickCloseBtn()
    {
        ReSetSetSlot();
        _owner.SelectGrowPanel(eItemGrowthType.NORAML);
        if (SelectItemSlot != null)
            SelectItemSlot.SetSelectedSlot(false);
    }
    public override void ClosePanel()
    {
        if (animator != null)
        {
            UIPopupAniSystem.PlayClose(animator);
        }
    }
    public void AniEventClose()
    {
        Util.SetActiveObject(this.gameObject, false);
    }
    public void Close()
    {
        ReSetSetSlot();
        _owner.SelectGrowPanel(eItemGrowthType.NORAML);
        SelectItemSlot.SetSelectedSlot(false);
    }
    public void OnClick_EquipBtn()
    {
        string beforeitem = UserGameData.Get().EquipItems;
        SelectItemSlot.OnClick_EquipBtn();
        RefleshUI();
        string state = "UserEquip" + UserGameData.Get().EquipItems;
        if (beforeitem != state)
        {
            NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "EQUIPITEM", state, "BeforeEquip:"+beforeitem);
        }

    }
    public void OnClick_AdvancementBtn()
    {
        if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPUPG))
        {
            UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPUPG);
            return;
        }

        _owner.ItemGrowthPopup.OpenPopup(eItemGrowthType.ADVANCEMENT, SelectItemSlot);
    }
    
    void ReSetSetSlot()
    {
        if (SetSlotList == null)
            SetSlotList = new List<UIItemSetSlot>();
        for (int i=0; i< SetSlotList.Count; i++)
        {
            SetSlotList[i].gameObject.SetActive(false);
        }
    }
    public void OnClick_ReinforceBtn()
    {
        if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPENC))
        {
            UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPENC);
            return;
        }

        _owner.ItemGrowthPopup.OpenPopup(eItemGrowthType.REINFORCE, SelectItemSlot);
    }
    public void OnClick_UpGardeBtn()
    {
        if (Detailslot.EquipDataInfo.Grade.Equals(ITEM_GRADE.ANCIENT))
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPUPG))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPUPG);
                return;
            }
        }
        else if (Detailslot.EquipDataInfo.Grade.Equals(ITEM_GRADE.MYTH))
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPAWK))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPAWK);
                return;
            }
        }
        else
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPCOM))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPCOM);
                return;
            }
        }

        _owner.ItemGrowthPopup.OpenPopup(eItemGrowthType.COMPOSE, Detailslot);
    }
    public void OnClick_RefiningBtn()
    {
        if (SelectItemSlotInfo.itemType.Equals(ITEM_TYPE.ACCESSARY))
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPWRK))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPWRK);
                return;
            }
        }
        else
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPREF))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPREF);
                return;
            }
        }

        _owner.ItemGrowthPopup.OpenPopup(eItemGrowthType.REFINING, SelectItemSlot);
    }
    
    public void OnClickReinforceBtn()
    {
        ReinforceBtn.enabled = false;
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        if (selectitem == null)
        {
            ReinforceBtn.enabled = true;
            return;
        }
        EquipLevelUpCostData levelupInfo = PopupManager.Instance.EquipLevelUpCostDatas.Where(n => n.equipID == selectitem.Index && n.level == selectitem.level).FirstOrDefault();
        if (levelupInfo == null)
        {
            ReinforceBtn.enabled = true;
            return;
        }
        EquipLevelUpCostData CostData = PopupManager.Instance.EquipLevelUpCostDatas.Where(n => n.equipID == selectitem.Index && n.level == selectitem.level).FirstOrDefault();
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
          && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        
        long beforepow = 0;
        if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
            beforepow = (long)CharacterManager.Instance.MyActor.stat.GetBattlePower();
        if (selectitem.level >= refiningdata.maxLevel)
        {
            ReinforceBtn.enabled = true;
            return;
        }

        if (0 < ItemSystem.LevelUp(SelectItemSlot.EquipDataInfo, CostData.itemTypes, 100f))
        {
            StartCoroutine(TryReinforce());
            EquipAbilityData equipstatdata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId));
            ItemSystem.CheckEnable(SelectItemSlot.EquipDataInfo.ItemId, out EnableItem enableItem);
            MainItemEffect.RollingNumber(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level)
                                , equipstatdata.MainabilityList[0].LevelValue);
            if (equipstatdata.MainabilityList.Count > 1)
            {
                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                {
                    ItemEffectList[i - 1].gameObject.SetActive(true);

                    if (equipstatdata.MainabilityList[i].LevelValue > 0)
                    {
                        ItemEffectList[i - 1].RollingNumber(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)
                            , equipstatdata.MainabilityList[i].LevelValue);
                    }

                    if (equipstatdata.MainabilityList[i].LevelValue <= 0)
                    {
                        if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                            ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                        else
                        {
                            string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                            ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                        }
                    }
                }
            }
            CharacterManager.Instance.MyActor.InitStat();
            
            if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.WEAPON)
            {
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.WEAPONLVUP] = true;
                UserManager.Instance.SetEventCount(eQuestType.WEAPONLVUP, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.WEAPONLVUP] = false;
            }
            else if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.SHIELD)
            {
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.SHIELDLVUP] = true;
                UserManager.Instance.SetEventCount(eQuestType.SHIELDLVUP, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.SHIELDLVUP] = false;
            }
            else if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
            {
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.ACCESSARYLVUP] = true;
                UserManager.Instance.SetEventCount(eQuestType.ACCESSARYLVUP, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.ACCESSARYLVUP] = false;
            }

            if (SelectItemSlotInfo != null)
            {
                ITEM_TYPE itemType = SelectItemSlotInfo.itemType;

                if (itemType.Equals(ITEM_TYPE.WEAPON))
                {
                    UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPWEAPONENC);
                }
                else if (itemType.Equals(ITEM_TYPE.SHIELD))
                {
                    UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPSHIELDENC);
                }
                else if (itemType.Equals(ITEM_TYPE.ACCESSARY))
                {
                    UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPACCESSARYENC);
                }
            }
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPLVUP);

            if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
            {
                long afterpow = (long)CharacterManager.Instance.MyActor.stat.GetBattlePower();
                long uppowerval = afterpow - beforepow;
                if (uppowerval != 0)
                {
                    UIManager.Instance.CreatePopMessage(PopMessageType.StatMessage, _popMsg =>
                    {
                        _popMsg.GetComponent<PopStatMessage>().Init(null, afterpow, uppowerval);
                    });
                }
            }
        }
        else
            return;
    }
    public void Still_OnClickReinforceBtn()
    {
        ReinforceBtn.enabled = false;
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        if (selectitem == null)
        {
            ReinforceBtn.enabled = true;
            clicked = false;
            levelupcheck = -1;
            return;
        }
        EquipLevelUpCostData levelupInfo = PopupManager.Instance.EquipLevelUpCostDatas.Where(n => n.equipID == selectitem.Index && n.level == selectitem.level).FirstOrDefault();
        if (levelupInfo == null)
        {
            ReinforceBtn.enabled = true;
            clicked = false;
            levelupcheck = -1;
            return;
        }
        EquipLevelUpCostData CostData = PopupManager.Instance.EquipLevelUpCostDatas.Where(n => n.equipID == selectitem.Index && n.level == selectitem.level).FirstOrDefault();
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
          && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        
        long beforepow = 0;
        if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
            beforepow = (long)CharacterManager.Instance.MyActor.stat.GetBattlePower();
        if (selectitem.level >= refiningdata.maxLevel)
        {
            ReinforceBtn.enabled = true;
            clicked = false;
            if (levelupcheck > 0)
            {
                UserGameData.Get().ModifyEquipItemData(selectitem.Index, selectitem.num, selectitem.refining, selectitem.level, selectitem.equipped);
                levelupcheck = -1;
            }
            return;
        }
        if(CostData==null)
        {
            ReinforceBtn.enabled = true;
            clicked = false;
            if (levelupcheck > 0)
            {
                UserGameData.Get().ModifyEquipItemData(selectitem.Index, selectitem.num, selectitem.refining, selectitem.level, selectitem.equipped);
                levelupcheck = -1;
            }
            return;
        }

        if (0 < ItemSystem.StillLevelUp(SelectItemSlot.EquipDataInfo, CostData.itemTypes, 100f))
        {
            EquipAbilityData equipstatdata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId));
            ItemSystem.CheckEnable(SelectItemSlot.EquipDataInfo.ItemId, out EnableItem enableItem);
            MainItemEffect.RollingNumber(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * enableItem.level)
                                , equipstatdata.MainabilityList[0].LevelValue);
            if (equipstatdata.MainabilityList.Count > 1)
            {
                for (int i = 1; i < equipstatdata.MainabilityList.Count; i++)
                {
                    ItemEffectList[i - 1].gameObject.SetActive(true);

                    if (equipstatdata.MainabilityList[i].LevelValue > 0)
                    {
                        ItemEffectList[i - 1].RollingNumber(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)
                            , equipstatdata.MainabilityList[i].LevelValue);
                    }

                    if (equipstatdata.MainabilityList[i].LevelValue <= 0)
                    {
                        if (equipstatdata.MainabilityList[1].Type != eStatType.atkSpeedBase)
                            ItemEffectList[i - 1].SetStatTypeValue(equipstatdata.MainabilityList[i].Type, equipstatdata.MainabilityList[i].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level));
                        else
                        {
                            string value = (equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[i].LevelValue * enableItem.level)).ToString("F3");
                            ItemEffectList[i - 1].SetStringTypeStringValue(StateSystem.GetStatString(equipstatdata.MainabilityList[i].Type), value);
                        }
                    }
                }
            }
            TryReinforcefun();
            if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.WEAPON)
            {
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.WEAPONLVUP] = true;
                UserManager.Instance.SetEventCount(eQuestType.WEAPONLVUP, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.WEAPONLVUP] = false;
            }
            else if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.SHIELD)
            {
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.SHIELDLVUP] = true;
                UserManager.Instance.SetEventCount(eQuestType.SHIELDLVUP, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.SHIELDLVUP] = false;
            }
            else if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
            {
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.ACCESSARYLVUP] = true;
                UserManager.Instance.SetEventCount(eQuestType.ACCESSARYLVUP, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
                UserManager.Instance.UserGameDataInfo._DicNotifiedEventFlag[eQuestType.ACCESSARYLVUP] = false;
            }
            if (SelectItemSlotInfo != null)
            {
                ITEM_TYPE itemType = SelectItemSlotInfo.itemType;

                if (itemType.Equals(ITEM_TYPE.WEAPON))
                {
                    UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPWEAPONENC);
                }
                else if (itemType.Equals(ITEM_TYPE.SHIELD))
                {
                    UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPSHIELDENC);
                }
                else if (itemType.Equals(ITEM_TYPE.ACCESSARY))
                {
                    UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPACCESSARYENC);
                }
            }
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPLVUP);

            levelupcheck = 1;
        }
        else
        {
            clicked = false;
            ReinforceBtn.enabled = true;
            if (levelupcheck > 0)
            {
                UserGameData.Get().ModifyEquipItemData(selectitem.Index, selectitem.num, selectitem.refining, selectitem.level, selectitem.equipped);
                levelupcheck = -1;
            }
            return;
        }
    }
    public void OnClick_Refining()
    {
        if (SelectItemSlotInfo.itemType.Equals(ITEM_TYPE.ACCESSARY))
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPWRK))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPWRK);
                return;
            }
        }
        else
        {
            if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPREF))
            {
                UISystemOpenPage.Get().LockClickAction(eContentsOpenType.EQUIPREF);
                return;
            }
        }

        if (Detailslot.EquipDataInfo.ItemId < 0)
            return;

        EnableItem _selectItem = UserManager.Instance.UserGameDataInfo.GetEnableItem(Detailslot.EquipDataInfo.ItemId);
        if (_selectItem == null)
            return;

        EquipReforgeData _refiningData = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == _selectItem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();

        if (_selectItem.level < _refiningData.maxLevel)
            return;

        long _beforePow = 0;
        if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
            _beforePow = (long)CharacterManager.Instance.MyActor.stat.GetBattlePower();

        ItemSystem.RefiningUp(Detailslot.EquipDataInfo);

        if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
        {
            long _afterPow = (long)CharacterManager.Instance.MyActor.stat.GetBattlePower();
            long _upPowerVal = _afterPow - _beforePow;
            if (_upPowerVal != 0)
            {
                UIManager.Instance.CreatePopMessage(PopMessageType.StatMessage, _popMsg =>
                {
                    _popMsg.GetComponent<PopStatMessage>().Init(null, _afterPow, _upPowerVal);
                });
            }
        }

        if (Detailslot.EquipDataInfo.itemType == ITEM_TYPE.WEAPON || Detailslot.EquipDataInfo.itemType == ITEM_TYPE.SHIELD)
        {
            UserManager.Instance.SetQuestCount(eQuestType.EQUIPREFOREGE);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPREFOREGE);
        }
        else if (Detailslot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
        {
            UserManager.Instance.SetQuestCount(eQuestType.EQUIPWORKMANSHIP);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPWORKMANSHIP);
        }

        ITEM_TYPE equipItemType = Detailslot.EquipDataInfo.itemType;

        if (equipItemType == ITEM_TYPE.WEAPON)
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONREFORGE] = true;
            UserManager.Instance.SetEventCount(eQuestType.WEAPONREFORGE, 1, (int)Detailslot.EquipDataInfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONREFORGE] = false;

            UserManager.Instance.SetQuestCount(eQuestType.EQUIPREFOREGE);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPREFOREGE);
        }
        else if (equipItemType == ITEM_TYPE.SHIELD)
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDREFORGE] = true;
            UserManager.Instance.SetEventCount(eQuestType.SHIELDREFORGE, 1, (int)Detailslot.EquipDataInfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDREFORGE] = false;

            UserManager.Instance.SetQuestCount(eQuestType.EQUIPREFOREGE);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPREFOREGE);
        }
        else if (equipItemType == ITEM_TYPE.ACCESSARY)
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYREFORGE] = true;

            UserManager.Instance.SetEventCount(eQuestType.ACCESSARYREFORGE);

            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYREFORGE] = false;

            UserManager.Instance.SetQuestCount(eQuestType.EQUIPWORKMANSHIP);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPWORKMANSHIP);
        }

        _selectItem.isRefining = UserManager.Instance.UserGameDataInfo.GetCanRefiningEquip(_selectItem);

        StartCoroutine(Detailslot.GradeStarAnimation(_selectItem.refining));

        EffectBase _effRefining = UIManager.Instance.CreateEffect(PopupManager.Instance.fxItemRefiningSuccess);
        _effRefining.transform.parent = EffectPos;
        _effRefining.transform.localPosition = Vector3.zero;
        _effRefining.transform.localScale = Vector3.one * 0.9f;
        _effRefining.SetLifeTime(1.5f);

        RefleshUI();

        EquipAbilityData _equipStatData = PopupManager.Instance.EquipAbilityDatas.Find(x => x.Index == Detailslot.EquipDataInfo.ItemId);
        for (int i = 0; i < _equipStatData.AbilityTypes.Count; i++)
        {
            if (ItemEquipEffectList[i].gameObject.activeSelf)
            {
                ItemEquipEffectList[i].RollingNumber(_equipStatData.AbilityTypes[i].Type, (_equipStatData.AbilityTypes[i].Value + _equipStatData.AbilityTypes[i].LevelValue * (_selectItem.refining - 1))
                    , _equipStatData.AbilityTypes[i].LevelValue);
            }
        }
    }
    public void LockEnchantBtn()
    {
        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPENC))
        {
            enchantLockBtn.SetActive(false);
            if (EnchantBtnCover.activeSelf)
            {
                EnchantBtn.SetActive(false);
                EnchantBtnCover.SetActive(true);
            }
            else
            {
                EnchantBtn.SetActive(true);
                EnchantBtnCover.SetActive(false);
            }
        }
        else
        {
            enchantLockBtn.SetActive(true);
            EnchantBtn.SetActive(false);
            EnchantBtnCover.SetActive(false);
        }
    }
    public void LockRefiningBtn(EquipInfoData equipInfo = null)
    {
        if (equipInfo == null)
        {
            return;
        }

        if (equipInfo.itemType.Equals(ITEM_TYPE.ACCESSARY))
        {
            if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPWRK))
            {
                refiningLockBtn.SetActive(false);
                if (RefiningBtnCover.activeSelf)
                {
                    RefiningBtn.SetActive(false);
                    RefiningBtnCover.SetActive(true);
                }
                else
                {
                    RefiningBtn.SetActive(true);
                    RefiningBtnCover.SetActive(false);
                }
            }
            else
            {
                RefiningBtn.SetActive(false);
                RefiningBtnCover.SetActive(false);
                refiningLockBtn.SetActive(true);
            }
        }
        else
        {
            if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPREF))
            {
                refiningLockBtn.SetActive(false);
                if (RefiningBtnCover.activeSelf)
                {
                    RefiningBtn.SetActive(false);
                    RefiningBtnCover.SetActive(true);
                }
                else
                {
                    RefiningBtn.SetActive(true);
                    RefiningBtnCover.SetActive(false);
                }
            }
            else
            {
                RefiningBtn.SetActive(false);
                RefiningBtnCover.SetActive(false);
                refiningLockBtn.SetActive(true);
            }
        }
    }
    public void LockUpgradeBtn(EquipInfoData equipInfo = null)
    {
        if (equipInfo == null)
        {
            return;
        }

        if (equipInfo.Grade.Equals(ITEM_GRADE.ANCIENT))
        {
            advancementLockBtn.SetActive(false);
            if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPUPG))
            {
                if (AdvancementBtnCover.activeSelf)
                {
                    AdvancementBtnCover.SetActive(true);
                    AdvancementBtn.SetActive(false);
                }
                else
                {
                    AdvancementBtnCover.SetActive(false);
                    AdvancementBtn.SetActive(true);
                }
            }
            else
            {
                advancementLockBtn.SetActive(true);
                AdvancementBtnCover.SetActive(false);
                AdvancementBtn.SetActive(false);
            }

            composeLockBtn.SetActive(false);
            UpGardeBtn.SetActive(false);
            UpGardeBtnCover.SetActive(false);
        }
    }
    public void LockComposeBtn(EquipInfoData equipInfo = null)
    {
        if (equipInfo == null)
        {
            return;
        }

        if(equipInfo.Grade.Equals(ITEM_GRADE.MYTH))
        {
            if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPAWK))
            {
                composeLockBtn.SetActive(false);
                if (UpGardeBtnCover.activeSelf)
                {
                    UpGardeBtnCover.SetActive(true);
                    UpGardeBtn.SetActive(false);
                }
                else
                {
                    UpGardeBtnCover.SetActive(false);
                    UpGardeBtn.SetActive(true);
                }
            }
            else
            {
                composeLockBtn.SetActive(true);
                UpGardeBtn.SetActive(false);
                UpGardeBtnCover.SetActive(false);
            }
        }
        else if (equipInfo.Grade.Equals(ITEM_GRADE.ANCIENT) == false)
        {
            if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPCOM))
            {
                composeLockBtn.SetActive(false);
                if (UpGardeBtnCover.activeSelf)
                {
                    UpGardeBtnCover.SetActive(true);
                    UpGardeBtn.SetActive(false);
                }
                else
                {
                    UpGardeBtnCover.SetActive(false);
                    UpGardeBtn.SetActive(true);
                }
            }
            else
            {
                composeLockBtn.SetActive(true);
                UpGardeBtn.SetActive(false);
                UpGardeBtnCover.SetActive(false);
            }

            advancementLockBtn.SetActive(false);
            AdvancementBtnCover.SetActive(false);
            AdvancementBtn.SetActive(false);
        }
    }
    
    public void TryReinforcefun()
    {
        EffectBase _effUpgrade = UIManager.Instance.CreateEffect(PopupManager.Instance.fxItemReinforceSuccess);
        _effUpgrade.transform.parent = EffectPos;
        _effUpgrade.transform.localPosition = Vector3.zero;
        _effUpgrade.transform.localScale = EffectPos.localScale; 
        _effUpgrade.SetLifeTime(0.3f);
        ReinforceBtn.enabled = true;
        RefleshUI();
    }
    IEnumerator TryReinforce()
    {
        EffectBase _effUpgrade = UIManager.Instance.CreateEffect(PopupManager.Instance.fxItemReinforceSuccess);
        _effUpgrade.transform.parent = EffectPos;
        _effUpgrade.transform.localPosition = Vector3.zero;
        _effUpgrade.transform.localScale = EffectPos.localScale; 
        _effUpgrade.SetLifeTime(0.2f);
        ReinforceBtn.enabled = true;
        RefleshUI();
        yield break;
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        clicked = true;
        startTime = Time.time + sequenceTime;
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        clicked = false;

        if (levelupcheck > 0)
        {
            EnableItem item = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
            if (item != null)
            {
                UserGameData.Get().ModifyEquipItemData(item.Index, item.num, item.refining, item.level, item.equipped);
                levelupcheck = -1;
            }
            else
            {

            }
        }
    }

    public void ButtonEnable()
    {
        clicked = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
            if (startTime < Time.time)
            {
                if (clickTime < Time.time)
                {
                    clickTime = Time.time + delayTime;
                    Still_OnClickReinforceBtn();
                }
            }
        }
        if (UpdateDirty)
        {
            RefleshUI();
            UpdateDirty = false;
        }
        else
            return;
    }
    public List<EquipPromotionData> GetPromotionDataList(ITEM_TYPE itemtype, int qualityIndex)
    {
        List<EquipPromotionData> list = new List<EquipPromotionData>();
        foreach (EquipPromotionData _info in PopupManager.Instance.EquipPromotionDatas)
        {
            if (_info.equipType == itemtype && _info.qualityIndex == qualityIndex)
            {
                list.Add(_info);
            }
        }
        list = list.OrderBy(x => x.Index).ToList();
        return list;
    }
}
