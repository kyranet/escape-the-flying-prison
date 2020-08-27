using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(AgentCreator))]
    public class Shoot : MonoBehaviour
    {
        public GameObject Laser;
        public MonoBehaviour Controller;
        private AgentCreator _ac;

        private void Awake()
        {
            _ac = GetComponent<AgentCreator>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Laser.SetActive(true);
                Controller.enabled = false;
                _ac.Shoot();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Laser.SetActive(false);
                Controller.enabled = true;
            }
        }
    }
}
