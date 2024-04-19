using Examen.Interactables.Resource;
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
        //dont destroy objects when they are need
        // fix cell issue
        //make it work in game (aka fix cell issue)

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
        /// Spawns randomly resources
        /// </summary>
        public void SpawnAllResources()
        {
            for (int i = 0; i < _spawnAreas.Count; i++)
                SpawnAreaResource(i);
        }

        public void SpawnAreaResource(int spawnAreaCount)
        {
            if (GridSystem.Instance.Cells == null)
                GridSystem.Instance.CreateGrid();

            ResourceSpawnAreas area = _spawnAreas[spawnAreaCount];
            _spawnPercentage = MAX_PERCENTAGE;
            int currentActiveNodes = 0;

            if (area.Zone == ZoneID.Border)
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
            SpawnArea spawnArea = _spawnAreas[spawnAreaCount].Area;

            if (_spawnAreas[spawnAreaCount].Zone == ZoneID.Border)
                _hasSpawnedBorder = false;

            List<GameObject> resources = spawnArea.SpawnedResources;

            for (int i = 0; i < resources.Count; i++)
                DestroyImmediate(resources[i].gameObject);

            for (int i = 0; i < _spawnAreas[spawnAreaCount].Area.AreaCells.Count; i++)
                GridSystem.Instance.UpdateCell(_spawnAreas[spawnAreaCount].Area.AreaCells[i].CellX, _spawnAreas[spawnAreaCount].Area.AreaCells[i].CellY);

            spawnArea.SpawnedResources?.Clear();
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
            _cells?.Clear();
            _cells.AddRange(spawnArea.Area.AreaCells);

            for (int i = 0; i < amount; i++)
            {
                GameObject newResource = Instantiate(gameObject, spawnArea.Area.transform);
                SpawnResource(newResource);
            }

            spawnArea.Area.SpawnedResources = _spawnedGameobjects;
        }

        public void SpawnResource(GameObject newResource)
        {
            int randomNumber = UnityEngine.Random.Range(0, _cells.Count);

            if (_cells[randomNumber].ActiveNodes.Count <= 0)
            {
                _cells.Remove(_cells[randomNumber]);
                randomNumber = UnityEngine.Random.Range(0, _cells.Count);
            }

            newResource.transform.position = RandomisePosition(_cells[randomNumber]);

            _spawnedGameobjects.Add(newResource);

            if (_cells[randomNumber].ActiveNodes.Count <= 0)
                _cells.Remove(_cells[randomNumber]);
        }

        private Vector3 RandomisePosition(Cell cell)
        {
            StartCoroutine(WaitToUpdateCell(cell));

            int randomNumber = UnityEngine.Random.Range(0, cell.ActiveNodes.Count);
            Node randomNode = cell.ActiveNodes.ElementAt(randomNumber);

            cell.ActiveNodes.Remove(randomNode);
            return randomNode.Position;
        }
        private IEnumerator WaitToUpdateCell(Cell currentCell)
        {
            yield return new WaitForSeconds(0.1f);
            GridSystem.Instance.UpdateCell(currentCell.CellX, currentCell.CellY);
        }

    }
}
