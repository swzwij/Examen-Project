using System;
using System.Collections.Generic;

namespace Examen.Items
{
    [Serializable]
    public class InventoryPackage
    {
        private Dictionary<ItemInstance, int> _items;
        
        public Dictionary<ItemInstance, int> Items => _items;

        public InventoryPackage()
        {
            _items = new Dictionary<ItemInstance, int>();
        }

        // Custom constructor
        public InventoryPackage(Dictionary<ItemInstance, int> items)
        {
            _items = items;
        }
    }
}