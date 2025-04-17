using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemQulitySlot : MonoBehaviour
{
    [SerializeField] Text LevelText;
    [SerializeField] Slider ExpSlider;
    [SerializeField] Text ExpText;
    [SerializeField] UIEffectText AddEffect;
    [SerializeField] GameObject LevelUpBtn;
    [SerializeField] Image QuilityImage;
    [SerializeField] Text QuilityTypeText;
    [SerializeField] Image Skill1image;
    [SerializeField] Image Skill2image;
    [SerializeField] Text Skill1LevelText;
    [SerializeField] Text Skill2LevelText;
    [SerializeField] public GameObject NoticeObj;
    [SerializeField] Material Disable;
    MasteryLevelData TableInfo = null;
    UIItemMasteryPopup masterpopup = null;
    public void SetData(MasteryLevelData info)
    {
        TableInfo = info;
        switch (TableInfo.qualityIndex)
        {
            case 1:
                {
                    QuilityTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_01");
                }
                break;
            case 2:
                {

                    QuilityTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_02");
                }
                break;
            case 3:
                {

                    QuilityTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_03");
                }
                break;
            case 4:
                {

                    QuilityTypeText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_WEAPON_MASTERY_04");
                }
                break;
        }
        SkillInfoData skill1= SkillManager.Instance.GetSkillData(TableInfo.skillid);
        SkillInfoData skill2 = SkillManager.Instance.GetSkillData(TableInfo.skillid2);

        Skill1image.sprite = SkillSystem.GetSkillSprite(skill1.skillIcon);
        Skill2image.sprite = SkillSystem.GetSkillSprite(skill2.skillIcon);
    }
    public void init(UIItemMasteryPopup owner)
    {
        masterpopup = owner;
    }
    public void SetUserData()
    {
        UserGameData data = UserGameData.Get();
        if (data._dicEquipWeaponTalentLevel == null|| data._dicEquipWeaponTalentExp==null)
            data.SetEquipqulityData();
        MasteryLevelData leveldata = PopupManager.Instance.MasteryLevelDatas.Where(n => n.itemType == ITEM_TYPE.WEAPON && n.qualityIndex == TableInfo.qualityIndex && n.level == data._dicEquipWeaponTalentLevel[TableInfo.qualityIndex]).FirstOrDefault();


        TableInfo = leveldata;
        LevelText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), data._dicEquipWeaponTalentLevel[TableInfo.qualityIndex]);
        if (TableInfo.skillLvBatch[0] > 0)
        {
            Skill1image.material = null;
            Skill1LevelText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), TableInfo.skillLvBatch[0]);
        }
        else
        {
            Skill1image.material = Disable;
            Skill1LevelText.text = "";
        }

        if (TableInfo.skillLvBatch[1] > 0)
        {
            Skill2image.material = null;
            Skill2LevelText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), TableInfo.skillLvBatch[1]);
        }
        else
        {
            Skill2image.material = Disable;
            Skill2LevelText.text = "";
        }
        if (TableInfo.needExp > 0)
        {
            ExpText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_ETC_001"), data._dicEquipWeaponTalentExp[TableInfo.qualityIndex], TableInfo.needExp);
            ExpSlider.value = (float)data._dicEquipWeaponTalentExp[TableInfo.qualityIndex] / (float)TableInfo.needExp;
        }
        else
        {
            ExpText.text = LocalizeManager.Instance.GetTXT("STR_UI_MAXLEVEL");
            ExpSlider.value = 1;
        }
        AddEffect.SetStatTypeAddValue(TableInfo.statType, TableInfo.statValue);
        if (TableInfo.needExp < 0)
        {
            LevelUpBtn.SetActive(false);
        }
        else
            LevelUpBtn.SetActive(true);
        if (data._dicEquipWeaponTalentExp[TableInfo.qualityIndex]>= TableInfo.needExp)
        {
            if (!masterpopup._owner.SetNotify)
            {
               masterpopup._owner.SetNotice();
            }
           masterpopup._owner.TalentNoitce.SetActive(true);

            NoticeObj.SetActive(true);
        }
        else
            NoticeObj.SetActive(false);

    }
    public void OnClick_LevelUpBtn()
    {
        UserGameData data = UserGameData.Get();
        if (data._dicEquipWeaponTalentLevel == null || data._dicEquipWeaponTalentExp == null)
            data.SetEquipqulityData();
        MasteryLevelData leveldata = PopupManager.Instance.MasteryLevelDatas.Where(n => n.itemType == ITEM_TYPE.WEAPON 
        && n.qualityIndex == TableInfo.qualityIndex && n.level == data._dicEquipWeaponTalentLevel[TableInfo.qualityIndex]).FirstOrDefault(); 
        if (leveldata.needExp < 0)
            return;

        MasteryLevelData nextleveldata = PopupManager.Instance.MasteryLevelDatas.Where(n => n.itemType == ITEM_TYPE.WEAPON
        && n.qualityIndex == TableInfo.qualityIndex && n.level == data._dicEquipWeaponTalentLevel[TableInfo.qualityIndex]+1).FirstOrDefault(); 

        
        if (data._dicEquipWeaponTalentExp[TableInfo.qualityIndex] >= leveldata.needExp)
        {
            data._dicEquipWeaponTalentLevel[TableInfo.qualityIndex] += 1;
            data._dicEquipWeaponTalentExp[TableInfo.qualityIndex] -= leveldata.needExp;
            data.UpdateEquipqulityData();
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.WPMASTERYL);
            if (nextleveldata != null)
            {
                if (leveldata.skillLvBatch[0] < nextleveldata.skillLvBatch[0])
                {
                    SkillInfoData skill = SkillManager.Instance.GetSkillData(nextleveldata.skillid);
                   masterpopup._owner.TalentSkillResult.SetSkill(skill, nextleveldata.skillLvBatch[0]);
                   masterpopup._owner.TalentSkillResult.OpenResult();
                }
                else if (leveldata.skillLvBatch[1] < nextleveldata.skillLvBatch[1])
                {
                    SkillInfoData skill = SkillManager.Instance.GetSkillData(nextleveldata.skillid2);
                   masterpopup._owner.TalentSkillResult.SetSkill(skill, nextleveldata.skillLvBatch[1]);
                   masterpopup._owner.TalentSkillResult.OpenResult();
                }
            }
        }
        else
            return;
        if (UIStage_Battle.Get() != null)
        {
            UIStage_Battle.Get().SkillSlotManager.UpDateEquipSKillSlot();
            UIStage_Battle.Get().SkillSlotManager.UpdateDirtyInfo = true;
        }
    }
    public void OnClick_Skillinfo()
    {
        masterpopup.OpenSkillInfoPopup(TableInfo.itemType, TableInfo.qualityIndex);
    }
    public void OnClick_Skill1info()
    {
        masterpopup.OpenSkillInfoPopup(TableInfo.itemType, TableInfo.qualityIndex,1);
    }
    public void OnClick_Skill2info()
    {
        masterpopup.OpenSkillInfoPopup(TableInfo.itemType, TableInfo.qualityIndex,2);
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
