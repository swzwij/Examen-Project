using Examen.Poolsystem;
using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawningManager : MonoBehaviour
{
    [SerializeField] private int _bossDownTimer = 60;
    [SerializeField] private HealthData _bossPrefab;

    private void CreateBoss()
    {
        HealthData healthData = Instantiate(_bossPrefab);
        healthData.onDie.AddListener(() => StartNextSpawnTimer(healthData.gameObject));
    }

    private void StartNextSpawnTimer(GameObject BossObject)
    {
        PoolSystem.Instance.StartRespawnTimer(_bossDownTimer, BossObject.name, BossObject.transform.parent);
        PoolSystem.Instance.DespawnObject(BossObject.name, BossObject);
    }
}
}
