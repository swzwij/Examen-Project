using FishNet;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDatabase : NetworkedSingletonInstance<PlayerDatabase>
    {
        [SerializeField] private float _requiredExp = 100f;
        [SerializeField] private float _expIncreaseFactor = 1.2f;

        [SerializeField] private Slider _expBar;
        [SerializeField] private Text _expText;

        private readonly Dictionary<int, PlayerDataHandler> _playerData = new();

        public Action<int> OnLevelChanged;

        /// <summary>
        /// Initiates a connection with a client using the specified client ID and player data handler.
        /// </summary>
        /// <param name="clientId">The ID of the client to connect to.</param>
        /// <param name="handler">The player data handler to use for the connection.</param>
        public void ConnectClient(int clientId, PlayerDataHandler handler) => SendClientConnection(clientId, handler);

        /// <summary>
        /// Disconnects the client with the specified client ID.
        /// </summary>
        /// <param name="clientId">The ID of the client to disconnect.</param>
        public void DisconnectClient(int clientId) => SendClientDisconnection(clientId);

        /// <summary>
        /// Updates the display based on the provided experience points.
        /// </summary>
        /// <param name="exp">The new experience points to display.</param>
        public void UpdateDisplay(int exp)
        {
            (int level, int remainingExp, int neededExp) = CalculateLevel(exp);

            int maxNeededExp = remainingExp + neededExp;

            _expBar.maxValue = maxNeededExp;
            _expBar.value = remainingExp;
            _expText.text = $"Level: {level} {remainingExp}/{maxNeededExp}";

            OnLevelChanged?.Invoke(level);
        }

        /// <summary>
        /// Adds experience points to the player associated with the specified network connection.
        /// </summary>
        /// <param name="connection">The network connection of the player.</param>
        /// <param name="exp">The amount of experience points to add.</param>
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
        
        /// <summary>
        /// Retrieves the experience points of the player associated with the specified network connection.
        /// </summary>
        /// <param name="connection">The network connection of the player.</param>
        /// <returns>The experience points of the player, or -1 if the player is not found.</returns>
        [Server]
        public int GetExp(NetworkConnection connection)
        {
            if (!_playerData.ContainsKey(connection.ClientId))
                return -1;

            return _playerData[connection.ClientId].Exp;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendClientConnection(int clientId, PlayerDataHandler handler) 
            => ProcessClientConnection(clientId, handler);

        [ServerRpc(RequireOwnership = false)]
        private void SendClientDisconnection(int clientId) => ProcessClientDisconnection(clientId);

        [Server]
        private void ProcessClientConnection(int clientId, PlayerDataHandler handler)
        {
            if (!_playerData.ContainsKey(clientId))
                _playerData.Add(clientId, handler);

            _playerData[clientId] = handler;
        }

        [Server]
        private void ProcessClientDisconnection(int clientId)
        {
            if (!_playerData.ContainsKey(clientId))
                return;

            _playerData.Remove(clientId);
        }

        private (int level, int remainingExp, int neededExp) CalculateLevel(int exp)
        {
            int level = 1;
            int remainingExp = exp;
            float requiredExp = _requiredExp;

            while (remainingExp >= requiredExp)
            {
                remainingExp -= (int)requiredExp;
                level++;
                requiredExp *= _expIncreaseFactor;
            }

            int neededExp = Mathf.CeilToInt(requiredExp - remainingExp);
            return (level, remainingExp, neededExp);
        }
    }
}