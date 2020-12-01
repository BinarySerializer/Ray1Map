namespace R1Engine
{
    public class GBC_Level : GBC_BaseBlock
    {
        public byte LevelIndex { get; set; }
        public byte InGameLevelIndex { get; set; } // The order of the levels in-game
        public byte[] UnkData0 { get; set; }
        public byte LinkedLevelsCount { get; set; }
        public byte[] UnkData1 { get; set; } // Second byte seems to determine the cutscene types before the level starts?
        public byte[] LinkedLevels { get; set; } // exit block index = exit + 1
        
        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            LevelIndex = s.Serialize<byte>(LevelIndex, name: nameof(LevelIndex));
            InGameLevelIndex = s.Serialize<byte>(InGameLevelIndex, name: nameof(InGameLevelIndex));
            UnkData0 = s.SerializeArray<byte>(UnkData0, 2, name: nameof(UnkData0));
            LinkedLevelsCount = s.Serialize<byte>(LinkedLevelsCount, name: nameof(LinkedLevelsCount));
            UnkData1 = s.SerializeArray<byte>(UnkData1, 61, name: nameof(UnkData1));
            LinkedLevels = s.SerializeArray<byte>(LinkedLevels, LinkedLevelsCount, name: nameof(LinkedLevels));

            // TODO: Parse remaining data

            // Parse data from pointers
            Scene = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }
    }
}