using System.Collections.Generic;
using UnityEngine;

namespace Examen.BrotherEye
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CameraTypes _currentCameraType = CameraTypes.Player;
        [SerializeField] private CameraSettings[] _cameraSettings;
        private Dictionary<CameraTypes, int> _cameraSettingsIndex = new()
        {
            {CameraTypes.Player, 0},
            {CameraTypes.Ballistae, 1}
        };

        public CameraTypes CurrentCameraType => _currentCameraType;
        public CameraSettings CurrentCameraSettings => _cameraSettings[_cameraSettingsIndex[_currentCameraType]];

        private void OnEnable() => SetCameraType(_currentCameraType);

        public void SetCameraType(CameraTypes cameraType)
        {
            if (_cameraSettingsIndex.TryGetValue(cameraType, out int _))
                _currentCameraType = cameraType;
        }

        public CameraSettings GetNextCameraSettings()
        {
            int nextCamera = (int) _currentCameraType + 1;
            if (nextCamera > (int) CameraTypes.Ballistae)
                nextCamera = (int) CameraTypes.Player;
            _currentCameraType = (CameraTypes) nextCamera;
            return _cameraSettings[_cameraSettingsIndex[_currentCameraType]];
        }
    }
}
