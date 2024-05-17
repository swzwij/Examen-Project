using System;
using System.Collections;
using System.Collections.Generic;
using Exame.Attacks;
using Examen.Attacks;
using Examen.Pathfinding;
using Examen.Proximity;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
using Swzwij.Extensions;
using UnityEngine;

namespace Examen.NPC
{
    [RequireComponent(typeof(WaypointFollower), typeof(ProximityAgent))]
    public class Boss : Interactable
    {
        [SerializeField] private WaypointFollower _waypointFollower;
        [SerializeField] private Animator p_animator;

        [Header("Attack Settings")]
        [SerializeField] private BaseAttack[] _attacks;
        [SerializeField] private float _attackInterval = 1f;
        [SerializeField] private float _scanInterval = 5f;
        [SerializeField] [Range(0,100)] private float _attackChance = 25f;

        [Header("Proximity Settings")]
        [SerializeField] private int _nearbyPlayerThreshold = 2;
        [SerializeField] private int _nearbyStructureThreshold = 1;
        
        private Dictionary<AttackTypes, BaseAttack> _attackTypes = new();
        private HashSet<ProximityAgent> _nearbyPlayers = new();
        private HashSet<ProximityAgent> _nearbyStructures = new();
        private HashSet<ProximityAgent> _lastNearbyStructures = new();
        private HealthData _healthData;
        private ProximityAgent _proximityAgent;
        private Coroutine _scanningCoroutine;
        private Coroutine _attackCoroutine;
        private Coroutine _lookAtTargetCoroutine;
        private bool _hasClearedPath;
        private bool _canAttack = true;

        public event Action<ProximityAgent> OnNewStructureEncountered;

        private void Awake()
        {
            _waypointFollower = GetComponent<WaypointFollower>();
            _waypointFollower.OnBossInitialised += InitBoss;
            _waypointFollower.OnPathStarted += TriggerWalking;

            _proximityAgent = GetComponent<ProximityAgent>();
        }

        private void FixedUpdate()
        {
            if (!IsServer || !_canAttack)
                return;

            ScanForNearbyAgents();
        }

        private void InitBoss()
        {
            _waypointFollower.OnPathCleared += SetHasClearedPath;
            _waypointFollower.OnPathCompleted += TriggerIdle;
            _waypointFollower.OnPathBlocked += ProcessStructureEncounter;

            OnNewStructureEncountered += TryAttackStructure;

            foreach (BaseAttack attack in _attacks)
            {
                _attackTypes.Add(attack.AttackType, attack);
                attack.OnAttackStarted += _waypointFollower.ToggleWaiting;
                attack.OnAttackFinished += SetCanAttack;
            }

            if (TryGetComponent(out _healthData))
                _healthData.onDie.AddListener(TriggerDie);

            _waypointFollower.OnBossInitialised -= InitBoss;
        }

        [Server]
        public override void Interact(NetworkConnection connection, float damageAmount = 0)
        {
            _healthData.TakeDamage(damageAmount);
        }

        public override void PlayInteractingSound()
        {
            
        }

        [ObserversRpc]
        public virtual void BroadcastInteract()
        {
            // Todo: Play given animation
        }

        private void ProcessStructureEncounter(RaycastHit healthData)
        {
            if (!healthData.collider.TryGetComponent(out HealthData _))
                return;
            
            TriggerIdle();
            _attackCoroutine = StartCoroutine(RepeatingAttack(_attackInterval, AttackTypes.AOE)); 
        }

        private void ProcessAttack(AttackTypes attackType)
        {
            if (GetAttack(attackType, out BaseAttack attack))
                attack.StartAttack();
            else
                Debug.LogError($"Attack type {attackType} not found in dictionary.");
        }

        private bool GetAttack(AttackTypes attackType, out BaseAttack attack)
        {
            if (_attackTypes.TryGetValue(attackType, out attack))
                return true;

            Debug.LogError($"Attack type {attackType} not found in dictionary.");
            return false;
        }

        protected IEnumerator RepeatingAttack(float interval, AttackTypes attackType)
        {
            BaseAttack attack = _attackTypes[attackType];
            float totalinterval = interval + attack.Cooldown + attack.PrepareTime;

            while (!_hasClearedPath) // TODO replace with a condition (such as the path no longer being blocked)
            {
                if (!attack.CanAttack)
                    yield return null;

                ProcessAttack(attackType);

                yield return new WaitForSeconds(totalinterval);                
            }
        }

        [Server]
        protected IEnumerator AttackUntil(ProximityAgent agent, AttackTypes attackType, float interval = 0f)
        {
            BaseAttack attack = _attackTypes[attackType];
            float totalinterval = interval + attack.Cooldown + attack.PrepareTime;
            HealthData healthdata = agent.gameObject.TryGetCachedComponent<HealthData>();

            _lookAtTargetCoroutine = StartCoroutine(LookAtTarget(agent.transform));
            TriggerIdle();
            while (!healthdata.isDead)
            {
                if (!attack.CanAttack)
                    yield return null;

                ProcessAttack(attackType);

                yield return new WaitForSeconds(totalinterval);
            }

            _attackCoroutine = null;
            _lookAtTargetCoroutine = null;
        }

        [Server]
        protected IEnumerator LookAtTarget(Transform target)
        {
            float angleToTarget = Vector3.Angle(transform.forward, target.position - transform.position);
            while (angleToTarget > 15f)
            {
                Vector3 direction = target.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5f);

                yield return null;
            }
        }

        [Server]
        private void SetHasClearedPath(bool hasClearedPath) => _hasClearedPath = hasClearedPath;

        [Server]
        private void UpdateProximityData()
        {
            _nearbyPlayers = _proximityAgent.RequestProximityData(AgentTypes.PLAYER);
            _nearbyStructures = _proximityAgent.RequestProximityData(AgentTypes.STRUCTURE);

            foreach (ProximityAgent structure in _nearbyStructures)
            {
                if (!_lastNearbyStructures.Contains(structure))
                    OnNewStructureEncountered?.Invoke(structure);
            }

            _lastNearbyStructures = _nearbyStructures;
        }

        [Server]
        private void ScanForNearbyAgents()
        {
            UpdateProximityData();

            if (_nearbyPlayers.Count >= _nearbyPlayerThreshold)
                ProcessAttack(AttackTypes.AOE);
            
            if (_scanningCoroutine != null)
                return;

            _scanningCoroutine = StartCoroutine(ScanInterval(_scanInterval));
        }

        [Server]
        private bool WillAttack() => UnityEngine.Random.Range(0, 100) <= _attackChance;

        [Server]
        private IEnumerator ScanInterval(float interval)
        {
            yield return new WaitForSeconds(interval);

            if (_nearbyStructures.Count <= _nearbyStructureThreshold || !WillAttack())
                yield break;
            
            ProcessAttack(AttackTypes.AOE);
        }

        [Server]
        private void TryAttackStructure(ProximityAgent structure)
        {
            if (_nearbyStructures.Count < _nearbyStructureThreshold)
                return;
            
            if (!WillAttack())
                return;

            if (_lookAtTargetCoroutine != null || _attackCoroutine != null)
                return;
            
            _attackCoroutine = StartCoroutine(AttackUntil(structure, AttackTypes.AOE, _attackInterval));
        }

        [Server]
        private void SetCanAttack(bool canAttack) => _canAttack = canAttack;

        [ObserversRpc]
        private void BroadCastAnimation(string trigger) => p_animator.SetTrigger(trigger);

        private void TriggerIdle()
        {
            p_animator.SetTrigger("Idle");
            BroadCastAnimation("Idle");
            _waypointFollower.ToggleWaiting(true);
        }

        private void TriggerWalking()
        {
            p_animator.SetFloat("WalkSpeed", _waypointFollower.Speed);
            p_animator.SetTrigger("Walk");
            BroadCastAnimation("Walk");
        }

        private void TriggerDie()
        {
            p_animator.SetTrigger("Die");
            BroadCastAnimation("Die");
            _waypointFollower.enabled = false;
            _proximityAgent.enabled = false;
        }

        private void RemoveListeners()
        {
            _waypointFollower.OnPathCleared -= SetHasClearedPath;
            _waypointFollower.OnPathStarted -= TriggerWalking;
            _waypointFollower.OnPathCompleted -= TriggerIdle;
            _waypointFollower.OnPathBlocked -= ProcessStructureEncounter;

            OnNewStructureEncountered -= TryAttackStructure;

            foreach (BaseAttack attack in _attacks)
            {
                attack.OnAttacked -= _waypointFollower.ToggleWaiting;
                attack.OnAttackFinished -= SetCanAttack;
            }

            if (_healthData)
                _healthData.onDie.RemoveListener(TriggerDie);
        }

        private void OnDestroy() => RemoveListeners();
    }
}
