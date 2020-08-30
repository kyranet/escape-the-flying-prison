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
            var ray = new Ray(Weapon.position, Weapon.up);
            if (Physics.Raycast(ray, out var hit, Distance))
            {
                if (!(CapturedAgent is null))
                {
                    SpawnAgent(hit.point);
                }
                else if (hit.transform.CompareTag("NPC"))
                {
                    CapturedAgent = hit.transform.gameObject;
                    CapturedAgent.SetActive(false);
                }

                return;
            }

            if (CapturedAgent is null) return;

            SpawnAgent(ray.GetPoint(Distance));
        }

        private void SpawnAgent(Vector3 point)
        {
            System.Diagnostics.Debug.Assert(CapturedAgent != null, nameof(CapturedAgent) + " != null");

            CapturedAgent.transform.position = point;
            CapturedAgent.SetActive(true);
            CapturedAgent = null;
        }
    }
}
