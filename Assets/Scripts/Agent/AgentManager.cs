using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Agent
{
    public class AgentManager : MonoBehaviour
    {
        [SerializeField] private List<AgentMovement> Agents = new List<AgentMovement>();
        [CanBeNull] public AgentMovement Leader;

        public void Awake()
        {
            Agents = GetComponentsInChildren<AgentMovement>().ToList();
            PickLeader();
        }

        public void AddAgent(AgentMovement agent)
        {
            Agents.Add(agent);
        }

        public void RemoveAgent(AgentMovement agent)
        {
            Agents.Remove(agent);
            if (agent == Leader) PickLeader();
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
