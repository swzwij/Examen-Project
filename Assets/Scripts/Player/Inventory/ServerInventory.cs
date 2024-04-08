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

    [Server]
    public void AddItem(NetworkConnection connection, Item newItem, int amountOfItem)
    {
        string connectionName = connection.ToString();

        if (!_inventorySystems.ContainsKey(connectionName))
              _inventorySystems.Add(connectionName, new InventorySystem());

        _inventorySystems[connectionName].AddItem(newItem, amountOfItem);

        UpdateClientInventory(connection, _inventorySystems[connectionName]);
    }

    [Server]
    public void RemoveItem(NetworkConnection connection, Item newItem, int amountOfItem)
    {
        if (!_inventorySystems.ContainsKey(connection.ToString()))
            return;

        _inventorySystems[connection.ToString()].RemoveItem(newItem, amountOfItem);

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
