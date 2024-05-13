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
        [SerializeField] private List<BossSpawnPoints> _enemySpawnPoints;

        private bool _hasConnected;
        private Dictionary<EnemyHealthBar, float> _enemiesHealth = new();

        private void Start()
        {
            ServerInstance.Instance.OnServerStarted += SpawnEnemy;
            _spawner.OnSpawned += SendEnemyInfoOnConnection;
        }

        public void DespawnEnemy(EnemyHealthBar bossHealthBar)
        {
            PoolSystem.Instance.DespawnObject(bossHealthBar.gameObject.name, bossHealthBar.gameObject);
            _enemiesHealth.Remove(bossHealthBar);
        }

        private void SendEnemyInfoOnConnection(NetworkObject obj) => ReceiveEnemyInfoOnConnection(_enemiesHealth);

        [ObserversRpc]
        private void ReceiveEnemyInfoOnConnection(Dictionary<EnemyHealthBar, float> currentActiveBossSliders)
        {
            if (_hasConnected)
                return;

            foreach (KeyValuePair<EnemyHealthBar, float> sliders in currentActiveBossSliders)
                sliders.Key.ClientInitialize(sliders.Value);

            _hasConnected = true;
        }

        [Server]
        private void SpawnEnemy()
        {
            int randomSpawnPointNumber = Random.Range(0, _enemySpawnPoints.Count);
            int randomBossPrefabNumber = Random.Range(0, _enemyPrefabs.Count);

            GameObject boss = PoolSystem.Instance.SpawnObject(_enemyPrefabs[randomBossPrefabNumber].name, _enemyPrefabs[randomBossPrefabNumber].gameObject);
            InstanceFinder.ServerManager.Spawn(boss);

            EnableEnemy(boss);

            if (_enemySpawnPoints[randomSpawnPointNumber].Waypoints.Count == 0)
                _enemySpawnPoints[randomSpawnPointNumber].SetWaypoints();

            boss.transform.position = _enemySpawnPoints[randomSpawnPointNumber].Spawnpoint.position;
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
            waypointfollower.Waypoints = _enemySpawnPoints[randomSpawnPointNumber].Waypoints;

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

            healthBar.EnemyHealthData = bossHealth;
            healthBar.ServerInitialize();
            ReceiveEnemyInfoOnSpawn(healthBar, bossHealth.MaxHealth);

            _enemiesHealth.Add(healthBar, bossHealth.Health);
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