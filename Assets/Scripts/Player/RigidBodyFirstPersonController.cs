using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Utility;

namespace Player
{
	[RequireComponent(typeof(GroundDetector))]
	[RequireComponent(typeof(AudioSource))]
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

			[HideInInspector]
			public float CurrentTargetSpeed = 8f;

			public void UpdateDesiredTargetSpeed(Vector2 input)
			{
				if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					// Strafe
					CurrentTargetSpeed = StrafeSpeed;
				}

				if (input.y < 0)
				{
					// Backwards
					CurrentTargetSpeed = BackwardSpeed;
				}

				if (input.y > 0)
				{
					// Forwards
					// Handled last as if strafing and moving forward at the same time forwards speed should take precedence
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

		public Camera Camera;
		public MovementSettings Movement = new MovementSettings();
		public MouseLook MouseLook = new MouseLook();
		public AdvancedSettings Advanced = new AdvancedSettings();
		public float StepLength = 1f;

		public InputAction MoveAction;
		private Vector2 _movement;

		public InputAction JumpAction;

		private Rigidbody _rigidBody;
		private CapsuleCollider _capsule;
		private GroundDetector _groundDetector;
		private float _yRotation;
		private bool _jump, _previouslyGrounded;
		private float _lastStep;

		public Vector3 Velocity => _rigidBody.velocity;
		public bool Jumping { get; private set; }
		public bool Running => Movement.Running;

		[SerializeField]
		private AudioClip[] FootstepSounds; // an array of footstep sounds that will be randomly selected from.

		[SerializeField]
		private AudioClip JumpSound; // the sound played when character leaves the ground.

		[SerializeField]
		private AudioClip LandSound; // the sound played when character touches back on ground.

		private AudioSource _audioSource;

		private void Start()
		{
			_rigidBody = GetComponent<Rigidbody>();
			_capsule = GetComponent<CapsuleCollider>();
			_audioSource = GetComponent<AudioSource>();
			_groundDetector = GetComponent<GroundDetector>();
			MouseLook.Init(transform, Camera.transform);

			MoveAction.Enable();
			MoveAction.started += HandleMovement;
			MoveAction.performed += HandleMovement;
			MoveAction.canceled += HandleStopMovement;

			JumpAction.Enable();
			JumpAction.started += HandleJump;
		}

		private void Update()
		{
			RotateView();
		}

		private void HandleJump(InputAction.CallbackContext ctx)
		{
			_jump = true;
		}

		private void HandleMovement(InputAction.CallbackContext ctx)
		{
			_movement = ctx.ReadValue<Vector2>();
			if (_movement.sqrMagnitude < 0.1f) _movement = Vector2.zero;
		}

		private void HandleStopMovement(InputAction.CallbackContext ctx)
		{
			_movement = Vector2.zero;
		}

		private void FixedUpdate()
		{
			var input = _movement;

			if (_movement != Vector2.zero &&
			    (Advanced.AirControl || _groundDetector.Grounded))
			{
				// always move along the camera forward as it is the direction that it being aimed at
				var desiredMove = Camera.transform.forward * input.y + Camera.transform.right * input.x;
				desiredMove = Vector3.ProjectOnPlane(desiredMove, _groundDetector.GroundContactNormal).normalized;

				desiredMove.x *= Movement.CurrentTargetSpeed;
				desiredMove.z *= Movement.CurrentTargetSpeed;
				desiredMove.y *= Movement.CurrentTargetSpeed;
				if (_rigidBody.velocity.sqrMagnitude <
				    Movement.CurrentTargetSpeed * Movement.CurrentTargetSpeed)
				{
					_rigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
				}

				PlayFootStepAudio();
			}

			if (_groundDetector.Grounded)
			{
				_rigidBody.drag = 5f;

				if (_jump)
				{
					_rigidBody.drag = 0f;
					_rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
					_rigidBody.AddForce(new Vector3(0f, Movement.JumpForce, 0f), ForceMode.Impulse);
					Jumping = true;
					PlayJumpSound();
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
			var angle = Vector3.Angle(_groundDetector.GroundContactNormal, Vector3.up);
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

		private void PlayLandingSound()
		{
			_audioSource.clip = LandSound;
			_audioSource.Play();
		}

		private void PlayJumpSound()
		{
			_audioSource.clip = JumpSound;
			_audioSource.Play();
		}

		private void PlayFootStepAudio()
		{
			if (!_groundDetector.Grounded)
			{
				_lastStep = 0f;
				return;
			}

			_lastStep += Time.fixedDeltaTime * (Running ? 1.5f : 1f);
			if (_lastStep < StepLength)
			{
				return;
			}

			_lastStep -= StepLength;

			// pick & play a random footstep sound from the array,
			// excluding sound at index 0
			var n = UnityEngine.Random.Range(1, FootstepSounds.Length);
			_audioSource.clip = FootstepSounds[n];
			_audioSource.PlayOneShot(_audioSource.clip);
			// move picked sound to index 0 so it's not picked next time
			FootstepSounds[n] = FootstepSounds[0];
			FootstepSounds[0] = _audioSource.clip;
		}

		private void RotateView()
		{
			//avoids the mouse looking if the game is effectively paused
			if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

			// get the rotation before it's changed
			var oldYRotation = transform.eulerAngles.y;

			MouseLook.LookRotation(transform, Camera.transform);

			if (!_groundDetector.Grounded && !Advanced.AirControl) return;

			// Rotate the rigidbody velocity to match the new direction that the character is looking
			var velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
			_rigidBody.velocity = velRotation * _rigidBody.velocity;
		}
	}
}
