using Examen.Networking;
using Examen.Pathfinding;
using Examen.Poolsystem;
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

    [Header("Boss UI")]
    [SerializeField] private List<BossHealthBar> _sliders;

    private int _spawnedBosses;
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
        if (_spawnedBosses >= _sliders.Count)
        {
            Debug.LogError("Cant add more Boss");
            return;
        }

        int randomNumber = Random.Range(0, _bossSpawnPoints.Count);
        int randomNumber2 = Random.Range(0, _bossPrefabs.Count);

        GameObject boss = PoolSystem.Instance.SpawnObject(_bossPrefabs[randomNumber2].name, _bossPrefabs[randomNumber2].gameObject);

        if (_bossSpawnPoints[randomNumber].Waypoints.Count == 0)
            _bossSpawnPoints[randomNumber].SetWaypoints();

        boss.transform.position = _bossSpawnPoints[randomNumber].Waypoints[0].position;
        boss.GetComponent<WaypointFollower>().InitBoss(_bossSpawnPoints[randomNumber].Waypoints);
        HealthData healthData = boss.GetComponent<HealthData>();

        healthData.onDie.AddListener(() => StartNextSpawnTimer(boss));
        _spawnedBosses++;

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
        {
            sliders.Value.gameObject.SetActive(true);
            sliders.Value.ClientInitialize(1000);
        }

        _hasConnected = true;
    }

    private void AddSlider(HealthData bossHealth)
    {
        if (_currentActiveBossSliders.Count >= _sliders.Count)
        {
            Debug.LogError("Cant add more sliders");
            return;
        }

        BossHealthBar healthBar = _sliders[_currentActiveBossSliders.Count];

        healthBar.BossHealthData = bossHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.ServerInitialize();

        if(IsServer) 
            _currentActiveBossSliders.Add(bossHealth.gameObject, _sliders[_currentActiveBossSliders.Count]);
    }

    private void StartNextSpawnTimer(GameObject bossObject)
    {
        if (_currentActiveBossSliders.TryGetValue(bossObject, out BossHealthBar healthBar))
        {
            healthBar.gameObject.SetActive(true);
            _currentActiveBossSliders.Remove(bossObject);
        }

        PoolSystem.Instance.DespawnObject(bossObject.name, bossObject);
        StartCoroutine(WaitForNextSpawn());
    }

    private IEnumerator WaitForNextSpawn()
    {
        yield return new WaitForSeconds(_bossDownTimer);

        SpawnBoss();
    }
  
}