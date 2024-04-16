using Examen.Pathfinding.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnAreas
    {
        [SerializeField] private ZoneID _zone;
        [SerializeField] private SpawnArea _area;
        [SerializeField] private int _resourceAmount;
        [SerializeField] private List<Cell> _cells;
        [SerializeField] private List<ResourceSpawnInfo> _spawnableResources;

        public SpawnArea Area { get => _area; set => _area = value; }
        public ZoneID Zone { get => _zone; set => _zone = value; }
        public int ResourceAmount { get => _resourceAmount; set => _resourceAmount = value; }
        public readonly List<Cell> Cells => _cells;
        public readonly List<ResourceSpawnInfo> SpawnableResources => _spawnableResources;
    }
}