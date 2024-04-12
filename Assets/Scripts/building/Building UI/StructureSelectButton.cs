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

        public void OnPointerDown(PointerEventData eventData)
            => FindAnyObjectByType<BuildingManager>().SpawnStructurePreview(_structurePreview, _structure);
    }
}
