using Agent;
using JetBrains.Annotations;
using UnityEngine;

namespace Map
{
	public class MapController : MonoBehaviour
	{
		public AgentManager AgentManager;
		public GameObject[] Buttons;
		public short ConnectionIndex = -1;

		[CanBeNull]
		public GameObject Current => ConnectionIndex == -1 ? null : Buttons[ConnectionIndex];

		public void Awake()
		{
			if (ConnectionIndex == -1 && Buttons.Length != 0) ConnectionIndex = 0;
			ReTarget();
		}

		public void Advance()
		{
			// If there were no connections, skip.
			if (ConnectionIndex == -1) return;

			// If this was the last connection, set index to -1 so [[MapController.Current]] returns null.
			if (++ConnectionIndex == Buttons.Length) ConnectionIndex = -1;

			// Retarget
			ReTarget();
		}

		private void ReTarget()
		{
			AgentManager.Target = Current != null ? Current.transform : null;
		}
	}
}
