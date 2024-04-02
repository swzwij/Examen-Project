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


        public Action<Vector3> OnPointedAtPosition;

        private void Start()
        {
            InputManager.SubscribeToAction("Click", OnPointPerformed, out _clickAction);
            InputManager.TryGetAction("PointerPosition").Enable();

            if (TryGetComponent(out Camera camera))
                _myCamera = camera;
            else
                _myCamera = Camera.main;
        }

        /// <summary>
        /// Points the pointer at the position based on the input from the "PointerPosition" action.
        /// </summary>
        public void PointAtPosition()
        {
            if (!IsOwner)
                return;

            Vector2 pointerPosition = InputManager.TryGetAction("PointerPosition").ReadValue<Vector2>();
            Ray pointerRay = _myCamera.ScreenPointToRay(pointerPosition);

            ProcessPointerPosition(pointerPosition, pointerRay);
        }

        private void ProcessPointerPosition(Vector2 pointerPosition, Ray pointerRay)
        {
            if (Physics.Raycast(pointerRay, out RaycastHit hit, _pointerLayerMask))
            {
                _pointerWorldPosition = hit.point;
                OnPointedAtPosition?.Invoke(_pointerWorldPosition);
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
            _clickAction.Disable();
            _clickAction.performed -= OnPointPerformed;
            InputManager.TryGetAction("PointerPosition").Disable();
        }
    }
}
