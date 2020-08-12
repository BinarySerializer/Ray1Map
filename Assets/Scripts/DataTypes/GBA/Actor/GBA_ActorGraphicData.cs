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

        public GBA_ActorState[] States { get; set; }

        public GBA_DES SpriteGroup { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, 8, name: nameof(UnkData));

            SpriteGroupOffsetIndex = s.Serialize<byte>(SpriteGroupOffsetIndex, name: nameof(SpriteGroupOffsetIndex));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));

            // TODO: Get number of entries - this doesn't always work
            States = s.SerializeObjectArray<GBA_ActorState>(States, (BlockSize - 12) / 8, name: nameof(States));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            SpriteGroup = s.DoAt(OffsetTable.GetPointer(SpriteGroupOffsetIndex), () => s.SerializeObject<GBA_DES>(SpriteGroup, name: nameof(SpriteGroup)));

            // TODO: Parse data for each state
        }
    }

    public class GBA_ActorState : R1Serializable
    {
        // Byte_07 seems to be offset index (it's not used if Byte_06 is -1)
        // Byte_06 seems to determine how the data structure from the offset should look like - struct type?
        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, 8, name: nameof(UnkData));
        }
    }

    public class GBA_DES : GBA_BaseBlock
    {
        #region Data

        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte TileMapOffsetIndex { get; set; }
        public byte PaletteOffsetIndex { get; set; }
        public byte UnkOffsetIndex3 { get; set; }
        public byte Byte_04 { get; set; }
        public byte AnimationsCount { get; set; }
        public byte Byte_06 { get; set; }

        public byte[] SpritesIndexTable { get; set; }

        #endregion

        #region Parsed

        public GBA_SpritePalette Palette { get; set; }
        public GBA_SpriteTileMap TileMap { get; set; }
        public GBA_Animation[] Animations { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            TileMapOffsetIndex = s.Serialize<byte>(TileMapOffsetIndex, name: nameof(TileMapOffsetIndex));
            PaletteOffsetIndex = s.Serialize<byte>(PaletteOffsetIndex, name: nameof(PaletteOffsetIndex));
            if (s.GameSettings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || s.GameSettings.EngineVersion == EngineVersion.StarWarsGBA)
            {
                UnkOffsetIndex3 = s.Serialize<byte>(UnkOffsetIndex3, name: nameof(UnkOffsetIndex3));
            }
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));

            SpritesIndexTable = s.SerializeArray<byte>(SpritesIndexTable, AnimationsCount, name: nameof(SpritesIndexTable));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Palette = s.DoAt(OffsetTable.GetPointer(PaletteOffsetIndex), () => s.SerializeObject<GBA_SpritePalette>(Palette, name: nameof(Palette)));
            TileMap = s.DoAt(OffsetTable.GetPointer(TileMapOffsetIndex), () => s.SerializeObject<GBA_SpriteTileMap>(TileMap, name: nameof(TileMap)));

            if (Animations == null)
                Animations = new GBA_Animation[AnimationsCount];

            for (int i = 0; i < Animations.Length; i++)
                Animations[i] = s.DoAt(OffsetTable.GetPointer(SpritesIndexTable[i]), () => s.SerializeObject<GBA_Animation>(Animations[i], name: $"{nameof(Animations)}[{i}]"));
        }

        #endregion
    }


    // Not sure this is an animation
    public class GBA_Animation : GBA_BaseBlock
    {
        // First byte is struct type?

        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, BlockSize, name: nameof(UnkData));
        }
    }
}