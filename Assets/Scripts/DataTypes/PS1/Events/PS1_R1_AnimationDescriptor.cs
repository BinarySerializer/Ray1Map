namespace R1Engine
{
    // TODO: Merge with PC_AnimationDescriptor
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
        public byte LayersPerFrame { get; set; }

        public byte Unknown1 { get; set; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public byte FrameCount { get; set; }

        public byte Unknown2 { get; set; }

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
            AnimLayersPointer = s.SerializePointer(AnimLayersPointer, name: nameof(AnimLayersPointer));
            AnimFramesPointer = s.SerializePointer(AnimFramesPointer, name: nameof(AnimFramesPointer));
            
            // Serialize data
            LayersPerFrame = s.Serialize<byte>(LayersPerFrame, name: nameof(LayersPerFrame));
            Unknown1 = s.Serialize<byte>(Unknown1, name: nameof(Unknown1));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));

            // Serialize data from pointers
            s.DoAt(AnimLayersPointer, () =>
            {
                Layers = s.SerializeObjectArray(Layers, LayersPerFrame * FrameCount, name: nameof(Layers));
            });
            s.DoAt(AnimFramesPointer, () =>
            {
                Frames = s.SerializeObjectArray(Frames, s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemo ? FrameCount : FrameCount + 1, name: nameof(Frames));
            });
        }
    }
}