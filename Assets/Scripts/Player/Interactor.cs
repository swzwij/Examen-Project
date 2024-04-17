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

        private Dictionary<InteractableTypes, string> _interactableTypeToAnimation = new()
        {
            {InteractableTypes.ResourceUnknown, "Interact"},
            {InteractableTypes.ResourceWood, "Chop"},
            {InteractableTypes.ResourceStone, "Mine"},
            {InteractableTypes.ResourceSpecial, "Special"},
            {InteractableTypes.StructureBallistae, "UseBallistae"},
            {InteractableTypes.InteractRepair, "Repair"}
        };

        public Action<Interactable> OnInteractableFound;

        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _pathFollower = GetComponent<PathFollower>();
            _animator = GetComponent<Animator>();

            _pointer.OnPointedAtInteractable += ProcessPointerGameObject;
            _pathFollower.OnInteractableReached += Interact;

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
            if (_hasInteracted)
                return;

            _animator.SetTrigger(_interactableTypeToAnimation[interactable.Type]);

            if (!IsOwner)
                return; 
            
            _hasInteracted = true;
            StartCoroutine(InteractCooldown(interactable, _animator.GetCurrentAnimatorStateInfo(0).length));
        }

        private IEnumerator InteractCooldown(Interactable interactable, float cooldownTime)
        {
            yield return new WaitForSeconds(cooldownTime);
            SentInteract(interactable, _networkManager.ClientManager.Connection);
            _hasInteracted = false;
        }

        [ServerRpc]
        private void SentInteract(Interactable interactable, NetworkConnection connection) 
            => interactable.Interact(connection, damageAmount);
    }
}
