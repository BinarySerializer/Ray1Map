using UnityEngine;

namespace Ray1Map
{
    public class AnimatedTransformComponent : ObjectAnimationComponent
    {
        public Transform animatedTransform;
        public Frame[] frames;

        public struct Frame
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public bool IsHidden;
        }

        protected override void UpdateAnimation()
        {
            if (frames == null || speed == null)
                return;

            speed.Update(frames.Length, loopMode);

            var frameInt = speed.CurrentFrameInt;

            var currentFrame = frames[frameInt];

            int nextFrameIndex = frameInt + 1 * speed.Direction;

            if (nextFrameIndex >= frames.Length)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.Repeat:
                        nextFrameIndex = 0;
                        break;

                    case AnimLoopMode.PingPong:
                        nextFrameIndex = frames.Length - 1;
                        break;
                }
            }
            else if (nextFrameIndex < 0)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.PingPong:
                        nextFrameIndex = 1;
                        break;
                }
            }

            var nextFrame = frames[nextFrameIndex];

            var lerpFactor = speed.CurrentFrame - frameInt;

            if (speed.Direction == -1)
                lerpFactor = 1 - lerpFactor;

            animatedTransform.localPosition = Vector3.Lerp(currentFrame.Position, nextFrame.Position, lerpFactor);
            animatedTransform.localRotation = Quaternion.Lerp(currentFrame.Rotation, nextFrame.Rotation, lerpFactor);

            if (currentFrame.IsHidden)
                animatedTransform.localScale = Vector3.zero;
            else
                animatedTransform.localScale = Vector3.Lerp(currentFrame.Scale, nextFrame.Scale, lerpFactor);
        }
    }
}