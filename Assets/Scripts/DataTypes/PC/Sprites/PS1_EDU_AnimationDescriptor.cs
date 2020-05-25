namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for EDU on PS1
    /// </summary>
    public class PS1_EDU_AnimationDescriptor : R1Serializable
    {
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }

        public ushort LayersPerFrame { get; set; }
        public ushort FrameCount { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: nameof(LayersPerFrame));
            FrameCount = s.Serialize<ushort>(FrameCount, name: nameof(FrameCount));
        }
    }
}