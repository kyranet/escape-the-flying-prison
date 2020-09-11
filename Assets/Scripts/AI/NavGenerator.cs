using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Joint = AI.NavPlatform.NavPlatformJoint;
using Platform = AI.NavPlatform;

namespace AI
{
	public class NavGenerator : MonoBehaviour
	{
		[CanBeNull]
		public NavPlatform Entry;

		public List<NavPlatform> Platforms;

		private void Awake()
		{
			Scan();
		}

		public void Scan()
		{
			if (Entry is null) return;

			Platforms.Clear();
			ScanPlatform(Entry);
		}

		[CanBeNull]
		public INavContainer GetPlatform(Vector3 position)
		{
			return Platforms.Select(platform => platform.Contains(position))
				.FirstOrDefault(platform => !(platform is null));
		}

		public Stack<Vector3> GetRoute(Vector3 a, Vector3 b)
		{
			if ((a - b).sqrMagnitude < 0.1f) return new Stack<Vector3>();

			var pa = GetPlatform(a);
			if (pa is null) return new Stack<Vector3>();

			var pb = GetPlatform(b);
			if (pb is null) return new Stack<Vector3>();

			var stack = new Stack<Vector3>();
			stack.Push(b);

			// Return empty stack:
			if (pa != pb && !GetPossibleRoute(pa, new List<INavContainer>(), pb, ref stack, out _))
				return new Stack<Vector3>();

			var closes = pa.Closest(stack.Peek());
			if (closes.HasValue) stack.Push(closes.Value);
			return stack;
		}

		private bool GetPossibleRoute(INavContainer platform, ICollection<INavContainer> scanned, INavContainer target,
			ref Stack<Vector3> stack, out float distance)
		{
			// Add the entry to the scan to avoid cyclic scans:
			scanned.Add(platform);

			distance = float.MaxValue;
			INavContainer shortestPlatform = null;

			foreach (var joint in platform.Siblings().Where(joint => !scanned.Contains(joint)))
			{
				// Check whether the platform we are scanning is the target:
				if (joint == target)
				{
					shortestPlatform = joint;
					break;
				}

				// If no route was found, skip:
				if (!GetPossibleRoute(joint, scanned, target, ref stack, out var possibleDistance))
					continue;

				// If the route was longer than the previous, skip:
				if (possibleDistance >= distance) continue;

				// Set the new shortest route:
				distance = possibleDistance;
				shortestPlatform = joint;
			}

			switch (shortestPlatform)
			{
				case null:
					return false;
				case Joint casted:
					stack.Push(casted.OutPosition);
					stack.Push(casted.InPosition);
					distance = casted.Distance;
					break;
				case NavPlatform casted:
					distance = (stack.Peek() - casted.Center).magnitude;
					stack.Push(casted.Center);
					break;
			}

			return true;
		}

		private void ScanPlatform([CanBeNull] NavPlatform platform)
		{
			if (platform is null) return;

			Platforms.Add(platform);
			platform.Refresh();

			foreach (var joint in platform.Connections.Where(joint => !Platforms.Contains(joint.TargetPlatform)))
			{
				ScanPlatform(joint.TargetPlatform);
			}
		}
	}
}
