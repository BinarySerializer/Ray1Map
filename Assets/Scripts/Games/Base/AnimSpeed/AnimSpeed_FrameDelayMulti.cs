using UnityEngine;

namespace Ray1Map
{
    public class AnimSpeed_FrameDelayMulti : AnimSpeedMulti
    {
        public AnimSpeed_FrameDelayMulti() { }
        public AnimSpeed_FrameDelayMulti(float[] speeds)
        {
            Speeds = speeds;
        }

        protected override float GetFrameChange() => Time.deltaTime * (LevelEditorData.FramesPerSecond / Speeds[CurrentFrameInt]);
    }
}