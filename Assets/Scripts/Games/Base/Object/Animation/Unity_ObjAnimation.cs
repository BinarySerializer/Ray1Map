namespace Ray1Map
{
    /// <summary>
    /// Common sprite animation data
    /// </summary>
    public class Unity_ObjAnimation
    {
        /// <summary>
        /// The animation frames
        /// </summary>
        public Unity_ObjAnimationFrame[] Frames { get; set; }

        public int? AnimSpeed { get; set; }
        public int[] AnimSpeeds { get; set; }
        public bool IsAdditionalAnimation { get; set; }
    }
}