using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// An animation speed where the speed is the amount to increase the animation frame by each game frame
    /// </summary>
    public class AnimSpeed_FrameIncrease : AnimSpeedWithValue
    {
        protected override float GetFrameChange() => Time.deltaTime * LevelEditorData.FramesPerSecond * Speed;
    }
}