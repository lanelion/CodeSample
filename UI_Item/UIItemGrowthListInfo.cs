using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public enum eItemSortType
{
    WEAPON,     //무기
    SHIELD,     //방패
    ACCESSORIE,  //액세서리
    __MAX__
}

public class UIItemGrowthListInfo : MonoBehaviour
{
    [SerializeField] private ScrollRect ScrollRectRoot = null;

    public Toggle[] ItemSortToggles = null;
    public UIToggleTextController[] ItemSortTogglesController = null; // 바로가기용 배열
    [NonSerialized] public PopupItemGrowth _owner = null;

    private List<UIItemSlot> ItemSlotList;
    public List<UIItemSlot> itemlist
    {
        get { return ItemSlotList; }
    }

    public ITEM_TYPE SelectedItemType = ITEM_TYPE.WEAPON;

    [SerializeField] List<Toggle> W_quilityindextoggle; //장비
    [SerializeField] List<Toggle> S_quilityindextoggle; //보조무기
    [SerializeField] List<Toggle> A_quilityindextoggle; //장신구
    [SerializeField] List<GameObject> ToggleGroupObjs;
    [SerializeField] Toggle ComposAllToggle;

    [SerializeField] GameObject WeapontapNotifyObj;
    [SerializeField] GameObject ShieldtapNotifyObj;
    [SerializeField] GameObject AccessorytapNotifyObj;

    [SerializeField] WeaponItemController weaponItemController;
    [SerializeField] ShieldItemController shieldItemController;
    [SerializeField] AccessaryItemController accessaryItemController;

    private List<int> UpdatedItemList = new List<int>();
    [NonSerialized] public int WeaponSeletindex = -1;
    [NonSerialized] public int ShieldSeletindex = -1;
    [NonSerialized] public int AccessorySeletindex = -1;
    [NonSerialized] public int SelectqualityIndex = -1;

    #region Tutorial Field
    [SerializeField] private GameObject detailEquipBtn;
    [SerializeField] private GameObject detailCloseBtn;
    [SerializeField] private GameObject detailComposeBtn;
    [SerializeField] private GameObject upgradeComposeBtn;

    #endregion Tutorial Field

    public void StartInitialize()
    {
        CreateItemSlots();

        ItemSortToggles[(int)eItemSortType.WEAPON].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
            {
                SelectItemSortType(ITEM_TYPE.WEAPON);
            }
            else
                return;
        });
        ItemSortToggles[(int)eItemSortType.SHIELD].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SelectItemSortType(ITEM_TYPE.SHIELD);
            else
                return;
        });
        ItemSortToggles[(int)eItemSortType.ACCESSORIE].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SelectItemSortType(ITEM_TYPE.ACCESSARY);
            else
                return;
        });
        
        SetSubToggle();

        weaponItemController.setVisibiltyItemSlot = SetVisibiltyItemSlot;
        weaponItemController.init(this);
        shieldItemController.setVisibiltyItemSlot = SetVisibiltyItemSlot;
        shieldItemController.init(this);
        accessaryItemController.setVisibiltyItemSlot = SetVisibiltyItemSlot;
        accessaryItemController.init(this);
    }
    public void Initowner(PopupItemGrowth popupItem)
    {
        _owner = popupItem;
    }
    public void SetSubTypeSort(int quilityindex)
    {
        _owner.SelectEquipData = null;

        switch (SelectedItemType)
        {
            case ITEM_TYPE.WEAPON:
                {
                    ItemSlotList.Clear();
                    SelectqualityIndex = quilityindex;
                    weaponItemController.scroller.ReloadData();

                    for (int i=0;i< W_quilityindextoggle.Count; i++)
                    {
                        if (i != quilityindex)
                            W_quilityindextoggle[i].isOn = false;
                        else
                            W_quilityindextoggle[i].isOn = true;
                    }
                    bool isselect = false;
                    for (int i = 0; i < W_quilityindextoggle.Count; i++)
                    {
                        if (W_quilityindextoggle[i].isOn)
                        {
                            isselect = true;
                            break;
                        }
                    }

                    UIItemSlot _selectSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == WeaponSeletindex);
                    if (_selectSlot != null)
                    {
                        _owner.OnSelectItem(ItemSlotList.Find(x => x.EquipDataInfo.ItemId == WeaponSeletindex));
                    }
                    if (!isselect)
                    {
                        WeaponSeletindex = -1;

                        for (int i = 0; i < ItemSlotList.Count; i++)
                        {
                            if (ItemSlotList[i] == null) continue;
                            
                            ItemSlotList[i].SetSelectedSlot(false);
                        }
                    }
                }
                break;
            case ITEM_TYPE.SHIELD:
                {
                    ItemSlotList.Clear();
                    SelectqualityIndex = quilityindex;
                    shieldItemController.scroller.ReloadData();

                    for (int i = 0; i < S_quilityindextoggle.Count; i++)
                    {
                        if (i != quilityindex)
                            S_quilityindextoggle[i].isOn = false;
                        else
                            S_quilityindextoggle[i].isOn = true;
                    }

                    bool isselect = false;
                    for (int i = 0; i < S_quilityindextoggle.Count; i++)
                    {
                        if (S_quilityindextoggle[i].isOn)
                        {
                            isselect = true;
                            break;
                        }
                    }

                    UIItemSlot _selectSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == ShieldSeletindex);
                    if (_selectSlot != null)
                    {
                        _owner.OnSelectItem(ItemSlotList.Find(x => x.EquipDataInfo.ItemId == ShieldSeletindex));
                    }
                    if (!isselect)
                    {
                        ShieldSeletindex = -1;

                        for (int i = 0; i < ItemSlotList.Count; i++)
                        {
                            ItemSlotList[i].SetSelectedSlot(false);
                        }
                    }
                }
                break;
            case ITEM_TYPE.ACCESSARY:
                {
                    ItemSlotList.Clear();
                    SelectqualityIndex = quilityindex;
                    accessaryItemController.scroller.ReloadData();

                    for (int i = 0; i < A_quilityindextoggle.Count; i++)
                    {
                        if (i != quilityindex)
                            A_quilityindextoggle[i].isOn = false;
                        else
                            A_quilityindextoggle[i].isOn = true;
                    }

                    bool isselect = false;
                    for (int i = 0; i < A_quilityindextoggle.Count; i++)
                    {
                        if (A_quilityindextoggle[i].isOn)
                        {
                            isselect = true;
                            break;
                        }
                    }
                    UIItemSlot _selectSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == AccessorySeletindex);
                    if (_selectSlot != null)
                    {
                        _owner.OnSelectItem(ItemSlotList.Find(x => x.EquipDataInfo.ItemId == AccessorySeletindex));
                    }
                    if (!isselect)
                    {
                        AccessorySeletindex = -1;

                        for (int i = 0; i < ItemSlotList.Count; i++)
                        {
                            ItemSlotList[i].SetSelectedSlot(false);
                        }
                    }
                }
                break;
        }
    }
    public void CheckSubSort()
    {
        _owner.SelectEquipData = null;

        switch (SelectedItemType)
        {
            case ITEM_TYPE.WEAPON:
                {
                    bool isselect = false;
                    for (int i = 0; i < W_quilityindextoggle.Count; i++)
                    {
                        if (W_quilityindextoggle[i].isOn)
                        {
                            SelectqualityIndex = i;
                            isselect = true;
                            break;
                        }
                    }
                    if (!isselect)
                    {
                        WeaponSeletindex = -1;
                        weaponItemController.scroller.ReloadData();

                        for (int i = 0; i < ItemSlotList.Count; i++)
                        {
                            if (ItemSlotList[i] == null) continue;
                            
                            ItemSlotList[i].Init(this);
                            ItemSlotList[i].SetSelectedSlot(false);
                        }
                    }
                }
                break;
            case ITEM_TYPE.SHIELD:
                {
                    bool isselect = false;
                    for (int i = 0; i < S_quilityindextoggle.Count; i++)
                    {
                        if (S_quilityindextoggle[i].isOn)
                        {
                            SelectqualityIndex = i;
                            isselect = true;
                            break;
                        }
                    }
                    if (!isselect)
                    {
                        ShieldSeletindex = -1;
                        shieldItemController.scroller.ReloadData();

                        for (int i = 0; i < ItemSlotList.Count; i++)
                        {
                            ItemSlotList[i].SetSelectedSlot(false);
                            ItemSlotList[i].Init(this);
                        }
                    }
                    
                }
                break;
            case ITEM_TYPE.ACCESSARY:
                {
                    bool isselect = false;
                    for (int i = 0; i < A_quilityindextoggle.Count; i++)
                    {
                        if (A_quilityindextoggle[i].isOn)
                        {
                            SelectqualityIndex = i;
                            isselect = true;
                            break;
                        }
                    }
                    if (!isselect)
                    {
                        AccessorySeletindex = -1;
                        accessaryItemController.scroller.ReloadData();

                        for (int i = 0; i < ItemSlotList.Count; i++)
                        {
                            ItemSlotList[i].SetSelectedSlot(false);
                            ItemSlotList[i].Init(this);
                        }
                    }
                    
                }
                break;
        }
    }
    public void LoadingComplete() 
    {
        _owner.SetNotify = false;
        List<EnableItem> useritemlist = UserGameData.Get().GetEnableItems();
        for (int i = 0; i < useritemlist.Count; i++)
        {
            if (!_owner.SetNotify)
            {
                if (useritemlist[i].isnew)
                {
                    _owner.SetNotify = true;
                }
            }
            if (useritemlist[i].isnew)
            {
                EquipInfoData iteminfo= UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(useritemlist[i].Index));
                SetTapNotice(iteminfo.itemType);
            }
        }

        if (CharacterManager.Instance.MyActor != null)
        {
            CharacterManager.Instance.MyActor.equipawkeakill.UpdateAwakeskill();
        }
    }
    public void SetSubToggle()
    {
        W_quilityindextoggle[0].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(0);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        }); W_quilityindextoggle[1].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(1);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        }); W_quilityindextoggle[2].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(2);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        }); W_quilityindextoggle[3].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)

                SetSubTypeSort(3);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        });

        S_quilityindextoggle[0].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(0);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        }); S_quilityindextoggle[1].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(1);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        });


        A_quilityindextoggle[0].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(0);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        });
        A_quilityindextoggle[1].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(1);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        });
        A_quilityindextoggle[2].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(2);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        });
        A_quilityindextoggle[3].onValueChanged.AddListener((bool ison) =>
        {
            if (ison)
                SetSubTypeSort(3);
            else
            {
                SelectqualityIndex = -1;
                CheckSubSort();
            }
        });
    }

    public void SetUpdateDirty(sUpdateItemDirtyInfo _info)
    {
        if (_info.type == ITEM_TYPE.ACCESSARY || _info.type == ITEM_TYPE.WEAPON || _info.type == ITEM_TYPE.SHIELD)
        {
            UpdatedItemList.Add(_info.ItemIndex);
            UIItemSlot _slot = ItemSlotList.Find(match => { return match.EquipDataInfo.ItemId == _info.ItemIndex; });
            if (_slot != null)
            {
                _slot.ReInit();
                if (_slot.CanSynthesis)
                {
                    if (ComposAllToggle != null)
                        ComposAllToggle.isOn = true;
                }
            }
            EquipInfoData item = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(_info.ItemIndex));
            EnableItem enableItem = UserGameData.Get().GetEnableItem(_info.ItemIndex);
            if (enableItem != null)
            {
                if (enableItem.isnew)
                {
                    if (!_owner.SetNotify)
                        _owner.SetNotify = true;
                    
                    SetTapNotice(item.itemType);
                }
                if (item.itemType == SelectedItemType && (SelectqualityIndex < 0 || SelectqualityIndex == item.qualityIndex))
                {
                    if (_slot != null) _slot.gameObject.SetActive(true);
                }

            }

            if (item != null)
            {
                if (item.skillinfoID > 0 && CharacterManager.Instance.MyActor != null)
                    CharacterManager.Instance.MyActor.equipawkeakill.UpdateAwakeskill();
            }
        }
        
    }

    private void CreateItemSlots()
    {
        ItemSlotList = new List<UIItemSlot>();
        UIItemSlot _slot = null;
        Dictionary<int, EquipInfoData> _container = UserManager.Instance.EquipInfoDatas.ToDictionary(x => x.ItemId, x => x); ;
        List<EquipInfoData> InfoList = new List<EquipInfoData>();

        SelectedItemType = ITEM_TYPE.WEAPON;

        UIManager.Instance.StartCoroutine(NextFrameScrollResetPos());//팝업매니저로바꿔야됨
    }
    
    IEnumerator NextFrameScrollResetPos()
    {
        yield return new WaitForEndOfFrame();
    }

    public void SelectItemSortType(ITEM_TYPE _sortType)
    {
        UserGameData data = UserGameData.Get();
        SelectedItemType = _sortType;
        WeaponSeletindex = -1;
        ShieldSeletindex = -1;
        AccessorySeletindex = -1;
        SelectqualityIndex = -1;
        _owner.SelectEquipData = null;
        if (_owner.NowPanel != null && _owner.NowPanel.GetGrowthType()!= eItemGrowthType.NORAML
            && _owner.NowPanel.GetGrowthType() != eItemGrowthType.DETAIL)
        {
            _owner.SelectGrowPanel(eItemGrowthType.NORAML);
        }

        UserManager.Instance.UserGameDataInfo.CheckEquipBadge();

        switch (SelectedItemType)
        {
            case ITEM_TYPE.WEAPON:
                {
                    if(!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPWP))
                    {
                        _owner.OnClick_WPLock();
                        return;
                    }

                    PlayerPrefs.SetString("SelectItemType", "WEAPON");
                    weaponItemController.gameObject.SetActive(true);
                    shieldItemController.gameObject.SetActive(false);
                    accessaryItemController.gameObject.SetActive(false);

                    WeaponSeletindex = -1;
                    ItemSlotList.Clear();
                    weaponItemController.scroller.ReloadData();

                    ItemSortToggles[(int)eItemSortType.WEAPON].isOn = true;
                    ToggleGroupObjs[0].SetActive(true);
                    ToggleGroupObjs[1].SetActive(false);
                    ToggleGroupObjs[2].SetActive(false);
                    for (int i = 0; i < W_quilityindextoggle.Count; i++)
                    {
                        W_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < S_quilityindextoggle.Count; i++)
                    {
                        S_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < A_quilityindextoggle.Count; i++)
                    {
                        A_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < ItemSlotList.Count; i++)
                    {
                        if (ItemSlotList[i] == null || ItemSlotList[i].EquipDataInfo == null) continue;

                        ItemSlotList[i].Init(this);
                        ItemSlotList[i].SetSelectedSlot(false);
                    }

                    List<EnableItem> _list = UserManager.Instance.UserGameDataInfo.myEquipItems.Where(x => x.isnew && x.Type == ITEM_TYPE.WEAPON).ToList();
                    foreach (EnableItem _item in _list)
                    {
                        _item.isnew = false;
                        UserManager.Instance.UserGameDataInfo.ModifyEquipItemData(_item.Index, _item.num, _item.refining, _item.level, _item.equipped);
                    }

                    UserManager.Instance.UserGameDataInfo.CheckEquipBadge();
                }
                break;
            case ITEM_TYPE.SHIELD:
                {
                    if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPSD))
                    {
                        _owner.OnClick_SDLock();
                        return;
                    }

                    PlayerPrefs.SetString("SelectItemType", "SHIELD");
                    weaponItemController.gameObject.SetActive(false);
                    shieldItemController.gameObject.SetActive(true);
                    accessaryItemController.gameObject.SetActive(false);

                    ShieldSeletindex = -1;
                    ItemSlotList.Clear();
                    shieldItemController.scroller.ReloadData();

                    ToggleGroupObjs[0].SetActive(false);
                    ToggleGroupObjs[1].SetActive(true);
                    ToggleGroupObjs[2].SetActive(false);

                    ItemSortToggles[(int)eItemSortType.SHIELD].isOn = true;
                    for (int i = 0; i < W_quilityindextoggle.Count; i++)
                    {
                        W_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < S_quilityindextoggle.Count; i++)
                    {
                        S_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < A_quilityindextoggle.Count; i++)
                    {
                        A_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < ItemSlotList.Count; i++)
                    {
                        if (ItemSlotList[i] == null || ItemSlotList[i].EquipDataInfo == null) continue;

                        ItemSlotList[i].Init(this);
                        ItemSlotList[i].SetSelectedSlot(false);
                    }

                    List<EnableItem> _list = UserManager.Instance.UserGameDataInfo.myEquipItems.Where(x => x.isnew && x.Type == ITEM_TYPE.SHIELD).ToList();
                    foreach (EnableItem _item in _list)
                    {
                        _item.isnew = false;
                        UserManager.Instance.UserGameDataInfo.ModifyEquipItemData(_item.Index, _item.num, _item.refining, _item.level, _item.equipped);
                    }

                    UserManager.Instance.UserGameDataInfo.CheckEquipBadge();
                }
                break;
            case ITEM_TYPE.ACCESSARY:
                {
                    if (!UISystemOpenPage.Get().SetOpenContent(eContentsOpenType.EQUIPAC))
                    {
                        _owner.OnClick_ACLock();
                        return;
                    }

                    PlayerPrefs.SetString("SelectItemType", "ACCESSARY");
                    weaponItemController.gameObject.SetActive(false);
                    shieldItemController.gameObject.SetActive(false);
                    accessaryItemController.gameObject.SetActive(true);

                    AccessorySeletindex = -1;
                    ItemSlotList.Clear();
                    accessaryItemController.scroller.ReloadData();

                    ToggleGroupObjs[0].SetActive(false);
                    ToggleGroupObjs[1].SetActive(false);
                    ToggleGroupObjs[2].SetActive(true);

                    ItemSortToggles[(int)eItemSortType.ACCESSORIE].isOn = true;
                    for (int i = 0; i < W_quilityindextoggle.Count; i++)
                    {
                        W_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < S_quilityindextoggle.Count; i++)
                    {
                        S_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < A_quilityindextoggle.Count; i++)
                    {
                        A_quilityindextoggle[i].isOn = false;
                    }
                    for (int i = 0; i < ItemSlotList.Count; i++)
                    {
                        if (ItemSlotList[i] == null || ItemSlotList[i].EquipDataInfo == null) continue;

                        ItemSlotList[i].Init(this);
                        ItemSlotList[i].SetSelectedSlot(false);
                    }

                    List<EnableItem> _list = UserManager.Instance.UserGameDataInfo.myEquipItems.Where(x => x.isnew && x.Type == ITEM_TYPE.ACCESSARY).ToList();
                    foreach (EnableItem _item in _list)
                    {
                        _item.isnew = false;
                        UserManager.Instance.UserGameDataInfo.ModifyEquipItemData(_item.Index, _item.num, _item.refining, _item.level, _item.equipped);
                    }

                    UserManager.Instance.UserGameDataInfo.CheckEquipBadge();
                }
                break;
            default:
                break;
        }
        _owner.ReSetScroll();
    }
    
    public void OnClick_ComposAll()
    {
        if (!ComposAllToggle.isOn)
            return;
        ComposAllToggle.isOn = false;
    }
    public void ReSetTapNotice()
    {
    }
    public void DeleitTapNotice(ITEM_TYPE type)
    {
    }
    public ITEM_TYPE GetNowSeletItemType()
    {
        return SelectedItemType;
    }
    private void Update()
    {
        if(UpdatedItemList.Count <= 0)
        {
            return;
        }

        UIItemSlot _slot = null;
        for(int i = 0; i < UpdatedItemList.Count; i++)
        {
            _slot = ItemSlotList.Find(match => { return match.EquipDataInfo.ItemId == UpdatedItemList[i]; });
            if(_slot == null)
            {
                continue;
            }
            _slot.ReInit();
        }
        UpdatedItemList.Clear();
    }

    public void RefreshEquipMark()
    {
        UIItemSlot _slot = null;
        for (int i = 0; i < ItemSlotList.Count; i++)
        {
            _slot = ItemSlotList[i];
            if (_slot == null)
            {
                continue;
            }
            _slot.RefreshEquipMark();
        }

    }
    public UIItemSlot GetItemSlot(int itemindex)
    {
        UIItemSlot slot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == itemindex);
        return slot;
    }

    private void SetVisibiltyItemSlot(UIItemSlotContainer slotContainer)
    {
        for (int i = 0; i < slotContainer.rowItemSlots.Length; i++)
        {
            if (slotContainer.rowItemSlots[i].EquipDataInfo != null)
            {
                if (ItemSlotList.Find(x => x.EquipDataInfo.ItemId == slotContainer.rowItemSlots[i].EquipDataInfo.ItemId) == null)
                {
                    ItemSlotList.Add(slotContainer.rowItemSlots[i]);
                    slotContainer.rowItemSlots[i].Init(this);
                }

                if (_owner.SelectEquipData == null
                        || _owner.SelectEquipData.ItemId != slotContainer.rowItemSlots[i].EquipDataInfo.ItemId
                        || _owner.SelectEquipData.itemType != slotContainer.rowItemSlots[i].EquipDataInfo.itemType)
                {
                    slotContainer.rowItemSlots[i].SetSelectedSlot(false);
                }
                else
                {
                    slotContainer.rowItemSlots[i].SetSelectedSlot(true);
                }
            }
        }
    }

    #region Tutorial Coroutine
    public IEnumerator Gear_EquipTutorial2()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetRaycaster(false);

        yield return new WaitForEndOfFrame();

        UIItemSlot tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);
        TutorialManager.Instance.SetUIGuidePosition(tempSlot.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (tempSlot.isnew)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial3();
    }

    public IEnumerator Gear_EquipTutorial3()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        UIItemSlot tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);
        TutorialManager.Instance.SetUIGuidePosition(tempSlot.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (_owner.NowPanel.GetGrowthType().Equals(eItemGrowthType.DETAIL) == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial4();
    }

    public IEnumerator Gear_EquipTutorial4()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailEquipBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (UserGameData.Get().GetEnableItem(1).equipped == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial5();
    }

    public IEnumerator Gear_EquipTutorial5()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailCloseBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (_owner.NowPanel.GetGrowthType().Equals(eItemGrowthType.DETAIL))
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial6();
    }

    public IEnumerator Gear_EquipTutorial6()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(ItemSortToggles[1].gameObject.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (shieldItemController.gameObject.activeSelf == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    Toggle toggle = result.gameObject.GetComponentInParent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.onValueChanged.Invoke(true);
                        toggle.isOn = true;
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial7();
    }

    public IEnumerator Gear_EquipTutorial7()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        UIItemSlot tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);
        TutorialManager.Instance.SetUIGuidePosition(tempSlot.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (tempSlot.isnew)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial8();
    }

    public IEnumerator Gear_EquipTutorial8()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        UIItemSlot tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);
        TutorialManager.Instance.SetUIGuidePosition(tempSlot.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (_owner.NowPanel.GetGrowthType().Equals(eItemGrowthType.DETAIL) == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_EquipTutorial9();
    }

    public IEnumerator Gear_EquipTutorial9()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_EQUIP) == false)
        {
            yield break;

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailEquipBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (UserGameData.Get().GetEnableItem(TutorialManager.Instance.CurrentTutorial.tutorialItemIndex).equipped == false)
        {
            yield return new WaitForEndOfFrame();
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();
    }

    public IEnumerator Gear_ComposeTutorial2()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        UIItemSlot tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);

        while (tempSlot == null)
        {
            yield return new WaitForEndOfFrame();

            if (SelectedItemType == ITEM_TYPE.WEAPON)
            {
                break;
            }
        }

        tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);
        TutorialManager.Instance.SetUIGuidePosition(tempSlot.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (_owner.NowPanel.GetGrowthType().Equals(eItemGrowthType.DETAIL) == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_ComposeTutorial3();
    }

    public IEnumerator Gear_ComposeTutorial3()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailComposeBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));
    
        while (_owner.ItemGrowthPopup.gameObject.activeSelf == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_ComposeTutorial4();
    }

    public IEnumerator Gear_ComposeTutorial4()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(upgradeComposeBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (UserGameData.Get().GetEnableItem(TutorialManager.Instance.CurrentTutorial.tutorialItemIndex) == null)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_ComposeTutorial5();
    }

    public IEnumerator Gear_ComposeTutorial5()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailCloseBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (_owner.NowPanel.GetGrowthType().Equals(eItemGrowthType.DETAIL))
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_ComposeTutorial6();
    }

    public IEnumerator Gear_ComposeTutorial6()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        UIItemSlot tempSlot = ItemSlotList.Find(x => x.EquipDataInfo.ItemId == TutorialManager.Instance.CurrentTutorial.tutorialItemIndex);

        while (tempSlot == null)
        {
            yield return new WaitForEndOfFrame();
        }

        TutorialManager.Instance.SetUIGuidePosition(tempSlot.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (_owner.NowPanel.GetGrowthType().Equals(eItemGrowthType.DETAIL) == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();

        yield return Gear_ComposeTutorial7();
    }

    public IEnumerator Gear_ComposeTutorial7()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_COMPOSE) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailEquipBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));

        while (UserGameData.Get().GetEnableItem(TutorialManager.Instance.CurrentTutorial.tutorialItemIndex).equipped == false)
        {
            yield return new WaitForEndOfFrame();

            if (TutorialManager.Instance.IsClicked)
            {
                foreach (RaycastResult result in TutorialManager.Instance.Results)
                {
                    if (result.gameObject.TryGetComponent<Button>(out Button button))
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
            }
        }

        TutorialManager.Instance.ActiveOffUIGuideObj();
        TutorialManager.Instance.NextStepTutorial();
    }

    public IEnumerator Gear_EnchantTutorial2()
    {
        if (TutorialManager.Instance.CurrentTutorial.contentType.Equals(TutorialContentType.GEAR_ENCHANT) == false)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        TutorialManager.Instance.SetUIGuidePosition(detailEquipBtn.GetComponent<RectTransform>(), LocalizeManager.Instance.GetTXT(TutorialManager.Instance.CurrentTutorial.tipTexts[0]));
    }

    #endregion Tutorial Coroutine

    List<EquipInfoData> GetListTierDownData()
    {
        List<EquipInfoData> list = new List<EquipInfoData>();
        foreach (EquipInfoData item in UserManager.Instance.EquipInfoDatas)
        {
            if (item.gradeIndex > 0)
                list.Add(item);
        }
        List<EquipInfoData> temp = list.OrderBy(x => x.gradeIndex).Reverse().ToList<EquipInfoData>();
        return temp;
    }
    List<EquipInfoData> GetListTierUpData()
    {
        List<EquipInfoData> list = new List<EquipInfoData>();
        foreach (EquipInfoData item in UserManager.Instance.EquipInfoDatas)
        {
            if (item.gradeIndex > 0)
                list.Add(item);
        }
        List<EquipInfoData> temp = list.OrderBy(x => x.gradeIndex).ToList<EquipInfoData>();
        return temp;
    }
}
