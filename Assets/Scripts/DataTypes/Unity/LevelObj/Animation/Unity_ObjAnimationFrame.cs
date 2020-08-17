namespace R1Engine
{
    /// <summary>
    /// Common animation frame data
    /// </summary>
    public class Unity_ObjAnimationFrame
    {
        /// <summary>
        /// The frame data
        /// </summary>
        public R1_AnimationFrame FrameData { get; set; }

        /// <summary>
        /// The layers
        /// </summary>
        public Unity_ObjAnimationPart[] Layers { get; set; }
    }
}