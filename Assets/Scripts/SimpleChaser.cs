using UnityEngine;
using UnityEngine.AI;

public class SimpleChaser : MonoBehaviour
{
    public Transform target;
    public float updateInterval = 0.2f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
        if (target == null || agent == null) return;
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(target.position);
    }
}