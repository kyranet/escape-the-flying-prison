using System;
using UnityEngine;

namespace AI
{
	public class NavJoint : MonoBehaviour
	{
		[Serializable]
		public struct NavJointValue
		{
			public Vector3 Position;
			public NavPlatform Platform;
		}

		[Serializable]
		public struct NavMeshSquare
		{
			public Vector3 TopLeft;
			public Vector3 TopRight;
			public Vector3 BottomLeft;
			public Vector3 BottomRight;
		}

		[Serializable]
		public struct NavMesh
		{
			public NavMeshSquare A;
			public NavMeshSquare B;
		}

		public NavJointValue A;
		public NavJointValue B;
		public NavMesh NavHitBox;
		public GameObject Bridge;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;

			var a = A.Platform.transform.position + A.Position;
			var b = B.Platform.transform.position + B.Position;

			Gizmos.DrawWireSphere(a, 0.2f);
			Gizmos.DrawWireSphere(b, 0.2f);

			Gizmos.DrawLine(a, b);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(NavHitBox.A.BottomLeft, NavHitBox.A.TopLeft);
			Gizmos.DrawLine(NavHitBox.A.BottomLeft, NavHitBox.A.BottomRight);
			Gizmos.DrawLine(NavHitBox.A.TopRight, NavHitBox.A.TopLeft);
			Gizmos.DrawLine(NavHitBox.A.TopRight, NavHitBox.A.BottomRight);

			Gizmos.DrawLine(NavHitBox.B.BottomLeft, NavHitBox.B.TopLeft);
			Gizmos.DrawLine(NavHitBox.B.BottomLeft, NavHitBox.B.BottomRight);
			Gizmos.DrawLine(NavHitBox.B.TopRight, NavHitBox.B.TopLeft);
			Gizmos.DrawLine(NavHitBox.B.TopRight, NavHitBox.B.BottomRight);

			Gizmos.DrawLine(NavHitBox.A.TopLeft, NavHitBox.B.TopLeft);
			Gizmos.DrawLine(NavHitBox.A.TopRight, NavHitBox.B.TopRight);
			Gizmos.DrawLine(NavHitBox.A.BottomLeft, NavHitBox.B.BottomLeft);
			Gizmos.DrawLine(NavHitBox.A.BottomRight, NavHitBox.B.BottomRight);
		}
	}
}
