using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemGrowth_Awake : UIItemGrowthBase
{
    public override eItemGrowthType GetGrowthType() { return eItemGrowthType.AWAKE; }
    [SerializeField] PopupItemBox Targetitembox;
    [SerializeField] Text NeedCostText;
    [SerializeField] GameObject Bottomobj;
    [SerializeField] GameObject GradeBG;
    [SerializeField] ScrollRect _scroll;
    [SerializeField] UIItemCountSlot needitemcountslot;
    [SerializeField] GameObject BackObj;
    [SerializeField] Image GradeImage;
    [SerializeField] Image TargetTypeImage;
    [SerializeField] Text CountText;
    [SerializeField] GameObject CoverObj;
    [SerializeField] Text NotifyText;
    List<UIItemCountSlot> NeeditemcountslotList;
    EquipInfoData SelectItemInfo = null;
    public override void OpenPanel(UIItemSlot _selectItemSlot = null)
    {
        base.OpenPanel(_selectItemSlot);
        BackObj.SetActive(true);
        GradeBG.SetActive(true);
        if (_selectItemSlot != null)
        {
            Bottomobj.SetActive(true);
            GradeImage.sprite = ItemSystem.GetGradeSprite(ITEM_GRADE.ANCIENT);
            TargetTypeImage.sprite = ItemSystem.GetItemSubTypeSprite(_selectItemSlot.EquipDataInfo.itemType, _selectItemSlot.EquipDataInfo.qualityIndex);
            string subtype = ItemSystem.GetItemSubTypeTitelText(_selectItemSlot.EquipDataInfo.itemType, _selectItemSlot.EquipDataInfo.qualityIndex);
            NotifyText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_FORGE_INFO"), subtype);
            
            RefreshUI();
        }
    }
    public override void ClosePanel()
    {
        base.ClosePanel();
        if (NeeditemcountslotList == null)
            NeeditemcountslotList = new List<UIItemCountSlot>();
        for (int i = 0; i < NeeditemcountslotList.Count; i++)
        {
            NeeditemcountslotList[i].gameObject.SetActive(false);
        }
        GradeBG.SetActive(false);
        Bottomobj.SetActive(false);

    }
    void AddSlot(EquipInfoData item,int count)
    {
        if (NeeditemcountslotList == null)
            NeeditemcountslotList = new List<UIItemCountSlot>();
        for (int i=0;i< NeeditemcountslotList.Count; i++)
        {
            if (!NeeditemcountslotList[i].gameObject.activeSelf)
            {
                NeeditemcountslotList[i].SetData(item, count);
                NeeditemcountslotList[i].gameObject.SetActive(true);
                return;
            }
        }
        GameObject go = Instantiate(needitemcountslot.gameObject, _scroll.content);
        go.SetActive(true);

        UIItemCountSlot slot = go.GetComponent<UIItemCountSlot>();
        slot.SetData(item, count);
        NeeditemcountslotList.Add(slot);
    }
    void AddSlot(ItemInfoData item, int count)
    {
        if (NeeditemcountslotList == null)
            NeeditemcountslotList = new List<UIItemCountSlot>();
        for (int i = 0; i < NeeditemcountslotList.Count; i++)
        {
            if (!NeeditemcountslotList[i].gameObject.activeSelf)
            {
                NeeditemcountslotList[i].SetData(item, count);
                NeeditemcountslotList[i].gameObject.SetActive(true);
                return;
            }
        }
        GameObject go = Instantiate(needitemcountslot.gameObject, _scroll.content);
        go.SetActive(true);
        UIItemCountSlot slot = go.GetComponent<UIItemCountSlot>();
        slot.SetData(item, count);
        NeeditemcountslotList.Add(slot);
    }

    public void OnClick_AwakeBtn()
    {

        EquipAwakeData awakeinfo = PopupManager.Instance.EquipAwakeDatas.Where(x => x.Equipid == SelectItemSlot.EquipDataInfo.ItemId).FirstOrDefault();
        bool checkcostval = false;
        ITEM_TYPE equipItemType = SelectItemSlot.EquipDataInfo.itemType;
        for (int i = 0; i < awakeinfo.CostList.Count; i++)
        {
            switch (awakeinfo.CostList[i].Itemtype)
            {
                case ITEM_TYPE.ACCESSARY:
                case ITEM_TYPE.WEAPON:
                case ITEM_TYPE.SHIELD:
                    {
                        EnableItem costitem = UserGameData.Get().GetEnableItem(awakeinfo.CostList[i].Itemid);
                        if (costitem == null)
                        {
                            checkcostval = false;
                            break;
                        }
                        if (costitem.num >= awakeinfo.CostList[i].Costcount)
                        {
                            checkcostval = true;
                        }
                        else
                        {
                            checkcostval = false;
                            break;
                        }
                    }
                    break;
                default:
                    {
                        ItemClass costitem = UserGameData.Get().GetItemClass(awakeinfo.CostList[i].Itemid);
                        if (costitem == null)
                        {
                            checkcostval = false;
                            break;
                        }
                        if (costitem.num >= awakeinfo.CostList[i].Costcount)
                        {
                            checkcostval = true;
                        }
                        else
                        {
                            checkcostval = false;
                            break;
                        }

                    }
                    break;
            }
        }
        if (!checkcostval)
            return;
        for (int i = 0; i < awakeinfo.CostList.Count; i++)
        {
            switch (awakeinfo.CostList[i].Itemtype)
            {
                case ITEM_TYPE.ACCESSARY:
                case ITEM_TYPE.WEAPON:
                case ITEM_TYPE.SHIELD:
                    {
                        EnableItem costitem = UserGameData.Get().GetEnableItem(awakeinfo.CostList[i].Itemid);
                        if (costitem.num >= awakeinfo.CostList[i].Costcount)
                        {
                            ItemSystem.AddEquipItem(awakeinfo.CostList[i].Itemid, -awakeinfo.CostList[i].Costcount);
                            EquipInfoData equipinfo = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(costitem.Index));
                            string getstate = equipinfo.itemType + "/" + costitem.Index + ":" + (costitem.num + awakeinfo.CostList[i].Costcount) + "-" + awakeinfo.CostList[i].Costcount;

                            NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "AWAKECOSTITEM", getstate,
                               "AwakeinfoIndex/" + awakeinfo.Index);
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                default:
                    {
                        ItemClass costitem = UserGameData.Get().GetItemClass(awakeinfo.CostList[i].Itemid);
                        if (costitem.num >= awakeinfo.CostList[i].Costcount)
                        {
                            ItemSystem.AddItem(awakeinfo.CostList[i].Itemid, -awakeinfo.CostList[i].Costcount);
                            ItemInfoData iteminfo = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==costitem.Index).FirstOrDefault();
                            string getstate = "";
                            if (iteminfo.itemType == ITEM_TYPE.GOLD)
                            {
                                getstate = "Gold"+ ":" + (UserGameData.Get().AssetGold+ awakeinfo.CostList[i].Costcount) + "-" + awakeinfo.CostList[i].Costcount;

                            }
                            else if (iteminfo.itemType == ITEM_TYPE.PEARL)
                            {
                                getstate = "Pearl" + ":" + (UserGameData.Get().AssetPearl + awakeinfo.CostList[i].Costcount) + "-" + awakeinfo.CostList[i].Costcount;
                            }
                            else
                            {
                                getstate = iteminfo.itemType + "/" + costitem.Index + ":" + (costitem.num + awakeinfo.CostList[i].Costcount) + "-" + awakeinfo.CostList[i].Costcount;
                            }
                            NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "AWAKECOSTITEM", getstate,
                               "AwakeinfoIndex/" + awakeinfo.Index);
                        }
                        else
                        {
                            return;
                        }

                    }
                    break;
            }
        }

        EnableItem _selectItem = UserManager.Instance.UserGameDataInfo.myEquipItems.Find(x => x.Index == SelectItemSlot.EquipDataInfo.ItemId);
        if (_selectItem != null)
        {
            _selectItem.isUpgrade = UserManager.Instance.UserGameDataInfo.GetCanUpgradeEquip(_selectItem);
            UserManager.Instance.UserGameDataInfo.CheckEquipBadge();
            SelectItemSlot.ReInit();
        }

        BoxInfoData boxinfo = PopupManager.Instance.BoxInfoDatas.Where(x => x.Index == awakeinfo.GetItemBoxId).FirstOrDefault();
        PopupManager.Instance.GetBoxlistGroupData(boxinfo.ItemListID, out List<BoxListData> boxlistinfo);
        int maxrate = 0;
        int nowrate = 0;
        
        for(int i=0;i< boxlistinfo.Count; i++)
        {
            maxrate += boxlistinfo[i].rate;
        }
        int rate = UnityEngine.Random.Range(0, maxrate);
        Debug.Log(rate);

        for (int i = 0; i < boxlistinfo.Count; i++)
        {
            nowrate += boxlistinfo[i].rate;
            if (rate < nowrate)
            {
                EquipInfoData getiteminfo = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(boxlistinfo[i].itemID));
                ItemSystem.AddEquipItem(getiteminfo.ItemId, 1);
                _owner.ItemGrowthPopup.OpenAwakeResult(getiteminfo);

                EnableItem useritem = UserGameData.Get().GetEnableItem(getiteminfo.ItemId);
                string getstate = getiteminfo.itemType + "/" + useritem.Index + ":" + (useritem.num - 1) + "+" + 1;

                NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "AWAKEITEM", getstate,
                   "AwakeinfoIndex/" + awakeinfo.Index);
                break;
            }
        }

        if (equipItemType.Equals(ITEM_TYPE.WEAPON))
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONAWAKEN] = true;
            UserManager.Instance.SetEventCount(eQuestType.WEAPONAWAKEN, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.WEAPONAWAKEN] = false;
        }
        else if (equipItemType.Equals(ITEM_TYPE.SHIELD))
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDAWAKEN] = true;
            UserManager.Instance.SetEventCount(eQuestType.SHIELDAWAKEN, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.SHIELDAWAKEN] = false;
        }
        else if (equipItemType.Equals(ITEM_TYPE.ACCESSARY))
        {
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYAWAKEN] = true;
            UserManager.Instance.SetEventCount(eQuestType.ACCESSARYAWAKEN, 1, (int)SelectItemSlot.EquipDataInfo.Grade);
            UserGameData.Get()._DicNotifiedEventFlag[eQuestType.ACCESSARYAWAKEN] = false;
        }

        UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPCRAFT);
        
    }
    void RefreshUI()
    {
        if (NeeditemcountslotList == null)
            NeeditemcountslotList = new List<UIItemCountSlot>();
        for (int i = 0; i < NeeditemcountslotList.Count; i++)
        {
            NeeditemcountslotList[i].gameObject.SetActive(false);
        }
        EquipAwakeData awakeinfo = PopupManager.Instance.EquipAwakeDatas.Where(x => x.Equipid == _owner.SelectedItemSlot.EquipDataInfo.ItemId).FirstOrDefault();


        CoverObj.SetActive(false);

        if (awakeinfo != null)
        {
            for (int i = 0; i < awakeinfo.CostList.Count; i++)
            {
                switch (awakeinfo.CostList[i].Itemtype)
                {
                    case ITEM_TYPE.ACCESSARY:
                    case ITEM_TYPE.WEAPON:
                    case ITEM_TYPE.SHIELD:
                        {
                            EquipInfoData info = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(awakeinfo.CostList[i].Itemid));
                            AddSlot(info, awakeinfo.CostList[i].Costcount);
                            EnableItem item = UserGameData.Get().GetEnableItem(awakeinfo.CostList[i].Itemid);
                            if (item == null || item.num < awakeinfo.CostList[i].Costcount)
                            {
                                CoverObj.SetActive(true);
                            }
                        }
                        break;
                    default:
                        {
                            ItemInfoData info = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == awakeinfo.CostList[i].Itemid).FirstOrDefault();
                            AddSlot(info, awakeinfo.CostList[i].Costcount);
                            ItemClass item = UserGameData.Get().GetItemClass(awakeinfo.CostList[i].Itemid);
                            if (item == null || item.num < awakeinfo.CostList[i].Costcount)
                            {
                                CoverObj.SetActive(true);
                            }
                        }
                        break;

                }
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateDirty == true)
        {
            RefreshUI();
            UpdateDirty = false;
        }
    }
}
