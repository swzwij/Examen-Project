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
        [SerializeField] protected float p_waitTime = 1f;

        protected Interactable p_targetInteractable;
        protected bool p_hasInteracted;
        protected Vector3 p_currentTarget;
        protected bool p_hasFoundBlockage;
        protected Pathfinder p_pathfinder;
        protected Pointer p_pointer;
        protected Interactor p_interactor;
        protected List<Node> p_currentPath = new();
        protected List<Vector3> P_CurrentPathPositions => p_currentPath.ConvertAll(node => node.Position);
        protected int p_currentNodeIndex = 0;
        protected Coroutine p_followPathCoroutine;
        protected Coroutine p_waitForClearance;
        protected LineRenderer p_pathRenderer;
        
        public bool IsPathBlocked 
            => Physics.Raycast(transform.position, transform.forward, p_obstacleCheckDistance, p_obstaclesLayerMask);

        public event Action OnPathCompleted;
        public event Action<Interactable> OnInteractableReached;

        protected virtual void Start() 
        {
            p_pathfinder = GetComponent<Pathfinder>();
            p_pathRenderer = GetComponent<LineRenderer>();

            if (TryGetComponent(out p_pointer))
                p_pointer.OnPointedAtPosition += ProcessPointerPosition;

            if (TryGetComponent(out p_interactor))
                p_interactor.OnInteractableFound += ProcessPointerPosition;
        }

        protected virtual void FixedUpdate()
        {
            if (!IsOwner)
                return;

            RequestDIstanceToTarget();
            if (p_hasInteracted)
                return;

            if (!IsPathBlocked || p_hasFoundBlockage)
                return;

            p_hasFoundBlockage = true;
            StartPath(p_currentTarget);
        }

        protected void ProcessPointerPosition(Interactable targetInteractable)
        {
            p_hasInteracted = false;
            p_targetInteractable = targetInteractable;
            PreProcessPointerPosition(p_targetInteractable.transform.position);
        }

        protected void ProcessPointerPosition(Vector3 position)
        {
            if (!IsOwner)
                return;

            PreProcessPointerPosition(position);
        }

        [ServerRpc]
        protected void PreProcessPointerPosition(Vector3 position)
        {
            if (p_waitForClearance != null)
                StopCoroutine(p_waitForClearance);

            StartPath(position);
        }

        /// <summary>
        /// Starts the pathfinding process to the specified target.
        /// </summary>
        /// <param name="target">The target position to pathfind to.</param>
        [Server]
        public void StartPath(Vector3 target)
        {   
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
                Vector3 adjustedNode = currentNode;
                adjustedNode.y = transform.localScale.y/2 + 
                p_currentPath[p_currentNodeIndex].NodeHeightOffset + 
                p_currentPath[p_currentNodeIndex].Position.y;

                while (Vector3.Distance(transform.position, adjustedNode) > 0.1f)
                {
                    transform.position = 
                        Vector3.MoveTowards(transform.position, adjustedNode, p_speed * Time.deltaTime);
                    transform.LookAt(adjustedNode);

                    BroadcastPosition(transform.position);
                    BroadcastPath(P_CurrentPathPositions);
                    Debug.DrawRay(transform.position, transform.forward * p_obstacleCheckDistance, Color.blue);
                    yield return null;
                }

                p_currentPath.RemoveAt(p_currentNodeIndex);
                yield return null;
            }
            
            p_hasFoundBlockage = false;
            OnPathCompleted?.Invoke();
        }

        [ObserversRpc]
        protected void BroadcastPosition(Vector3 position) => transform.position = position;

        [ObserversRpc]
        protected void BroadcastPath(List<Vector3> path)
        {
            if (!IsOwner)
                return;

            DrawPath(path);
        }

        [Server]
        protected void ResetBlockage() => p_hasFoundBlockage = false;

        private void RequestDIstanceToTarget()
        {
            if (p_targetInteractable == null)
                return;

            if (DistanceToTarget(p_targetInteractable.transform.position) >= p_obstacleCheckDistance)
                return;
            
            BroadcastInteractableReached();
        }

        private float DistanceToTarget(Vector3 target)
        {
            Vector3 difference = transform.position - target;
            float distance = Mathf.Sqrt(Mathf.Pow(difference.x, 2f) 
                + Mathf.Pow(difference.y, 2f) + Mathf.Pow(difference.z, 2f));

            return distance;
        }

        private void BroadcastInteractableReached()
        {
            Vector3 targetPosition = p_targetInteractable.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);

            if (!p_hasInteracted)
            {
                p_hasInteracted = true;
                OnInteractableReached?.Invoke(p_targetInteractable);
            }
        }
        
        protected void DrawPath(List<Vector3> path)
        {
            p_pathRenderer.positionCount = path.Count;
            p_pathRenderer.SetPositions(path.ToArray());
        }

        protected virtual void OnDisable()
        {
            if (p_pointer == null)
                return;
            
            p_pointer.OnPointedAtPosition -= StartPath;
        }
    }
}
