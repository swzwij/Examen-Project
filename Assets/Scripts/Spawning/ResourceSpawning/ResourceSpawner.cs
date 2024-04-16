using Examen.Interactables.Resource;
using Examen.Pathfinding.Grid;
using Examen.Spawning.ResourceSpawning.Structs;
using MarkUlrich.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Cell> _cells;

        public List<ResourceSpawnAreas> SpawnAreas => _spawnAreas;

        public void CreateSpawnAreas()
        {
            Transform[] resources = _areaParent.GetComponentsInChildren<Transform>();
            _spawnAreas.Clear();

            for (int i = 1; i < resources.Length; i++)
                DestroyImmediate(resources[i].gameObject);

            ZoneID[] enumNames = (ZoneID[])Enum.GetValues(typeof(ZoneID));

            for (int i = 0; i < enumNames.Length; i++)
            {
                GameObject spawnArea = new(enumNames[i].ToString());

                _spawnAreas.Add(new() { Zone = enumNames[i], Area = spawnArea.AddComponent<SpawnArea>() });

                spawnArea.transform.parent = _areaParent;
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
            ResourceSpawnAreas area = _spawnAreas[spawnAreaCount];
            _spawnPercentage = MAX_PERCENTAGE;
            int currentActiveNodes = 0;

            DestroyAreaResources(spawnAreaCount);

            if (area.Zone == ZoneID.Border)
                area.ResourceAmount = area.Cells[0].AllNodes.Count * area.Cells.Count;

            _currentSpawnAmount = area.ResourceAmount;


            for (int i = 0; i < area.Cells.Count; i++)
                currentActiveNodes += area.Cells[i].ActiveNodes.Count;

            if (_currentSpawnAmount > currentActiveNodes)
                _currentSpawnAmount = currentActiveNodes;

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
        }

        public void DestroyAreaResources(int spawnAreaCount)
        {
            GameObject spawnArea = _spawnAreas[spawnAreaCount].Area.gameObject;

            Transform[] resources = spawnArea.GetComponentsInChildren<Transform>();
            _spawnAreas[spawnAreaCount].Area.SpawnedResources?.Clear();

            for (int i = 1; i < resources.Length; i++)
                DestroyImmediate(resources[i].gameObject);

            for (int i = 0; i < _spawnAreas[spawnAreaCount].Cells.Count; i++)
                GridSystem.Instance.UpdateCell(_spawnAreas[spawnAreaCount].Cells[i].CellX, _spawnAreas[spawnAreaCount].Cells[i].CellY);
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
            _cells.Clear();
            _cells.AddRange(spawnArea.Cells);

            for (int i = 0; i < amount; i++)
            {
                int randomNumber = UnityEngine.Random.Range(0, _cells.Count);

                GameObject newResource = Instantiate(gameObject, spawnArea.Area.transform);
                Resource resourceComponent = newResource.GetComponent<Resource>();

                if (_cells[randomNumber].ActiveNodes.Count <= 0)
                {
                    _cells.Remove(_cells[randomNumber]);
                     randomNumber = UnityEngine.Random.Range(0, _cells.Count);
                }

                resourceComponent.SetRandomPosition(_cells[randomNumber]);
                _spawnedGameobjects.Add(newResource);

                if (_cells[randomNumber].ActiveNodes.Count <= 0)
                    _cells.Remove(_cells[randomNumber]);
            }

            spawnArea.Area.SpawnedResources = _spawnedGameobjects;
        }
    }
}
