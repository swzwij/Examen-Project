using UnityEngine;
using FishNet.Object;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _damage;

    private Transform _target;

    public Transform Target { set => _target = value; }

    private void Awake() => Destroy(gameObject, 10f);

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        Move();

        if (Vector3.Distance(transform.position, _target.position) < 0.1f)
            Destroy(gameObject);
    }

    [Server]
    private void Move()
    {
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
}