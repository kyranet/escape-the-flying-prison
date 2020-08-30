using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class NavPlatform : MonoBehaviour
    {
        public List<NavJoint> Joints;
        public Bounds Bounds;

        [Serializable]
        public struct NavPlatformJoint
        {
            public NavPlatform Platform;

            /// <summary>
            /// Entrance's position.
            /// </summary>
            public Vector3 InPosition;

            /// <summary>
            /// Exit's position.
            /// </summary>
            public Vector3 OutPosition;

            /// <summary>
            /// The calculated distance from {InPosition} and {OutPosition}
            /// </summary>
            public float Distance => (InPosition - OutPosition).magnitude;
        }
        public List<NavPlatformJoint> Connections;

        public Vector3 Position => transform.position;
        public Vector3 Center => Position + Bounds.center;

        public bool Contains(Vector3 position)
        {
            return Bounds.Contains(position - Position);
        }

        public void Refresh()
        {
            Connections.Clear();
            foreach (var joint in Joints)
            {
                if (joint.A.Platform == this)
                {
                    Connections.Add(new NavPlatformJoint
                    {
                        Platform = joint.B.Platform,
                        InPosition = joint.A.Position + joint.A.Platform.Position,
                        OutPosition = joint.B.Position + joint.B.Platform.Position
                    });
                }
                else
                {
                    Connections.Add(new NavPlatformJoint
                    {
                        Platform = joint.A.Platform,
                        InPosition = joint.B.Position + joint.B.Platform.Position,
                        OutPosition = joint.A.Position + joint.A.Platform.Position
                    });
                }
            }
        }

        private void OnDrawGizmos()
        {
            var position = Position;
            var center = Center;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, Bounds.size);

            // Draw joints
            Gizmos.color = Color.blue;
            foreach (var joint in Joints)
            {
                if (joint.A.Platform == this)
                {
                    Gizmos.DrawLine(center, position + joint.A.Position);
                }
                else if (joint.B.Platform == this)
                {
                    Gizmos.DrawLine(center, position + joint.B.Position);
                }
            }
        }
    }
}
