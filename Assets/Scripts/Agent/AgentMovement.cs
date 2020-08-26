using System;
using UnityEngine;
using UnityEngine.AI;

namespace Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentMovement : MonoBehaviour
    {
        [SerializeField] private Transform Destination;
        private NavMeshAgent _agent;

        public void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent is null) throw new NullReferenceException("NavMeshAgent must be set.");

            SetDestination();
        }

        public void OnDestroy()
        {
            GetComponentInParent<AgentManager>()?.RemoveAgent(this);
        }

        private void SetDestination()
        {
            if (Destination == null) return;

            var target = Destination.transform.position;
            _agent.SetDestination(target);
        }
    }
}
