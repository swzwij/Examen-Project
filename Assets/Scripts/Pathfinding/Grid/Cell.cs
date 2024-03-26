using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    [RequireComponent(typeof(BoxCollider))]
    public class Cell : MonoBehaviour
    {
        [SerializeField] private LayerMask _updateCellMask;

        private BoxCollider _collider;

        private HashSet<Cell> _connectedCells = new();
        private HashSet<Node> _nodes = new();
        public GridSystem GridSystem { private get; set; }
        public int CellX { private get; set; }
        public int CellY { private get; set; }

        public HashSet<Node> Nodes => _nodes;

        public void AddNode(Node node) => _nodes.Add(node);
        public void AddConnectedCell(Cell cell) => _connectedCells.Add(cell);

        private void OnEnable()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.includeLayers = _updateCellMask;
            _collider.excludeLayers = ~_updateCellMask;
        }

        private void OnTriggerEnter(Collider other) => GridSystem.UpdateCell(CellX, CellY);

        private void OnTriggerExit(Collider other) => GridSystem.UpdateCell(CellX, CellY);
    }
}
