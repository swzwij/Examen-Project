using System.Collections;
using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using FishNet.Object;
using UnityEngine;

namespace Examen.Pathfinding
{
    public class WaypointFollower : PathFollower
    {
        private List<Transform> _waypoints = new();
        private List<Node> _completePath = new();
        private int _currentWaypointIndex = 0;

        public List<Transform> Waypoints { set { _waypoints = value; } }

        protected override void Start()
        {
            base.Start();

            if (_waypoints.Count == 0)
                return;

            GenerateCompletePath();
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

            UpdateBoss(_waypoints);
        }

        [Server]
        private void UpdateBoss(List<Transform> waypoints)
        {
            if (Vector3.Distance(transform.position, waypoints[_currentWaypointIndex].position) < 5f 
            && _currentWaypointIndex < waypoints.Count-1)
                _currentWaypointIndex++;

            if (IsPathBlocked && !p_hasFoundBlockage)
            {
                p_hasFoundBlockage = true;
                List<Node> newPath = p_pathfinder.FindPath(transform.position, p_currentTarget);

                if (newPath.Count == 0)
                {
                    p_waitForClearance = StartCoroutine(WaitForPathClearance());
                    return;
                }
                
                GenerateCompletePath();
            }
        }

        [Server]
        protected IEnumerator WaitForPathClearance()
        {
            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            yield return new WaitUntil(() => !IsPathBlocked);

            yield return new WaitForSeconds(p_waitTime);
            ContinuePath();
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
