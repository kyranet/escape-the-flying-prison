using System;
using UnityEngine;

namespace AI
{
    public class NavJoint : MonoBehaviour
    {
        [Serializable]
        public struct NavJointValue
        {
            public Vector3 Position;
            public NavPlatform Platform;
        }

        public NavJointValue A;
        public NavJointValue B;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            var a = A.Platform.transform.position + A.Position;
            var b = B.Platform.transform.position + B.Position;

            Gizmos.DrawWireSphere(a, 0.2f);
            Gizmos.DrawWireSphere(b, 0.2f);

            Gizmos.DrawLine(a, b);
        }
    }
}
