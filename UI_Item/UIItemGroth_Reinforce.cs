using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//장비강화
public partial class UIItemGroth_Reinforce : UIItemGrowthBase
{


    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.REINFORCE; }

    [SerializeField] Text itemname;
    [SerializeField] UIItemSlot SelectSlot;
    [SerializeField] Text MaxLevelPerText;
    [SerializeField] Text NowLevel;
    [SerializeField] Text NextLevel;
    [SerializeField] GameObject NextLevelObj;
    [SerializeField] GameObject MaxLevelObj;
    [SerializeField] UIEffectText WeaponMainEffect;
    [SerializeField] UIEffectText OtherMainEffect;
    [SerializeField] UIEffectText TopEffect;
    [SerializeField] List<UIEffectText> AddEffectList;
    [SerializeField] List<Text> LockObjTextList;
    [SerializeField] Text GoldCostText;
    [SerializeField] Text CoverGoldCostText;
    [SerializeField] Text CostText;
    [SerializeField] Text ItemTypeText;
    [SerializeField] List<Image> CostImagelist;
    [SerializeField] Text UserCostText;
    [SerializeField] Image GradeIcon;
    [SerializeField] GameObject BottomObj;
    [SerializeField] List<GameObject> CostObjs;
    [SerializeField] GameObject BtnObj;
    [SerializeField] GameObject BtnCover;
    [SerializeField] GameObject BackObj;
    [SerializeField] GameObject MaxCover;
    [SerializeField] Transform EffectPos;
    [SerializeField] Button ReinforceBtn;
    [SerializeField] Color CantRefiningTextColor;
    [SerializeField] Color CanRefiningTextColor;
    Table_EquipLevelUpCostData.Info CostData = null;

    private bool UpdateDirtyInfo = false;
    public override void StartInitialize()
    {
        SelectItemSlot = SelectSlot;
    }
    public override void ClosePanel()
    {

        Util.SetActiveObject(this.gameObject, false);
        BottomObj.SetActive(false);
    }
    public override void OpenPanel(UIItemSlot _selectItemSlot)
    {
        base.OpenPanel(_selectItemSlot);
        BottomObj.SetActive(true);
        if (_selectItemSlot != null)
        {
            EnableItem selectitem = UserGameData.Get().GetEnableItem(_selectItemSlot.EquipDataInfo.ItemId);
            EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == _selectItemSlot.EquipDataInfo.itemType && x.grade == _selectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == _selectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
            EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                                where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade
                                                && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                                orderby v.maxLevel descending
                                                select v).FirstOrDefault();
            ItemTypeText.text = ItemSystem.GetItemSubTypeTitelText(_selectItemSlot.EquipDataInfo.itemType, _selectItemSlot.EquipDataInfo.qualityIndex);

            BackObj.SetActive(true);
            
        }
        RefreshUI();
    }

    private void Update()
    {
        if (UpdateDirtyInfo == true)
        {
        if (EffectPos.childCount > 0)
            return;
            RefreshUI();
            UpdateDirtyInfo = false;
        }
    }

    private void RefreshUI()
    {
        for (int i = 0; i < AddEffectList.Count; i++)
        {
            AddEffectList[i].gameObject.SetActive(false);
        }
        if (UserGameData.Get().CheckNickNameEmpty())
            return;
        _owner.ItemGrowthPopup.ReinforceTabText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ENCHANT");
        SelectSlot.SetData(SelectItemSlot.EquipDataInfo, null);
        GradeIcon.sprite = ItemSystem.GetGradeiconSprite(SelectItemSlot.EquipDataInfo.Grade);
        itemname.text = LocalizeManager.Instance.GetTXT(SelectItemSlot.EquipDataInfo.nameTextID);
        EnableItem selectitem = UserGameData.Get().GetEnableItem(SelectItemSlot.EquipDataInfo.ItemId);
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == SelectItemSlot.EquipDataInfo.itemType && x.grade == SelectItemSlot.EquipDataInfo.Grade
           && x.reforge == selectitem.refining && x.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipReforgeData maxrefiningdata = (from v in PopupManager.Instance.EquipReforgeDatas
                                            where v.equipType == SelectItemSlot.EquipDataInfo.itemType && v.grade == SelectItemSlot.EquipDataInfo.Grade 
                                            && v.gradeIndex == SelectItemSlot.EquipDataInfo.gradeIndex
                                            orderby v.maxLevel descending
                                            select v).FirstOrDefault();
        if (refiningdata.Index > 0)
        {
           
            if (refiningdata.maxLevel == selectitem.level)
            {
                NextLevelObj.SetActive(false);
                MaxLevelObj.SetActive(true);
                MaxCover.SetActive(true);
                BtnCover.SetActive(false);
                BtnObj.SetActive(false);
                for (int i = 0; i < CostObjs.Count; i++)
                {
                    CostObjs[i].SetActive(false);
                }
                MaxLevelPerText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MAXENCHANT"), refiningdata.maxLevel);
                EquipAbilityData equipstatdata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId)); 
                switch (SelectItemSlot.EquipDataInfo.itemType)
                {
                    case ITEM_TYPE.WEAPON:
                        {
                            WeaponMainEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * selectitem.level)
                                );
                            WeaponMainEffect.gameObject.SetActive(true);
                            OtherMainEffect.gameObject.SetActive(false);
                        }
                        break;
                    default:
                        {
                            WeaponMainEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * selectitem.level)
                                );
                            OtherMainEffect.SetStatTypeValue(equipstatdata.MainabilityList[1].Type, equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[1].LevelValue * selectitem.level)
                                );
                            WeaponMainEffect.gameObject.SetActive(true);
                            OtherMainEffect.gameObject.SetActive(true);
                        }
                        break;
                }
                List<AbilityType> abliftylist = new List<AbilityType>();
                for (int i = 0; i < equipstatdata.AbilityTypes.Count; i++)
                {

                    abliftylist.Add(new AbilityType
                    {
                        Value = equipstatdata.AbilityTypes[i].Value,
                        LevelValue = equipstatdata.AbilityTypes[i].LevelValue,
                        Type = equipstatdata.AbilityTypes[i].Type
                    });
                    AddEffectList[i].gameObject.SetActive(true);
                }
                for (int i = 0; i < abliftylist.Count; i++)
                {
                    AddEffectList[i].SetStatTypeValue(abliftylist[i].Type, abliftylist[i].Value + abliftylist[i].LevelValue * selectitem.refining);
                    AddEffectList[i].LockObj.SetActive(false);
                }
               

            }
            else
            {
                MaxCover.SetActive(false);
                NextLevelObj.SetActive(true);
                MaxLevelObj.SetActive(false);
                BtnObj.SetActive(true);
                MaxLevelPerText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MAXENCHANT"), refiningdata.maxLevel);
                NowLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ENCHANTING"), selectitem.level);
                NextLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_ENCHANTING"), selectitem.level+1);

                for (int i = 0; i < CostObjs.Count; i++)
                {
                    CostObjs[i].SetActive(true);
                }
                EquipAbilityData equipstatdata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId)); 
                switch (SelectItemSlot.EquipDataInfo.itemType)
                {
                    case ITEM_TYPE.WEAPON:
                        {
                            WeaponMainEffect.SetStatTypeAddValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + equipstatdata.MainabilityList[0].LevelValue * selectitem.level, equipstatdata.MainabilityList[0].LevelValue
                                );
                            WeaponMainEffect.gameObject.SetActive(true);
                            OtherMainEffect.gameObject.SetActive(false);
                        }
                        break;
                    default:
                        {

                            WeaponMainEffect.SetStatTypeAddValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + equipstatdata.MainabilityList[0].LevelValue * selectitem.level, equipstatdata.MainabilityList[0].LevelValue
                                );
                            OtherMainEffect.SetStatTypeAddValue(equipstatdata.MainabilityList[1].Type, equipstatdata.MainabilityList[1].Value + equipstatdata.MainabilityList[1].LevelValue * selectitem.level, equipstatdata.MainabilityList[1].LevelValue
                                );
                            WeaponMainEffect.gameObject.SetActive(true);
                            OtherMainEffect.gameObject.SetActive(true);
                        }
                        break;
                }
                List<AbilityType> abliftylist = new List<AbilityType>();
                for (int i = 0; i < equipstatdata.AbilityTypes.Count; i++)
                {

                    AddEffectList[i].gameObject.SetActive(true);
                    abliftylist.Add(new AbilityType
                    {
                        Value = equipstatdata.AbilityTypes[i].Value,
                        LevelValue = equipstatdata.AbilityTypes[i].LevelValue,
                        Type = equipstatdata.AbilityTypes[i].Type
                    });

                }

                for (int i = 0; i < abliftylist.Count; i++)
                {
                    AddEffectList[i].SetStatTypeValue(abliftylist[i].Type, abliftylist[i].Value + abliftylist[i].LevelValue * selectitem.refining);
                    AddEffectList[i].gameObject.SetActive(true);
                    AddEffectList[i].LockObj.SetActive(false);
                }

            }
            EquipLevelUpCostData CostData = PopupManager.Instance.EquipLevelUpCostDatas.Where(n => n.equipID == selectitem.Index && n.level == selectitem.level).FirstOrDefault();
            CheckCost();
            if (CostData.itemTypes.Count > 0)
            {
                if (CostData.itemTypes[0].ID > 0)
                {
                    ItemInfoData itemdata = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==CostData.itemTypes[0].ID).FirstOrDefault();
                    if (itemdata.itemType == ITEM_TYPE.GOLD)
                    {
                        GoldCostText.text = CostData.itemTypes[0].ToString();
                        CoverGoldCostText.text = CostData.itemTypes[0].ToString();
                    }
                    else
                    {
                        ItemClass usecostitem = UserGameData.Get().GetItemClass(CostData.itemTypes[0].ID);
                        if(usecostitem==null|| usecostitem.num< CostData.itemTypes[0].value)
                        {
                            CostText.color = CantRefiningTextColor;
                        }
                        else
                        {
                            CostText.color = CanRefiningTextColor;
                        }
                        CostText.text = CostData.itemTypes[0].ToString(); 
                    }
                    itemdata = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == CostData.itemTypes[1].ID).FirstOrDefault();
                    if (itemdata.itemType == ITEM_TYPE.GOLD)
                    {
                        GoldCostText.text = CostData.itemTypes[1].ToString();
                        CoverGoldCostText.text = CostData.itemTypes[1].ToString();
                    }
                    else
                    {
                        ItemClass usecostitem = UserGameData.Get().GetItemClass(CostData.itemTypes[1].ID);
                        if (usecostitem == null || usecostitem.num < CostData.itemTypes[1].value)
                        {
                            CostText.color = CantRefiningTextColor;
                        }
                        else
                        {
                            CostText.color = CanRefiningTextColor;
                        }
                        CostText.text = CostData.itemTypes[1].ToString();
                    }
                }
            }
            //데이터로빼야됨
            ItemClass costitem = UserGameData.Get().GetItemClass(90011);
            if (costitem == null)
            {
                UserCostText.text = "0";
                BtnCover.SetActive(true);
            }
            else
            {
                UserCostText.text = costitem.num.ToString();
            }

            if (!ReinforceBtn.enabled)
                ReinforceBtn.enabled = true;
        }
        else
        {
            {
                if (maxrefiningdata.maxLevel == selectitem.level)
                {
                    NextLevelObj.SetActive(false);
                    MaxLevelObj.SetActive(true);
                    MaxCover.SetActive(true);
                    BtnCover.SetActive(false); 
                    BtnObj.SetActive(false);
                    for (int i = 0; i < CostObjs.Count; i++)
                    {
                        CostObjs[i].SetActive(false);
                    }
                    MaxLevelPerText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MAXENCHANT"), refiningdata.maxLevel);
                    EquipAbilityData equipstatdata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(SelectItemSlot.EquipDataInfo.ItemId));
                    switch (SelectItemSlot.EquipDataInfo.itemType)
                    {
                        case ITEM_TYPE.WEAPON:
                            {
                                WeaponMainEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * selectitem.level)
                                    );
                                WeaponMainEffect.gameObject.SetActive(true);
                                OtherMainEffect.gameObject.SetActive(false);
                            }
                            break;
                        default:
                            {
                                WeaponMainEffect.SetStatTypeValue(equipstatdata.MainabilityList[0].Type, equipstatdata.MainabilityList[0].Value + (equipstatdata.MainabilityList[0].LevelValue * selectitem.level)
                                    );
                                OtherMainEffect.SetStatTypeValue(equipstatdata.MainabilityList[1].Type, equipstatdata.MainabilityList[1].Value + (equipstatdata.MainabilityList[1].LevelValue * selectitem.level)
                                    );
                                WeaponMainEffect.gameObject.SetActive(true);
                                OtherMainEffect.gameObject.SetActive(true);
                            }
                            break;
                    }
                    List<AbilityType> abliftylist = new List<AbilityType>();
                    for (int i = 0; i < equipstatdata.AbilityTypes.Count; i++)
                    {

                        abliftylist.Add(new AbilityType
                        {
                            Value = equipstatdata.AbilityTypes[i].Value,
                            LevelValue = equipstatdata.AbilityTypes[i].LevelValue,
                            Type = equipstatdata.AbilityTypes[i].Type
                        });
                        AddEffectList[i].gameObject.SetActive(true);
                    }
                    for (int i = 0; i < abliftylist.Count; i++)
                    {

                        {
                                AddEffectList[i].SetStatTypeValue(abliftylist[i].Type, abliftylist[i].Value + abliftylist[i].LevelValue * selectitem.refining);

                        }


                    }

                }

                CheckCost();
                
                ItemClass costitem = UserGameData.Get().GetItemClass(90011);
                if (costitem == null)
                {
                    UserCostText.text = "0";
                    BtnCover.SetActive(true);
                }
                else
                {
                    UserCostText.text = costitem.num.ToString();
                }

                if (!ReinforceBtn.enabled)
                    ReinforceBtn.enabled = true;
            }
        }
        
        }

    
    void CheckCost()
    {
        if (CostData == null)
            return;
        if (CostData.Index < 0)
            return;
        if (CostData.itemTypes.Count <= 0)
            return;
        if (CostData.itemTypes[0].ID < 0)
            return;
        UserGameData data = UserGameData.Get();
        if (data.CheckNickNameEmpty())
            return;
        bool CanLevelup = false;
        for(int i=0;i< CostData.itemTypes.Count; i++)
        {
            ItemInfoData itemdata = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==CostData.itemTypes[i].ID).FirstOrDefault();
            switch (itemdata.itemType)
            {
                case ITEM_TYPE.GOLD:
                    {
                        if(data.AssetGold>= CostData.itemTypes[i].value)
                        {
                            CanLevelup = true;
                        }
                    }
                    break;
                default:
                    {
                        ItemClass costitem = UserGameData.Get().GetItemClass(itemdata.Index);
                        if (costitem != null)
                        {
                            if(costitem.num >= CostData.itemTypes[i].value)
                            {
                                CanLevelup = true;
                            }
                        }

                    }
                    break;
            }
            if (!CanLevelup)
            {
                break;
            }
        }
        BtnCover.SetActive(!CanLevelup);
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
            if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
            {
                CharacterManager.Instance.MyActor.InitStat();
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
    IEnumerator TryReinforce()
    {
        EffectBase _effUpgrade = UIManager.Instance.CreateEffect(PopupManager.Instance.fxItemReinforceSuccess);
        _effUpgrade.transform.parent = EffectPos;
        _effUpgrade.transform.localPosition = Vector3.zero;
        _effUpgrade.transform.localScale = EffectPos.localScale; 
        _effUpgrade.SetLifeTime(0.5f);
        while (EffectPos.childCount>0)
        {
            yield return new WaitForEndOfFrame();
        }
        
        ReinforceBtn.enabled = true;
        RefreshUI();
        yield break;
    }
    public void EquipItemAction()
    {
        SetUpgradeBtn(out ItemSystem.ItemTotalInfo _totalInfo);
    }

    public void OpenReinforcePopup()
    {
        _owner.ItemGrowthPopup.OpenPopup(eItemGrowthType.REINFORCE,SelectItemSlot);
    }
    public void OpenRefiningPopup()
    {
        if (_owner.ItemGrowthPopup == null)
        {
        }
        _owner.ItemGrowthPopup.OpenPopup(eItemGrowthType.REFINING, SelectItemSlot);
    }


}
