using Examen.Interactables.Resource;
using Examen.Networking;
using Examen.Pathfinding.Grid;
using Examen.Poolsystem;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    public class SpawnArea : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> _spawnedResources = new();
        [SerializeField] private List<Cell> _areaCells = new();

        public List<GameObject> SpawnedResources { set => _spawnedResources.AddRange(value); get => _spawnedResources; }
        public LineRenderer LineRenderer { set ; get ; }

        public List<Cell> AreaCells => _areaCells;

        private void OnEnable() => ServerInstance.Instance.OnServerStarted += InitSpawnedResources;

        private void Start() => AddUpdateAreaToGridCreated();

        /// <summary>
        /// Adds the function UpdateArea to the action invoke of OnGridCreated.
        /// </summary>
        public void AddUpdateAreaToGridCreated() => GridSystem.Instance.OnGridCreated += UpdateArea; 

        /// <summary>
        /// Checks the linerenderer points for cells and updates which cells it touches.
        /// </summary>
        public void UpdateArea()
        {
            _areaCells.Clear();

            if (LineRenderer == null)
                LineRenderer = GetComponent<LineRenderer>();

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

        /// <summary>
        /// Gets a random node postion from a random cell.
        /// </summary>
        /// <param name="currentCell"> the cell that the position is going to be in.</param>
        /// <returns>random position.</returns>
        public Vector3 GetRandomPosition(out Cell currentCell)
        {
            currentCell = _areaCells[Random.Range(0, _areaCells.Count)];
            Node randomNode = currentCell.ActiveNodes.ElementAt(Random.Range(0, currentCell.ActiveNodes.Count));

            return randomNode.Position;
        }

        /// <summary>
        /// Starts coroutine to delay the cell update
        /// </summary>
        /// <param name="cell">The cell you want to update</param>
        public void DelayCellUpdate(Cell cell) => StartCoroutine(DelayedCellUpdate(cell));

        private void InitSpawnedResources()
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _spawnedResources.Count; i++)
            {
                Resource resource = _spawnedResources[i].GetComponent<Resource>();
                string resourceName = resource.ResourceItem.Name;
                resource.SpawnArea = this;

                PoolSystem.Instance.AddActiveObject(resourceName, _spawnedResources[i]);
            }
        }


        private IEnumerator DelayedCellUpdate(Cell cell)
        {
            yield return new WaitForSeconds(0.1f);
            GridSystem.Instance.UpdateCell(cell.CellX, cell.CellY);
        }

        private void OnDestroy() => GridSystem.Instance.OnGridCreated -= UpdateArea;
    }
}
