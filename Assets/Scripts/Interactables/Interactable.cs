using FishNet.Connection;
using FishNet.Object;

namespace Examen.Interactables
{
    public abstract class Interactable : NetworkBehaviour
    {
        public abstract InteractableTypes Type { get; }

        /// <summary>
        /// Calls all functionalities that need to happen when you are interacting with this object
        /// </summary>
        public abstract void Interact(NetworkConnection connection, float damageAmount = 0);

        /// <summary>
        /// Calls the sound that plays when interacting with this object
        /// </summary>
        public abstract void PlayInteractingSound();
    }
}
