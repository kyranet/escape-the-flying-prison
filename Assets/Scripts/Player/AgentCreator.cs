using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Player
{
    public class AgentCreator : MonoBehaviour
    {
        public float Distance = 8f;
        [SerializeField] private Camera Camera;
        [CanBeNull] private GameObject CapturedAgent;

        public void Shoot()
        {
            var ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Distance))
            {
                if (!(CapturedAgent is null))
                {
                    CapturedAgent.transform.position = hit.point;
                    CapturedAgent.SetActive(true);
                    CapturedAgent = null;
                }
                else if (hit.transform.CompareTag("NPC"))
                {
                    CapturedAgent = hit.transform.gameObject;
                    CapturedAgent.SetActive(false);
                }

                return;
            }

            if (CapturedAgent is null) return;

            CapturedAgent.transform.position = transform.position + ray.direction * Distance;
            CapturedAgent.SetActive(true);
            CapturedAgent = null;
        }
    }
}
