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
        [SerializeField] private int _enemyDownTimer = 60;
        [SerializeField] private PlayerSpawner _spawner;
        [SerializeField] private List<HealthData> _enemyPrefabs;
        [SerializeField] private List<EnemySpawnPoints> _enemySpawnPoints;

        private bool _hasConnected;
        private Dictionary<EnemyHealthBar, float> _enemiesHealth = new();

        private void Start()
        {
            ServerInstance.Instance.OnServerStarted += SpawnEnemy;
            _spawner.OnSpawned += SendEnemyInfoOnConnection;
        }

        public void DespawnEnemy(EnemyHealthBar enemyHealthBar)
        {
            PoolSystem.Instance.DespawnObject(enemyHealthBar.gameObject.name, enemyHealthBar.gameObject);
            _enemiesHealth.Remove(enemyHealthBar);
        }

        private void SendEnemyInfoOnConnection(NetworkObject obj) => ReceiveEnemyInfoOnConnection(_enemiesHealth);

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

            GameObject enemy = PoolSystem.Instance.SpawnObject(_enemyPrefabs[randomEnemyPrefabNumber].name, _enemyPrefabs[randomEnemyPrefabNumber].gameObject);
            InstanceFinder.ServerManager.Spawn(enemy);

            EnableEnemy(enemy);

            if (_enemySpawnPoints[randomSpawnPointNumber].Waypoints.Count == 0)
                _enemySpawnPoints[randomSpawnPointNumber].SetWaypoints();

            enemy.transform.position = _enemySpawnPoints[randomSpawnPointNumber].Spawnpoint.position;
            StartCoroutine(DelayedPathStart(enemy, randomSpawnPointNumber));

            StartCoroutine(StartNextSpawnTimer());

            AddSlider(enemy);
        }

        [ObserversRpc]
        private void EnableEnemy(GameObject enemy) => enemy.SetActive(true);


        private IEnumerator DelayedPathStart(GameObject enemy, int randomSpawnPointNumber)
        {
            yield return new WaitForSeconds(0.33f);

            enemy.TryGetComponent(out WaypointFollower waypointfollower);
            waypointfollower.Waypoints = _enemySpawnPoints[randomSpawnPointNumber].Waypoints;

            if (enemy.TryGetComponent(out Pathfinder pathfinder))
            {
                waypointfollower.MyPathFinder = pathfinder;
                waypointfollower.ResetWaypointIndex();
            }
        }

        private void AddSlider(GameObject enemy)
        {
            EnemyHealthBar healthBar = enemy.GetComponent<EnemyHealthBar>();
            HealthData enemyHealth = enemy.GetComponent<HealthData>();

            if (enemyHealth.isDead)
                enemyHealth.Resurrect(enemyHealth.MaxHealth);

            healthBar.EnemyHealthData = enemyHealth;
            healthBar.ServerInitialize();
            ReceiveEnemyInfoOnSpawn(healthBar, enemyHealth.MaxHealth);

            _enemiesHealth.Add(healthBar, enemyHealth.Health);
        }

        [ObserversRpc]
        private void ReceiveEnemyInfoOnSpawn(EnemyHealthBar healthbar, float healthAmount) => healthbar.ClientInitialize(healthAmount);


        private void DespawnEnemy()
        {
            foreach (KeyValuePair<EnemyHealthBar, float> slider in _enemiesHealth)
                PoolSystem.Instance.DespawnObject(slider.Key.name, slider.Key.gameObject);

            _enemiesHealth.Clear();
        }

        private IEnumerator StartNextSpawnTimer()
        {
            yield return new WaitForSeconds(_enemyDownTimer);

            SpawnEnemy();
        }

        private void OnDestroy()
        {
            ServerInstance.Instance.OnServerStarted -= SpawnEnemy;
            _spawner.OnSpawned -= SendEnemyInfoOnConnection;
        }
    }
}