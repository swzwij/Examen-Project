using Examen.Networking;
using Examen.Poolsystem;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.BossSpawning
{
    public class EnemySpawner : NetworkBehaviour
    {
        [SerializeField] private int _enemyDownTimer = 60;
        [SerializeField] private HealthData _enemyPrefab;
        [SerializeField] private PlayerSpawner _spawner;

        private bool _hasConnected;
        private readonly Dictionary<GameObject, EnemyHealthBar> _currentActiveEnemySliders = new();

        private void Start()
        {
            ServerInstance.Instance.OnServerStarted += CreateEnemy;
            _spawner.OnSpawned += SendEnemyInfoOnConnection;
        }

        private void CreateEnemy()
        {
            HealthData healthData = Instantiate(_enemyPrefab);
            InstanceFinder.ServerManager.Spawn(healthData.gameObject);

            PoolSystem.Instance.AddActiveObject(healthData.name, healthData.gameObject);
            healthData.onDie.AddListener(() => StartNextSpawnTimer(healthData.gameObject));

            AddSlider(healthData);
        }

        private void SendEnemyInfoOnConnection(NetworkObject obj) => ReceiveEnemyInfoOnConnection(_currentActiveEnemySliders);

        [ObserversRpc]
        private void ReceiveEnemyInfoOnConnection(Dictionary<GameObject, EnemyHealthBar> currentActiveBossSliders)
        {
            if (_hasConnected)
                return;

            foreach (KeyValuePair<GameObject, EnemyHealthBar> sliders in currentActiveBossSliders)
                sliders.Value.ClientInitialize(1000);

            _hasConnected = true;
        }

        private void AddSlider(HealthData enemyHealth)
        {
            EnemyHealthBar healthBar = enemyHealth.gameObject.GetComponent<EnemyHealthBar>();

            healthBar.EnemyHealthData = enemyHealth;
            healthBar.ServerInitialize();

            if (IsServer)
                _currentActiveEnemySliders.Add(enemyHealth.gameObject, healthBar);
        }

        private void StartNextSpawnTimer(GameObject enemyObject)
        {
            if (_currentActiveEnemySliders.TryGetValue(enemyObject, out EnemyHealthBar healthBar))
                _currentActiveEnemySliders.Remove(enemyObject);

            PoolSystem.Instance.StartRespawnTimer(_enemyDownTimer, enemyObject.name, enemyObject.transform.parent); // todo: remove this line when we have more bosses
            PoolSystem.Instance.DespawnObject(enemyObject.name, enemyObject);
        }

        private void OnDestroy()
        {
            ServerInstance.Instance.OnServerStarted -= CreateEnemy;
            _spawner.OnSpawned -= SendEnemyInfoOnConnection;
        }
    }
}