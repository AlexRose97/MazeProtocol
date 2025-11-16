using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class SimpleChaser : MonoBehaviour
    {
        public Transform target;
        public float updateInterval = 0.2f;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            InvokeRepeating(nameof(UpdateDestination), 0f, updateInterval);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(UpdateDestination));
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }

        private void UpdateDestination()
        {
            if (target == null || _agent == null) return;
            if (!_agent.isOnNavMesh) return;

            _agent.SetDestination(target.position);
        }
    }
}