using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// An animation speed where the speed is the amount to increase the animation frame by each second
    /// </summary>
    public class AnimSpeed_SecondIncrease : AnimSpeedWithValue
    {
        protected override float GetFrameChange() => Time.deltaTime * Speed;
    }
}