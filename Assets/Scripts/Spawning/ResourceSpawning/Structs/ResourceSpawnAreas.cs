using Examen.Spawning.ResourceSpawning.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnAreas
    {
        [SerializeField] private NodePlacements _zone;
        [SerializeField] private SpawnArea _area;
        [SerializeField] private int _resourceAmount;
        [SerializeField] private List<ResourceSpawnInfo> _spawnableResources;

        public SpawnArea Area { get => _area; set => _area = value; }
        public NodePlacements Zone { get => _zone; set => _zone = value; }
        public int ResourceAmount { get => _resourceAmount; set => _resourceAmount = value; }
        public readonly List<ResourceSpawnInfo> SpawnableResources => _spawnableResources;
    }
}