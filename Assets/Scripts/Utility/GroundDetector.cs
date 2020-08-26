using System;
using UnityEngine;

namespace Utility
{
    public class GroundDetector : MonoBehaviour
    {
        public Vector3 Offset;
        public bool Grounded;
        public AdvancedSettings Advanced;
        public Vector3 GroundContactNormal;
        private CapsuleCollider _capsule;

        public void Awake()
        {
            _capsule = GetComponent<CapsuleCollider>();
        }

        public void FixedUpdate()
        {
            if (Physics.SphereCast(transform.position, _capsule.radius * (1.0f - Advanced.ShellOffset),
                Vector3.down, out var hitInfo,
                _capsule.height / 2f - _capsule.radius + Advanced.GroundCheckDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore))
            {
                Grounded = true;
                GroundContactNormal = hitInfo.normal;
            }
            else
            {
                Grounded = false;
                GroundContactNormal = Vector3.up;
            }
        }
    }
}
