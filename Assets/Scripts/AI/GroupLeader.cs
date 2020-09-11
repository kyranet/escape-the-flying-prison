using System;
using System.Collections;
using Agent;
using UnityEngine;

namespace AI
{
	[RequireComponent(typeof(AgentMovement))]
	public class GroupLeader : MonoBehaviour
	{
		private AgentMovement _am;

		private void Awake()
		{
			_am = GetComponent<AgentMovement>();
		}

		private void OnEnable()
		{
			transform.GetChild(0).gameObject.SetActive(true);
			GetComponent<GroupFollower>().enabled = false;

			StartCoroutine(nameof(Follow));
		}

		private IEnumerator Follow()
		{
			yield return new WaitForFixedUpdate();

			while (enabled)
			{
				var target = _am.AgentManager.Target;
				_am.SetRoute(target is null
					? null
					: _am.AgentManager.Map.GetRoute(transform.position, target.position));

				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}
