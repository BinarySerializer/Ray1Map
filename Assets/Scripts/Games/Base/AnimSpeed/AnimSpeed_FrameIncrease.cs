using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// An animation speed where the speed is the amount to increase the animation frame by each game frame
    /// </summary>
    public class AnimSpeed_FrameIncrease : AnimSpeedSingle
    {
        public AnimSpeed_FrameIncrease() { }
        public AnimSpeed_FrameIncrease(float speed)
        {
            Speed = speed;
        }

        protected override float GetFrameChange() => Time.deltaTime * LevelEditorData.FramesPerSecond * Speed;
    }
}