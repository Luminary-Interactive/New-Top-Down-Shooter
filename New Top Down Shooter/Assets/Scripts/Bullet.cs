using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    private const string PLAYERBULLETLAYER = "PlayerBullet";
    private const string ENEMYBULLETLAYER = "EnemyBullet";

    public float Damage { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider collider)
    {

        if (!collider.TryGetComponent<IDamagable>(out var damagable))
        {
            // TODO : Logic / VFX, etc. for hitting a non damagable part, like a wall.
            Destroy(gameObject);
            return;
        }
        damagable.OnHitByBullet(this);
        Destroy(gameObject);
    }
    public void Initialize(float damage, float speed, BulletShooter shooter)
    {
        _rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        Damage = damage;
        gameObject.layer = shooter switch
        {
            BulletShooter.Player => LayerMask.NameToLayer(PLAYERBULLETLAYER),
            BulletShooter.Enemy => LayerMask.NameToLayer(ENEMYBULLETLAYER),
            _ => LayerMask.NameToLayer(PLAYERBULLETLAYER)
        };
    }
}
public enum BulletShooter
{
    Player,
    Enemy
}