using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using System.Linq;
public partial class BattleStage_Raid
{
    public override void NotifyActorDamaged(ActorBase _actor)
    {
        if (!_actor.IsMyActor)
        {
            if (_actor.battle.TeamID != CharacterManager.Instance.MyActor.battle.TeamID)
                _actor.stat.CurHP = _actor.stat.maxHp;
        }
    }

    public override void NotifyMonsterKill(ActorMonster _monster)
    {//레이드는여기안들어가고있음
        //소환수 이면, 
        if (_monster.OwnerActor != null)
        {
            return;
        }

        ActorBase _myActor = CharacterManager.Instance.MyActor;
        if (_myActor == null)
        {
            return;
        }

        if (_monster.GroupID >= 0 && _monster.TableDataInfo.CreatureType != eMonsterType.MIDDLE_BOSS)
        {
            if (groupDic.ContainsKey(_monster.GroupID) == false)
            {
                return;
            }

            groupDic[_monster.GroupID].units.Remove(_monster);
            if (groupDic[_monster.GroupID].units.Count == 0)
            {
                groupDic.Remove(_monster.GroupID);
            }
            
        }
        UserManager.Instance.SetKillCount();
        UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLALL);
        UserManager.Instance.UserGameDataInfo.AddSeasonPassExp(SeasonPassGoalType.KILLALL, 1);
        if (_monster.TableDataInfo.CreatureType == eMonsterType.NORMAL)
        {
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLNORMAL);
            UserManager.Instance.SetQuestCount(eQuestType.KILLNORMAL);
        }
        UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLALLRESET);
        if (_monster.TableDataInfo.CreatureType == eMonsterType.MIDDLE_BOSS || _monster.TableDataInfo.CreatureType == eMonsterType.ELITE)
        {
            UserManager.Instance.SetQuestCount(eQuestType.KILLONLYMIDDLE);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLALLBOSS);
            UserManager.Instance.AddPassExp(eQuestType.KILLALLBOSS, 1);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLONLYMIDDLE);
            UserManager.Instance.SetQuestCount(eQuestType.KILLALLBOSS);
        }
        if (_monster.TableDataInfo.CreatureType == eMonsterType.BOSS)
        {
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLALLBOSS);
            UserManager.Instance.AddPassExp(eQuestType.KILLALLBOSS, 1);
            UIStage_Battle.Get().GuideQuest.GuideQuestCount(eGuideType.KILLONLYBOSS);
            UserManager.Instance.SetQuestCount(eQuestType.KILLALLBOSS);
        }

        //Table_StageRuinsReward.Info CurStageRewardInfo = Table_StageRuinsReward.Get().GetData(_monster.CreaureGroupID);
        //if (CurStageRewardInfo != null)
        {
            //골드 획득
            /*int _rewardGold = UnityEngine.Random.Range(CurStageRewardInfo.rewardGold, CurStageRewardInfo.rewardGold + 1);
            int _rewardGold = (int)CurStageInfoList[CurStep].rewardGoldStandard;
            _rewardGold = (int)(_rewardGold * (1 + _myActor.buff.GetDungeonGold(SKILL_VALUE_TYPE.PERCENT)));
            _rewardGold += (int)_myActor.buff.GetDungeonGold(SKILL_VALUE_TYPE.INTERGER);
            DropGold(_monster.TF.position, _rewardGold);
            TotalRewardGold += _rewardGold;

            TotalRewardSton += (int)CurStageInfoList[CurStep].rewardSpiritCount;*/


            //rewardSpiritID
            //rewardSpiritCount

            /*
            int _boxChance = (int)(CurStageInfoList[CurStep].boxChance * (1 + _myActor.buff.GetDungeonItem(SKILL_VALUE_TYPE.PERCENT)));
            _boxChance += (int)_myActor.buff.GetDungeonItem(SKILL_VALUE_TYPE.INTERGER);
            float randomPer = UnityEngine.Random.Range(0f, 100f);
            if (randomPer < (float)(_boxChance / 100.0f))
            {*/
                //박스 획득
                TotalBoxDropCount += 1;

                /*BoxRewardInfo _boxRewardInfo = BoxRewardList.Find(match => match.BoxID == CurStageInfoList[CurStep].dropItemId);
                if (_boxRewardInfo == null)
                {
                    _boxRewardInfo = new BoxRewardInfo()
                    {
                        BoxID = CurStageInfoList[CurStep].dropItemId,
                        BoxCount = CurStageInfoList[CurStep].dropItemCount
                    };
                    BoxRewardList.Add(_boxRewardInfo);
                }
                else
                {
                    _boxRewardInfo.BoxCount += 1;
                }*/
            //}

            //경험치 획득
            //int _addExp = UnityEngine.Random.Range(CurStageRewardInfo.rewardExp, CurStageRewardInfo.rewardExp + 1);
            /*int _addExp = (int)CurStageInfoList[CurStep].rewardExpStandard;
            TotalExp += _addExp;*/
            /*bool isLevelup = UserGameData.Get().AddExp(_addExp);
            if (isLevelup)
            {
                Debug.Log("레벨업 이펙트 표시한다.");
            }*/
        }

        UIBattleRoot_Raid.Get().RefreshReward();

        //모든 적 서
        if (CharacterManager.Instance.IsAllMonsterDead() == true)
        {
            NextStep();
        }

        //int _rewardGold = UnityEngine.Random.Range(CurStageInfo.rewardGoldMin, CurStageInfo.rewardGoldMax + 1);
        //DropGold(_monster.TF.position, _rewardGold);
        //DropItem(_monster.TF.position, CurStageInfo);
    }

    public override void NotifyUserDead()
    {
        StageClear();
        if(UIBattleRoot_Raid.Get().groggybar.bossTimerCrt!=null)
        {
             UIManager.Instance.StopCoroutine(UIBattleRoot_Raid.Get().groggybar.bossTimerCrt);
        }
    }

    public override void NotifyTimeOut()
    {
        StageClear();
        if (UIBattleRoot_Raid.Get().groggybar.bossTimerCrt != null)
        {
             UIManager.Instance.StopCoroutine(UIBattleRoot_Raid.Get().groggybar.bossTimerCrt);
        }
    }

    private void DropGold(Vector3 _pos, double _gold)
    {

        for (int i = 0; i < 10; i++)
        {
            Entity e = manager.Instantiate(PrefabEntity.Inst.data["coin"].entity);
            manager.AddComponentData(e, new Translation { Value = _pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(1f, 3f), UnityEngine.Random.Range(-1f, 1f)) });
            float3 dir = new float3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            manager.AddComponentData(e, new PhysicsAddForceData { pow = UnityEngine.Random.Range(2f, 5f), dir = dir });
            manager.AddComponentData(e, new ObjectData { index = 0 });
            manager.AddComponentData(e, new NonUniformScale { Value = new float3(1, 1, 1) });
        }


        string name = UserManager.Instance.ItemInfoDatas.Where(x => x.itemType == ITEM_TYPE.GOLD).FirstOrDefault().nameTextID;
        LocalServer_Drop.BoxItemResult result = new LocalServer_Drop.BoxItemResult { itemType = ITEM_TYPE.GOLD, itemGrade = ITEM_GRADE.NORMAL, itemName = name, count = (int)_gold, icon = "gold" };
        UIAddItemMsgSystem.Get().AddItemMsg(result);
    }
}
