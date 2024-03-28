using System;
using System.Collections;
using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using Examen.Player;
using UnityEngine;

namespace Examen.Pathfinding
{
    [RequireComponent(typeof(Pathfinder))]
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] protected float p_speed = 5f;
        [SerializeField] protected float p_obstacleCheckDistance = 1f;
        [SerializeField] protected LayerMask p_obstaclesLayerMask;
        [SerializeField] protected Color p_pathColor = Color.red;

        protected Vector3 p_currentTarget;
        protected bool p_hasFoundBlockage;
        protected Pathfinder p_pathfinder;
        protected Pointer p_pointer;
        protected List<Node> p_currentPath = new();
        protected int p_currentPathIndex = 0;
        protected Coroutine p_followPathCoroutine;

        public bool IsPathBlocked 
            => Physics.Raycast(transform.position, transform.forward, p_obstacleCheckDistance, p_obstaclesLayerMask);

        public event Action OnPathCompleted;

        protected virtual void Start()
        {
            p_pathfinder = GetComponent<Pathfinder>();

            if (TryGetComponent(out p_pointer))
                p_pointer.OnPointedAtPosition += StartPath;
            
            OnPathCompleted += ResetBlockage;
        }

        protected virtual void Update()
        {
            if (IsPathBlocked && !p_hasFoundBlockage)
            {
                p_hasFoundBlockage = true;
                StartPath(p_currentTarget);
            }
        }

        public void StartPath(Vector3 target)
        {
            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            Vector3 startPosition = transform.position;
            p_currentTarget = target;
            p_currentPath = p_pathfinder.FindPath(startPosition, target);
            p_currentPathIndex = 0;

            if (p_currentPath != null && p_currentPath.Count > 0)
                p_followPathCoroutine = StartCoroutine(FollowPath());
        }

        protected IEnumerator FollowPath()
        {
            while (p_currentPathIndex < p_currentPath.Count)
            {
                Vector3 currentWaypoint = p_currentPath[p_currentPathIndex].position;

                while (Vector3.Distance(transform.position, currentWaypoint) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, p_speed * Time.deltaTime);
                    transform.LookAt(currentWaypoint);
                    Debug.DrawRay(transform.position, transform.forward * p_obstacleCheckDistance, Color.blue);
                    yield return null;
                }

                p_currentPathIndex++;
                yield return null;
            }

            OnPathCompleted?.Invoke();
        }

        protected void ResetBlockage() => p_hasFoundBlockage = false;

        protected virtual void OnDisable()
        {
            if (p_pointer != null)
                p_pointer.OnPointedAtPosition -= StartPath;
            
            OnPathCompleted -= ResetBlockage;
        }

        protected void OnDrawGizmos()
        {
            if (p_currentPath != null)
            {
                Gizmos.color = p_pathColor;

                for (int i = p_currentPathIndex; i < p_currentPath.Count - 1; i++)
                    Gizmos.DrawLine(p_currentPath[i].position, p_currentPath[i + 1].position);
            }
        }
    }
}
