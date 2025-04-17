using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class BattleStage_Pvp
{
    void FirstSpawn()
    {
        groupDic.Clear();
        CharacterManager.Instance.ReflashMonsterList();
        SpawnMyAlly();
    }

    public void SpawnMyAlly()
    {
        if (EnemyUserData == null)
            return;
        stageArenaData areana = UIManager.Instance.stageArenaDatas[0];
        ActorEnemyUser _emyActor = CharacterManager.Instance.CreateEnemyUser(EnemyUserData, Util.GetLocalID(), 10, new Vector3(areana.setPosAi[0], areana.setPosAi[1], areana.setPosAi[2]),
            Vector3.forward);
        _EnemyUser = _emyActor;
    }
    public void SpawnUser()
    {
        stageArenaData areana= UIManager.Instance.stageArenaDatas[0];
        ActorUser _myActor = CharacterManager.Instance.CreateUser(Util.GetLocalID(), 10, new Vector3(areana.setPosPlayer[0], areana.setPosPlayer[1], areana.setPosPlayer[2]),
            Vector3.forward);
        CharacterManager.Instance.MyActor = _myActor;
        CharacterManager.Instance.MyActor.action.SetAction(eActionType.IDLE);

        FollowCam.Get().SetFollowTarget(_myActor.TF);
    }
}
