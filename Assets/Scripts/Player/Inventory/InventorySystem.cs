using Examen.Items;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;

namespace Examen.Inventory
{
    public class InventorySystem : NetworkedSingletonInstance<InventorySystem>
    {
        private static Dictionary<Item, int> _currentItems = new();

        public Dictionary<Item, int> CurrentItems => _currentItems;

        public Action<Dictionary<Item, int>> OnItemsChanged;

        /// <summary>
        /// Overrides currentItems with the new given items.
        /// </summary>
        /// <param name="newItems">The new items that will overwrite the current items.</param>
        public void SetItems(Dictionary<Item, int> newItems) 
        {
            _currentItems = newItems;
            OnItemsChanged?.Invoke(_currentItems);
        } 
    }
}