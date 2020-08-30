using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Joint = AI.NavPlatform.NavPlatformJoint;

namespace AI
{
    public class NavGenerator : MonoBehaviour
    {
        [CanBeNull] public NavPlatform Entry;
        public List<NavPlatform> Platforms;

        private void Awake()
        {
            Scan();
            // GetRoute(new Vector3(17, 2, 0), new Vector3(30, -7, -28));
        }

        public void Scan()
        {
            if (Entry is null) return;

            Platforms.Clear();
            ScanPlatform(Entry);
        }

        [CanBeNull]
        public NavPlatform GetPlatform(Vector3 position)
        {
            return Platforms.FirstOrDefault(platform => platform.Contains(position));
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
            if (pa == pb || GetPossibleRoute(pa, new List<NavPlatform>(), pb, ref stack, out _)) return stack;

            // Return empty stack:
            return new Stack<Vector3>();
        }

        private bool GetPossibleRoute(NavPlatform platform, ICollection<NavPlatform> scanned, NavPlatform target,
            ref Stack<Vector3> stack, out float distance)
        {
            // Add the entry to the scan to avoid cyclic scans:
            scanned.Add(platform);

            distance = float.MaxValue;
            Joint? shortestPlatform = null;

            foreach (var joint in platform.Connections.Where(joint => !scanned.Contains(joint.Platform)))
            {
                // Check whether the platform we are scanning is the target:
                if (joint.Platform == target)
                {
                    shortestPlatform = joint;
                    break;
                }

                // If no route was found, skip:
                if (!GetPossibleRoute(joint.Platform, scanned, target, ref stack, out var possibleDistance))
                    continue;

                // If the route was longer than the previous, skip:
                if (possibleDistance >= distance) continue;

                // Set the new shortest route:
                distance = possibleDistance;
                shortestPlatform = joint;
            }

            if (!shortestPlatform.HasValue) return false;

            if (stack.Count != 1) stack.Push(shortestPlatform.Value.OutPosition);
            stack.Push(shortestPlatform.Value.InPosition);
            distance = shortestPlatform.Value.Distance;
            return true;
        }

        private void ScanPlatform([CanBeNull] NavPlatform platform)
        {
            if (platform is null) return;

            Platforms.Add(platform);
            platform.Refresh();

            foreach (var joint in platform.Connections.Where(joint => !Platforms.Contains(joint.Platform)))
            {
                ScanPlatform(joint.Platform);
            }
        }
    }
}
