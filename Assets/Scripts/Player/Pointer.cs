using System;
using Minoord.Input;
using UnityEngine;

namespace Examen.Player
{
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private LayerMask _pointerLayerMask;
        private Camera _myCamera; // Replace with camera manager once this is implemented
        private Vector3 _pointerWorldPosition;

        public Action<Vector3> OnPointedAtPosition;

        private void OnEnable() => InputManager.TryGetAction("PointerPosition").Enable();

        private void OnDisable() => InputManager.TryGetAction("PointerPosition").Disable();

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
            }
        }
    }
}
