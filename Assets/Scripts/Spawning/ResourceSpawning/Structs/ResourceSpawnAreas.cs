using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnAreas
    {
        [SerializeField] private SpawnArea _area;
        [SerializeField] private int _amountOfResourcesInTheArea;
        [SerializeField] private List<ResourceSpawnInfo> _spawnableResources;

        public readonly SpawnArea Area { get { return _area; } }
        public readonly int AmountOfResourcesInTheArea { get { return _amountOfResourcesInTheArea; } }
        public readonly List<ResourceSpawnInfo> SpawnableResources { get { return _spawnableResources; } }
    }
}