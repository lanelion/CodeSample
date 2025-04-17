using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
public class DamageData
{
   public int skillindex;
    public long value;
}

public class UIBattleRoot_Raid : UIBattleRootBase
{
    static UIBattleRoot_Raid em = null;
    
    public static UIBattleRoot_Raid Get()
    {
        if (null == em)
            em = UIStage_Battle.Get().GetBattleRoot(eBattleStageType.RAID) as UIBattleRoot_Raid;

        return em;
    }
    public Text TotalDamageText = null;
    public Text TotalDamageValText = null;
    public Text DamageText = null;
    public ScrollRect ScrollPos = null;
    public Slider LimitTime = null;
    public Text WaveText = null;
    public Text BossNameText = null;
    public Image BossRomNameImage = null; 
    public ContentSizeFitter NameSizeFitter = null;
    public UIBattleStatus_Boss bossui = null;
    private float LastRefreshTime = 0;
    public UIDamageSlot damageslot =null;
    [SerializeField] ContentSizeFitter _slotsizefitter;
    [SerializeField] public Text RewardItemCountText;
    [SerializeField] public Image RewardItemIcon;
    [SerializeField] public GameObject RaidTipObj;
    [SerializeField] GameObject RaidToolTipObj;

    private bool isTouch = false;
    bool isTooltipOpen = false;
    long maxdamage = 0;
    public Dictionary<int, UIDamageSlot> _dicDamageSlot;
    public List<UIDamageSlot> DamageSlotList;
    public UIGroggyBar groggybar;
    public UIRaidHPBar RaidHpbar;
    List<DamageData> templist;
    List<StageRaidDamageData> hplist;

    public override eBattleStageType GetBattleStageType() { return eBattleStageType.RAID; }

    public override void LoadingComplete()
    {
        base.LoadingComplete();
        for(int i=0;i< DamageSlotList.Count; i++)
        {
            DamageSlotList[i].ResetData();
        }
        RefreshReward();
        RefreshDPS();
        hplist = UIManager.Instance.StageRaidDamageDatas.OrderBy(x => x.Index).ToList();
        RaidHpbar.EnterStatus(hplist);
        groggybar.SkillTimerBG.SetActive(false);
        groggybar.AwakenObj.SetActive(false);
        groggybar.BreakObj.SetActive(false);
        SetRaidTip(false); 
        CloseTooltip();
        isTouch = false;
    }

    public void RefreshReward()
    {
        int _totalRewardReforge = BattleStage_Raid.Get().TotalRewardSton;
        int _totalBoxDropCount = BattleStage_Raid.Get().TotalBoxDropCount;
        SetTimer();
        if (BattleStage_Raid.Get().NowStep + 1 <= BattleStage_Raid.Get().CurStageInfoList.Count)
            WaveText.text = string.Format(LocalizeManager.Instance.GetTXT("STR_UI_UNDERMINE_PHASEINFO"), BattleStage_Raid.Get().NowStep + 1, BattleStage_Raid.Get().CurStageInfoList.Count);
        
    }
    public void RefreshDPS()
    {
        long _totalDamage = BattleStage_Raid.Get().m_TotlaDamge;
        TotalDamageValText.text = string.Format("{0:#,###}", _totalDamage);
    }
    public void SetRaidTip(bool enable)
    {
        RaidTipObj.SetActive(enable);
    }
    //딜지분표기
    public void SetDamageData(int skillid,long Damge)
    {   //skillid가 -1이면평타
        RaidHpbar.SetDamge(Damge);
        if (_dicDamageSlot==null)
        {
            _dicDamageSlot = new Dictionary<int, UIDamageSlot>();
        }
        if (_dicDamageSlot.ContainsKey(skillid))
        {
            DamageSlotList.Find(match => match.skillid == skillid).SetData(skillid, Damge, BattleStage_Raid.Get().m_TotlaDamge);
        }
        else
        {
            GameObject go = Instantiate(damageslot.gameObject, ScrollPos.content);
            UIDamageSlot slot= go.GetComponent<UIDamageSlot>();
            slot.SetData(skillid, Damge, BattleStage_Raid.Get().m_TotlaDamge);
            DamageSlotList.Add(slot);
            _dicDamageSlot.Add(skillid, slot);
        }
        templist = new List<DamageData>();
        for (int i=0;i< DamageSlotList.Count; i++)
        {
            DamageData _damagedata = new DamageData { skillindex = DamageSlotList[i].skillid, value = DamageSlotList[i].TotalDamage };
            templist.Add(_damagedata);
            if (maxdamage < DamageSlotList[i].TotalDamage)
            {
                maxdamage = DamageSlotList[i].TotalDamage;
            }
        }

        templist = templist.OrderBy(x => x.value).Reverse().ToList();

        for (int i = 0; i < templist.Count; i++)
        {
            if (i < 5)
                DamageSlotList[i].gameObject.SetActive(true);
            else
                DamageSlotList[i].gameObject.SetActive(false);
            DamageSlotList[i].UpDateData(templist[i].skillindex, templist[i].value, BattleStage_Raid.Get().m_TotlaDamge);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_slotsizefitter.transform);


    }

    private void Update()
    {
        RefreshTime();
#if UNITY_EDITOR
        isTouch = Input.GetMouseButtonDown(0);
#else
        if (Input.touchCount > 0)
        {
            isTouch = ((Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetTouch(0).phase == TouchPhase.Moved));
        }
        else
        {
            isTouch = false;
        }
#endif

        if (isTooltipOpen && isTouch)
        {
            CloseTooltip();
        }
    }

        private void RefreshTime()
    {
        if (Time.time < LastRefreshTime)
        {
            return;
        }
    }

    /// <summary>
    /// OnClick
    /// </summary>
    public void OnClick_ExitDungeon()
    {
        PopupManager.Instance.CreatePopup(PopupType.PopupCommonMsg, true, _popup =>
        {
            PopupCommonMsg _popupCommonMsg = _popup.GetComponent<PopupCommonMsg>();
            _popupCommonMsg.Init(null);
            _popupCommonMsg.ShowMsgBox(LocalizeManager.Instance.GetTXT(BattleStage_Raid.Get().CurStageInfoList[0].stageNameText), LocalizeManager.Instance.GetTXT("STR_UI_UNDERMINE_PAUSE_PENALTY"))
            .UseTwoButton(GoToInfiniteWar, null).StopGame();
        });
    }

    private void GoToInfiniteWar()
    {
        groggybar.ExitStatus();
        CharacterManager.Instance.UpdateEnable = false;
        BattleStage_Raid.Get().GameReStart();
    }
    public void OnClick_TooltipBtn()
    {
        RaidToolTipObj.SetActive(true);
        isTooltipOpen = true;
    }
    public void CloseTooltip()
    {
        RaidToolTipObj.SetActive(false);
        isTooltipOpen = false;
    }
}
