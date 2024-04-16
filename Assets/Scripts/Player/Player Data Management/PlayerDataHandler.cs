using FishNet.Object;
using UnityEngine;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDataHandler : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.LogError($"Started client {LocalConnection.ClientId}");
            FindObjectOfType<PlayerDatabase>().ConnectClient(LocalConnection.ClientId, this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Debug.LogError($"Stopped client {LocalConnection.ClientId}");
            FindObjectOfType<PlayerDatabase>().DisconnectClient(LocalConnection.ClientId);
        }
    }
}