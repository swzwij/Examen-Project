using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Networking
{
    [RequireComponent(typeof(NetworkConfigurationManager))]
    public class NetworkInterfaceConsole : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _consoleToggle;
        [SerializeField] private Text _consoleCallback;
        [SerializeField] private GameObject _consoleWindow;

        private NetworkConfigurationManager _networkClientManager;

        private Dictionary<string, Action<string>> _commands;

        private void Awake()
        {
            _networkClientManager = GetComponent<NetworkConfigurationManager>();
            _consoleWindow.SetActive(false);
            InitCommands();
        }

        private void OnEnable()
        {
            _consoleToggle.onClick.AddListener(ToggleConsoleVisablity);
            _inputField.onSubmit.AddListener(HandleCommand);
        }

        private void OnDisable()
        {
            _consoleToggle.onClick.RemoveListener(ToggleConsoleVisablity);
            _inputField.onSubmit.RemoveListener(HandleCommand);
        }

        /// <summary>
        /// Sends a callback message to the console, coloring the text based on the provided log type.
        /// </summary>
        /// <param name="callback">The text of the callback message.</param>
        /// <param name="logType">The log type (e.g., Error, Warning) determining the message color.</param>
        public void SendCallback(string callback, LogType logType)
        {
            _consoleCallback.color = logType switch
            {
                LogType.Error => Color.red,
                LogType.Warning => Color.yellow,
                _ => Color.white,
            };

            _consoleCallback.text = callback;
        }

        private void InitCommands()
        {
            _commands = new()
            {
                { "server", (argument) =>
                    {
                        bool isServer = int.Parse(argument) != 0;
                        _networkClientManager.IsServer = isServer;
                        SendCallback("Converting to server.", LogType.Log);
                    }
                },
                { "connect", (argument) =>
                    {
                        _networkClientManager.ReconnectClient();
                        SendCallback("Started reconnecting.", LogType.Log);
                    }
                },
                { "connectip", (argument) =>
                    {
                        _networkClientManager.ReconnectClient(argument);
                        SendCallback($"connecting to {argument}.", LogType.Log);
                    }
                }
            };
        }

        private void ToggleConsoleVisablity() => _consoleWindow.SetActive(!_consoleWindow.activeSelf);

        private void HandleCommand(string command)
        {
            _inputField.text = string.Empty;

            string[] parts = command.Split(' ', 2);

            if (parts.Length != 2)
            {
                SendCallback("Invalid command format.", LogType.Error);
                return;
            }

            string commandName = parts[0];
            string argument = parts[1];

            if (!_commands.ContainsKey(commandName))
            {
                SendCallback("Unknown Command.", LogType.Error);
                return;
            }

            _commands[commandName](argument);
        }
    }
}