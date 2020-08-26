using UnityEngine;

namespace Player
{
    public class Shoot : MonoBehaviour
    {
        public GameObject Laser;
        public MonoBehaviour Controller;

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Laser.SetActive(true);
                Controller.enabled = false;
            }
            else if (Laser.activeSelf)
            {
                Laser.SetActive(false);
                Controller.enabled = true;
            }
        }
    }
}
