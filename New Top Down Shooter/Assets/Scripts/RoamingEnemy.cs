using UnityEngine;
using UnityEngine.AI;

namespace TimeStrike.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class RoamingEnemy : Enemy
    {
        private NavMeshAgent _navAgent;
        private Vector3 _lastRememberedPlayerPos;
        private NavMeshPath _path;

        protected override void Awake()
        {
            base.Awake();
            _navAgent = GetComponent<NavMeshAgent>();
            _lastRememberedPlayerPos = transform.position;
        }
        protected override void Update()
        {
            if (Player is null) return;

            base.Update();
            
            if (CanSeePlayer())
            {
                _lastRememberedPlayerPos = Player.position;
                _navAgent.ResetPath();
                _path = null;
            }
            else
            {
                _path = new();
                _navAgent.CalculatePath(_lastRememberedPlayerPos, _path);
                if (_path.status != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogWarning("RoamingEnemy was unable to find a valid path to player.", this);
                    _path = null;
                }
                _navAgent.SetPath(_path);
            }
        }
    }
}