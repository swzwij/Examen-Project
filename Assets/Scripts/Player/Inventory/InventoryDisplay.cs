using System.Collections.Generic;
using Examen.Items;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform _content;

        private readonly Dictionary<ItemInstance, InventoryDisplayItem> _inventoryItems = new();

        private void OnEnable() 
        {
            InventorySystem.Instance.OnItemsChanged += UpdateDisplay;
        }

        private void OnDisable() => InventorySystem.Instance.OnItemsChanged -= UpdateDisplay;

        private void UpdateDisplay(InventoryPackage package)
        {            
            foreach (InventoryDisplayItem displayItem in _inventoryItems.Values)
                Destroy(displayItem.gameObject);

            _inventoryItems.Clear();

            foreach (ItemInstance item in package.Items.Keys)
                UpdateDisplayItem(item, package.Items[item]);
        }

        private void UpdateDisplayItem(ItemInstance item, int itemAmount)
        {
            Debug.LogError($"UpdateDisplayItem {item.Name} {itemAmount}");
            InventoryDisplayItem displayItem = Instantiate(_displayItem, _content);
            displayItem.Initialize(item, itemAmount);
            _inventoryItems.Add(item, displayItem);
        }
    }
}