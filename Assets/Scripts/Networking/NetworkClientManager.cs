using FishNet.Managing;
using UnityEngine;

namespace Examen.Networking
{
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkClientManager : MonoBehaviour
    {
        private NetworkManager _networkManager;
        private bool _isServer;

        public bool IsServer
        {
            get { return _isServer; }
            set 
            { 
                _isServer = value;

                if (_isServer)
                    InitializeServerClient();
                else
                    DeinitializeServerClient();
            }
        }

        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
        }

        private void Start()
        {
#if UNITY_SERVER
            InitializeServerClient();
#else
            InitializeUserClient();
#endif
        }

        public void ReconnectClient()
        {
            InitializeUserClient();
        }

        private void InitializeUserClient()
        {
            _networkManager.ClientManager.StartConnection();
        }

        private void DeinitializeUserClient()
        {
            _networkManager.ClientManager.StopConnection();
        }

        private void InitializeServerClient()
        {
            DeinitializeUserClient();
            _networkManager.ServerManager.StartConnection();

        }

        private void DeinitializeServerClient()
        {
            _networkManager.ServerManager.StopConnection(true);
            InitializeUserClient();
        }
    }
}