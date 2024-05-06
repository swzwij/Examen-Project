using Examen.Networking;
using Examen.Pathfinding;
using Examen.Poolsystem;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BossSpawningManager : NetworkBehaviour
{
    [Header("Boss Spawning")]
    [SerializeField] private int _bossDownTimer = 60;
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
        int randomNumber = Random.Range(0, _bossSpawnPoints.Count);
        int randomNumber2 = Random.Range(0, _bossPrefabs.Count);

        GameObject boss = PoolSystem.Instance.SpawnObject(_bossPrefabs[randomNumber2].name, _bossPrefabs[randomNumber2].gameObject);
        InstanceFinder.ServerManager.Spawn(boss);

        if (_bossSpawnPoints[randomNumber].Waypoints.Count == 0)
            _bossSpawnPoints[randomNumber].SetWaypoints();

        boss.transform.position = _bossSpawnPoints[randomNumber].Waypoints[0].position;
        boss.GetComponent<WaypointFollower>().InitBoss(_bossSpawnPoints[randomNumber].Waypoints);
        HealthData healthData = boss.GetComponent<HealthData>();

        healthData.onDie.AddListener(() => StartNextSpawnTimer(boss));

        AddSlider(healthData);
    }

    private void SendBossInfoOnConnection()
    {
        ReceiveBossInfoOnConnection(_currentActiveBossSliders);
    }

    [ObserversRpc]
    private void ReceiveBossInfoOnConnection(Dictionary<GameObject, BossHealthBar> currentActiveBossSliders)
    {
        if (_hasConnected)
            return;

        foreach (var sliders in currentActiveBossSliders)
            sliders.Value.ClientInitialize(1000);

        _hasConnected = true;
    }

    private void AddSlider(HealthData bossHealth)
    {
        BossHealthBar healthBar = bossHealth.gameObject.GetComponent<BossHealthBar>();

        healthBar.BossHealthData = bossHealth;
        healthBar.ServerInitialize();

        if(IsServer) 
            _currentActiveBossSliders.Add(bossHealth.gameObject, healthBar);
    }

    private void StartNextSpawnTimer(GameObject bossObject)
    {
        if (_currentActiveBossSliders.TryGetValue(bossObject, out BossHealthBar healthBar))
            _currentActiveBossSliders.Remove(bossObject);
       

        PoolSystem.Instance.DespawnObject(bossObject.name, bossObject);
        StartCoroutine(WaitForNextSpawn());
    }

    private IEnumerator WaitForNextSpawn()
    {
        yield return new WaitForSeconds(_bossDownTimer);

        SpawnBoss();
    }
  
}