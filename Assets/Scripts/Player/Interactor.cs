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

        private Pointer _pointer;
        private PathFollower pathFollower;
        private GameObject clickedObject;

        public Action<Interactable> OnInteractableFound;

        private void OnEnable()
        {
            _pointer = GetComponent<Pointer>();
            pathFollower = GetComponent<PathFollower>();

            _pointer.OnPointedGameobject += SetObject;
            pathFollower.OnGameObjectReached += ProcessPointerGameObject;
        }

        private void SetObject(GameObject obj) => clickedObject = obj;

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
            if (clickedObject == objectInQuestion && objectInQuestion.TryGetComponent<Interactable>(out Interactable interactable))
            {
                interactable.Interact(damageAmount);
                OnInteractableFound?.Invoke(interactable);
            }
        }
    }
}
