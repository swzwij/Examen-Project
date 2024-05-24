using System;
using FishNet.Managing.Server;
using MarkUlrich.Utils;

namespace Examen.Networking
{
    public class ServerInstance : SingletonInstance<ServerInstance>
    {
        private ServerManager _serverManager;
        private bool _isServerStarted;

        public Action OnServerStarted;

        private void Start() => TryGetComponent(out _serverManager);

        private void FixedUpdate()
        {
            if (!_serverManager.Started || _isServerStarted)
                return;
            
            InitServer();
        }

        private void InitServer()
        {
            _isServerStarted = true;
            OnServerStarted?.Invoke();
        }
    }
}
