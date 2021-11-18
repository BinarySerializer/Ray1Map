using UnityEngine;

namespace Ray1Map
{
    public class AnimatedTransformComponent : ObjectAnimationComponent
    {
        public Transform animatedTransform;

        public bool autoPlay = true;

        public Animation[] animations;
        public int animIndex;

        public Animation CurrentAnimation => animations[animIndex];

        public struct Animation
        {
            public Frame[] frames;
        }

        public struct Frame
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public bool IsHidden;
        }

        protected override void UpdateAnimation()
        {
            if (animations == null || speed == null)
                return;

            speed.Update(CurrentAnimation.frames.Length, loopMode);

            int frameInt = speed.CurrentFrameInt;

            // Check if the animation looped back to the start and if a new one should play instead
            if (autoPlay && animations.Length > 1 && speed.CurrentFrame == 0)
            {
                speed.Reset();
                frameInt = 0;
                animIndex = (animIndex + 1) % animations.Length;
            }

            Frame currentFrame = CurrentAnimation.frames[frameInt];

            int nextFrameIndex = frameInt + 1 * speed.Direction;

            bool interpolate = true;

            if (nextFrameIndex >= CurrentAnimation.frames.Length)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.Repeat:
                        nextFrameIndex = 0;
                        interpolate = false;
                        break;

                    case AnimLoopMode.PingPong:
                        nextFrameIndex = CurrentAnimation.frames.Length - 1;
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

            Frame nextFrame = CurrentAnimation.frames[nextFrameIndex];

            float lerpFactor = interpolate ? speed.CurrentFrame - frameInt : 0;

            if (interpolate && speed.Direction == -1)
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