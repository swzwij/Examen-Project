using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> allObjects;

    [SerializeField] private GameObject _buildingModel;
    [SerializeField] private Material _placeAllowed;
    [SerializeField] private Material _placeDisallowed;

    private List<Collider> _intersectingStructures = new();
    private bool _canPlace = false;

    private MeshRenderer _meshRenderer;
    private void Start() => _meshRenderer = _buildingModel.GetComponent<MeshRenderer>();

    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            SetStructurePosition(hit);
    }

    public void SpawnStructure()
    {

    }

    public void PlaceStructure(GameObject structurePrefab, Vector3 spawnLocation)
    {
        if (_canPlace)
            Instantiate(structurePrefab); //NEEDS TO BE DONE! (everything)
    }

    private void SetStructurePosition(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer != 3)
            return;

        Vector3 mousePosition = hit.point;

        transform.position = new Vector3(mousePosition.x, hit.point.y, mousePosition.z);
        transform.up = hit.normal;


        float dotProduct = Vector3.Dot(hit.normal, Vector3.up);
        CheckPlacable();

        if (dotProduct > 0.5 && _canPlace == true)
            _meshRenderer.material = _placeAllowed;
        else
            _meshRenderer.material = _placeDisallowed;
    }

    private void CheckPlacable()
    {
        _canPlace = true;

        foreach (var structure in allObjects)
        {
            float distanceX = Mathf.Abs(transform.position.x - structure.transform.position.x);
            float distanceZ = Mathf.Abs(transform.position.z - structure.transform.position.z);

            float halfSizeX = transform.localScale.x;
            float halfSizeZ = transform.localScale.z;

            bool withinX = distanceX <= halfSizeX;
            bool withinZ = distanceZ <= halfSizeZ;

            if (withinX && withinZ)
            {
                _canPlace = false;
                break;
            }
        }
    }
}
