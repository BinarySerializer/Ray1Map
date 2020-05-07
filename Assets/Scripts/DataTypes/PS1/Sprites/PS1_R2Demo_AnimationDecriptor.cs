namespace R1Engine
{
    /// <summary>
    /// Animation descriptor for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_AnimationDecriptor : R1Serializable
    {
        #region Animation Data

        /// <summary>
        /// Pointer to the animation layers
        /// </summary>
        public Pointer LayersPointer { get; set; }

        /// <summary>
        /// Pointer to the animation frames
        /// </summary>
        public Pointer FramesPointer { get; set; }

        // TODO: Parse the data from this pointer
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

        // Most likely related to UnkPointer3 - usually 0 - 1 or 2 when UnkPointer3 is valid
        public byte Unk2 { get; set; }

        #endregion

        #region Pointer Data

        /// <summary>
        /// The pointers to the layers
        /// </summary>
        public Pointer[] LayerPointers { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public Common_AnimationLayer[][] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public Common_AnimationFrame[] Frames { get; set; }

        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            LayersPointer = s.SerializePointer(LayersPointer, name: nameof(LayersPointer));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3)); // ^points to 8 bytes

            // Serialize values
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: nameof(LayersPerFrame));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unk2 = s.Serialize<byte>(Unk2, name: nameof(Unk2));

            // Serialize layers
            s.DoAt(LayersPointer, () =>
            {
                // Serialize the layer pointers
                LayerPointers = s.SerializePointerArray(LayerPointers, FrameCount, name: nameof(LayerPointers));

                if (Layers == null)
                    Layers = new Common_AnimationLayer[FrameCount][];

                // Serialize the layers for each frame
                for (int i = 0; i < Layers.Length; i++)
                    Layers[i] = s.SerializeObjectArray<Common_AnimationLayer>(Layers[i], LayersPerFrame, name: $"{nameof(Layers)} [{i}]");
            });

            // Serialize frames
            s.DoAt(FramesPointer, () => Frames = s.SerializeObjectArray(Frames, FrameCount, name: nameof(Frames)));
        }
    }
}