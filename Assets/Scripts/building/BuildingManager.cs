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
        // Get the current mouse position in the world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Ensure the object stays at its original z-position
        mousePosition.z = transform.position.z;

        // Move the object smoothly towards the mouse position
        transform.position = Vector3.Lerp(transform.position, mousePosition, followSpeed * Time.deltaTime);
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
