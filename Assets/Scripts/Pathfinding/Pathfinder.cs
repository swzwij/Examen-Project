using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using FishNet.Object;
using UnityEngine;

namespace Examen.Pathfinding
{
    public class Pathfinder : NetworkBehaviour
    {
        [SerializeField] private int _diagonalCost = 14;
        [SerializeField] private int _straightCost = 10;

        private GridSystem _gridSystem;

        private void OnEnable() => _gridSystem = FindObjectOfType<GridSystem>();

        [Server]
        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _gridSystem.GetNodeFromWorldPosition(startPos);
            Node targetNode = _gridSystem.GetNodeFromWorldPosition(targetPos);

            List<Node> openSet = new();
            HashSet<Node> closedSet = new();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    bool isLowerFCost = openSet[i].FCost < currentNode.FCost;
                    bool isSameFCost = openSet[i].FCost == currentNode.FCost;
                    bool isLowerHCost = openSet[i].HCost < currentNode.HCost;

                    if (isLowerFCost || (isSameFCost && isLowerHCost))
                        currentNode = openSet[i];
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighbor in currentNode.ConnectedNodes)
                {
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                        continue;

                    int newMovementCostToNeighbor = currentNode.GCost + CalculateDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = CalculateDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return new();
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
