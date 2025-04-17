using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIItemAwakeResult : MonoBehaviour
{
    [SerializeField] UIItemSlot Resultitemslot;
    [SerializeField] Text ItemnameText;
    PopupItemGrowth _owner = null;
    public void Init(PopupItemGrowth owner)
    {
        _owner = owner;
    }
    public void CloseResult()
    {
        gameObject.SetActive(false);
    }
    public void OpenResult(EquipInfoData item)
    {
        gameObject.SetActive(true);
        Resultitemslot.SetData(item);
        ItemnameText.text = string.Format( LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_GET"), item.nameTextID);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
