using System.Linq;
using Ray1Map;
using UnityEngine;

public class SkeletonAnimationComponent : ObjectAnimationComponent
{
    public bool autoPlay = true;

    public Animation[] animations;
    public int animIndex;

    public Animation CurrentAnimation => animations[animIndex];

    public struct Animation
    {
        public Bone[] bones;
        public int FramesCount => bones?.FirstOrDefault().frames?.Length ?? 0;
    }

    public struct Bone
    {
        public Transform animatedTransform;
        public Frame[] frames;
    }

    public struct Frame 
    {
        public Vector3? Position;
        public Quaternion? Rotation;
        public Vector3 Scale;
        public bool IsHidden;
    }

    protected override void UpdateAnimation()
    {
        if (animations == null || speed == null)
            return;

        Animation anim = CurrentAnimation;
        int framesCount = anim.FramesCount;

        speed.Update(framesCount, loopMode);

        int frameInt = speed.CurrentFrameInt;

        // Check if the animation looped back to the start and if a new one should play instead
        if (autoPlay && animations.Length > 1 && speed.CurrentFrame == 0)
        {
            speed.Reset();
            frameInt = 0;
            animIndex = (animIndex + 1) % animations.Length;
            anim = CurrentAnimation;
            framesCount = anim.FramesCount;
        }

        int nextFrameIndex = frameInt + 1 * speed.Direction;

        bool interpolate = true;

        if (nextFrameIndex >= framesCount)
        {
            switch (loopMode)
            {
                case AnimLoopMode.Repeat:
                    nextFrameIndex = 0;
                    interpolate = false;
                    break;

                case AnimLoopMode.PingPong:
                    nextFrameIndex = framesCount - 1;
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

        float lerpFactor = interpolate ? speed.CurrentFrame - frameInt : 0;

        if (interpolate && speed.Direction == -1)
            lerpFactor = 1 - lerpFactor;

        foreach (Bone bone in anim.bones)
        {
            Frame currentFrame = bone.frames[frameInt];
            Frame nextFrame = bone.frames[nextFrameIndex];

            if (currentFrame.Position != null && nextFrame.Position != null)
                bone.animatedTransform.localPosition = Vector3.Lerp(currentFrame.Position.Value, nextFrame.Position.Value, lerpFactor);

            if (currentFrame.Rotation != null && nextFrame.Rotation != null)
                bone.animatedTransform.localRotation = Quaternion.Lerp(currentFrame.Rotation.Value, nextFrame.Rotation.Value, lerpFactor);

            if (currentFrame.IsHidden)
                bone.animatedTransform.localScale = Vector3.zero;
            else
                bone.animatedTransform.localScale = Vector3.Lerp(currentFrame.Scale, nextFrame.Scale, lerpFactor);
        }
    }
}