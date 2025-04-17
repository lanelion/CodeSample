using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemTransendence_Popup : MonoBehaviour
{
    [System.Serializable]
    public class ItmeEffect
    {
        public Text BeforeEfftetTypeText;
        public Text BeforeEfftetValText;
        public Text AfterEfftetTypeText;
        public Text AfterEfftetValText;

        public Text NewEfftetnameText;
        public Text NewEfftetText;
    }

    [SerializeField] Text EffectTitleText;
    [SerializeField] Text CreateItemTitleText;
    [SerializeField] ItmeEffect ItemEffectInfo;
    [SerializeField] Text TranscendenceBtntext;
    [SerializeField] Text TranscendenceChecktext;
    [SerializeField] Text ExplanationText;


    public void StartInitialize()
    {
        EffectTitleText.text= LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_BONUS");
        CreateItemTitleText.text= LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_FORGE");
        ExplanationText.text= LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_BONUS_INFO");
        TranscendenceChecktext.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_AWAKE_FORGE_INFO");

        ClosePopup();
    }
    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
    public void OpenPopup()
    {
        this.gameObject.SetActive(true);
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
