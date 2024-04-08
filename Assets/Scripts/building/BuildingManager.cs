using Examen.Player;
using MarkUlrich.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Building
{
    public class BuildingManager : SingletonInstance<BuildingManager>
    {
        [SerializeField] private Material _placeAllowed;
        [SerializeField] private Material _placeDisallowed;

        private GameObject _placePrefab;

        private GameObject _currentPreview;
        private MeshRenderer _meshRenderer;

        private Vector3 _currentPosition;
        private Vector3 _currentRotation;
        private Vector3 _currentNormal;

        private Vector3 _lastMousePosition;

        private bool _canPlace = false;
        private bool _isHolding = false;

        private GameObject rotationButtons;

        private Pointer _pointer;
        private Vector3 _pointerLocation;

        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _pointer.OnPointedAtPosition += SetPointerVector;
        }

        void Update()
        {
            if (!_isHolding || _currentPreview == null)
                return;
           
            Ray ray = Camera.main.ScreenPointToRay(_pointerLocation);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject.layer != 7)
                    return;

                if (hit.point == _lastMousePosition)
                    return;

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (rotationButtons != null)
                        rotationButtons.SetActive(false);

                    SetStructurePosition(hit);
                }
                else
                {
                    if(rotationButtons != null)
                        rotationButtons.SetActive(true);

                    return;
                }
            }
            _lastMousePosition = hit.point;
        }

        public GameObject GetCurrentStructure() => _currentPreview;

        public void SetStructure()
        {
            _currentRotation = _currentPreview.transform.rotation.eulerAngles;
            PlaceStructure(_placePrefab, _currentPosition, _currentRotation);
        }

        public void RotateStructure(int rotationAmount)
        {
            _currentPreview.transform.Rotate(Vector3.up, rotationAmount);
            _currentRotation = _currentPreview.transform.rotation.eulerAngles;
        }

        [System.Obsolete]
        public void SpawnStructurePreview(GameObject structurePreview, GameObject structure)
        {
            if (_currentPreview != null)
                Destroy(_currentPreview);

            _placePrefab = structure;

            _currentPreview = Instantiate(structurePreview);
            rotationButtons = _currentPreview.transform.FindChild("RotationButtons").gameObject;
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
        }

        private void SetStructurePosition(RaycastHit hit)
        {
            if (hit.collider.gameObject.layer != 7)
                return;

            Vector3 mousePosition = hit.point;

            _currentPosition = new Vector3(mousePosition.x, hit.point.y, mousePosition.z);
            _currentPreview.transform.position = _currentPosition;

            _currentNormal = hit.normal;
            _currentPreview.transform.up = _currentNormal;


            float dotProduct = Vector3.Dot(hit.normal, Vector3.up);
            CheckPlacable();

            if (dotProduct > 0.5 && _canPlace == true)
            {
                _meshRenderer.material = _placeAllowed;
            }
            else
            {
                _canPlace = false;
                _meshRenderer.material = _placeDisallowed;
            }
        }

        private void CheckPlacable()
        {
            print("Start Check");
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

        private void SetPointerVector(Vector3 pointerLocation) => _pointerLocation = pointerLocation;
    }

}