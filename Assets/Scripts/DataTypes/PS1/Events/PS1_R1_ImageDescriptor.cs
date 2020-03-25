namespace R1Engine
{
    // TODO: Merge with PC_ImageDescriptor
    /// <summary>
    /// Image descriptor data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_ImageDescriptor : R1Serializable
    {
        // Always 0
        public uint Unknown0 { get; set; }

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

        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// The sprite offset
        /// </summary>
        public uint SpriteOffset { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            Unknown0 = s.Serialize<uint>(Unknown0, name: "Unknown0");
            Unknown1 = s.Serialize<byte>(Unknown1, name: "Unknown1");
            OuterWidth = s.Serialize<byte>(OuterWidth, name: "OuterWidth");
            OuterHeight = s.Serialize<byte>(OuterHeight, name: "OuterHeight");
            InnerWidth = s.Serialize<byte>(InnerWidth, name: "InnerWidth");
            InnerHeight = s.Serialize<byte>(InnerHeight, name: "InnerHeight");
            Unknown2 = s.Serialize<byte>(Unknown2, name: "Unknown2");
            Unknown3 = s.Serialize<byte>(Unknown3, name: "Unknown3");
            Unknown4 = s.SerializeArray<byte>(Unknown4, 5, name: "Unknown4");
            SpriteOffset = s.Serialize<uint>(SpriteOffset, name: "SpriteOffset");
        }
    }
}