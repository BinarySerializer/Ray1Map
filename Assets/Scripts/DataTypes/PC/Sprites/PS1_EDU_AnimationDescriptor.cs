namespace R1Engine
{
    // TODO: Merge with PS1 anim descriptor class once we parse pointers!

    /// <summary>
    /// Animation descriptor data for EDU on PS1
    /// </summary>
    public class PS1_EDU_AnimationDescriptor : R1Serializable
    {
        public uint AnimLayersPointer { get; set; }
        public uint AnimFramesPointer { get; set; }

        public ushort LayersPerFrame { get; set; }
        public ushort FrameCount { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            AnimLayersPointer = s.Serialize<uint>(AnimLayersPointer, name: nameof(AnimLayersPointer));
            AnimFramesPointer = s.Serialize<uint>(AnimFramesPointer, name: nameof(AnimFramesPointer));
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: nameof(LayersPerFrame));
            FrameCount = s.Serialize<ushort>(FrameCount, name: nameof(FrameCount));
        }
    }
}