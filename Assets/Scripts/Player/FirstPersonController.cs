using System;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool IsWalking;
        [SerializeField] private float WalkSpeed;
        [SerializeField] private float RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float RunstepLenghten;
        [SerializeField] private float JumpSpeed;
        [SerializeField] private float StickToGroundForce;
        [SerializeField] private float GravityMultiplier;
        [SerializeField] private MouseLook MouseLook;
        [SerializeField] private bool UseFovKick;
        [SerializeField] private FOVKick FovKick = new FOVKick();
        [SerializeField] private bool UseHeadBob;
        [SerializeField] private CurveControlledBob HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob JumpBob = new LerpControlledBob();
        [SerializeField] private float StepInterval;

        [SerializeField]
        private AudioClip[] FootstepSounds; // an array of footstep sounds that will be randomly selected from.

        [SerializeField] private AudioClip JumpSound; // the sound played when character leaves the ground.
        [SerializeField] private AudioClip LandSound; // the sound played when character touches back on ground.

        private Transform _camera;
        private bool _jump;
        private float _yRotation;
        private Vector2 _input;
        private Vector3 _moveDir = Vector3.zero;
        private CharacterController _characterController;
        private CollisionFlags _collisionFlags;
        private bool _previouslyGrounded;
        private Vector3 _originalCameraPosition;
        private float _stepCycle;
        private float _nextStep;
        private bool _jumping;
        private AudioSource _audioSource;

        // Use this for initialization
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            var mainCamera = Camera.main;
            if (mainCamera is null)
                throw new Exception(
                    "FOVKick Increase curve is null, please define the curve for the field of view kicks");
            
            FovKick.Setup(mainCamera);
            HeadBob.Setup(mainCamera, StepInterval);

            _camera = mainCamera.transform;
            _originalCameraPosition = _camera.localPosition;
            _stepCycle = 0f;
            _nextStep = _stepCycle / 2f;
            _jumping = false;
            _audioSource = GetComponent<AudioSource>();
            MouseLook.Init(transform, _camera);
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (!_jump)
            {
                _jump = Input.GetKeyDown("Jump");
            }

            if (!_previouslyGrounded && _characterController.isGrounded)
            {
                StartCoroutine(JumpBob.DoBobCycle());
                PlayLandingSound();
                _moveDir.y = 0f;
                _jumping = false;
            }

            if (!_characterController.isGrounded && !_jumping && _previouslyGrounded)
            {
                _moveDir.y = 0f;
            }

            _previouslyGrounded = _characterController.isGrounded;
        }

        private void PlayLandingSound()
        {
            _audioSource.clip = LandSound;
            _audioSource.Play();
            _nextStep = _stepCycle + .5f;
        }

        private void FixedUpdate()
        {
            GetInput(out var speed);

            // always move along the camera forward as it is the direction that it being aimed at
            var desiredMove = transform.forward * _input.y + transform.right * _input.x;

            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out var hitInfo,
                _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            _moveDir.x = desiredMove.x * speed;
            _moveDir.z = desiredMove.z * speed;

            if (_characterController.isGrounded)
            {
                _moveDir.y = -StickToGroundForce;

                if (_jump)
                {
                    _moveDir.y = JumpSpeed;
                    PlayJumpSound();
                    _jump = false;
                    _jumping = true;
                }
            }
            else
            {
                _moveDir += Physics.gravity * (GravityMultiplier * Time.fixedDeltaTime);
            }

            _collisionFlags = _characterController.Move(_moveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            MouseLook.UpdateCursorLock();
        }

        private void PlayJumpSound()
        {
            _audioSource.clip = JumpSound;
            _audioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (_characterController.velocity.sqrMagnitude > 0 && (_input.x != 0 || _input.y != 0))
            {
                _stepCycle += (_characterController.velocity.magnitude + speed * (IsWalking ? 1f : RunstepLenghten)) *
                              Time.fixedDeltaTime;
            }

            if (!(_stepCycle > _nextStep))
            {
                return;
            }

            _nextStep = _stepCycle + StepInterval;

            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!_characterController.isGrounded)
            {
                return;
            }

            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            var n = Random.Range(1, FootstepSounds.Length);
            _audioSource.clip = FootstepSounds[n];
            _audioSource.PlayOneShot(_audioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            FootstepSounds[n] = FootstepSounds[0];
            FootstepSounds[0] = _audioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!UseHeadBob)
            {
                return;
            }

            if (_characterController.velocity.magnitude > 0 && _characterController.isGrounded)
            {
                _camera.localPosition =
                    HeadBob.DoHeadBob(_characterController.velocity.magnitude +
                                      speed * (IsWalking ? 1f : RunstepLenghten));
                newCameraPosition = _camera.localPosition;
                newCameraPosition.y = _camera.localPosition.y - JumpBob.Offset();
            }
            else
            {
                newCameraPosition = _camera.localPosition;
                newCameraPosition.y = _originalCameraPosition.y - JumpBob.Offset();
            }

            _camera.localPosition = newCameraPosition;
        }

        private void GetInput(out float speed)
        {
            // Read input
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var wasWalking = IsWalking;

            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            IsWalking = !Input.GetKey(KeyCode.LeftShift);

            // set the desired speed to be walking or running
            speed = IsWalking ? WalkSpeed : RunSpeed;
            _input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (_input.sqrMagnitude > 1)
            {
                _input.Normalize();
            }

            // Handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fov kick is to be used
            if (IsWalking == wasWalking || !UseFovKick || !(_characterController.velocity.sqrMagnitude > 0)) return;

            StopAllCoroutines();
            StartCoroutine(IsWalking ? FovKick.FOVKickDown() : FovKick.FOVKickUp());
        }

        private void RotateView()
        {
            MouseLook.LookRotation(transform, _camera);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var body = hit.collider.attachedRigidbody;

            // Do not move the rigidbody if the character is on top of it
            if (_collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }

            body.AddForceAtPosition(_characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}