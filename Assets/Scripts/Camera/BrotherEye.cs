using System;
using Minoord.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Examen.BrotherEye
{
    [RequireComponent(typeof(ZoomManager))]
    public class BrotherEye : MonoBehaviour
    {
        [SerializeField] private float _cameraDistance = 10f;
        [SerializeField] private float _cameraAngle = 45f;
        [SerializeField] private float _transitionSpeed = 0.25f;
        [SerializeField] private Transform _player; // replace with GetComponent when prefab is merged

        private InputAction _zoomAction;
        private ZoomManager _zoomManager;
        private Transform _trackedObject;

        public float ZoomModifier => _zoomManager.GetZoom();
        public float CameraDistance => _cameraDistance * ZoomModifier;

        public Action<CameraSettings> OnCameraSettingsChanged;

        private void OnEnable()
        {
            _zoomManager = GetComponent<ZoomManager>();
            _trackedObject = _player;
        }

        private void Start() => InputManager.SubscribeToAction("Zoom", OnZoomPerformed, out _zoomAction);

        private void Update() => TrackObject(_trackedObject);

        private void OnZoomPerformed(InputAction.CallbackContext context) => _zoomManager.GetNextZoom();
        
        private void TrackObject(Transform trackedObject)
        {
            float cameraX = _trackedObject.position.y + CameraDistance * Mathf.Cos(_cameraAngle * Mathf.Deg2Rad);
            float cameraZ = _trackedObject.position.z + CameraDistance * Mathf.Sin(_cameraAngle * Mathf.Deg2Rad);

            Vector3 rotatedPosition = new(transform.position.x, cameraX, cameraZ);
            transform.position = Vector3.Lerp(transform.position, rotatedPosition, _transitionSpeed);
            transform.LookAt(trackedObject);
        }

        private void OnDisable() => _zoomAction.performed -= OnZoomPerformed;
    }
}
