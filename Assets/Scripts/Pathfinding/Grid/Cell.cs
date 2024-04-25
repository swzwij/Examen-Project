using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private LayerMask _updateCellMask;

        private HashSet<Node> _nodes = new();
        private HashSet<Node> _activeNodes = new();
        private HashSet<Cell> _adjacentCells = new();

        public GridSystem GridSystem { private get; set; }
        public int CellX { get; set; }
        public int CellY { get; set; }
        public HashSet<Node> ActiveNodes { get => _activeNodes; set => _activeNodes = value; }
        public HashSet<Node> Nodes => _nodes;
        public BoxCollider Collider => GetComponent<BoxCollider>();
        public Bounds Bounds => new(transform.position, transform.localScale);
        public HashSet<Cell> AdjacentCells => _adjacentCells;

        private void OnTriggerExit(Collider other) => GridSystem.UpdateCell(CellX, CellY);

        /// <summary>
        /// Adds a node to the cell.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(Node node) => _nodes.Add(node);
        
        /// <summary>
        /// Updates this cell.
        /// </summary>
        public void UpdateCell() => GridSystem.UpdateCell(CellX, CellY);

        public HashSet<Cell> GetAdjacentCells()
        {
            HashSet<Cell> adjacentCells = new();
            Vector3[] directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right
            };

            foreach (Vector3 direction in directions)
            {
                Vector3 cellPosition = transform.position + direction;
                Cell cell = GridSystem.GetCellFromWorldPosition(cellPosition);
                if (cell != null)
                    adjacentCells.Add(cell);
            }

            return adjacentCells;
        }
    }
}
