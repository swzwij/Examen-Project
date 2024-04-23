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

        public Action<Item, int> ItemAdded;

        /// <summary>
        /// Add given item amount to the current item count.
        /// </summary>
        /// <param name="newItem"> The item you want to add.</param>
        /// <param name="amountOfItem"> Amount of the certain item you want to add.</param>
        public void AddItem(Item newItem, int amountOfItem)
        {
            if (!_currentItems.ContainsKey(newItem))
                _currentItems.Add(newItem, amountOfItem);
            else
                _currentItems[newItem] += amountOfItem;

            ItemAdded?.Invoke(newItem, _currentItems[newItem]);
        }

        /// <summary>
        /// Remove given item amount to the current item count.
        /// </summary>
        /// <param name="removeItem"> The item you want to remove.</param>
        /// <param name="amountOfItem"> Amount of the certain item you want to remove.</param>
        public void RemoveItem(Item removeItem, int itemAmount)
        {
            if (!_currentItems.ContainsKey(removeItem))
                return;
          
           _currentItems[removeItem] = _currentItems[removeItem] - itemAmount < 0 
                ?  0 
                : _currentItems[removeItem] - itemAmount;
        }

        /// <summary>
        /// Overrides currentItems with the new given items.
        /// </summary>
        /// <param name="newItems">The new items you want the current items to override with.</param>
        public void SetItems(Dictionary<Item, int> newItems) => _currentItems = newItems;
    }
}
