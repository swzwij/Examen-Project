using UnityEngine;
using System;
using FishNet.Object;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer))]
    public class Interactor : NetworkBehaviour
    {
        #region Testing
        private void Start() => _pointer.OnPointedAtPosition += DebugPointer;
        private void OnDisable() => _pointer.OnPointedAtPosition -= DebugPointer;

        private void DebugPointer(Vector3 position)
            => Debug.DrawLine(transform.position, position, Color.red, 1f);
        #endregion

        [SerializeField] private float damageAmount = 1;

        private Pointer _pointer;

        public Action<Interactable> OnInteractableFound;

        private void OnEnable()
        {
            _pointer = GetComponent<Pointer>();
            _pointer.OnPointedGameobject += ProcessPointerGameObject;
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
                interactable.Interact(damageAmount);//Replace this later with OnInteractableFound?.Invoke(interactable);
        }
    }
}
