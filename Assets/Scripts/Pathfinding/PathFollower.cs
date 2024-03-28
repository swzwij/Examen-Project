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
        [SerializeField] protected float p_waitTime = 1f;

        protected Vector3 p_currentTarget;
        protected bool p_hasFoundBlockage;
        protected bool p_isDetermined;
        protected Pathfinder p_pathfinder;
        protected Pointer p_pointer;
        protected List<Node> p_currentPath = new();
        protected int p_currentNodeIndex = 0;
        protected Coroutine p_followPathCoroutine;
        protected Coroutine p_waitForClearance;

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
                List<Node> newPath = p_pathfinder.FindPath(transform.position, p_currentTarget);

                if (p_isDetermined && newPath == null || newPath.Count == 0)
                {
                    p_waitForClearance = StartCoroutine(WaitForPathClearance());
                    return;
                }

                StartPath(p_currentTarget);
            }
        }

        public void StartPath(Vector3 target)
        {
            p_currentPath.Clear();

            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            Vector3 startPosition = transform.position;
            p_currentTarget = target;
            p_currentPath = p_pathfinder.FindPath(startPosition, target);
            p_currentNodeIndex = 0;

            if (p_currentPath != null && p_currentPath.Count > 0)
                p_followPathCoroutine = StartCoroutine(FollowPath());
            
            ResetBlockage();
        }

        public void ContinuePath()
        {
            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            if (p_currentPath != null && p_currentPath.Count > 0)
                p_followPathCoroutine = StartCoroutine(FollowPath());
        }

        protected IEnumerator FollowPath()
        {
            while (p_currentNodeIndex < p_currentPath.Count)
            {
                Vector3 currentNode = p_currentPath[p_currentNodeIndex].position;

                while (Vector3.Distance(transform.position, currentNode) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentNode, p_speed * Time.deltaTime);
                    transform.LookAt(currentNode);
                    Debug.DrawRay(transform.position, transform.forward * p_obstacleCheckDistance, Color.blue);
                    yield return null;
                }

                p_currentNodeIndex++;
                yield return null;
            }
            
            p_hasFoundBlockage = false;
            OnPathCompleted?.Invoke();
        }

        protected IEnumerator WaitForPathClearance()
        {
            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            yield return new WaitUntil(() => !IsPathBlocked);

            yield return new WaitForSeconds(p_waitTime);
            //ContinuePath();
            StartPath(p_currentTarget);
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

                for (int i = p_currentNodeIndex; i < p_currentPath.Count - 1; i++)
                    Gizmos.DrawLine(p_currentPath[i].position, p_currentPath[i + 1].position);
            }
        }
    }
}
