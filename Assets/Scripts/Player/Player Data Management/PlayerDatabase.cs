using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using MarkUlrich.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDatabase : NetworkedSingletonInstance<PlayerDatabase>
    {
        [SerializeField] private Slider _expBar;
        [SerializeField] private Text _expText;

        private Dictionary<int, PlayerDataHandler> _playerData = new();

        public void ConnectClient(int clientId, PlayerDataHandler handler)
        {
            SendClientConnection(clientId, handler);
        }

        public void DisconnectClient(int clientId)
        {
            SendClientDisconnection(clientId);
        }

        public void UpdateDisplay(int exp)
        {
            _expBar.value = exp;
            _expText.text = $"{exp}/100";
        }

        [Server]
        public void AddExp(NetworkConnection connection, int exp)
        {
            if (!_playerData.ContainsKey(connection.ClientId))
                return;

            AddedExp(connection.ClientId, _playerData[connection.ClientId], exp);
        }

        [ObserversRpc]
        private void AddedExp(int clientId, PlayerDataHandler handler, int exp)
        {
            if (InstanceFinder.NetworkManager.ClientManager.Connection.ClientId != clientId)
                return;

            handler.AddExp(exp);
        }

        [Server]
        public int GetExp(NetworkConnection connection)
        {
            if (!_playerData.ContainsKey(connection.ClientId))
                return -1;

            return _playerData[connection.ClientId].Exp;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendClientConnection(int clientId, PlayerDataHandler handler)
        {
            ProcessClientConnection(clientId, handler);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendClientDisconnection(int clientId)
        {
            ProcessClienDisconnection(clientId);
        }

        [Server]
        private void ProcessClientConnection(int clientId, PlayerDataHandler handler)
        {
            if (!_playerData.ContainsKey(clientId))
                _playerData.Add(clientId, handler);

            _playerData[clientId] = handler;

            Debug.LogError($"Conncted {clientId}");
        }

        [Server]
        private void ProcessClienDisconnection(int clientId)
        {
            if (!_playerData.ContainsKey(clientId))
                return;

            _playerData.Remove(clientId);
        }
    }
}