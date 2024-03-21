using UnityEngine;
using FishNet.Object;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5.0f;

    private void Update()
    {
        if (!IsOwner) // Check if this client is allowed to move this player
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        SendInputToServer(horizontalInput, verticalInput);
    }

    [ServerRpc] // ServerRpc sends data to the server
    private void SendInputToServer(float horizontalInput, float verticalInput)
    {
        // Send input to the server to set the position, this makes it so there is server authority which means players cannot set their own position.

        ProcessInput(horizontalInput, verticalInput);
    }

    [Server] // Server methods only get executed on server cleints.
    private void ProcessInput(float horizontalInput, float verticalInput)
    {
        // Server takes inputs and converts it into a position

        Vector3 movement = new(horizontalInput, 0, verticalInput); 
        Vector3 scaledMovement = (float)TimeManager.TickDelta * speed * movement;
        transform.position += scaledMovement;

        BroadcastPosition(transform.position);
    }

    [ObserversRpc] // ObserversRpc wil send things to other clients in the server
    private void BroadcastPosition(Vector3 newPosition)
    {
        // new position gets broadcasted to all the other players in the server.

        transform.position = newPosition;
    }
}
