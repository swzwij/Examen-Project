using Examen.Inventory;
using Examen.Networking;
using Examen.Structures;
using FishNet.Managing;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Building.BuildingUI
{
    public class StructurePreviewButtons : MonoBehaviour
    {
        private NetworkManager _networkManager;
        private Canvas _canvas;
        private HashSet<Transform> _buttons = new();

        private bool _isCameraInitialised;

        public List<StructureCost> StructureCost { private get; set; }
        public BuildingManager OwnedBuildingManager { private get; set; }
        public UnityEngine.Camera Camera { get; set; }

        public HashSet<Transform> MyButtons => _buttons;

        private void Start() => _canvas = GetComponent<Canvas>();

        private void Awake()
        {
            ServerInstance.Instance.TryGetComponent(out _networkManager);

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                _buttons.Add(child);
            }

        }

        private void Update()
        {
            if (Camera == null)
                return;

            if (!_isCameraInitialised)
            {
                _isCameraInitialised = true;
                _canvas.worldCamera = Camera;
            }

            gameObject.transform.LookAt(Camera.gameObject.transform);
        }

        /// <summary>
        /// Sets the active state of all child buttons within the specified container.
        /// </summary>
        /// <param name="active">The desired active state for the buttons.</param>
        public void SetButtonsActive(bool active)
        {
            foreach (Transform child in _buttons)
                child.gameObject.SetActive(active);
        }

        /// <summary>
        /// Rotates the structure managed by the OwnedBuildingManager by a specified amount (15) in degrees.
        /// </summary>
        /// <param name="negativeDirection">Specifies if the rotation should be in the negative direction (counter-clockwise).</param>
        public void RotateStructure(bool negativeDirection)
        {
            int rotationAmount = 15 * (negativeDirection ? -1 : 1);
            OwnedBuildingManager.RotateStructure(rotationAmount);
        }

        /// <summary>
        /// Places the structure managed by the OwnedBuildingManager.
        /// </summary>
        public void PlaceStructure()
        {
            OwnedBuildingManager.SetStructure();
            InventorySystem.Instance.RemoveItems(_networkManager.ClientManager.Connection,StructureCost);
        }

    }
}