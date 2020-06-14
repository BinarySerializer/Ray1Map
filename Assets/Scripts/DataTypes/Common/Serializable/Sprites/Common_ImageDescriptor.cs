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
        /// Index of the image? Doesn't always match. Is 0 for dummy sprites.
        /// </summary>
        public ushort Index { get; set; }
        
        /// <summary>
        /// The outer image width (including the margins)
        /// </summary>
        public ushort OuterWidth { get; set; }

        /// <summary>
        /// The outer image height (including the margins)
        /// </summary>
        public ushort OuterHeight { get; set; }

        // These are most likely the hitbox sizes for hitsprite and followsprite
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

        public byte Jag_Byte04 { get; set; }
        public ushort Jag_Ushort05 { get; set; }
        public ushort Jag_Ushort07 { get; set; }

        // Four bits from offset 1 are palette offset for 4-bit sprites
        public byte Jag_Byte0A { get; set; }
        public byte[] Jag_Bytes0B { get; set; }

        // Flags - bit 4 indicates if it's 8-bit (otherwise 4-bit)
        public byte Jag_Byte0E { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.Jaguar)
            {
                // TODO: Is the fourth byte always 0?
                ImageBufferOffset = s.Serialize<uint>(ImageBufferOffset << 8, name: nameof(ImageBufferOffset)) >> 8;

                Jag_Byte04 = s.Serialize<byte>(Jag_Byte04, name: nameof(Jag_Byte04));
                Jag_Ushort05 = s.Serialize<ushort>(Jag_Ushort05, name: nameof(Jag_Ushort05));
                OuterHeight = (ushort)(Jag_Ushort05 >> 6);
                Jag_Ushort07 = s.Serialize<ushort>(Jag_Ushort07, name: nameof(Jag_Ushort07));
                OuterWidth = s.Serialize<byte>((byte)OuterWidth, name: nameof(OuterWidth));

                Jag_Byte0A = s.Serialize<byte>(Jag_Byte0A, name: nameof(Jag_Byte0A));
                Jag_Bytes0B = s.SerializeArray<byte>(Jag_Bytes0B, 3, name: nameof(Jag_Bytes0B));
                Jag_Byte0E = s.Serialize<byte>(Jag_Byte0E, name: nameof(Jag_Byte0E));
                Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
            }
            else
            {
                if (s.GameSettings.Game != Game.Rayman2)
                    ImageBufferOffset = s.Serialize<uint>(ImageBufferOffset, name: nameof(ImageBufferOffset));

                if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.PS1)
                {
                    if (s.Context.Settings.Game == Game.Rayman2)
                    {
                        OuterWidth = s.Serialize<byte>((byte)OuterWidth, name: nameof(OuterWidth));
                        OuterHeight = s.Serialize<byte>((byte)OuterHeight, name: nameof(OuterHeight));
                        InnerWidth = s.Serialize<byte>(InnerWidth, name: nameof(InnerWidth));
                        InnerHeight = s.Serialize<byte>(InnerHeight, name: nameof(InnerHeight));
                        Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                        Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
                        ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
                        ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
                        PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));
                        TexturePageInfo = s.Serialize<ushort>(TexturePageInfo, name: nameof(TexturePageInfo));
                    }
                    else if (s.Context.Settings.EngineVersion == EngineVersion.RayPS1JP || s.Context.Settings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.Context.Settings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
                    {
                        Index = s.Serialize<ushort>(Index, name: nameof(Index));
                        ImageType = s.Serialize<ushort>(ImageType, name: nameof(ImageType));
                        OuterWidth = s.Serialize<ushort>(OuterWidth, name: nameof(OuterWidth));
                        OuterHeight = s.Serialize<ushort>(OuterHeight, name: nameof(OuterHeight));
                        Unknown2 = (byte)s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));
                    }
                    else if (s.GameSettings.EngineVersion == EngineVersion.RaySaturn)
                    {
                        Index = s.Serialize<ushort>(Index, name: nameof(Index));

                        // ?
                        ImageType = s.Serialize<ushort>(ImageType, name: nameof(ImageType));

                        OuterWidth = s.Serialize<ushort>(OuterWidth, name: nameof(OuterWidth));
                        OuterHeight = s.Serialize<ushort>(OuterHeight, name: nameof(OuterHeight));

                        // Unsure below here...
                        PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));

                        Unknown2 = (byte)s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));
                    }
                    else
                    {
                        Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
                        OuterWidth = s.Serialize<byte>((byte)OuterWidth, name: nameof(OuterWidth));
                        OuterHeight = s.Serialize<byte>((byte)OuterHeight, name: nameof(OuterHeight));
                        InnerWidth = s.Serialize<byte>(InnerWidth, name: nameof(InnerWidth));
                        InnerHeight = s.Serialize<byte>(InnerHeight, name: nameof(InnerHeight));
                        Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
                    }

                    if (s.GameSettings.EngineVersion != EngineVersion.RaySaturn
                        && s.GameSettings.Game != Game.Rayman2)
                    {
                        Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                        Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
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
                    OuterWidth = s.Serialize<byte>((byte)OuterWidth, name: nameof(OuterWidth));
                    OuterHeight = s.Serialize<byte>((byte)OuterHeight, name: nameof(OuterHeight));
                    InnerWidth = s.Serialize<byte>(InnerWidth, name: nameof(InnerWidth));
                    InnerHeight = s.Serialize<byte>(InnerHeight, name: nameof(InnerHeight));
                    Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
                    Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                    Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
                }
            }
        }
    }
}