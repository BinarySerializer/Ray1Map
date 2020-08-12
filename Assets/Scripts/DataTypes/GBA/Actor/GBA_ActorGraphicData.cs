namespace R1Engine
{
    // TODO: Clean up, move to separate files, rename classes

    public class GBA_ActorGraphicData : GBA_BaseBlock
    {
        public byte[] UnkData { get; set; }

        public byte SpriteGroupOffsetIndex { get; set; }
        public byte Byte_09 { get; set; }
        public byte Byte_0A { get; set; }
        public byte Byte_0B { get; set; }

        public GBA_ActorGraphicDataEntry[] Entries { get; set; }

        public GBA_ActorGraphicSpriteGroup SpriteGroup { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, 8, name: nameof(UnkData));

            SpriteGroupOffsetIndex = s.Serialize<byte>(SpriteGroupOffsetIndex, name: nameof(SpriteGroupOffsetIndex));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));

            // TODO: Get number of entries - this doesn't always work
            Entries = s.SerializeObjectArray<GBA_ActorGraphicDataEntry>(Entries, (BlockSize - 12) / 8, name: nameof(Entries));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            SpriteGroup = s.DoAt(OffsetTable.GetPointer(SpriteGroupOffsetIndex), () => s.SerializeObject<GBA_ActorGraphicSpriteGroup>(SpriteGroup, name: nameof(SpriteGroup)));
        }
    }

    public class GBA_ActorGraphicDataEntry : R1Serializable
    {
        // Byte_07 seems to be offset index (it's not used if Byte_06 is -1)
        // Byte_06 seems to determine how the data structure from the offset should look like
        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, 8, name: nameof(UnkData));
        }
    }

    public class GBA_ActorGraphicSpriteGroup : GBA_BaseBlock
    {
        #region Data

        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte UnkOffsetIndex { get; set; }
        public byte PaletteOffsetIndex { get; set; }
        public byte UnkOffsetIndex3 { get; set; }
        public byte Byte_04 { get; set; }
        public byte SpritesCount { get; set; }
        public byte Byte_06 { get; set; }

        public byte[] SpritesIndexTable { get; set; }

        #endregion

        #region Parsed

        public GBA_SpritePalette Palette { get; set; }
        public GBA_Sprite[] Sprites { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            UnkOffsetIndex = s.Serialize<byte>(UnkOffsetIndex, name: nameof(UnkOffsetIndex));
            PaletteOffsetIndex = s.Serialize<byte>(PaletteOffsetIndex, name: nameof(PaletteOffsetIndex));
            if (s.GameSettings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || s.GameSettings.EngineVersion == EngineVersion.StarWarsGBA) {
                UnkOffsetIndex3 = s.Serialize<byte>(UnkOffsetIndex3, name: nameof(UnkOffsetIndex3));
            }
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            SpritesCount = s.Serialize<byte>(SpritesCount, name: nameof(SpritesCount));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));

            SpritesIndexTable = s.SerializeArray<byte>(SpritesIndexTable, SpritesCount, name: nameof(SpritesIndexTable));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Palette = s.DoAt(OffsetTable.GetPointer(PaletteOffsetIndex), () => s.SerializeObject<GBA_SpritePalette>(Palette, name: nameof(Palette)));

            if (Sprites == null)
                Sprites = new GBA_Sprite[SpritesCount];

            for (int i = 0; i < Sprites.Length; i++)
                Sprites[i] = s.DoAt(OffsetTable.GetPointer(SpritesIndexTable[i]), () => s.SerializeObject<GBA_Sprite>(Sprites[i], name: $"{nameof(Sprites)}[{i}]"));
        }

        #endregion
    }

    // Sprites have tilemaps and are 4bpp
    public class GBA_Sprite : GBA_BaseBlock
    {
        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, BlockSize, name: nameof(UnkData));
        }
    }

    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, BlockSize / 2, name: nameof(Palette));
        }
    }
}