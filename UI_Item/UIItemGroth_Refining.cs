using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AwakeRefining
{
    public UIItemSlot ItemSlot;
    public Image CostImage;
    public List<UIEffectText> EffectTextList;
    public Text MaxLevelUptext;
    public Text NeedCostText;
    Image GradeIcon;
    public Text ItemTypeText;
    public Color LockEffectTextColor;
    public Color AddEffectTextColor;
    public Color NormalEffectTextColor;
    public GameObject AwakeRefiningObj;
    public Image Skillicon;
    public Text SkillName;
    public Text NowSkilllevel;
    public Text NextSkilllevel;
    public Text NowSkillEffect;
    public Text NextSkillEffect;
    public List<ContentSizeFitter> sizefitterlist;
    public void UpdataSizeFitter()
    {
        for (int i = 0; i < sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sizefitterlist[i].transform);
        }

    }
}
//제련
public class UIItemGroth_Refining : UIItemGrowthBase
{
    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.REFINING; }

    //재련관련
    [SerializeField] UIItemSlot ItemSlot;
    [SerializeField] List<Image> CostImageList;
    [SerializeField] List<UIEffectText> EffectTextList;             //5성이하
    [SerializeField] List<UIEffectText> AddEffectTextList;          //5성이하
    [SerializeField] List<UIEffectText> OverEffectTextList;         //5성이상
    [SerializeField] List<UIEffectText> AddOverEffectTextList;      //5성이상
    [SerializeField] List<Text> LockTextList;
    [SerializeField] Text MaxLevelUptext;
    [SerializeField] GameObject Bottomobj;
    [SerializeField] Text NeedCostText;
    [SerializeField] Text UseUserCostText;
    [SerializeField] Text RefiningBtnText;
    [SerializeField] Text RefiningBtnCoverText;
    [SerializeField] List<Text> ItemTypeText;
    [SerializeField] Text NowMaxLevelText;
    [SerializeField] Text NextMaxLevelText;
    [SerializeField] Color CantRefiningTextColor;
    [SerializeField] Color CanRefiningTextColor;
    [SerializeField] GameObject BtnCover;
    [SerializeField] GameObject NormalRefiningObj;
    [SerializeField] Image GradeIcon;
    [SerializeField] AwakeRefining _awakeRefining;
    [SerializeField] GameObject BackObj;
    [SerializeField] GameObject[] EffectTextObjs;
    [SerializeField] Text UserHaveCount;
    private bool UpdateDirtyInfo = false;

    [Header("툴팁용 필드 데이터")]
    [SerializeField] private RectTransform costImageRectTransform;

    public override void OpenPanel(UIItemSlot _selectItemSlot)
    {
        base.OpenPanel(_selectItemSlot);
        Bottomobj.SetActive(true);
        if (_selectItemSlot != null)
        {
            for (int i = 0; i < ItemTypeText.Count; i++)
            {
                ItemTypeText[i].text = ItemSystem.GetItemSubTypeEffectTitelText(_selectItemSlot.EquipDataInfo.itemType, _selectItemSlot.EquipDataInfo.qualityIndex);
            }
            GradeIcon.sprite = ItemSystem.GetGradeiconSprite((_selectItemSlot.EquipDataInfo.Grade));
            BackObj.SetActive(true);

           
            RefreshUI();
        }
    }

    private void Update()
    {
        if (UpdateDirtyInfo == true)
        {
            RefreshUI();
            UpdateDirtyInfo = false;
        }
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
        Bottomobj.SetActive(false);
    }
    public void RefreshUI()
    {
        if (SelectItemSlot == null)
            return;
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        
        EquipAbilityData abilitydata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId));
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData nextrefiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
          && x.reforge == selectitem.refining+1 && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                            where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade
                                            && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                            orderby v.maxLevel descending
                                            select v).FirstOrDefault();
        ItemInfoData iInfo = null;

        ItemSlot.SetData(SelectItemSlot.EquipDataInfo, null);
        if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
        {
            RefiningBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_JEWELRY");
            RefiningBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_JEWELRY");
        }
        else
        {
            RefiningBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
            RefiningBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
        }
        if (refiningdata.Index > 0)
        {
            iInfo = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==refiningdata.reforgeItem[0].itemID).FirstOrDefault();
            for (int i = 0; i < CostImageList.Count; i++)
            {
                if (iInfo != null)
                    CostImageList[i].sprite = ItemSystem.GetItemSprite(iInfo.icon);
            }
            if (iInfo != null)
            {
                ItemClass costitem = UserGameData.Get().GetItemClass(iInfo.Index);
                if (costitem == null || refiningdata.reforgeItem[0].itemValue > costitem.num)
                {
                    NeedCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.color = CantRefiningTextColor;
                    if (costitem != null)
                    {
                        UseUserCostText.text = costitem.num.ToString();
                        UseUserCostText.color = CantRefiningTextColor;
                        UserHaveCount.text= costitem.num.ToString();
                    }
                    else
                    {
                        UseUserCostText.text = "0";
                        UseUserCostText.color = CantRefiningTextColor;
                        UserHaveCount.text = "0";
                    }
                }
                else
                {
                    NeedCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.color = CanRefiningTextColor;
                    UserHaveCount.text = costitem.num.ToString();
                }
            }
        }
        else
        {
            iInfo = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == maxrefiningdata.reforgeItem[0].itemID).FirstOrDefault();
            if (iInfo != null)
            {
                for (int i = 0; i < CostImageList.Count; i++)
                {
                    CostImageList[i].sprite = ItemSystem.GetItemSprite(iInfo.icon);
                }
                ItemClass costitem = UserGameData.Get().GetItemClass(iInfo.Index);
                if (costitem != null)
                {
                    UseUserCostText.text = costitem.num.ToString();
                    UseUserCostText.color = CanRefiningTextColor;
                    UserHaveCount.text = costitem.num.ToString();
                }
                else
                {
                    UseUserCostText.text = "0";
                    UseUserCostText.color = CantRefiningTextColor;
                    UserHaveCount.text = "0";
                }
            }
        }
        BtnCover.SetActive(false);
        if (SelectItemSlot.EquipDataInfo.Grade == ITEM_GRADE.ANCIENT)
        {
            if (refiningdata.Index > 0)
            {
                if (maxrefiningdata.maxLevel <= selectitem.level)
                {
                    BtnCover.SetActive(true);
                }
                else
                {
                    if (refiningdata.maxLevel > selectitem.level)
                    {
                        BtnCover.SetActive(true);
                    }
                }
            }
            
            for (int i = 0; i < EffectTextObjs.Length; i++)
            {
                EffectTextObjs[i].SetActive(false);
            }
            bool isoverrefing = true;
            EnableItem item = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
            if (item != null)
            {
                isoverrefing = true;
                for (int i = 0; i < abilitydata.AbilityTypes.Count; i++)
                {

                    isoverrefing = false;
                    break;

                }
                if (item.promotionTier > 0)
                {
                    EquipPromotionData nowequippromotion = PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == SelectItemSlot.EquipDataInfo.itemType
                && n.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex && n.promotionTier == item.promotionTier).FirstOrDefault();
                    if (nowequippromotion != null)
                    {
                        if (item.refining == nowequippromotion.maxReforge)
                        {
                            BtnCover.SetActive(true);
                            RefiningBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_SKILL_018");
                        }
                        else
                        {
                            ItemSlot.ReInit(item.refining + 1);
                        }
                    }
                }
                else
                {
                    EquipPromotionData nowequippromotion = PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == SelectItemSlot.EquipDataInfo.itemType
                && n.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex && n.promotionTier == 1).FirstOrDefault();
                    if (nowequippromotion != null)
                    {
                        if (item.refining == nowequippromotion.termsReforge)
                        {
                            BtnCover.SetActive(true);
                            RefiningBtnCoverText.text = LocalizeManager.Instance.GetTXT("STR_UI_SKILL_018");
                        }
                        else
                        {
                            ItemSlot.ReInit(item.refining + 1);
                        }
                    }
                }
            }
            else
                isoverrefing = false;
            if (!isoverrefing)
            {
                EffectTextObjs[0].SetActive(true);
                SetNomalRefining();
            }
            else
            {
                EffectTextObjs[1].SetActive(true);
                SetOverRefining();
            }

            NowMaxLevelText.text = refiningdata.maxLevel.ToString();
            if (nextrefiningdata.Index > 0)
                NextMaxLevelText.text = nextrefiningdata.maxLevel.ToString();

            for (int i = 0; i < refiningdata.reforgeItem.Count; i++)
            {
                if (UserGameData.Get().CheckNickNameEmpty())
                    return;
                ItemClass costitem = UserGameData.Get().GetItemClass(refiningdata.reforgeItem[i].itemID);

                if (costitem == null)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = "0";
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else if (costitem.num < refiningdata.reforgeItem[i].itemValue)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = costitem.num.ToString();
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else
                {
                    UseUserCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.color = CanRefiningTextColor;
                }
            }
            if (selectitem.num < refiningdata.needSelfValue)
                BtnCover.SetActive(true);

        }
        else
        {
            _awakeRefining.AwakeRefiningObj.SetActive(false);
            NormalRefiningObj.SetActive(true);
            for (int i = 0; i < EffectTextObjs.Length; i++)
            {
                EffectTextObjs[i].SetActive(false);
            }
            if (selectitem == null || selectitem.refining < 5)
            {
                EffectTextObjs[0].SetActive(true);
                if (selectitem != null)
                {
                    if (nextrefiningdata != null && nextrefiningdata.Index > 0)
                    {
                        ItemSlot.ReInit(selectitem.refining + 1);
                    }
                }
            }
            
            SetNomalRefining();
        }
    }
    void SetNomalRefining()
    {
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        EquipAbilityData abilitydata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId)); 
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData nextrefiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining +1&& x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                            where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade
                                            && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                            orderby v.maxLevel descending
                                            select v).FirstOrDefault();

        _awakeRefining.AwakeRefiningObj.SetActive(false);
        NormalRefiningObj.SetActive(true);
        if (refiningdata.Index > 0)
        {
            if (maxrefiningdata.maxLevel <= selectitem.level)
            {
                BtnCover.SetActive(true);
            }
            else
            {
                if (refiningdata.maxLevel > selectitem.level)
                {

                    RefiningBtnText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_043"), refiningdata.maxLevel);
                    RefiningBtnText.color = CantRefiningTextColor;
                }
                else
                {
                    RefiningBtnText.color = CanRefiningTextColor;
                    if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
                    {
                        RefiningBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_JEWELRY");
                    }
                    else
                    {
                        RefiningBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
                    }
                }
            }
            NowMaxLevelText.text = refiningdata.maxLevel.ToString();
            if (nextrefiningdata.Index > 0)
                NextMaxLevelText.text = nextrefiningdata.maxLevel.ToString();
            else
            {
                NextMaxLevelText.text = refiningdata.maxLevel.ToString();
            }

            if (UserGameData.Get().CheckNickNameEmpty())
                return;
            for (int i = 0; i < refiningdata.reforgeItem.Count; i++)
            {
                ItemClass costitem = UserGameData.Get().GetItemClass(refiningdata.reforgeItem[i].itemID);
                if (costitem == null)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = "0";
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else if (costitem.num < refiningdata.reforgeItem[i].itemValue)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = costitem.num.ToString();
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else
                {
                    UseUserCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.color = CanRefiningTextColor;
                }
                if (selectitem.num < refiningdata.needSelfValue)
                    BtnCover.SetActive(true);
            }
        }
        else
        {
            NowMaxLevelText.text = maxrefiningdata.maxLevel.ToString();
            NextMaxLevelText.text = maxrefiningdata.maxLevel.ToString();
            if (maxrefiningdata.maxLevel <= selectitem.level)
            {
                BtnCover.SetActive(true);
            }
            else
            {
                if (refiningdata.maxLevel != selectitem.level)
                {
                    BtnCover.SetActive(true);
                }
            }
            for (int i = 0; i < maxrefiningdata.reforgeItem.Count; i++)
            {
                if (UserGameData.Get().CheckNickNameEmpty())
                    return;
                ItemClass costitem = UserGameData.Get().GetItemClass(maxrefiningdata.reforgeItem[i].itemID);
                if (costitem == null)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = "0";
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else if (costitem.num < maxrefiningdata.reforgeItem[i].itemValue)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = costitem.num.ToString();
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else
                {
                    UseUserCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.color = CanRefiningTextColor;
                }
            }
            if (selectitem.num < maxrefiningdata.needSelfValue)
                BtnCover.SetActive(true);
        }
        for (int i = 0; i < EffectTextList.Count; i++)
        {
            EffectTextList[i].gameObject.SetActive(false);
            EffectTextList[i].LockObj.SetActive(true);
            EffectTextList[i].NewAddObj.SetActive(false);
        }
        if (selectitem.refining < 5)
        {
            for (int i = 0; i < abilitydata.AbilityTypes.Count; i++)
            {

                EffectTextList[i].SetStatTypeValue(abilitydata.AbilityTypes[i].Type, (abilitydata.AbilityTypes[i].LevelValue));

                AddEffectTextList[i].SetStatTypeValue(abilitydata.AbilityTypes[i].Type, abilitydata.AbilityTypes[i].Value);
                EffectTextList[i].LockObj.SetActive(false);
                EffectTextList[i].NewAddObj.SetActive(false);
                EffectTextList[i].gameObject.SetActive(true);


            }
        }
        else
        {
            for (int i = 0; i < EffectTextList.Count; i++)
            {
                EffectTextList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < abilitydata.AbilityTypes.Count; i++)

            {
                EffectTextList[i].SetStatTypeValue(abilitydata.AbilityTypes[i].Type, (abilitydata.AbilityTypes[i].LevelValue));
                AddEffectTextList[i].SetStatTypeValue(abilitydata.AbilityTypes[i].Type, abilitydata.AbilityTypes[i].Value);
                EffectTextList[i].LockObj.SetActive(false);
                EffectTextList[i].NewAddObj.SetActive(false);
                EffectTextList[i].gameObject.SetActive(true);
            }
        }
        if (abilitydata.AbilityTypes.Count > 0)
        {
            for (int i = 0; i < ItemTypeText.Count; i++)
            {
                ItemTypeText[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < ItemTypeText.Count; i++)
            {
                ItemTypeText[i].gameObject.SetActive(false);
            }

        }
    }

    void SetOverRefining()
    {
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        EquipAbilityData abilitydata =PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId));
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData nextrefiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining+1 && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                            where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade
                                            && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                            orderby v.maxLevel descending
                                            select v).FirstOrDefault();

        _awakeRefining.AwakeRefiningObj.SetActive(true);
        NormalRefiningObj.SetActive(false);
        if (refiningdata.Index > 0)
        {
            if (maxrefiningdata.maxLevel <= selectitem.level)
            {
                BtnCover.SetActive(true);
            }
            else
            {
                if (refiningdata.maxLevel > selectitem.level)
                {

                    RefiningBtnText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_043"), refiningdata.maxLevel);
                    RefiningBtnText.color = CantRefiningTextColor;
                }
                else
                {
                    RefiningBtnText.color = CanRefiningTextColor;
                    if (SelectItemSlot.EquipDataInfo.itemType == ITEM_TYPE.ACCESSARY)
                    {
                        RefiningBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ACCESSARY_JEWELRY");
                    }
                    else
                    {
                        RefiningBtnText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE");
                    }
                }
            }
            NowMaxLevelText.text = refiningdata.maxLevel.ToString();
            if (nextrefiningdata.Index > 0)
                NextMaxLevelText.text = nextrefiningdata.maxLevel.ToString();
            else
            {
                NextMaxLevelText.text = refiningdata.maxLevel.ToString();
            }

            if (UserGameData.Get().CheckNickNameEmpty())
                return;
            for (int i = 0; i < refiningdata.reforgeItem.Count; i++)
            {
                ItemClass costitem = UserGameData.Get().GetItemClass(refiningdata.reforgeItem[i].itemID);
                if (costitem == null)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = "0";
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else if (costitem.num < refiningdata.reforgeItem[i].itemValue)
                {
                    BtnCover.SetActive(true);
                    UseUserCostText.text = costitem.num.ToString();
                    UseUserCostText.color = CantRefiningTextColor;
                }
                else
                {
                    UseUserCostText.text = refiningdata.reforgeItem[0].itemValue.ToString();
                    UseUserCostText.color = CanRefiningTextColor;
                }
                if (selectitem.num < refiningdata.needSelfValue)
                    BtnCover.SetActive(true);
            }
        }
        else
        {
            NowMaxLevelText.text = maxrefiningdata.maxLevel.ToString();
            NextMaxLevelText.text = maxrefiningdata.maxLevel.ToString();
            if (maxrefiningdata.maxLevel <= selectitem.level)
            {
                BtnCover.SetActive(true);
            }
            else
            {
                if (refiningdata.maxLevel != selectitem.level)
                {
                    BtnCover.SetActive(true);
                }
            }
            
            if (selectitem.num < maxrefiningdata.needSelfValue)
                BtnCover.SetActive(true);
        }
        for (int i = 0; i < EffectTextList.Count; i++)
        {
            OverEffectTextList[i].gameObject.SetActive(false);
            OverEffectTextList[i].NewAddObj.SetActive(false);
        }

        for (int i = 0; i < EffectTextList.Count; i++)
        {
            OverEffectTextList[i].gameObject.SetActive(false);
        }

        List<int> abilityindexlist = new List<int>();
        List<float> AddEffectList = new List<float>();
        for (int i = 0; i < abilitydata.AbilityTypes.Count; i++)
        {

            AddEffectList.Add(abilitydata.AbilityTypes[i].Value);

        }
        for (int i = 0; i < abilityindexlist.Count; i++)
        {
            AddEffectList[abilityindexlist[i]] += abilitydata.AbilityTypes[abilityindexlist[i]].LevelValue * (selectitem.refining - 1);
        }
        for (int i = 0; i < abilitydata.AbilityTypes.Count; i++)
        {
            OverEffectTextList[i].SetStatTypeValue(abilitydata.AbilityTypes[i].Type, AddEffectList[i]);
            OverEffectTextList[i].gameObject.SetActive(true);
        }

    }
    public void OnClick_Refining()
    {
        if (SelectItemSlot.EquipDataInfo.ItemId < 0)
            return;

        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        if (selectitem == null)
            return;
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        
        EquipReforgeData nextrefiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
            && x.reforge == selectitem.refining +1&& x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        
        EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                            where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade
                                            && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                            orderby v.maxLevel descending
                                            select v).FirstOrDefault();
        
        if (selectitem.level < refiningdata.maxLevel)
            return;
        if (selectitem.refining == maxrefiningdata.reforge)
            return;
        BtnCover.SetActive(true);
        int isUp = ItemSystem.RefiningUp(SelectItemSlot.EquipDataInfo);
        
        _owner.ItemGrowthPopup.OpenRefiningResult();
        RefreshUI();

        BtnCover.SetActive(false);
    }
    public void OnClick_ShowTooltip()
    {
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        ItemInfoData iInfo = null;

        if (refiningdata.Index > 0)
        {
            iInfo = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==refiningdata.reforgeItem[0].itemID).FirstOrDefault();
            if (iInfo != null)
            {
                if (PopupManager.Instance.IsPopupCheck(PopupType.PopupItemTooltip) == false)
                {
                    PopupManager.Instance.CreatePopup(PopupType.PopupItemTooltip, true, _popup =>
                    {
                        PopupItemTooltip _tooltip = _popup.GetComponent<PopupItemTooltip>();
                        _tooltip.Init(null);
                        _tooltip.OnClick_OpenTooltip(costImageRectTransform, iInfo);
                    });
                }
            }
        }
    }
}
