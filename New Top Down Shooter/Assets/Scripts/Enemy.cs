using System.Collections;
using UnityEngine;

namespace TimeStrike.Enemies
{
    public class Enemy : MonoBehaviour
    {
        protected EnemyWeaponImplementations.IEnemyWeapon WeaponImplementation { get; private set; }
        protected Transform Player { get; private set; }
        protected bool HasSeenPlayer { get; private set; }

        [SerializeField]
        private WeaponInspectorSelection.WeaponSelectorEnum _weapon;
        [SerializeField]
        private float _sightDistance;
        [SerializeField]
        private LayerMask _sightMask;

        protected virtual void Awake()
        {
            WeaponImplementation = WeaponInspectorSelection.WeaponsFromEnumArray[(int)_weapon];
            WeaponImplementation.Initialize(this);
        }
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            
            if (Player is null) Debug.LogWarning("Enemy was unable to find player. Ensure there is a player in scene, and it is tagged with the Player tag.", this);
        }
        protected virtual void Update()
        {
            if (Player is null) return;

            if (HasSeenPlayer)
            {
                transform.LookAt(Player);
            }
            else
            {
                if (CanSeePlayer()) HasSeenPlayer = true;
            }
            WeaponImplementation.Update();
        }
        public bool CanSeePlayer()
        {
            Ray ray = new Ray(transform.position, Player.position - transform.position);
            Physics.Raycast(ray, out var hitData, _sightDistance, _sightMask);
            if (hitData.collider.transform == Player) return true;
            
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _sightDistance);
        }


        // God have mercy on me for this entire section of code, but this was the best way i could think of to make an inspector
        // weapon selector.
        private class WeaponInspectorSelection
        {
            public enum WeaponSelectorEnum
            {
                DebugNull = 0,
                DebugM9Beretta = 1
            }
            public static EnemyWeaponImplementations.IEnemyWeapon[] WeaponsFromEnumArray = new EnemyWeaponImplementations.IEnemyWeapon[] {
                new EnemyWeaponImplementations.DebugNull(),
                new EnemyWeaponImplementations.DebugM9Beretta()
            };
        }
    }
    public class EnemyWeaponImplementations
    {
        public interface IEnemyWeapon
        {
            void Initialize(Enemy enemy);
            void Update();
        }
        public class DebugNull : IEnemyWeapon
        {
            public void Initialize(Enemy enemy)
            {
                return;
            }
            public void Update()
            {
                return;
            }
        }
        public class DebugM9Beretta : IEnemyWeapon
        {
            private const int ROUNDSPERMAG = 18;
            private const float DAMAGEPERROUND = 50f;
            private const float BULLETSPEED = 20f;
            private const float RELOADSECONDS = 5f;
            private const int ROUNDSPERMINUTE = 200;

            private Enemy _enemy;
            private int _ammo;
            private bool _isReloading;
            private WaitForSeconds _reloadWfs;
            private float _timeBetweenShots => 60f / ROUNDSPERMINUTE;
            private float _fireTimer;
            private Bullet _bulletPrefab;

            public void Initialize(Enemy enemy)
            {
                _enemy = enemy;
                _reloadWfs = new(RELOADSECONDS);
                _ammo = ROUNDSPERMAG;
                _bulletPrefab = ReferenceManager.Instance.Bullet;
                Debug.Log("Debug M9 initialize");
            }
            public void Update()
            {
                Debug.Log("Debug M9 update call");
                if (!_isReloading)
                {
                    if (_ammo <= 0)
                    {
                        _enemy.StartCoroutine(Co_Reload());
                    }
                    if (_enemy.CanSeePlayer())
                    {
                        _fireTimer -= Time.deltaTime;
                        if (_fireTimer <= 0f)
                        {
                            Fire();
                            _fireTimer = _timeBetweenShots;
                        }
                    }
                    else
                    {
                        _fireTimer = 0; // So the enemy fires immediately after seeing the player
                    }
                }
                
                void Fire()
                {
                    Bullet instantiatedBullet = Object.Instantiate(_bulletPrefab, _enemy.transform.position, _enemy.transform.rotation);
                    instantiatedBullet.Initialize(DAMAGEPERROUND, BULLETSPEED, BulletShooter.Enemy);
                }
            }
            private IEnumerator Co_Reload()
            {
                _isReloading = true;
                yield return _reloadWfs;
                _ammo = ROUNDSPERMAG;
                _isReloading = false;
            }
        }
    }
}