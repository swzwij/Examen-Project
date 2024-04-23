using System;
using System.Collections;
using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

namespace Examen.Pathfinding
{
    public class WaypointFollower : PathFollower
    {
        [SerializeField] private Transform _waypointsParent;
        [SerializeField] private List<Transform> _waypoints = new();
        private List<Node> _completePath = new();
        private int _currentWaypointIndex = 0;
        private bool _hasInitialised;

        public event Action OnBossInitialised;
        public event Action<HealthData> OnStructureEncountered;
        public event Action<bool> OnPathCleared;

        protected override void Start() => base.Start();

        protected void InitFollower()
        {
            _waypointsParent.name = $"{gameObject.name} - Waypoints";

            _waypoints.Clear();

            for (int i = _waypointsParent.childCount - 1; i >= 0; i--)
                _waypoints.Add(_waypointsParent.GetChild(i));

            _waypointsParent.SetParent(null);
            _waypoints.Reverse();

            if (_waypoints.Count == 0)
                return;

            GenerateCompletePath();
            OnBossInitialised?.Invoke();
        }

        [Server]
        private void GenerateCompletePath()
        {
            for (int i = _currentWaypointIndex; i < _waypoints.Count; i++)
            {
                if (i == _currentWaypointIndex)
                {
                    StartPath(_waypoints[i].position);
                    _completePath.AddRange(p_currentPath);
                }
                else
                {
                    _completePath.AddRange(p_pathfinder.FindPath(_waypoints[i - 1].position, _waypoints[i].position));
                }
            }

            p_currentPath = _completePath;
            p_currentTarget = p_currentPath[^1].Position;
        }

        protected override void FixedUpdate() 
        {
            if (!IsServer)
                return;

            if (!_hasInitialised)
            {
                _hasInitialised = true;
                InitFollower();
            }

            UpdateFollower(_waypoints);
        }

        [Server]
        private void UpdateFollower(List<Transform> waypoints)
        {
            if (Vector3.Distance(transform.position, waypoints[_currentWaypointIndex].position) < 5f 
            && _currentWaypointIndex < waypoints.Count-1)
                _currentWaypointIndex++;

            if (IsPathBlocked && !p_hasFoundBlockage)
            {
                p_hasFoundBlockage = true;

                p_waitForClearance = StartCoroutine(WaitForPathClearance());

                if (p_obstacleHit.collider.TryGetComponent(out HealthData healthData))
                    OnStructureEncountered?.Invoke(healthData);
            }
        }

        [Server]
        protected IEnumerator WaitForPathClearance()
        {
            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            OnPathCleared?.Invoke(false);

            yield return new WaitUntil(() => !IsPathBlocked);

            yield return new WaitForSeconds(p_waitTime);
            ContinuePath();
            OnPathCleared?.Invoke(true);
        }

        /// <summary>
        /// Continues following the current path.
        /// </summary>
        [Server]
        public void ContinuePath()
        {
            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            if (p_currentPath != null && p_currentPath.Count > 0)
                p_followPathCoroutine = StartCoroutine(FollowPath());
        }
    }
}
