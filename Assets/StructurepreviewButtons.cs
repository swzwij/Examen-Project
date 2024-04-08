using Examen.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurepreviewButtons : MonoBehaviour
{
    private Camera _camera;
    private Canvas _canvas;

    // Start is called before the first frame update
    void Start()
    {
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
        BuildingManager.Instance.RotateStructure(rotationAmount);
    }

    public void PlaceStructure()
    {
        BuildingManager.Instance.SetStructure();
    }
}
