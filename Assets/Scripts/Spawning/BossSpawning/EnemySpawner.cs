using Examen.Networking;
using Examen.Pathfinding;
using Examen.Poolsystem;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using MarkUlrich.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : NetworkedSingletonInstance<EnemySpawner>
{
    [Header("Boss Spawning")]
    [SerializeField] private int _bossDownTimer = 120;
    [SerializeField] private PlayerSpawner _spawner;
    [SerializeField] private List<HealthData> _bossPrefabs;
    [SerializeField] private List<BossSpawnPoints> _bossSpawnPoints;

    private bool _hasConnected;
    private Dictionary<EnemyHealthBar, float> _bossesHealth = new();

    private void Start()
    {
        ServerInstance.Instance.OnServerStarted += SpawnBoss;

        _spawner.OnSpawned += (NetworkObject obj) => SendBossInfoOnConnection();
    }

    public void DespawnBoss(EnemyHealthBar bossHealthBar)
    {
        PoolSystem.Instance.DespawnObject(bossHealthBar.gameObject.name, bossHealthBar.gameObject);
        _bossesHealth.Remove(bossHealthBar);
    }

    private void SendBossInfoOnConnection() => ReceiveBossInfoOnConnection(_bossesHealth);

    [ObserversRpc]
    private void ReceiveBossInfoOnConnection(Dictionary<EnemyHealthBar, float> currentActiveBossSliders)
    {
        if (_hasConnected)
            return;

        foreach (KeyValuePair<EnemyHealthBar, float> sliders in currentActiveBossSliders)
            sliders.Key.ClientInitialize(sliders.Value);

        _hasConnected = true;
    }

    [Server]
    private void SpawnBoss()
    {
        int randomSpawnPointNumber = Random.Range(0, _bossSpawnPoints.Count);
        int randomBossPrefabNumber = Random.Range(0, _bossPrefabs.Count);

        GameObject boss = PoolSystem.Instance.SpawnObject(_bossPrefabs[randomBossPrefabNumber].name, _bossPrefabs[randomBossPrefabNumber].gameObject);
        InstanceFinder.ServerManager.Spawn(boss);

        EnableEnemy(boss);

        if (_bossSpawnPoints[randomSpawnPointNumber].Waypoints.Count == 0)
            _bossSpawnPoints[randomSpawnPointNumber].SetWaypoints();

        boss.transform.position = _bossSpawnPoints[randomSpawnPointNumber].Spawnpoint.position;
        StartCoroutine(DelayedPathStart(boss, randomSpawnPointNumber));
        
        StartCoroutine(StartNextSpawnTimer());

        AddSlider(boss);
    }

    [ObserversRpc]
    private void EnableEnemy(GameObject boss) => boss.SetActive(true);


    private IEnumerator DelayedPathStart(GameObject boss, int randomSpawnPointNumber)
    {
        yield return new WaitForSeconds(0.33f);

        boss.TryGetComponent(out WaypointFollower waypointfollower);
        waypointfollower.Waypoints = _bossSpawnPoints[randomSpawnPointNumber].Waypoints;

        if (boss.TryGetComponent(out Pathfinder pathfinder))
        {
            waypointfollower.MyPathFinder = pathfinder;
            waypointfollower.ResetWaypointIndex();
        }
    }

    private void AddSlider(GameObject boss)
    {
        EnemyHealthBar healthBar = boss.GetComponent<EnemyHealthBar>();
        HealthData bossHealth = boss.GetComponent<HealthData>();

        if (bossHealth.isDead)
            bossHealth.Resurrect(bossHealth.MaxHealth);

        healthBar.BossHealthData = bossHealth;
        healthBar.ServerInitialize();
        ReceiveBossInfoOnSpawn(healthBar, bossHealth.MaxHealth);

        _bossesHealth.Add(healthBar, bossHealth.Health);
    }

    [ObserversRpc]
    private void ReceiveBossInfoOnSpawn(EnemyHealthBar healthbar, float healthAmount) => healthbar.ClientInitialize(healthAmount);


    private void DespawnBosses()
    {
        foreach (KeyValuePair<EnemyHealthBar, float> slider in _bossesHealth)
            PoolSystem.Instance.DespawnObject(slider.Key.name, slider.Key.gameObject);

        _bossesHealth.Clear();
    }

    private IEnumerator StartNextSpawnTimer()
    {
        yield return new WaitForSeconds(_bossDownTimer);

        SpawnBoss();
    }
  
}