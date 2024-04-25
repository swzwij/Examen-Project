using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Examen.Pathfinding.Grid
{
    public class BoundingBoxTest : NetworkBehaviour
    {
        private void Start()
        {
            if (!IsServer)
                return;

            StartCoroutine(NewStart());
        }

        [Server]
        private void GetNodesInBounds()
        {
            HashSet<Node> nodes = GridSystem.Instance.GetNodesInTransformBounds(transform.position, transform.localScale, transform.rotation);
            foreach (Node node in nodes)
            {
                node.IsWalkable = false;
                node.IsOccupied = true; // TODO: Make sure this is set to false when the object is destroyed.
                Debug.LogError($"Node at {node.Position} is now occupied.");
            }

            // Cell currentCell = GridSystem.Instance.GetCellFromWorldPosition(transform.position);
            // currentCell.UpdateCell();
        }

        [Server]
        private IEnumerator NewStart()
        {
            Debug.LogError("Waiting");
            yield return new WaitForSeconds(10f);
            Debug.LogError("Starting");
            GetNodesInBounds();
        }
    }
}