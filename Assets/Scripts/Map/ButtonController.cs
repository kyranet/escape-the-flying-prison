using System.Collections;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class ButtonController : MonoBehaviour
    {
        public Material Off;
        public Material On;
        public bool Active;
        public float Lower = 0.06f;
        public float Transition = 1f;
        public GameObject Connection;
        public MapController Controller;
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.material = Off;
            Connection.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Active) return;
            var current = Controller.Current;

            Debug.Log(current);

            // If there was no bridge enabled or no bridge left, skip.
            if (current == null) return;

            // If the current bridge isn't the one following, don't activate.
            if (current != gameObject) return;

            Active = true;
            StartCoroutine(nameof(Activate));
        }

        private IEnumerator Activate()
        {
            var remaining = Transition;
            while (remaining > 0)
            {
                yield return null;

                var previous = remaining;
                remaining -= Time.deltaTime;
                var movement = (previous - remaining) * Lower;

                transform.Translate(0f, 0f, -movement, Space.Self);
            }

            _renderer.material = On;
            Connection.SetActive(true);
            Controller.Advance();
        }
    }
}
