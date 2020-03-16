namespace R1Engine
{
    /// <summary>
    /// Image descriptor data for PC
    /// </summary>
    public class PC_ImageDescriptor : IBinarySerializable
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

        public byte Unknown4{ get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(ImageOffset));
            serializer.Serialize(nameof(Unknown1));
            serializer.Serialize(nameof(OuterWidth));
            serializer.Serialize(nameof(OuterHeight));
            serializer.Serialize(nameof(InnerWidth));
            serializer.Serialize(nameof(InnerHeight));
            serializer.Serialize(nameof(Unknown2));
            serializer.Serialize(nameof(Unknown3));
            serializer.Serialize(nameof(Unknown4));
        }
    }
}