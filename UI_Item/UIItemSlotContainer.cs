using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;

public class UIItemSlotContainer : EnhancedScrollerCellView
{
    public UIItemSlot[] rowItemSlots;
    UIItemGrowthListInfo _owner = null;
    public void init(UIItemGrowthListInfo Owner)
    {
        _owner = Owner;
    }
    public void SetData(List<EquipInfoData> equipList, int startingIndex)
    {
        for (int i = 0; i < rowItemSlots.Length; i++)
        {
            if (rowItemSlots[i].gameObject.activeSelf == false)
            {
                rowItemSlots[i].gameObject.SetActive(true);
            }

            EquipInfoData info = startingIndex + i < equipList.Count ? equipList[startingIndex + i] : null;
            rowItemSlots[i].SetData(info, _owner._owner.OnSelectItem);
            rowItemSlots[i].ReInit();
        }
    }

    public UIItemSlot GetRowItemSlot(int index)
    {
        UIItemSlot result = rowItemSlots[index];

        return result;
    }
}
