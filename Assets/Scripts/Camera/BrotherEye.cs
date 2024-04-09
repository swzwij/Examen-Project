using System;
using FishNet.Object;
using Minoord.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Examen.Camera.BrotherEye
{
    [RequireComponent(typeof(ZoomManager), typeof(CameraManager))]
    public class BrotherEye : NetworkBehaviour
    {
        private InputAction _zoomAction;
        private CameraManager _cameraManager;
        private ZoomManager _zoomManager;
        private Transform _trackedObject;
        private Transform _player;
        public float CameraDistance => _cameraManager.CurrentCameraSettings.Distance 
            * _zoomManager.ZoomSettings[_cameraManager.CurrentCameraSettings.ZoomLevel];
        public float CameraAngle => _cameraManager.CurrentCameraSettings.Angle;
        public float TransitionSpeed => _cameraManager.CurrentCameraSettings.TransitionSpeed;
        public Vector3 CameraOffset => _cameraManager.CurrentCameraSettings.Offset;

        public Action<CameraSettings> OnCameraSettingsChanged;

        private void OnEnable()
        {
            _player = GetComponentInParent<Player.Pointer>().transform;
            _cameraManager = GetComponent<CameraManager>();
            _zoomManager = GetComponent<ZoomManager>();
            _trackedObject = _player;
        }

        private void Start() => InputManager.SubscribeToAction("Zoom", OnZoomPerformed, out _zoomAction);

        private void Update() => TrackObject(_trackedObject);

        private void OnZoomPerformed(InputAction.CallbackContext context) => _cameraManager.GetNextCameraType();
        
        private void TrackObject(Transform trackedObject)
        {
            if (!IsOwner)
                return;

            Vector3 offset = CameraOffset;
            if (CameraOffset == Vector3.zero)
                offset = new(0, CameraDistance, -CameraDistance);

            offset.y += CameraDistance;
            Vector3 newPosition = Vector3.Lerp(transform.position, trackedObject.position + offset, TransitionSpeed);
            transform.position = newPosition;

            float newAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.x, CameraAngle, TransitionSpeed);
            transform.RotateAround(trackedObject.position, Vector3.right, newAngle - transform.rotation.eulerAngles.x);
            SmoothLookAtTarget(_trackedObject);
        }

        private void SmoothLookAtTarget(Transform target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, TransitionSpeed);
        }

        private void OnDisable() => _zoomAction.performed -= OnZoomPerformed;
    }
}
