using Examen.Items;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;

namespace Examen.Inventory
{
    public class InventorySystem : NetworkedSingletonInstance<InventorySystem>
    {
        private static Dictionary<ItemInstance, int> _currentItems = new();

        public Dictionary<ItemInstance, int> CurrentItems => _currentItems;

        public Action<InventoryPackage> OnItemsChanged;

        /// <summary>
        /// Overrides currentItems with the new given items.
        /// </summary>
        /// <param name="newItems">The new items that will overwrite the current items.</param>
        public void SetItems(Dictionary<ItemInstance, int> newItems, InventoryPackage package) 
        {
            _currentItems = newItems;
            OnItemsChanged?.Invoke(package);
        } 
    }
}