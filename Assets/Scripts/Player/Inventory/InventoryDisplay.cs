using System.Collections.Generic;
using Examen.Items;
using FishNet;
using FishNet.Managing.Server;
using PlayFlow;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventoryDisplay : MonoBehaviour
    {
        public Item item;
        public Item items;
        [SerializeField] private InventoryDisplayItem _displayItem;
        [SerializeField] private Transform _content;

        private Dictionary<Item, InventoryDisplayItem> _inventoryItems = new();

        private void OnEnable()
        {
            InventorySystem.Instance.ItemAdded += UpdateDisplay;
        }

        private void UpdateDisplay(Item item, int itemAmount)
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

        public void Test()
        {
            ServerInventory.Instance.AddItem(InstanceFinder.ClientManager.Connection, item, 1);
        }

        public void Test2()
        {
            ServerInventory.Instance.AddItem(InstanceFinder.ClientManager.Connection, items, 1);
        }
    }
}