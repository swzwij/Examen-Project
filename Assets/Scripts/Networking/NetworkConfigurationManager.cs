using FishNet.Transporting.Tugboat;
using FishNet.Managing;
using UnityEngine;

namespace Examen.Networking
{
    [RequireComponent(typeof(NetworkManager), typeof(Tugboat), typeof(NetworkInterfaceConsole))]
    public class NetworkConfigurationManager : MonoBehaviour
    {
        [SerializeField] private bool _isDevelopment;

        [Header("Production Settings")]
        [SerializeField] private string _productionAddress;
        [SerializeField] private ushort _productionPort;

        [Header("Development Settings")]
        [SerializeField] private string _developmentAddress;
        [SerializeField] private ushort _developmentPort;
        [Range(1, 1800)]
        [SerializeField] private float _timeout;

        private Tugboat _tugboat;
        private NetworkManager _networkManager;
        private NetworkInterfaceConsole _console;

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
            _tugboat = GetComponent<Tugboat>();
            _networkManager = GetComponent<NetworkManager>();
            _console = GetComponent<NetworkInterfaceConsole>();
        }

        private void Start()
        {
#if UNITY_SERVER
            InitializeServerClient();
#else
            InitializeUserClient();
#endif
        }

        /// <summary>
        /// Reconnect the client to the default or the given server address.
        /// </summary>
        /// <param name="address">The server address the client will connect to.</param>
        public void ReconnectClient(string address = "")
        {
            if (address != string.Empty)
                _tugboat.SetClientAddress(address);
            InitializeUserClient();
        }

        private void InitializeUserClient()
        {
            SetConnectionClientAddress();
            SetConnectionPort();
            SetTimeout();

            _networkManager.ClientManager.StartConnection();
        }

        private void DeinitializeUserClient() => _networkManager.ClientManager.StopConnection();

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

        private void SetConnectionClientAddress()
        {
            string clientAdress = _isDevelopment ? _developmentAddress : _productionAddress;

            if (string.IsNullOrEmpty(clientAdress))
            {
                CallbackError("Invalid client address.");
                return;
            }

            _tugboat.SetClientAddress(clientAdress);
        }

        private void SetConnectionPort()
        {
            ushort port = _isDevelopment ? _developmentPort : _productionPort;

            if (port == 0)
            {
                CallbackError("Invalid port.");
                return;
            }

            _tugboat.SetPort(port);
        }

        private void SetTimeout()
        {
            if (!_isDevelopment)
                return;

            _tugboat.SetTimeout(_timeout, false);
        }

        private void CallbackError(string message) 
        {
            Debug.LogError(message);
            _console.SendCallback(message, LogType.Error);
        }
    }
}