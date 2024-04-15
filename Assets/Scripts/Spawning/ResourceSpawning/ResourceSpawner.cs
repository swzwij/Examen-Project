using Examen.Interactables.Resource;
using Examen.Pathfinding.Grid;
using Examen.Spawning.ResourceSpawning.Structs;
using MarkUlrich.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class ResourceSpawner : SingletonInstance<ResourceSpawner>
    {
        [SerializeField] private Transform _areaParent;
        [SerializeField] private List<ResourceSpawnAreas> _spawnAreas = new();

        private float _spawnPercentage;
        private float _currentSpawnAmount;

        private const float MAX_PERCENTAGE = 100;

        private readonly List<GameObject> _spawnedGameobjects = new();

        public List<ResourceSpawnAreas> SpawnAreas => _spawnAreas;

        public void CreateSpawnAreas()
        {
            Transform[] resources = _areaParent.GetComponentsInChildren<Transform>();
            _spawnAreas.Clear();

            for (int i = 1; i < resources.Length; i++)
                DestroyImmediate(resources[i].gameObject);

            List<Cell> currentCells = GridSystem.Instance.CurrentCells;
            for (int i = 0; i < currentCells.Count; i++)
            {
                GameObject spawnArea =  new GameObject(currentCells[i].name);

                spawnArea.transform.parent = _areaParent;
                spawnArea.transform.position = currentCells[i].transform.position;

                _spawnAreas.Add(new() { Area = spawnArea.AddComponent<SpawnArea>() });
            }
          
        }

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

            for (int i = 0; i < area.SpawnableResources.Count; i++)
            {
                if (_spawnPercentage <= 0 || _currentSpawnAmount <= 0)
                {
                    Debug.LogError($"Can't spawn {area.SpawnableResources[i].Resource.name}, " +
                        $"because you can't spawn more then 100% Resources");
                    break;
                }

                float resourcesAmount = CalculateResrouceAmount(area, area.SpawnableResources[i]);

                SpawnResources(area, area.SpawnableResources[i].Resource, resourcesAmount);
            }
        }

        public void DestoryAllResources()
        {
            for (int i = 0; i < _spawnAreas.Count; i++)
                DestroyAreaResources(i);
            StopAllCoroutines();
        }

        public void DestroyAreaResources(int spawnAreaCount)
        {
            GameObject spawnArea = _spawnAreas[spawnAreaCount].Area.gameObject;

            Transform[] resources = spawnArea.GetComponentsInChildren<Transform>();
            _spawnAreas[spawnAreaCount].Area.SpawnedResources?.Clear();

            for (int i = 1; i < resources.Length; i++)
                DestroyImmediate(resources[i].gameObject);
        }


        public void SetResourcePosition(Resource resource)
        {
             resource.SetRandomPosition(out bool HasGottenSetPosition);

             if (HasGottenSetPosition)
                SetResourcePosition(resource);
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

/*        public void SpawnResourcesInQueue(List<Resource> resources)
        {
            StartCoroutine(resources);
        }

        private IEnumerator SpawnQueue(List<Resource> resources)
        {
            // wait for seconds or smth
            // spawn resource
        }*/

        private void SpawnResources(ResourceSpawnAreas spawnArea, GameObject gameObject, float amount)
        {
            _spawnedGameobjects.Clear();
            List<Resource> resourcesToSet = new();

            for (int i = 0; i < amount; i++)
            {
                GameObject newResource = Instantiate(gameObject, spawnArea.Area.transform);
                Resource resourceComponent = newResource.GetComponent<Resource>();

                resourceComponent.SetRandomPosition(out bool HasGottenSetPosition);

                if(!HasGottenSetPosition) 
                    resourcesToSet.Add(resourceComponent);

                _spawnedGameobjects.Add(newResource);
            }

            if(resourcesToSet.Count > 0)
                SetResourcesPosition(resourcesToSet);

            spawnArea.Area.SpawnedResources = _spawnedGameobjects;
        }

        private void SetResourcesPosition(List<Resource> resources) => StartCoroutine(WaitToSetPosition(resources));

        IEnumerator WaitToSetPosition(List<Resource> resources)
        {
            for (int i = 0; i < resources.Count; i++)
                yield return new WaitUntil(() => GetPositionBool(resources[i]));
        }

        private bool GetPositionBool(Resource resource)
        {
            resource.SetRandomPosition(out bool HasGottenSetPosition);
            return HasGottenSetPosition;
        }
    }
}
