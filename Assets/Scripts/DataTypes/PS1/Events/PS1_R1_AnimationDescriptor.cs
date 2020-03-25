namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_AnimationDescriptor : R1Serializable
    {
        public Pointer UnkPointer1 { get; set; } // Unknown 2 times Unknown1 dwords
        
        public Pointer UnkPointer2 { get; set; } // Struct: Unknown2 dwords, then 2 ushorts.

        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        public ushort LayersPerFrame { get; set; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public ushort FrameCount { get; set; }
        
        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize pointers
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: "UnkPointer1");
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: "UnkPointer2");
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: "LayersPerFrame");
            FrameCount = s.Serialize<ushort>(FrameCount, name: "Unknown2");
        }
    }
}