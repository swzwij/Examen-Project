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

        public InventoryPackage(Dictionary<ItemInstance, int> items)
        {
            _items = items;
        }
    }
}