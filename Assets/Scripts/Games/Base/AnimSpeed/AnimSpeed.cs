using System;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// Handles updating the frame of a generic animation
    /// </summary>
    public abstract class AnimSpeed : ICloneable
    {
        public float CurrentFrame { get; private set; }
        public int CurrentFrameInt => Mathf.FloorToInt(CurrentFrame);

        public int Direction { get; private set; } = 1;

        /// <summary>
        /// Indicates if the animation speed has a non-zero speed
        /// </summary>
        /// <returns>True if the speed is non-zero, otherwise false</returns>
        public abstract bool HasSpeed();

        /// <summary>
        /// Gets the change in the current frame index for this update
        /// </summary>
        /// <returns>The frame change</returns>
        protected abstract float GetFrameChange();

        /// <summary>
        /// Updates the <see cref="CurrentFrame"/>
        /// </summary>
        /// <param name="framesCount">The amount of frames in the animation</param>
        /// <param name="loopMode">The animation loop mode</param>
        /// <returns>True if the current frame is a new value, otherwise false</returns>
        public bool Update(int framesCount, AnimLoopMode loopMode = AnimLoopMode.Repeat) => Update(0, framesCount, loopMode);

        /// <summary>
        /// Updates the <see cref="CurrentFrame"/>
        /// </summary>
        /// <param name="startFrame">The first frame of the animation. Usually 0.</param>
        /// <param name="framesCount">The amount of frames in the animation</param>
        /// <param name="loopMode">The animation loop mode</param>
        /// <returns>True if the current frame is a new value, otherwise false</returns>
        public bool Update(int startFrame, int framesCount, AnimLoopMode loopMode = AnimLoopMode.Repeat)
        {
            if (!HasSpeed())
                return false;

            int curFrame = Mathf.FloorToInt(CurrentFrame);
            CurrentFrame += GetFrameChange() * Direction;

            if (CurrentFrame >= framesCount)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.Repeat:
                        CurrentFrame = 0;
                        break;

                    case AnimLoopMode.PingPong:
                        Direction = -1;
                        CurrentFrame = framesCount - 1;
                        break;
                }
            }
            else if (CurrentFrame <= startFrame)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.PingPong:
                        Direction = 1;
                        CurrentFrame = startFrame + 1;
                        break;
                }
            }

            int newFrame = Mathf.FloorToInt(CurrentFrame);

            return newFrame != curFrame;
        }

        /// <summary>
        /// Resets the frame to 0
        /// </summary>
        public void Reset()
        {
            CurrentFrame = 0;
            Direction = 1;
        }

        public object Clone() => MemberwiseClone();
        public AnimSpeed CloneAnimSpeed() => (AnimSpeed)Clone();
    }
}