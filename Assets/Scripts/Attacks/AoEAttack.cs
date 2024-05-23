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
        [SerializeField] private float _resetTime = 1f;
        [SerializeField] private float _radius = 5f;
        [SerializeField] private LayerMask _layerMask;

        [Header("Visuals")]
        [SerializeField] private Projector _projector;

        private readonly Dictionary<Collider, HealthData> _damagedTargets = new();
        private float AoERadius => _radius * 1.2f;

        protected void Start() => ServerInstance.Instance.OnServerStarted += InitAoEAttack;

        protected void InitAoEAttack()
        {
            _projector = GetComponentInChildren<Projector>();
            _projector.orthographicSize = 0;
            BroadCastProjectorSize(_projector.orthographicSize);
        }

        public override void StopAttack()
        {
            base.StopAttack();
            _projector.orthographicSize = 0;
            BroadCastProjectorSize(_projector.orthographicSize);
        }

        protected override void PrepareAttack()
        {
            base.PrepareAttack();

            if (!isActiveAndEnabled)
                return;
            
            StartCoroutine(PrepareProjector());
        }

        protected override void Attack() => ActivateAoEAttack();

        private void ActivateAoEAttack()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _layerMask);
            foreach (Collider collider in colliders)
            {
                if (_damagedTargets.TryGetValue(collider, out _))
                {
                    _damagedTargets[collider].TakeDamage(p_damage);
                    continue;
                }

                if (!collider.TryGetComponent(out HealthData healthData))
                    continue;
                
                _damagedTargets.Add(collider, healthData);
                healthData.TakeDamage(p_damage);
            }

            if (!isActiveAndEnabled)
                return;

            StartCoroutine(ResetProjector());
        }

        private IEnumerator PrepareProjector()
        {
            yield return new WaitForSeconds(p_prepareTime + CurrentAnimationTime / 2);
            float prepareTime = p_prepareTime / 2;
            while (prepareTime > 0)
            {
                prepareTime -= Time.deltaTime;
                _projector.orthographicSize = Mathf.Lerp(0, AoERadius, 1 - prepareTime / p_prepareTime); 
                BroadCastProjectorSize(_projector.orthographicSize);
                yield return null;
            }
        }

        private IEnumerator ResetProjector()
        {
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
        private void BroadCastProjectorSize(float size) => _projector.orthographicSize = size;

        private void OnDrawGizmos() 
        {
            if (IsServer)
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
