using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class NavPlatform : MonoBehaviour
    {
        public List<NavJoint> Joints;
        public Vector3 Position;
        public Vector3 Size;

        private void OnDrawGizmos()
        {
            var center = transform.position;
            var boxCenter = center + Position;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCenter, Size);

            // Draw joints
            Gizmos.color = Color.blue;
            foreach (var joint in Joints)
            {
                if (joint.A.Platform == this)
                {
                    Gizmos.DrawLine(boxCenter, center + joint.A.Position);
                }
                else if (joint.B.Platform == this)
                {
                    Gizmos.DrawLine(boxCenter, center + joint.B.Position);
                }
            }
        }
    }
}
