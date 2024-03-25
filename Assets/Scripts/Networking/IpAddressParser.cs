using FishNet.Transporting.Tugboat;
using UnityEngine.UI;
using UnityEngine;

namespace Examen.Networking
{
    [RequireComponent(typeof(Tugboat))]
    public class IpAddressParser : MonoBehaviour
    {
        [SerializeField] private InputField _ipAddressInput;

        private Tugboat _tugboat;

        private void Awake() => _tugboat = GetComponent<Tugboat>();

        private void OnEnable()
        {
            _ipAddressInput.onEndEdit.AddListener(ParseIP);
            _ipAddressInput.onSubmit.AddListener(ParseIP);
        }

        private void OnDisable()
        {
            _ipAddressInput.onEndEdit.RemoveListener(ParseIP);
            _ipAddressInput.onSubmit.RemoveListener(ParseIP);
        }

        private void ParseIP(string input) => _tugboat.SetClientAddress(input);
    }
}