namespace R1Engine
{
    public class GBARRR_ROM : GBA_ROMBase
    {
        public GBARRR_OffsetTable OffsetTable { get; set; }
        public GBARRR_LevelInfo[] LevelInfo { get; set; }
        public GBARRR_LocalizationBlock Localization { get; set; }

        public GBARRR_TileMap TileMap { get; set; }

        public GBARRR_Scene LevelScene { get; set; }
        public GBARRR_MapBlock CollisionMap { get; set; }
        public GBARRR_MapBlock LevelMap { get; set; }
        public GBARRR_MapBlock LightMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get pointer table
            var pointerTable = PointerTables.GBARRR_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize offset table
            OffsetTable = s.DoAt(pointerTable[GBARRR_Pointer.OffsetTable], () => s.SerializeObject<GBARRR_OffsetTable>(OffsetTable, name: nameof(OffsetTable)));

            // Serialize level info
            LevelInfo = s.DoAt(pointerTable[GBARRR_Pointer.LevelInfo], () => s.SerializeObjectArray<GBARRR_LevelInfo>(LevelInfo, 29, name: nameof(LevelInfo)));

            // Serialize localization
            OffsetTable.DoAtBlock(s.Context, 3, size =>
                Localization = s.SerializeObject<GBARRR_LocalizationBlock>(Localization, name: nameof(Localization)));

            // Serialize tile map
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].TileMapIndex, size =>
                TileMap = s.SerializeObject<GBARRR_TileMap>(TileMap, name: nameof(TileMap)));

            // Serialize level scene
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].SceneIndex, size => LevelScene = s.SerializeObject<GBARRR_Scene>(LevelScene, name: nameof(LevelScene)));

            // Serialize maps
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].CollisionMapIndex, size => 
                CollisionMap = s.SerializeObject<GBARRR_MapBlock>(CollisionMap, name: nameof(CollisionMap), onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Collision));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].LevelMapIndex, size => 
                LevelMap = s.SerializeObject<GBARRR_MapBlock>(LevelMap, name: nameof(LevelMap), onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Tiles));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].LightMapIndex, size => 
                LightMap = s.SerializeObject<GBARRR_MapBlock>(LightMap, name: nameof(LightMap), onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Light));
        }
    }
}