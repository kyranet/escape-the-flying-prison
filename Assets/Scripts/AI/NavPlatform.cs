using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
	public class NavPlatform : MonoBehaviour, INavContainer
	{
		public List<NavJoint> Joints;
		public Bounds Bounds;

		[Serializable]
		public class NavPlatformJoint : INavContainer
		{
			/// <summary>
			/// The origin platform.
			/// </summary>
			public NavPlatform OriginPlatform;

			/// <summary>
			/// The target platform.
			/// </summary>
			public NavPlatform TargetPlatform;

			/// <summary>
			/// Entrance's position.
			/// </summary>
			public Vector3 InPosition;

			/// <summary>
			/// Exit's position.
			/// </summary>
			public Vector3 OutPosition;

			/// <summary>
			/// The bridge.
			/// </summary>
			public GameObject Bridge { private get; set; }

			/// <summary>
			/// Whether the joint is enabled.
			/// </summary>
			public bool Enabled => Bridge.activeSelf;

			/// <summary>
			/// The calculated distance from {InPosition} and {OutPosition}
			/// </summary>
			public float Distance => (InPosition - OutPosition).magnitude;

			public INavContainer Contains(Vector3 position)
			{
				var direction = OutPosition - InPosition;
				var line = direction.normalized;
				var v = position - InPosition;
				var d = Vector3.Dot(v, line);

				if (d < 0) return null;
				if (d > direction.magnitude) return null;

				var point = InPosition + line * d;
				var distance = (position - point).magnitude;
				if (distance > 1f) return null;

				Debug.DrawLine(point, position, Color.yellow, 1f);
				return this;
			}

			public IEnumerable<INavContainer> Siblings()
			{
				yield return OriginPlatform;
				yield return TargetPlatform;
			}

			public NavPlatform Platform => TargetPlatform;

			public Vector3? Closest(Vector3 position)
			{
				return (OutPosition - position).magnitude > (InPosition - position).magnitude
					? InPosition
					: OutPosition;
			}
		}

		public List<NavPlatformJoint> Connections;

		public Vector3 Position => transform.position;
		public Vector3 Center => Position + Bounds.center;

		public INavContainer Contains(Vector3 position)
		{
			if (Bounds.Contains(position - Position)) return this;

			Debug.DrawLine(position, position + Vector3.up, Color.green, 1f);
			return Connections.Select(connection => connection.Contains(position))
				.FirstOrDefault(instance => !(instance is null));
		}

		public IEnumerable<INavContainer> Siblings()
		{
			return Connections.Where(joint => joint.Enabled);
		}

		public NavPlatform Platform => this;

		public Vector3? Closest(Vector3 position)
		{
			return null;
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
						OriginPlatform = joint.A.Platform,
						TargetPlatform = joint.B.Platform,
						InPosition = joint.A.Position + joint.A.Platform.Position,
						OutPosition = joint.B.Position + joint.B.Platform.Position,
						Bridge = joint.Bridge
					});
				}
				else
				{
					Connections.Add(new NavPlatformJoint
					{
						OriginPlatform = joint.B.Platform,
						TargetPlatform = joint.A.Platform,
						InPosition = joint.B.Position + joint.B.Platform.Position,
						OutPosition = joint.A.Position + joint.A.Platform.Position,
						Bridge = joint.Bridge
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
