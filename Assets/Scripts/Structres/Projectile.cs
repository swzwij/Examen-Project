using FishNet.Object;
using MarkUlrich.Health;
using Swzwij.Extensions;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _damage;

    private Transform _target;

    public Transform Target { set => _target = value; }

    private void Awake() => Destroy(gameObject, 10f);

    private void FixedUpdate()
    {
        Debug.LogError("FixedUpdate");

        if (!IsServer)
            return;

        Debug.LogError("TryMove");

        Move();
    }

    [Server]
    private void OnDestroy() => DestroyProjectile();

    [Server]
    private void Move()
    {
        Debug.LogError("Move");

        transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed);
        Vector3 direction = _target.position - transform.position;

        if (direction == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(direction);

        UpdatePosition(transform.position);
        UpdateRotation(transform.rotation);
    }

    [ObserversRpc]
    private void UpdatePosition(Vector3 position) => transform.position = position;

    [ObserversRpc]
    private void UpdateRotation(Quaternion rotation) => transform.rotation = rotation;

    [ObserversRpc]
    private void DestroyProjectile() => Destroy(gameObject);

    [ObserversRpc]
    private void UpdateParent(Transform parent) => transform.parent = parent;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer)
            return;

        transform.parent = collision.transform;
        UpdateParent(collision.transform);

        HealthData health = collision.gameObject.TryGetCachedComponent<HealthData>();

        if (health != null)
            health.TakeDamage(_damage);

        DestroyProjectile();
    }
}