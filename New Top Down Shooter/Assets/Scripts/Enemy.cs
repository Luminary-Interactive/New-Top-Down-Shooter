using UnityEngine;

namespace TimeStrike.Enemies
{
    public class Enemy : MonoBehaviour
    {
        protected EnemyWeaponImplementations.IEnemyWeapon WeaponImplementation { get; private set; }

        [SerializeField]
        private WeaponInspectorSelection.WeaponSelectorEnum _weapon;
        [SerializeField]
        private float _sightDistance;

        private bool _lockOnToPlayer;
        private Transform _player;

        protected virtual void Awake()
        {
            WeaponImplementation = WeaponInspectorSelection.WeaponsFromEnumArray[(int)_weapon];
            WeaponImplementation.Initialize(this);
        }
        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            
            if (_player is null) Debug.LogWarning("Enemy was unable to find player. Ensure there is a player in scene, and it is tagged with the Player tag.", this);
        }
        protected virtual void Update()
        {
            if (_player is null) return;

            if (_lockOnToPlayer)
            {
                transform.LookAt(_player);
            }
            else
            {
                if (CanSeePlayer()) _lockOnToPlayer = true;
            }
        }
        private bool CanSeePlayer()
        {
            Ray ray = new Ray(transform.position, _player.position - transform.position);
            Physics.Raycast(ray, out var hitData, _sightDistance, Physics.DefaultRaycastLayers);
            if (hitData.collider.transform == _player) return true;
            
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
                DebugNull = 0
            }
            public static EnemyWeaponImplementations.IEnemyWeapon[] WeaponsFromEnumArray = new EnemyWeaponImplementations.IEnemyWeapon[] {
                new EnemyWeaponImplementations.DebugNull()
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
    }
}