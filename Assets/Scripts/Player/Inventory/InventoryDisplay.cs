using Examen.Items;
using FishNet.Object;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : NetworkBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform[] _contents;

        private InventoryPackage _currentPackage;

        private void OnEnable() => InventorySystem.Instance.OnItemsChanged += UpdateDisplay;

        private void OnDisable() => InventorySystem.Instance.OnItemsChanged -= UpdateDisplay;
        

        /// <summary>
        /// Updates the display with the current package.
        /// </summary>
        public void UpdateDisplay() => UpdateDisplay(_currentPackage);
        
        private void UpdateDisplay(InventoryPackage package)
        {            
            ClearDisplay();

            _currentPackage = package;

            foreach (ItemInstance item in package.Items.Keys)
                UpdateDisplayItem(item.Name, package.Items[item]);
        }

        [ObserversRpc]
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

        [ObserversRpc]
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