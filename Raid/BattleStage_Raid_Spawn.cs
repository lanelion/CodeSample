using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class BattleStage_Raid
{
    void FirstSpawn()
    {
        groupDic.Clear();
        CreateBoss(CurStageInfoList[CurStep].bossID);
        
    }

    public void SpawnMyAlly()
    {
        CommonData charspawn = DataManager.Instance.CommonDatas.Find(x => x.DataType == "raidCharSpawn");
        if (CharacterManager.Instance.MyActor == null)
        {
            ActorUser _myActor = CharacterManager.Instance.CreateUser(Util.GetLocalID(), 10, Vector3.zero, Vector3.forward);
            CharacterManager.Instance.MyActor = _myActor;
            CharacterManager.Instance.MyActor.action.SetAction(eActionType.IDLE);
        }
        if (charspawn != null)
        {
            float.TryParse(charspawn.DataValues[0], out float posx);
            float.TryParse(charspawn.DataValues[1], out float posy);
            float.TryParse(charspawn.DataValues[2], out float posz);
            Vector3 spawnpos = new Vector3(posx, posy, posz);
            CharacterManager.Instance.MyActor.SetPosition(spawnpos);
        }
        CharacterManager.Instance.MyActor.stat.CurHP = CharacterManager.Instance.MyActor.stat.maxHp;

        FollowCam.Get().SetFollowTarget(CharacterManager.Instance.MyActor.TF);
    }

    GroupClass SpawnMonster(int _groupID)
    {
        if (_groupID <= 0)
        {
            return null;
        }

        // 위치랜덤
        SpawnPos sPos = GetSpawnPos();
        // 소환
        CreatureGropupData creatureGroup = CharacterManager.Instance.CreatureGropupDatas.Find(x=>x.CreatureGroupID== _groupID);
        Vector3[] r = randomPos.OrderBy(n => UnityEngine.Random.value).ToArray();

        List<ActorBase> groupUnit = new List<ActorBase>();
        int rcount = 0;
        float AttackSight = 0;
        foreach (var i in creatureGroup.Creatures)
        {
            ActorMonster _monster = CharacterManager.Instance.CreateMonster(Util.GetLocalID(), i, (int)TEAM.RED, new Vector3(sPos.pos.x, 0, sPos.pos.y) + r[rcount], Vector3.zero, sPos.id);
            groupUnit.Add(_monster);

            AttackSight = ((ActorMonster)_monster).creature_aidata.CreatureAIData.AttackSight;
            _monster.CreaureGroupID = creatureGroup.CreatureGroupID;

            rcount++;
        }

        GroupClass _groupClass = new GroupClass()
        {
            GroupID = sPos.id,
            checkRadius = AttackSight,
            target = null,
            units = groupUnit,
            aggroType = creatureGroup.AggroType
        };

        if (_groupClass.aggroType == eTypeAggro.INSTANT)
        {
            _groupClass.checkRadius = float.MaxValue;
            _groupClass.FindTarget();
        }

        return _groupClass;
    }


    void CreateBoss(int _groupID)
    {
        if (_groupID <= 0)
        {
            return;
        }
        if (_curboss != null)
        {
            PoolInfoManager.Instance.Push(_curboss.gameObject);
            _curboss = null;
        }
        UIManager.Instance.StartCoroutine(SpawnBoss());
    }
    public void SwapBoss()
    {
        if (awaking)
        {
            CurStep++;
            if (CurStep >= CurStageInfoList.Count)
                CurStep = CurStageInfoList.Count - 1;

        }
        StageRaidData raidbossinfo = UIManager.Instance.StageRaidDatas.Find(x=>x.Index== CurStep + 1);
        if (raidbossinfo==null)
        {
            CurStep = 0; 
            raidbossinfo = UIManager.Instance.StageRaidDatas.Find(x => x.Index == CurStep + 1);

        }
        CreatureData awakecreature = CharacterManager.Instance.CreatureDatas.Find(x=>x.Index==(raidbossinfo.bossTransformID));
        CreatureGropupData creatureGroup = CharacterManager.Instance.CreatureGropupDatas.Find(x => x.CreatureGroupID == raidbossinfo.bossID);

        CommonData spawnposdata = DataManager.Instance.CommonDatas.Find(x => x.DataType == "raidSpawn");
        float.TryParse(spawnposdata.DataValues[0], out float posx);
        float.TryParse(spawnposdata.DataValues[1], out float posy);
        float.TryParse(spawnposdata.DataValues[2], out float posz);
        Vector3 spawnpos = new Vector3(posx, posy, posz);
        ActorMonster _monster = null;
        if (awaking)
        {
            _monster = CharacterManager.Instance.SwapMonster(Util.GetLocalID(), raidbossinfo.bossID, (int)TEAM.RED, _curboss.gameObject.transform.position, Vector3.zero, _curboss, awaking);
            awaking = false;
            _monster.NavAgent.enabled = true; 
            _monster.battle.SetTargetActor(CharacterManager.Instance.MyActor);
            _monster.stat.raidawaken = true;
            _monster.skill.InitMonsterSkill(raidbossinfo.transformEndSkill);
            UIBattleRoot_Raid.Get().groggybar.EnterStatus(raidbossinfo);
        }
        else
        {
            _monster = CharacterManager.Instance.SwapMonster(Util.GetLocalID(), awakecreature.Index, (int)TEAM.RED, _curboss.gameObject.transform.position, Vector3.zero, _curboss, awaking);
            UserManager.Instance.SetEventCount(eQuestType.RAIDCLEAR);
            UserManager.Instance.SetEventCount(eQuestType.RAIDCLEAR_TIMELIMIT);
            UserManager.Instance.SetEventCount(eQuestType.RAIDCLEAR_NPSKILL);
            _monster.battle.SetTargetActor(CharacterManager.Instance.MyActor);
            _monster.skill.InitMonsterSkill(raidbossinfo.transformStartSkill);
            _monster.stat.raidawaken = false;
            awaking = true;
            UIBattleRoot_Raid.Get().groggybar.barAni.ResetBar();
            UIBattleRoot_Raid.Get().groggybar.SetRaidAwake(raidbossinfo.transformEndSkill);
        }
        _monster.battle.SetTargetActor(CharacterManager.Instance.MyActor);
        _curboss = _monster;
        _curboss.hud.ActiveCharHUD(false);
    }
    void CreateMiddleBoss(int _groupID)
    {

        if (_groupID <= 0)
        {
            return;
        }

        CreatureGropupData creatureGroup = CharacterManager.Instance.CreatureGropupDatas.Find(x=>x.CreatureGroupID== _groupID);
        if (creatureGroup == null)
        {
            Debug.LogFormat(" CreateBoss - creatureGroup == null : {0}", _groupID);
            return;
        }

        ActorMonster _monster = CharacterManager.Instance.CreateMonster(Util.GetLocalID(), creatureGroup.Creatures[0], (int)TEAM.RED, new Vector3(0, 0, 13), Vector3.zero);

        Comp_HUDMonster _monhud = _monster.hud as Comp_HUDMonster;
        _monhud.SetBossname(LocalizeManager.Instance.GetTXT(_monster.TableDataInfo.NameText));
        _monster.battle.SetTargetActor(CharacterManager.Instance.MyActor);
    }
    IEnumerator BossCamMoving(GameObject obj)
    {
        FollowCam.Get().SetFollowTarget(obj.transform);
        yield return Yielders.Get(1f);
        FollowCam.Get().SetFollowTarget(CharacterManager.Instance.MyActor.TF);
    }

    public IEnumerator SpawnBoss()
    {
        UIManager.Instance.CreatePopMessage(PopMessageType.BossMessage, _popMsg =>
        {
            _popMsg.GetComponent<PopBossMessage>().Init(null);
        });
        CommonData spawnposdata = DataManager.Instance.CommonDatas.Find(x => x.DataType == "raidSpawn");
        float.TryParse(spawnposdata.DataValues[0], out float posx);
        float.TryParse(spawnposdata.DataValues[1], out float posy);
        float.TryParse(spawnposdata.DataValues[2], out float posz);
        Vector3 spawnpos = new Vector3(posx, posy, posz);
        StageRaidData raidbossinfo = UIManager.Instance.StageRaidDatas.Find(x => x.Index == CurStep + 1);
        if (raidbossinfo==null)
        {
            CurStep = 0;
            raidbossinfo = UIManager.Instance.StageRaidDatas.Find(x => x.Index == CurStep + 1);

        }
        CharacterManager.Instance.UpdateEnable = false;

        ActorMonster _monster = CharacterManager.Instance.CreateMonster(Util.GetLocalID(), raidbossinfo.bossID, (int)TEAM.RED, spawnpos, Vector3.zero);
        _monster.battle.SetTargetActor(CharacterManager.Instance.MyActor);
        if (_monster == CharacterManager.Instance.MyActor.battle.GetTargetActor())
        {
            CharacterManager.Instance.MyActor.battle.InitTargetChar();
        }
        CharacterManager.Instance.MyActor.battle.SetTargetActor(_monster);

        FollowCam.Get().SetCamOffset(_monster.TableDataInfo.CamOffsetType);

        UIBattleRoot_Raid.Get().groggybar.SetRaidBoss(_monster);
        _curboss = _monster;
        UIManager.Instance.StartCoroutine(BossCamMoving(_monster));
        UIBattleStatusInfo.Get().NotifySpawnBoss(_monster);
        yield break;
    }
    IEnumerator BossCamMoving(ActorBase obj)
    {
        obj.gameObject.SetActive(false);
        FollowCam.Get().SetFollowTarget(obj.TF);
        CharacterManager.Instance.UpdateEnable = false;
        yield return Yielders.Get(1f);
        obj.gameObject.SetActive(true);
        obj.action.ChangeAction(eActionType.SPAWN);
        CharacterManager.Instance.UpdateEnable = false;
        yield return Yielders.Get(0.5f);
        FollowCam.Get().SetFollowTarget(CharacterManager.Instance.MyActor.TF);
        CharacterManager.Instance.MyActor.battle.SetTargetActor(obj);
    }
}
