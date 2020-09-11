using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Agent
{
	public class AgentMovement : MonoBehaviour
	{
		public float Speed = 3f;
		public AgentManager AgentManager;
		private Stack<Vector3> Steps;

		[CanBeNull]
		private Coroutine Coroutine;

		private void Awake()
		{
			AgentManager = GetComponentInParent<AgentManager>();
		}

		public void OnDestroy()
		{
			AgentManager.RemoveAgent(this);
		}

		public void SetRoute([CanBeNull] Stack<Vector3> steps)
		{
			if (!(Coroutine is null))
			{
				StopCoroutine(Coroutine);
				Coroutine = null;
			}

			if (steps is null)
			{
				Steps = new Stack<Vector3>();
			}
			else
			{
				Steps = steps;
				Coroutine = StartCoroutine(nameof(MoveTowards));
			}
		}

		private IEnumerator MoveTowards()
		{
			while (Steps.Count > 0)
			{
				var movement = Speed * Time.deltaTime;
				var difference = Steps.Peek() - transform.position;
				Debug.DrawLine(transform.position, transform.position + difference, Color.magenta, 5f);
				if (difference.sqrMagnitude < 0.1f)
				{
					Steps.Pop();
					continue;
				}

				var vectorMovement = difference.normalized * movement;
				transform.Translate(vectorMovement, Space.Self);
				yield return new WaitForFixedUpdate();
			}
		}
	}
}
