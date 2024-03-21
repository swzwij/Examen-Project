using UnityEngine;
using FishNet.Object;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5.0f;

    private void Update()
    {
        if (!base.IsOwner)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        SendInputToServer(horizontalInput, verticalInput);
    }

    [ServerRpc]
    void SendInputToServer(float horizontalInput, float verticalInput)
    {
        ProcessInput(horizontalInput, verticalInput);
    }

    [Server]
    private void ProcessInput(float horizontalInput, float verticalInput)
    {
        Vector3 movement = new(horizontalInput, 0, verticalInput); 
        Vector3 scaledMovement = (float)TimeManager.TickDelta * speed * movement;
        transform.position += scaledMovement;

        BroadcastPosition(transform.position);
    }

    [ObserversRpc]
    void BroadcastPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
