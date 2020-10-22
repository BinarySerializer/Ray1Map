namespace R1Engine
{
    public class GBARRR_LevelInfo : R1Serializable
    {
        public uint LevelTilesetIndex { get; set; }

        public uint BG0MapIndex { get; set; }
        public uint BG0TilesetIndex { get; set; }

        public uint FGTilesetIndex { get; set; }

        public uint BG1TilesetIndex { get; set; }
        public uint BG1MapIndex { get; set; }

        public uint SceneIndex { get; set; }

        public uint CollisionMapIndex { get; set; }
        public uint LevelMapIndex { get; set; }
        public uint FGMapIndex { get; set; }

        public uint SpritePaletteIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelTilesetIndex = s.Serialize<uint>(LevelTilesetIndex, name: nameof(LevelTilesetIndex));
            BG0MapIndex = s.Serialize<uint>(BG0MapIndex, name: nameof(BG0MapIndex));
            BG0TilesetIndex = s.Serialize<uint>(BG0TilesetIndex, name: nameof(BG0TilesetIndex));
            FGTilesetIndex = s.Serialize<uint>(FGTilesetIndex, name: nameof(FGTilesetIndex));
            BG1TilesetIndex = s.Serialize<uint>(BG1TilesetIndex, name: nameof(BG1TilesetIndex));
            BG1MapIndex = s.Serialize<uint>(BG1MapIndex, name: nameof(BG1MapIndex));
            SceneIndex = s.Serialize<uint>(SceneIndex, name: nameof(SceneIndex));
            CollisionMapIndex = s.Serialize<uint>(CollisionMapIndex, name: nameof(CollisionMapIndex));
            LevelMapIndex = s.Serialize<uint>(LevelMapIndex, name: nameof(LevelMapIndex));
            FGMapIndex = s.Serialize<uint>(FGMapIndex, name: nameof(FGMapIndex));
            SpritePaletteIndex = s.Serialize<uint>(SpritePaletteIndex, name: nameof(SpritePaletteIndex));
        }
    }
}