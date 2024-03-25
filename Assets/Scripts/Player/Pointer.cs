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
        public Action<GameObject> OnPointedGameobject;

        private void OnEnable()
        {
            InputManager.SubscribeToAction("Click", OnPointPerformed, out _clickAction);
            InputManager.TryGetAction("PointerPosition").Enable();
        }

        private void OnDisable()
        {
            _clickAction.Disable();
            _clickAction.performed -= OnPointPerformed;
            InputManager.TryGetAction("PointerPosition").Disable();
        }

        private void Awake()
        {
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
            Vector2 pointerPosition = InputManager.TryGetAction("PointerPosition").ReadValue<Vector2>();
            
            Ray pointerRay = _myCamera.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(pointerRay, out RaycastHit hit, _pointerLayerMask))
            {
                _pointerWorldPosition = hit.point;
                OnPointedAtPosition?.Invoke(_pointerWorldPosition);
                OnPointedGameobject?.Invoke(hit.transform.gameObject);
            }
        }

        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            /*if(!IsOwner) 
                return;*/

            PointAtPosition();
        }
    }
}
