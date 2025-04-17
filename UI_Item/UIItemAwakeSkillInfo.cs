using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class skilltooltipdata
{
    public SKILL_VALUE_TYPE valuetype;
    public eStatType type;
    public float value;
}
//각성스킬정보
public class UIItemAwakeSkillInfo : MonoBehaviour
{
    [SerializeField] Image SkillImage;
    [SerializeField] Image PopupSkillImage;
    [SerializeField] GameObject Skillicon;
    [SerializeField] Text SkillnameText;
    [SerializeField] Text SkillLeveText;
    [SerializeField] Text ImageSkillLeveText;
    [SerializeField] Text infoSkillTooltip;
    [SerializeField] Transform LevelEffectPos;
    [SerializeField] UIEffectText LevelText;
    [SerializeField] List<UIEffectText> LevelTextList;
    [SerializeField] List<UIEffectText> LockTextList;
    [SerializeField] List<Text> LevelList;

    [SerializeField] List<ContentSizeFitter> _sizefitterlist;
    [SerializeField] public GameObject SkillPopupObj;
    [SerializeField] Color OnEffectColor;
    [SerializeField] Color OffEffectColor;
    
    public void SetData(SkillInfoData skillinfo,EquipInfoData equipinfo)
    {
        Sprite sp = Resources.Load<Sprite>(StringConst.SKILL_SPRITE_PATH + skillinfo.skillIcon);
        if (sp == null) 
            sp = Resources.Load<Sprite>(StringConst.SKILL_SPRITE_PATH + "main_skillicon_bg");
        SkillImage.sprite = sp;
        PopupSkillImage.sprite = sp;
        SkillnameText.text = LocalizeManager.Instance.GetTXT(skillinfo.skillName);
        EnableItem useritem = UserGameData.Get().GetEnableItem(equipinfo.ItemId);
        int level = 1;
        if (useritem != null)
        {
            level = useritem.promotionTier+ 1;
        }
        ImageSkillLeveText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), level);
        for (int i = 0; i < LevelList.Count; i++)
        {
            LevelList[i].gameObject.SetActive(false);
        }
        int maxtlevel = SkillManager.Instance.SkillLevelDatas.Where(n => n.groupID == skillinfo.skillLevelId).OrderByDescending(n => n.levelID).FirstOrDefault().levelID;
        List<float> applyInfoTooltipList = SkillSystem.GetTooltipApplyInfo(skillinfo);
        if (applyInfoTooltipList.Count > 0) 
        {
            if (applyInfoTooltipList.Count == 1)
            {
                float _applyValue_1 = (applyInfoTooltipList[0] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                infoSkillTooltip.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillTooltip), _applyValue_1);

                for (int i = 1; i <= maxtlevel; i++)
                {
                    bool isset = false;
                    for (int n = 0; n < LevelList.Count; n++)
                    {
                        if (!LevelList[n].gameObject.activeSelf)
                        {
                            LevelList[n].gameObject.SetActive(true);
                            string leveltext = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_LV_01"), i);
                            string valuetext = string.Format("{0}%", (applyInfoTooltipList[0] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, i)) * 100);
                            string leveltooltiptext = LocalizeManager.Instance.GetTXT(skillinfo.skillLvUpTooltip);
                            if (level >= i)
                            {
                                LevelTextList[n].SetStringValueAddColor(leveltext, valuetext, OnEffectColor);
                                LevelTextList[n].SetStringTypeStringValue(leveltooltiptext, valuetext);
                                LockTextList[n].SetStringTypeStringValue(leveltooltiptext, valuetext);
                                LevelTextList[n].LockObj.SetActive(false);
                                LevelList[n].text = leveltext;
                                LevelList[n].color = OnEffectColor;
                            }
                            else
                            {
                                LevelTextList[n].SetStringTypeStringValue(leveltooltiptext, valuetext);
                                LockTextList[n].SetStringTypeStringValue(leveltooltiptext, valuetext);
                                LevelTextList[n].LockObj.SetActive(true);
                                LevelTextList[n].SetStringValueAddColor(leveltext, valuetext, OffEffectColor);
                                LevelList[n].text = leveltext;
                                LevelList[n].color = OffEffectColor;
                            }
                            isset = true;
                            break;
                        }
                    }
                   
                }
            }
            else if (applyInfoTooltipList.Count == 2)
            {
                float _applyValue_1 = (applyInfoTooltipList[0] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                float _applyValue_2 = (applyInfoTooltipList[1] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                infoSkillTooltip.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillTooltip), _applyValue_1, _applyValue_2);
            }
            else if (applyInfoTooltipList.Count == 3)
            {
                float _applyValue_1 = (applyInfoTooltipList[0] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                float _applyValue_2 = (applyInfoTooltipList[1] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                float _applyValue_3 = (applyInfoTooltipList[2] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                infoSkillTooltip.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillTooltip), _applyValue_1, _applyValue_2, _applyValue_3);
            }
            else if (applyInfoTooltipList.Count == 4)
            {
                float _applyValue_1 = (applyInfoTooltipList[0] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                float _applyValue_2 = (applyInfoTooltipList[1] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                float _applyValue_3 = (applyInfoTooltipList[2] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                float _applyValue_4 = (applyInfoTooltipList[3] + SkillManager.Instance.GetSkillLevelValue(skillinfo.Index, level)) * 100;
                infoSkillTooltip.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillTooltip), _applyValue_1, _applyValue_2, _applyValue_3, _applyValue_4);
            }
        }
        else
        {
            infoSkillTooltip.text = LocalizeManager.Instance.GetTXT(skillinfo.skillTooltip);
        }
        UpdataSizeFitter();
    }
    void UpdataSizeFitter()
    {
        for (int i = 0; i < _sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_sizefitterlist[i].transform);
        }

    }
    public void OpenSkillinfoPopup()
    {
        SkillPopupObj.SetActive(true);
        gameObject.SetActive(true);
        UpdataSizeFitter();
    }
    public void CloseSkillinfoPopup()
    {
        SkillPopupObj.SetActive(false);
        gameObject.SetActive(false);
    }
    public void SetAwakeSkillicon(bool enable)
    {
        Skillicon.SetActive(enable);

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
