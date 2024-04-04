using UnityEngine;
using System;
using FishNet.Object;
using Examen.Pathfinding;

namespace Examen.Player
{
    [RequireComponent(typeof(PathFollower))]
    public class Interactor : NetworkBehaviour
    {
        #region Testing

        private void DebugPointer(Vector3 position)
            => Debug.DrawLine(transform.position, position, Color.red, 1f);
        #endregion

        [SerializeField] private float damageAmount = 1;

        private PathFollower _pointer;

        public Action<Interactable> OnInteractableFound;

        private void OnEnable()
        {
            _pointer = GetComponent<PathFollower>();
            _pointer.OnGameObjectReached += ProcessPointerGameObject;
        }

        private void ProcessPointerGameObject(GameObject pointedObject)
        {
            if (!IsOwner)
                return;

            PreProcessPointerObject(pointedObject);
        }

        [ServerRpc]
        private void PreProcessPointerObject(GameObject pointedObject) => CheckForInteractable(pointedObject);

        [Server]
        private void CheckForInteractable(GameObject objectInQuestion)
        {
            if (objectInQuestion.TryGetComponent<Interactable>(out Interactable interactable))
            {
                interactable.Interact(damageAmount);
                OnInteractableFound?.Invoke(interactable);
            }
        }
    }
}
