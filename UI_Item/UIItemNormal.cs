using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UIItemNormal : UIItemGrowthBase
{
    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.NORAML; }
    [SerializeField] public EquipPart _equipparts;
    [SerializeField] Text TotalPower;
    [SerializeField] Transform NormalStatPos;
    [SerializeField] Transform AtkPos;
    [SerializeField] Transform DefPos;
    [SerializeField] Transform SkillStatPos;
    [SerializeField] Transform EconomyStatPos;
    [SerializeField] GameObject statScroll;
    [SerializeField] GameObject SuggestionEquipBtnCover;
    [SerializeField] UIEffectText NormalStatObj;
    [SerializeField] UIEffectText ScrollStatObj;
    [SerializeField] UIItemGrowthListInfo itemlistinfo;
    [SerializeField] List<ContentSizeFitter> _sizefitterlist;
    List<EquipSortStatData> statlist;
    Dictionary<int, UIEffectText> _DicEffectText;

    public override void StartInitialize()
    {
        base.StartInitialize();
        _equipparts.WeaponSlot.SetEmpty(true);
        _equipparts.SubWeaponSlot.SetEmpty(true);
        _equipparts.WeaponSlot.SetLockInfo(eContentsOpenType.EQUIPWPSLOT);
        _equipparts.SubWeaponSlot.SetLockInfo(eContentsOpenType.EQUIPSDSLOT);
        _equipparts.WeaponSlot.SetLock(false);
        _equipparts.SubWeaponSlot.SetLock(false);
        _equipparts.RingSlot.SetEmpty(true);
        _equipparts.EarringSlot.SetEmpty(true);
        _equipparts.NecklaceSlot.SetEmpty(true);
        _equipparts.BraceletSlot.SetEmpty(true);
        _equipparts.RingSlot.SetLockInfo(eContentsOpenType.EQUIPACSLOTA);
        _equipparts.EarringSlot.SetLockInfo(eContentsOpenType.EQUIPACSLOTA);
        _equipparts.BraceletSlot.SetLockInfo(eContentsOpenType.EQUIPACSLOTA);
        _equipparts.NecklaceSlot.SetLockInfo(eContentsOpenType.EQUIPACSLOTA);
        statlist = Getlist();
        statScroll.gameObject.SetActive(false);
        _DicEffectText = new Dictionary<int, UIEffectText>();
        for (int i = 0; i < statlist.Count; i++)
        {
            UIEffectText effecttext = null;
            switch (statlist[i].statGroup)
            {
                case 1:
                    {
                        GameObject go = Instantiate(NormalStatObj.gameObject, NormalStatPos);
                        effecttext = go.GetComponent<UIEffectText>();
                        effecttext.SetStatTypeValue(statlist[i].statType, 0);
                    }
                    break;
                case 2:
                    {
                        GameObject go = Instantiate(ScrollStatObj.gameObject, AtkPos);
                        effecttext = go.GetComponent<UIEffectText>();
                        effecttext.SetStatTypeValue(statlist[i].statType, 0);
                    }
                    break;
                case 3:
                    {
                        GameObject go = Instantiate(ScrollStatObj.gameObject, DefPos);
                        effecttext = go.GetComponent<UIEffectText>();
                        effecttext.SetStatTypeValue(statlist[i].statType, 0);
                    }
                    break;
                case 4:
                    {
                        GameObject go = Instantiate(ScrollStatObj.gameObject, SkillStatPos);
                        effecttext = go.GetComponent<UIEffectText>();
                        effecttext.SetStatTypeValue(statlist[i].statType, 0);
                    }
                    break;
                case 5:
                    {
                        GameObject go = Instantiate(ScrollStatObj.gameObject, EconomyStatPos);
                        effecttext = go.GetComponent<UIEffectText>();
                        effecttext.SetStatTypeValue(statlist[i].statType, 0);
                    }
                    break;

            }

            if (effecttext != null) 
            {
                effecttext.gameObject.SetActive(true);
            }

            if (!_DicEffectText.ContainsKey(statlist[i].Index))
            {
                _DicEffectText.Add(statlist[i].Index, effecttext);
            }
        }
    }
    public override void OpenPanel(UIItemSlot _selectItemSlot)
    {
        base.OpenPanel(_selectItemSlot);
        RefleshUI();
         _equipparts.WeaponSlot.SetSlotLock();
         _equipparts.SubWeaponSlot.SetSlotLock();
         _equipparts.EarringSlot.SetSlotLock();
         _equipparts.RingSlot.SetSlotLock();
         _equipparts.BraceletSlot.SetSlotLock();
         _equipparts.NecklaceSlot.SetSlotLock();
    }
    public override void ClosePanel()
    {
        base.ClosePanel();
        gameObject.SetActive(true);
    }
    public void RefleshUI()
    {
        if (UserGameData.Get().CheckNickNameEmpty())
            return;
        UserGameData data = UserGameData.Get();
        if (CharacterManager.Instance.MyActor == null)
            return;
        TotalPower.text = StringUtil.Format("{0:0,0}", (long)CharacterManager.Instance.MyActor.stat.GetBattlePower());

        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPWPSLOT))
        {
            if (data.EquipWeaponItemid >= 0)
            {
                _equipparts.WeaponSlot.SetData(UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(data.EquipWeaponItemid)), ClickEquipSlot);
                _equipparts.WeaponSlot.SetEmpty(false);
            }
            else
                _equipparts.WeaponSlot.SetEmpty(true);
        }

        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPSDSLOT))
        {
            if (data.EquipSubWeaponItemid >= 0)
            {
                _equipparts.SubWeaponSlot.SetData(UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(data.EquipSubWeaponItemid)), ClickEquipSlot);
                _equipparts.SubWeaponSlot.SetEmpty(false);
            }
            else
                _equipparts.SubWeaponSlot.SetEmpty(true);
        }
        
        
        foreach (int key in data.EquipAccessoryItemidlist.Keys)
        {
            EquipInfoData info= UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(data.EquipAccessoryItemidlist[key]));
            switch (key)
            {
                case 1:
                    {
                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPACSLOTA))
                        {
                            _equipparts.RingSlot.SetData(info, ClickEquipSlot);
                            _equipparts.RingSlot.SetEmpty(false);
                        }
                    }
                    break;
                case 2:
                    {
                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPACSLOTA))
                        {
                            _equipparts.EarringSlot.SetData(info, ClickEquipSlot);
                            _equipparts.EarringSlot.SetEmpty(false);
                        }
                    }
                    break;
                case 3:
                    {
                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPACSLOTA))
                        {
                            _equipparts.BraceletSlot.SetData(info, ClickEquipSlot);
                            _equipparts.BraceletSlot.SetEmpty(false);
                        }
                    }
                    break;
                case 4:
                    {
                        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPACSLOTA))
                        {
                            _equipparts.NecklaceSlot.SetData(info, ClickEquipSlot);
                            _equipparts.NecklaceSlot.SetEmpty(false);
                        }
                    }
                    break;
            }
        }

        itemlistinfo.RefreshEquipMark();
        UpDateStat();
        bool canSuggestionEquip = false;
        if (_equipparts.WeaponSlot.EquipDataInfo != null)
        {
            if (UserGameData.Get().CheckStatValue(_equipparts.WeaponSlot.EquipDataInfo.ItemId, ITEM_TYPE.WEAPON, _equipparts.WeaponSlot.EquipDataInfo.qualityIndex))
            {
                canSuggestionEquip = true;
            }
        }
        else
        {
           if(UserGameData.Get().CheckStatValue(-1, ITEM_TYPE.WEAPON, 1))
            {
                canSuggestionEquip = true;
            }
        }
        if (_equipparts.SubWeaponSlot.EquipDataInfo != null)
        {
           if( UserGameData.Get().CheckStatValue(_equipparts.SubWeaponSlot.EquipDataInfo.ItemId, ITEM_TYPE.SHIELD, _equipparts.SubWeaponSlot.EquipDataInfo.qualityIndex))
            {
                canSuggestionEquip = true;
            }
        }
        else
        {
           if( UserGameData.Get().CheckStatValue(-1, ITEM_TYPE.SHIELD, 1))
            {
                canSuggestionEquip = true;
            }
        }

        if (_equipparts.RingSlot.EquipDataInfo != null)
        {
           if( UserGameData.Get().CheckStatValue(_equipparts.RingSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.RingSlot.EquipDataInfo.qualityIndex))
            {
                canSuggestionEquip = true;
            }
        }
        else
        {
           if( UserGameData.Get().CheckStatValue(-1, ITEM_TYPE.ACCESSARY, 1))
            {
                canSuggestionEquip = true;
            }
        }

        if (_equipparts.EarringSlot.EquipDataInfo != null)
        {
            if( UserGameData.Get().CheckStatValue(_equipparts.EarringSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.EarringSlot.EquipDataInfo.qualityIndex))
            {
                canSuggestionEquip = true;
            }
        }
        else
        {
           if( UserGameData.Get().CheckStatValue(-1, ITEM_TYPE.ACCESSARY, 2))
            {
                canSuggestionEquip = true;
            }
        }

        if (_equipparts.BraceletSlot.EquipDataInfo != null)
        {
           if( UserGameData.Get().CheckStatValue(_equipparts.BraceletSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.BraceletSlot.EquipDataInfo.qualityIndex))
            {
                canSuggestionEquip = true;
            }
        }
        else
        {
          if( UserGameData.Get().CheckStatValue(-1, ITEM_TYPE.ACCESSARY, 3))
            {
                canSuggestionEquip = true;
            }
        }

        if (_equipparts.NecklaceSlot.EquipDataInfo != null)
        {
           if( UserGameData.Get().CheckStatValue(_equipparts.NecklaceSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.NecklaceSlot.EquipDataInfo.qualityIndex))
            {
                canSuggestionEquip = true;
            }
        }
        else
        {
           if( UserGameData.Get().CheckStatValue(-1, ITEM_TYPE.ACCESSARY, 4))
            {
                canSuggestionEquip = true;
            }
        }
        SetSuggestionEquipBtnCover(!canSuggestionEquip);

    }
    public void ClickEquipSlot(UIEquipSlot slot)
    {
        switch (slot.EquipDataInfo.itemType)
        {
            case ITEM_TYPE.WEAPON:
                {
                    itemlistinfo.ItemSortToggles[0].isOn = true;
                }
                break;
            case ITEM_TYPE.SHIELD:
                {
                    itemlistinfo.ItemSortToggles[1].isOn = true;
                }
                break;
            case ITEM_TYPE.ACCESSARY:
                {
                    itemlistinfo.ItemSortToggles[2].isOn = true;
                }
                break;

        }

        itemlistinfo.SetSubTypeSort(slot.EquipDataInfo.qualityIndex - 1);
        UIItemSlot itemslot = itemlistinfo.GetItemSlot(slot.EquipDataInfo.ItemId);
        itemslot.OnClick_ItemSlot();
    }
    public void OpenStatScroll()
    {
        statScroll.SetActive(true); 
        UpDateStatSize();
    }
    public void CloseStatScroll()
    {
        statScroll.SetActive(false);
        UpDateStatSize();

    }
    public void UpDateStatSize()
    {
        for(int i=0;i< _sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_sizefitterlist[i].transform);
        }
    }
    //스탯갱신
    public void UpDateStat()
    {
        ActorUser myactor = CharacterManager.Instance.MyActor;
        for(int i=0;i< statlist.Count; i++)
        {
            float value = 0;
            string valuetext = "";
            float[] minmaxvalue = null;
            string _strfromat= StateSystem.GetStatFormatString(statlist[i].statType);
            if (statlist[i].statType != eStatType.None)
            {
                minmaxvalue =DataManager.Instance.GetLimitStat(statlist[i].statType);

                if (statlist[i].statType == eStatType.atkSpeedBase)
                {
                    if (UserGameData.Get().EquipWeaponItemid > 0)
                    {
                        if (PopupManager.Instance.EquipAbilityDatas.Find(x => x.Index == UserGameData.Get().EquipWeaponItemid) != null)
                        {
                            EquipAbilityData weaponabilityinfo = PopupManager.Instance.EquipAbilityDatas.Find(x => x.Index == UserGameData.Get().EquipWeaponItemid);

                            value = weaponabilityinfo.MainabilityList[1].Value / ((float)myactor.stat.FinalStatValue.Values[(int)eStatType.atkSpeed]);
                            valuetext = string.Format(_strfromat, value);
                        }
                    }
                    else
                    {
                        value = 1 / (0 + (float)myactor.stat.FinalStatValue.Values[(int)eStatType.atkSpeed]);
                        valuetext = string.Format(_strfromat, value);
                    }

                    _DicEffectText[statlist[i].Index].SetStringTypeStringValue(LocalizeManager.Instance.GetTXT(statlist[i].statText), valuetext);
                    continue;
                }
                else
                {
                    if (statlist[i].statIncludeStat[0] != eStatType.None)
                    {
                        value = (float)myactor.stat.FinalStatValue.Values[(int)statlist[i].statType];
                    }
                    else
                    {
                        if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                        {
                            value = (float)myactor.stat.BaseStatValue.Values[(int)statlist[i].statType] + (float)myactor.stat.AddAbsoluteStatValue.Values[(int)statlist[i].statType];
                            if (minmaxvalue != null)
                            {
                                if (value < minmaxvalue[0])
                                {
                                    value = minmaxvalue[0];
                                }
                                else if (value > minmaxvalue[1])
                                {
                                    value = minmaxvalue[1];
                                }
                            }
                        }
                        else if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                        {
                            value = (float)myactor.stat.BaseStatValue.Values[(int)statlist[i].statType] + (float)myactor.stat.AddRatioStatValue.Values[(int)statlist[i].statType];
                            double check = myactor.stat.AddAbsoluteStatValue.Values[(int)statlist[i].statType];
                            if (minmaxvalue != null)
                            {
                                if (value < minmaxvalue[0])
                                {
                                    value = minmaxvalue[0];
                                }
                                else if (value > minmaxvalue[1])
                                {
                                    value = minmaxvalue[1];
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                if(statlist[i].abilityType== eSkillAbilityType.campain_item)
                {
                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                        value = myactor.buff.GetCampainItem(SKILL_VALUE_TYPE.PERCENT);
                    }
                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {
                        value = myactor.buff.GetCampainItem(SKILL_VALUE_TYPE.INTERGER);
                    }
                }
                else if (statlist[i].abilityType == eSkillAbilityType.campain_exp)
                {
                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                        value = myactor.buff.GetCampainExp(SKILL_VALUE_TYPE.PERCENT);
                    }
                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {
                        value = myactor.buff.GetCampainExp(SKILL_VALUE_TYPE.INTERGER);
                    }
                }
                else if (statlist[i].abilityType == eSkillAbilityType.campain_gold)
                {
                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                         value = myactor.buff.GetCampainGold(SKILL_VALUE_TYPE.PERCENT);
                    }
                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {
                         value = myactor.buff.GetCampainGold(SKILL_VALUE_TYPE.INTERGER);
                    }
                }
                else if (statlist[i].abilityType == eSkillAbilityType.offline_gold)
                {
                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                         value = myactor.buff.GetOfflineGold(SKILL_VALUE_TYPE.PERCENT);
                    }
                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {
                         value = myactor.buff.GetOfflineGold(SKILL_VALUE_TYPE.INTERGER); 
                    }
                }
                else if (statlist[i].abilityType == eSkillAbilityType.chargrowcost_down)
                {

                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                        value = myactor.buff.GetChargrowcostdown();
                    }
                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {

                    }
                }
                else if (statlist[i].abilityType == eSkillAbilityType.equipenc_cost_down)
                {
                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                         value = myactor.buff.GetEquipenccostdown(SKILL_VALUE_TYPE.PERCENT);
                    }

                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {

                         value = myactor.buff.GetEquipenccostdown(SKILL_VALUE_TYPE.INTERGER); 
                    }
                }
                else if(statlist[i].abilityType == eSkillAbilityType.dungeon_gold)
                {

                    if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                    {
                         value = myactor.buff.GetDungeonGold(SKILL_VALUE_TYPE.PERCENT);
                    }
                    else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    {
                        value = myactor.buff.GetDungeonGold(SKILL_VALUE_TYPE.INTERGER);
                    }

                }
                else if (statlist[i].abilityType == eSkillAbilityType.dungeon_item)
                {
                    if(statlist[i].abilityTargetStat!= eStatType.None)
                    {
                        if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                        {
                             value = myactor.buff.GetDungeonItem(SKILL_VALUE_TYPE.PERCENT, statlist[i].abilityTargetStat);
                        }
                        else if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                        {
                             value = myactor.buff.GetDungeonItem(SKILL_VALUE_TYPE.INTERGER, statlist[i].abilityTargetStat);
                        }
                    }
                }
            }
            string colorcode = "";
            if (minmaxvalue!=null) 
            {
                if(value<= minmaxvalue[0])
                {
                    colorcode= DataManager.Instance.CommonDatas.Find(x => x.DataType == "limitMinNumColor").DataValues[0];
                }
                else if(value >= minmaxvalue[1])
                {
                    colorcode = DataManager.Instance.CommonDatas.Find(x => x.DataType == "limitMaxNumColor").DataValues[0];
                }
            }

            if (statlist[i].statCorrection != 0)
            {
                value += statlist[i].statCorrection;
            }
            if (statlist[i].statType == eStatType.moveSpeed)
            {
            }
            if(statlist[i].statType == eStatType.fatalBlowDmg)
            {
                value -= statlist[i].statCorrection;
            }
            if (string.IsNullOrEmpty(colorcode))
            {
                if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    valuetext = string.Format(_strfromat, value);
                else if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                {
                    value *= 100;
                    valuetext = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SETUP_005"),string.Format( _strfromat, value));
                }
                else
                    valuetext = string.Format(_strfromat, (long)value);// ((long)value).ToString();
            }
            else
            {
                if (statlist[i].statShowNumType == eTypeStatusValue.INTEGER)
                    valuetext = string.Format("<color=" + colorcode + ">"  + _strfromat + "</color>", value);
                else if (statlist[i].statShowNumType == eTypeStatusValue.PERCENT)
                {
                    value *= 100;
                    valuetext = string.Format("<color="+ colorcode + ">" + LocalizeManager.Instance.GetTXT("STR_UI_SETUP_005") + "</color>", string.Format(_strfromat, value));
                }
                else
                    valuetext = string.Format("<color=" + colorcode + ">"+ _strfromat + "</color>", (long)value);// valuetext = "<color=" + colorcode + ">" + ((long)value).ToString() + "</color>";
            }
            _DicEffectText[statlist[i].Index].SetStringTypeStringValue(LocalizeManager.Instance.GetTXT(statlist[i].statText), valuetext);
        }
    }
    //자동장착
    public void OnClick_SuggestionEquipBtn()
    {
        int Weaponindex = -1;
        int SubWeaponindex = -1;
        int Ringindex=-1;
        int Earringindex = -1;
        int Necklaceindex = -1;
        int Braceletindex = -1;
        string beforeitem = UserGameData.Get().EquipItems;
        if (_equipparts.WeaponSlot.EquipDataInfo != null)
        {
            Weaponindex = UserGameData.Get().GetSuggestionItem(_equipparts.WeaponSlot.EquipDataInfo.ItemId, ITEM_TYPE.WEAPON, _equipparts.WeaponSlot.EquipDataInfo.qualityIndex);
        }
        else
        {
            Weaponindex = UserGameData.Get().GetSuggestionItem(-1, ITEM_TYPE.WEAPON, 1);
        }
        if (_equipparts.SubWeaponSlot.EquipDataInfo != null)
        {
            SubWeaponindex = UserGameData.Get().GetSuggestionItem(_equipparts.SubWeaponSlot.EquipDataInfo.ItemId, ITEM_TYPE.SHIELD, _equipparts.SubWeaponSlot.EquipDataInfo.qualityIndex);
        }
        else
        {
            SubWeaponindex = UserGameData.Get().GetSuggestionItem(-1, ITEM_TYPE.SHIELD, 1);
        }

        if (_equipparts.RingSlot.EquipDataInfo != null)
        {
            Ringindex = UserGameData.Get().GetSuggestionItem(_equipparts.RingSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.RingSlot.EquipDataInfo.qualityIndex);
        }
        else
        {
            Ringindex = UserGameData.Get().GetSuggestionItem(-1, ITEM_TYPE.ACCESSARY, 1);
        }

        if (_equipparts.EarringSlot.EquipDataInfo != null)
        {
            Earringindex= UserGameData.Get().GetSuggestionItem(_equipparts.EarringSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.EarringSlot.EquipDataInfo.qualityIndex);
        }
        else
        {
            Earringindex= UserGameData.Get().GetSuggestionItem(-1, ITEM_TYPE.ACCESSARY, 2);
        }

        if (_equipparts.BraceletSlot.EquipDataInfo != null)
        {
            Braceletindex = UserGameData.Get().GetSuggestionItem(_equipparts.BraceletSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.BraceletSlot.EquipDataInfo.qualityIndex);
        }
        else
        {
            Braceletindex = UserGameData.Get().GetSuggestionItem(-1, ITEM_TYPE.ACCESSARY, 3);
        }

        if (_equipparts.NecklaceSlot.EquipDataInfo != null)
        {
            Necklaceindex= UserGameData.Get().GetSuggestionItem(_equipparts.NecklaceSlot.EquipDataInfo.ItemId, ITEM_TYPE.ACCESSARY, _equipparts.NecklaceSlot.EquipDataInfo.qualityIndex);
        }
        else
        {
            Necklaceindex= UserGameData.Get().GetSuggestionItem(-1, ITEM_TYPE.ACCESSARY, 4);
        }

        if (Weaponindex > 0 && UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPWPSLOT))
        {
            
            ItemSystem.SetEquipItem(Weaponindex);
            if (CharacterManager.Instance.MyActor != null)
            {
                CharacterManager.Instance.MyActor.UpDatePrefab();
                
            }
            _owner.ActorDummy.UpDatePrefab(UserGameData.Get().EquipWeaponItemid, UserGameData.Get().EquipSubWeaponItemid);
        }
        if (SubWeaponindex > 0 && UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPSDSLOT))
        {
            ItemSystem.SetEquipItem(SubWeaponindex);
            if (CharacterManager.Instance.MyActor != null)
            {
                CharacterManager.Instance.MyActor.UpDatePrefab();

            }
            _owner.ActorDummy.UpDatePrefab(UserGameData.Get().EquipWeaponItemid, UserGameData.Get().EquipSubWeaponItemid);

        }

        if (UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPACSLOTA))
        {
            if (Ringindex > 0)
            {
                ItemSystem.SetEquipItem(Ringindex);
            }
            if (Earringindex > 0)
            {
                ItemSystem.SetEquipItem(Earringindex);
            }
            if (Necklaceindex > 0)
            {
                ItemSystem.SetEquipItem(Necklaceindex);
            }
            if (Braceletindex > 0)
            {
                ItemSystem.SetEquipItem(Braceletindex);
            }
        }

        _owner.UpdateDirtyInfo();
        _owner.GrowthItemListInfo.ReSetTapNotice();
        RefleshUI();
        string state = "EquipData" + UserGameData.Get().EquipItems;
        if (beforeitem != state)
        {
            NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "SUGGESTIONEQUIP", state, "BeforeEquip:" + beforeitem);
        }
    }
    public void SetSuggestionEquipBtnCover(bool enable)
    {
        SuggestionEquipBtnCover.SetActive(enable);
    }
    public void ReSetEquipSlot()
    {
         _equipparts.WeaponSlot.EquipDataInfo = null;
        _equipparts.WeaponSlot.SetEmpty(true);
        _equipparts.SubWeaponSlot.EquipDataInfo = null;
        _equipparts.SubWeaponSlot.SetEmpty(true);
        _equipparts.RingSlot.EquipDataInfo = null;
        _equipparts.RingSlot.SetEmpty(true);
        _equipparts.EarringSlot.EquipDataInfo = null;
        _equipparts.EarringSlot.SetEmpty(true);
        _equipparts.BraceletSlot.EquipDataInfo = null;
        _equipparts.BraceletSlot.SetEmpty(true);
        _equipparts.NecklaceSlot.EquipDataInfo = null;
        _equipparts.NecklaceSlot.SetEmpty(true);
    }

    void OnEnable()
    {
        _equipparts.WeaponSlot.SetSlotLock();
        _equipparts.SubWeaponSlot.SetSlotLock();
        _equipparts.EarringSlot.SetSlotLock();
        _equipparts.RingSlot.SetSlotLock();
        _equipparts.BraceletSlot.SetSlotLock();
        _equipparts.NecklaceSlot.SetSlotLock();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateDirty)
        {
            UpdateDirty = false;
            RefleshUI();
        }
        else
            return;
    }
    public List<EquipSortStatData> Getlist()
    {
        List<EquipSortStatData> list = new List<EquipSortStatData>();
        list = PopupManager.Instance.EquipSortStatDatas.OrderBy(x => x.Index).ToList();
        return list;
    }
}
