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

        public byte Unknown4 { get; set; }

        public ushort PaletteInfo { get; set; }
        public ushort TexturePageInfo { get; set; }
        public byte ImageOffsetInPageX { get; set; }
        public byte ImageOffsetInPageY { get; set; }
        public ushort Unknown6 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            Unknown0 = s.Serialize<uint>(Unknown0, name: nameof(Unknown0));
            if (s.Context.Settings.EngineVersion == EngineVersion.RayPS1JP || s.Context.Settings.EngineVersion == EngineVersion.RayPS1JPDemo) {
                Unknown1 = (byte)s.Serialize<ushort>(Unknown1, name: nameof(Unknown1));
                Unknown2 = (byte)s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));
                OuterWidth = (byte)s.Serialize<ushort>(OuterWidth, name: nameof(OuterWidth));
                OuterHeight = (byte)s.Serialize<ushort>(OuterHeight, name: nameof(OuterHeight));
                Unknown2 = (byte)s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));
                Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
                PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));
            } else {
                Unknown1 = s.Serialize<byte>(Unknown1, name: nameof(Unknown1));
                OuterWidth = s.Serialize<byte>(OuterWidth, name: nameof(OuterWidth));
                OuterHeight = s.Serialize<byte>(OuterHeight, name: nameof(OuterHeight));
                InnerWidth = s.Serialize<byte>(InnerWidth, name: nameof(InnerWidth));
                InnerHeight = s.Serialize<byte>(InnerHeight, name: nameof(InnerHeight));
                Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
                Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
                PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));
            }
            TexturePageInfo = s.Serialize<ushort>(TexturePageInfo, name: nameof(TexturePageInfo));
            ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
            ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
            Unknown6 = s.Serialize<ushort>(Unknown6, name: nameof(Unknown6));
        }
    }
}