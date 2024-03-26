using System.Collections.Generic;
using System.Linq;
using GameKit.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    public class GridSystem : MonoBehaviour
    {
        [SerializeField] private bool showGrid = true;
        [SerializeField] private float _maxWorldHeight = 30f;
        [SerializeField] private float maxElevationDifference = 0.6f;
        [SerializeField] private float maxConnectionDistance = 1f;
        [SerializeField] private float _nodeHeightOffset = 0.2f;
        [SerializeField] private LayerMask walkableLayerMask;
        [SerializeField] private LayerMask obstacleLayerMask;
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private int _cellSize = 10;
        [SerializeField] private bool useRefinedConnections = true;
        [SerializeField] int gridSizeX, gridSizeY;
        [SerializeField] private float nodeDistance = 1f;
        private Node[,] nodes;
        private Cell[,] cells;

        private Vector3 CellSize => new(_cellSize * nodeDistance, _cellSize * nodeDistance, _cellSize * nodeDistance);

        private void OnEnable() => CreateGrid();

        public void CreateGrid()
        {
            Vector3 startPosition = transform.position;

            nodes = new Node[gridSizeX, gridSizeY];

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 nodePosition = startPosition + new Vector3(x * nodeDistance, 0, y * nodeDistance);
                    Node newNode = new(nodePosition);

                    if (IsWalkableArea(nodePosition, out float elevation))
                    {
                        newNode.elevation = elevation;
                        newNode.position = new Vector3(nodePosition.x, elevation, nodePosition.z);
                        newNode.isWalkable = true;
                        newNode.maxConnectionDistance = maxConnectionDistance;
                        newNode.maxElevationDifference = maxElevationDifference;
                    }

                    nodes[x, y] = newNode;
                    newNode.gridPosition = new Vector2Int(x, y);
                }
            }

            ConnectNodes();
            InitializeCells();
        }

        private void InitializeCells()
        {
            DestroyCells();

            int numCellsX = Mathf.CeilToInt((float)gridSizeX / _cellSize);
            int numCellsY = Mathf.CeilToInt((float)gridSizeY / _cellSize);

            cells = new Cell[numCellsX, numCellsY];
            for (int x = 0; x < numCellsX; x++)
            {
                for (int y = 0; y < numCellsY; y++)
                {
                    Cell newCell = Instantiate(_cellPrefab);
                    cells[x, y] = newCell;

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
                    int endX = Mathf.Min(startX + _cellSize, gridSizeX);
                    int endY = Mathf.Min(startY + _cellSize, gridSizeY);

                    for (int nodeX = startX; nodeX < endX; nodeX++)
                    {
                        for (int nodeY = startY; nodeY < endY; nodeY++)
                        {
                            Node node = nodes[nodeX, nodeY];
                            newCell.AddNode(node);
                        }
                    }
                }
            }
        }

        public bool IsWalkableArea(Vector3 position, out float elevation)
        {
            // Create a ray from above the position downward
            Ray ray = new Ray(position + Vector3.up * _maxWorldHeight, Vector3.down);
            RaycastHit hit;

            float maxRaycastDistance = 200f;

            if (Physics.Raycast(ray, out hit, maxRaycastDistance, obstacleLayerMask))
            {
                elevation = 0f;
                return false; // If an obstacle is hit
            }

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, walkableLayerMask))
            {
                elevation = hit.point.y + _nodeHeightOffset;
                return true; // If walkable terrain is hit
            }

            elevation = 0f;
            return false; // If no walkable terrain is found
        }

        public void UpdateCell(int cellX, int cellY)
        {
            Cell cell = cells[cellX, cellY];
            foreach (Node node in cell.Nodes)
            {
                Vector3 position = node.position;
                if (IsWalkableArea(position, out float elevation))
                {
                    node.isWalkable = true;
                    node.elevation = elevation;
                    node.position = new Vector3(position.x, elevation, position.z);
                    node.maxConnectionDistance = maxConnectionDistance;
                    node.maxElevationDifference = maxElevationDifference;
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
            foreach (int x in Enumerable.Range(0, gridSizeX))
                foreach (int y in Enumerable.Range(0, gridSizeY))
                    CheckNodeConnections(x, y);
        }

        private void CheckNodeConnections(int x, int y)
        {
            Node currentNode = nodes[x, y];

            foreach (int i in Enumerable.Range(x - 1, 3))
            {
                foreach (int j in Enumerable.Range(y - 1, 3))
                {
                    if (i == x && j == y || i < 0 || i >= gridSizeX || j < 0 || j >= gridSizeY)
                        continue;

                    Node otherNode = nodes[i, j];

                    if (otherNode == null)
                        continue;
                    
                    // perform linecast between nodes to check for obstacles
                    if (useRefinedConnections)
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
            nodes = null;
            cells = null;
            DestroyCells();
        }

        private void DestroyCells()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x - transform.position.x) / (gridSizeX * nodeDistance));
            float percentY = Mathf.Clamp01((worldPosition.z - transform.position.z) / (gridSizeY * nodeDistance));

            int x = Mathf.RoundToInt(gridSizeX * percentX);
            int y = Mathf.RoundToInt(gridSizeY * percentY);

            Node currentNode = nodes[x, y];

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
            if (!showGrid)
                return;

            // draw a bounding box around the grid based on grid size
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(gridSizeX * nodeDistance / 2f, _maxWorldHeight / 2, gridSizeY * nodeDistance / 2f), new Vector3(gridSizeX * nodeDistance, _maxWorldHeight, gridSizeY * nodeDistance));

            if (nodes != null)
            {
                foreach (Node node in nodes)
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