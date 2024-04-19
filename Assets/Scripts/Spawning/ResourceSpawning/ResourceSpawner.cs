using Examen.Pathfinding.Grid;
using Examen.Spawning.ResourceSpawning.Structs;
using MarkUlrich.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class ResourceSpawner : SingletonInstance<ResourceSpawner>
    {
        [SerializeField] private Transform _areaParent;
        [SerializeField] private bool _hasSpawnedBorder;
        [SerializeField] private List<ResourceSpawnAreas> _spawnAreas = new();

        private float _spawnPercentage;
        private float _currentSpawnAmount;

        private const float MAX_PERCENTAGE = 100;

        private readonly List<GameObject> _spawnedGameobjects = new();
        private readonly List<Cell> _cells = new();

        public List<ResourceSpawnAreas> SpawnAreas => _spawnAreas;

        /// <summary>
        /// Spawns randomly resources.
        /// </summary>
        public void SpawnAllResources()
        {
            for (int i = 0; i < _spawnAreas.Count; i++)
                SpawnResourcesInArea(i);
        }

        /// <summary>
        /// Destroys old resources, checks if and how many resources can spawn in the area and spawns them.
        /// </summary>
        /// <param name="spawnAreaCount">The position index of the area you want the resource to spawn in.</param>
        public void SpawnResourcesInArea(int spawnAreaCount)
        {
            if (GridSystem.Instance.Cells == null)
                GridSystem.Instance.CreateGrid();

            ResourceSpawnAreas area = _spawnAreas[spawnAreaCount];
            _spawnPercentage = MAX_PERCENTAGE;
            int currentActiveNodes = 0;

            if (area.Zone == NodePlacements.Full)
            {
                if (_hasSpawnedBorder)
                    return;

                area.ResourceAmount = area.Area.AreaCells[0].AllNodes.Count * area.Area.AreaCells.Count;
                _hasSpawnedBorder = true;
            }
            else
                DestroyAreaResources(spawnAreaCount);

            _currentSpawnAmount = area.ResourceAmount;


            for (int i = 0; i < area.Area.AreaCells.Count; i++)
                currentActiveNodes += area.Area.AreaCells[i].ActiveNodes.Count;

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

                CreateResources(area, area.SpawnableResources[i].Resource, resourcesAmount);
            }
        }

        /// <summary>
        /// Destroys all resources in the scene.
        /// </summary>
        public void DestoryAllResources()
        {
            for (int i = 0; i < _spawnAreas.Count; i++)
                DestroyAreaResources(i);
        }

        /// <summary>
        /// Destroys Resource under the given spawnArea.
        /// </summary>
        /// <param name="spawnAreaCount">The position index of the area you want the resource to be destroyed.</param>
        public void DestroyAreaResources(int spawnAreaCount)
        {
            SpawnArea spawnArea = _spawnAreas[spawnAreaCount].Area;

            if (_spawnAreas[spawnAreaCount].Zone == NodePlacements.Full)
                _hasSpawnedBorder = false;

            List<GameObject> resources = spawnArea.SpawnedResources;

            for (int i = 0; i < resources.Count; i++)
                DestroyImmediate(resources[i].gameObject);

            for (int i = 0; i < _spawnAreas[spawnAreaCount].Area.AreaCells.Count; i++)
                GridSystem.Instance.UpdateCell(_spawnAreas[spawnAreaCount].Area.AreaCells[i].CellX, _spawnAreas[spawnAreaCount].Area.AreaCells[i].CellY);

            spawnArea.SpawnedResources?.Clear();
        }

        /// <summary>
        /// Sets the resource to a random spawn point.
        /// </summary>
        /// <param name="newResource"> the resource you want the position to be set.</param>
        /// <param name="area">The are the resource is in.</param>
        public void SetResourceSpawnPostion(GameObject newResource, SpawnArea area)
        {
            newResource.transform.position = area.GetRandomPosition(out Cell currentCell);

            area.DelayCellUpdate(currentCell);
            _spawnedGameobjects.Add(newResource);
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

        private void CreateResources(ResourceSpawnAreas spawnArea, GameObject gameObject, float amount)
        {
            _spawnedGameobjects.Clear();
            _cells?.Clear();
            _cells.AddRange(spawnArea.Area.AreaCells);

            for (int i = 0; i < amount; i++)
            {
                GameObject newResource = Instantiate(gameObject, spawnArea.Area.transform);
                SetResourceSpawnPostion(newResource, spawnArea.Area);
            }

            spawnArea.Area.SpawnedResources = _spawnedGameobjects;
        }
    }
}
