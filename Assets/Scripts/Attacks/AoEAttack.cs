using System.Collections;
using System.Collections.Generic;
using Examen.Networking;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

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

        [SerializeField] private Projector _projector;
        private readonly Dictionary<Collider, HealthData> _damagedTargets = new();

        private float AoERadius => _radius * 1.2f;

        protected void Start() => ServerInstance.Instance.OnServerStarted += InitAoEAttack;

        protected void InitAoEAttack()
        {
            _projector = GetComponentInChildren<Projector>();
            _projector.orthographicSize = 0;
            BroadCastProjectorSize(0);
        }

        protected override void Attack()
        {
            Debug.LogError("Preparing AoE Attack");
            StartCoroutine(PrepareAoEAttack());
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
            StartCoroutine(PrepareProjector());
            yield return new WaitForSeconds(p_prepareTime);
            ActivateAoEAttack();
        }

        private IEnumerator PrepareProjector()
        {
            float prepareTime = p_prepareTime / 2;
            while (prepareTime > 0)
            {
                prepareTime -= Time.deltaTime;
                _projector.orthographicSize = Mathf.Lerp(0, AoERadius, 1 - prepareTime / (p_prepareTime / 2));
                BroadCastProjectorSize(_projector.orthographicSize);
                yield return null;
            }
        }

        private IEnumerator ResetProjector()
        {
            yield return new WaitForSeconds(p_attackActiveDuration);

            float resetTime = _resetTime;
            while (resetTime > 0)
            {
                resetTime -= Time.deltaTime;
                _projector.orthographicSize = Mathf.Lerp(0, AoERadius, resetTime / _resetTime);
                BroadCastProjectorSize(_projector.orthographicSize);
                yield return null;
            }
        }

        [ObserversRpc]
        private void BroadCastProjectorSize(float size)
        {
            _projector.orthographicSize = size;
        }

#region Testing
        [Header("Testing")]
        [SerializeField] private float interval = 1f;

        private Coroutine _attackIntervalCoroutine;

        private void FixedUpdate() 
        {
            if (!IsServer)
                return;

            _attackIntervalCoroutine ??= StartCoroutine(AttackInterval());
        }

        protected IEnumerator AttackInterval()
        {
            float totalinterval = interval + p_attackActiveDuration + p_cooldown + _resetTime;

            while (true)
            {
                if (!CanAttack)
                    yield return null;

                yield return new WaitForSeconds(totalinterval);
                ActivateAttack();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_attackIntervalCoroutine != null)
            {
                StopCoroutine(_attackIntervalCoroutine);
                _attackIntervalCoroutine = null;
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
