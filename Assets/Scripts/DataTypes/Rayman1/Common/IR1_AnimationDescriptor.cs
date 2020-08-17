namespace R1Engine
{
    /// <summary>
    /// Interface for an animation descriptor
    /// </summary>
    public interface IR1_AnimationDescriptor
    {
        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        byte LayersPerFrame { get; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        byte FrameCount { get;  }

        /// <summary>
        /// The animation layers
        /// </summary>
        R1_AnimationLayer[] Layers { get; }

        /// <summary>
        /// The animation frames
        /// </summary>
        R1_AnimationFrame[] Frames { get; }
    }
}