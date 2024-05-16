using Examen.Structures;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;

namespace Examen.Inventory
{
    public class InventorySystem : NetworkedSingletonInstance<InventorySystem>
    {
        private static Dictionary<string, int> _currentItems = new();

        public Dictionary<string, int> CurrentItems => _currentItems;

        public Action<Dictionary<string, int>> OnItemsChanged;


        /// <summary>
        /// Overrides currentItems with the new given items.
        /// </summary>
        /// <param name="newItems">The new items that will overwrite the current items.</param>
        public void SetItems(Dictionary<string, int> newItems) 
        {
            _currentItems = newItems;
            OnItemsChanged?.Invoke(_currentItems);
        }

        /// <summary>
        /// Removes given items from given player.
        /// </summary>
        /// <param name="connection"> Connection of the player you want to remove from. </param>
        /// <param name="structureCost"> The item and amount you want to remove. /param>
        public void RemoveItems(NetworkConnection connection ,List<StructureCost> structureCost)
        {
            foreach (StructureCost item in structureCost)
                RemoveItem(connection, item.ItemName, item.Amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RemoveItem(NetworkConnection connection, string itemName, int amount) => ServerInventory.Instance.RemoveItem(connection, itemName, amount);
    }
}