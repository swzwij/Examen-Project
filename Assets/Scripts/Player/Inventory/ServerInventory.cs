using Examen.Inventory;
using Examen.Items;
using Examen.Networking;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using MarkUlrich.Utils;
using System.Collections.Generic;
using UnityEngine;

public class ServerInventory : NetworkedSingletonInstance<ServerInventory>
{
    private Dictionary<int, Dictionary<ItemInstance, int>> _inventorySystems = new();
    private NetworkManager _networkManager;

    private void Start()
    {
        ServerInstance.Instance.TryGetComponent(out _networkManager);

        if (_networkManager == null)
            Debug.LogError("Couldn't find NetworkManager");
    }

    /// <summary>
    /// Adds given item amount to the inventory of the client.
    /// </summary>
    /// <param name="connection">The client you want to add to.</param>
    /// <param name="newItem">The item you want to add.</param>
    /// <param name="itemAmount">The amount of items you want to add</param>
    [Server]
    public void AddItem(NetworkConnection connection, Item newItem, int itemAmount)
    {
        if (!_inventorySystems.ContainsKey(connection.ClientId))
            _inventorySystems.Add(connection.ClientId, new());
        if (!_inventorySystems[connection.ClientId].ContainsKey(new(newItem.Name)))
            _inventorySystems[connection.ClientId].Add(new(newItem.Name), itemAmount);
        else
            _inventorySystems[connection.ClientId][new(newItem.Name)] += itemAmount;

        InventoryPackage package = new(_inventorySystems[connection.ClientId]);

        UpdateClientInventory(connection, package);
    }

    /// <summary>
    /// Removes given item amount from the inventory of the client.
    /// </summary>
    /// <param name="connection">The client you want the item removed from.</param>
    /// <param name="newItem">The item you want to remove.</param>
    /// <param name="itemAmount">The amount of items you want to remove</param>
    [Server]
    public void RemoveItem(NetworkConnection connection, Item newItem, int itemAmount)
    {
        if (!_inventorySystems.ContainsKey(connection.ClientId) || _inventorySystems[connection.ClientId][new(newItem.Name)] - itemAmount < 0)
            return;

        _inventorySystems[connection.ClientId][new(newItem.Name)] -= itemAmount;

        InventoryPackage package = new(_inventorySystems[connection.ClientId]);
        UpdateClientInventory(connection, package);
    }

    [ServerRpc(RunLocally = true)]
    private void UpdateClientInventory(NetworkConnection connection, InventoryPackage package)
    {
        // if (_networkManager.ClientManager.Connection.ClientId != connection.ClientId)
        //     return;

        InventorySystem.Instance.SetItems(package.Items, package);
    }
}
