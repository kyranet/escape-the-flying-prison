using System.Collections.Generic;
using System.Linq;
using AI;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agent
{
    public class AgentManager : MonoBehaviour
    {
        public NavGenerator Map;
        [SerializeField] private List<AgentMovement> Agents;
        [CanBeNull] public AgentMovement Leader;

        public void Awake()
        {
            Agents = GetComponentsInChildren<AgentMovement>().ToList();
            PickLeader();
        }

        public void RemoveAgent(AgentMovement agent)
        {
            Agents.Remove(agent);
            if (agent == Leader) PickLeader();
        }

        public void SetTarget([CanBeNull] Transform destination)
        {
            foreach (var agent in Agents)
            {
                agent.SetDestination(destination);
            }
        }

        private void PickLeader()
        {
            if (Agents.Count == 0)
            {
                Leader = null;
            }
            else
            {
                var random = Random.Range(0, Agents.Count);
                Leader = Agents[random];
            }
        }
    }
}
