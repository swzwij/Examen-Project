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
            if (_cameraSettingsIndex.TryGetValue(cameraType, out int index))
            {
                _currentCameraType = cameraType;
                _currentCameraSettings = _cameraSettings[index];
            }
        }
    }
}
