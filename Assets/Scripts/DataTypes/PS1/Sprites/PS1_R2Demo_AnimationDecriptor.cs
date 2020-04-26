namespace R1Engine
{
    // TODO: Merge with normal anim desc class?
    /// <summary>
    /// Animation descriptor for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_AnimationDecriptor : R1Serializable
    {
        /// <summary>
        /// Pointer to the animation layers
        /// </summary>
        public Pointer LayersPointer { get; set; }

        /// <summary>
        /// Pointer to the animation frames
        /// </summary>
        public Pointer FramesPointer { get; set; }

        // Unknown - usually null
        public Pointer UnkPointer3 { get; set; }

        /// <summary>
        /// The amount of layers per frame
        /// </summary>
        public ushort LayersPerFrame { get; set; }

        /// <summary>
        /// The amount of frames in the animation
        /// </summary>
        public byte FrameCount { get; set; }

        // Most likely related to UnkPointer3 - usually 0
        public byte Unk2 { get; set; }


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
            LayersPointer = s.SerializePointer(LayersPointer, name: nameof(LayersPointer));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            // Serialize values
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: nameof(LayersPerFrame));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unk2 = s.Serialize<byte>(Unk2, name: nameof(Unk2));

            // Serialize layers
            s.DoAt(LayersPointer + (4 * FrameCount), () => Layers = s.SerializeObjectArray<Common_AnimationLayer>(Layers, LayersPerFrame * FrameCount, name: nameof(Layers)));

            // Serialize frames
            s.DoAt(FramesPointer, () => Frames = s.SerializeObjectArray(Frames, FrameCount, name: nameof(Frames)));
        }
    }
}