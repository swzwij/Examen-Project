using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarkUlrich.Utils;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    public class GridSystem : NetworkedSingletonInstance<GridSystem>
    {
        [SerializeField] private bool _showGrid = true;

        [SerializeField] private float _maxWorldHeight = 30f;
        [SerializeField] private float _maxElevationDifference = 0.6f;
        [SerializeField] private float _maxConnectionDistance = 1f;
        [SerializeField] private float _nodeHeightOffset = 0.2f;

        [SerializeField] private LayerMask _walkableLayerMask;
        [SerializeField] private LayerMask _obstacleLayerMask;

        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private int _cellSize = 10;
        [SerializeField] private Vector2Int _gridSize;

        [SerializeField] private float _nodeDistance = 1f;
        [SerializeField] private int _maxNodeConnections = 3;

        [SerializeField] private List<Cell> _currentCells;
        
        private Node[,] _nodes;
        private Cell[,] _cells;
        private bool _isInitialized;

        public Action<Vector2Int> OnCellCreated;
        public Action OnGridCreated;

        private Vector3 CellSize 
            => new(_cellSize * _nodeDistance, _cellSize * _nodeDistance, _cellSize * _nodeDistance);

        public Cell[,] Cells => _cells;

        public List<Cell> CurrentCells => _currentCells; 

        private void FixedUpdate()
        {
            if (!IsServer || _isInitialized)
                return;
            
            _isInitialized = true;
            CreateGrid();
        }

        /// <summary>
        /// Creates the grid by initializing nodes, connecting them, and initializing cells.
        /// </summary>
        public void CreateGrid()
        {
            _nodes = new Node[_gridSize.x, _gridSize.y];
            InitializeGrid(_gridSize, InitializeNode);
            ConnectNodes();
            InitializeCells();
            ConnectCells();
            OnGridCreated?.Invoke();
        }

        private void InitializeGrid(Vector2Int gridSize, System.Action<int, int> initializeElement)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                    initializeElement(x, y);
            }
        }

        private void InitializeNode(int x, int y)
        {
            Vector3 nodePosition = CalculateNodePosition(x, y);
            Node newNode = new(nodePosition);

            newNode = ConfigureNode(newNode, nodePosition);
            _nodes[x, y] = newNode;
            newNode.GridPosition = new Vector2Int(x, y);
        }

        private Vector3 CalculateNodePosition(int x, int y) 
            => transform.position + new Vector3(x * _nodeDistance, 0, y * _nodeDistance);

        private Node ConfigureNode(Node node, Vector3 nodePosition)
        {
            if (!IsWalkableArea(nodePosition, out float elevation))
                return node;
            
            node.Elevation = elevation;
            node.Position = new Vector3(nodePosition.x, elevation, nodePosition.z);
            node.IsWalkable = true;
            node.MaxConnectionDistance = _maxConnectionDistance;
            node.MaxElevationDifference = _maxElevationDifference;
            node.NodeHeightOffset = _nodeHeightOffset;

            return node;
        }

        private void InitializeCells()
        {
            DestroyCells();

            int numCellsX = Mathf.CeilToInt(_gridSize.x / _cellSize);
            int numCellsY = Mathf.CeilToInt(_gridSize.y / _cellSize);

            _cells = new Cell[numCellsX, numCellsY];
            InitializeGrid(new Vector2Int(numCellsX, numCellsY), InitializeCell);
        }

        private void InitializeCell(int x, int y)
        {
            Cell newCell = Instantiate(_cellPrefab);
            _cells[x, y] = newCell;

            ConfigureCell(newCell, x, y);
            PopulateCellWithNodes(newCell, x, y);

            _currentCells.Add(newCell);
        }

        private void ConfigureCell(Cell cell, int x, int y)
        {
            cell.name = $"Cell {x}-{y}";
            cell.gameObject.layer = 2; // Ignore raycast
            cell.transform.SetParent(transform);
            cell.transform.position = GetCellPosition(x, y);
            cell.GridSystem = this;
            cell.CellX = x;
            cell.CellY = y;
        }

        private void PopulateCellWithNodes(Cell cell, int x, int y)
        {
            int startX = x * _cellSize;
            int startY = y * _cellSize;
            int endX = Mathf.Min(startX + _cellSize, _gridSize.x);
            int endY = Mathf.Min(startY + _cellSize, _gridSize.y);

            for (int nodeX = startX; nodeX < endX; nodeX++)
            {
                for (int nodeY = startY; nodeY < endY; nodeY++)
                {
                    Node node = _nodes[nodeX, nodeY];
                    cell.AddNode(node);

                    if (node.IsWalkable)
                        cell.ActiveNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// Checks if an area is walkable at the given position and returns the elevation of the area.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="elevation">The elevation of the walkable area.</param>
        /// <returns>True if the area is walkable, false otherwise.</returns>
        public bool IsWalkableArea(Vector3 position, out float elevation)
        {
            Ray ray = new(position + Vector3.up * _maxWorldHeight, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f, _obstacleLayerMask))
            {
                elevation = 0f;
                return false;
            }

            if (Physics.Raycast(ray, out hit, 200f, _walkableLayerMask))
            {
                elevation = hit.point.y + _nodeHeightOffset;
                return true;
            }

            elevation = 0f;
            return false;
        }


        /// <summary>
        /// Updates the specified cell in the grid system.
        /// </summary>
        /// <param name="cellX">The x-coordinate of the cell.</param>
        /// <param name="cellY">The y-coordinate of the cell.</param>
        public void UpdateCell(int cellX, int cellY) => StartCoroutine(UpdateCellDelayed(cellX, cellY));

        private IEnumerator UpdateCellDelayed(int cellX, int cellY)
        {
            yield return new WaitForEndOfFrame();

            Cell cell = _cells[cellX, cellY];

            foreach (Node node in cell.Nodes)
            {
                Vector3 position = node.Position;
                
                if (IsWalkableArea(position, out float elevation))
                {
                    node.IsWalkable = true;
                    node.Elevation = elevation;
                    node.Position = new Vector3(position.x, elevation, position.z);
                    node.MaxConnectionDistance = _maxConnectionDistance;
                    node.MaxElevationDifference = _maxElevationDifference;
                    cell.ActiveNodes.Add(node);
                    continue;
                }
                
                node.IsWalkable = false;
            }

            ConnectNodesInCell(cell);
        }

        private void ConnectNodes()
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                    CheckNodeConnections(x, y);
            }
        }

        private void ConnectNodesInCell(Cell cell)
        {
            for (int i = 0; i < cell.Nodes.Count; i++)
                CheckNodeConnections(cell.Nodes.ElementAt(i).GridPosition.x, cell.Nodes.ElementAt(i).GridPosition.y);
        }

        private void CheckNodeConnections(int x, int y)
        {
            Node currentNode = _nodes[x, y];

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y || i < 0 || i >= _gridSize.x || j < 0 || j >= _gridSize.y)
                        continue;

                    Node otherNode = _nodes[i, j];

                    if (otherNode == null)
                        continue;

                    currentNode.AddConnectedNode(otherNode);
                }
            }
        }

        private void ConnectCells()
        {
            foreach (Cell cell in _cells)
                ConnectCell(cell);
        }

        private void ConnectCell(Cell cell)
        {
            for (int i = cell.CellX - 1; i <= cell.CellX + 1; i++)
            {
                for (int j = cell.CellY - 1; j <= cell.CellY + 1; j++)
                {
                    if (i == cell.CellX && j == cell.CellY || i < 0 || i >= _cells.GetLength(0) || j < 0 || j >= _cells.GetLength(1))
                        continue;

                    Cell neighbour = _cells[i, j];
                    cell.Neighbours.Add(neighbour);
                }
            }
        }

        /// <summary>
        /// Clears the grid by resetting the nodes and cells, and destroying any existing cells.
        /// </summary>
        public void ClearGrid()
        {
            _nodes = null;
            _cells = null;
            DestroyCells();
        }

        private void DestroyCells()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            _currentCells.Clear();
        }

        /// <summary>
        /// Returns the node at the specified world position.
        /// </summary>
        /// <param name="worldPosition">The world position to check.</param>
        /// <returns>The node at the specified world position.</returns>
        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x - transform.position.x) / (_gridSize.x * _nodeDistance));
            float percentY = Mathf.Clamp01((worldPosition.z - transform.position.z) / (_gridSize.y * _nodeDistance));

            int x = Mathf.RoundToInt(_gridSize.x * percentX);
            int y = Mathf.RoundToInt(_gridSize.y * percentY);

            Node currentNode = _nodes[x, y];

            return currentNode;
        }

        /// <summary>
        /// Returns the closest walkable node to the specified position.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>The closest walkable node to the specified position.</returns>
        public Node GetClosestWalkableNode(Vector3 position)
        {
            Node closestNode = null;
            float closestDistance = float.MaxValue;

            foreach (Node node in _nodes)
            {
                if (!node.IsWalkable)
                    continue;

                float distance = Vector3.Distance(node.Position, position);
                if (distance < closestDistance)
                {
                    closestNode = node;
                    closestDistance = distance;
                }
            }

            return closestNode;
        }

        private Vector3 GetCellPosition(int xIndex, int yIndex)
        {
            // Calculate the bottom left position of the cell
            Vector3 cellBottomLeft = transform.position + new Vector3(xIndex * CellSize.x, 0, yIndex * CellSize.y);

            // Calculate the center position of the cell
            Vector3 cellCenter = cellBottomLeft + new Vector3(CellSize.x / 2, 0, CellSize.z / 2);
            return cellCenter;
        }

        /// <summary>
        /// Returns the cell at the specified world position.
        /// </summary>
        /// <param name="worldPosition">The world position to check.</param>
        /// <returns>The cell at the specified world position.</returns>
        public Cell GetCellFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x - transform.position.x) / (_gridSize.x * _nodeDistance));
            float percentY = Mathf.Clamp01((worldPosition.z - transform.position.z) / (_gridSize.y * _nodeDistance));

            int x = Mathf.RoundToInt((_gridSize.x / _cellSize) * percentX);
            int y = Mathf.RoundToInt((_gridSize.y / _cellSize) * percentY);

            Cell currentCell = _cells[x, y];

            return currentCell;
        }

        private void OnDrawGizmos()
        {
            if (!_showGrid)
                return;

            // draw a bounding box around the grid based on grid size
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube
            (
                transform.position + 
                new Vector3(_gridSize.x * _nodeDistance / 2f, _maxWorldHeight / 2, _gridSize.y * _nodeDistance / 2f), new Vector3(_gridSize.x * _nodeDistance, _maxWorldHeight, _gridSize.y * _nodeDistance)
            );

            if (_nodes == null || _nodes.Length == 0)
                return;
            
            foreach (Node node in _nodes)
            {
                if (!node.IsWalkable)
                    continue;

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(node.Position, 0.1f);

                Gizmos.color = Color.cyan;
                foreach (Node connectedNode in node.ConnectedNodes)
                {
                    if (!connectedNode.IsWalkable)
                        continue;

                    Gizmos.DrawLine(node.Position, connectedNode.Position);
                }
            }
        }
    }
}