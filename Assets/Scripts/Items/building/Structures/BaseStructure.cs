using Examen.Pathfinding.Grid;
using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace Examen.Structure
{
    public class BaseStructure : NetworkBehaviour
    {
        private GridSystem _gridSystem;

        public void Initialze()
        {
            StartCoroutine(WaitBeforeActivate());
            gameObject.SetActive(true);
        }

        private IEnumerator WaitBeforeActivate()
        {
            yield return new WaitForSeconds(2);
            ActivateStructure();
        }

        [ObserversRpc]
        private void ActivateStructure() => gameObject.SetActive(true);

        private void OnDestroy() 
        {
            if (!IsServer)
                return;

            _gridSystem = FindObjectOfType<GridSystem>();
            Cell cell = _gridSystem.GetCellFromWorldPosition(transform.position);
            _gridSystem.UpdateCell(cell);
        }
    }
}