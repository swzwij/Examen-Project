using Examen.Networking;
using Examen.Poolsystem;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawningManager : NetworkBehaviour
{
    [Header("Boss Spawning")]
    [SerializeField] private int _bossDownTimer = 60;
    [SerializeField] private HealthData _bossPrefab;
    [SerializeField] private PlayerSpawner _spawner;

    [Header("Boss UI")]
    [SerializeField] private List<BossHealthBar> _sliders;

    private int _spawnedBosses;
    private bool _hasConnected;
    private Dictionary<GameObject, BossHealthBar> _currentActiveBossSliders = new();

    private void Start()
    {
        ServerInstance.Instance.OnServerStarted += CreateBoss;

        _spawner.OnSpawned += (NetworkObject obj) => SendBossInfoOnConnection();
    }

    [Server]
    private void CreateBoss()
    {
        if(_spawnedBosses >= _sliders.Count)
        {
            Debug.LogError("Cant add more Boss");
            return;
        }

        HealthData healthData = Instantiate(_bossPrefab);

        PoolSystem.Instance.AddActiveObject(healthData.name, healthData.gameObject);
        healthData.onDie.AddListener(() => StartNextSpawnTimer(healthData.gameObject));
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

        PoolSystem.Instance.StartRespawnTimer(_bossDownTimer, bossObject.name, bossObject.transform.parent); // todo: remove this line when we have more bosses
        PoolSystem.Instance.DespawnObject(bossObject.name, bossObject);
    }
}