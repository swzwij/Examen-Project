using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Proximity
{
    public static class ProximityManager
    {
        private static readonly Dictionary<ProximityAgent, AgentTypes> _proximityAgents = new();

        /// <summary>
        /// Subscribes a proximity agent to the proximity manager.
        /// </summary>
        /// <param name="agent">The proximity agent to subscribe.</param>
        /// <param name="agentType">The type of the proximity agent.</param>
        public static void Subscribe(this ProximityAgent agent, AgentTypes agentType)
        {
            if (_proximityAgents.TryGetValue(agent, out _))
                return;

            _proximityAgents.Add(agent, agentType);
        }

        /// <summary>
        /// Unsubscribes the specified proximity agent from the proximity manager.
        /// </summary>
        /// <param name="agent">The proximity agent to unsubscribe.</param>
        public static void Unsubscribe(this ProximityAgent agent)
        {
            if (_proximityAgents.TryGetValue(agent, out _))
                _proximityAgents.Remove(agent);
        }

        /// <summary>
        /// Retrieves all ProximityAgents of the specified AgentTypes.
        /// </summary>
        /// <param name="agentTypes">The AgentTypes to filter the ProximityAgents by.</param>
        /// <returns>A HashSet of ProximityAgents of the specified AgentTypes.</returns>
        public static HashSet<ProximityAgent> GetAgentsOfType(AgentTypes agentTypes)
        {
            HashSet<ProximityAgent> entities = new();

            foreach (var entity in _proximityAgents)
            {
                if (entity.Value == agentTypes)
                    entities.Add(entity.Key);
            }

            return entities;
        }

        /// <summary>
        /// Gets all ProximityAgents within a specified range from the source agent.
        /// </summary>
        /// <param name="sourceAgent">The source ProximityAgent.</param>
        /// <param name="range">The range within which to find ProximityAgents.</param>
        /// <param name="agentTypes">Optional parameter specifying the types of agents to consider. If not provided, all agent types will be considered.</param>
        /// <returns>A HashSet of ProximityAgents within the specified range.</returns>
        public static HashSet<ProximityAgent> GetAgentsInRange(this ProximityAgent sourceAgent, float range, params AgentTypes[] agentTypes)
        {
            HashSet<ProximityAgent> allAgents = new();

            if (agentTypes.Length == 0)
                agentTypes = new[] {AgentTypes.PLAYER, AgentTypes.NPC, AgentTypes.STRUCTURE};

            foreach (AgentTypes agentType in agentTypes)
                allAgents.AddRange(GetAgentsOfType(agentType));

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
