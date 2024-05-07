using System.Collections.Generic;
using Examen.Items;
using FishNet.Object;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : NetworkBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform _content;

        private readonly Dictionary<ItemInstance, InventoryDisplayItem> _inventoryItems = new();

        private void OnEnable() 
        {
            InventorySystem.Instance.OnItemsChanged += UpdateDisplay;
        }

        private void OnDisable() 
        {
            InventorySystem.Instance.OnItemsChanged -= UpdateDisplay;
        }

        private void UpdateDisplay(InventoryPackage package)
        {            
            ClearDisplay();

            foreach (ItemInstance item in package.Items.Keys)
                UpdateDisplayItem(item, package.Items[item]);
        }

        [ObserversRpc]
        private void ClearDisplay()
        {
            if (!IsOwner)
                return;
            
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            _inventoryItems.Clear();
        }

        [ObserversRpc]
        private void UpdateDisplayItem(ItemInstance item, int itemAmount)
        {
            if (!IsOwner)
                return;

            InventoryDisplayItem displayItem = Instantiate(_displayItem, _content);
            displayItem.Initialize(item, itemAmount);
            _inventoryItems.Add(item, displayItem);
        }
    }
}