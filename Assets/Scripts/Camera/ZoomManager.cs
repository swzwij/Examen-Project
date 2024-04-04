using System.Collections.Generic;
using UnityEngine;

namespace Examen.BrotherEye
{
    public class ZoomManager : MonoBehaviour
    {
        private ZoomTypes _currentZoom = ZoomTypes.Normal;
        private Dictionary<ZoomTypes, float> _zoomSettings = new()
        {
            {ZoomTypes.Half, 0.5f},
            {ZoomTypes.Normal, 1f},
            {ZoomTypes.Double, 2f},
            {ZoomTypes.Triple, 3f},
            {ZoomTypes.Quintuple, 5f}
        };

        private void OnEnable() => SetZoom(_currentZoom);

        /// <summary>
        /// Sets the zoom level based on the provided <see cref="ZoomTypes"/>.
        /// </summary>
        /// <param name="zoomSettings">The zoom settings to apply.</param>
        public void SetZoom(ZoomTypes zoomSettings)
        {
            if (_zoomSettings.TryGetValue(zoomSettings, out float _))
                _currentZoom = zoomSettings;
        }

        /// <summary>
        /// Gets the current zoom value.
        /// </summary>
        /// <returns>The current zoom value.</returns>
        public float GetZoom() => _zoomSettings[_currentZoom];

        /// <summary>
        /// Gets the next zoom level.
        /// </summary>
        /// <returns>The next zoom level.</returns>
        public float GetNextZoom()
        {
            int nextZoom = (int) _currentZoom + 1;
            if (nextZoom > (int) ZoomTypes.Quintuple)
                nextZoom = (int) ZoomTypes.Half;
            _currentZoom = (ZoomTypes) nextZoom;
            return _zoomSettings[_currentZoom];
        }
    }
}