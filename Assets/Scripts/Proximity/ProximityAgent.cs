using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;

namespace Examen.Proximity
{
    public class ProximityAgent : NetworkBehaviour
    {
        [SerializeField] private AgentTypes _agentType;
        [SerializeField] private float _defaultRange = 15f;

        [Header("Continueous Proximity Settings")]
        [SerializeField] private bool _checkProximity;
        [SerializeField] private AgentTypes[] _agentTypesToCheck;

        private HashSet<ProximityAgent> _nearbyAgents = new();
        
        public HashSet<ProximityAgent> NearbyAgents => RequestProximityData();

        private void Awake() => InitProximity();

        private void InitProximity() => ProximityManager.Subscribe(this, _agentType);

        private void FixedUpdate() 
        {
            if (!IsServer)
                return;

            if (!_checkProximity)
                return;

            GetProximityData(_defaultRange, _agentTypesToCheck);
        }

        public HashSet<ProximityAgent> RequestProximityData(float range = 0, params AgentTypes[] agentTypesToCheck)
        {
            if (range <= 0)
                range = _defaultRange;

            GetProximityData(range, agentTypesToCheck);
            return _nearbyAgents;
        }

        [Server]
        private void GetProximityData(float range = 0, params AgentTypes[] agentTypesToCheck)
        {
            if (range <= 0)
                range = _defaultRange;

            _nearbyAgents = this.GetAgentsInRange(range, agentTypesToCheck);
            BroadCastNearbyAgents(_nearbyAgents.ToArray());

#region Testing
            // foreach (ProximityAgent agent in _nearbyAgents)
            //     Debug.DrawLine(transform.position, agent.transform.position, Color.black);
#endregion
        }

        [ObserversRpc]
        private void BroadCastNearbyAgents(ProximityAgent[] agents)
        {
            if (!IsOwner)
                return;

            _nearbyAgents = agents.ToHashSet();
#region Testing
            // if (_nearbyAgents.Count == 0)
            //     return;

            // foreach (ProximityAgent agent in _nearbyAgents)
            //     Debug.LogError($"Agent: {agent.name} is nearby");
#endregion
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _defaultRange);
        }

        private void UnInitProximity() => ProximityManager.Unsubscribe(this);

        private void OnDestroy() => UnInitProximity();
    }
}