namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for PC
    /// </summary>
    public class R1_PC_AnimationDescriptor : R1Serializable, IR1_AnimationDescriptor
    {
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

        // ID?
        public uint Unknown3 { get; set; }

        /// <summary>
        /// The length of <see cref="Layers"/>, <see cref="DefaultFrame"/> and <see cref="Frames"/>
        /// </summary>
        public ushort AnimationDataLength { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public R1_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The default animation frame (seems to always match the first frame)
        /// </summary>
        public R1_AnimationFrame DefaultFrame { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public R1_AnimationFrame[] Frames { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            LayersPerFrame = s.Serialize<byte>(LayersPerFrame, name: nameof(LayersPerFrame));
            Unknown1 = s.Serialize<byte>(Unknown1, name: nameof(Unknown1));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
            Unknown3 = s.Serialize<uint>(Unknown3, name: nameof(Unknown3));
            AnimationDataLength = s.Serialize<ushort>(AnimationDataLength, name: nameof(AnimationDataLength));
            Layers = s.SerializeObjectArray<R1_AnimationLayer>(Layers, LayersPerFrame * FrameCount, name: nameof(Layers));
            DefaultFrame = s.SerializeObject<R1_AnimationFrame>(DefaultFrame, name: nameof(DefaultFrame));
            Frames = s.SerializeObjectArray<R1_AnimationFrame>(Frames, FrameCount, name: nameof(Frames));
        }
    }
}