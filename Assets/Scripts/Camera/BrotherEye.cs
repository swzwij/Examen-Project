using System;
using FishNet.Object;
using Minoord.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Examen.BrotherEye
{
    [RequireComponent(typeof(ZoomManager), typeof(CameraManager))]
    public class BrotherEye : NetworkBehaviour
    {
        //[SerializeField] private float _cameraDistance = 10f;
        //[SerializeField] private float _cameraAngle = 45f;
        //[SerializeField] private float _transitionSpeed = 0.05f;
        
        private InputAction _zoomAction;
        private CameraManager _cameraManager;
        private ZoomManager _zoomManager;
        private Transform _trackedObject;
        private Transform _player;

        //public float ZoomModifier => _zoomManager.GetZoom();
        //public float CameraDistance => _cameraDistance * ZoomModifier;
        public float CameraDistance => _cameraManager.CurrentCameraSettings.Distance * _zoomManager.ZoomSettings[_cameraManager.CurrentCameraSettings.ZoomLevel];
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

        private void OnZoomPerformed(InputAction.CallbackContext context) => _cameraManager.GetNextCameraSettings();
        
        private void TrackObject(Transform trackedObject)
        {
            if (!IsOwner)
                return;

            Vector3 offset = CameraOffset;
            if (CameraOffset == Vector3.zero)
                offset = new(0, CameraDistance, -CameraDistance);

            Vector3 newPosition = Vector3.Lerp(transform.position, trackedObject.position + offset, TransitionSpeed);
            transform.position = newPosition;

            float newAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.x, CameraAngle, TransitionSpeed);
            transform.RotateAround(trackedObject.position, Vector3.right, newAngle - transform.rotation.eulerAngles.x);

            //transform.LookAt(_trackedObject, Vector3.up);
            SmoothLookAtTarget(_trackedObject);
        }

        private void SmoothLookAtTarget(Transform target)
        {
            // Determine the target rotation. This is the rotation where the object is looking directly at the target.
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

            // Smoothly interpolate between the object's current rotation and the target rotation at smoothingSpeed.
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, TransitionSpeed);
        }

        private void OnDisable() => _zoomAction.performed -= OnZoomPerformed;
    }
}
