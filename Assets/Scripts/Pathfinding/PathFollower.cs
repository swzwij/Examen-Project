using System;
using System.Collections;
using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using Examen.Player;
using FishNet.Object;
using UnityEngine;

namespace Examen.Pathfinding
{
    [RequireComponent(typeof(Pathfinder))]
    public class PathFollower : NetworkBehaviour
    {
        [SerializeField] protected float p_speed = 5f;
        [SerializeField] protected float p_obstacleCheckDistance = 1f;
        [SerializeField] protected LayerMask p_obstaclesLayerMask;
        [SerializeField] protected Color p_pathColor = Color.red;
        [SerializeField] protected float p_waitTime = 1f;

        protected Vector3 p_currentTarget;
        protected bool p_hasFoundBlockage;
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
                p_pointer.OnPointedAtPosition += ProcessPointerPosition;
        }

        protected virtual void FixedUpdate()
        {
            if (IsPathBlocked && !p_hasFoundBlockage)
            {
                p_hasFoundBlockage = true;
                StartPath(p_currentTarget);
            }
        }


        
        protected void ProcessPointerPosition(Vector3 position)
        {
            if (!IsOwner)
                return;
            PreProcessPointerPosition(position);
        }

        protected void PreProcessPointerPositionByPass(Vector3 position)
        {
            PreProcessPointerPosition(position);
        }

        [ServerRpc]
        protected void PreProcessPointerPosition(Vector3 position)
        {
            if (p_waitForClearance != null)
                StopCoroutine(p_waitForClearance);

            StartPath(position);
        }

        [Server]
        public void StartPath(Vector3 target)
        {
            // if (!IsOwner)
            //     return;
            
            p_currentPath.Clear();

            if (p_followPathCoroutine != null)
                StopCoroutine(p_followPathCoroutine);

            p_currentTarget = target;
            p_currentPath = p_pathfinder.FindPath(transform.position, target);
            p_currentNodeIndex = 0;

            if (p_currentPath.Count > 0)
                p_followPathCoroutine = StartCoroutine(FollowPath());
        }

        [Server]
        protected IEnumerator FollowPath()
        {
            ResetBlockage();
            while (p_currentNodeIndex < p_currentPath.Count)
            {
                Vector3 currentNode = p_currentPath[p_currentNodeIndex].Position;

                while (Vector3.Distance(transform.position, currentNode) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentNode, p_speed * Time.deltaTime);
                    transform.LookAt(currentNode);

                    BroadcastPosition(transform.position);
                    Debug.DrawRay(transform.position, transform.forward * p_obstacleCheckDistance, Color.blue);
                    yield return null;
                }

                p_currentNodeIndex++;
                yield return null;
            }
            
            p_hasFoundBlockage = false;
            OnPathCompleted?.Invoke();
        }

        [ObserversRpc]
        protected void BroadcastPosition(Vector3 position)
        {
            transform.position = position;
        }

        [Server]
        protected void ResetBlockage() => p_hasFoundBlockage = false;

        protected virtual void OnDisable()
        {
            if (p_pointer != null)
                p_pointer.OnPointedAtPosition -= StartPath;
        }

        protected void OnDrawGizmos()
        {
            // if (!IsOwner)
            //     return;

            if (p_currentPath != null)
            {
                Gizmos.color = p_pathColor;

                for (int i = p_currentNodeIndex; i < p_currentPath.Count - 1; i++)
                    Gizmos.DrawLine(p_currentPath[i].Position, p_currentPath[i + 1].Position);
            }
        }
    }
}
