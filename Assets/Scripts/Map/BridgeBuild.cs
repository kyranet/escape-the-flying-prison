using UnityEngine;
using UnityEngine.AI;

namespace Map
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class BridgeBuild : MonoBehaviour
    {
        public void OnEnable()
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
