using System.Linq;
using Ray1Map;
using UnityEngine;

public class SkeletonAnimationComponent : ObjectAnimationComponent
{
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

        int nextFrameIndex = frameInt + 1 * speed.Direction;

        if (nextFrameIndex >= framesCount)
        {
            switch (loopMode)
            {
                case AnimLoopMode.Repeat:
                    nextFrameIndex = 0;
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

        float lerpFactor = speed.CurrentFrame - frameInt;

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

    public void CombineAnimations()
    {
        var bonesCount = animations[0].bones.Length;
        var bones = new Bone[bonesCount];

        for (int boneIndex = 0; boneIndex < bonesCount; boneIndex++)
        {
            bones[boneIndex] = new Bone()
            {
                animatedTransform = animations[0].bones[boneIndex].animatedTransform,
                frames = animations.SelectMany(x => x.bones[boneIndex].frames).ToArray(),
            };
        }

        animations = new Animation[]
        {
            new Animation()
            {
                bones = bones
            }
        };
    }
}