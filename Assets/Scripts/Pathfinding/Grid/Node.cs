using System.Collections.Generic;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    [System.Serializable]
    public class Node
    {
        public Vector3 position;
        public List<Node> connectedNodes;
        public float elevation;
        public float maxElevationDifference;
        public float maxConnectionDistance;
        public bool isWalkable;
        public Vector2Int gridPosition;
        public Node parent;

        // Cost variables for A* algorithm
        public int gCost; // Movement cost
        public int hCost; // Heuristic cost
        public int FCost { get { return gCost + hCost; } } // Total cost 

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
                connectedNodes.Add(node);
        }

        private float CalculateElevation(Node otherNode) => Mathf.Abs(elevation - otherNode.elevation);
        private float CalculateDistance(Node otherNode) => Vector3.Distance(position, otherNode.position);
    }
}

