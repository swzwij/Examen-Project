using System;
using UnityEngine;

namespace Examen.Camera.BrotherEye
{
    [Serializable]
    public struct CameraSettings
    {
        [SerializeField] private float _distance;
        [SerializeField] private ZoomTypes _zoomLevel;
        [SerializeField] private float _angle;
        [SerializeField] private float _transitionSpeed;
        [SerializeField] private Vector3 _offset;

        public readonly float Distance => _distance;
        public readonly ZoomTypes ZoomLevel => _zoomLevel;
        public readonly float Angle => _angle;
        public readonly float TransitionSpeed => _transitionSpeed;
        public readonly Vector3 Offset => _offset;
    }
}
