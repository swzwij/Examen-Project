using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private LayerMask _updateCellMask;

        private HashSet<Node> _nodes = new();
        private HashSet<Node> _activeNodes = new();

        public GridSystem GridSystem { private get; set; }
        public int CellX { get; set; }
        public int CellY { get; set; }
        public HashSet<Node> ActiveNodes { get => _activeNodes; set => _activeNodes = value; }
        public HashSet<Node> Nodes => _nodes;
        public HashSet<Cell> Neighbours { get; set; } = new HashSet<Cell>();

        /// <summary>
        /// Adds a node to the cell.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(Node node) => _nodes.Add(node);
        
        /// <summary>
        /// Updates this cell.
        /// </summary>
        public void UpdateCell()
        {
            GridSystem.UpdateCell(CellX, CellY);
            foreach (var neighbour in Neighbours)
                GridSystem.UpdateCell(neighbour.CellX, neighbour.CellY);
        }
    }
}
