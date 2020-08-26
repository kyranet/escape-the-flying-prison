using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class AdvancedSettings
    {
        // Distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float GroundCheckDistance = 0.01f;

        public float StickToGroundHelperDistance = 0.5f; // stops the character
        public float SlowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
        public bool AirControl; // can the user control the direction that is being moved in the air

        // Reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        [Tooltip("set it to 0.1 or more if you get stuck in wall")]
        public float ShellOffset;
    }
}
