namespace R1Engine
{
    // TODO: Merge with PS1_R1_Event once all values have been parsed
    /// <summary>
    /// Event data for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_Event : R1Serializable
    {
        /// <summary>
        /// The pointer to the image descriptors
        /// </summary>
        public Pointer ImageDescriptorsPointer { get; set; }

        /// <summary>
        /// The pointer to the animation descriptors
        /// </summary>
        public Pointer AnimDescriptorsPointer { get; set; }

        public Pointer UnknownPointer1 { get; set; }

        // ETA?
        public Pointer UnknownPointer2 { get; set; }

        /// <summary>
        /// Pointer to the event commands
        /// </summary>
        public Pointer CommandsPointer { get; set; }

        public byte[] Unk1 { get; set; }

        public ushort XPosition { get; set; }

        public ushort YPosition { get; set; }

        // Last byte might be anim desc count
        public byte[] Unk2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
            UnknownPointer1 = s.SerializePointer(UnknownPointer1, name: nameof(UnknownPointer1));
            UnknownPointer2 = s.SerializePointer(UnknownPointer2, name: nameof(UnknownPointer2));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));

            // Serialize values
            Unk1 = s.SerializeArray(Unk1, 46, name: nameof(Unk1));
            XPosition = s.Serialize(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize(YPosition, name: nameof(YPosition));
            Unk2 = s.SerializeArray(Unk2, 74, name: nameof(Unk2));
        }
    }
}