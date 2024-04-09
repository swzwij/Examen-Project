using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using FishNet.Object;
using UnityEngine;

namespace Examen.Pathfinding
{
    public class Pathfinder : NetworkBehaviour
    {
        [SerializeField] private FollowerTypes _followerType = FollowerTypes.Player;
        [SerializeField] private int _diagonalCost = 14;
        [SerializeField] private int _straightCost = 10;

        private GridSystem _gridSystem;
        private HashSet<Node> _openSet = new();
        private HashSet<Node> _closedSet = new();
        private List<Node> _path = new();
        private List<Node> _retracedPath = new();

        private void OnEnable() => _gridSystem = FindObjectOfType<GridSystem>();

        /// <summary>
        /// Finds a path from the specified start position to the target position using the A* pathfinding algorithm.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <returns>A list of nodes representing the path from the start position to the target position.</returns>
        [Server]
        public List<Node> FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            Node startNode = _gridSystem.GetNodeFromWorldPosition(startPosition);
            Node targetNode = _gridSystem.GetNodeFromWorldPosition(targetPosition);

            _openSet.Clear();
            _closedSet.Clear();

            _openSet.Add(startNode);

            while (_openSet.Count > 0)
            {
                GetNodeWithLowestCost(out Node currentNode);

                if (currentNode == null)
                    break;
                
                MoveToClosedSet(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                ProcessNeighbours(targetNode, currentNode);
            }

            return _path;
        }

        private Node GetNodeWithLowestCost(out Node currentNode)
        {
            currentNode = null;
            foreach (Node node in _openSet)
            {
                if (currentNode == null || node.FinalCost < currentNode.FinalCost 
                || (node.FinalCost == currentNode.FinalCost && node.HeuristicCost < currentNode.HeuristicCost))
                    currentNode = node;
            }

            return currentNode;
        }

        private void MoveToClosedSet(Node currentNode)
        {
            _openSet.Remove(currentNode);
            _closedSet.Add(currentNode);
        }

        private void ProcessNeighbours(Node targetNode, Node currentNode)
        {
            foreach (Node neighbour in currentNode.ConnectedNodes)
            {
                if (!neighbour.IsWalkable || _closedSet.TryGetValue(neighbour, out _))
                    continue;

                if (_followerType == FollowerTypes.Enemy && !neighbour.IsEnemyWalkable)
                    continue;

                int newMovementCostToNeighbor = currentNode.GoalCost + CalculateDistance(currentNode, neighbour);
                if (newMovementCostToNeighbor < neighbour.GoalCost || !_openSet.TryGetValue(neighbour, out _))
                {
                    neighbour.GoalCost = newMovementCostToNeighbor;
                    neighbour.HeuristicCost = CalculateDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (!_openSet.TryGetValue(neighbour, out _))
                        _openSet.Add(neighbour);
                }
            }
        }

        [Server]
        private int CalculateDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
            int yDistance = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);

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
            _retracedPath.Clear();

            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                _retracedPath.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            _retracedPath.Reverse();

            return _retracedPath;
        }
    }
}
