using System.Collections;
using System.Linq;
using FishNet.Object;
using UnityEngine;

namespace Examen.Proximity
{
    public class TestingProximity : NetworkBehaviour
    {
        [SerializeField] private AgentTypes[] _agentTypesToCheck;
        [SerializeField] private float _range = 15f;
        [SerializeField] private ProximityAgent[] _nearbyAgents;
        private ProximityAgent _proximityAgent;

        private void Awake()
        {
            _proximityAgent = GetComponent<ProximityAgent>();
            StartCoroutine(CheckProximity(1f));
        }

        private IEnumerator CheckProximity(float interval)
        {
            while (true)
            {
                _nearbyAgents = _proximityAgent.RequestProximityData(_range, _agentTypesToCheck).ToArray();
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
