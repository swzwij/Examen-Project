using Examen.Networking;
using Examen.Pathfinding;
using Examen.Poolsystem;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawningManager : NetworkBehaviour
{
    [Header("Boss Spawning")]
    [SerializeField] private int _bossDownTimer = 120;
    [SerializeField] private PlayerSpawner _spawner;
    [SerializeField] private List<HealthData> _bossPrefabs;
    [SerializeField] private List<BossSpawnPoints> _bossSpawnPoints;

    private bool _hasConnected;
    private Dictionary<GameObject, BossHealthBar> _currentActiveBossSliders = new();

    private void Start()
    {
        ServerInstance.Instance.OnServerStarted += SpawnBoss;

        _spawner.OnSpawned += (NetworkObject obj) => SendBossInfoOnConnection();
    }

    [Server]
    private void SpawnBoss()
    {
        int randomSpawnPointNumber = Random.Range(0, _bossSpawnPoints.Count);
        int randomBossPrefabNumber = Random.Range(0, _bossPrefabs.Count);

        GameObject boss = PoolSystem.Instance.SpawnObject(_bossPrefabs[randomBossPrefabNumber].name, _bossPrefabs[randomBossPrefabNumber].gameObject);
        InstanceFinder.ServerManager.Spawn(boss);

        if (_bossSpawnPoints[randomSpawnPointNumber].Waypoints.Count == 0)
            _bossSpawnPoints[randomSpawnPointNumber].SetWaypoints();

        boss.transform.position = _bossSpawnPoints[randomSpawnPointNumber].Spawnpoint.position;
        boss.GetComponent<WaypointFollower>().Waypoints = _bossSpawnPoints[randomSpawnPointNumber].Waypoints;

        StartCoroutine(StartNextSpawnTimer());

        AddSlider(boss);
    }

    private void SendBossInfoOnConnection() => ReceiveBossInfoOnConnection(_currentActiveBossSliders);

    [ObserversRpc]
    private void ReceiveBossInfoOnConnection(Dictionary<GameObject, BossHealthBar> currentActiveBossSliders)
    {
        if (_hasConnected)
            return;

        foreach (var sliders in currentActiveBossSliders)
            sliders.Value.ClientInitialize(1000);

        _hasConnected = true;
    }

    private void AddSlider(GameObject boss)
    {
        BossHealthBar healthBar = boss.GetComponent<BossHealthBar>();
        HealthData bossHealth = boss.GetComponent<HealthData>();

        healthBar.BossHealthData = bossHealth;
        healthBar.ServerInitialize();

        _currentActiveBossSliders.Add(bossHealth.gameObject, healthBar);
    }

    private void DespawnBosses()
    {
        foreach (KeyValuePair<GameObject, BossHealthBar> slider in _currentActiveBossSliders)
            PoolSystem.Instance.DespawnObject(slider.Key.name, slider.Key);

        _currentActiveBossSliders.Clear();
    }

    private IEnumerator StartNextSpawnTimer()
    {
        yield return new WaitForSeconds(_bossDownTimer);

        SpawnBoss();
    }
  
}