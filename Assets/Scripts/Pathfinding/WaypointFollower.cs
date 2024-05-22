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
        [SerializeField] private List<Transform> _waypoints = new();
        [SerializeField] private float _waypointDistanceThreshold = 5f;
        private List<Node> _completePath = new();
        private int _currentWaypointIndex = 0;
        
        public event Action OnFollowerInitialised;
        public event Action<HealthData> OnStructureEncountered;
        public event Action<bool> OnPathCleared;

        public List<Transform> Waypoints { get => _waypoints; set { _waypoints = value; } }

        /// <summary>
        /// Reset the waypoint index to 0 and Generates a complete path
        /// </summary>
        [Server]
        public void ResetWaypointIndex()
        {
            _currentWaypointIndex = 0;
            GenerateCompletePath();
            OnFollowerInitialised?.Invoke();
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
            if (!IsServer || _waypoints.Count <= 0)
                return;

            UpdateFollower(_waypoints);
        }

        [Server]
        private void UpdateFollower(List<Transform> waypoints)
        {
            if (_currentWaypointIndex > _waypoints.Count)
                return;

            float sqrDistanceToTarget = (transform.position - waypoints[_currentWaypointIndex].position).sqrMagnitude;
            float sqrDistanceThreshold = _waypointDistanceThreshold * _waypointDistanceThreshold;
            if (sqrDistanceToTarget < sqrDistanceThreshold && _currentWaypointIndex < waypoints.Count - 1)
                _currentWaypointIndex++;

            if (!IsPathBlocked || p_hasFoundBlockage)
                return;
            
            p_hasFoundBlockage = true;

            GenerateCompletePath();
            if (IsNewPathBlocked())
                return;

            if (IsNextWaypointCloser())
                _currentWaypointIndex++;
        }

        [Server]
        protected bool IsNewPathBlocked()
        {
            if (p_currentPath.Count > 0)
                return false;
            
            p_waitForClearance = StartCoroutine(WaitForPathClearance());

            if (p_obstacleHit.collider.TryGetComponent(out HealthData healthData))
                OnStructureEncountered?.Invoke(healthData);

            return true;
        }

        [Server]
        protected bool IsNextWaypointCloser()
        {
            if (_currentWaypointIndex >= _waypoints.Count)
                return false;

            float sqrDistanceToTarget = (transform.position - _waypoints[_currentWaypointIndex].position).sqrMagnitude;
            float sqrDistanceThreshold = _waypointDistanceThreshold * _waypointDistanceThreshold;
            if (sqrDistanceToTarget >= sqrDistanceThreshold)
                return false;
            
            return true;
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
        /// Toggles the waiting state of the waypoint follower.
        /// </summary>
        /// <param name="isWaiting">A boolean value indicating whether the follower should wait or continue the path.</param>
        [Server]
        public void ToggleWaiting(bool isWaiting)
        {
            if (isWaiting)
            {
                if (p_followPathCoroutine != null)
                    StopCoroutine(p_followPathCoroutine);
            }
            else
            {
                ContinuePath();
            }
        }

        /// <summary>
        /// Continues following the current path.
        /// </summary>
        [Server]
        public void ContinuePath()
        {
            if (p_hasStoppedPath)
                return;

            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            if (p_currentPath != null && p_currentPath.Count > 0)
                p_followPathCoroutine = StartCoroutine(FollowPath());
        }
    }
}
