using System;
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
        public Action<HashSet<ProximityAgent>> OnProximityDataReceived;

        private void Awake() => InitProximity();

        private void InitProximity() => ProximityManager.Subscribe(this, _agentType);

        private void FixedUpdate() 
        {
            if (!IsServer)
                return;

            if (!_checkProximity)
                return;

            RequestProximityData(_defaultRange, _agentTypesToCheck);
        }

        /// <summary>
        /// Sets the agent type.
        /// </summary>
        /// <param name="agentType">The agent type to set.</param>
        public void SetAgentType(AgentTypes agentType)
        {
            _agentType = agentType;
            BroadcastAgentType(agentType);
        }

        [ObserversRpc]
        private void BroadcastAgentType(AgentTypes agentType) => _agentType = agentType;
        
        /// <summary>
        /// Requests proximity data for the specified range and agent types.
        /// </summary>
        /// <param name="range">The range within which to search for nearby agents. If set to 0 or less, the default range will be used.</param>
        /// <param name="agentTypesToCheck">The agent types to check for proximity.</param>
        /// <returns>A HashSet containing the nearby ProximityAgent instances.</returns>
        public HashSet<ProximityAgent> RequestProximityData(float range = 0, params AgentTypes[] agentTypesToCheck)
        {
            if (range <= 0)
                range = _defaultRange;

            GetProximityData(range, agentTypesToCheck);
            OnProximityDataReceived?.Invoke(_nearbyAgents);
            return _nearbyAgents;
        }

        /// <summary>
        /// Requests proximity data for the specified range and agent types.
        /// </summary>
        /// <param name="range">The range within which to search for nearby agents. If set to 0 or less, the default range will be used.</param>
        /// <param name="agentTypesToCheck">The agent types to check for proximity.</param>
        /// <returns>A HashSet containing the nearby ProximityAgent instances.</returns>
        public HashSet<ProximityAgent> RequestProximityData(params AgentTypes[] agentTypesToCheck)
        {
            RequestProximityData(_defaultRange, agentTypesToCheck);
            return _nearbyAgents;
        }

        [Server]
        private void GetProximityData(float range = 0, params AgentTypes[] agentTypesToCheck)
        {
            if (range <= 0)
                range = _defaultRange;

            _nearbyAgents = this.GetAgentsInRange(range, agentTypesToCheck);
            BroadCastNearbyAgents(_nearbyAgents.ToArray());
        }

        [ObserversRpc]
        private void BroadCastNearbyAgents(ProximityAgent[] agents)
        {
            if (!IsOwner)
                return;

            _nearbyAgents = agents.ToHashSet();
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