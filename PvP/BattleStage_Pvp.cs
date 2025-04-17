using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using DG.Tweening;
using System.Linq;

public partial class BattleStage_Pvp : BattleStageBase
{
    public enum eStageProcType
    {
        WAIT_MONSTERSPAWN,
        BATTLE,
        WAIT_NEXTSTEP,
        WAIT_RESULT,
    }
    public static BattleStage_InfiniteWar Get()
    {
        return (BattleStage_InfiniteWar)StageManager.Instance.GetBattleStage(eBattleStageType.PVP);
    }

    public override eBattleStageType GetStageType() { return eBattleStageType.PVP; }
    EntityManager manager;
    //적유저오브젝트
    [System.NonSerialized] public ActorEnemyUser _EnemyUser = null;
    private eStageProcType StageProcType = eStageProcType.WAIT_MONSTERSPAWN;
    float findTargetTimer = 0;
    public float RemainTime = 0;
    public float LimitMaxTime = 0;
    public float CostTime = 0f;
    //적유저 데이터
    UserGameData EnemyUserData = null;
    public override void StartBattle()
    {
        base.StartBattle();

        CharacterManager.Instance.UpdateEnable = false;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < StageManager.Instance.DropObjList.Count; i++)
        {
            manager.DestroyEntity(StageManager.Instance.DropObjList[i]);
        }
        StageManager.Instance.DropObjList.Clear();

        StageProcType = eStageProcType.BATTLE;
        UIPopupHandler.Get().OpenPopup(ePopupType.LOADING_GAME);
    }

    public override IEnumerator LoadBattleSync()
    {

        yield return LoadBattleScene();
        yield return null;

        FollowCam.Get().SetCamOffset("CamNormalType");
        
        CharacterManager.Instance.UpdateEnable = false;
        //유저스폰
        SpawnUser();
        //상대스폰
        FirstSpawn();

        yield return null;

        yield return LoadingComplete();

        yield return WaitForGameStart();
    }

    private IEnumerator LoadBattleScene()
    {
            SceneLoadManager.Instance.LoadBattleScene("StageArena_01");

        yield return null;

        Scene checkScene;
        bool check = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            checkScene = SceneManager.GetSceneAt(i);
            if (checkScene.name == "StageArena_01")
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
        RandomPosSet();
        CommonData _timedata= DataManager.Instance.CommonDatas.Find(x => x.DataType == "arenaRemainTimeLimit");
        if (_timedata != null)
            float.TryParse(_timedata.DataValues[0], out LimitMaxTime);
        else
            LimitMaxTime = 60f;
        RemainTime = LimitMaxTime;

        StageManager.Instance.SetLight();
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

    }

    private IEnumerator WaitForGameStart()
    {
        yield return Yielders.Get(1.5f);

        UIManager.Instance.CreatePopMessage(PopMessageType.BattleReadyMessage, _popMsg =>
        {
            _popMsg.GetComponent<PopBattleReadyMessage>().Init(null);
        });

        UIBattleRoot_Pvp.Get().gameObject.SetActive(true);
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
                    SettingSpawnPos();
                    RandomPosSet();
                    StageProcType = eStageProcType.BATTLE;

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
        CostTime += Time.deltaTime;
        RemainTime -= Time.deltaTime;
        if (RemainTime <= 0)
        {
            RemainTime = 0;
            CostTime = 0f;
            NotifyTimeOut();
            return;
        }
    }


    public void StageFail(GAME_FINISH_TYPE _finishType)
    {

        CharacterManager.Instance.UpdateEnable = false;
        PopupManager.Instance.CreatePopup(PopupType.PopupStageGameOver, true, _popup =>
        {
            PopupStageGameOver _gameOverPopup = _popup.GetComponent<PopupStageGameOver>();
            _gameOverPopup.GameOverAndClearOff(true, false, NextStage);
            _gameOverPopup.Init(() =>
            {
                if (PopupManager.Instance.IsPopupCheck(PopupType.PopupPvp) == false)
                {
                    PopupManager.Instance.CreatePopup(PopupType.PopupPvp, false, _dungeonPopup =>
                    {
                        _dungeonPopup.GetComponent<PopupPVP>().Init(null);
                    });
                }
            });
        });
        EnemyUserData = null;
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
    }

    public void StageClear()
    {

        FollowCam.Get().SetCamOffset("CamNormalType");

        CharacterManager.Instance.UpdateEnable = false;
       
        {
            EnemyUserData = null;
            PopupManager.Instance.CreatePopup(PopupType.PopupStageGameOver, true, _popup =>
            {
                PopupStageGameOver _gameOverPopup = _popup.GetComponent<PopupStageGameOver>();
                _gameOverPopup.GameOverAndClear(GAME_FINISH_TYPE.STAGE_CLEAR, null, NextStage);
                _gameOverPopup.Init(() =>
                {
                    if (PopupManager.Instance.IsPopupCheck(PopupType.PopupPvp) == false)
                    {
                        PopupManager.Instance.CreatePopup(PopupType.PopupPvp, false, _dungeonPopup =>
                        {
                            _dungeonPopup.GetComponent<PopupPVP>().Init(null);
                        });
                    }
                });
            });
            
            
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
        EnemyUserData = null;
        
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
    public void OnClick_ExitDungeon()
    {
        if (PopupManager.Instance.IsPopupCheck(PopupType.PopupCommonMessage) == false)
        {
            PopupManager.Instance.CreatePopup(PopupType.PopupCommonMessage, true, _popup =>
            {
                PopupCommonMessage _msg = _popup.GetComponent<PopupCommonMessage>();
                _msg.Init(1, LocalizeManager.Instance.GetTXT("STR_UI_GROW_PROMOTION_COMBAT"), LocalizeManager.Instance.GetTXT("STR_UI_GROW_PROMOTION_COMBAT_INFO"), LocalizeManager.Instance.GetTXT("STR_UI_CONFIRM"), LocalizeManager.Instance.GetTXT("STR_UI_CANCEL"), _callBack =>
                {
                    if (_callBack) GameReStart();
                });
            });
        }
    }
    public void SetEnemyUserData(UserGameData enemy)
    {
        EnemyUserData = enemy;
    }
}
