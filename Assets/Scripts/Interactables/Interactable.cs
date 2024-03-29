public interface Interactable
{
    /// <summary>
    /// Calls all functionalities that need to happen when you are interacting with this object
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Calls the sound that plays when interacting with this object
    /// </summary>
    public abstract void PlayInteractingSound();
}
