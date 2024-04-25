using System.Collections;
using System.Collections.Generic;
using Exame.Attacks;
using Examen.Attacks;
using Examen.Pathfinding;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

namespace Examen.NPC
{
    [RequireComponent(typeof(WaypointFollower))]
    public class Boss : Interactable
    {
        [SerializeField] private WaypointFollower _waypointFollower;
        [SerializeField] private float _turnSpeed = 1f;

        [Header("Attack Settings")]
        [SerializeField] private BaseAttack[] _attacks;
        [SerializeField] private float _repeatInterval = 1f;
        
        private Dictionary<AttackTypes, BaseAttack> _attackTypes = new();
        private HealthData _healthData;
        private bool _hasClearedPath;

        private void Awake()
        {
            _waypointFollower = GetComponent<WaypointFollower>();
            _waypointFollower.OnBossInitialised += InitBoss;
        }

        [Server]
        private void InitBoss()
        {
            _waypointFollower.OnStructureEncountered += ProcessStructureEncounter;
            _waypointFollower.OnPathCleared += SetHasClearedPath;

            foreach (BaseAttack attack in _attacks)
                _attackTypes.Add(attack.AttackType, attack);
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
            StartCoroutine(RepeatingAttack(_repeatInterval, AttackTypes.AOE, !_hasClearedPath)); // TODO: Add functionality for determining if should use AOE or SPECIAL attack.
        }

        private void ProcessAttack(AttackTypes attackType)
        {
            if (GetAttack(attackType, out BaseAttack attack))
                attack.ActivateAttack();
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

        protected IEnumerator RepeatingAttack(float interval, AttackTypes attackType, bool routineCondition)
        {
            BaseAttack attack = _attackTypes[attackType];
            float totalinterval = interval + attack.Cooldown;

            while (routineCondition) // TODO replace with a condition (such as the path no longer being blocked)
            {
                if (!attack.CanAttack)
                    yield return null;

                yield return new WaitForSeconds(totalinterval);
                ProcessAttack(attackType);
            }
        }

        [Server]
        private void SetHasClearedPath(bool hasClearedPath) => _hasClearedPath = hasClearedPath;
    }
}
