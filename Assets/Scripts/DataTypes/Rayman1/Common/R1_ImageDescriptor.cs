namespace R1Engine
{
    /// <summary>
    /// Common image descriptor data
    /// </summary>
    public class R1_ImageDescriptor : R1Serializable
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
        /// The outer image width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The outer image height
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The inner image width
        /// </summary>
        public byte HitBoxWidth { get; set; }

        /// <summary>
        /// The inner image height
        /// </summary>
        public byte HitBoxHeight { get; set; }

        /// <summary>
        /// Image type (JP versions).
        /// 3: 8-bit
        /// 2: 4-bit
        /// 1: Null?
        /// </summary>
        public ushort ImageType { get; set; }

        // Some of these are hitbox related
        public byte HitBoxOffsetX { get; set; }
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
            if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.Rayman1_Jaguar)
            {
                // TODO: Is the fourth byte always 0?
                ImageBufferOffset = s.Serialize<uint>(ImageBufferOffset << 8, name: nameof(ImageBufferOffset)) >> 8;

                Jag_Byte04 = s.Serialize<byte>(Jag_Byte04, name: nameof(Jag_Byte04));
                Jag_Ushort05 = s.Serialize<ushort>(Jag_Ushort05, name: nameof(Jag_Ushort05));
                Height = (ushort)(Jag_Ushort05 >> 6);
                Jag_Ushort07 = s.Serialize<ushort>(Jag_Ushort07, name: nameof(Jag_Ushort07));
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));

                Jag_Byte0A = s.Serialize<byte>(Jag_Byte0A, name: nameof(Jag_Byte0A));
                Jag_Bytes0B = s.SerializeArray<byte>(Jag_Bytes0B, 3, name: nameof(Jag_Bytes0B));
                Jag_Byte0E = s.Serialize<byte>(Jag_Byte0E, name: nameof(Jag_Byte0E));
                Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
            }
            else
            {
                if (s.GameSettings.Game != Game.R1_Rayman2)
                    ImageBufferOffset = s.Serialize<uint>(ImageBufferOffset, name: nameof(ImageBufferOffset));

                // PS1
                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1 || 
                    s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JP || 
                    s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || 
                    s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6 || 
                    s.GameSettings.EngineVersion == EngineVersion.R1_Saturn || 
                    s.GameSettings.EngineVersion == EngineVersion.R2_PS1)
                {
                    if (s.Context.Settings.Game == Game.R1_Rayman2)
                    {
                        Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                        Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                        HitBoxWidth = s.Serialize<byte>(HitBoxWidth, name: nameof(HitBoxWidth));
                        HitBoxHeight = s.Serialize<byte>(HitBoxHeight, name: nameof(HitBoxHeight));
                        Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                        Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
                        ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
                        ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
                        PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));
                        TexturePageInfo = s.Serialize<ushort>(TexturePageInfo, name: nameof(TexturePageInfo));
                    }
                    else if (s.Context.Settings.EngineVersion == EngineVersion.R1_PS1_JP || 
                             s.Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || 
                             s.Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                    {
                        Index = s.Serialize<ushort>(Index, name: nameof(Index));
                        ImageType = s.Serialize<ushort>(ImageType, name: nameof(ImageType));
                        Width = s.Serialize<ushort>(Width, name: nameof(Width));
                        Height = s.Serialize<ushort>(Height, name: nameof(Height));

                        // Which value is this?
                        HitBoxOffsetX = (byte)s.Serialize<ushort>(HitBoxOffsetX, name: nameof(HitBoxOffsetX));
                    }
                    else if (s.GameSettings.EngineVersion == EngineVersion.R1_Saturn)
                    {
                        Index = s.Serialize<ushort>(Index, name: nameof(Index));

                        // ?
                        ImageType = s.Serialize<ushort>(ImageType, name: nameof(ImageType));

                        Width = s.Serialize<ushort>(Width, name: nameof(Width));
                        Height = s.Serialize<ushort>(Height, name: nameof(Height));

                        // Unsure below here...
                        PaletteInfo = s.Serialize<ushort>(PaletteInfo, name: nameof(PaletteInfo));

                        // Which value is this?
                        HitBoxOffsetX = (byte)s.Serialize<ushort>(HitBoxOffsetX, name: nameof(HitBoxOffsetX));
                    }
                    else
                    {
                        Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
                        Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                        Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                        HitBoxWidth = s.Serialize<byte>(HitBoxWidth, name: nameof(HitBoxWidth));
                        HitBoxHeight = s.Serialize<byte>(HitBoxHeight, name: nameof(HitBoxHeight));
                        HitBoxOffsetX = s.Serialize<byte>(HitBoxOffsetX, name: nameof(HitBoxOffsetX));
                    }

                    if (s.GameSettings.EngineVersion != EngineVersion.R1_Saturn && 
                        s.GameSettings.Game != Game.R1_Rayman2)
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
                // PC
                else if (s.GameSettings.EngineVersion == EngineVersion.R1_PC || 
                         s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit || 
                         s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu || 
                         s.GameSettings.EngineVersion == EngineVersion.R1_PS1_Edu || 
                         s.GameSettings.EngineVersion == EngineVersion.R1_PocketPC || 
                         s.GameSettings.EngineVersion == EngineVersion.R1_GBA || 
                         s.GameSettings.EngineVersion == EngineVersion.R1_DSi)
                {
                    Index = s.Serialize<byte>((byte)Index, name: nameof(Index));
                    Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                    Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                    HitBoxWidth = s.Serialize<byte>(HitBoxWidth, name: nameof(HitBoxWidth));
                    HitBoxHeight = s.Serialize<byte>(HitBoxHeight, name: nameof(HitBoxHeight));
                    HitBoxOffsetX = s.Serialize<byte>(HitBoxOffsetX, name: nameof(HitBoxOffsetX));
                    Unknown3 = s.Serialize<byte>(Unknown3, name: nameof(Unknown3));
                    Unknown4 = s.Serialize<byte>(Unknown4, name: nameof(Unknown4));
                }
            }
        }
    }
}