using Examen.Networking;
using Examen.Pathfinding;
using Examen.Poolsystem;
using Examen.UI;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Object;
using MarkUlrich.Health;
using MarkUlrich.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.BossSpawning
{
    public class EnemySpawner : NetworkedSingletonInstance<EnemySpawner>
    {
        [Header("Boss Spawning")]
        [SerializeField] private int _enemyCooldown = 60;
        [SerializeField] private PlayerSpawner _spawner;
        [SerializeField] private List<HealthData> _enemyPrefabs;
        [SerializeField] private List<EnemySpawnPoints> _enemySpawnPoints;

        private bool _hasConnected;
        private Dictionary<EnemyHealthBar, float> _enemiesHealth = new();

        private const float DELAY_PATH_TIMER = 0.33f;

        private void Start()
        {
            ServerInstance.Instance.OnServerStarted += SpawnEnemy;
            _spawner.OnSpawned += SendEnemyInfoOnConnection;
        }

        /// <summary>
        /// Despawns the enemy with the help of the poolsystem.
        /// </summary>
        /// <param name="enemyHealthBar">The healthbar of the boss that you want to despawn.</param>
        public void DespawnEnemy(EnemyHealthBar enemyHealthBar)
        {
            PoolSystem.Instance.DespawnObject(enemyHealthBar.gameObject.name, enemyHealthBar.gameObject);
            _enemiesHealth.Remove(enemyHealthBar);
        }

        private void SendEnemyInfoOnConnection(NetworkObject networkObject) => ReceiveEnemyInfoOnConnection(_enemiesHealth);

        [ObserversRpc]
        private void ReceiveEnemyInfoOnConnection(Dictionary<EnemyHealthBar, float> currentActiveEnemySliders)
        {
            if (_hasConnected)
                return;

            foreach (KeyValuePair<EnemyHealthBar, float> sliders in currentActiveEnemySliders)
                sliders.Key.ClientInitialize(sliders.Value);

            _hasConnected = true;
        }

        [Server]
        private void SpawnEnemy()
        {
            int randomSpawnPointNumber = Random.Range(0, _enemySpawnPoints.Count);
            int randomEnemyPrefabNumber = Random.Range(0, _enemyPrefabs.Count);

            GameObject enemy = PoolSystem.Instance.SpawnObject(_enemyPrefabs[randomEnemyPrefabNumber].name, 
                _enemyPrefabs[randomEnemyPrefabNumber].gameObject);

            InstanceFinder.ServerManager.Spawn(enemy);

            if (_enemySpawnPoints[randomSpawnPointNumber].Waypoints.Count == 0)
                _enemySpawnPoints[randomSpawnPointNumber].SetWaypoints();

            enemy.transform.position = _enemySpawnPoints[randomSpawnPointNumber].Spawnpoint.position;
            StartCoroutine(DelayedPathStart(enemy, randomSpawnPointNumber));

            StartCoroutine(StartNextSpawnTimer());

            AddSlider(enemy);

            EnableEnemy(enemy);
        }

        [ObserversRpc]
        private void EnableEnemy(GameObject enemy) => enemy.SetActive(true);


        private IEnumerator DelayedPathStart(GameObject enemy, int randomSpawnPointNumber)
        {
            yield return new WaitForSeconds(DELAY_PATH_TIMER);

            enemy.TryGetComponent(out WaypointFollower waypointfollower);
            waypointfollower.Waypoints = _enemySpawnPoints[randomSpawnPointNumber].Waypoints;

            if (enemy.TryGetComponent(out Pathfinder pathfinder))
            {
                waypointfollower.PathFinder = pathfinder;
                waypointfollower.ResetWaypointIndex();
            }
        }

        private void AddSlider(GameObject enemy)
        {
            EnemyHealthBar healthBar = enemy.GetComponent<EnemyHealthBar>();
            HealthData enemyHealth = enemy.GetComponent<HealthData>();

            float maxEnemyHealth = enemyHealth.MaxHealth == 0 ? enemyHealth.Health : enemyHealth.MaxHealth;

            if (enemyHealth.isDead)
                enemyHealth.Resurrect(maxEnemyHealth);

            healthBar.EnemyHealthData = enemyHealth;
            healthBar.ServerInitialize();
            ReceiveEnemyInfoOnSpawn(healthBar, maxEnemyHealth);

            _enemiesHealth.Add(healthBar, maxEnemyHealth);
        }

        [ObserversRpc]
        private void ReceiveEnemyInfoOnSpawn(EnemyHealthBar healthbar, float healthAmount) 
            => healthbar.ClientInitialize(healthAmount);


        private void DespawnEnemy()
        {
            foreach (EnemyHealthBar slider in _enemiesHealth.Keys)
                PoolSystem.Instance.DespawnObject(slider.name, slider.gameObject);

            _enemiesHealth.Clear();
        }

        private IEnumerator StartNextSpawnTimer()
        {
            yield return new WaitForSeconds(_enemyCooldown);

            SpawnEnemy();
        }

        private void OnDestroy()
        {
            ServerInstance.Instance.OnServerStarted -= SpawnEnemy;
            _spawner.OnSpawned -= SendEnemyInfoOnConnection;
        }
    }
}