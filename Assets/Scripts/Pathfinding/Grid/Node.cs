using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    [System.Serializable]
    public class Node
    {
        private Vector3 position;
        private List<Node> connectedNodes;
        private float elevation;
        private float maxElevationDifference;
        private float maxConnectionDistance;
        private bool isWalkable;
        private Vector2Int gridPosition;
        private Node parent;

        // Cost variables for A* algorithm
        private int gCost; // Movement cost
        private int hCost; // Heuristic cost

        public int GCost { get => gCost; set => gCost = value; }
        public int HCost { get => hCost; set => hCost = value; }
        public int FCost { get { return gCost + hCost; } } // Total cost 

        public List<Node> ConnectedNodes => connectedNodes;
        public Node Parent { get => parent; set => parent = value; }
        public Vector3 Position { get => position; set => position = value; }
        public float Elevation { get => elevation; set => elevation = value; }
        public bool IsWalkable { get => isWalkable; set => isWalkable = value; }
        public float MaxElevationDifference { get => maxElevationDifference; set => maxElevationDifference = value; }
        public float MaxConnectionDistance { get => maxConnectionDistance; set => maxConnectionDistance = value; }
        public Vector2Int GridPosition { get => gridPosition; set => gridPosition = value; }

        public Node(Vector3 pos)
        {
            position = pos;
            connectedNodes = new List<Node>();
            elevation = pos.y;
            maxElevationDifference = 0.6f;
            maxConnectionDistance = 1.5f;
            isWalkable = false;
        }

        public void AddConnectedNode(Node node)
        {
            if (CalculateElevation(node) <= maxElevationDifference && CalculateDistance(node) <= maxConnectionDistance)
            {
                Vector3 direction = node.Position - Position;
                float distance = Vector3.Distance(Position, node.Position);
                Ray ray = new(Position, direction);

                if (Physics.Linecast(ray.origin, ray.origin + ray.direction * distance))
                    return;
                
                connectedNodes.Add(node);
            }
        }

        public void RemoveConnectedNode(Node node) => connectedNodes.Remove(node);

        private float CalculateElevation(Node otherNode) => Mathf.Abs(elevation - otherNode.elevation);
        private float CalculateDistance(Node otherNode) => Vector3.Distance(position, otherNode.position);
    }
}

