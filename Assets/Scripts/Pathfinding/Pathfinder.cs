using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Profiling;

namespace Examen.Pathfinding
{
    public class Pathfinder : NetworkBehaviour
    {
        [SerializeField] private int _diagonalCost = 14;
        [SerializeField] private int _straightCost = 10;

        private GridSystem _gridSystem;
        private HashSet<Node> _openSet = new();
        private HashSet<Node> _closedSet = new();

        private void OnEnable() => _gridSystem = FindObjectOfType<GridSystem>();

        /// <summary>
        /// Finds a path from the specified start position to the target position using the A* pathfinding algorithm.
        /// </summary>
        /// <param name="startPos">The starting position.</param>
        /// <param name="targetPos">The target position.</param>
        /// <returns>A list of nodes representing the path from the start position to the target position.</returns>
        [Server]
        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _gridSystem.GetNodeFromWorldPosition(startPos);
            Node targetNode = _gridSystem.GetNodeFromWorldPosition(targetPos);

            _openSet.Clear();
            _closedSet.Clear();

            _openSet.Add(startNode);

            while (_openSet.Count > 0)
            {
                Node currentNode = null;
                foreach (var node in _openSet)
                {
                    if (currentNode == null || node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    {
                        currentNode = node;
                    }
                }

                if (currentNode == null)
                    break;

                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighbor in currentNode.ConnectedNodes)
                {
                    if (!neighbor.IsWalkable || _closedSet.TryGetValue(neighbor, out _))
                        continue;

                    int newMovementCostToNeighbor = currentNode.GCost + CalculateDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !_openSet.TryGetValue(neighbor, out _))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = CalculateDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!_openSet.TryGetValue(neighbor, out _))
                            _openSet.Add(neighbor);
                    }
                }
            }

            return new List<Node>();
        }

        [Server]
        private int CalculateDistance(Node a, Node b)
        {
            int xDistance = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
            int yDistance = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);

            if (xDistance > yDistance)
            {
                int diagonalDistance = _diagonalCost * yDistance + _straightCost * (xDistance - yDistance);
                return diagonalDistance;
            }

            int straightDistance = _diagonalCost * xDistance + _straightCost * (yDistance - xDistance);
            return straightDistance;
        }

        [Server]
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();

            return path;
        }
    }
}
