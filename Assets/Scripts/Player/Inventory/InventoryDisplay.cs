using Examen.Items;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : NetworkBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform[] _contents;

        Dictionary<string, int> _currentItems = new();

        private void OnEnable() => InventorySystem.Instance.OnItemsChanged += UpdateDisplay;

        private void OnDestroy() => InventorySystem.Instance.OnItemsChanged -= UpdateDisplay;
        
        /// <summary>
        /// Updates the display with the current package.
        /// </summary>
        public void UpdateDisplay() => UpdateDisplay(_currentItems);
        
        private void UpdateDisplay(Dictionary<string, int> currentItems)
        {            
            ClearDisplay();

            _currentItems = currentItems;

            foreach (string itemName in currentItems.Keys)
                UpdateDisplayItem(itemName, currentItems[itemName]);
        }

        
        private void ClearDisplay()
        {
            if (!IsOwner)
                return;
            
            foreach (Transform content in _contents)
            {
                foreach (Transform child in content)
                    Destroy(child.gameObject);
            }
        }

        private void UpdateDisplayItem(string item, int itemAmount)
        {
            if (!IsOwner)
                return;

            foreach (Transform child in _contents)
            {
                InventoryDisplayItem displayItem = Instantiate(_displayItem, child);
                displayItem.Initialize(item, itemAmount);
            }
        }
    }
}