using UnityEngine;
using Minoord.Input;
using UnityEngine.InputSystem;
using System;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer))]
    public class Interactor : MonoBehaviour
    {
        #region Testing
        private void Start() => _pointer.OnPointedAtPosition += DebugPointer;
        private void DebugPointer(Vector3 position) 
            => Debug.DrawLine(transform.position, position, Color.red, 1f);
        #endregion

        private InputAction _interactAction;
        private Pointer _pointer;

        public Action OnInteract; // Change to Action<Interactable>?

        private void OnEnable() => InputManager.SubscribeToAction("Interact", OnInteractPerformed, out _interactAction);

        private void OnDisable()
        {
            _interactAction.Disable();
            _interactAction.performed -= OnInteractPerformed;
        }

        private void Awake() => _pointer = GetComponent<Pointer>();

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _pointer.PointAtPosition();
            OnInteract?.Invoke();
        }
    }
}
