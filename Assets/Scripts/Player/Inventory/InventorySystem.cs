using Examen.Items;
using MarkUlrich.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Inventory
{
    public class InventorySystem : NetworkedSingletonInstance<InventorySystem>
    {
        private static Dictionary<Item, int> _currentItems = new();

        public Dictionary<Item, int> CurrentItems => _currentItems;

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
        /// 
        /// </summary>
        /// <param name="newItems"></param>
        public void SetItems(Dictionary<Item, int> newItems)
        {
            _currentItems = newItems;

            foreach (var item in _currentItems)
            {
                Debug.Log(item);
            }
        }
    }
}
