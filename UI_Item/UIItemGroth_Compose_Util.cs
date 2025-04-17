using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public partial class UIItemGroth_Compose
{
    Table_EquipComposeData.UpgradeRate RandomItem(List<Table_EquipComposeData.UpgradeRate> rates)
    {
        int selectnum = 0;
        int total = 0;
        foreach (Table_EquipComposeData.UpgradeRate rate in rates)
            total += rate.rate;
        int weight = 0;
        selectnum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

        for (int i = 0; i < rates.Count; i++)
        {
            weight += rates[i].rate;
            if (selectnum <= weight)
            {
                return rates[i];
            }
        }

        return null;
    }

   

}
