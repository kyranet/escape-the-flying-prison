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
		public Transform Player;

		[CanBeNull]
		public Transform Target { get; set; }

		[SerializeField]
		private List<AgentMovement> Agents;

		[CanBeNull]
		public AgentMovement Leader;

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

		private void PickLeader()
		{
			if (Agents.Count == 0)
			{
				Leader = null;
			}
			else
			{
				var random = Random.Range(0, Agents.Count);
				var leader = Agents[random];
				leader.GetComponent<GroupLeader>().enabled = true;
				leader.GetComponent<GroupFollower>().enabled = false;
				Leader = leader;
			}
		}
	}
}
