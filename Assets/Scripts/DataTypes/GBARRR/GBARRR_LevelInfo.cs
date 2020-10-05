namespace R1Engine
{
    public class GBARRR_LevelInfo : R1Serializable
    {
        public uint TileMapIndex { get; set; }
        public uint Index_01 { get; set; }
        public uint Index_02 { get; set; }
        public uint Index_03 { get; set; }
        public uint Index_04 { get; set; }
        public uint Index_05 { get; set; }
        public uint SceneIndex { get; set; }
        public uint CollisionMapIndex { get; set; }
        public uint LevelMapIndex { get; set; }
        public uint LightMapIndex { get; set; }
        public uint Index_0A { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapIndex = s.Serialize<uint>(TileMapIndex, name: nameof(TileMapIndex));
            Index_01 = s.Serialize<uint>(Index_01, name: nameof(Index_01));
            Index_02 = s.Serialize<uint>(Index_02, name: nameof(Index_02));
            Index_03 = s.Serialize<uint>(Index_03, name: nameof(Index_03));
            Index_04 = s.Serialize<uint>(Index_04, name: nameof(Index_04));
            Index_05 = s.Serialize<uint>(Index_05, name: nameof(Index_05));
            SceneIndex = s.Serialize<uint>(SceneIndex, name: nameof(SceneIndex));
            CollisionMapIndex = s.Serialize<uint>(CollisionMapIndex, name: nameof(CollisionMapIndex));
            LevelMapIndex = s.Serialize<uint>(LevelMapIndex, name: nameof(LevelMapIndex));
            LightMapIndex = s.Serialize<uint>(LightMapIndex, name: nameof(LightMapIndex));
            Index_0A = s.Serialize<uint>(Index_0A, name: nameof(Index_0A));
        }
    }
}