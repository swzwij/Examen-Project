using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    [RequireComponent(typeof(BoxCollider))]
    public class Cell : MonoBehaviour
    {
        private BoxCollider _collider;

        private HashSet<Cell> _connectedCells = new();
        private HashSet<Node> _nodes = new();

        public HashSet<Node> Nodes => _nodes;

        public void AddNode(Node node) => _nodes.Add(node);
        public void AddConnectedCell(Cell cell) => _connectedCells.Add(cell);

        private void OnEnable()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
    }
}
