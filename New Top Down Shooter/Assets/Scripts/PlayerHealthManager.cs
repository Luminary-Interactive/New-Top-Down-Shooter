using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IDamagable
{
    public void OnHitByBullet(Bullet bullet)
    {
        Debug.Log($"Player hit by a bullet with {bullet.Damage} damage.", this);
    }
}