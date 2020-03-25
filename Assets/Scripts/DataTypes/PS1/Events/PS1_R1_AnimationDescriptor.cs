namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_AnimationDescriptor : R1Serializable
    {
        /// <summary>
        /// Pointer to the animation layers
        /// </summary>
        public Pointer AnimLayersPointer { get; set; }
        
        /// <summary>
        /// Pointer to the animation frames
        /// </summary>
        public Pointer AnimFramesPointer { get; set; }

        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        public ushort LayersPerFrame { get; set; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public ushort FrameCount { get; set; }

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
            // Serialize pointers
            AnimLayersPointer = s.SerializePointer(AnimLayersPointer, name: "AnimLayersPointer");
            AnimFramesPointer = s.SerializePointer(AnimFramesPointer, name: "AnimFramesPointer");
            
            // Serialize data
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: "LayersPerFrame");
            FrameCount = s.Serialize<ushort>(FrameCount, name: "FrameCount");

            // Serialize data from pointers
            s.DoAt(AnimLayersPointer, () =>
            {
                Layers = s.SerializeObjectArray(Layers, LayersPerFrame * FrameCount, name: "Layers");
            });
            s.DoAt(AnimFramesPointer, () =>
            {
                Frames = s.SerializeObjectArray(Frames, FrameCount + 1, name: "Frames");
            });
        }
    }
}