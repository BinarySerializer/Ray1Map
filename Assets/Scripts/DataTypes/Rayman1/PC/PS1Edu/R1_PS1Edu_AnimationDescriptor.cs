namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for EDU on PS1
    /// </summary>
    public class R1_PS1Edu_AnimationDescriptor : R1Serializable, IR1_AnimationDescriptor
    {
        #region Descriptor Properties

        // These get set during runtime
        public uint AnimLayersPointer { get; set; }
        public uint AnimFramesPointer { get; set; }

        public ushort LayersPerFrame { get; set; }
        public ushort FrameCount { get; set; }

        #endregion

        #region Parsed Properties

        // Parsed from world files
        public byte[] LayersData { get; set; }
        public R1_AnimationFrame[] Frames { get; set; }
        public R1_AnimationLayer[] Layers { get; set; }

        // Interface members
        byte IR1_AnimationDescriptor.LayersPerFrame => (byte)LayersPerFrame;
        byte IR1_AnimationDescriptor.FrameCount => (byte)FrameCount;

        #endregion

        #region Public Methods

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

        #endregion
    }
}