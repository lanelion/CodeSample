using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eItemGrowthType
{
    NORAML,
    DETAIL,
    REINFORCE,  //강화
    REFINING,      //제련
    REFINING2,      //제련
    COMPOSE,    //합성
    DECOMPOSITION,//분해
    ADVANCEMENT,
    AWAKE,      //초월
    __MAX__
}

public class UIItemGrowthBase : MonoBehaviour
{
    [System.NonSerialized] public PopupItemGrowth _owner = null;
    public virtual eItemGrowthType GetGrowthType() { return eItemGrowthType.__MAX__; }

    public bool UpdateDirty = true;

    protected UIItemSlot SelectItemSlot = null;

    public virtual EquipInfoData SelectItemSlotInfo => null;

    public virtual void StartInitialize() { }
    public void Init(PopupItemGrowth owner)
    {
        _owner = owner;
    }

    public virtual void LoadingComplete() { }

    public virtual void OpenPanel(UIItemSlot _selectItemSlot=null)
    {
        Util.SetActiveObject(this.gameObject, true);

        SelectItemSlot = _selectItemSlot;
    }

    public virtual void ClosePanel()
    {
        Util.SetActiveObject(this.gameObject, false);
    }

  
}
