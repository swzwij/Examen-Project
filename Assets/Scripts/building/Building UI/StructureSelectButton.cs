using System.Collections;
using System.Collections.Generic;
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

        [System.Obsolete]
        public void OnPointerDown(PointerEventData eventData)
            => BuildingManager.Instance.SpawnStructurePreview(_structurePreview, _structure);
    }
}
