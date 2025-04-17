using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


//합성.
public partial class UIItemGroth_Compose : UIItemGrowthBase
{
    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.COMPOSE; }

    private bool UpdateDirtyInfo = false;

    //합성관련
    [SerializeField] UIItemSlot Targetitembox;
    [SerializeField] List<UIItemSlot> needselfslotlist;
    [SerializeField] Image costitemimage;
    [SerializeField] Text NeedCostText;
    [SerializeField] GameObject Bottomobj;
    [SerializeField] GameObject GradeBG;
    [SerializeField] GameObject BtnCover;
    [SerializeField] Text TargeItemName;
    [SerializeField] Text NeedSelfText;
    [SerializeField] Text NeedNotifyText;
    [SerializeField] Text SelectCountText;
    [SerializeField] Image GradeIcon;
    [SerializeField] GameObject BackObj;
    [SerializeField] GameObject gradeUpBtn;
    EquipInfoData SelectItemInfo=null;
    EquipComposeData composedata = null;
    List<EnableItem> composableItemList = new List<EnableItem>();
    int SelectCount = 0;
    private ITEM_TYPE selectType = ITEM_TYPE.WEAPON;

    public override EquipInfoData SelectItemSlotInfo => SelectItemInfo;

    public override void OpenPanel(UIItemSlot _selectItemSlot)
    {
        base.OpenPanel(_selectItemSlot);

        GradeBG.SetActive(true);
        if (_selectItemSlot != null)
        {
            BackObj.SetActive(true);
            if (_selectItemSlot.EquipDataInfo.Grade>= ITEM_GRADE.MYTH)
            {
                _owner.ItemGrowthPopup.OpenAwake();
                return;
            }
            SelectItemInfo = _selectItemSlot.EquipDataInfo;
        }
        SelectCount = 0;
        RefreshUI();
    }
    public override void ClosePanel()
    {
        base.ClosePanel();
        Bottomobj.SetActive(false);
        GradeBG.SetActive(false);
    }

    private void Update()
    {
        if (UpdateDirtyInfo == true)
        {
            RefreshUI();
            UpdateDirtyInfo = false;
        }
    }
    //ui갱신
    private void RefreshUI()
    {
        if (SelectItemInfo != null)
        {
            composedata = PopupManager.Instance.EquipComposeDatas.Find(x=>x.Index==(SelectItemInfo.ItemId));
            if(composedata==null|| composedata.UpgradeItem < 0)
            {//아마초월로보낼듯
                return;
            }
            EquipInfoData Targetitemdata = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(composedata.UpgradeItem));
            Targetitembox.SetData(Targetitemdata);
            Targetitembox.SetComposeDataText(Targetitemdata);
            TargeItemName.text = LocalizeManager.Instance.GetTXT(Targetitemdata.nameTextID);
            TargeItemName.color = ItemSystem.GetGradeColor(Targetitemdata.Grade);
            GradeIcon.sprite = ItemSystem.GetGradeiconSprite(Targetitemdata.Grade);
            for (int i=0;i< needselfslotlist.Count; i++)
            {
                needselfslotlist[i].gameObject.SetActive(false);
            }
            needselfslotlist[0].SetData(SelectItemInfo);
            needselfslotlist[0].gameObject.SetActive(true);
            NeedNotifyText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_COMPOSE_NEED_INFO"), composedata.ResourceCount);
        
            EnableItem enableItem = UserGameData.Get().GetEnableItem(SelectItemInfo.ItemId);
            if (enableItem != null&& enableItem.num>= composedata.ResourceCount)
            {
                SelectCount = 1;
                NeedSelfText.text = composedata.ResourceCount.ToString();
                SelectCountText.text = SelectCount.ToString();
                BtnCover.SetActive(false);
            }
            else
            {
                SelectCount = 0;
                NeedSelfText.text = "0";
                SelectCountText.text = "0";
                BtnCover.SetActive(true);
            }
            if (composedata.Costitemid > 0)
            {
                ItemInfoData costitem = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == composedata.Costitemid).FirstOrDefault();
                costitemimage.sprite = ItemSystem.GetItemSprite(costitem.icon);
                costitemimage.gameObject.SetActive(true);
            }
            else
                costitemimage.gameObject.SetActive(false);
            NeedCostText.text = composedata.Costitemval.ToString();
            Bottomobj.SetActive(true);
        }
    }

    public void OnClick_UpBtn()
    {
        if (composedata != null)
        {
            EnableItem costitem = UserGameData.Get().GetEnableItem(SelectItemInfo.ItemId);
            if (costitem != null && composedata.ResourceCount * (SelectCount + 1) <= costitem.num)
            {
                SelectCount++;
                SelectCountText.text = SelectCount.ToString();
                NeedSelfText.text = (composedata.ResourceCount * SelectCount).ToString();
            }
        }
    }
    public void OnClick_DownBtn()
    {
        if (composedata != null)
        {
            if (SelectCount > 2)
            {
                SelectCount--;
                SelectCountText.text = SelectCount.ToString();
                NeedSelfText.text = (composedata.ResourceCount * SelectCount).ToString();
            }
        }
    }
    public void OnClick_MAXBtn()
    {
        if (composedata != null)
        {
            EnableItem costitem = UserGameData.Get().GetEnableItem(SelectItemInfo.ItemId);
            if (costitem != null)
            {
                int maxnum = costitem.num / composedata.ResourceCount;
                if (maxnum > 0)
                {
                    SelectCount = maxnum;
                    SelectCountText.text = SelectCount.ToString();
                    NeedSelfText.text = (composedata.ResourceCount * SelectCount).ToString();
                }
            }
        }
    }
    public void OnClick_MINBtn()
    {
        if (composedata != null)
        {
            EnableItem costitem = UserGameData.Get().GetEnableItem(SelectItemInfo.ItemId);
            if (costitem != null && composedata.ResourceCount  <= costitem.num)
            {
                SelectCount = 1;
                SelectCountText.text = SelectCount.ToString();
                NeedSelfText.text = (composedata.ResourceCount * SelectCount).ToString();
            }
        }
    }

    public void OnClick_SelectAllCompose()
    {
        selectType = _owner.GrowthItemListInfo.GetNowSeletItemType();
        string titleText = "";

        UserGameData data = UserGameData.Get();
        List<EnableItem> enableList = data.GetEnableItems();

        for (int i = 0; i < enableList.Count; i++)
        {
            EquipInfoData equipInfo = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(enableList[i].Index));
            if (equipInfo.itemType != selectType || enableList[i].num <= 0)
            {
                continue;
            }

            EquipComposeData resultInfo = GetEuipData(equipInfo.ItemId);
            if (resultInfo != null)
            {
                if (enableList[i].num >= resultInfo.ResourceCount)
                {
                    if (resultInfo.UpgradeItem < 0)
                    {
                        continue;
                    }
                    composableItemList.Add(enableList[i]);
                }
            }
        }

        if (composableItemList.Count > 0)
        {
            switch (selectType)
            {
                case ITEM_TYPE.WEAPON:
                    {
                        titleText = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_BATCHCOMPOSE_WP");
                    }
                    break;
                case ITEM_TYPE.SHIELD:
                    {
                        titleText = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_BATCHCOMPOSE_SD");
                    }
                    break;
                case ITEM_TYPE.ACCESSARY:
                    {
                        titleText = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_BATCHCOMPOSE_AC");
                    }
                    break;
                default:
                    {
                        titleText = LocalizeManager.Instance.GetTXT("");
                    }
                    break;
            }
 
            if (PopupManager.Instance.IsPopupCheck(PopupType.PopupCommonMessage) == false)
            {
                PopupManager.Instance.CreatePopup(PopupType.PopupCommonMessage, true, _popup =>
                {
                    PopupCommonMessage _msg = _popup.GetComponent<PopupCommonMessage>();
                    _msg.Init(1, titleText, LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_BATCHCOMPOSE_INFO"), LocalizeManager.Instance.GetTXT("STR_UI_CONFIRM"), LocalizeManager.Instance.GetTXT("STR_UI_CANCEL"), _callBack =>
                    {
                        if (_callBack)
                            Process_AllCompose();
                        else
                            composableItemList.Clear();
                    });
                });
            }
        }
        else
        {
            UIManager.Instance.CreatePopMessage(PopMessageType.SimpleMessage, _popMsg =>
            {
                _popMsg.GetComponent<PopSimpleMessage>().Init(null, PopSimpleMessageType.PopSimpleMsg, "STR_MSG_EQUIPCOMPOSENOTENOUGH");
            });
        }
    }

    public void Process_AllCompose()
    {
        UIManager.Instance.StartCoroutine(Process_AllComposeAsync());//팝업매니저로바꿔야됨
    }

    private IEnumerator Process_AllComposeAsync()
    {
        UserGameData data = UserGameData.Get();
        Dictionary<int, int> _DicComposeResult = new Dictionary<int, int>();
        PopupResultPage _resultPage = null;
        Dictionary<int, int> _DicComposeItem = new Dictionary<int, int>();
        //일괄합성수정
        if (composableItemList.Count <= 0)
        {
            yield break;
        }

        string state = "";
        int count = 0;
        
        for (int i = 0; i < composableItemList.Count; i++)
        {
            EquipComposeData resultInfo =GetEuipData(composableItemList[i].Index);
            if (resultInfo != null)
            {
                if (composableItemList[i].num >= resultInfo.ResourceCount)
                {
                    EquipInfoData upgradeEquipInfo = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(resultInfo.UpgradeItem));

                    if (_DicComposeResult.ContainsKey(composableItemList[i].Index))
                    {
                        _DicComposeResult[composableItemList[i].Index] = composableItemList[i].num / resultInfo.ResourceCount;
                    }
                    else
                    {
                        _DicComposeResult.Add(composableItemList[i].Index, composableItemList[i].num / resultInfo.ResourceCount);
                    }

                    ItemSystem.AddEquipItem_NoModify(composableItemList[i].Index, -1 * resultInfo.ResourceCount * _DicComposeResult[composableItemList[i].Index]);

                    UserGameData.Get().Notify_InventoryItemChangeInfo(composableItemList[i].Index);
                    if (_DicComposeItem.ContainsKey(composableItemList[i].Index))
                    {
                        _DicComposeItem[composableItemList[i].Index] += resultInfo.ResourceCount * _DicComposeResult[composableItemList[i].Index];
                    }
                    else
                    {
                        _DicComposeItem.Add(composableItemList[i].Index, resultInfo.ResourceCount * _DicComposeResult[composableItemList[i].Index]);
                    }
                }
            }
        }

        if (PopupManager.Instance.IsPopupCheck(PopupType.PopupResultPage) == false)
        {
            PopupManager.Instance.CreatePopup(PopupType.PopupResultPage, true, _popup =>
            {
                _resultPage = _popup.GetComponent<PopupResultPage>();
                _resultPage.Init(null);

                foreach (int key in _DicComposeResult.Keys)
                {
                    EquipComposeData resultInfo = GetEuipData(key);
                    if (resultInfo != null)
                    {
                        _resultPage.AddItemGradeBox(UserManager.Instance.EquipInfoDatas.Find(x => x.ItemId == key), _DicComposeResult[key]);
                    }
                }

                for (int i = 0; i < composableItemList.Count; i++)
                {
                    EquipComposeData resultInfo = GetEuipData(composableItemList[i].Index);
                    if (resultInfo != null)
                    {
                        if (composableItemList[i].num >= resultInfo.ResourceCount)
                        {
                            EquipInfoData upgradeEquipInfo = UserManager.Instance.EquipInfoDatas.Find(x => x.ItemId == (resultInfo.UpgradeItem));
                            _resultPage.AddItemGradeBox(upgradeEquipInfo, _DicComposeResult[composableItemList[i].Index]);
                        }
                    }
                }

                composableItemList.Clear();

                _resultPage.SetData(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_BATCHCOMPOSE_RESULT"));
                _resultPage.OpenWithPlayani();

                _resultPage.disabledAction = () =>
                {
                    if (_DicComposeItem.Count > 0)
                    {
                        state = "";
                        foreach (int index in _DicComposeItem.Keys)
                        {
                            EquipInfoData equipInfo = UserManager.Instance.EquipInfoDatas.Find(x => x.ItemId == (index));
                            EquipComposeData resultInfo = GetEuipData(equipInfo.ItemId);
                            EnableItem enableItem = UserGameData.Get().GetEnableItem(index);
                            if (enableItem == null)
                                state += equipInfo.ItemId + ":" + (0) + "-" + _DicComposeItem[index] + "Get" + resultInfo.UpgradeItem + "#";
                            else
                                state += equipInfo.ItemId + ":" + (enableItem.num + resultInfo.ResourceCount * _DicComposeItem[index]) + "-" + _DicComposeItem[index]
                                    + "Get" + resultInfo.UpgradeItem + "#";

                        }

                        NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "REMOVEALLCOPOSEEQUIPITEM", state,
                            "ComposeItemType:" + selectType.ToString());
                    }

                    if (_DicComposeResult.Count > 0)
                    {
                        UIItemGrowthBase originPanel = _owner.NowPanel;
                        _owner.SelectGrowPanel(eItemGrowthType.COMPOSE);
                        if (selectType.Equals(ITEM_TYPE.WEAPON))
                        {
                            if (_owner.eventItemGradeList[ITEM_TYPE.WEAPON] == null)
                                _owner.eventItemGradeList[ITEM_TYPE.WEAPON] = new Dictionary<ITEM_GRADE, int>();

                            foreach (int key in _DicComposeResult.Keys)
                            {
                                EquipInfoData upgradeEquipInfo = UserManager.Instance.EquipInfoDatas.Find(x => x.ItemId == (GetEuipData(key).UpgradeItem));
                                ItemSystem.AddEquipItem_NoModify(upgradeEquipInfo.ItemId, _DicComposeResult[key]);
                                UserGameData.Get().Notify_InventoryItemChangeInfo(upgradeEquipInfo.ItemId);

                                if (_owner.eventItemGradeList[selectType].ContainsKey(upgradeEquipInfo.Grade))
                                {
                                    _owner.eventItemGradeList[selectType][upgradeEquipInfo.Grade] += _DicComposeResult[key];
                                }
                                else
                                {
                                    _owner.eventItemGradeList[selectType].Add(upgradeEquipInfo.Grade, _DicComposeResult[key]);
                                }
                                count += _DicComposeResult[key];

                                EnableItem enableItem = UserGameData.Get().GetEnableItem(upgradeEquipInfo.ItemId);

                            }

                            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPCOMPOSE, count);
                            UserManager.Instance.SetQuestCount(eQuestType.EQUIPCOMPOSE, count);
                        }
                        else if (selectType.Equals(ITEM_TYPE.SHIELD))
                        {
                            if (_owner.eventItemGradeList[ITEM_TYPE.SHIELD] == null)
                                _owner.eventItemGradeList[ITEM_TYPE.SHIELD] = new Dictionary<ITEM_GRADE, int>();

                            foreach (int key in _DicComposeResult.Keys)
                            {
                                EquipInfoData upgradeEquipInfo = UserManager.Instance.EquipInfoDatas.Find(x => x.ItemId == (GetEuipData(key).UpgradeItem));
                                ItemSystem.AddEquipItem_NoModify(upgradeEquipInfo.ItemId, _DicComposeResult[key]);
                                UserGameData.Get().Notify_InventoryItemChangeInfo(upgradeEquipInfo.ItemId);

                                if (_owner.eventItemGradeList[selectType].ContainsKey(upgradeEquipInfo.Grade))
                                {
                                    _owner.eventItemGradeList[selectType][upgradeEquipInfo.Grade] += _DicComposeResult[key];
                                }
                                else
                                {
                                    _owner.eventItemGradeList[selectType].Add(upgradeEquipInfo.Grade, _DicComposeResult[key]);
                                }
                                count += _DicComposeResult[key];

                                EnableItem enableItem = UserGameData.Get().GetEnableItem(upgradeEquipInfo.ItemId);
                                if (enableItem == null)
                                    state += upgradeEquipInfo.ItemId + ":" + (0) + "+" + _DicComposeResult[key] + "#";
                                else
                                    state += upgradeEquipInfo.ItemId + ":" + (enableItem.num - _DicComposeResult[key]) + "+" + _DicComposeResult[key] + "#";
                            }

                            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPCOMPOSE, count);
                            UserManager.Instance.SetQuestCount(eQuestType.EQUIPCOMPOSE, count);
                        }
                        else if (selectType.Equals(ITEM_TYPE.ACCESSARY))
                        {
                            if (_owner.eventItemGradeList[ITEM_TYPE.ACCESSARY] == null)
                                _owner.eventItemGradeList[ITEM_TYPE.ACCESSARY] = new Dictionary<ITEM_GRADE, int>();

                            foreach (int key in _DicComposeResult.Keys)
                            {
                                EquipInfoData upgradeEquipInfo = UserManager.Instance.EquipInfoDatas.Find(x => x.ItemId == (GetEuipData(key).UpgradeItem));
                                ItemSystem.AddEquipItem_NoModify(upgradeEquipInfo.ItemId, _DicComposeResult[key]);
                                UserGameData.Get().Notify_InventoryItemChangeInfo(upgradeEquipInfo.ItemId);

                                if (_owner.eventItemGradeList[selectType].ContainsKey(upgradeEquipInfo.Grade))
                                {
                                    _owner.eventItemGradeList[selectType][upgradeEquipInfo.Grade] += _DicComposeResult[key];
                                }
                                else
                                {
                                    _owner.eventItemGradeList[selectType].Add(upgradeEquipInfo.Grade, _DicComposeResult[key]);
                                }
                                count += _DicComposeResult[key];

                                EnableItem enableItem = UserGameData.Get().GetEnableItem(upgradeEquipInfo.ItemId);
                                if (enableItem == null)
                                    state += upgradeEquipInfo.ItemId + ":" + (0) + "+" + _DicComposeResult[key] + "#";
                                else
                                    state += upgradeEquipInfo.ItemId + ":" + (enableItem.num - _DicComposeResult[key]) + "+" + _DicComposeResult[key] + "#";
                            }

                            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPCOMPOSE, count);
                            UserManager.Instance.SetQuestCount(eQuestType.EQUIPCOMPOSE, count);
                        }

                        foreach (ITEM_TYPE _key in _owner.eventItemGradeList.Keys)
                        {
                            if (_key == ITEM_TYPE.WEAPON)
                            {
                                UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONGRADE] = true;
                                foreach (ITEM_GRADE _grade in _owner.eventItemGradeList[_key].Keys)
                                {
                                    UserManager.Instance.SetEventCount(eQuestType.WEAPONGRADE, _owner.eventItemGradeList[_key][_grade], (int)_grade);
                                }
                                UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONGRADE] = false;
                            }
                            else if (_key == ITEM_TYPE.SHIELD)
                            {
                                UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDGRADE] = true;
                                foreach (ITEM_GRADE _grade in _owner.eventItemGradeList[_key].Keys)
                                {
                                    UserManager.Instance.SetEventCount(eQuestType.SHIELDGRADE, _owner.eventItemGradeList[_key][_grade], (int)_grade);
                                }
                                UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDGRADE] = false;
                            }
                            else if (_key == ITEM_TYPE.ACCESSARY)
                            {
                                UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYGRADE] = true;
                                foreach (ITEM_GRADE _grade in _owner.eventItemGradeList[_key].Keys)
                                {
                                    UserManager.Instance.SetEventCount(eQuestType.ACCESSARYGRADE, _owner.eventItemGradeList[_key][_grade], (int)_grade);
                                }
                                UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYGRADE] = false;
                            }

                            _owner.eventItemGradeList[_key].Clear();
                        }

                        if (!string.IsNullOrEmpty(state))
                        {
                            NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "GETALLCOPOSEEQUIPITEM", state,
                               "ComposeItemType:" + selectType.ToString() + " GetItemtypeCount:" + _DicComposeResult.Count);
                        }
                        UserGameData.Get().ModifyEquipItemData(selectType);
                        _owner.SelectGrowPanel(originPanel.GetGrowthType());
                        _owner.GrowthItemListInfo.SelectItemSortType(selectType);
                        _owner.ItemGrowthPopup.Close();
                    }
                };
            });
        }
    }

    /// <summary>
    /// 합성
    /// </summary>
    /// <param name="type">0.한개 1.선택아이템모두 2.일괄</param>
    public void Process_Compose()
    {
        if (SelectItemInfo == null)
            return;
        EquipComposeData composedata =PopupManager.Instance.EquipComposeDatas.Find(x=>x.Index==(SelectItemInfo.ItemId));
        if (composedata == null || composedata.UpgradeItem < 0)
        {//아마초월로보낼듯
            UIManager.Instance.CreatePopMessage(PopMessageType.SimpleMessage, _popMsg =>
            {
                _popMsg.GetComponent<PopSimpleMessage>().Init(null, PopSimpleMessageType.PopSimpleMsg, "STR_MSG_EQUIPCOMPOSENOTNEXT");
            });
            return;
        }
        EnableItem CostEquipitem = UserGameData.Get().GetEnableItem(SelectItemInfo.ItemId);
        long beforepow = 0;
        if (CharacterManager.Instance.MyActor != null && CharacterManager.Instance.MyActor.stat.CurHP > 0)
            beforepow = (long)CharacterManager.Instance.MyActor.stat.GetBattlePower();
        if (CostEquipitem == null)
        {
            UIManager.Instance.CreatePopMessage(PopMessageType.SimpleMessage, _popMsg =>
            {
                _popMsg.GetComponent<PopSimpleMessage>().Init(null, PopSimpleMessageType.PopSimpleMsg, "STR_MSG_NOTENOUGHITEM");
            });
            return;
        }
        if (CostEquipitem.num < composedata.ResourceCount * SelectCount)
        {
            UIManager.Instance.CreatePopMessage(PopMessageType.SimpleMessage, _popMsg =>
            {
                _popMsg.GetComponent<PopSimpleMessage>().Init(null, PopSimpleMessageType.PopSimpleMsg, "STR_MSG_EQUIPCOMPOSENOTENOUGH");
            });
            return;
        }
        if (SelectCount <= 0)
        {
            UIManager.Instance.CreatePopMessage(PopMessageType.SimpleMessage, _popMsg =>
            {
                _popMsg.GetComponent<PopSimpleMessage>().Init(null, PopSimpleMessageType.PopSimpleMsg, "STR_MSG_NOTENOUGHITEM");
            });
            return;
        }
        ItemSystem.AddEquipItem(CostEquipitem.Index, -composedata.ResourceCount * SelectCount);

        EnableItem useritem = UserGameData.Get().GetEnableItem(CostEquipitem.Index);
        EquipInfoData equipinfo = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(CostEquipitem.Index));
        string getstate = equipinfo.itemType + "/" + useritem.Index + ":" + (useritem.num + composedata.ResourceCount * SelectCount) + "-" + composedata.ResourceCount * SelectCount;

        NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "COMPOSITEM", getstate,
           "composedataindex" + composedata.Index + "NeedresourceCount/" + composedata.ResourceCount + " SelectCount" + SelectCount);

        ItemSystem.AddEquipItem(composedata.UpgradeItem, SelectCount);

        EnableItem Getitem = UserGameData.Get().GetEnableItem(composedata.UpgradeItem);
        EquipInfoData Getiteminfo = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(composedata.UpgradeItem));
        string state = Getiteminfo.itemType + "/" + Getitem.Index + ":" + (Getitem.num - SelectCount) + "+" + SelectCount;

        NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "COMPOSGETITEM", state,
            " SelectCount" + SelectCount);

        if (Getiteminfo.itemType.Equals(ITEM_TYPE.WEAPON))
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONGRADE] = true;
            _owner.eventItemGradeList[Getiteminfo.itemType].Add(Getiteminfo.Grade, SelectCount);
            UserManager.Instance.SetEventCount(eQuestType.WEAPONGRADE, _owner.eventItemGradeList[ITEM_TYPE.WEAPON][Getiteminfo.Grade], (int)Getiteminfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONGRADE] = false;
            _owner.eventItemGradeList[Getiteminfo.itemType].Clear();
        }
        else if (Getiteminfo.itemType.Equals(ITEM_TYPE.SHIELD))
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDGRADE] = true;
            _owner.eventItemGradeList[Getiteminfo.itemType].Add(Getiteminfo.Grade, SelectCount);
            UserManager.Instance.SetEventCount(eQuestType.SHIELDGRADE, _owner.eventItemGradeList[ITEM_TYPE.SHIELD][Getiteminfo.Grade], (int)Getiteminfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDGRADE] = false;
            _owner.eventItemGradeList[Getiteminfo.itemType].Clear();
        }
        else if (Getiteminfo.itemType.Equals(ITEM_TYPE.ACCESSARY))
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYGRADE] = true;
            _owner.eventItemGradeList[Getiteminfo.itemType].Add(Getiteminfo.Grade, SelectCount);
            UserManager.Instance.SetEventCount(eQuestType.ACCESSARYGRADE, _owner.eventItemGradeList[ITEM_TYPE.ACCESSARY][Getiteminfo.Grade], (int)Getiteminfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYGRADE] = false;
            _owner.eventItemGradeList[Getiteminfo.itemType].Clear();
        }

        UserManager.Instance.SetQuestCount(eQuestType.EQUIPCOMPOSE, SelectCount);
        UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPCOMPOSE, SelectCount);

        _owner.GrowthItemListInfo.SetUpdateDirty(new sUpdateItemDirtyInfo() { ItemIndex = SelectItemInfo.ItemId, type = SelectItemInfo.itemType });
        _owner.GrowthItemListInfo.SetUpdateDirty(new sUpdateItemDirtyInfo() { ItemIndex = Getiteminfo.ItemId, type = Getiteminfo.itemType });
        _owner.ItemGrowthPopup.OpenComposeResult(SelectCount);
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

    #region Tutorial Coroutine
    public IEnumerator Gear_ComposeTutorial4()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        if (this.gameObject.activeSelf == false)
        {
            yield break;
        }

        TutorialManager.Instance.SetUIGuidePosition(Bottomobj.GetComponentInChildren<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (UserGameData.Get().GetEnableItem(5) == null)
        {
            yield return new WaitForEndOfFrame();
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

       // yield return UIItemGrowthListInfo.Get().Gear_ComposeTutorial5();
    }

    #endregion Tutorial Coroutine
    public EquipComposeData GetEuipData(int _index)
    {
        EquipComposeData info = PopupManager.Instance.EquipComposeDatas.Where(n => n.EquipID == _index).FirstOrDefault();
        return info;
    }
}
