using Examen.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurepreviewButtons : MonoBehaviour
{
    [SerializeField] private BuildingManager _manager;
    private Camera _camera;
    private Canvas _canvas;

    // Start is called before the first frame update
    void Start()
    {
        _manager = FindObjectOfType<BuildingManager>();

        _camera = Camera.main;

        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = _camera;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform);
    }

    public void RotateStructure(bool nagativeDiraction)
    {
        int rotationAmount = 15 * (nagativeDiraction ? -1 : 1);
        _manager.RotateStructure(rotationAmount);
    }

    public void PlaceStructure()
    {
        _manager.SetStructure();
    }
}
