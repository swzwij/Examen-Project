using System;
using UnityEngine;

namespace Examen.Camera.BrotherEye
{
    [Serializable]
    public struct CameraTypeSettings
    {
        public CameraTypeSettings(CameraTypes cameraType, CameraSettings cameraSettings)
        {
            _cameraType = cameraType;
            _cameraSettings = cameraSettings;
        }

        [SerializeField] private CameraTypes _cameraType;
        [SerializeField] private CameraSettings _cameraSettings;

        public readonly CameraTypes CameraType => _cameraType;
        public readonly CameraSettings CameraSettings => _cameraSettings;
    }
}
