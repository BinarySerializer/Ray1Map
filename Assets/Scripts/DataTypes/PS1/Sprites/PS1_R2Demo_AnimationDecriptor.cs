namespace R1Engine
{
    /// <summary>
    /// Animation descriptor for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_AnimationDecriptor : R1Serializable
    {
        /// <summary>
        /// Pointer to the frames pointers
        /// </summary>
        public Pointer FramesPointer { get; set; }

        /// <summary>
        /// Pointer to the animation layers
        /// </summary>
        public Pointer LayersPointer { get; set; }

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
        /// The frame pointers
        /// </summary>
        public Pointer[] FramePointers { get; set; }

        // Unknown frame data
        public byte[][] FrameData { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public PS1_R2Demo_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            LayersPointer = s.SerializePointer(LayersPointer, name: nameof(LayersPointer));
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            // Serialize values
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: nameof(LayersPerFrame));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unk2 = s.Serialize<byte>(Unk2, name: nameof(Unk2));

            // Serialize frame pointers
            s.DoAt(FramesPointer, () => FramePointers = s.SerializePointerArray(FramePointers, FrameCount, name: nameof(FramePointers)));

            // Create frame data array
            if (FrameData == null)
                FrameData = new byte[FrameCount][];

            // Serialize frame data
            for (int i = 0; i < FrameData.Length; i++)
                s.DoAt(FramePointers[i], () => FrameData[i] = s.SerializeArray<byte>(FrameData[i], LayersPerFrame * 4, name: $"{nameof(FrameData)} [{i}]"));

            // Serialize layers
            s.DoAt(LayersPointer, () => Layers = s.SerializeObjectArray<PS1_R2Demo_AnimationLayer>(Layers, LayersPerFrame * FrameCount, name: nameof(Layers)));
        }
    }
}