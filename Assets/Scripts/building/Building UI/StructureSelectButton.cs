using Examen.Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Examen.Building
{
    [RequireComponent(typeof(Button))]
    public class StructureSelectButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] protected GameObject _structurePreview;
        [SerializeField] protected GameObject _structure;

        private BuildingManager _buildingManager;

        private void Awake()
        {
            _buildingManager = GetComponentInParent<BuildingManager>();
        }

        /*        public void OnPointerClick(PointerEventData eventData)
                    => */

        public void OnPointerDown(PointerEventData eventData)
        {
            _buildingManager.gameObject.GetComponent<PathFollower>().ManualBlock = true; // Todo: Fix performance later
            _buildingManager.SpawnStructurePreview(_structurePreview, _structure);
        }
    }
}
