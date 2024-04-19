using Examen.Interactables.Resource;
using Examen.Pathfinding.Grid;
using Examen.Poolsystem;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class SpawnArea : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> _spawnedResources = new();
        [SerializeField] private List<Cell> _areaCells = new();

        public List<GameObject> SpawnedResources { set => _spawnedResources.AddRange(value); get => _spawnedResources; }
        public LineRenderer LineRenderer { get; set; }

        public List<Cell> AreaCells => _areaCells;

        private void OnEnable()
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _spawnedResources.Count; i++)
            {
               string resourceName = _spawnedResources[i].GetComponent<Resource>().ResourceItem.Name;

                PoolSystem.Instance.AddActiveObject(resourceName, _spawnedResources[i]);
            }
        }

        public void AddAction() => GridSystem.Instance.OnGridCreated += UpdateArea; 

        public void UpdateArea()
        {
            _areaCells.Clear();

            for (int i = 0; i < LineRenderer.positionCount; i++)
            {
                Vector3 position = LineRenderer.GetPosition(i);
                if (GridSystem.Instance.Cells == null)
                    GridSystem.Instance.CreateGrid();

                Cell cell = GridSystem.Instance.GetCellFromWorldPosition(position);

                if (_areaCells.Contains(cell))
                    continue;

                _areaCells.Add(cell);
            }
        }
    }
}
