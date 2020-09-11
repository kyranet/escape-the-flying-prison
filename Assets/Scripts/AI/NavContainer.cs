using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
	public interface INavContainer
	{
		[CanBeNull]
		INavContainer Contains(Vector3 position);

		NavPlatform Platform { get; }
		Vector3? Exit { get; }
	}
}
