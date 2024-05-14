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
        private NetworkManager _networkManager;
        private static Dictionary<string, int> _currentItems = new();

        public Dictionary<string, int> CurrentItems => _currentItems;

        public Action<Dictionary<string, int>> OnItemsChanged;

        private void Start() => ServerInstance.Instance.TryGetComponent(out _networkManager);


        /// <summary>
        /// Overrides currentItems with the new given items.
        /// </summary>
        /// <param name="newItems">The new items that will overwrite the current items.</param>
        public void SetItems(Dictionary<string, int> newItems) 
        {
            _currentItems = newItems;
            OnItemsChanged?.Invoke(_currentItems);
        }


        public void RemoveItems(List<StructureCost> structureCost)
        { 
            print("AAHHH");

            foreach (var item in structureCost)
                ServerInventory.Instance.RemoveItem(_networkManager.ClientManager.Connection, item.Item, item.Amount);
        }
    }
}