using Examen.Spawning.ResourceSpawning.Structs;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class ResourceSpawner : SingletonInstance<ResourceSpawner>
    {
        [SerializeField] private List<ResourceSpawnAreas> _spawnAreas = new();

        private float _spawnPercentage;
        private float _currentSpawnAmount;

        private const float MAX_PERCENTAGE = 100;

        private readonly List<GameObject> _spawnedGameobjects = new();

        public List<ResourceSpawnAreas> SpawnAreas => _spawnAreas;

        /// <summary>
        /// Spawns randomly resources
        /// </summary>
        public void SpawnAllResources()
        {
            for (int i = 0; i < _spawnAreas.Count; i++)
                SpawnAreaResource(i);               
        }

        public void SpawnAreaResource(int spawnAreaCount)
        {
            DestroyAreaResources(spawnAreaCount);

            ResourceSpawnAreas area = _spawnAreas[spawnAreaCount];
            _spawnPercentage = MAX_PERCENTAGE;
            _currentSpawnAmount = area.ResourceAmount;

            for (int j = 0; j < area.SpawnableResources.Count; j++)
            {
                if (_spawnPercentage <= 0 || _currentSpawnAmount <= 0)
                {
                    Debug.LogError($"Can't spawn {area.SpawnableResources[j].Resource.name}, " +
                        $"because you can't spawn more then 100% Resources");
                    break;
                }

                float resourcesAmount = CalculateResrouceAmount(area, area.SpawnableResources[j]);

                SpawnResources(area, area.SpawnableResources[j].Resource, resourcesAmount);
            }
        }

        public void DestoryAllResources()
        {
            for (int i = 0; i < _spawnAreas.Count; i++)
            {
                DestroyAreaResources(i);
            }
        }

        public void DestroyAreaResources(int spawnAreaCount)
        {
            GameObject spawnArea = _spawnAreas[spawnAreaCount].Area.gameObject;

            Transform[] resources = spawnArea.GetComponentsInChildren<Transform>();
            _spawnAreas[spawnAreaCount].Area.SpawnedResources.Clear();

            for (int i = 1; i < resources.Length; i++)
                DestroyImmediate(resources[i].gameObject);
        }

        private float CalculateResrouceAmount(ResourceSpawnAreas area, ResourceSpawnInfo resourceSpawnInfo)
        {
            float percentage = resourceSpawnInfo.Chance;

            if (_spawnPercentage - percentage < 0)
            {
                Debug.LogWarning($"Percentage of {resourceSpawnInfo.Resource.name}" +
                    $" was to high so we rounded it down to {_spawnPercentage}");
                percentage = _spawnPercentage;
                _spawnPercentage = 0;
            }
            else
            {
                _spawnPercentage -= percentage;
            }

            float resourcesAmount = MathF.Round((float)area.ResourceAmount / 100 * percentage);
            float newSpawnAmount = _currentSpawnAmount - resourcesAmount;

            if (newSpawnAmount < 0)
            {
                resourcesAmount = _currentSpawnAmount;
                _currentSpawnAmount = 0;
            }
            else
            {
                _currentSpawnAmount = newSpawnAmount;
            }

            return resourcesAmount;
        }

        private void SpawnResources(ResourceSpawnAreas spawnArea, GameObject gameObject, float amount)
        {
            _spawnedGameobjects.Clear();

            for (int i = 0; i < amount; i++)
                _spawnedGameobjects.Add(Instantiate(gameObject, spawnArea.Area.transform));

            spawnArea.Area.SpawnedResources = _spawnedGameobjects;
        }

        private void SetResourceLocation()
        {

        }
    }
}
