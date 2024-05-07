using Examen.Items;
using FishNet.Object;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : NetworkBehaviour
    {
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform _content;

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
                UpdateDisplayItem(item.Name, package.Items[item]);
        }

        [ObserversRpc]
        private void ClearDisplay()
        {
            if (!IsOwner)
                return;
            
            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }

        [ObserversRpc]
        private void UpdateDisplayItem(string item, int itemAmount)
        {
            if (!IsOwner)
                return;

            InventoryDisplayItem displayItem = Instantiate(_displayItem, _content);
            displayItem.Initialize(item, itemAmount);
        }
    }
}