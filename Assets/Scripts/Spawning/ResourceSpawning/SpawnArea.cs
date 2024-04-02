using Examen.Interactables.Resource;
using Examen.Poolsystem;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class SpawnArea : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> _spawnedResources;

        public List<GameObject> SpawnedResources { set => _spawnedResources.AddRange(value); }

        private void Start()
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _spawnedResources.Count; i++)
            {
               string resourceName = _spawnedResources[i].GetComponent<Resource>().ResourceItem.Name;

                PoolSystem.Instance.AddActiveObject(resourceName, _spawnedResources[i]);
            }
        }
    }
}
