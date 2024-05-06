using System.Collections.Generic;
using Examen.Items;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform _content;

        private readonly Dictionary<Item, InventoryDisplayItem> _inventoryItems = new();

        private void OnEnable() 
        {
            InventorySystem.Instance.OnItemsChanged += UpdateDisplay;
            UpdateDisplay(InventorySystem.Instance.CurrentItems);
        }

        private void OnDisable() => InventorySystem.Instance.OnItemsChanged -= UpdateDisplay;

        private void UpdateDisplay(Dictionary<Item, int> items)
        {
            foreach (InventoryDisplayItem displayItem in _inventoryItems.Values)
                Destroy(displayItem.gameObject);

            _inventoryItems.Clear();

            foreach (Item item in items.Keys)
                UpdateDisplayItem(item, items[item]);
        }

        private void UpdateDisplayItem(Item item, int itemAmount)
        {
            if (_inventoryItems.ContainsKey(item))
            {
                _inventoryItems[item].UpdateItem(itemAmount);
                return;
            }
            
            InventoryDisplayItem displayItem = Instantiate(_displayItem, _content);
            displayItem.Initialize(item, itemAmount);
            _inventoryItems.Add(item, displayItem);
        }
    }
}