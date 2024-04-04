using UnityEngine;
using System;
using FishNet.Object;
using Examen.Pathfinding;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer), typeof(PathFollower))]
    public class Interactor : NetworkBehaviour
    {
        #region Testing

        private void DebugPointer(Vector3 position)
            => Debug.DrawLine(transform.position, position, Color.red, 1f);
        #endregion

        [SerializeField] private float damageAmount = 1;

        private Pointer _pointer;
        private PathFollower _pathFollower;

        public Action<Interactable> OnInteractableFound;

        private void OnEnable()
        {
            _pointer = GetComponent<Pointer>();
            _pathFollower = GetComponent<PathFollower>();

            _pointer.OnPointedGameobject += ProcessPointerGameObject;
            _pathFollower.OnInteractableReached += Interact;
        }

        private void ProcessPointerGameObject(GameObject pointedObject)
        {
            if (!IsOwner)
                return;

            PreProcessPointerObject(pointedObject);
        }

        [ServerRpc]
        private void PreProcessPointerObject(GameObject pointedObject) => CheckForInteractable(pointedObject);

        [ObserversRpc]
        private void CheckForInteractable(GameObject objectInQuestion)
        {
            if (objectInQuestion.TryGetComponent(out Interactable interactable))
            {
                //interactable.Interact(damageAmount);
                OnInteractableFound?.Invoke(interactable);
            }
        }

        [Server]
        private void Interact(Interactable interactable) => interactable.Interact(damageAmount);
    }
}
