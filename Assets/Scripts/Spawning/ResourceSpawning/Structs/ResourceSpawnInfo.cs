using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Examen.Spawning.ResourceSpawning.Structs
{
    [Serializable]
    public struct ResourceSpawnInfo
    {
        [SerializeField] private GameObject _spawnResource;
        [Range(0, 100)]
        [SerializeField] private float _spawnChance;

        public GameObject SpawnResource { get { return _spawnResource; } }
        public float SpawnChance { get { return _spawnChance; } }
    }
}
