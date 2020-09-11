using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	[RequireComponent(typeof(AgentCreator))]
	public class Shoot : MonoBehaviour
	{
		public GameObject Laser;
		public MonoBehaviour Controller;
		private AgentCreator _ac;

		public InputAction FireAction;

		private void Awake()
		{
			_ac = GetComponent<AgentCreator>();

			FireAction.Enable();

			FireAction.started += FireEnable;
			FireAction.canceled += FireDisable;
		}

		private void OnDestroy()
		{
			FireAction.Disable();

			FireAction.started -= FireEnable;
			FireAction.canceled -= FireDisable;
		}

		private void FireEnable(InputAction.CallbackContext _)
		{
			Laser.SetActive(true);
			Controller.enabled = false;
			_ac.Shoot();
		}

		private void FireDisable(InputAction.CallbackContext _)
		{
			Laser.SetActive(false);
			Controller.enabled = true;
		}
	}
}
