using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemGrowth_Advancement : UIItemGrowthBase
{
    
    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.REINFORCE; }
    [SerializeField] GameObject Bottomobj;
    [SerializeField] GameObject BackObj;
    [SerializeField] GameObject BtnCoverObj;
    [SerializeField] PopupItemBox itemslot;

    [SerializeField] Image ItemImage;
    [SerializeField] Image GradeImage;
    [SerializeField] Image GradeIcon;
    [SerializeField] List<Image> starlist;
    [SerializeField] List<Image> disstarlist;
    [SerializeField] ScrollRect NeedItemScroll;

    [SerializeField] Image NowGradeStar;
    [SerializeField] Image NextGradeStar;
    [SerializeField] Text NowMaxGrade;
    [SerializeField] Text NextMaxGrade;

    [SerializeField] GameObject SkillInfoObj;
    [SerializeField] Image SkillImage;
    [SerializeField] Text SkillName;
    [SerializeField] Text NowSkillLevel;
    [SerializeField] Text NextSkillLevel;
    [SerializeField] Text SkillEffect;
    [SerializeField] List<ContentSizeFitter> sizefitterlist;
    [SerializeField] List<Image> BeforeStarImage;
    [SerializeField] List<Image> AfterStarImage;
    [SerializeField] GameObject Romobj;
    [SerializeField] Image Romnum;

    EquipPromotionData Promotioninfo = null;
    List<PopupItemBox> slotlist;

    public override void OpenPanel(UIItemSlot _selectItemSlot)
    {
        base.OpenPanel(_selectItemSlot);
        BackObj.SetActive(true);
        Bottomobj.SetActive(true);
        if (slotlist == null)
            slotlist = new List<PopupItemBox>();
        if (SelectItemSlot != null)
        {
            if (SelectItemSlot.EquipDataInfo.Grade < ITEM_GRADE.ANCIENT)
                return;
            EnableItem item = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
            if (item == null)
                return;
            CommonData starimagedata = DataManager.Instance.CommonDatas.Find(x => x.DataType == "starsGradeImage");
            CommonData disstarimagedata = DataManager.Instance.CommonDatas.Find(x => x.DataType == "starsGradeOffImage");
            if (item.promotionTier > 0)
            {
                Promotioninfo = PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == SelectItemSlot.EquipDataInfo.itemType
                && n.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex && n.promotionTier == item.promotionTier+1).FirstOrDefault();
                if (item.refining < Promotioninfo.termsReforge)
                {
                    BtnCoverObj.SetActive(true);
                    return;
                }
            }
            else
            {
                Promotioninfo = PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == SelectItemSlot.EquipDataInfo.itemType
                 && n.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex && n.promotionTier == 1).FirstOrDefault();
                if (item.refining < Promotioninfo.termsReforge)
                {
                    BtnCoverObj.SetActive(true);
                    Debug.Log("여기1");
                    return;
                }
            }
            BtnCoverObj.SetActive(false);
            if (Romobj != null)
            {
                Romobj.SetActive(true);
                string spritename = string.Format("Icon_Roman_Item_No_{0:D2}", SelectItemSlot.EquipDataInfo.gradeIndex);
                Sprite check = Resources.Load<Sprite>(StringConst.Common_SPRITE_PATH + spritename);
                if (check == null)//고대등급예외처리
                    check = Resources.Load<Sprite>(StringConst.Common_SPRITE_PATH + "Icon_Roman_Item_No__01");
                Romnum.sprite = check;
            }
            RefreshUI();
        }
    }
    public override void ClosePanel()
    {
        base.ClosePanel();
        BackObj.SetActive(false);
        Bottomobj.SetActive(false);
        if (slotlist == null)
            slotlist = new List<PopupItemBox>();
        for(int i = 0; i < slotlist.Count; i++)
        {
            slotlist[i].gameObject.SetActive(false);
        }
    }
    public void RefreshUI()
    {
        if (SelectItemSlot == null)
            return;
        EnableItem item = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        if (item == null)
            return;
        ItemImage.sprite=ItemSystem.GetItemSprite(SelectItemSlot.EquipDataInfo.icon);
        GradeImage.sprite = ItemSystem.GetGradeSprite(SelectItemSlot.EquipDataInfo.Grade);
        GradeIcon.sprite = ItemSystem.GetGradeiconSprite(SelectItemSlot.EquipDataInfo.Grade); 
        for (int i = 0; i < starlist.Count; i++)
        {
            starlist[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < disstarlist.Count; i++)
        {
            disstarlist[i].gameObject.SetActive(false);
        }
        for (int i=0;i< BeforeStarImage.Count; i++)
        {
            BeforeStarImage[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < AfterStarImage.Count; i++)
        {
            AfterStarImage[i].gameObject.SetActive(false);
        }
        if (slotlist == null)
            slotlist = new List<PopupItemBox>();
        for (int i=0;i< slotlist.Count; i++)
        {
            slotlist[i].gameObject.SetActive(false);
        }
        CommonData starimagedata = DataManager.Instance.CommonDatas.Find(x => x.DataType == "starsGradeImage");
        CommonData disstarimagedata = DataManager.Instance.CommonDatas.Find(x => x.DataType == "starsGradeOffImage");
        int count = 0;
        if (Promotioninfo != null)
        {
            if (Promotioninfo.promotionTier == 1)
            {
                for (int i = 0; i < Promotioninfo.maxReforge-5; i++)
                {
                    disstarlist[i].sprite = ItemSystem.GetItemSprite(disstarimagedata.DataValues[1]);
                    disstarlist[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < item.refining-5; i++)
                {
                    starlist[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                    starlist[i].gameObject.SetActive(true);
                    count++;
                }
                for (int i = count; i < Promotioninfo.maxReforge-5; i++)
                {
                    disstarlist[i].sprite = ItemSystem.GetItemSprite(disstarimagedata.DataValues[1]);
                    disstarlist[i].gameObject.SetActive(true);
                }
            }
            EquipReforgeData nowreforge = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
            && x.reforge == item.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();

            EquipReforgeData NextMaxreforge = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
            && x.reforge == Promotioninfo.maxReforge && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();

            if (item.promotionTier > 0)
            {
                EquipPromotionData NowPromotioninfo =PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == SelectItemSlot.EquipDataInfo.itemType 
                && n.qualityIndex == SelectItemSlot.EquipDataInfo.qualityIndex && n.promotionTier == item.promotionTier).FirstOrDefault();
                if (NowPromotioninfo.maxReforge > 5)
                {
                    NowGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                    for (int i = 0; i < NowPromotioninfo.maxReforge-5; i++)
                    {
                        if (i >= BeforeStarImage.Count)
                            continue;
                        BeforeStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                        BeforeStarImage[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    NowGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                    for (int i = 0; i < NowPromotioninfo.maxReforge; i++)
                    {
                        if (i >= BeforeStarImage.Count)
                            continue;
                        BeforeStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                        BeforeStarImage[i].gameObject.SetActive(true);
                    }
                }
                if (Promotioninfo.maxReforge > 5)
                {
                    NextGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                    for (int i = 0; i < Promotioninfo.maxReforge-5; i++)
                    {
                        if (i >= AfterStarImage.Count)
                            continue;
                        AfterStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                        AfterStarImage[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    NextGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                    for (int i = 0; i < Promotioninfo.maxReforge; i++)
                    {
                        if (i >= AfterStarImage.Count)
                            continue;
                        AfterStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                        AfterStarImage[i].gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (Promotioninfo.termsReforge > 5)
                {
                    NowGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);

                    for (int i = 0; i < Promotioninfo.termsReforge - 5; i++)
                    {
                        if (i >= BeforeStarImage.Count)
                            continue;
                        BeforeStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                        BeforeStarImage[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    NowGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                    for (int i = 0; i < Promotioninfo.termsReforge; i++)
                    {
                        if (i >= BeforeStarImage.Count)
                            continue;
                        BeforeStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                        BeforeStarImage[i].gameObject.SetActive(true);
                    }
                }
                if (Promotioninfo.maxReforge > 5)
                {
                    NextGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                    for (int i = 0; i < Promotioninfo.maxReforge-5; i++)
                    {
                        if (i >= AfterStarImage.Count)
                            continue;
                        AfterStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                        AfterStarImage[i].gameObject.SetActive(true);
                    }

                }
                else
                {
                    NextGradeStar.sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[0]);
                    for (int i = 0; i < Promotioninfo.maxReforge; i++)
                    {
                        if (i >= BeforeStarImage.Count)
                            continue;
                        AfterStarImage[i].sprite = ItemSystem.GetItemSprite(starimagedata.DataValues[1]);
                        AfterStarImage[i].gameObject.SetActive(true);
                    }
                }
            }
            if (Promotioninfo.needSelf > 0)
            {
                AddItemSlot(SelectItemSlot.EquipDataInfo, Promotioninfo.needSelf);
            }
            for(int i=0;i< Promotioninfo.CostItems.Count; i++)
            {
                ItemInfoData iteminfo = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == Promotioninfo.CostItems[i].itemID).FirstOrDefault();
                AddItemSlot(iteminfo, Promotioninfo.CostItems[i].itemValue);
            }
        }
        if (SelectItemSlot.EquipDataInfo.skillinfoID > 0)
        {
            SkillInfoObj.SetActive(true);
            SkillInfoData skillinfo = SkillManager.Instance.GetSkillData(SelectItemSlot.EquipDataInfo.skillinfoID);
            SkillImage.sprite = SkillSystem.GetSkillSprite(skillinfo.skillIcon);
            SkillName.text = LocalizeManager.Instance.GetTXT(skillinfo.skillName);
            NowSkillLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), item.promotionTier + 1);
            NextSkillLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), item.promotionTier + 2);
            List<float> value = SkillSystem.GetTooltipApplyInfo(skillinfo);
            SkillLevelData skilllevel = SkillManager.Instance.SkillLevelDatas.Where(x => x.groupID == skillinfo.skillLevelId ).FirstOrDefault();

            float levelvalue = SkillSystem.GetLevelValue(skillinfo);

            if (value.Count > 0)
            {
                if (skilllevel != null)
                {
                    SkillEffect.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillLvUpTooltip), (value[0] ) * 100);
                }
                else
                {
                    skilllevel = SkillManager.Instance.SkillLevelDatas.Where(x => x.groupID == skillinfo.skillLevelId && x.levelID == item.promotionTier + 1).FirstOrDefault();
                    SkillEffect.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillLvUpTooltip), (value[0] + levelvalue* (item.promotionTier + 1)) * 100);
                }
            }
        }
        else
            SkillInfoObj.SetActive(false);
        UpDateSizeFitter();
    }
    public void OnClick_AdvancementBtn()
    {
        UserGameData data = UserGameData.Get();
        if (Promotioninfo != null)
        {
            EnableItem item = data.GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
            if (item.refining < Promotioninfo.termsReforge)
            {
                return;
            }
            if (Promotioninfo.needSelf > item.num)
                return;
            for (int i = 0; i < Promotioninfo.CostItems.Count; i++)
            {
                ItemClass cost = data.GetItemClass(Promotioninfo.CostItems[i].itemID);
                if (cost == null || cost.num < Promotioninfo.CostItems[i].itemValue)
                {
                    return;
                }
            }
            int isUp = ItemSystem.TierUp(SelectItemSlot.EquipDataInfo);
            if (isUp > 0)
            {
                _owner.ItemGrowthPopup.OpenTierUpResult();

                UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPPROMOTION);
            }
            else
                return;
        }
        else
            return;
        RefreshUI();
    }
    // Update is called once per frame
    private void Update()
    {
        if (UpdateDirty == true)
        {
            RefreshUI();
            UpdateDirty = false;
        }
    }
    void AddItemSlot(EquipInfoData info,int count)
    {
        EnableItem item = UserGameData.Get().GetEnableItem(info.ItemId);
        for (int i = 0; i < slotlist.Count; i++)
        {
            if (!slotlist[i].gameObject.activeSelf)
            {
                slotlist[i].gameObject.SetActive(true);
                slotlist[i].InitResult(info, 0);
                if(item==null|| item.num < count)
                {
                    if (item != null)
                    {
                        slotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + item.num + "</color>", count));
                    }
                    else
                    {
                        slotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + 0 + "</color>", count));

                    }
                    slotlist[i].SetDisable(true);
                    BtnCoverObj.SetActive(true);

                }
                else
                {
                    slotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FFFFFF>" + item.num + "</color>", count));
                    slotlist[i].SetDisable(false);
                }
                return;
            }
        }
        GameObject go = Instantiate(itemslot.gameObject, NeedItemScroll.content);
        PopupItemBox slot = go.GetComponent<PopupItemBox>();
        slot.gameObject.SetActive(true);
        slot.InitResult(info, 0);
        if (item == null || item.num < count)
        {
            if (item != null)
            {
                slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + item.num + "</color>", count));
            }
            else
            {
                slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + 0 + "</color>", count));
            }
            slot.SetDisable(true);
            BtnCoverObj.SetActive(true);
        }
        else
        {
            slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FFFFFF>" + item.num + "</color>", count));
            slot.SetDisable(false);
        }
        slotlist.Add(slot);
    }
    void AddItemSlot(ItemInfoData info, int count)
    {
        ItemClass item = UserGameData.Get().GetItemClass(info.Index);
        for (int i = 0; i < slotlist.Count; i++)
        {
            if (!slotlist[i].gameObject.activeSelf)
            {
                slotlist[i].gameObject.SetActive(true);
                slotlist[i].InitResult(info, 0);
                if (item == null || item.num < count)
                {
                    if (item != null)
                    {
                        slotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + item.num + "</color>", count));
                    }
                    else
                    {
                        slotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + 0 + "</color>", count));
                    }
                    slotlist[i].SetDisable(true);
                    BtnCoverObj.SetActive(true);
                }
                else
                {
                    slotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FFFFFF>" + item.num + "</color>", count));
                    slotlist[i].SetDisable(false);
                }
                return;
            }
        }
        GameObject go = Instantiate(itemslot.gameObject, NeedItemScroll.content);
        PopupItemBox slot = go.GetComponent<PopupItemBox>();
        slot.gameObject.SetActive(true);
        slot.InitResult(info, 0);
        if (item == null || item.num < count)
        {
            if (item != null)
            {
                slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + item.num + "</color>", count));
            }
            else
            {
                slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FF3B3B>" + 0 + "</color>", count));

            }
            slot.SetDisable(true);
            BtnCoverObj.SetActive(true);
        }
        else
        {
            slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), "<color=#FFFFFF>" + item.num + "</color>", count));
            slot.SetDisable(false);
        }
        slotlist.Add(slot);
    }
    void UpDateSizeFitter()
    {
        for (int i = 0; i < sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sizefitterlist[i].transform);
        }
    }
}
