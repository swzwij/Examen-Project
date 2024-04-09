using System;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnInfo
    {
        [SerializeField] private GameObject _resource;
        [Range(0, 100)]
        [SerializeField] private float _chance;

        public readonly GameObject resource => _resource;
        public readonly float Chance => _chance;
    }
}