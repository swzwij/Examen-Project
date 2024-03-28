using System;
using System.Collections.Generic;
using UnityEngine;

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
            Dictionary<GameObject,float> spawnTypes = new();

            for (int j = 0; j < area.spawnableResources.Count; j++)
            {
                if (spawnPercentage <= 0 || currentSpawnAmount <= 0)
                {
                    Debug.LogError($"Can't spawn {area.spawnableResources[j].resource.name}, because you can't spawn more then 100% Resources");
                    break;
                }

                float percentage = area.spawnableResources[j].spawnChance;

                if (spawnPercentage - percentage < 0)
                {
                    Debug.LogWarning($"Percentage of {area.spawnableResources[j].resource.name} was to high so we rounded it down to {spawnPercentage}");
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

                spawnTypes.Add(area.spawnableResources[j].resource, amountOfrescoures);
            }

            area.Area.SpawnResources(spawnTypes);
        }
    }

    [Serializable]
    private struct SpawnAreas
    {
        public SpawnArea Area;
        public int AmountOfResourcesInTheArea;
        public List<ResourceInfo> spawnableResources;
    }

    [Serializable]
    private struct ResourceInfo
    {
        public GameObject resource;
        [Range(0, 100)] public float spawnChance;
    }
}
