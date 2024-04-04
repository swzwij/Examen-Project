using FishNet.Object;

public abstract class Interactable : NetworkBehaviour
{
    /// <summary>
    /// Calls all functionalities that need to happen when you are interacting with this object
    /// </summary>
    public abstract void Interact(float damageAmount = 0);

    /// <summary>
    /// Calls the sound that plays when interacting with this object
    /// </summary>
    public abstract void PlayInteractingSound();
}
