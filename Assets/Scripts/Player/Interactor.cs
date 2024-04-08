using UnityEngine;
using System;
using FishNet.Object;
using Examen.Pathfinding;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer), typeof(PathFollower))]
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private float damageAmount = 1;

        private Pointer _pointer;
        private PathFollower _pathFollower;
        private bool p_hasInteracted;

        public Action<Interactable> OnInteractableFound;

        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _pathFollower = GetComponent<PathFollower>();

            _pointer.OnPointedAtInteractable += ProcessPointerGameObject;
            _pathFollower.OnInteractableReached += Interact;
        }

        private void ProcessPointerGameObject(Interactable pointedObject)
        {
            if (!IsOwner)
                return;

            PreProcessPointerObject(pointedObject);
        }

        private void PreProcessPointerObject(Interactable pointedObject) => CheckForInteractable(pointedObject);

        private void CheckForInteractable(Interactable objectInQuestion)
        {
            p_hasInteracted = false;
            OnInteractableFound?.Invoke(objectInQuestion);
        }

        private void Interact(Interactable interactable)
        {
            if (p_hasInteracted)
                return;

            SentInteract(interactable);
            p_hasInteracted = true;
        }

        [ServerRpc]
        private void SentInteract(Interactable interactable) => interactable.Interact(ClientManager.Connection, damageAmount);
    }
}
