using Examen.Spawning.ResourceSpawning;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    [RequireComponent(typeof(BoxCollider))]
    public class Cell : MonoBehaviour
    {
        [SerializeField] private LayerMask _updateCellMask;

        private BoxCollider _collider;

        private HashSet<Node> _allNodes = new();
        private HashSet<Node> _activeNodes = new();
        public GridSystem GridSystem { private get; set; }
        public int CellX { get; set; }
        public int CellY { get; set; }
        public HashSet<Node> ActiveNodes { get => _activeNodes; set => _activeNodes = value; }
        public HashSet<Node> AllNodes => _allNodes;
        public BoxCollider Collider => GetComponent<BoxCollider>();

        private void OnEnable()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.includeLayers = _updateCellMask;
            _collider.excludeLayers = ~_updateCellMask;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(GridSystem != null)
                GridSystem.UpdateCell(CellX, CellY);
        }
        private void OnTriggerExit(Collider other) => GridSystem.UpdateCell(CellX, CellY);

        /// <summary>
        /// Adds a node to the cell.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(Node node) => _allNodes.Add(node);

    }
}
