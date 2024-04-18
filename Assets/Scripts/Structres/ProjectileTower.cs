using FishNet;
using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace TrustFall.Structures
{
    public class ProjectileTower : NetworkBehaviour
    {
        [SerializeField] private Transform _barrel;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private NetworkObject projectilePrefab;
        [SerializeField] private float shootForce;

        private Transform _boss;

        private void Awake()
        {
            _boss = GameObject.Find("Boss").transform;

        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            LookAtBoss();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(Shoot());

        }

        private void LookAtBoss()
        {
            if (_boss == null)
            {
                return;
            }

            Vector3 direction = _boss.position - _barrel.transform.position;
            direction.y = 0;
            _barrel.transform.rotation = Quaternion.LookRotation(direction);
        }

        private IEnumerator Shoot()
        {
            yield return new WaitForSeconds(1);

            Debug.Log("Shoot");

            ShootProjectile();
            StartCoroutine(Shoot());
        }

        void ShootProjectile()
        {
            Debug.Log("Shoot proj");

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
            {
                projectileMotion.Target = _boss;
            }
        }
    }
}