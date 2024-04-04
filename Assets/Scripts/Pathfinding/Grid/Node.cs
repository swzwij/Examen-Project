using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    [System.Serializable]
    public class Node
    {
        private Vector3 _position;
        private List<Node> _connectedNodes;
        private float _elevation;
        private float _maxElevationDifference;
        private float _maxConnectionDistance;
        private bool _isWalkable;
        private Vector2Int _gridPosition;
        private float _nodeHeightOffset = 0.3f;
        private Node _parent;

        // Cost variables for A* algorithm
        private int _goalCost;
        private int _heuristicCost;

        public int GoalCost { get => _goalCost; set => _goalCost = value; }
        public int HeuristicCost { get => _heuristicCost; set => _heuristicCost = value; }
        public int FinalCost => _goalCost + _heuristicCost;

        public List<Node> ConnectedNodes => _connectedNodes;
        public Node Parent { get => _parent; set => _parent = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public float Elevation { get => _elevation; set => _elevation = value; }
        public bool IsWalkable { get => _isWalkable; set => _isWalkable = value; }
        public float MaxElevationDifference { get => _maxElevationDifference; set => _maxElevationDifference = value; }
        public float MaxConnectionDistance { get => _maxConnectionDistance; set => _maxConnectionDistance = value; }
        public Vector2Int GridPosition { get => _gridPosition; set => _gridPosition = value; }
        public float NodeHeightOffset { get => _nodeHeightOffset; set => _nodeHeightOffset = value; }

        /// <summary>
        /// Represents a node in a grid-based pathfinding system.
        /// </summary>
        /// <param name="pos">The position of the node in 3D space.</param>
        public Node(Vector3 pos)
        {
            _position = pos;
            _connectedNodes = new List<Node>();
            _elevation = pos.y;
            _maxElevationDifference = 0.6f;
            _maxConnectionDistance = 1.5f;
            _isWalkable = false;
        }

        /// <summary>
        /// Adds a connected node to the current node if it meets the criteria for elevation difference and connection distance.
        /// </summary>
        /// <param name="node">The node to add as a connected node.</param>
        public void AddConnectedNode(Node node)
        {
            if (CalculateElevation(node) > _maxElevationDifference || CalculateDistance(node) > _maxConnectionDistance)
                return;
            
            Vector3 direction = node.Position - Position;
            float distance = Vector3.Distance(Position, node.Position);
            Ray ray = new(Position, direction);

            if (Physics.Linecast(ray.origin, ray.origin + ray.direction * distance))
                return;

            _connectedNodes.Add(node);
        }

        private float CalculateElevation(Node otherNode) => Mathf.Abs(_elevation - otherNode._elevation);
        private float CalculateDistance(Node otherNode) => Vector3.Distance(_position, otherNode._position);
    }
}

