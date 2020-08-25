using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class CurveControlledBob
    {
        public float HorizontalBobRange = 0.33f;
        public float VerticalBobRange = 0.33f;

        public AnimationCurve BobCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
            new Keyframe(2f, 0f)); // sin curve for head bob

        public float VerticalToHorizontalRatio = 1f;

        private float _cyclePositionX;
        private float _cyclePositionY;
        private float _bobBaseInterval;
        private float _time;
        private Vector3 _originalCameraPosition;

        public void Setup(Camera camera, float bobBaseInterval)
        {
            _bobBaseInterval = bobBaseInterval;
            _originalCameraPosition = camera.transform.localPosition;

            // get the length of the curve in time
            _time = BobCurve[BobCurve.length - 1].time;
        }
        
        public Vector3 DoHeadBob(float speed)
        {
            var xPos = _originalCameraPosition.x + BobCurve.Evaluate(_cyclePositionX) * HorizontalBobRange;
            var yPos = _originalCameraPosition.y + BobCurve.Evaluate(_cyclePositionY) * VerticalBobRange;

            _cyclePositionX += speed * Time.deltaTime / _bobBaseInterval;
            _cyclePositionY += speed * Time.deltaTime / _bobBaseInterval * VerticalToHorizontalRatio;

            if (_cyclePositionX > _time)
            {
                _cyclePositionX -= _time;
            }

            if (_cyclePositionY > _time)
            {
                _cyclePositionY -= _time;
            }

            return new Vector3(xPos, yPos, 0f);
        }
    }
}