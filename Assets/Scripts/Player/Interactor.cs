using UnityEngine;
using System;
using FishNet.Object;
using Examen.Pathfinding;
using Examen.Networking;
using FishNet.Managing;
using FishNet.Connection;
using Examen.Interactables.Resource;
using System.Collections;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer), typeof(PathFollower))]
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private float damageAmount = 1;

        private NetworkManager _networkManager;
        private Pointer _pointer;
        private PathFollower _pathFollower;
        private bool _hasInteracted;
        private bool _isGathering;
        private Resource _currentInteractableResource;

        public Action<Interactable> OnInteractableFound;


        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _pathFollower = GetComponent<PathFollower>();

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
            _hasInteracted = false;
            OnInteractableFound?.Invoke(currentInteractable);
        }

        private void Interact(Interactable interactable)
        {
            if (_hasInteracted)
                return;

            SentInteract(interactable, _networkManager.ClientManager.Connection);

            _hasInteracted = true;
        }

        [ServerRpc]
        private void SentInteract(Interactable interactable, NetworkConnection connection)
        {
            if (interactable is Resource resource)
                StartCoroutine(Gathering(resource, connection));
            else
                interactable.Interact(connection, damageAmount);
        }

        protected IEnumerator Gathering(Resource resource, NetworkConnection connection)
        {
            _isGathering = true;

            while (_isGathering && !resource.IsDead)
            {
                resource.Interact(connection, damageAmount);
                yield return new WaitForSeconds(6); //TODO: make this timer the timer of the player gather information
            }

            _isGathering = false;
        }

        private void OnDestroy() => _isGathering = false;

    }
}
