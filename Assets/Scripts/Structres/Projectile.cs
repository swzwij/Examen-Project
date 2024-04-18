using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private Transform _target;

    private bool _shouldMove = true;
    private bool _hasCollided;

    public Transform Target
    {
        set => _target = value;
    }

    private void FixedUpdate()
    {
        if (!_shouldMove)
            return;

        transform.position = Vector3.MoveTowards(transform.position, _target.position, speed);
        Vector3 direction = _target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCollided)
            return;

        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        transform.parent = collision.transform;

        transform.position = position;
        transform.rotation = rotation;
        
        _shouldMove = false;
        _hasCollided = true;
    }
}
