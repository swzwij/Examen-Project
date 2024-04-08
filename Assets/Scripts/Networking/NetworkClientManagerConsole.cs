using UnityEngine;
using UnityEngine.UI;

namespace Examen.Networking
{
    [RequireComponent(typeof(NetworkClientManager))]
    public class NetworkClientManagerConsole : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _consoleToggle;

        private NetworkClientManager _networkClientManager;

        private void Awake()
        {
            _networkClientManager = GetComponent<NetworkClientManager>();
            _inputField.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _consoleToggle.onClick.AddListener(ToggleConsoleVisablity);
            // Listen to inut field.
        }

        private void OnDisable()
        {
            _consoleToggle.onClick.RemoveListener(ToggleConsoleVisablity);
            
        }

        public void ToggleConsoleVisablity() => _inputField.gameObject.SetActive(!_inputField.gameObject.active);
        
        // Is server command
    }
}