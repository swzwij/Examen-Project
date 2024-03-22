using UnityEngine;
using System;
using FishNet.Object;

namespace Examen.Player
{
    [RequireComponent(typeof(Pointer))]
    public class Interactor : NetworkBehaviour
    {
        #region Testing
        private void Start() => _pointer.OnPointedAtPosition += DebugPointer;
        private void OnDisable() => _pointer.OnPointedAtPosition -= DebugPointer;

        private void DebugPointer(Vector3 position) 
            => Debug.DrawLine(transform.position, position, Color.red, 1f);
        #endregion

        private Pointer _pointer;

        public Action OnInteract; // Change to Action<Interactable>?

        private void OnEnable() => _pointer = GetComponent<Pointer>();
    }
}
