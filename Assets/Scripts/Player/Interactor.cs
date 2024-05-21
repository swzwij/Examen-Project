using UnityEngine;
using System;
using FishNet.Object;
using Examen.Pathfinding;
using Examen.Networking;
using FishNet.Managing;
using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using Examen.Interactables;
using Examen.Interactables.Resource;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer), typeof(PathFollower), typeof(Animator))]
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private float damageAmount = 1;

        private NetworkManager _networkManager;
        private Pointer _pointer;
        private PathFollower _pathFollower;
        private bool _hasInteracted;
        private Animator _animator;
        private Coroutine _interactCooldown;
        private Coroutine _gatheringCoroutine;

        private Dictionary<InteractableTypes, string> _interactableTypeToAnimation = new()
        {
            {InteractableTypes.ResourceUnknown, "Interact"},
            {InteractableTypes.ResourceWood, "Chop"},
            {InteractableTypes.ResourceStone, "Mine"},
            {InteractableTypes.ResourceSpecial, "Special"},
            {InteractableTypes.StructureBallistae, "UseBallistae"},
            {InteractableTypes.InteractRepair, "Repair"}
        };
        private bool _isGathering;
        private Resource _currentInteractableResource;

        public Action<Interactable> OnInteractableFound;


        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _pathFollower = GetComponent<PathFollower>();
            _animator = GetComponent<Animator>();

            _pointer.OnPointedAtInteractable += ProcessPointerGameObject;
            _pathFollower.OnInteractableReached += Interact;
            _pathFollower.OnPathStarted += () => _isGathering = false;

            ServerInstance.Instance.TryGetComponent(out _networkManager);

            if (_networkManager == null)
                Debug.LogError("Couldn't find NetworkManager");
        }

        private void ProcessPointerGameObject(Interactable pointedObject)
        {
            if (!IsOwner)
                return;

            PreProcessPointerObject(pointedObject);
        }

        private void PreProcessPointerObject(Interactable pointedObject) => CheckForInteractable(pointedObject);

        private void CheckForInteractable(Interactable currentInteractable)
        {
            OnInteractableFound?.Invoke(currentInteractable);
        }

        private void Interact(Interactable interactable)
        {
            if (_hasInteracted || _isGathering || _interactCooldown != null)
                return;

            _animator.SetTrigger(_interactableTypeToAnimation[interactable.Type]);
            BroadcastAnimationTrigger(_interactableTypeToAnimation[interactable.Type]);

            if (!IsOwner)
                return; 
            
            SentInteract(interactable, _networkManager.ClientManager.Connection);

            _hasInteracted = true;
            _interactCooldown = 
                StartCoroutine(InteractCooldown(interactable, _animator.GetCurrentAnimatorStateInfo(0).length));
        }

        private IEnumerator InteractCooldown(Interactable interactable, float cooldownTime)
        {
            SentInteract(interactable, _networkManager.ClientManager.Connection);
            _hasInteracted = false;
            yield return new WaitForSeconds(cooldownTime);
            _interactCooldown = null;
        }

        [ServerRpc]
        private void SentInteract(Interactable interactable, NetworkConnection connection)
        {
            if (interactable is Resource resource)
            {
                if (_gatheringCoroutine != null)
                    return;
                
                _gatheringCoroutine = StartCoroutine(Gathering(resource, connection));
            }
            else
                interactable.Interact(connection, damageAmount);
        }

        protected IEnumerator Gathering(Resource resource, NetworkConnection connection)
        {
            _isGathering = true;

            while (_isGathering && !resource.IsDead)
            {
                // if (!_isGathering || resource.IsDead)
                //     break;

                _animator.SetTrigger(_interactableTypeToAnimation[resource.Type]);
                BroadcastAnimationTrigger(_interactableTypeToAnimation[resource.Type]);
                resource.Interact(connection, damageAmount);
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
            }

            _isGathering = false;
            _gatheringCoroutine = null;
        }

        [ObserversRpc]
        protected void BroadcastAnimationTrigger(string trigger) => _animator.SetTrigger(trigger);

        private void OnDestroy() => _isGathering = false;

    }
}
