using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using UnityEngine;

namespace Examen.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        private GridSystem gridSystem; // Mark Todo: Parent grid. Divide grid system into grid and grid system later and then replace this with grid.

        private void OnEnable() => gridSystem = FindObjectOfType<GridSystem>(); 

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = gridSystem.GetNodeFromWorldPosition(startPos);
            Node targetNode = gridSystem.GetNodeFromWorldPosition(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbor in currentNode.connectedNodes)
                {
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + CalculateDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = CalculateDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private int CalculateDistance(Node a, Node b)
        {
            int xDistance = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            int yDistance = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }

            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            return path;
        }
    }
}
