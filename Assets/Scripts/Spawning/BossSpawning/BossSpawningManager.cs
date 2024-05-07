using Examen.Networking;
using Examen.Poolsystem;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawningManager : NetworkBehaviour
{
    [SerializeField] private int _bossDownTimer = 60;
    [SerializeField] private HealthData _bossPrefab;
    [SerializeField] private PlayerSpawner _spawner;

    private bool _hasConnected;
    private readonly Dictionary<GameObject, BossHealthBar> _currentActiveBossSliders = new();

    private void Start()
    {
        ServerInstance.Instance.OnServerStarted += CreateBoss;

        _spawner.OnSpawned += (NetworkObject obj) => SendBossInfoOnConnection();
    }

    private void CreateBoss()
    { 
        HealthData healthData = Instantiate(_bossPrefab);
        InstanceFinder.ServerManager.Spawn(healthData.gameObject);

        PoolSystem.Instance.AddActiveObject(healthData.name, healthData.gameObject);
        healthData.onDie.AddListener(() => StartNextSpawnTimer(healthData.gameObject));

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
       

        PoolSystem.Instance.StartRespawnTimer(_bossDownTimer, bossObject.name, bossObject.transform.parent); // todo: remove this line when we have more bosses
        PoolSystem.Instance.DespawnObject(bossObject.name, bossObject);
    }
}