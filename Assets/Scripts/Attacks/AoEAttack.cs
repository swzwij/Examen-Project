using System.Collections;
using System.Collections.Generic;
using Examen.Networking;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;
using UnityEngine.VFX;

namespace Exame.Attacks
{
    public class AoEAttack : BaseAttack
    {
        [Header("AoE Attack Settings")]
        [SerializeField] private float p_prepareTime = 1f;
        [SerializeField] private float p_attackActiveDuration = 1f;
        [SerializeField] private float _resetTime = 1f;
        [SerializeField] private float _radius = 5f;
        [SerializeField] private LayerMask _layerMask;

        [Header("Visuals")]
        [SerializeField] private Projector _projector;

        private Coroutine _attackCoroutine;
        
        private readonly Dictionary<Collider, HealthData> _damagedTargets = new();
        private float AoERadius => _radius * 1.2f;

        protected void Start() => ServerInstance.Instance.OnServerStarted += InitAoEAttack;

        protected void InitAoEAttack()
        {
            _projector = GetComponentInChildren<Projector>();
            _projector.orthographicSize = 0;
            ProcessProjectSize(_projector.orthographicSize);
        }

        protected override void Attack()
        {
            //StartCoroutine(PrepareAoEAttack());
            if (_attackCoroutine != null)
                return;
            
            _attackCoroutine = StartCoroutine(Attacking());
        }

        private void ActivateAoEAttack()
        {
            Debug.LogError("Activating AoE Attack");
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _layerMask);
            foreach (Collider collider in colliders)
            {
                if (_damagedTargets.ContainsKey(collider))
                {
                    _damagedTargets[collider].TakeDamage(p_damage);
                    Debug.LogError($"Damaging {collider.name} again");
                    continue;
                }

                if (!collider.TryGetComponent(out HealthData healthData))
                    continue;
                
                _damagedTargets.Add(collider, healthData);
                healthData.TakeDamage(p_damage);
                Debug.LogError($"Damaging {collider.name}");
            }

            StartCoroutine(ResetProjector());
        }

        private IEnumerator PrepareAoEAttack()
        {
            Debug.LogError("Preparing AoE Attack");
            StartCoroutine(PrepareProjector());
            yield return new WaitForSeconds(p_prepareTime + p_attackActiveDuration);
            ActivateAoEAttack();
        }

        private IEnumerator PrepareProjector()
        {
            float prepareTime = p_prepareTime / 2;
            while (prepareTime > 0)
            {
                prepareTime -= Time.deltaTime;
                _projector.orthographicSize = Mathf.Lerp(0, AoERadius, 1 - prepareTime / p_prepareTime); 
                // -> p_prepareTime Was (p_prepareTime / 2)
                BroadCastProjectorSize(_projector.orthographicSize);
                yield return null;
            }
        }

        private IEnumerator ResetProjector()
        {
            //yield return new WaitForSeconds(p_attackActiveDuration);

            float resetTime = _resetTime;
            while (resetTime > 0)
            {
                resetTime -= Time.deltaTime;
                _projector.orthographicSize = Mathf.Lerp(0, AoERadius, resetTime / _resetTime);
                BroadCastProjectorSize(_projector.orthographicSize);
                yield return null;
            }
        }

        [Server]
        private void ProcessProjectSize(float size) => BroadCastProjectorSize(size);

        [ObserversRpc]
        private void BroadCastProjectorSize(float size) => _projector.orthographicSize = size;

#region Testing
        [Header("Testing")]
        [SerializeField] private float interval = 1f;

        protected IEnumerator Attacking()
        {
            float totalinterval = interval + p_attackActiveDuration + p_cooldown + _resetTime;

            while (true) // TODO replace with a condition (such as the path no longer being blocked)
            {
                if (!CanAttack)
                    yield return null;

                yield return new WaitForSeconds(totalinterval);
                StartCoroutine(PrepareAoEAttack());
            }
        }

#endregion

        private void OnDrawGizmos() 
        {
            if (IsServer)
                return;
            
            _projector.orthographicSize = AoERadius;
        }
    }
}
