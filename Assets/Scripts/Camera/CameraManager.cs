using System.Collections.Generic;
using UnityEngine;

namespace Examen.BrotherEye
{
    public class CameraManager : MonoBehaviour
    {
        private CameraTypes _currentCamera = CameraTypes.Player;
        CameraSettings[] _cameraSettings;
        Dictionary<CameraTypes, int> _cameraSettingsIndex = new()
        {
            {CameraTypes.Player, 0},
            {CameraTypes.Ballistae, 1}
        };
    }
}
