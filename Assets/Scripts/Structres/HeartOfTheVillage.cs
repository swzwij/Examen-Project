using Examen.Networking;
using Examen.Spawning.BossSpawning;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

[RequireComponent(typeof(HealthData))]
public class HeartOfTheVillage : NetworkBehaviour
{
    private HealthData _healthData;
    private void Start() => ServerInstance.Instance.OnServerStarted += InitialiseHeart;

    private void InitialiseHeart()
    {
        _healthData = GetComponent<HealthData>();
        _healthData.onDie.AddListener(ResetLevel);
    }

    private void ResetLevel()
    {
        EnemySpawner.Instance.DespawnEnemies();
        //TODO: reset streak to 0
    }

}
