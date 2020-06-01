namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for EDU on PS1
    /// </summary>
    public class PS1_EDU_AnimationDescriptor : R1Serializable, IAnimationDescriptor
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
        public Common_AnimationFrame[] Frames { get; set; }
        public Common_AnimationLayer[] Layers { get; set; }

        // Interface members
        byte IAnimationDescriptor.LayersPerFrame => (byte)LayersPerFrame;
        byte IAnimationDescriptor.FrameCount => (byte)FrameCount;

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