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

        public void Set(Transform transform) {
            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale = Scale;

            if (IsHidden)
                transform.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.LoadState != Controller.State.Finished || !Settings.AnimateTiles) 
            return;

        if (frames == null || speed == null)
            return;

        if (speed.Update(frames.Length, loopMode))
            frames[speed.CurrentFrameInt].Set(animatedTransform);
    }
}