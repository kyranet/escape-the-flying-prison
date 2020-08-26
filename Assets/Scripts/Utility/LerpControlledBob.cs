using System;
using System.Collections;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class LerpControlledBob
    {
        public float BobDuration;
        public float BobAmount;

        private float _offset;

        // Provides the offset that can be used
        public float Offset()
        {
            return _offset;
        }

        public IEnumerator DoBobCycle()
        {
            // Make the camera move down slightly
            var t = 0f;
            while (t < BobDuration)
            {
                _offset = Mathf.Lerp(0f, BobAmount, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Make it move back to neutral
            t = 0f;
            while (t < BobDuration)
            {
                _offset = Mathf.Lerp(BobAmount, 0f, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            _offset = 0f;
        }
    }
}
