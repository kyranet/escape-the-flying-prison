using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    [RequireComponent(typeof(MeshCollider))]
    public class DeathFloor : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Player"))
            {
                SceneManager.LoadSceneAsync("Scenes/Game");
            }
        }
    }
}
