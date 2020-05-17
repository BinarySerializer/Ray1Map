namespace R1Engine
{
    /// <summary>
    /// Interface for an animation descriptor
    /// </summary>
    public interface IAnimationDescriptor
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
        Common_AnimationLayer[] Layers { get; }

        /// <summary>
        /// The animation frames
        /// </summary>
        Common_AnimationFrame[] Frames { get; }
    }
}