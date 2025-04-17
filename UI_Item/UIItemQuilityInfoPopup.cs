using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class QuilityInfoObjs
{

    public Text SkillTypeText;
    public Text SkillName;
    public Text Skillinfo;
    public List<Text> SkillLevelText;
    public Text MasteryTypeText;
    public List<Image> MasteryTypeIcon;

    public Image Skillimg;
    public Image DisenableSkillimg;
    public List<UIEffectText> EffectList;
    public List<UIEffectText> LockEffectList;
    public GameObject SkillPopupObj;
    public List<RectTransform> traTitle;
    public List<RectTransform> SubtraTitle;
    public List<RectTransform> LocktraTitle;
    public List<RectTransform> LockSubtraTitle;
    public List<RectTransform> BgRect;
}
public class UIItemQuilityInfoPopup : MonoBehaviour
{
    [SerializeField] Text ItemTypeText;
    [SerializeField] Color NextColor;
    [SerializeField] Color LockColor;
    [SerializeField] Color NormalColor;

    [SerializeField]public QuilityInfoObjs Skill1Objs;
    [SerializeField] public QuilityInfoObjs Skill2Objs;
    [SerializeField] List<ContentSizeFitter> _sizefitterlist;

    [SerializeField] int movespeed = 5;
    Vector2 direction = Vector2.left;
    List<Coroutine> coroutinelist;
    bool checkend;
    int skillordernum = 0;
    List<float> endposlist;
    public void SetData(MasteryLevelData info)
    {
        if (coroutinelist == null)
            coroutinelist = new List<Coroutine>();
        else
        {
            for(int i=0;i< coroutinelist.Count; i++)
            {
                StopCoroutine(coroutinelist[i]);
            }
        }
        switch (info.qualityIndex)
        {
            case 1:
                {
                    ItemTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_01_TITLE");
                    Skill1Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_01_TITLE");
                    Skill2Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_01_TITLE");
                }
                break;
            case 2:
                {

                    ItemTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_02_TITLE");
                    Skill1Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_02_TITLE");
                    Skill2Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_02_TITLE");
                }
                break;
            case 3:
                {

                    ItemTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_03_TITLE");
                    Skill1Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_03_TITLE");
                    Skill2Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_03_TITLE");
                }
                break;
            case 4:
                {

                    ItemTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_04_TITLE");
                    Skill1Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_04_TITLE");
                    Skill2Objs.MasteryTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_04_TITLE");
                }
                break;

        }

        Skill1Objs.SkillTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MASTERY_TYPE1");
        Skill2Objs.SkillTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MASTERY_TYPE2");
        List<MasteryLevelData> Skilllist = GetQuilitySkillData(info.itemType, info.qualityIndex);
        List<MasteryLevelData> Skill2list = GetQuilitySkill2Data(info.itemType, info.qualityIndex);
        int talentlevel = 0;
        if (UserGameData.Get()._dicEquipWeaponTalentLevel.ContainsKey(info.qualityIndex))
        {
            talentlevel = UserGameData.Get()._dicEquipWeaponTalentLevel[info.qualityIndex];
        }
        #region 스킬1
        SkillInfoData skill = SkillManager.Instance.GetSkillData(info.skillid);
        Skill1Objs.DisenableSkillimg.gameObject.SetActive(false);
        Skill1Objs.SkillName.text = LocalizeManager.Instance.GetTXT(skill.skillName);
        Skill1Objs.Skillinfo.text = LocalizeManager.Instance.GetTXT(skill.skillTooltip);
        Skill1Objs.Skillimg.sprite = SkillSystem.GetSkillSprite(skill.skillIcon);
        for (int i = 0; i < Skill1Objs.EffectList.Count; i++) 
        {
            Skill1Objs.EffectList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < Skilllist.Count; i++)
        {
            SkillInfoData qualityskill = SkillManager.Instance.GetSkillData(Skilllist[i].skillid);
            if (qualityskill == null)
                continue;
            Skill1Objs.EffectList[i].SetStringTypeStringValue(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skilllist[i].level), LocalizeManager.Instance.GetTXT(qualityskill.gradeTooltip));
            Skill1Objs.SkillLevelText[i].text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skilllist[i].level);
            Skill1Objs.LockEffectList[i].SetStringTypeStringValue(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skilllist[i].level), LocalizeManager.Instance.GetTXT(qualityskill.gradeTooltip));

            if (Skilllist[i].level <= info.level)
            {
                Skill1Objs.SkillLevelText[i].color = NormalColor;
                Skill1Objs.EffectList[i].LockObj.SetActive(false);
            }
            else
            {
                Skill1Objs.SkillLevelText[i].color = LockColor;
                Skill1Objs.EffectList[i].LockObj.SetActive(true);
            }
            
            Skill1Objs.EffectList[i].gameObject.SetActive(true);
        }
        #endregion

        #region 스킬2
        if (info.skillid2 > 0)
        {
            Skill2Objs.SkillPopupObj.SetActive(true);
            SkillInfoData skill2 = SkillManager.Instance.GetSkillData(info.skillid2);
            Skill2Objs.DisenableSkillimg.gameObject.SetActive(false);
            Skill2Objs.SkillName.text = LocalizeManager.Instance.GetTXT(skill2.skillName);
            Skill2Objs.Skillinfo.text = LocalizeManager.Instance.GetTXT(skill2.skillTooltip);
            Skill2Objs.Skillimg.sprite = SkillSystem.GetSkillSprite(skill2.skillIcon);
            for (int i = 0; i < Skill2Objs.EffectList.Count; i++)
            {
                Skill2Objs.EffectList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < Skill2list.Count; i++)
            {
                if (Skill2list[i].skillid2 < 0)
                    continue;
                SkillInfoData qualityskill = SkillManager.Instance.GetSkillData(Skill2list[i].skillid2);
                if (qualityskill == null)
                    continue;
                Skill2Objs.EffectList[i].SetStringTypeStringValue(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skill2list[i].level), LocalizeManager.Instance.GetTXT(qualityskill.gradeTooltip));
                Skill2Objs.LockEffectList[i].SetStringTypeStringValue(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skill2list[i].level), LocalizeManager.Instance.GetTXT(qualityskill.gradeTooltip));
                Skill2Objs.SkillLevelText[i].text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skill2list[i].level);
                if (Skill2list[i].level <= info.level)
                {
                    Skill2Objs.SkillLevelText[i].color = NormalColor;
                    Skill2Objs.EffectList[i].LockObj.SetActive(false);
                }
                else
                {
                    Skill2Objs.SkillLevelText[i].color = LockColor;
                    Skill2Objs.EffectList[i].LockObj.SetActive(true);
                }
                Skill2Objs.EffectList[i].gameObject.SetActive(true);
            }
        }
        else
        {
            int skill2index = -1;
            for(int i=0;i< Skilllist.Count; i++)
            {
                if(Skilllist[i].skillid2>0)
                {
                    skill2index = Skilllist[i].skillid2;
                    break;
                }
            }
            SkillInfoData skill2 = SkillManager.Instance.GetSkillData(skill2index);
            if (skill2 == null)
            {
                Skill2Objs.SkillPopupObj.SetActive(false);
                return;
            }
            Skill2Objs.DisenableSkillimg.sprite = SkillSystem.GetSkillSprite(skill2.skillIcon);
            Skill2Objs.DisenableSkillimg.gameObject.SetActive(true);
            Skill2Objs.SkillName.text = LocalizeManager.Instance.GetTXT(skill2.skillName);
            Skill2Objs.Skillinfo.text = LocalizeManager.Instance.GetTXT(skill2.skillTooltip);
            Skill2Objs.Skillimg.sprite = SkillSystem.GetSkillSprite(skill2.skillIcon);
            for (int i = 0; i < Skill2Objs.EffectList.Count; i++)
            {
                Skill2Objs.EffectList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < Skill2list.Count; i++)
            {
                if (Skill2list[i].skillid2 < 0)
                    continue;
                SkillInfoData qualityskill = SkillManager.Instance.GetSkillData(Skill2list[i].skillid2);
                if (qualityskill == null)
                    continue;
                if (Skill2list[i].level <= info.level - 1)
                {
                    Skill2Objs.EffectList[i].SetStringValueAddColor(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skill2list[i].level), "", LocalizeManager.Instance.GetTXT(qualityskill.gradeTooltip), Color.black);
                }
                else
                {
                    Skill2Objs.EffectList[i].SetStringValueAddColor(string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_SKILLLV"), Skill2list[i].level), "", LocalizeManager.Instance.GetTXT(qualityskill.gradeTooltip), NextColor);
                }
                Skill2Objs.EffectList[i].gameObject.SetActive(true);
            }
            //Skill2Objs.SkillPopupObj.SetActive(false);
        }
        #endregion

    }
    public void ShowPopup(int skillorder)
    {
        if (endposlist == null)
            endposlist = new List<float>();
        endposlist.Clear();
        checkend = false;
        skillordernum = skillorder;
        gameObject.SetActive(true);
        if (coroutinelist == null)
            coroutinelist = new List<Coroutine>();
        if (skillorder == 1)
        {
            Skill1Objs.SkillPopupObj.SetActive(true);
            Skill2Objs.SkillPopupObj.SetActive(false);
        }

        else if (skillorder == 2)
        {
            Skill1Objs.SkillPopupObj.SetActive(false);
            Skill2Objs.SkillPopupObj.SetActive(true);
        }
        for (int i = 0; i < _sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_sizefitterlist[i].transform);
        }
        if (skillorder == 1)
        {
            for (int i = 0; i < Skill1Objs.BgRect.Count; i++)
            {
                Skill1Objs.traTitle[i].Translate(direction * 0);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill1Objs.traTitle[i].transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill1Objs.SubtraTitle[i].transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill1Objs.LocktraTitle[i].transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill1Objs.LockSubtraTitle[i].transform);
                Skill1Objs.traTitle[i].anchoredPosition = new Vector2(0, Skill1Objs.traTitle[i].anchoredPosition.y);
                Skill1Objs.LocktraTitle[i].anchoredPosition = new Vector2(0, Skill1Objs.LocktraTitle[i].anchoredPosition.y);
                float width = Skill1Objs.traTitle[i].rect.width + Skill1Objs.SubtraTitle[i].rect.width + Skill1Objs.SubtraTitle[i].rect.x;
                if (width > Skill1Objs.BgRect[i].rect.width)
                {
                    float Endps = Skill1Objs.traTitle[i].rect.width + Skill1Objs.SubtraTitle[i].rect.width + Skill1Objs.SubtraTitle[i].rect.x - Skill1Objs.BgRect[i].rect.width;
                    Vector2 Startposx = Skill1Objs.traTitle[i].anchoredPosition;

                    coroutinelist.Add(StartCoroutine(CorMoveText(Skill1Objs.traTitle[i], Startposx, -Endps)));
                    coroutinelist.Add(StartCoroutine(CorMoveText(Skill1Objs.LocktraTitle[i], Startposx, -Endps)));
                    endposlist.Add(Endps);
                }
                else
                {
                    Skill1Objs.traTitle[i].anchoredPosition = new Vector2(0, Skill1Objs.traTitle[i].anchoredPosition.y);
                    Skill1Objs.LocktraTitle[i].anchoredPosition = new Vector2(0, Skill1Objs.LocktraTitle[i].anchoredPosition.y);
                    endposlist.Add(-1);

                }
            }
        }
        else if (skillorder == 2)
        {
            for (int i = 0; i < Skill2Objs.BgRect.Count; i++)
            {
                Skill2Objs.traTitle[i].Translate(direction * 0);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill2Objs.traTitle[i].transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill2Objs.SubtraTitle[i].transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill2Objs.LocktraTitle[i].transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Skill2Objs.LockSubtraTitle[i].transform);
                Skill2Objs.traTitle[i].anchoredPosition = new Vector2(0, Skill2Objs.traTitle[i].anchoredPosition.y);
                Skill2Objs.LocktraTitle[i].anchoredPosition = new Vector2(0, Skill2Objs.LocktraTitle[i].anchoredPosition.y);
                float width = Skill2Objs.traTitle[i].rect.width + Skill2Objs.SubtraTitle[i].rect.width + Skill2Objs.SubtraTitle[i].rect.x;
                if (width > Skill2Objs.BgRect[i].rect.width)
                {
                    float Endps = Skill2Objs.traTitle[i].rect.width + Skill2Objs.SubtraTitle[i].rect.width + Skill2Objs.SubtraTitle[i].rect.x - Skill2Objs.BgRect[i].rect.width;
                    Vector2 Startposx = Skill2Objs.traTitle[i].anchoredPosition;

                    coroutinelist.Add(StartCoroutine(CorMoveText(Skill2Objs.traTitle[i], Startposx, -Endps)));
                    coroutinelist.Add(StartCoroutine(CorMoveText(Skill2Objs.LocktraTitle[i], Startposx, -Endps)));
                    endposlist.Add(Endps);
                }
                else
                {
                    Skill2Objs.traTitle[i].anchoredPosition = new Vector2(0, Skill2Objs.traTitle[i].anchoredPosition.y);
                    Skill2Objs.LocktraTitle[i].anchoredPosition = new Vector2(0, Skill2Objs.LocktraTitle[i].anchoredPosition.y);
                    endposlist.Add(-1);

                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckText();
    }
    private IEnumerator CorMoveText(RectTransform _traTitle,Vector2 startpos, float sendposx)
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            _traTitle.Translate(direction * movespeed);
            if (IsEndPos(_traTitle, sendposx-10))
            {
                while (!checkend)
                {
                    yield return new WaitForEndOfFrame();
                }
                 yield return new WaitForSeconds(1);
                _traTitle.gameObject.SetActive(true);
                _traTitle.anchoredPosition = startpos;
                yield return new WaitForSeconds(1f);
            }
            checkend = false;

            yield return null;
        }
    }
    private bool IsEndPos(RectTransform _traTitle,float sendposx)
    {
            return sendposx > _traTitle.anchoredPosition.x;
    }
    void CheckText()
    {
        bool check = false;
        if (skillordernum == 1)
        {
            for (int i = 0; i < Skill1Objs.traTitle.Count; i++)
            {
                if (endposlist[i] <= 0)
                {
                    check = true;
                    continue;
                }
                if (Skill1Objs.traTitle[i].anchoredPosition.x != 0)
                {
                    if (IsEndPos(Skill1Objs.traTitle[i], -endposlist[i] - 10))
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                        checkend = false;
                        break;
                    }
                }
                else
                {
                    check = false;
                    checkend = false;
                    break;
                }

            }
            if (check)
            {
                checkend = true;
            }
        }
        else if (skillordernum == 2)
        {
            for (int i = 0; i < Skill2Objs.traTitle.Count; i++)
            {
                if (endposlist[i] <= 0)
                {
                    check = true;
                    continue;
                }
                if (Skill2Objs.traTitle[i].anchoredPosition.x != 0)
                {
                    if (IsEndPos(Skill2Objs.traTitle[i], -endposlist[i]-10))
                    {
                        check = true;
                    }
                    else
                    {
                        checkend = false;
                        check = false;
                        break;
                    }
                }
                else
                {
                    checkend = false;
                    check = false;
                    break;
                }
            }
            if (check)
            {
                checkend = true;
            }
        }
    }

    public List<MasteryLevelData> GetQuilitySkillData(ITEM_TYPE type, int quality)
    {
        List<MasteryLevelData> list = new List<MasteryLevelData>();
        Dictionary<int, int> skillcheck = new Dictionary<int, int>();
        foreach (MasteryLevelData info in PopupManager.Instance.MasteryLevelDatas)
        {
            if (info.itemType == type && info.qualityIndex == quality)
            {
                if (!skillcheck.ContainsKey(info.skillid) && info.skillid > 0 && info.skillLvBatch[0] > 0)
                {
                    skillcheck.Add(info.skillid, info.Index);
                    list.Add(info);
                }
            }
        }
        list = list.OrderBy(x => x.Index).ToList();
        return list;
    }
    public List<MasteryLevelData> GetQuilitySkill2Data(ITEM_TYPE type, int quality)
    {
        List<MasteryLevelData> list = new List<MasteryLevelData>();
        Dictionary<int, int> skillcheck = new Dictionary<int, int>();
        foreach (MasteryLevelData info in PopupManager.Instance.MasteryLevelDatas)
        {
            if (info.itemType == type && info.qualityIndex == quality)
            {
                if (!skillcheck.ContainsKey(info.skillid2) && info.skillid2 > 0 && info.skillLvBatch[1] > 0)
                {
                    skillcheck.Add(info.skillid2, info.Index);
                    list.Add(info);
                }
            }
        }
        list = list.OrderBy(x => x.Index).ToList();
        return list;
    }
}
