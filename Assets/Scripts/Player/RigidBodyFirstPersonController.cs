using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidBodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f; // Speed when walking forward
            public float BackwardSpeed = 4.0f; // Speed when walking backwards
            public float StrafeSpeed = 4.0f; // Speed when walking sideways
            public float RunMultiplier = 2.0f; // Speed when sprinting
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;

            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f),
                new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));

            [HideInInspector] public float CurrentTargetSpeed = 8f;

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed = StrafeSpeed;
                }

                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed = BackwardSpeed;
                }

                if (input.y > 0)
                {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }

                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    Running = true;
                }
                else
                {
                    Running = false;
                }
            }

            public bool Running { get; private set; }
        }
        
        [Serializable]
        public class AdvancedSettings
        {
            // Distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float GroundCheckDistance = 0.01f;

            public float StickToGroundHelperDistance = 0.5f; // stops the character
            public float SlowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool AirControl; // can the user control the direction that is being moved in the air

            // Reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float ShellOffset;
        }
        
        public Camera Camera;
        public MovementSettings Movement = new MovementSettings();
        public MouseLook MouseLook = new MouseLook();
        public AdvancedSettings Advanced = new AdvancedSettings();

        private Rigidbody _rigidBody;
        private CapsuleCollider _capsule;
        private float _yRotation;
        private Vector3 _groundContactNormal;
        private bool _jump, _previouslyGrounded;
        
        public Vector3 Velocity => _rigidBody.velocity;
        public bool Grounded { get; private set; }
        public bool Jumping { get; private set; }
        public bool Running => Movement.Running;
        
        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            MouseLook.Init(transform, Camera.transform);
        }
        
        private void Update()
        {
            RotateView();

            if (Input.GetButtonDown("Jump") && !_jump)
            {
                _jump = true;
            }
        }

        private void FixedUpdate()
        {
            GroundCheck();
            var input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) &&
                (Advanced.AirControl || Grounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                var desiredMove = Camera.transform.forward * input.y + Camera.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, _groundContactNormal).normalized;

                desiredMove.x *= Movement.CurrentTargetSpeed;
                desiredMove.z *= Movement.CurrentTargetSpeed;
                desiredMove.y *= Movement.CurrentTargetSpeed;
                if (_rigidBody.velocity.sqrMagnitude <
                    Movement.CurrentTargetSpeed * Movement.CurrentTargetSpeed)
                {
                    _rigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (Grounded)
            {
                _rigidBody.drag = 5f;

                if (_jump)
                {
                    _rigidBody.drag = 0f;
                    _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
                    _rigidBody.AddForce(new Vector3(0f, Movement.JumpForce, 0f), ForceMode.Impulse);
                    Jumping = true;
                }

                if (!Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon &&
                    _rigidBody.velocity.magnitude < 1f)
                {
                    _rigidBody.Sleep();
                }
            }
            else
            {
                _rigidBody.drag = 0f;
                if (_previouslyGrounded && !Jumping)
                {
                    StickToGroundHelper();
                }
            }

            _jump = false;
        }
        
        private float SlopeMultiplier()
        {
            var angle = Vector3.Angle(_groundContactNormal, Vector3.up);
            return Movement.SlopeCurveModifier.Evaluate(angle);
        }
        
        private void StickToGroundHelper()
        {
            if (!Physics.SphereCast(transform.position, _capsule.radius * (1.0f - Advanced.ShellOffset),
                Vector3.down, out var hitInfo,
                _capsule.height / 2f - _capsule.radius +
                Advanced.StickToGroundHelperDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore)) return;

            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                _rigidBody.velocity = Vector3.ProjectOnPlane(_rigidBody.velocity, hitInfo.normal);
            }
        }
        
        private Vector2 GetInput()
        {
            var input = new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
            Movement.UpdateDesiredTargetSpeed(input);
            return input;
        }

        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            var oldYRotation = transform.eulerAngles.y;

            MouseLook.LookRotation(transform, Camera.transform);

            if (!Grounded && !Advanced.AirControl) return;

            // Rotate the rigidbody velocity to match the new direction that the character is looking
            var velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            _rigidBody.velocity = velRotation * _rigidBody.velocity;
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            _previouslyGrounded = Grounded;

            if (Physics.SphereCast(transform.position, _capsule.radius * (1.0f - Advanced.ShellOffset),
                Vector3.down, out var hitInfo,
                _capsule.height / 2f - _capsule.radius + Advanced.GroundCheckDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore))
            {
                Grounded = true;
                _groundContactNormal = hitInfo.normal;
            }
            else
            {
                Grounded = false;
                _groundContactNormal = Vector3.up;
            }

            if (!_previouslyGrounded && Grounded && Jumping)
            {
                Jumping = false;
            }
        }
    }
}