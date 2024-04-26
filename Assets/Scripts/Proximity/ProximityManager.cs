using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Proximity
{
    public static class ProximityManager
    {
        private static readonly Dictionary<ProximityAgent, AgentTypes> _proximityAgents = new();
        private static readonly Dictionary<AgentTypes, List<ProximityAgent>> _proximityAgentsByType = new(); // Test later

        public static void Subscribe(this ProximityAgent agent, AgentTypes agentType)
        {
            if (_proximityAgents.TryGetValue(agent, out _))
                return;

            _proximityAgents.Add(agent, agentType);
        }

        public static void Unsubscribe(this ProximityAgent agent)
        {
            if (_proximityAgents.TryGetValue(agent, out _))
                _proximityAgents.Remove(agent);
        }

        public static HashSet<ProximityAgent> GetEntitesOfType(AgentTypes agentTypes)
        {
            HashSet<ProximityAgent> entities = new();

            foreach (var entity in _proximityAgents)
            {
                if (entity.Value == agentTypes)
                    entities.Add(entity.Key);
            }

            return entities;
        }

        public static HashSet<ProximityAgent> GetAgentsInRange(this ProximityAgent sourceAgent, float range, params AgentTypes[] agentTypes)
        {
            HashSet<ProximityAgent> allAgents = new();

            if (agentTypes.Length == 0)
                agentTypes = new[] {AgentTypes.PLAYER, AgentTypes.NPC, AgentTypes.STRUCTURE};

            foreach (AgentTypes agentType in agentTypes)
                allAgents.AddRange(GetEntitesOfType(agentType));

            if (allAgents.Count == 0)
                return allAgents;

            HashSet<ProximityAgent> agents = new();
            float rangeSquared = range * range;

            foreach (ProximityAgent agent in allAgents)
            {
                if (agent == sourceAgent)
                    continue;

                float distanceSquared = (agent.transform.position - sourceAgent.transform.position).sqrMagnitude;
                if (distanceSquared >= rangeSquared)
                    continue;

                agents.Add(agent);
            }

            return agents;
        }
    }

}
