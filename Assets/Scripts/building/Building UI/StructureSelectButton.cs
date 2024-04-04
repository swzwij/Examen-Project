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
        [SerializeField] private BuildingManager _manager;
        [SerializeField] protected GameObject _structurePreview;
        [SerializeField] protected GameObject _structure;

        public void OnPointerDown(PointerEventData eventData)
        {
            _manager.SpawnStructurePreview(_structurePreview, _structure);
        }
    }
}
