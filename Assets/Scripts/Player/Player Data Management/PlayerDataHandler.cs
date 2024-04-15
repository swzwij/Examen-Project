using FishNet.Object;
using Examen.Utils;
using UnityEngine;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDataHandler : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            Connect();
        }

        public bool a;

        private void Update()
        {
            if(a)
            {
                a = false;
                Debug.Log("add exp");
                AddExp(1);
            }
        }

        [ServerRpc]
        private void Connect()
        {
            Debug.LogError($"Connecting as {PlayerGUID.Get}");

            PlayerDatabase.Instance.Connect(PlayerGUID.Get);
        }

        public void AddExp(int exp)
        {
            PlayerDatabase.Instance.AddExp(PlayerGUID.Get, exp);
        }
    }
}