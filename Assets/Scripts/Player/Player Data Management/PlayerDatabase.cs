using FishNet.Connection;
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

        private readonly Dictionary<string, PlayerData> _playerData = new();

        private void Awake()
        {
            _expBar.value = 0;
            _expBar.maxValue = 100;
            _expText.text = "0/100";
        }

        [Server]
        public PlayerData Connect(NetworkConnection connection)
        {
            string clientAddress = connection.GetAddress();

            if (!_playerData.TryGetValue(clientAddress, out PlayerData existingPlayerData))
            {
                existingPlayerData = new PlayerData(0);
                _playerData.Add(clientAddress, existingPlayerData);
            }

            Debug.LogError($"Connected {clientAddress} with {existingPlayerData.Exp} exp");

            Display(existingPlayerData);
            return existingPlayerData;
        }

        [Server]
        public PlayerData AddExp(NetworkConnection connection, int exp)
        {
            string clientAddress = connection.GetAddress();

            if (!_playerData.TryGetValue(clientAddress, out PlayerData existingPlayerData))
            {
                existingPlayerData = new PlayerData(0);
                _playerData.Add(clientAddress, existingPlayerData);
                Debug.LogError($"Unable to find {clientAddress} in playerData");
            }

            Debug.LogError($"player exp {existingPlayerData.Exp} + added exp {exp} = ");

            existingPlayerData.AddExp(exp);

            Debug.LogError($"{existingPlayerData.Exp}");

            Display(existingPlayerData);

            return existingPlayerData;
        }

        [ObserversRpc]
        private void Display(PlayerData playerData)
        {
            Debug.LogError($"updated {playerData.Exp}");

            _expBar.value = playerData.Exp;
            _expText.text = $"{playerData.Exp}/100";
        }
    }
}