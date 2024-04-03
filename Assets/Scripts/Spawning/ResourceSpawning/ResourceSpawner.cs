using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class ResourceSpawner : MonoBehaviour
    {
        [SerializeField] private List<SpawnAreas> spawnAreas;

        private float _spawnPercentage;
        private float currentSpawnAmount;
        private List<GameObject> _spawnedGameobjects = new();

        /// <summary>
        /// Spawns randomly resources
        /// </summary>
        public void InitializedSpawning()
        {
            for (int i = 0; i < spawnAreas.Count; i++)
            {
                SpawnAreas area = spawnAreas[i];
                _spawnPercentage = 100;
                currentSpawnAmount = area.AmountOfResourcesInTheArea;

                for (int j = 0; j < area.spawnableResources.Count; j++)
                {
                    if (_spawnPercentage <= 0 || currentSpawnAmount <= 0)
                    {
                        Debug.LogError($"Can't spawn {area.spawnableResources[j].SpawnResource.name}, " +
                            $"because you can't spawn more then 100% Resources");
                        break;
                    }

                    float amountOfResources = CalculateAmountofResources(area.spawnableResources[j],)

                    SpawnResources(area ,area.spawnableResources[j].SpawnResource, amountOfResources);
                }
            }
        }

        private float CalculateAmountofResources(SpawnAreas area, ResourceSpawnInfo resourceSpawnInfo)
        {
            float percentage = resourceSpawnInfo.spawnChance;

            if (_spawnPercentage - percentage < 0)
            {
                Debug.LogWarning($"Percentage of {resourceSpawnInfo.SpawnResource.name}" +
                    $" was to high so we rounded it down to {_spawnPercentage}");
                percentage = _spawnPercentage;
                _spawnPercentage = 0;
            }
            else
            {
                _spawnPercentage -= percentage;
            }

            float amountOfRescources = MathF.Round((float)area.AmountOfResourcesInTheArea / 100 * percentage);
            float newSpawnAmount = currentSpawnAmount - amountOfRescources;

            if (newSpawnAmount < 0)
            {
                amountOfRescources = currentSpawnAmount;
                currentSpawnAmount = 0;
            }
            else
            {
                currentSpawnAmount = newSpawnAmount;
            }

            return amountOfRescources;
        }

        private void SpawnResources(SpawnAreas spawnArea, GameObject gameObject, float amount)
        {
            _spawnedGameobjects.Clear();

            for (int i = 0; i < amount; i++)
                _spawnedGameobjects.Add(Instantiate(gameObject, spawnArea.Area.transform));

            spawnArea.Area.SpawnedResources = _spawnedGameobjects;
        }

        [Serializable]
        private struct SpawnAreas
        {
            public SpawnArea Area;
            public int AmountOfResourcesInTheArea;
            public List<ResourceSpawnInfo> spawnableResources;
        }

        [Serializable]
        private struct ResourceSpawnInfo
        {
            public GameObject SpawnResource;
            [Range(0, 100)] public float spawnChance;
        }
    }
}
