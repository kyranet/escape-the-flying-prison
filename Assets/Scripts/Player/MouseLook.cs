using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	[Serializable]
	public class MouseLook
	{
		public float XSensitivity = 2f;
		public float YSensitivity = 2f;
		public bool ClampVerticalRotation = true;
		public float MinimumX = -90F;
		public float MaximumX = 90F;
		public bool Smooth;
		public float SmoothTime = 5f;
		public bool LockCursor = true;

		public InputAction AxisAction;
		private Vector2 _axis;

		private Quaternion _characterTargetRot;
		private Quaternion _cameraTargetRot;
		private bool _cursorIsLocked = true;

		public void Init(Transform character, Transform camera)
		{
			_characterTargetRot = character.localRotation;
			_cameraTargetRot = camera.localRotation;
			AxisAction.Enable();

			AxisAction.started += HandleAxis;
			AxisAction.performed += HandleAxis;
			AxisAction.canceled += HandleAxis;
		}

		private void HandleAxis(InputAction.CallbackContext ctx)
		{
			_axis = ctx.ReadValue<Vector2>();
		}

		public void LookRotation(Transform character, Transform camera)
		{
			var yRot = _axis.x * XSensitivity;
			var xRot = _axis.y * YSensitivity;

			_characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
			_cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

			if (ClampVerticalRotation)
				_cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

			if (Smooth)
			{
				character.localRotation = Quaternion.Slerp(character.localRotation, _characterTargetRot,
					SmoothTime * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp(camera.localRotation, _cameraTargetRot,
					SmoothTime * Time.deltaTime);
			}
			else
			{
				character.localRotation = _characterTargetRot;
				camera.localRotation = _cameraTargetRot;
			}

			UpdateCursorLock();
		}

		public void SetCursorLock(bool value)
		{
			LockCursor = value;
			if (LockCursor) return;

			// Force unlock the cursor if the user disable the cursor locking helper
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		public void UpdateCursorLock()
		{
			// If the user set "lockCursor" we check & properly lock the cursor
			if (LockCursor)
				InternalLockUpdate();
		}

		private void InternalLockUpdate()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				_cursorIsLocked = false;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				_cursorIsLocked = true;
			}

			if (_cursorIsLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else if (!_cursorIsLocked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		private Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;

			var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

			angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

			q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

			return q;
		}
	}
}
