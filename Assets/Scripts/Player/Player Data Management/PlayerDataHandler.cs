using FishNet;
using FishNet.Managing.Client;
using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace Examen.Player.PlayerDataManagement
{
    public class PlayerDataHandler : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            Debug.LogError("Start Player, connecting..");

            StartCoroutine(WaitForConnection());
        }

        private IEnumerator WaitForConnection()
        {
            if (IsServer)
                yield return null;

            yield return new WaitUntil(() => IsClient);

            Debug.LogError($"Waited for connection, got {LocalConnection}");
            Connect();
        }

        [ServerRpc]
        private void Connect()
        {
            Debug.LogError($"Connecting as {LocalConnection}");

            PlayerDatabase.Instance.Connect(LocalConnection);
        }
    }
}