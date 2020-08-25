using UnityEngine;
using Utility;

namespace Player
{
    public class HeadBob : MonoBehaviour
    {
        public Camera Camera;
        public CurveControlledBob MotionBob = new CurveControlledBob();
        public LerpControlledBob JumpAndLandingBob = new LerpControlledBob();
        public RigidBodyFirstPersonController RigidBodyFirstPersonController;
        public float StrideInterval;
        [Range(0f, 1f)] public float RunningStrideLengthen;

        private bool _previouslyGrounded;
        private Vector3 _originalCameraPosition;

        private void Start()
        {
            MotionBob.Setup(Camera, StrideInterval);
            _originalCameraPosition = Camera.transform.localPosition;
        }

        private void Update()
        {
            Vector3 newCameraPosition;
            if (RigidBodyFirstPersonController.Velocity.magnitude > 0 && RigidBodyFirstPersonController.Grounded)
            {
                Camera.transform.localPosition = MotionBob.DoHeadBob(RigidBodyFirstPersonController.Velocity.magnitude *
                                                                     (RigidBodyFirstPersonController.Running
                                                                         ? RunningStrideLengthen
                                                                         : 1f));
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = Camera.transform.localPosition.y - JumpAndLandingBob.Offset();
            }
            else
            {
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = _originalCameraPosition.y - JumpAndLandingBob.Offset();
            }

            Camera.transform.localPosition = newCameraPosition;

            if (!_previouslyGrounded && RigidBodyFirstPersonController.Grounded)
            {
                StartCoroutine(JumpAndLandingBob.DoBobCycle());
            }

            _previouslyGrounded = RigidBodyFirstPersonController.Grounded;
        }
    }
}