using System;
using Minoord.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;

namespace Examen.Player
{
    public class Pointer : NetworkBehaviour
    {
        [SerializeField] private float _pointerDistance = 10000f;
        [SerializeField] private UnityEngine.Camera _myCamera;
        private Vector3 _pointerWorldPosition;
        private InputAction _clickAction;

        public Action<Vector3> OnPointedAtPosition;
        public Action<Interactable> OnPointedAtInteractable;

        private void Start()
        {
            InputManager.SubscribeToAction("Click", OnPointPerformed, out _clickAction);
            InputManager.TryGetAction("PointerPosition").Enable();

            InitCamera();
        }

        private void InitCamera()
        {
            if (!IsOwner)
                return;

            _myCamera.gameObject.SetActive(true);
            _myCamera.transform.SetParent(null);
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

            ProcessPointerPosition(pointerRay);
        }

        private void ProcessPointerPosition(Ray pointerRay)
        {
            if (Physics.Raycast(pointerRay, out RaycastHit hit, _pointerDistance))
            {
                _pointerWorldPosition = hit.point;
                OnPointedAtPosition?.Invoke(_pointerWorldPosition);

                if (hit.transform.TryGetComponent(out Interactable interactable))
                    OnPointedAtInteractable?.Invoke(interactable);
            }
        }

        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            if (!IsOwner)
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
