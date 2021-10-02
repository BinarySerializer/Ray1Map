using System;
using R1Engine;
using UnityEngine;

public class AnimatedTransformComponent : MonoBehaviour
{
    public Transform animatedTransform;
    public Frame[] frames;
    public AnimSpeed speed = new AnimSpeed_FrameDelay(1);
    public AnimLoopMode loopMode = AnimLoopMode.Repeat;

    public struct Frame 
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public bool IsHidden;
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.LoadState != Controller.State.Finished || !Settings.AnimateTiles) 
            return;

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

        transform.localPosition = Vector3.Lerp(currentFrame.Position, nextFrame.Position, lerpFactor);
        transform.localRotation = Quaternion.Lerp(currentFrame.Rotation, nextFrame.Rotation, lerpFactor);

        if (currentFrame.IsHidden)
            transform.localScale = Vector3.zero;
        else
            transform.localScale = Vector3.Lerp(currentFrame.Scale, nextFrame.Scale, lerpFactor);
    }
}