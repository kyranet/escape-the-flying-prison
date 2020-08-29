using System;
using UnityEngine;
using Utility;

namespace Shared
{
    [RequireComponent(typeof(GroundDetector))]
    public class FallDamage : MonoBehaviour
    {
        public bool IsAlive = true;
        public float DamageMultiplier = 0.1f;

        [Tooltip("Whether the component should call Destroy() on death.")]
        public bool DestroyItself = false;

        [Range(-20f, 0f)] [Tooltip("Velocity at which the player will receive damage.")]
        public float MinimumVelocityCrash = -4f;

        private GroundDetector _gd;
        [SerializeField] private Health Health = new Health();
        private float _distanceY;
        private float _positionY;

        private void Start()
        {
            _gd = GetComponent<GroundDetector>();
        }

        private void OnEnable()
        {
            _distanceY = 0f;
            _positionY = transform.position.y;
        }

        private void FixedUpdate()
        {
            if (_gd.Grounded)
            {
                _positionY = transform.position.y;

                if (_distanceY >= 0f) return;

                DealDamage();
                _distanceY = 0f;
            }
            else
            {
                var oldPosition = _positionY;
                _positionY = transform.position.y;
                _distanceY += _positionY - oldPosition;
            }
        }

        private void DealDamage()
        {
            if (_distanceY >= MinimumVelocityCrash) return;

            var damage = Mathf.Pow(-_distanceY, DamageMultiplier);
            Health.ReduceHealth(damage);

            if (!DestroyItself || !Health.Dead) return;

            IsAlive = false;
            Destroy(gameObject);
        }
    }
}
