using FishNet.Transporting.Tugboat;
using FishNet.Managing;
using UnityEngine;

namespace Examen.Networking
{
    [RequireComponent(typeof(NetworkManager), typeof(Tugboat))]
    public class NetworkConfigurationManager : MonoBehaviour
    {
        [SerializeField] private bool _isDevelopment;

        [Header("Production Settings")]
        [SerializeField] private string _productionAddress;
        [SerializeField] private ushort _productionPort;

        [Header("Development Settings")]
        [SerializeField] private string _developmentAddress;
        [SerializeField] private ushort _developmentPort;

        private Tugboat _tugboat;
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
            _tugboat = GetComponent<Tugboat>();
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
            string clientAdress = _isDevelopment ? _developmentAddress : _productionAddress;
            _tugboat.SetClientAddress(clientAdress);

            ushort port = _isDevelopment ? _developmentPort : _productionPort;
            _tugboat.SetPort(port);

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