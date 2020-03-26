namespace R1Engine
{
    // TODO: Merge with PS1_R1_ImageDescriptor
    /// <summary>
    /// Image descriptor data for PC
    /// </summary>
    public class PC_ImageDescriptor : R1Serializable
    {
        /// <summary>
        /// The image offset in the image data
        /// </summary>
        public uint ImageOffset { get; set; }

        // Index?
        public byte Unknown1 { get; set; }

        /// <summary>
        /// The outer image width (including the margins)
        /// </summary>
        public byte OuterWidth { get; set; }

        /// <summary>
        /// The outer image height (including the margins)
        /// </summary>
        public byte OuterHeight { get; set; }

        /// <summary>
        /// The inner image width
        /// </summary>
        public byte InnerWidth { get; set; }

        /// <summary>
        /// The inner image height
        /// </summary>
        public byte InnerHeight { get; set; }

        public byte Unknown2 { get; set; }

        public byte Unknown3 { get; set; }

        public byte Unknown4 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            ImageOffset = s.Serialize<uint>(ImageOffset, name: nameof(ImageOffset));
            Unknown1 = s.Serialize<byte>(Unknown1, name: nameof(Unknown1));
            OuterWidth = s.Serialize<byte>(OuterWidth, name: nameof(OuterWidth));
            OuterHeight = s.Serialize<byte>(OuterHeight, name: nameof(OuterHeight));
            InnerWidth = s.Serialize<byte>(InnerWidth, name: nameof(InnerWidth));
            InnerHeight = s.Serialize<byte>(InnerHeight, name: nameof(InnerHeight));
            Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
            Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
            Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
        }
    }
}