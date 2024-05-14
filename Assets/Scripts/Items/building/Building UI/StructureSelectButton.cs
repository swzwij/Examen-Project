using Examen.Inventory;
using Examen.Items;
using FishNet.Object;
using System;
using System.Collections.Generic;
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
        [SerializeField] protected List<StructureCost> _structureCost;

        private Button _button;
        private BuildingManager _buildingManager;

        private void Awake()
        {
            _buildingManager = GetComponentInParent<BuildingManager>();
            _button = GetComponent<Button>();

            InventorySystem.Instance.OnItemsChanged += PlaceStructure;

            PlaceStructure(null);
        }

        private void PlaceStructure(Dictionary<string, int> _)
        {
            int _currentItems = 0;

            foreach (StructureCost item in _structureCost)
            {
                if (InventorySystem.Instance.CurrentItems.ContainsKey(item.ItemName) && InventorySystem.Instance.CurrentItems[item.ItemName] > item.Amount)
                    _currentItems++;
            }

            _button.interactable = _currentItems >= _structureCost.Count;
        }

        public void OnPointerDown(PointerEventData eventData)
            => _buildingManager.SpawnStructurePreview(_structurePreview, _structure, _structureCost);
    }

    [Serializable]
    public struct StructureCost
    {
        public Item Item;
        public int Amount;

        public readonly string ItemName => Item.Name;
    }
}
