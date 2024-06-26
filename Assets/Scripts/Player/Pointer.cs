using System;
using Minoord.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;
using Swzwij.Extensions;
using Examen.Interactables.Resource;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Examen.Interactables;

namespace Examen.Player
{
    public class Pointer : NetworkBehaviour
    {
        [SerializeField] private float _pointerDistance = 10000f;
        [SerializeField] private UnityEngine.Camera _myCamera;
        private Vector3 _pointerWorldPosition;
        private InputAction _clickAction;

        public Action<RaycastHit> OnHovering;
        public Action<Vector3> OnPointedAtPosition;
        public Action<Vector3> OnPointedUIInteraction;
        public Action<GameObject> OnPointedGameobject;
        public Action<RaycastHit> OnPointedHitInfo;
        public Action<Interactable> OnPointedAtInteractable;

        public bool HasClickedUI { get; set; }
        public UnityEngine.Camera Camera => _myCamera;
        public Ray PointerRay 
        {
            get
            {
                Vector2 pointerPosition = InputManager.TryGetAction("PointerPosition").ReadValue<Vector2>();
                return _myCamera.ScreenPointToRay(pointerPosition);
            }
        }
        

        public bool CanPoint;

        private void Start()
        {
            CanPoint = true;

            InputManager.SubscribeToAction("Click", OnPointPerformed, out _clickAction);
            InputManager.TryGetAction("PointerPosition").Enable();

            InitCamera();
        }

        private void FixedUpdate() => Hover();

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

            if (!CanPoint)
                return;

            Vector2 pointerPosition = InputManager.TryGetAction("PointerPosition").ReadValue<Vector2>();

            PointerEventData eventData = new(EventSystem.current) { position = pointerPosition };
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                HasClickedUI = true;
                return;
            }

            Ray pointerRay = _myCamera.ScreenPointToRay(pointerPosition);

            ProcessPointerPosition(pointerRay);
        }

        private void ProcessPointerPosition(Ray pointerRay)
        {
            if (Physics.Raycast(pointerRay, out RaycastHit hit, _pointerDistance))
            {
                _pointerWorldPosition = hit.point;

                OnPointedHitInfo?.Invoke(hit);

                if (hit.collider.gameObject.layer == 5) // UI layer
                {
                    HasClickedUI = true;
                    return;
                }
                if (HasClickedUI)
                {
                    OnPointedUIInteraction?.Invoke(_pointerWorldPosition);
                    return;
                }

                OnPointedAtPosition?.Invoke(_pointerWorldPosition);
                OnPointedGameobject?.Invoke(hit.transform.gameObject);

                if (hit.transform.TryGetComponent(out Interactable interactable))
                    OnPointedAtInteractable?.Invoke(interactable);
            }
        }

        private void Hover()
        {
            if (!IsOwner)
                return;

            if (!Physics.Raycast(PointerRay, out RaycastHit hit, _pointerDistance))
                return;

            if (!hit.collider.gameObject.TryGetCachedComponent<Resource>())
                return;

            ProcessHover(hit.collider.gameObject.TryGetCachedComponent<Resource>());
            OnHovering?.Invoke(hit);
        }

        [ServerRpc]
        private void ProcessHover(Resource resource) => resource.ProcessHover();

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
