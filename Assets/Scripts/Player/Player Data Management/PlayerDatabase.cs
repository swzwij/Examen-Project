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
            Debug.LogError($"Connecting client {clientId}");
            SendClientConnection(clientId, handler);
        }

        public void DisconnectClient(int clientId)
        {
            Debug.LogError($"Disconnecting cleint {clientId}");
            SendClientDisconnection(clientId);
        }

        [ServerRpc]
        private void SendClientConnection(int clientId, PlayerDataHandler handler)
        {
            Debug.LogError($"Send Connecting client {clientId}");

            ProcessClientConnection(clientId, handler);
        }

        [ServerRpc]
        private void SendClientDisconnection(int clientId)
        {
            Debug.LogError($"Send Disconnecting cleint {clientId}");

            ProcessClienDisconnection(clientId);
        }

        [Server]
        private void ProcessClientConnection(int clientId, PlayerDataHandler handler)
        {
            Debug.LogError($"Processing {clientId} connection");

            if (!_playerData.ContainsKey(clientId))
                _playerData.Add(clientId, handler);

            _playerData[clientId] = handler;

            Debug.LogError($"Conncted {clientId}");
        }

        [Server]
        private void ProcessClienDisconnection(int clientId)
        {
            Debug.LogError($"Processing {clientId} disconnection");

            if (!_playerData.ContainsKey(clientId))
                return;

            _playerData.Remove(clientId);

            Debug.LogError($"Disconnected {clientId}");
        }
    }
}