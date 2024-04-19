using UnityEngine;
using Minoord.Input;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet;
using FishNet.Managing.Server;
using Examen.Pathfinding.Grid;

namespace Examen.Building
{
    [RequireComponent(typeof(Player.Pointer))]
    public class BuildingManager : NetworkBehaviour
    {
        public UnityEngine.Camera Camera => _camera;

        [SerializeField] private Material _placeAllowed;
        [SerializeField] private Material _placeDisallowed;

        private NetworkObject _placePrefab;
        private UnityEngine.Camera _camera;

        private GameObject _currentPreview;
        private MeshRenderer _meshRenderer;
        private StructurepreviewButtons rotationButtons;

        private Vector3 _currentPosition;
        private Vector3 _currentRotation;
        private Vector3 _currentNormal;

        private InputAction _clickAction;

        private GridSystem _gridSystem;

        private Player.Pointer _pointer;
        private Vector3 _pointerLocation;
        private RaycastHit _pointerHitInfo;

        private bool _canPlace = false;
        private bool _isHolding;

        private void Start()
        {
            InputManager.SubscribeToAction("HoldDown", OnHoldPressed, out _clickAction);
            _clickAction.canceled += OnReleasePressed;

            SetGridSystem();

            _pointer = GetComponent<Player.Pointer>();
            _camera = _pointer.Camera;
            _pointer.OnPointedUIInteraction += SetPointerVector;

            _pointer.OnPointedHitInfo += SetPointerHitInfo;
        }

        private void Update()
        {
            if (!_isHolding || _currentPreview == null)
                return;

            if (_pointer.HasClickedUI)
                _pointer.PointAtPosition();

            if (_pointerHitInfo.collider?.gameObject.layer != 7) //Ground layer
                return;

            if (_isHolding)
                SetStructurePosition();
        }

        /// <summary>
        /// Returns the current structure preview GameObject.
        /// </summary>
        /// <returns>The current structure preview GameObject.</returns>
        public GameObject GetCurrentStructure() => _currentPreview;

        /// <summary>
        /// Sets the structure position based on the current pointer location and hit information.
        /// </summary>
        public void SetStructure()
        {
            _currentRotation = _currentPreview.transform.rotation.eulerAngles;
            PlaceStructure(_placePrefab, _currentPosition, _currentRotation);
        }

        /// <summary>
        /// Rotates the structure preview by the specified amount.
        /// </summary>
        /// <param name="rotationAmount">The amount to rotate the structure preview.</param>
        public void RotateStructure(int rotationAmount)
        {
            _currentPreview.transform.Rotate(Vector3.up, rotationAmount);
            _currentRotation = _currentPreview.transform.rotation.eulerAngles;
        }

        /// <summary>
        /// Spawns a structure preview GameObject.
        /// </summary>
        /// <param name="structurePreview">The structure preview GameObject to spawn.</param>
        /// <param name="structure">The structure GameObject to place.</param>
        public void SpawnStructurePreview(GameObject structurePreview, NetworkObject structure)
        {
            if (_currentPreview != null)
                Destroy(_currentPreview);

            _placePrefab = structure;

            _currentPreview = Instantiate(structurePreview);
            _currentPreview.gameObject.SetActive(true);

            rotationButtons = _currentPreview.GetComponentInChildren<StructurepreviewButtons>();
            rotationButtons.OwnedBuildingManager = this;
            rotationButtons.Camera = Camera;
            rotationButtons.SetButtonsActive(false);

            _meshRenderer = _currentPreview.GetComponentInChildren<MeshRenderer>();

            _isHolding = true;
        }

        private void PlaceStructure(NetworkObject structurePrefab, Vector3 spawnLocation, Vector3 spawnRotation)
        {
            if (!_canPlace)
                return; 

            Destroy(_currentPreview);
            UpdateStructrePlacement(structurePrefab, spawnLocation, spawnRotation);
            UpdateCells(spawnLocation);
        }

        [ServerRpc]
        private void UpdateStructrePlacement(NetworkObject structurePrefab, Vector3 spawnLocation, Vector3 spawnRotation)
        {
            NetworkObject structure = InstanceFinder.NetworkManager.GetPooledInstantiated(structurePrefab, spawnLocation, Quaternion.Euler(spawnRotation), true);

            ServerManager serverManager = InstanceFinder.ServerManager;
            serverManager.Spawn(structure);
            InstanceFinder.NetworkManager.SceneManager.AddOwnerToDefaultScene(structure);

            StructureList.AddStructure(structure.GetComponent<Examen.Structure.BaseStructure>());
        }

        [ServerRpc]
        private void UpdateCells(Vector3 placedPosition)
        {
            Cell currentCell = _gridSystem.GetCellFromWorldPosition(placedPosition);
            currentCell.UpdateCell();
        }

        private void SetStructurePosition()
        {
            _currentPosition = _pointerLocation;
            _currentPreview.transform.position = _currentPosition;

            _currentNormal = _pointerHitInfo.normal;
            _currentPreview.transform.up = _currentNormal;

            float dotProduct = Vector3.Dot(_pointerHitInfo.normal, Vector3.up);
            CheckPlaceable();

            _meshRenderer.material = dotProduct > 0.5 && _canPlace ? _placeAllowed : _placeDisallowed;
            _canPlace = dotProduct > 0.5 && _canPlace;
        }

        private void CheckPlaceable()
        {
            _canPlace = true;

            foreach (var structure in StructureList.GetList())
            {
                float distanceX = Mathf.Abs(_currentPreview.transform.position.x - structure.transform.position.x);
                float distanceZ = Mathf.Abs(_currentPreview.transform.position.z - structure.transform.position.z);

                float halfSizeX = _currentPreview.transform.localScale.x;
                float halfSizeZ = _currentPreview.transform.localScale.z;

                bool withinX = distanceX <= halfSizeX;
                bool withinZ = distanceZ <= halfSizeZ;

                if (withinX && withinZ)
                {
                    _canPlace = false;
                    break;
                }
            }
        }

        private void OnHoldPressed(InputAction.CallbackContext context)
        {
            _isHolding = true;

            if (rotationButtons != null && !_isHolding)
                _currentPreview.GetComponentInChildren<StructurepreviewButtons>().SetButtonsActive(false);
        }

        private void OnReleasePressed(InputAction.CallbackContext context)
        {
            _pointer.HasClickedUI = false;
            _isHolding = false;

            if (rotationButtons != null)
                _currentPreview.GetComponentInChildren<StructurepreviewButtons>().SetButtonsActive(true);
        }

        [Server]
        private void SetGridSystem() => _gridSystem = FindAnyObjectByType<GridSystem>();

        private void SetPointerVector(Vector3 pointerLocation) => _pointerLocation = pointerLocation;

        private void SetPointerHitInfo(RaycastHit hitInfo) => _pointerHitInfo = hitInfo;
    }
}