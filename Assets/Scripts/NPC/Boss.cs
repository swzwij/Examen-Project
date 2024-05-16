using System.Collections;
using System.Collections.Generic;
using Exame.Attacks;
using Examen.Attacks;
using Examen.Pathfinding;
using Examen.Proximity;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
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
        [SerializeField] private float _repeatInterval = 1f;

        [Header("Proximity Settings")]
        [SerializeField] private int _nearbyPlayerThreshold = 2;
        [SerializeField] private int _nearbyStructureThreshold = 1;
        
        private Dictionary<AttackTypes, BaseAttack> _attackTypes = new();
        private HashSet<ProximityAgent> _nearbyPlayers = new();
        private HashSet<ProximityAgent> _nearbyStructures = new();
        private HealthData _healthData;
        private ProximityAgent _proximityAgent;
        private bool _hasClearedPath;
        private bool _canAttack = true;

        private void Awake()
        {
            _waypointFollower = GetComponent<WaypointFollower>();
            _waypointFollower.OnBossInitialised += InitBoss;
            _waypointFollower.OnPathStarted += StartWalking;

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
            _waypointFollower.OnStructureEncountered += ProcessStructureEncounter;
            _waypointFollower.OnPathCleared += SetHasClearedPath;
            _waypointFollower.OnPathCompleted += Idle;
            _waypointFollower.OnPathBlocked += Idle;

            foreach (BaseAttack attack in _attacks)
            {
                _attackTypes.Add(attack.AttackType, attack);
                attack.OnAttacked += _waypointFollower.ToggleWaiting;
                attack.OnAttackFinished += SetCanAttack;
            }

            if (TryGetComponent(out _healthData))
                _healthData.onDie.AddListener(Die);

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

        private void ProcessStructureEncounter(HealthData healthData)
        {
            StartCoroutine(RepeatingAttack(_repeatInterval, AttackTypes.AOE)); // TODO: Add functionality for determining if should use AOE or SPECIAL attack.
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
        private void SetHasClearedPath(bool hasClearedPath) => _hasClearedPath = hasClearedPath;

        private void UpdateProximityData()
        {
            _nearbyPlayers = _proximityAgent.RequestProximityData(AgentTypes.PLAYER);
            _nearbyStructures = _proximityAgent.RequestProximityData(AgentTypes.STRUCTURE);
        }

        [Server]
        private void ScanForNearbyAgents()
        {
            _canAttack = false;
            UpdateProximityData();

            if (_nearbyPlayers.Count < _nearbyPlayerThreshold && _nearbyStructures.Count < _nearbyStructureThreshold)
                return;
            
            ProcessAttack(AttackTypes.AOE);
        }

        [Server]
        private void SetCanAttack(bool canAttack) => _canAttack = canAttack;

        [ObserversRpc]
        private void BroadCastAnimation(string trigger) => p_animator.SetTrigger(trigger);

        private void Idle()
        {
            p_animator.SetTrigger("Idle");
            BroadCastAnimation("Idle");
        }

        private void StartWalking()
        {
            p_animator.SetFloat("WalkSpeed", _waypointFollower.Speed);
            p_animator.SetTrigger("Walk");
            BroadCastAnimation("Walk");
        }

        private void Die()
        {
            p_animator.SetTrigger("Die");
            BroadCastAnimation("Die");
            _waypointFollower.enabled = false;
            _proximityAgent.enabled = false;
        }

        private void RemoveListeners()
        {
            _waypointFollower.OnStructureEncountered -= ProcessStructureEncounter;
            _waypointFollower.OnPathCleared -= SetHasClearedPath;
            _waypointFollower.OnPathStarted -= StartWalking;
            _waypointFollower.OnPathCompleted -= Idle;
            _waypointFollower.OnPathBlocked -= Idle;

            foreach (BaseAttack attack in _attacks)
            {
                attack.OnAttacked -= _waypointFollower.ToggleWaiting;
                attack.OnAttackFinished -= SetCanAttack;
            }

            if (_healthData)
                _healthData.onDie.RemoveListener(Die);
        }

        private void OnDestroy() => RemoveListeners();
    }
}
