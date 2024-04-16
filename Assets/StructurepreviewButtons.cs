using Examen.Building;
using UnityEngine;

public class StructurepreviewButtons : MonoBehaviour
{
    //private Camera _camera;
    //private Canvas _canvas;

    public BuildingManager OwnedBuildingManager { private get; set; }
    public UnityEngine.Camera Camera => OwnedBuildingManager.Camera;

    void Start()
    {
        /*_canvas = GetComponent<Canvas>();
        _canvas.worldCamera = _camera;*/
        //_camera = BuildingManager.Instance.Camera;
    }

    void Update()
    {
        gameObject.transform.LookAt(Camera.gameObject.transform);
    }

    public void RotateStructure(bool nagativeDiraction)
    {
        int rotationAmount = 15 * (nagativeDiraction ? -1 : 1);
        OwnedBuildingManager.RotateStructure(rotationAmount);
    }

    public void PlaceStructure() => OwnedBuildingManager.SetStructure();
}
