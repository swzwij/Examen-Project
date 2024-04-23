using System.Collections.Generic;
using Examen.Items;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform _content;

        private Dictionary<Item, InventoryDisplayItem> _inventoryItems = new();

        private void OnEnable() 
        {
            InventorySystem.Instance.ItemAdded += UpdateDisplayItem;
            UpdateDisplay();
        }

        private void OnDisable() => InventorySystem.Instance.ItemAdded -= UpdateDisplayItem;

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

        private void UpdateDisplay()
        {
            Dictionary<Item, int> items = InventorySystem.Instance.CurrentItems;

            foreach(Item item in items.Keys)
                UpdateDisplayItem(item, items[item]);
        }
    }
}