using Examen.Items;
using System.Collections.Generic;

namespace Examen.Inventory
{
    public static class InventorySystem
    {
        private static Dictionary<Item, int> _currentItems = new();

        /// <summary>
        /// Add given item amount to the current item count.
        /// </summary>
        /// <param name="newItem"> The item you want to add.</param>
        /// <param name="amountOfItem"> Amount of the certain item you want to add.</param>
        public static void AddItem(Item newItem ,int amountOfItem)
        {
            if (!_currentItems.ContainsKey(newItem))
                _currentItems.Add(newItem, amountOfItem);
            else
                _currentItems[newItem] += amountOfItem;
        }

        /// <summary>
        /// Remove given item amount to the current item count.
        /// </summary>
        /// <param name="removeItem"> The item you want to remove.</param>
        /// <param name="amountOfItem"> Amount of item you want to remove.</param>
        public static void RemoveItem(Item removeItem, int amountOfItem)
        {
            if (!_currentItems.ContainsKey(removeItem))
                return;
          
           _currentItems[removeItem] = _currentItems[removeItem] - amountOfItem < 0 ?  0 : _currentItems[removeItem] - amountOfItem;
        }
    }
}
