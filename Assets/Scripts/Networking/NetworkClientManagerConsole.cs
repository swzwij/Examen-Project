using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Networking
{
    [RequireComponent(typeof(NetworkClientManager))]
    public class NetworkClientManagerConsole : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _consoleToggle;
        [SerializeField] private Text _consoleCallback;
        [SerializeField] private GameObject _consoleWindow;

        private NetworkClientManager _networkClientManager;

        private Dictionary<string, Action<int>> _commands;


        private void Awake()
        {
            _networkClientManager = GetComponent<NetworkClientManager>();
            _consoleWindow.gameObject.SetActive(false);
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

        private void InitCommands()
        {
            _commands = new()
            {
                { "server", (argument) =>
                    {
                        bool isServer = argument != 0;
                        _networkClientManager.IsServer = isServer;
                        SendCallback("Command successfully parsed.", LogType.Log);
                    }
                }
            };
        }

        private void ToggleConsoleVisablity() => _consoleWindow.SetActive(!_consoleWindow.active);

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
            string argumentString = parts[1];

            if (!int.TryParse(argumentString, out int argument))
            {
                SendCallback("Invalid argument. Expected an integer.", LogType.Error);
                return;
            }

            if (!_commands.ContainsKey(commandName))
            {
                SendCallback("Unknown Command.", LogType.Error);
                return;
            }

            _commands[commandName](argument);
        }

        private void SendCallback(string callback, LogType logType)
        {
            _consoleCallback.color = logType switch
            {
                LogType.Error => Color.red,
                LogType.Warning => Color.yellow,
                _ => Color.white,
            };

            _consoleCallback.text = callback;
        }
    }
}