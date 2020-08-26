using System;

namespace Shared
{
    [Serializable]
    public class Health
    {
        public float Value = 100f;
        public bool Dead => Value == 0f;

        public void SetHealth(float value)
        {
            Value = Math.Max(value, 0f);
        }

        public void ReduceHealth(float value)
        {
            SetHealth(Value - value);
        }
    }
}
