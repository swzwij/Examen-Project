using FishNet.Object;
using System.Collections;
using UnityEngine;

public class PingObject : NetworkBehaviour
{
    public UnityEngine.Camera Camera { get; set; }

    [SerializeField] private float _despawnTime = 5f;

    private Canvas _canvas;
    private bool _isCameraInitialised;

    private void Update()
    {
        if (Camera == null)
            return;

        if (!_isCameraInitialised)
        {
            _isCameraInitialised = true;
            _canvas.worldCamera = Camera;
        }

        Vector3 cameraPosition = Camera.transform.position;
        Vector3 lookPosition = new Vector3(cameraPosition.x, gameObject.transform.position.y, cameraPosition.z);

        gameObject.transform.LookAt(lookPosition);
    }

    private void OnEnable() => StartCoroutine(DespawnTimer());
    private void OnDisable() => StopCoroutine(DespawnTimer());

    IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(_despawnTime);
        Destroy(gameObject);

        ProcessDespawn(gameObject);
    }

    [Server]
    private void ProcessDespawn(GameObject pingObject) => BroadcastDespawn(pingObject);

    [ObserversRpc]
    private void BroadcastDespawn(GameObject pingObject) => Destroy(pingObject);
}
