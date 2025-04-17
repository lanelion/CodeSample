using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class BattleStage_Raid
{//float defaultRadius = 15f;

    Dictionary<int, GroupClass> groupDic = new Dictionary<int, GroupClass>();
    [SerializeField] List<SpawnPos> spawnPos;
    [SerializeField] Vector3[] randomPos;

    [SerializeField] float spawnDistance = 16f;
    [SerializeField] Vector2 plusSpawnPos = Vector2.zero;


    private Vector3 MapCenter = Vector3.zero;

    void SettingSpawnPos()
    {
        float radius = 3;
        float node = radius * 2;
        spawnPos = new List<SpawnPos>();
        Vector2 worldSize = new Vector2(CurStageInfoList[CurStep].stageSize[0], CurStageInfoList[CurStep].stageSize[0]);

        int gridx = Mathf.RoundToInt(worldSize.x / node);
        int gridy = Mathf.RoundToInt(worldSize.y / node);

        Vector3 bottom = MapCenter - Vector3.right * worldSize.x / 2 - Vector3.forward * worldSize.y / 2;
        int i = 0;
        for (int x = 0; x < gridx; x++)
        {
            for (int y = 0; y < gridy; y++)
            {
                Vector3 wp = bottom + Vector3.right * (x * node + radius) + Vector3.forward * (y * node + radius);
                spawnPos.Add(new SpawnPos { id = i, pos = new Vector2(wp.x, wp.z) });
                i++;
            }
        }
    }

    void RandomPosSet()
    {
        randomPos = new Vector3[9];
        int i = 0;
        for (int x = 0; x <= 2; x++)
        {
            for (int y = 0; y <= 2; y++)
            {
                randomPos[i] = new Vector3(x - 1, 0, y - 1);
                i++;
            }
        }
    }

    SpawnPos GetSpawnPos()
    {
        ActorUser _myActor = CharacterManager.Instance.MyActor;
        if (_myActor == null)
        {
            return null;
        }

        List<SpawnPos> worldPos = spawnPos.Where(n => ((
            n.pos - new Vector2(_myActor.TF.position.x, _myActor.TF.position.z) + plusSpawnPos).sqrMagnitude > spawnDistance * spawnDistance
            && !groupDic.ContainsKey(n.id)
        )).ToList();
        SpawnPos sPos = worldPos.OrderBy(n => UnityEngine.Random.value).FirstOrDefault();
        return sPos;
    }
}
