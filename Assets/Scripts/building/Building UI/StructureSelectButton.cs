using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Examen.Building.BuildingUI
{
    [RequireComponent(typeof(Button))]
    public class StructureSelectButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] protected GameObject _structurePreview;
        [SerializeField] protected NetworkObject _structure;

        private BuildingManager _buildingManager;

        private void Awake() => _buildingManager = GetComponentInParent<BuildingManager>();

        public void OnPointerDown(PointerEventData eventData)
            => _buildingManager.SpawnStructurePreview(_structurePreview, _structure);
    }
}
