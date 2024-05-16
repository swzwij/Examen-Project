using UnityEngine;
using FishNet;
using FishNet.Object;

namespace Examen.Structures
{
    public class ProjectileTower : NetworkBehaviour
    {
        [SerializeField] private Transform _barrel;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private NetworkObject projectilePrefab;
        [SerializeField] private float _shootInterval;
        [SerializeField] private float _range;

        private Transform _boss;
        private float _timer = 0f;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _boss = GameObject.Find("Boss").transform; // TODO: Replace with a more reliable way to find the boss
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            LookAtBoss();

            if ((_boss.position - transform.position).sqrMagnitude > _range * _range)
                return;

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
            if (!IsServer)
                return;

            if (_boss == null)
                return;

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}