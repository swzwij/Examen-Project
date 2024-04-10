using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnAreas
    {
        [SerializeField] private SpawnArea _area;
        [SerializeField] private int _resourceAmount;
        [SerializeField] private List<ResourceSpawnInfo> _spawnableResources;

        public readonly SpawnArea Area => _area;
        public readonly int ResourceAmount => _resourceAmount;
        public readonly List<ResourceSpawnInfo> SpawnableResources => _spawnableResources;
    }
}