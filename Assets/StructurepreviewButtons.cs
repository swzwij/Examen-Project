using Examen.Building;
using UnityEngine;

public class StructurepreviewButtons : MonoBehaviour
{
    private Camera _camera;
    private Canvas _canvas;

    void Start()
    {
        _camera = FindAnyObjectByType<Camera>();

        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = _camera;
    }

    void Update() => gameObject.transform.LookAt(_camera.gameObject.transform);

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
