using R1Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTransformComponent : MonoBehaviour
{
    public Transform animatedTransform;
    public Frame[] frames;
    public float speed = 1f; // In frames, for 60FPS
    public float currentFrame;

    public struct Frame {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public void Set(Transform transform) {
            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale = Scale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Controller.LoadState != Controller.State.Finished) return;
        if(!Settings.AnimateTiles) return;
        if (frames != null && speed != 0) {
            int curFrame = Mathf.FloorToInt(currentFrame);
            currentFrame += Time.deltaTime * (LevelEditorData.FramesPerSecond / speed);
            if(currentFrame >= frames.Length) currentFrame = 0;
            int newFrame = Mathf.FloorToInt(currentFrame);
            if (newFrame != curFrame) {
                frames[newFrame].Set(animatedTransform);
            }
        }
    }
}
