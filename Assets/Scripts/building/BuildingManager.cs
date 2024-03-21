using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject StartObject;

    [SerializeField] private GameObject _currentStructure;

    // Start is called before the first frame update
    void Start()
    {
        SetBuilding(StartObject);
    }

    // Adjust this speed to control how fast the object follows the mouse
    public float followSpeed = 5f;

    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
            GetDraggablePiece(hit);
    }
    private void GetDraggablePiece(RaycastHit hit)
    {
        Vector3 mousePosition = hit.point;

        transform.position = new Vector3(mousePosition.x, 0.01f, mousePosition.z);
    }

    private void SetBuilding(GameObject structure)
    {
        _currentStructure = structure;
        FollowMouse();
    }

    private void FollowMouse()
    {

    }
}
