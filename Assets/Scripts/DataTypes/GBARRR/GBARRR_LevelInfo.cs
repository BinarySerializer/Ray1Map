namespace R1Engine
{
    public class GBARRR_LevelInfo : R1Serializable
    {
        public uint TileMapIndex { get; set; }
        public uint Index_01 { get; set; }
        public uint Index_02 { get; set; }
        public uint AlphaTileMapIndex { get; set; }
        public uint BG1TileMapIndex { get; set; }
        public uint Index_05 { get; set; } // Some scene-like struct with objects
        public uint SceneIndex { get; set; }
        public uint CollisionMapIndex { get; set; }
        public uint LevelMapIndex { get; set; }
        public uint AlphaBlendingMapIndex { get; set; }
        public uint Index_0A { get; set; } // 512 bytes - palette?

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapIndex = s.Serialize<uint>(TileMapIndex, name: nameof(TileMapIndex));
            Index_01 = s.Serialize<uint>(Index_01, name: nameof(Index_01));
            Index_02 = s.Serialize<uint>(Index_02, name: nameof(Index_02));
            AlphaTileMapIndex = s.Serialize<uint>(AlphaTileMapIndex, name: nameof(AlphaTileMapIndex));
            BG1TileMapIndex = s.Serialize<uint>(BG1TileMapIndex, name: nameof(BG1TileMapIndex));
            Index_05 = s.Serialize<uint>(Index_05, name: nameof(Index_05));
            SceneIndex = s.Serialize<uint>(SceneIndex, name: nameof(SceneIndex));
            CollisionMapIndex = s.Serialize<uint>(CollisionMapIndex, name: nameof(CollisionMapIndex));
            LevelMapIndex = s.Serialize<uint>(LevelMapIndex, name: nameof(LevelMapIndex));
            AlphaBlendingMapIndex = s.Serialize<uint>(AlphaBlendingMapIndex, name: nameof(AlphaBlendingMapIndex));
            Index_0A = s.Serialize<uint>(Index_0A, name: nameof(Index_0A));
        }
    }
}