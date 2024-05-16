using System;
using System.Collections.Generic;
using FishNet.Broadcast;

namespace Examen.Items
{
    [Serializable]
    public struct InventoryPackage : IBroadcast
    {
        private Dictionary<ItemInstance, int> _items;
        
        public Dictionary<ItemInstance, int> Items => _items;

        /// <summary>
        /// Initializes the inventory package with the items.
        /// </summary>
        /// <param name="items">The InventoryPackages</param>
        public InventoryPackage(Dictionary<ItemInstance, int> items) => _items = items;
        
    }
}