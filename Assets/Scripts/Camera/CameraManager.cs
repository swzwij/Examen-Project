using System.Collections.Generic;
using UnityEngine;

namespace Examen.Camera.BrotherEye
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CameraTypes _currentCameraType = CameraTypes.Player;
        [SerializeField] private CameraTypeSettings[] _cameraSettings;

        private Dictionary<CameraTypes, CameraTypeSettings> _cameraTypeSettings = new();

        public CameraTypeSettings CurrentCameraTypeSettings => _cameraTypeSettings[_currentCameraType];
        public CameraSettings CurrentCameraSettings => _cameraTypeSettings[_currentCameraType].CameraSettings;

        private void OnEnable() => InitCameraSettings();
        private void InitCameraSettings()
        {
            foreach (CameraTypeSettings cameraTypeSetting in _cameraSettings)
            {
                if (_cameraTypeSettings.TryGetValue(cameraTypeSetting.CameraType, out CameraTypeSettings _) == false)
                    _cameraTypeSettings.Add(cameraTypeSetting.CameraType, cameraTypeSetting);
            }
        }

        public void SetCameraType(CameraTypes cameraType)
        {
            if (_cameraTypeSettings.TryGetValue(cameraType, out CameraTypeSettings _))
                _currentCameraType = cameraType;
        }

        public void GetNextCameraType()
        {
            int nextCameraType = (int) _currentCameraType + 1;
            if (nextCameraType > (int) CameraTypes.Ballistae)
                nextCameraType = (int) CameraTypes.Player;
            _currentCameraType = (CameraTypes) nextCameraType;
        }
    }
}
