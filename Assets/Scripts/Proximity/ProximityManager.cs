
using System.Collections.Generic;
using MarkUlrich.Health;
using Unity.VisualScripting;

namespace Examen.Proximity
{
    public static class ProximityManager
    {
        private static readonly Dictionary<HealthData, AgentTypes> _proximityAgents = new(); // rename to agent
        private static readonly Dictionary<AgentTypes, List<HealthData>> _newProximityAgents = new(); // Test later

        public static void Subscribe(HealthData agent, AgentTypes agentType)
        {
            if (_proximityAgents.TryGetValue(agent, out _))
                return;

            _proximityAgents.Add(agent, agentType);
        }

        public static void Unsubscribe(HealthData agent)
        {
            if (_proximityAgents.TryGetValue(agent, out _))
                _proximityAgents.Remove(agent);
        }

        public static HashSet<HealthData> GetEntitesOfType(AgentTypes agentTypes)
        {
            HashSet<HealthData> entities = new();

            foreach (var entity in _proximityAgents)
            {
                if (entity.Value == agentTypes)
                    entities.Add(entity.Key);
            }

            return entities;
        }

        public static HashSet<HealthData> GetEntitiesInRange(this HealthData sourceAgent, float range, params AgentTypes[] agentTypes)
        {
            // TODO: Add default parameter which defaults to all entities
            HashSet<HealthData> entitites = new();

            if (agentTypes.Length == 0)
                agentTypes = new[] {AgentTypes.PLAYER, AgentTypes.NPC, AgentTypes.STRUCTURE};

            foreach (AgentTypes agentType in agentTypes)
                entitites.AddRange(GetEntitesOfType(agentType));

            foreach (HealthData agent in entitites)
            {
                float distance = (sourceAgent.transform.position - agent.transform.position).sqrMagnitude;
                if (distance >= range)
                    continue;
                
                entitites.Add(agent);
            }

            return entitites;
        }
    }

}
