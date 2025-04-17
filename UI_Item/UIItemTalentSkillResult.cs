using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemTalentSkillResult : MonoBehaviour
{
    [SerializeField] Text SkillNametext;
    [SerializeField] PopupGachaSlot skillslot;
    [SerializeField] Text SkillLevel;
    [SerializeField] Text SkillAddEffect;
    public void CloseResult()
    {
        gameObject.SetActive(false);
    }
    public void SetSkill(SkillInfoData skilldata,int level)
    {
        skillslot.SetSkillGradeUpData(skilldata);
        SkillNametext.text = LocalizeManager.Instance.GetTXT(skilldata.skillName);
        SkillLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_LEVEL_TITLE"), level);
        SkillAddEffect.text = LocalizeManager.Instance.GetTXT(skilldata.gradeTooltip);
    }
    public void OpenResult()
    {
        gameObject.SetActive(true);
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
