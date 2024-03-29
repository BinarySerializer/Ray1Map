﻿using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// An animation speed where the speed is the number of game frames to wait between each animation frame
    /// </summary>
    public class AnimSpeed_FrameDelay : AnimSpeedSingle
    {
        public AnimSpeed_FrameDelay() { }
        public AnimSpeed_FrameDelay(float speed)
        {
            Speed = speed;
        }

        protected override float GetFrameChange() => Time.deltaTime * (LevelEditorData.FramesPerSecond / Speed);
    }
}