using System;
using UnityEngine;

namespace Player
{
    public class AgentCreator : MonoBehaviour
    {
        public float Distance = 8f;
        [SerializeField] private Transform Weapon;
        [SerializeField] private Transform Agents;
        [SerializeField] private GameObject AgentPrefab;

        public void Shoot()
        {
            var point = GetSpawnPoint();
            Instantiate(AgentPrefab, point, Quaternion.identity, Agents);
        }

        private Vector3 GetSpawnPoint()
        {
            if (Physics.Raycast(transform.position, Weapon.transform.up, out var hit, Distance))
            {
                return hit.point;
            }

            return transform.position + Weapon.transform.up * Distance;
        }
    }
}
