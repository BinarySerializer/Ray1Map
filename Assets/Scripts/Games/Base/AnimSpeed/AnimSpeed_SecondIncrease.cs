using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// An animation speed where the speed is the amount to increase the animation frame by each second
    /// </summary>
    public class AnimSpeed_SecondIncrease : AnimSpeedSingle
    {
        public AnimSpeed_SecondIncrease() { }
        public AnimSpeed_SecondIncrease(float speed)
        {
            Speed = speed;
        }

        protected override float GetFrameChange() => Time.deltaTime * Speed;
    }
}