using System;
using Minoord.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;

namespace Examen.Player
{
    public class Pointer : NetworkBehaviour
    {
        [SerializeField] private LayerMask _pointerLayerMask;
        private Camera _myCamera; // Replace with camera manager once this is implemented
        private Vector3 _pointerWorldPosition;
        private InputAction _clickAction;
        private InputManager _inputManager;

        public Action<Vector3> OnPointedAtPosition;

        private void Start()
        {
            if (TryGetComponent(out _inputManager))
            {
                _inputManager.SubscribeToAction("Click", OnPointPerformed, out _clickAction);
                _inputManager.TryGetAction("PointerPosition").Enable();
            }

            if (TryGetComponent(out Camera camera))
                _myCamera = camera;
            else
                _myCamera = Camera.main;
        }

        [ServerRpc]
        /// <summary>
        /// Points the pointer at the position based on the input from the "PointerPosition" action.
        /// </summary>
        public void PointAtPosition()
        {
            if (!IsOwner)
                return;

            Vector2 pointerPosition = _inputManager.TryGetAction("PointerPosition").ReadValue<Vector2>();
            
            ProcessPointerPosition(pointerPosition);
        }

        [Server]
        private void ProcessPointerPosition(Vector2 pointerPosition)
        {
            Ray pointerRay = _myCamera.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(pointerRay, out RaycastHit hit, _pointerLayerMask))
            {
                _pointerWorldPosition = hit.point;
                OnPointedAtPosition?.Invoke(_pointerWorldPosition);
                Debug.Log($"Pointer pointed at position: {_pointerWorldPosition}");
            }
        }

        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            if(!IsOwner) 
                return;

            PointAtPosition();
        }

        private void OnDestroy() 
        {
            if (_inputManager == null)
                return;

            _clickAction.Disable();
            _clickAction.performed -= OnPointPerformed;
            _inputManager.TryGetAction("PointerPosition").Disable();
        }
    }
}
