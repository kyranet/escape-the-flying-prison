using System.Collections;
using Agent;
using UnityEngine;

namespace AI
{
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(AgentMovement))]
	public class GroupFollower : MonoBehaviour
	{
		public float Calm = 100f;
		public float CalmNearLeader = 10f;
		public float CalmNearPlayer = 1f;
		public float CalmFarFromAnybody = -5f;
		public Material ScaredMaterial;
		public Material RegularMaterial;
		private bool _scared;
		private AgentMovement _am;
		private Renderer _renderer;

		private bool Scared => Calm < 20f;

		private void Awake()
		{
			_am = GetComponent<AgentMovement>();
			_renderer = GetComponent<Renderer>();
			_scared = Scared;
		}

		private void OnEnable()
		{
			StartCoroutine(nameof(Follow));
		}

		private IEnumerator Follow()
		{
			yield return new WaitForFixedUpdate();

			while (true)
			{
				if (FollowLeader(out var leaderDistance))
				{
					if (leaderDistance == 0) Calm += CalmNearLeader;
					else Calm -= CalmNearLeader / leaderDistance * 10f;
				}
				else if (FollowPlayer(out var playerDistance))
				{
					if (playerDistance == 0) Calm += CalmNearPlayer;
					else Calm -= CalmNearPlayer / playerDistance * 10f;
				}
				else
				{
					Calm -= CalmFarFromAnybody;
					_am.SetRoute(null);
				}

				if (!_scared && Scared)
				{
					_scared = true;
					_renderer.material = ScaredMaterial;
				}
				else if (_scared && !Scared)
				{
					_scared = false;
					_renderer.material = RegularMaterial;
				}

				yield return new WaitForSeconds(0.5f);
			}
		}

		private bool FollowLeader(out float distance)
		{
			var leader = _am.AgentManager.Leader;
			if (leader is null)
			{
				distance = 0f;
				return false;
			}

			if (!leader.gameObject.activeSelf)
			{
				distance = 0f;
				return false;
			}

			var route = _am.AgentManager.Map.GetRoute(transform.position, leader.transform.position);
			_am.SetRoute(route);
			distance = route.Count - 1;
			return true;
		}

		private bool FollowPlayer(out float distance)
		{
			var player = _am.AgentManager.Player;
			if (player is null)
			{
				distance = 0f;
				return false;
			}

			var route = _am.AgentManager.Map.GetRoute(transform.position, player.transform.position);
			_am.SetRoute(route);
			distance = route.Count - 1;
			return true;
		}
	}
}
