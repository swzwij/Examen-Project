using System;
using UnityEngine;

namespace Examen.BrotherEye
{
    [Serializable]
    public struct CameraSettings
    {
        [SerializeField] private ZoomTypes _zoomLevel;
        [SerializeField] private float _angle;
        [SerializeField] private Transform _trackedObject;

        public readonly ZoomTypes ZoomLevel => _zoomLevel;
        public readonly float Angle => _angle;
        public readonly Transform TrackedObject => _trackedObject;
    }
}
