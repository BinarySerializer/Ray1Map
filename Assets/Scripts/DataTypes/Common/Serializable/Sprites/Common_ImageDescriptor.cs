namespace R1Engine
{
    /// <summary>
    /// Common image descriptor data
    /// </summary>
    public class Common_ImageDescriptor : R1Serializable
    {
        /// <summary>
        /// The image buffer offset. In final PS1 versions this is always 0 except for backgrounds.
        /// </summary>
        public uint ImageBufferOffset { get; set; }

        /// <summary>
        /// Index of the image? Doesn't always match.
        /// </summary>
        public ushort Index { get; set; }
        
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

        /// <summary>
        /// Image type (JP versions).
        /// 3: 8-bit
        /// 2: 4-bit
        /// 1: Null?
        /// </summary>
        public ushort ImageType { get; set; }

        // Some of these are hitbox related
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
            ImageBufferOffset = s.Serialize<uint>(ImageBufferOffset, name: nameof(ImageBufferOffset));

            if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.PS1)
            {
                if (s.Context.Settings.EngineVersion == EngineVersion.RayPS1JP || s.Context.Settings.EngineVersion == EngineVersion.RayPS1JPDemo)
                {
                    Index = s.Serialize<ushort>(Index, name: nameof(Index));
                    ImageType = s.Serialize<ushort>(ImageType, name: nameof(ImageType));
                    OuterWidth = (byte)s.Serialize<ushort>(OuterWidth, name: nameof(OuterWidth));
                    OuterHeight = (byte)s.Serialize<ushort>(OuterHeight, name: nameof(OuterHeight));
                    Unknown2 = (byte)s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.RaySaturn)
                {
                    Index = s.Serialize<ushort>(Index, name: nameof(Index));

                    // ?
                    ImageType = s.Serialize<ushort>(ImageType, name: nameof(ImageType));

                    OuterWidth = (byte)s.Serialize<ushort>(OuterWidth, name: nameof(OuterWidth));
                    OuterHeight = (byte)s.Serialize<ushort>(OuterHeight, name: nameof(OuterHeight));

                    // Unsure below here...
                    Unknown2 = (byte)s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));
                }
                else
                {
                    Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
                    OuterWidth = s.Serialize<byte>(OuterWidth, name: nameof(OuterWidth));
                    OuterHeight = s.Serialize<byte>(OuterHeight, name: nameof(OuterHeight));
                    InnerWidth = s.Serialize<byte>(InnerWidth, name: nameof(InnerWidth));
                    InnerHeight = s.Serialize<byte>(InnerHeight, name: nameof(InnerHeight));
                    Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
                }

                Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));

                if (s.GameSettings.EngineVersion != EngineVersion.RaySaturn)
                {
                    PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));

                    TexturePageInfo = s.Serialize<ushort>(TexturePageInfo, name: nameof(TexturePageInfo));
                    ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
                    ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
                    Unknown6 = s.Serialize<ushort>(Unknown6, name: nameof(Unknown6));
                }
            }
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.PC)
            {
                Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
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
}