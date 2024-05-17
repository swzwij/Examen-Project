using UnityEngine;
using FishNet;
using FishNet.Object;
using Examen.Proximity;
using System.Collections.Generic;

namespace Examen.Structures
{
    [RequireComponent(typeof(ProximityAgent))]
    public class ProjectileTower : NetworkBehaviour
    {
        [SerializeField] private Transform _barrel;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private NetworkObject projectilePrefab;
        [SerializeField] private float _shootInterval;

        private ProximityAgent _proximityAgent;
        private Transform _boss;
        private float _timer = 0f;

        private void Awake() => _proximityAgent = GetComponent<ProximityAgent>();

        private void OnEnable() =>  _proximityAgent.OnProximityDataReceived += OnProximityDataReceived;

        private void OnDisable() => _proximityAgent.OnProximityDataReceived -= OnProximityDataReceived;

        private void OnProximityDataReceived(HashSet<ProximityAgent> nearbyAgents)
        {

            if (nearbyAgents.Count == 0)
                return;

            Transform closestAgent = null;
            float closestDistance = float.MaxValue;

            foreach (ProximityAgent agent in nearbyAgents)
            {
                float distance = (agent.transform.position - transform.position).sqrMagnitude;

                if (distance >= closestDistance || !agent.gameObject.activeSelf)
                    continue;
                    
                closestDistance = distance;
                closestAgent = agent.transform;
            }

            _boss = closestAgent;
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            if (_boss == null)
                return;

            LookAtBoss();

            _timer += Time.fixedDeltaTime;
            if (_timer >= _shootInterval)
            {
                _timer = 0f;
                ShootProjectile();
            }
        }

        [Server]
        private void LookAtBoss()
        {
            Vector3 direction = _boss.position - _barrel.transform.position;
            direction.y = 0;
            _barrel.transform.rotation = Quaternion.LookRotation(direction);
        }

        [Server]
        private void ShootProjectile()
        {
            FishNet.Managing.NetworkManager _networkManager = InstanceFinder.NetworkManager;

            NetworkObject projectile = _networkManager.GetPooledInstantiated
            (
                projectilePrefab,
                _firePoint.position,
                Quaternion.identity,
                true
            );

            _networkManager.ServerManager.Spawn(projectile);
            _networkManager.SceneManager.AddOwnerToDefaultScene(projectile);

            if (projectile.TryGetComponent(out Projectile projectileMotion))
                projectileMotion.Target = _boss;
        }
    }
}