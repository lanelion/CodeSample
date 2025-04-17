using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UISelectDecompositionItemPopup : MonoBehaviour
{
    [SerializeField] Text ItemCountText;
    [SerializeField] PopupItemBox resultslot;
    [SerializeField] UIItemSlot ItemSlot;
    [SerializeField] Image GradeImage;
    [SerializeField] Animator animator;
    [SerializeField] Transform gridpos;
    [SerializeField] GameObject BackObj;
    [SerializeField] GameObject SelectBtnCover;
    [SerializeField] ContentSizeFitter SizeFitter;
    [SerializeField] ScrollRect ItemScrollRect;
    List<PopupItemBox> resultslotlist;
    UIItemSlot SelectSlot;
    EquipDecomposeData decomposinfo = null;
    int MaxDecompositionCount = 0;
    int NowSelectDecompositionCount = 0;
    public void StartInitialize()
    {
        if (resultslotlist == null)
            resultslotlist = new List<PopupItemBox>();
        AniEventClose();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick_MINBtn()
    {
        EnableItem item = UserGameData.Get().GetEnableItem(SelectSlot.EquipDataInfo.ItemId);
        if (item == null || item.num <= 0)
            return;
        Dictionary<int, int> _dicslotnum = new Dictionary<int, int>();
        if (decomposinfo != null)
        {
            for (int i = 0; i < decomposinfo.GetItemList.Count; i++)
            {
                for (int n = 0; n < decomposinfo.GetItemList[i].Count; n++)
                {
                    if (!decomposinfo.GetItemList[i][n].Israre)
                    {
                        if (!_dicslotnum.ContainsKey(decomposinfo.GetItemList[i][n].Getitemindex))
                        {
                            _dicslotnum.Add(decomposinfo.GetItemList[i][n].Getitemindex, decomposinfo.GetItemList[i][n].Getmin);
                        }
                    }
                }
            }
        }
        foreach(int index in _dicslotnum.Keys)
        {
            PopupItemBox slot = resultslotlist.Find(x => x._iteminfo.Index == index && x.gameObject.activeSelf);
            if (slot != null)
                slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_045"), _dicslotnum[index]));
        }
        NowSelectDecompositionCount = 1;
        ItemCountText.text = "1";
    }
    public void OnClick_MAXBtn()
    {
        EnableItem item = UserGameData.Get().GetEnableItem(SelectSlot.EquipDataInfo.ItemId);
        if (item == null || item.num <= 0)
            return;
        Dictionary<int, int> _dicslotnum = new Dictionary<int, int>();
        if (decomposinfo != null)
        {
            for (int i = 0; i < decomposinfo.GetItemList.Count; i++)
            {
                for (int n = 0; n < decomposinfo.GetItemList[i].Count; n++)
                {
                    if (!decomposinfo.GetItemList[i][n].Israre)
                    {
                        if (!_dicslotnum.ContainsKey(decomposinfo.GetItemList[i][n].Getitemindex))
                        {
                            _dicslotnum.Add(decomposinfo.GetItemList[i][n].Getitemindex, decomposinfo.GetItemList[i][n].Getmin);
                        }
                    }
                }
            }
        }
        
        NowSelectDecompositionCount = item.num; 
        foreach (int index in _dicslotnum.Keys)
        {
            PopupItemBox slot = resultslotlist.Find(x => x._iteminfo.Index == index && x.gameObject.activeSelf);
            if (slot != null)
                slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_045"), _dicslotnum[index] * NowSelectDecompositionCount));
        }
        ItemCountText.text = item.num.ToString();
    }
    public void OnClick_UPBtn()
    {
        if (MaxDecompositionCount > NowSelectDecompositionCount)
        {
            NowSelectDecompositionCount++;
            ItemCountText.text = NowSelectDecompositionCount.ToString(); 
            Dictionary<int, int> _dicslotnum = new Dictionary<int, int>();
            if (decomposinfo != null)
            {
                for (int i = 0; i < decomposinfo.GetItemList.Count; i++)
                {
                    for (int n = 0; n < decomposinfo.GetItemList[i].Count; n++)
                    {
                        if (!decomposinfo.GetItemList[i][n].Israre)
                        {
                            if (!_dicslotnum.ContainsKey(decomposinfo.GetItemList[i][n].Getitemindex))
                            {
                                _dicslotnum.Add(decomposinfo.GetItemList[i][n].Getitemindex, decomposinfo.GetItemList[i][n].Getmin);
                            }
                        }
                    }
                }
            }
            foreach (int index in _dicslotnum.Keys)
            {
                PopupItemBox slot = resultslotlist.Find(x => x._iteminfo.Index == index && x.gameObject.activeSelf);
                if (slot != null)
                    slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_045"), _dicslotnum[index] * NowSelectDecompositionCount));
            }
        }
    }
    public void OnClick_DownBtn()
    {
        if (NowSelectDecompositionCount > 1)
        {
            --NowSelectDecompositionCount;
            ItemCountText.text = NowSelectDecompositionCount.ToString();
            Dictionary<int, int> _dicslotnum = new Dictionary<int, int>();
            if (decomposinfo != null)
            {
                for (int i = 0; i < decomposinfo.GetItemList.Count; i++)
                {
                    for (int n = 0; n < decomposinfo.GetItemList[i].Count; n++)
                    {
                        if (!decomposinfo.GetItemList[i][n].Israre)
                        {
                            if (!_dicslotnum.ContainsKey(decomposinfo.GetItemList[i][n].Getitemindex))
                            {
                                _dicslotnum.Add(decomposinfo.GetItemList[i][n].Getitemindex, decomposinfo.GetItemList[i][n].Getmin);
                            }
                        }
                    }
                }
            }
            foreach (int index in _dicslotnum.Keys)
            {
                PopupItemBox slot = resultslotlist.Find(x => x._iteminfo.Index == index && x.gameObject.activeSelf);
                if (slot != null)
                    slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_045"), _dicslotnum[index] * NowSelectDecompositionCount));
            }
        }
    }
    public void SliderValChange(float value)
    {
        ItemCountText.text = NowSelectDecompositionCount.ToString();
    }
    public void OpenPopup(UIItemSlot _slot)
    {
        SelectSlot = _slot;
        GradeImage.sprite = ItemSystem.GetGradeiconSprite(SelectSlot.EquipDataInfo.Grade);
        ItemSlot.SetData(SelectSlot.EquipDataInfo);
        ItemSystem.CheckEnableItem(SelectSlot.EquipDataInfo,out bool enble, out MaxDecompositionCount, out int needcount,out EnableItem enableitem);
        if (enble)
        {
            if (enableitem.num > 0)
            {
                NowSelectDecompositionCount = 1;
                ItemCountText.text = "1";
                SelectBtnCover.SetActive(false);
                decomposinfo = PopupManager.Instance.EquipDecomposeDatas.Where(n => n.EquipID == SelectSlot.EquipDataInfo.ItemId).FirstOrDefault();
                if (decomposinfo != null)
                {
                    for (int i = 0; i < decomposinfo.GetItemList.Count; i++)
                    {
                        for (int n = 0; n < decomposinfo.GetItemList[i].Count;n++) 
                        {
                            SetResult(decomposinfo.GetItemList[i][n].Getitemindex, decomposinfo.GetItemList[i][n].Israre, decomposinfo.GetItemList[i][n].Getmin);
                        }
                    }
                }
            }
            else
            {
                NowSelectDecompositionCount = 0;
                ItemCountText.text = "0";
                SelectBtnCover.SetActive(true);
            }

        }
        else
        {
            NowSelectDecompositionCount = 0;
            ItemCountText.text = "0"; 
            SelectBtnCover.SetActive(true);
        }
        BackObj.SetActive(true);
        UIPopupAniSystem.PlayOpen(animator);
        this.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)SizeFitter.transform);
        ItemScrollRect.content.anchoredPosition = new Vector3(ItemScrollRect.content.anchoredPosition.x, 0, 0);
    }
    public void OnClick_SelectBtn()
    {
        EnableItem item = UserGameData.Get().GetEnableItem(SelectSlot.EquipDataInfo.ItemId);
        if (item == null || item.num <= 0)
            return;
        EquipDecomposeData resultdata = PopupManager.Instance.EquipDecomposeDatas.Where(n => n.EquipID == SelectSlot.EquipDataInfo.ItemId).FirstOrDefault();
        Dictionary<int, int> _dicresult = new Dictionary<int, int>();
        for (int v = 0; v < resultdata.GetItemList.Count; v++)
        {
            List<GetItem> getitemlist = resultdata.GetItemList[v];
            int maxrate = 0;
            for (int i = 0; i < getitemlist.Count; i++)
            {
                maxrate += getitemlist[i].GetRate;
            }
            for (int i = 0; i < NowSelectDecompositionCount; i++)
            {
                int rate = Random.Range(1, maxrate + 1);
                int nextrate = 0;
                int getitemindex = 0;
                int getnum = 0;
                for (int n = 0; n < getitemlist.Count; n++)
                {
                    if (rate <= nextrate + getitemlist[n].GetRate)
                    {
                        getitemindex = getitemlist[n].Getitemindex;
                        getnum = Random.Range(getitemlist[n].Getmin, getitemlist[n].Getmax + 1);
                        break;
                    }
                    nextrate += getitemlist[n].GetRate;
                }
                if (_dicresult.ContainsKey(getitemindex))
                {
                    _dicresult[getitemindex] += getnum;
                }
                else
                {
                    _dicresult.Add(getitemindex, getnum);
                }
            }
        }
        for (int i = 0; i < NowSelectDecompositionCount; i++)
        {
            UserManager.Instance.SetQuestCount(eQuestType.EQUIPDECOMPOSE);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.EQUIPDECOMPOSE);
        }
        List<RewardItem> resultitemlist = new List<RewardItem>();
        List<int> summonticketkey = new List<int>();
        foreach (int index in _dicresult.Keys)
        {
            ItemInfoData resultitem = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == index).FirstOrDefault();
            if (resultitem.itemType == ITEM_TYPE.SUMMON_TICKET)
            {
                
                BoxInfoData boxinfo = PopupManager.Instance.BoxInfoDatas.Find(x => x.Index == resultitem.itemValue);
                for (int i = 0; i < _dicresult[index]; i++)
                {
                    RewardItem resultitme = PopupManager.Instance.GetGachItme(boxinfo.ItemListID);
                    if (resultitme != null)
                    {
                        resultitemlist.Add(resultitme);
                    }
                }
                summonticketkey.Add(index);
            }
        }
        for (int i = 0; i < summonticketkey.Count; i++)
        {
            if (_dicresult.ContainsKey(summonticketkey[i]))
                _dicresult.Remove(summonticketkey[i]);
        }
        for (int i = 0; i < resultitemlist.Count; i++)
        {

            if (resultitemlist[i].itemtype == ITEM_TYPE.WEAPON || resultitemlist[i].itemtype == ITEM_TYPE.SHIELD || resultitemlist[i].itemtype == ITEM_TYPE.ACCESSARY)
            {

            }
            else if (resultitemlist[i].itemtype == ITEM_TYPE.SKILL)
            {

            }
            else
            {
                ItemInfoData resultitem = UserManager.Instance.ItemInfoDatas.Where(x => x.Index == resultitemlist[i].index).FirstOrDefault();
                if (_dicresult.ContainsKey(resultitemlist[i].index))
                {
                    _dicresult[resultitemlist[i].index] += resultitemlist[i].count;
                }
                else
                {
                    _dicresult.Add(resultitemlist[i].index, resultitemlist[i].count);
                }
            }
        }
        List<ItemInfoData> resultlist = new List<ItemInfoData>();
        foreach (int key in _dicresult.Keys)
        {
            ItemInfoData resultitem = UserManager.Instance.ItemInfoDatas.Where(x => x.Index ==key).FirstOrDefault();
            resultlist.Add(resultitem);
        }
        resultlist = resultlist.OrderBy(x => x.invenSort[1]).ToList();
        for (int i=0;i< resultlist.Count; i++)
        {
            ItemSystem.AddItem(resultlist[i].Index, _dicresult[resultlist[i].Index]);
            ItemClass useritem = UserGameData.Get().GetItemClass(resultlist[i].Index);

            

        }
        NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "DECOMPOSITIONITEMREWARD", state,
            "DecompositemIndex/" + SelectSlot.EquipDataInfo.ItemId + " Count/" + NowSelectDecompositionCount);
        ItemSystem.AddEquipItem(SelectSlot.EquipDataInfo.ItemId, -1 * NowSelectDecompositionCount);
        EnableItem userequipitem = UserGameData.Get().GetEnableItem(SelectSlot.EquipDataInfo.ItemId);
        string logstate = SelectSlot.EquipDataInfo.itemType + ":" + (userequipitem.num + NowSelectDecompositionCount) + "-" + NowSelectDecompositionCount;

        NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, ePopupType.ITEM_GROWTH.ToString(), "DECOMPOSITIONITEM", logstate,
            "DecompositemIndex/" + SelectSlot.EquipDataInfo.ItemId + " Count/" + NowSelectDecompositionCount);

        PopupManager.Instance.CreatePopup(PopupType.PopupResultPage, true, _popup =>
        {
            PopupResultPage _resultPage = _popup.GetComponent<PopupResultPage>();
            _resultPage.Init(null);

            for (int i = 0; i < resultlist.Count; i++)
            {
                _resultPage.AddItemBox(UserManager.Instance.ItemInfoDatas.Where(x => x.Index == resultlist[i].Index).FirstOrDefault(), _dicresult[resultlist[i].Index]);
            }

            _resultPage.SetData(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_DECOMPOSE_RESULT"));
            _resultPage.OpenWithPlayani();
        });
        
        AniEventClose();
    }
    public void ClosePopup()
    {
        UIPopupAniSystem.PlayClose(animator);
    }
    public void AniEventClose()
    {
        if (resultslotlist == null)
            resultslotlist = new List<PopupItemBox>();
        for(int i=0;i< resultslotlist.Count; i++)
        {
            resultslotlist[i].gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
        BackObj.SetActive(false);
    }
    public void SetResult(int index, bool israre,int minnum)
    {
        for (int i = 0; i < resultslotlist.Count; i++)
        {
            if (!resultslotlist[i].gameObject.activeSelf)
            {
                resultslotlist[i].gameObject.SetActive(true);
                resultslotlist[i].InitResult(UserManager.Instance.ItemInfoDatas.Where(x => x.Index == index).FirstOrDefault(), 0);
                if (israre)
                {
                    resultslotlist[i].SetBonusText(LocalizeManager.Instance.GetTXT("STR_UI_ETC_046"));
                    resultslotlist[i].SetBonusObj();
                }
                else
                    resultslotlist[i].SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_045"), minnum));
                //resultslotlist[i].SetNumString(string.Format("{0}+", minnum));
                return;
            }
        }
        GameObject go = Instantiate(resultslot.gameObject, gridpos);
        go.SetActive(true);
        PopupItemBox slot = go.GetComponent<PopupItemBox>();
        slot.InitResult(UserManager.Instance.ItemInfoDatas.Where(x => x.Index == index).FirstOrDefault(), 0);
        if (israre)
        {
            slot.SetBonusText(LocalizeManager.Instance.GetTXT("STR_UI_ETC_046"));
            slot.SetBonusObj();
        }
        else
            slot.SetNumString(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_045"), minnum));
        //slot.SetNumString(string.Format("{0}+", minnum));
        resultslotlist.Add(slot);

    }
    public void SetSprite(Sprite image,bool israre, ItemInfoData tooltipInfo = null) // 툴팁을 나타내야 되서 매개변수 추가 했습니다. 혹시 수정해야 된다면 말씀 해주세요.
    {
        for (int i = 0; i < resultslotlist.Count; i++)
        {
            if (!resultslotlist[i].gameObject.activeSelf)
            {
                resultslotlist[i].gameObject.SetActive(true);
                resultslotlist[i].InItSpreite(image);
                if (israre)
                {
                    resultslotlist[i].SetBonusText(LocalizeManager.Instance.GetTXT("STR_UI_ETC_046"));
                    resultslotlist[i].SetBonusObj();
                }
                //resultslotlist[i].SetNumString(string.Format("{0}+", minnum));
                return;
            }
        }
        GameObject go = Instantiate(resultslot.gameObject, gridpos);
        go.SetActive(true);
        PopupItemBox slot = go.GetComponent<PopupItemBox>();
        slot.InItSpreite(image);
        if (tooltipInfo != null)
        {
            slot._iteminfo = tooltipInfo;
        }
        if (israre)
        {
            slot.SetBonusText(LocalizeManager.Instance.GetTXT("STR_UI_ETC_046"));
            slot.SetBonusObj();
        }
        //slot.SetNumString(string.Format("{0}+", minnum));
        resultslotlist.Add(slot);
        
    }
    public void SetResultNum(int index, int minnum)
    {

    }
}
