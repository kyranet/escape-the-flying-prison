using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Agent
{
    public class AgentMovement : MonoBehaviour
    {
        public float Speed = 3f;
        private AgentManager AgentManager;
        private Stack<Vector3> Steps;
        [CanBeNull] private Coroutine Coroutine;

        public void Awake()
        {
            AgentManager = GetComponentInParent<AgentManager>();
        }

        public void OnDestroy()
        {
            AgentManager.RemoveAgent(this);
        }

        public void SetDestination([CanBeNull] Transform position)
        {
            if (position is null) Steps.Clear();
            else Steps = AgentManager.Map.GetRoute(transform.position, position.position);

            if (!(Coroutine is null)) StopCoroutine(Coroutine);
            Coroutine = StartCoroutine(nameof(MoveTowards));
        }

        private IEnumerator MoveTowards()
        {
            while (Steps.Count > 0)
            {
                var movement = Speed * Time.deltaTime;
                var difference = Steps.Peek() - transform.position;
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
