using Examen.Inventory;
using Examen.Items;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class ServerInventory : NetworkBehaviour
{
    private Dictionary<string, InventorySystem> _inventorySystems = new();
    private ClientManager _clientManager = new();

    /// <summary>
    /// Adds given item amount to the inventory of the client.
    /// </summary>
    /// <param name="connection">The client you want to add to.</param>
    /// <param name="newItem">The item you want to add.</param>
    /// <param name="itemAmount">The amount of items you want to add</param>
    [Server]
    public void AddItem(NetworkConnection connection, Item newItem, int itemAmount)
    {
        string connectionName = connection.ToString();

        if (!_inventorySystems.ContainsKey(connectionName))
              _inventorySystems.Add(connectionName, new InventorySystem());

        _inventorySystems[connectionName].AddItem(newItem, itemAmount);

        UpdateClientInventory(connection, _inventorySystems[connectionName]);
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
        if (!_inventorySystems.ContainsKey(connection.ToString()))
            return;

        _inventorySystems[connection.ToString()].RemoveItem(newItem, itemAmount);

        UpdateClientInventory(connection, _inventorySystems[connection.ToString()]);
    }

    [ObserversRpc]
    private void UpdateClientInventory(NetworkConnection connection, InventorySystem newInventorySystem)
    {
        if (_clientManager.Connection != connection)
            return;

        InventorySystem.Instance.SetItems(newInventorySystem.CurrentItems);
    }
}
