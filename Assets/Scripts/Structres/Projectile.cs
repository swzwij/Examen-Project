using UnityEngine;
using FishNet.Object;
using Swzwij.Extensions;
using MarkUlrich.Health;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _damage;
    [SerializeField] private float _maxTravelDistance;
    [SerializeField] private float _castLength = 10f;
    [SerializeField] private LayerMask _layerMask;

    private Transform _target;
    private float _distanceTraveled;

    public Transform Target { set => _target = value; }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        Move();

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _castLength, _layerMask))
        {
            HealthData healthData = hit.collider.gameObject.TryGetCachedComponent<HealthData>();
            
            if (healthData != null)
                healthData.TakeDamage(_damage);

            Destroy(gameObject);
        }

        if ((_distanceTraveled * _distanceTraveled) >= _maxTravelDistance)
            Destroy(gameObject);
    }

    [Server]
    private void Move()
    {
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.fixedDeltaTime);
        _distanceTraveled += (previousPosition - transform.position).sqrMagnitude;

        Vector3 direction = _target.position - transform.position;

        if (direction == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(direction);

        BroadcastPosition(transform.position);
        BroadcastRotation(transform.rotation);
    }

    [ObserversRpc]
    private void BroadcastPosition(Vector3 position) => transform.position = position;

    [ObserversRpc]
    private void BroadcastRotation(Quaternion rotation) => transform.rotation = rotation;
}