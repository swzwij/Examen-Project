using UnityEngine;
using FishNet.Object;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 5.0f;

    private void Update()
    {
        if (!IsOwner)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new(horizontalInput, 0, verticalInput);

        transform.position += (float)TimeManager.TickDelta * _speed * movement;

        SendMovementToServer(movement);
    }

    [ServerRpc]
    private void SendMovementToServer(Vector3 movement)
    {
        transform.position += (float)TimeManager.TickDelta * _speed * movement;
        BroadcastMovement(transform.position); // Broadcast to all clients
    }

    [ObserversRpc] // Client version will receive position update
    private void BroadcastMovement(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
