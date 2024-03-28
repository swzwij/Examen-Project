using Examen.Interactables.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class ResourceSpawner : MonoBehaviour
    {
        [SerializeField] private List<SpawnAreas> spawnAreas;

        public void InitializedSpawning()
        {
            for (int i = 0; i < spawnAreas.Count; i++)
            {
                SpawnAreas area = spawnAreas[i];
                float spawnPercentage = 100;
                float currentSpawnAmount = area.AmountOfResourcesInTheArea;

                for (int j = 0; j < area.spawnableResources.Count; j++)
                {
                    if (spawnPercentage <= 0 || currentSpawnAmount <= 0)
                    {
                        Debug.LogError($"Can't spawn {area.spawnableResources[j].SpawnResource.name}, because you can't spawn more then 100% Resources");
                        break;
                    }

                    float percentage = area.spawnableResources[j].spawnChance;

                    if (spawnPercentage - percentage < 0)
                    {
                        Debug.LogWarning($"Percentage of {area.spawnableResources[j].SpawnResource.name} was to high so we rounded it down to {spawnPercentage}");
                        percentage = spawnPercentage;
                        spawnPercentage = 0;
                    }
                    else
                        spawnPercentage -= percentage;

                    float amountOfrescoures = MathF.Round((float)area.AmountOfResourcesInTheArea / 100 * percentage);
                    float newSpawnAmount = currentSpawnAmount - amountOfrescoures;

                    if (newSpawnAmount < 0)
                    {
                        amountOfrescoures = currentSpawnAmount;
                        currentSpawnAmount = 0;
                    }
                    else
                        currentSpawnAmount = newSpawnAmount;

                    SpawnResources(area ,area.spawnableResources[j].SpawnResource, amountOfrescoures);
                }
            }
        }

        private void SpawnResources(SpawnAreas spawnArea, GameObject gameObject, float amount)
        {
            List<GameObject> spawnedGameobjects = new();

            for (int i = 0; i < amount; i++)
            {
                spawnedGameobjects.Add(Instantiate(gameObject, spawnArea.Area.transform));
            }

            spawnArea.Area.SpawnedResources = spawnedGameobjects;
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
