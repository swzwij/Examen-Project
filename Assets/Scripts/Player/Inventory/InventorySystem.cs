using Examen.Building.BuildingUI;
using Examen.Items;
using Examen.Networking;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using JetBrains.Annotations;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public void RemoveItems(NetworkConnection connection ,List<StructureCost> structureCost)
        {
            foreach (var item in structureCost)
                RemoveItem(connection, item.ItemName, item.Amount);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveItem(NetworkConnection connection, string itemName, int amount) => ServerInventory.Instance.RemoveItem(connection, itemName, amount);
    }
}