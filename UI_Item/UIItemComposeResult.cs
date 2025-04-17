using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemComposeResult : MonoBehaviour
{
    [SerializeField] PopupGachaSlot Slot;
    [SerializeField] Text ItemName;
    [SerializeField] Animator Resultami;
    [SerializeField] Text itemcounttext;
    [SerializeField] UIItemSlot TartgetSlot;


    PopupItemGrowth _owner = null;
    public void Init(PopupItemGrowth owner)
    {
        _owner = owner;
    }
    public void CloseResult()
    {
        gameObject.SetActive(false);

        if (_owner != null)
            _owner.ItemGrowthPopup.ClosePopup();
    }
    public void OpenResult(UIItemSlot slot,int count)
    {
        gameObject.SetActive(true);

        EquipComposeData composedata = PopupManager.Instance.EquipComposeDatas.Find(x=>x.Index==slot.EquipDataInfo.ItemId);
        EquipInfoData TargetItem = UserManager.Instance.EquipInfoDatas.Find(x=>x.ItemId==(composedata.UpgradeItem));
        TartgetSlot.SetData(TargetItem);
        TartgetSlot.SetComposeDataText(TargetItem);
        TartgetSlot.ReInit();
        itemcounttext.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_034"), count);
    }

}
