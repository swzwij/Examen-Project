using Examen.Building;
using System.Collections.Generic;
using UnityEngine;

public class StructurepreviewButtons : MonoBehaviour
{
    private Canvas _canvas;
    private HashSet<Transform> _myButtons = new();

    private bool _isCameraInitialised;

    public BuildingManager OwnedBuildingManager { private get; set; }
    public Camera Camera { get; set; }

    public HashSet<Transform> MyButtons => _myButtons;

    void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            _myButtons.Add(child);
        }
    }

    void Update()
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

    public void SetButtonsActive(bool active)
    {
        foreach (Transform child in _myButtons)
            child.gameObject.SetActive(active);
    }

    public void RotateStructure(bool nagativeDiraction)
    {
        int rotationAmount = 15 * (nagativeDiraction ? -1 : 1);
        OwnedBuildingManager.RotateStructure(rotationAmount);
    }

    public void PlaceStructure() => OwnedBuildingManager.SetStructure();
}
