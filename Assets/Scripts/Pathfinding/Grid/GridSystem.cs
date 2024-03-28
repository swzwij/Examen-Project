using System.Collections.Generic;
using System.Linq;
using GameKit.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    public class GridSystem : MonoBehaviour
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
        [SerializeField] private bool _useRefinedConnections = true;
        [SerializeField] Vector2Int _gridSize;
        [SerializeField] private float _nodeDistance = 1f;
        [SerializeField] private int _maxNodeConnections = 3;
        private Node[,] _nodes;
        private Cell[,] _cells;

        private Vector3 CellSize 
            => new(_cellSize * _nodeDistance, _cellSize * _nodeDistance, _cellSize * _nodeDistance);

        private void OnEnable() => CreateGrid();

        public void CreateGrid()
        {
            _nodes = new Node[_gridSize.x, _gridSize.y];
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector3 nodePosition = transform.position + new Vector3(x * _nodeDistance, 0, y * _nodeDistance);
                    Node newNode = new(nodePosition);

                    if (IsWalkableArea(nodePosition, out float elevation))
                    {
                        newNode.elevation = elevation;
                        newNode.position = new Vector3(nodePosition.x, elevation, nodePosition.z);
                        newNode.isWalkable = true;
                        newNode.maxConnectionDistance = _maxConnectionDistance;
                        newNode.maxElevationDifference = _maxElevationDifference;
                    }

                    _nodes[x, y] = newNode;
                    newNode.gridPosition = new Vector2Int(x, y);
                }
            }

            ConnectNodes();
            InitializeCells();
        }

        private void InitializeCells()
        {
            DestroyCells();

            int numCellsX = Mathf.CeilToInt(_gridSize.x / _cellSize);
            int numCellsY = Mathf.CeilToInt(_gridSize.y / _cellSize);

            _cells = new Cell[numCellsX, numCellsY];
            for (int x = 0; x < numCellsX; x++)
            {
                for (int y = 0; y < numCellsY; y++)
                {
                    Cell newCell = Instantiate(_cellPrefab);
                    _cells[x, y] = newCell;

                    newCell.name = $"Cell {x}-{y}";
                    newCell.gameObject.layer = 2; // Ignore raycast
                    newCell.transform.SetParent(transform);
                    newCell.transform.position = GetCellPosition(x, y);
                    newCell.transform.localScale = CellSize;
                    newCell.GridSystem = this;
                    newCell.CellX = x;
                    newCell.CellY = y;

                    // Determine the nodes that belong to this cell
                    int startX = x * _cellSize;
                    int startY = y * _cellSize;
                    int endX = Mathf.Min(startX + _cellSize, _gridSize.x);
                    int endY = Mathf.Min(startY + _cellSize, _gridSize.y);

                    for (int nodeX = startX; nodeX < endX; nodeX++)
                    {
                        for (int nodeY = startY; nodeY < endY; nodeY++)
                        {
                            Node node = _nodes[nodeX, nodeY];
                            newCell.AddNode(node);
                        }
                    }
                }
            }
        }

        public bool IsWalkableArea(Vector3 position, out float elevation)
        {
            // Create a ray from above the position downward
            Ray ray = new(position + Vector3.up * _maxWorldHeight, Vector3.down);
            RaycastHit hit;

            float maxRaycastDistance = 200f;

            if (Physics.Raycast(ray, out hit, maxRaycastDistance, _obstacleLayerMask))
            {
                elevation = 0f;
                return false; // If an obstacle is hit
            }

            if (Physics.Raycast(ray, out hit, maxRaycastDistance, _walkableLayerMask))
            {
                elevation = hit.point.y + _nodeHeightOffset;
                return true; // If walkable terrain is hit
            }

            elevation = 0f;
            return false; // If no walkable terrain is found
        }

        public void UpdateCell(int cellX, int cellY)
        {
            Cell cell = _cells[cellX, cellY];
            foreach (Node node in cell.Nodes)
            {
                Vector3 position = node.position;
                if (IsWalkableArea(position, out float elevation))
                {
                    node.isWalkable = true;
                    node.elevation = elevation;
                    node.position = new Vector3(position.x, elevation, position.z);
                    node.maxConnectionDistance = _maxConnectionDistance;
                    node.maxElevationDifference = _maxElevationDifference;
                }
                else
                {
                    node.isWalkable = false;
                }
            }

            ConnectNodes();
        }

        private void ConnectNodes()
        {
            foreach (int x in Enumerable.Range(0, _gridSize.x))
                foreach (int y in Enumerable.Range(0, _gridSize.y))
                    CheckNodeConnections(x, y);
        }

        private void CheckNodeConnections(int x, int y)
        {
            Node currentNode = _nodes[x, y];

            foreach (int i in Enumerable.Range(x - 1, _maxNodeConnections))
            {
                foreach (int j in Enumerable.Range(y - 1, _maxNodeConnections))
                {
                    if (i == x && j == y || i < 0 || i >= _gridSize.x || j < 0 || j >= _gridSize.y)
                        continue;

                    Node otherNode = _nodes[i, j];

                    if (otherNode == null)
                        continue;
                    
                    // perform linecast between nodes to check for obstacles
                    if (_useRefinedConnections)
                    {
                        Vector3 direction = otherNode.position - currentNode.position;
                        float distance = Vector3.Distance(currentNode.position, otherNode.position);
                        Ray ray = new Ray(currentNode.position, direction);

                        if (Physics.Linecast(ray.origin, ray.origin + ray.direction * distance))
                            continue;
                    }

                    currentNode.AddConnectedNode(otherNode);
                }
            }
        }

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
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x - transform.position.x) / (_gridSize.x * _nodeDistance));
            float percentY = Mathf.Clamp01((worldPosition.z - transform.position.z) / (_gridSize.y * _nodeDistance));

            int x = Mathf.RoundToInt(_gridSize.x * percentX);
            int y = Mathf.RoundToInt(_gridSize.y * percentY);

            Node currentNode = _nodes[x, y];

            return currentNode;
        }

        private Vector3 GetCellPosition(int xIndex, int yIndex)
        {
            // Calculate the bottom left position of the cell
            Vector3 cellBottomLeft = transform.position + new Vector3(xIndex * CellSize.x, 0, yIndex * CellSize.y);

            // Calculate the center position of the cell
            Vector3 cellCenter = cellBottomLeft + new Vector3(CellSize.x / 2, 0, CellSize.z / 2);
            return cellCenter;
        }

        private void OnDrawGizmos() // Mark Todo: Clean up this method. Move drawing to owner classes.
        {
            if (!_showGrid)
                return;

            // draw a bounding box around the grid based on grid size
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(_gridSize.x * _nodeDistance / 2f, _maxWorldHeight / 2, _gridSize.y * _nodeDistance / 2f), new Vector3(_gridSize.x * _nodeDistance, _maxWorldHeight, _gridSize.y * _nodeDistance));

            if (_nodes != null)
            {
                foreach (Node node in _nodes)
                {
                    if (!node.isWalkable)
                        continue;
                    
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(node.position, 0.1f);
                
                    Gizmos.color = Color.cyan;
                    foreach (Node connectedNode in node.connectedNodes)
                    {
                        if (!connectedNode.isWalkable)
                            continue;
                        
                        Gizmos.DrawLine(node.position, connectedNode.position);
                    }
                }
            }
        }
    }
}