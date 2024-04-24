using System;
using System.Collections;
using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using Examen.Player;
using FishNet.Component.Spawning;
using FishNet.Managing.Client;
using FishNet.Object;
using UnityEngine;

namespace Examen.Pathfinding
{
    [RequireComponent(typeof(Pathfinder))]
    public class PathFollower : NetworkBehaviour
    {
        [SerializeField] protected float p_speed = 5f;
        [SerializeField] protected float p_turnSpeed = 15f;
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
        protected Coroutine p_turnCoroutine;
        protected LineRenderer p_pathRenderer;
        
        protected RaycastHit p_obstacleHit;
        public bool IsPathBlocked 
            => Physics.Raycast(transform.position, transform.forward, out p_obstacleHit, p_obstacleCheckDistance, p_obstaclesLayerMask);

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

                Vector3 nodeDirection = adjustedNode - transform.position;

                // if (p_turnCoroutine == null)
                //     StopCoroutine(p_turnCoroutine);

                // p_turnCoroutine = StartCoroutine(TurnToTarget(adjustedNode, p_turnSpeed));

                while (Vector3.Distance(transform.position, adjustedNode) > 0.1f)
                {
                    float angleToNode = Vector3.Angle(nodeDirection, transform.forward);
                    print($"Angle to node: {angleToNode / 100 + 1}");
                    MoveToTarget(adjustedNode, p_speed / (angleToNode / 100 + 1));
                    TurnToTarget(adjustedNode);

                    //BroadcastPosition(transform.position);
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
        protected void BroadcastRotation(Quaternion rotation) => transform.rotation = rotation;

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

        [Server]
        private void MoveToTarget(Vector3 target, float speed)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            BroadcastPosition(transform.position);
        }

        [Server]
        private void TurnToTarget(Vector3 target)
        {
            Vector3 targetDirection = target - transform.position;
            float angle = Vector3.Angle(targetDirection, transform.forward);

            if (angle > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards
                (
                    transform.rotation, Quaternion.LookRotation(targetDirection), p_turnSpeed * Time.deltaTime
                );
                BroadcastRotation(transform.rotation);
            }
        }

        private IEnumerator TurnToTarget(Vector3 target, float turnSpeed)
        {
            Vector3 targetDirection = target - transform.position;
            float angle = Vector3.Angle(targetDirection, transform.forward);

            while (angle > 0.1f)
            {
                targetDirection.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), turnSpeed * Time.deltaTime);
                angle = Vector3.Angle(targetDirection, transform.forward);
                BroadcastRotation(transform.rotation);
                yield return null;
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
