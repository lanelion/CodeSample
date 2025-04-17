using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class BattleStage_Raid : BattleStageBase
{
    public enum eStageProcType
    {
        WAIT_MONSTERSPAWN,
        BATTLE,
        WAIT_NEXTSTEP,
        WAIT_RESULT,
    }
    public static BattleStage_Raid Get()
    {
        return (BattleStage_Raid)StageManager.Instance.GetBattleStage(eBattleStageType.RAID);
    }

    public override eBattleStageType GetStageType() { return eBattleStageType.RAID; }
    public int TotalRewardGold     //총획득 골드량
    {
        get
        {
            return _totalRewardGold;
        }
        set
        {
            Debug.LogFormat("_totalRewardGold : {0} / {1}", _totalRewardGold, value);
            _totalRewardGold = value;
        }
    }
    public int _totalRewardGold = 0;     //총획득 골드량


    public int TotalBoxDropCount = 0;   //총박스 획득량
    public int TotalRewardSton = 0;
    public int TotalExp = 0;
    public float RemainTime = 0;
    public float LimitMaxTime = 0;
    public float CostTime = 0f;
     long TotalDamage = 0;
    public long m_TotlaDamge
    {
        get { return TotalDamage; }
    }
    public RaidReWard raidrward = null;
    public List<BoxRewardInfo> BoxRewardList = new List<BoxRewardInfo>(); //획득 보상상자 리스트.
    bool isEnd = false;
    public bool awaking=false;
    public int CurStageGroupID = 1; //현재 도전 스테이지 그룹.
    public List<StageRaidData> CurStageInfoList = new List<StageRaidData>();

    public ActorMonster _curboss;
    private int CurStep = 0;
    float NextStepWaitTiem = 0;
    public int NowStep
    {
        get { return CurStep; }
    }

    private eStageProcType StageProcType = eStageProcType.WAIT_MONSTERSPAWN;
    private float SpawnWaitTime = 0;

    float findTargetTimer = 0;

    EntityManager manager;
    public override void StartBattle()
    {
        base.StartBattle();

        StageProcType = eStageProcType.BATTLE;

        CurStep = 0;
        CurStageInfoList.Clear();
        CurStageInfoList = GetStageGroupDataList(CurStageGroupID);


        TotalRewardGold = 0;
        TotalRewardSton = 0;
        TotalBoxDropCount = 0;
        TotalExp = 0;
        TotalDamage = 0; 
        awaking = false;
        UIBattleRoot_Raid.Get().groggybar.isEnd = false;
         //BoxRewardList.Clear();
         UIBattleRoot_Raid.Get()._dicDamageSlot = new Dictionary<int, UIDamageSlot>();
        UIBattleRoot_Raid.Get().DamageSlotList = new List<UIDamageSlot>();
        isEnd = false;
        if (!float.TryParse(DataManager.Instance.CommonDatas.Find(x => x.DataType == "undermineWaveFlowTime").DataValues[0], out NextStepWaitTiem))
            NextStepWaitTiem = 5;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < StageManager.Instance.DropObjList.Count; i++)
        {
            manager.DestroyEntity(StageManager.Instance.DropObjList[i]);
        }
        StageManager.Instance.DropObjList.Clear();
        //InfiniteWarSpawnManager.Get().StartBattle(CurChapter, CurStage);

        CharacterManager.Instance.UpdateEnable = false;

        UIPopupHandler.Get().OpenPopup(ePopupType.LOADING_GAME);
    }

    public override IEnumerator LoadBattleSync()
    {

        yield return LoadBattleScene();
        yield return null;

        FollowCam.Get().SetCamOffset("CamNormalType");

        SettingSpawnPos();
        RandomPosSet();

        SpawnMyAlly();

        UIManager.Instance.SetDongenPopupType = eContentType.RAID;
        yield return null;

        yield return LoadingComplete();

        yield return WaitForGameStart();
        StageManager.Instance.SetLight();
    }

    private IEnumerator LoadBattleScene()
    {
        CurStep = 0;
        TotalDamage = 0;
        CurStageInfoList = GetStageGroupDataList(CurStageGroupID);
        if (CurStageInfoList.Count > 0)
        {
            SceneLoadManager.Instance.LoadBattleScene(CurStageInfoList[CurStep].stageScene);
            UIBattleRoot_Raid.Get().groggybar.EnterStatus(CurStageInfoList[0]);
        }
        else
        {
            Debug.LogErrorFormat("!!!!!!!!! LoadBaseScene - _curMapInfo == null: {0}", CurStageGroupID);
        }

        yield return null;
        Scene checkScene;
        bool check = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            checkScene = SceneManager.GetSceneAt(i);
            if (checkScene.name == CurStageInfoList[CurStep].stageScene)
            {
                if (check)
                {
                    SceneManager.UnloadSceneAsync(checkScene);
                }
                else
                {
                    SceneManager.SetActiveScene(checkScene);
                    check = true;
                }
            }
            else
                SceneManager.UnloadSceneAsync(checkScene);
        }
        LimitMaxTime = CurStageInfoList[0].stageTimeLimit;
        RemainTime = LimitMaxTime;
    }

    public IEnumerator LoadingComplete()
    {
        yield return null;

        while (UIStage_Battle.Get() == null)
        {
            yield return Yielders.Get(.1f);
        }

        UIStage_Battle.Get().LoadingComplete();

        UIPopupHandler.Get().ClosePopup(ePopupType.LOADING_GAME);
        UserManager.Instance.CheckBuff();

        UIBattleRoot_Raid.Get().groggybar.EnterStatus(CurStageInfoList[0]);
    }

    private IEnumerator WaitForGameStart()
    {
        yield return null;
        FirstSpawn();
        yield return new WaitUntil(() => SceneLoadManager.Instance.fadeCanvasGroup.blocksRaycasts == false);
        UIBattleRoot_Raid.Get().groggybar.isEnd = false;
        UIBattleRoot_Raid.Get().groggybar.SetTime(LimitMaxTime, RemainTime);
        yield return Yielders.Get(1f);

        CharacterManager.Instance.UpdateEnable = true;
    }
    
    public override void UpdateEx()
    {
        base.UpdateEx();

        if (CharacterManager.Instance.UpdateEnable == false)
        {
            return;
        }
        if (CharacterManager.Instance.UpdateEnable == true)
        {
            CheckLimitTime();
        }

        switch (StageProcType)
        {
            case eStageProcType.WAIT_MONSTERSPAWN:
                {
                    if (Time.time > SpawnWaitTime)
                    {
                        UIManager.Instance.CreatePopMessage(PopMessageType.SimpleMessage, _popMsg =>
                        {
                            _popMsg.GetComponent<PopSimpleMessage>().Init(null, PopSimpleMessageType.PopSimpleMsg, CurStageInfoList[CurStep].stageNameText);
                        });

                        SettingSpawnPos();
                        RandomPosSet();
                        
                        SwapBoss();

                        StageProcType = eStageProcType.BATTLE;
                    }
                }
                break;

            case eStageProcType.BATTLE:
                {
                    if (findTargetTimer < Time.time)
                    {
                        foreach (KeyValuePair<int, GroupClass> group in groupDic)
                        {
                            group.Value.CheckTargetState();
                        }
                        findTargetTimer = Time.time + .2f;
                    }
                }
                break;

            case eStageProcType.WAIT_RESULT:
                {

                }
                break;
        }

    }

    private void CheckLimitTime()
    {

        if (!isEnd && UIBattleRoot_Raid.Get().groggybar.isEnd)
        {
            StageClear();
            isEnd = true;
        }
        return;

    }

    private void NextStep()
    {
        if (StageProcType != eStageProcType.BATTLE)
        {
            return;
        }

        StageProcType = eStageProcType.WAIT_NEXTSTEP;
        StageManager.Instance.StartCoroutine(NextStepProcess());
        UserManager.Instance.UserGameDataInfo.Notify_EventChangeInfo();
        UserManager.Instance.SetEventCount(eQuestType.RAIDCLEAR);
        UserManager.Instance.SetEventCount(eQuestType.RAIDCLEAR_TIMELIMIT);
        UserManager.Instance.SetEventCount(eQuestType.RAIDCLEAR_NPSKILL);
    }

    public IEnumerator NextStepProcess()
    {
        yield return Yielders.Get(.1f);
        
        CurStep++;
        UIBattleRoot_Raid.Get().RefreshReward();
        if (CurStep >= CurStageInfoList.Count)
        {
            StageProcType = eStageProcType.WAIT_RESULT;

            StageClear();
        }
        else
        {
            StageProcType = eStageProcType.WAIT_MONSTERSPAWN;
            SpawnWaitTime = Time.time + NextStepWaitTiem;

        }

    }


    public void StageFail(GAME_FINISH_TYPE _finishType)
    {
        CharacterManager.Instance.UpdateEnable = false;
        isEnd = true;
        PopupManager.Instance.CreatePopup(PopupType.PopupStageGameOver, true, _popup =>
        {
            PopupStageGameOver _gameOverPopup = _popup.GetComponent<PopupStageGameOver>();
            _gameOverPopup.GameOverAndClearOff(true, false, NextStage);
            _gameOverPopup.Init(() =>
            {
                if (PopupManager.Instance.IsPopupCheck(PopupType.PopupRaid) == false)
                {
                    PopupManager.Instance.CreatePopup(PopupType.PopupRaid, false, _dungeonPopup =>
                    {
                        _dungeonPopup.GetComponent<PopupRaid>().Init(null);
                    });
                }
            });
        });
        UIBattleRoot_Raid.Get(). groggybar.ExitStatus();



        string state = "Weapon" + UserGameData.Get().EquipWeaponItemid + "SubWeapon" + UserGameData.Get().EquipSubWeaponItemid +
           "Gold" + UserGameData.Get().AssetGold + "Pearl" + UserGameData.Get().AssetPearl;
        string skilldata = "Skill";
        if (UserGameData.Get()._DicActSlot.ContainsKey(UserGameData.Get().NowSlotNum))
        {
            for (int i = 0; i < UserGameData.Get()._DicActSlot[UserGameData.Get().NowSlotNum].Count; i++)
            {
                skilldata += UserGameData.Get()._DicActSlot[UserGameData.Get().NowSlotNum][i] + "#";
            }
        }
        if (UserGameData.Get()._DicPassSlot.ContainsKey(UserGameData.Get().NowSlotNum))
        {
            skilldata += "/Passive:";
            for (int i = 0; i < UserGameData.Get()._DicPassSlot[UserGameData.Get().NowSlotNum].Count; i++)
            {
                skilldata += UserGameData.Get()._DicPassSlot[UserGameData.Get().NowSlotNum][i] + "#";
            }
        }
        state += skilldata;
        NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, eContentType.RAID.ToString(), "CLEARFAIL", state, "TotalDmage:" + TotalDamage + _finishType);
        SendRaidRank();
    }

    public void StageClear()
    {

        FollowCam.Get().SetCamOffset("CamNormalType");

        CharacterManager.Instance.UpdateEnable = false;
        UIManager.Instance.StopAllCoroutines();
        isEnd = true;
        UIBattleRoot_Raid.Get().groggybar.ExitStatus();

        {
            //총획득 골드 저장, 
            UserManager.Instance.UserGameDataInfo.AddGold(TotalRewardGold);




            List<StageFirstRewardData> dictotaldata = NetworkManager.Instance.GetContensGroupData(eContentType.RAID);




            PopupManager.Instance.CreatePopup(PopupType.PopupStageGameOver, true, _popup =>
            {
                PopupStageGameOver _gameOverPopup = _popup.GetComponent<PopupStageGameOver>();
                _gameOverPopup.GameOverAndClear(GAME_FINISH_TYPE.STAGE_CLEAR, null, NextStage);
                _gameOverPopup.Init(() =>
                {
                });
            });
            string state = "Weapon " + UserGameData.Get().EquipWeaponItemid + "#" + "SubWeapon " + UserGameData.Get().EquipSubWeaponItemid + "#" + "Accessory ";
            foreach (int accindex in UserGameData.Get().EquipAccessoryItemidlist.Values)
            {
                state += accindex + "#";
            }
            state += "Avata " + UserGameData.Get().EquipAvataIndex;
            string skilldata = "Skill";
            if (UserGameData.Get()._DicActSlot.ContainsKey(UserGameData.Get().NowSlotNum))
            {
                for (int i = 0; i < UserGameData.Get()._DicActSlot[UserGameData.Get().NowSlotNum].Count; i++)
                {
                    skilldata += UserGameData.Get()._DicActSlot[UserGameData.Get().NowSlotNum][i] + "#";
                }
            }
            if (UserGameData.Get()._DicPassSlot.ContainsKey(UserGameData.Get().NowSlotNum))
            {
                skilldata += "/Passive:";
                for (int i = 0; i < UserGameData.Get()._DicPassSlot[UserGameData.Get().NowSlotNum].Count; i++)
                {
                    skilldata += UserGameData.Get()._DicPassSlot[UserGameData.Get().NowSlotNum][i] + "#";
                }
            }
            state += skilldata;
            NetworkManager.Instance.SendLog(eTypeLogCode.TRACE, eContentType.RAID.ToString(), "CLEARSTAGE", state, "TotalDmage:" + TotalDamage);
            SendRaidRank();

        }
    }

    public void NextStage()
    {
       
        DOTween.CompleteAll();

        UIPopupHandler.Get().CloseAllPopup();

        CharacterManager.Instance.UpdateEnable = false;

        StageData _stage = UIManager.Instance.StageDatas.Find(x => x.chapterID == UserGameData.Get().Infinity_CurChapterNum && x.stageIndex == UserGameData.Get().Infinity_CurStageNum);
        string stagename = "";
        if (_stage != null)
            stagename = LocalizeManager.Instance.GetTXT(_stage.stageNameText);
        string _strnowstage = string.Format("{0}-{1}", UserGameData.Get().Infinity_CurChapterNum, UserGameData.Get().Infinity_CurStageNum);
        CommonData stageimg = DataManager.Instance.CommonDatas.Find(x => x.DataType == "chapterTitleSubImage");
        SceneLoadManager.Instance.SetStageimage(stageimg.DataValues[UserGameData.Get().Infinity_CurChapterNum - 1]);
        SceneLoadManager.Instance.TransitionerFadeInOut_Action(stagename, _strnowstage, 0.5f, () => StageManager.Instance.ChangeBattleStage(eBattleStageType.INFINITE_WAR));
        UIManager.Instance.BattleUI.LoadingComplete();
    }

    public void GameReStart()
    {
       
        DOTween.CompleteAll();

        UIPopupHandler.Get().CloseAllPopup();

        CharacterManager.Instance.UpdateEnable = false;

        StageData _stage = UIManager.Instance.StageDatas.Find(x => x.chapterID == UserGameData.Get().Infinity_CurChapterNum && x.stageIndex == UserGameData.Get().Infinity_CurStageNum);
        string stagename = "";
        if (_stage != null)
            stagename = LocalizeManager.Instance.GetTXT(_stage.stageNameText);
        string _strnowstage = string.Format("{0}-{1}", UserGameData.Get().Infinity_CurChapterNum, UserGameData.Get().Infinity_CurStageNum);
        CommonData stageimg = DataManager.Instance.CommonDatas.Find(x => x.DataType == "chapterTitleSubImage");
        SceneLoadManager.Instance.SetStageimage(stageimg.DataValues[UserGameData.Get().Infinity_CurChapterNum - 1]);
        SceneLoadManager.Instance.TransitionerFadeInOut_Action(stagename, _strnowstage, 0.5f, () => StageManager.Instance.ChangeBattleStage(eBattleStageType.INFINITE_WAR));
        UIManager.Instance.BattleUI.LoadingComplete();

    }


    private void SendRaidRank()
    {
        RecvRaidRank _data = new RecvRaidRank();
        _data.photo = UserManager.Instance.UserGameDataInfo.Avataicon;
        _data.nickname = UserManager.Instance.UserGameDataInfo.NickName;
        _data.score = TotalDamage.ToString();
        _data.zone = "kr";

        NetworkManager.Instance.SEND_SetRaidRank(_data, null, null);
    }


    public void StayLoadingvoid()
    {
        if (UIManager.Instance.SetPopupType == eFailGuideShortcutType.NONE)
        {
            return;
        }

        switch (UIManager.Instance.SetPopupType)
        {
            case eFailGuideShortcutType.AVATAR:
                {
                    if (PopupManager.Instance.IsPopupCheck(PopupType.PopupAvatarGrowth) == false)
                    {
                        PopupManager.Instance.CreatePopup(PopupType.PopupAvatarGrowth, true, _popup => { _popup.GetComponent<PopupAvatarGrowth>().Init(null); });
                    }
                }
                break;
            case eFailGuideShortcutType.BUFF:
                {

                }
                break;
            case eFailGuideShortcutType.GROW1:
                {
                    if (PopupManager.Instance.IsPopupCheck(PopupType.PopupHeroGrowth) == false)
                    {
                        PopupManager.Instance.CreatePopup(PopupType.PopupHeroGrowth, true, _popup => { _popup.GetComponent<PopupHeroGrowth>().Init(null); });
                    }
                }
                break;
            case eFailGuideShortcutType.EQUIP:
                {
                    if (PopupManager.Instance.IsPopupCheck(PopupType.PopupItemGrowth) == false)
                    {
                        PopupManager.Instance.CreatePopup(PopupType.PopupItemGrowth, true, _popup => { PopupItemGrowth itempopup = _popup.GetComponent<PopupItemGrowth>(); itempopup.Init(null); });
                    }
                }
                break;
            case eFailGuideShortcutType.SKILL:
                {
                    PopupManager.Instance.CreatePopup(PopupType.PopupSkillGrowth, true, _popup =>{ _popup.GetComponent<PopupSkillGrowth>().Init(null); });
                }
                break;
            case eFailGuideShortcutType.GACHA:
                {
                    if (PopupManager.Instance.IsPopupCheck(PopupType.PopupGacha) == false)
                    {
                        PopupManager.Instance.CreatePopup(PopupType.PopupGacha, true, _popup => { _popup.GetComponent<PopupGacha>().Init(null); });
                    }
                }
                break;
        }
        UIManager.Instance.SetPopupType = eFailGuideShortcutType.NONE;
    }
    public void SetDamage(long Damage)
    {
        TotalDamage += Damage;
        UIBattleRoot_Raid.Get().RefreshDPS();
    }
    public override Vector3 CheckMapEage(Vector3 pos)
    {
        return base.CheckMapEage(pos);
    }
    public List<StageRaidData> GetStageGroupDataList(int _groupID)
    {
        List<StageRaidData> list = new List<StageRaidData>();
        foreach (StageRaidData item in UIManager.Instance.StageRaidDatas)
        {
            if (item.stageGroupID == _groupID)
            {
                list.Add(item);
            }
        }

        return list;
    }
}
