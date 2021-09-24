using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// An animation speed where the speed is the number of game frames to wait between each animation frame
    /// </summary>
    public class AnimSpeed_FrameDelay : AnimSpeedWithValue
    {
        protected override float GetFrameChange() => Time.deltaTime * (LevelEditorData.FramesPerSecond / Speed);
    }
}