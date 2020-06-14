namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_AnimationDescriptor : R1Serializable, IAnimationDescriptor
    {
        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        public byte LayersPerFrame { get; set; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public byte FrameCount { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public Common_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public Common_AnimationFrame[] Frames { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // TODO: Are there frames anywhere?

            // Serialize data
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            s.Serialize<byte>(default, name: "Padding");
            LayersPerFrame = s.Serialize<byte>(LayersPerFrame, name: nameof(LayersPerFrame));
            s.Serialize<byte>(default, name: "Padding");

            // Serialize data from pointers
            Layers = s.SerializeObjectArray(Layers, LayersPerFrame * FrameCount, name: nameof(Layers));
        }
    }
}