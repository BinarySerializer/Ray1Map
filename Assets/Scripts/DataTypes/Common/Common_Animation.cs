using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common sprite animation data
    /// </summary>
    public class Common_Animation
    {
        /// <summary>
        /// The frames in the animation
        /// </summary>
        public Common_AnimationPart[,] Frames { get; set; }

        /// <summary>
        /// The frame rate is frames per second
        /// </summary>
        public int Framerate { get; set; }
    }
}