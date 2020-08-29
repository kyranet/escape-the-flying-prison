using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentMovement : MonoBehaviour
    {
        public Transform Destination;
        private NavMeshAgent _agent;
        private bool _pendingReTarget = true;

        public void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent is null) throw new NullReferenceException("NavMeshAgent must be set.");

            SetDestination();
        }

        public void OnEnable()
        {
            SetDestination();
        }

        public void OnDestroy()
        {
            GetComponentInParent<AgentManager>()?.RemoveAgent(this);
        }

        public void SetDestination([CanBeNull] Transform destination)
        {
            Destination = destination;
            SetDestination();
        }

        private void SetDestination()
        {
            if (Destination == null) return;

            if (enabled && _agent.isOnNavMesh)
            {
                var target = Destination.transform.position;
                _agent.SetDestination(target);
                _pendingReTarget = false;
            }
            else
            {
                _pendingReTarget = true;
            }
        }
    }
}
