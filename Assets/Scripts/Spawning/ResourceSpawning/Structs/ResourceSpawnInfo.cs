using System;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnInfo
    {
        [SerializeField] private GameObject _spawnResource;
        [Range(0, 100)]
        [SerializeField] private float _spawnChance;

        public readonly GameObject SpawnResource => _spawnResource;
        public readonly float SpawnChance => _spawnChance;
    }
}