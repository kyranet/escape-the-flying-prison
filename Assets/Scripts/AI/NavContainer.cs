using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
	public interface INavContainer
	{
		[CanBeNull]
		INavContainer Contains(Vector3 position);

		IEnumerable<INavContainer> Siblings();

		NavPlatform Platform { get; }
		Vector3? Closest(Vector3 position);
	}
}
