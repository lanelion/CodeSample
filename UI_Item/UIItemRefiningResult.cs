using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemRefiningResult : MonoBehaviour
{
    [SerializeField] List<GameObject> EffectObjs;
    [SerializeField] PopupGachaSlot Slot;
    [SerializeField] Text ItemName;
    [SerializeField] Text AddEffect;
    [SerializeField]Animator Resultami;
    [SerializeField] GameObject AddEffctObj;
    [SerializeField] GameObject NoneEffctObj;
    [SerializeField] GameObject AddEffectBg;
    [SerializeField] GameObject NormalRefining;
    [SerializeField] Text MaxLevel;
    [SerializeField] Text ResultTitleText;

    [SerializeField] GameObject AwakeRefining;
    [SerializeField] UIItemSlot AwakeSlot;
    [SerializeField] GameObject AwakeAddEffectBg;
    [SerializeField] Text AwakeAddEffect;
    [SerializeField] GameObject AwakeSkillObj;
    [SerializeField] Image AwakeSkillicon;
    [SerializeField] Text AwakeSkillName;
    [SerializeField] Text AwakeSkillBeforeLevel;
    [SerializeField] Text AwakeSkillNowLevel;
    [SerializeField] Text AwakeSkillEffect;
    [SerializeField] Animator[] Awakestarani;
    [SerializeField] List<ContentSizeFitter> sizefitterlist;
    EquipInfoData itemdata = null;
    PopupItemGrowth _owner;

    public void CloseResult()
    {
        gameObject.SetActive(false);
        if (_owner.ItemGrowthPopup.gameObject.activeSelf)
            _owner.ItemGrowthPopup.Close();
    }
    public void Init(PopupItemGrowth owner)
    {
        _owner = owner;
    }
    public void OpenResult(UIItemSlot slot, PopupItemGrowth owner)
    {
        gameObject.SetActive(true);
        _owner = owner;
        AddEffect.text = "";

        itemdata = slot.EquipDataInfo;
        NormalRefining.SetActive(true);
        AwakeRefining.SetActive(false);
        Slot.SetItemRefiningData(slot.EquipDataInfo);
        ItemName.text = LocalizeManager.Instance.GetTXT(slot.EquipDataInfo.nameTextID);
        if (itemdata.itemType == ITEM_TYPE.ACCESSARY)
        {
            ResultTitleText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE_SUCCESS2");
        }
        else
        {
            ResultTitleText.text = LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_REFORGE_SUCCESS");
        }
        EnableItem selectitem = UserGameData.Get().GetEnableItem(slot.EquipDataInfo.ItemId);
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == slot.EquipDataInfo.itemType && x.grade == slot.EquipDataInfo.Grade
      && x.reforge == selectitem.refining && x.gradeIndex == slot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipAbilityData abilitydata = PopupManager.Instance.EquipAbilityDatas.Find(x => x.Index == (slot.EquipDataInfo.ItemId));

        string text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_MAXENCHANT"), refiningdata.maxLevel);

        if (string.IsNullOrEmpty(text))
        {
            AddEffectBg.SetActive(false);
            NoneEffctObj.SetActive(true);
            AddEffctObj.SetActive(false);
        }
        else
        {
            NoneEffctObj.SetActive(false);
            AddEffectBg.SetActive(true);
            AddEffctObj.SetActive(true);
        }
        MaxLevel.text = refiningdata.maxLevel.ToString();
        AddEffect.text = text;

        StartCoroutine(CheckAnimation());
    }
    public void OpenAdvancementResult(UIItemSlot slot,PopupItemGrowth owner)
    {
        gameObject.SetActive(true);
        NormalRefining.SetActive(false);
        AwakeRefining.SetActive(true);
        _owner = owner;

        itemdata = slot.EquipDataInfo;

        EnableItem selectitem = UserGameData.Get().GetEnableItem(slot.EquipDataInfo.ItemId);
        EquipReforgeData refiningdata = PopupManager.Instance.EquipReforgeDatas.Where(x => x.equipType == slot.EquipDataInfo.itemType && x.grade == slot.EquipDataInfo.Grade
          && x.reforge == selectitem.refining && x.gradeIndex == slot.EquipDataInfo.gradeIndex).FirstOrDefault();
        EquipAbilityData abilitydata = PopupManager.Instance.EquipAbilityDatas.Find(x=>x.Index==(slot.EquipDataInfo.ItemId)); 
        EquipPromotionData NowPromotioninfo = PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == slot.EquipDataInfo.itemType
              && n.qualityIndex == slot.EquipDataInfo.qualityIndex && n.promotionTier == selectitem.promotionTier).FirstOrDefault();
        
        AwakeSlot.SetData(itemdata);
        AwakeSlot.SetComposeDataText(itemdata);
        if (selectitem.promotionTier == 1)
        {
            AwakeSlot.ReInit(6);
        }
        AwakeAddEffect.text=string.Format(LocalizeManager.Instance.GetTXT("STR_UI_EQUIP_PROMOTION_DONEINFO"), NowPromotioninfo.maxReforge);
        if (itemdata.skillinfoID > 0)
        {
            AwakeSkillObj.SetActive(true);
            SkillInfoData skillinfo = SkillManager.Instance.GetSkillData(itemdata.skillinfoID);
            AwakeSkillicon.sprite = SkillSystem.GetSkillSprite(skillinfo.skillIcon);
            AwakeSkillName.text = LocalizeManager.Instance.GetTXT(skillinfo.skillName);
            AwakeSkillBeforeLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), selectitem.promotionTier);
            AwakeSkillNowLevel.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_SKILL_010"), selectitem.promotionTier + 1);
            List<float> skillval = SkillSystem.GetTooltipApplyInfo(skillinfo);
            float levelvalue = SkillSystem.GetLevelValue(skillinfo);
            if (skillval.Count > 0)
            {
                SkillLevelData nowlevelinfo = SkillManager.Instance.SkillLevelDatas.Where(x => x.groupID == skillinfo.skillLevelId ).FirstOrDefault();
                if (nowlevelinfo != null) 
                {
                    AwakeSkillEffect.text = string.Format(LocalizeManager.Instance.GetTXT(skillinfo.skillLvUpTooltip), (skillval[0] + levelvalue* (selectitem.promotionTier + 1)) * 100);
                }
            }
        }
        else
        {
            AwakeSkillObj.SetActive(false);
        }
        StartCoroutine(CheckAnimation());
        UpDateSizeFitter();
    }

    IEnumerator CheckAnimation()
    {
        while (Resultami.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.3f);
        if (NormalRefining.activeSelf)
            Slot.PlayStarAni(Slot._EquipDataInfo);
        if (AwakeRefining.activeSelf)
        {
            EnableItem selectitem = UserGameData.Get().GetEnableItem(itemdata.ItemId);
            EquipPromotionData Promotioninfo= PopupManager.Instance.EquipPromotionDatas.Where(n => n.equipType == itemdata.itemType
               && n.qualityIndex == itemdata.qualityIndex && n.promotionTier == selectitem.promotionTier).FirstOrDefault();
            int count = Promotioninfo.maxReforge - selectitem.refining;
            for (int i = 0; i < Awakestarani.Length; i++)
            {
                Awakestarani[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < count; i++)
            {
                Awakestarani[i].gameObject.SetActive(true);
                Awakestarani[i].SetTrigger("StartScale");
                while (Awakestarani[i].GetCurrentAnimatorStateInfo(0).normalizedTime < 1.5)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

    }
    void UpDateSizeFitter()
    {
        for (int i = 0; i < sizefitterlist.Count; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sizefitterlist[i].transform);
        }
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
