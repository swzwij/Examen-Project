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

        public SpawnArea Area { get => _area; set => _area = value; }
        public readonly int ResourceAmount => _resourceAmount;
        public readonly List<ResourceSpawnInfo> SpawnableResources => _spawnableResources;
    }
}