using FishNet.Object;
using UnityEngine;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDataHandler : NetworkBehaviour
    {
        private int _exp;

        private const string PLAYER_PREF_EXP_KEY = "PlayerExp";

        public int Exp
        {
            get
            {
                LoadExp();
                return _exp;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.LogError($"Started client {LocalConnection.ClientId}");
            PlayerDatabase.Instance.ConnectClient(LocalConnection.ClientId, this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Debug.LogError($"Stopped client {LocalConnection.ClientId}");
            PlayerDatabase.Instance.DisconnectClient(LocalConnection.ClientId);
        }

        public void AddExp(int exp)
        {
            Debug.LogError($"Add {exp} exp");

            LoadExp();
            _exp += exp;
            SaveExp();

            Debug.LogError($"Added {exp} exp");
        }

        private void SaveExp()
        {
            PlayerPrefs.SetInt(PLAYER_PREF_EXP_KEY, _exp);
            PlayerPrefs.Save();

            Debug.LogError($"saved {_exp}");
        }

        private void LoadExp()
        {
            _exp = PlayerPrefs.GetInt(PLAYER_PREF_EXP_KEY, 0);

            Debug.LogError($"Loaded {_exp} exp");
        }
    }
}