using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Player
{
    public class AgentCreator : MonoBehaviour
    {
        public float Distance = 8f;
        [SerializeField] private Transform Weapon;
        [CanBeNull] private GameObject CapturedAgent;

        public void Shoot()
        {
            if (Physics.Raycast(transform.position, Weapon.transform.up, out var hit, Distance))
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

            CapturedAgent.transform.position = transform.position + Weapon.transform.up * Distance;
            CapturedAgent.SetActive(true);
            CapturedAgent = null;
        }
    }
}
