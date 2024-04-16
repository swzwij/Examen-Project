using UnityEngine;
using Minoord.Input;
using UnityEngine.InputSystem;
using Examen.Pathfinding;

namespace Examen.Building
{
    [RequireComponent(typeof(Player.Pointer))]
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField] private Material _placeAllowed;
        [SerializeField] private Material _placeDisallowed;

        private GameObject _placePrefab;
        private UnityEngine.Camera _camera;

        private GameObject _currentPreview;
        private MeshRenderer _meshRenderer;

        private Vector3 _currentPosition;
        private Vector3 _currentRotation;
        private Vector3 _currentNormal;

        private bool _canPlace = false;
        private bool _isHolding;

        private GameObject rotationButtons;

        private InputAction _clickAction;

        private Player.Pointer _pointer;
        private Vector3 _pointerLocation;
        private RaycastHit _pointerHitInfo;

        private PathFollower _pathFollower;

        public UnityEngine.Camera Camera => _camera;

        private void Start()
        {
            InputManager.SubscribeToAction("HoldDown", OnHoldPressed, out _clickAction);
            _clickAction.canceled += OnReleasePressed;

            _pointer = GetComponent<Player.Pointer>();
            _camera = _pointer.Camera;
            _pointer.OnPointedUIInteraction += SetPointerVector;

            _pointer.OnPointedHitInfo += SetPointerHitInfo;

            _pathFollower = GetComponent<PathFollower>();
        }

        void Update()
        {
            if (!_isHolding || _currentPreview == null)
                return;

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
        public void SpawnStructurePreview(GameObject structurePreview, GameObject structure)
        {
            _pathFollower.ManualBlock = true;

            if (_currentPreview != null)
                Destroy(_currentPreview);

            _placePrefab = structure;

            _currentPreview = Instantiate(structurePreview);
            _currentPreview.GetComponent<StructurepreviewButtons>().OwnedBuildingManager = this;

            rotationButtons = _currentPreview.transform.Find("RotationButtons").gameObject;
            rotationButtons.SetActive(false);

            _meshRenderer = _currentPreview.GetComponentInChildren<MeshRenderer>();

            _isHolding = true;
        }

        private void PlaceStructure(GameObject structurePrefab, Vector3 spawnLocation, Vector3 spawnRotation)
        {
            if (!_canPlace)
                return;

            Destroy(_currentPreview);

            GameObject placedStructure = Instantiate(structurePrefab, spawnLocation, Quaternion.Euler(spawnRotation));
            StructureList.AddStructure(placedStructure);

            _pathFollower.ManualBlock = false;
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
            if (_pointerHitInfo.collider?.gameObject.layer == 5) return;

            _isHolding = true;

            if (rotationButtons != null)
                rotationButtons.SetActive(false);
        }

        private void OnReleasePressed(InputAction.CallbackContext context)
        {
            _pointer.HasClickedUI = false;
            if (_pointerHitInfo.collider?.gameObject.layer == 5) return;

            _isHolding = false;

            if (rotationButtons != null)
                rotationButtons.SetActive(true);
        }

        private void SetPointerVector(Vector3 pointerLocation) => _pointerLocation = pointerLocation;

        private void SetPointerHitInfo(RaycastHit hitInfo) => _pointerHitInfo = hitInfo;
    }
}