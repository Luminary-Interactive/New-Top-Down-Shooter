using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float Damage { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.TryGetComponent<IDamagable>(out var damagable))
        {
            // TODO : Logic / VFX, etc. for hitting a non damagable part, like a wall.
            return;
        }
        damagable.OnHitByBullet(this);
        Destroy(gameObject);
    }
    public void Initialize(float damage, float speed)
    {
        _rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        Damage = damage;
    }
}