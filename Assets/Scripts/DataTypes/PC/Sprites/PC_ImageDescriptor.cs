namespace R1Engine
{
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

        public byte Unknown4{ get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            ImageOffset = s.Serialize(ImageOffset, name: "ImageOffset");
            Unknown1 = s.Serialize(Unknown1, name: "Unknown1");
            OuterWidth = s.Serialize(OuterWidth, name: "OuterWidth");
            OuterHeight = s.Serialize(OuterHeight, name: "OuterHeight");
            InnerWidth = s.Serialize(InnerWidth, name: "InnerWidth");
            InnerHeight = s.Serialize(InnerHeight, name: "InnerHeight");
            Unknown2 = s.Serialize(Unknown2, name: "Unknown2");
            Unknown3 = s.Serialize(Unknown3, name: "Unknown3");
            Unknown4 = s.Serialize(Unknown4, name: "Unknown4");
        }
    }
}