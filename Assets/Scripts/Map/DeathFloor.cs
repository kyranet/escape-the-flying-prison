using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    [RequireComponent(typeof(MeshCollider))]
    public class DeathFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.CompareTag("Player"))
            {
                SceneManager.LoadSceneAsync("Scenes/Game");
            }
            else if (collider.transform.CompareTag("NPC"))
            {
                Destroy(collider.transform.gameObject);
            }
        }
    }
}
