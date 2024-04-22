using System.Collections;
using System.Collections.Generic;
using Exame.Attacks;
using Examen.Attacks;
using Examen.Pathfinding;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

namespace Examen.NPC
{
    [RequireComponent(typeof(WaypointFollower))]
    public class Boss : NetworkBehaviour
    {
        [SerializeField] private WaypointFollower _waypointFollower;
        [SerializeField] private BaseAttack[] _attacks;
        [SerializeField] private float _turnSpeed = 1f;

        private Dictionary<AttackTypes, BaseAttack> _attackTypes = new();

        private void Awake()
        {
            _waypointFollower = GetComponent<WaypointFollower>();
            _waypointFollower.OnBossInitialised += InitBoss;
        }

        private void InitBoss()
        {
            _waypointFollower.OnStructureEncountered += ProcessStructureEncounter;

            foreach (BaseAttack attack in _attacks)
                _attackTypes.Add(attack.AttackType, attack);
        }

        private void ProcessStructureEncounter(HealthData healthData)
        {
            StartCoroutine(LookAtTarget(healthData.transform));

            ProcessAttack(AttackTypes.AOE); // TODO: Add functionality for determining if should use AOE or SPECIAL attack.
        }

        private void ProcessAttack(AttackTypes attackType)
        {
            if (_attackTypes.TryGetValue(attackType, out BaseAttack attack))
                attack.ActivateAttack();
            else
                Debug.LogError($"Attack type {attackType} not found in dictionary.");
        }

        private IEnumerator LookAtTarget(Transform target)
        {
            while (transform.rotation != Quaternion.LookRotation(target.position - transform.position))
            {
                transform.rotation = Quaternion.Lerp
                (
                    transform.rotation, Quaternion.LookRotation(target.position - transform.position), 
                    _turnSpeed * Time.deltaTime
                );
                yield return null;
            }
        }
    }
}
