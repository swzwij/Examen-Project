using Examen.UI;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDataHandler : NetworkBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _text;
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

            PlayerDatabase.Instance.ExpBar = _slider;
            PlayerDatabase.Instance.ExpText = _text;
            PlayerDatabase.Instance.ConnectClient(LocalConnection.ClientId, this);

            LoadExp();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            PlayerDatabase.Instance.DisconnectClient(LocalConnection.ClientId);
            SaveExp();
        }

        public void AddExp(int exp)
        {
            LoadExp();
            _exp += exp;
            SaveExp();
        }

        private void SaveExp()
        {
            PlayerPrefs.SetInt(PLAYER_PREF_EXP_KEY, _exp);
            PlayerPrefs.Save();

            PlayerDatabase.Instance.UpdateDisplay(_exp);
        }

        private void LoadExp()
        {
            _exp = PlayerPrefs.GetInt(PLAYER_PREF_EXP_KEY, 0);

            PlayerDatabase.Instance.UpdateDisplay(_exp);
        }
    }
}