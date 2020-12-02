namespace R1Engine
{
    public class GBC_Level : GBC_BaseBlock
    {
        public byte LevelIndex { get; set; }
        public sbyte InGameLevelIndex { get; set; } // The order of the levels in-game (-1 for optional levels)
        public byte[] UnkData0 { get; set; }
        public byte LinkedLevelsCount { get; set; }
        public bool IsGift { get; set; }
        public sbyte LinkedLevelsStartIndex { get; set; } // Actually start index + 1 (-1 if there are no linked levels)
        public byte[] UnkData1 { get; set; }
        public byte[] LinkedLevels { get; set; } // level block index = LinkedLevelsStartIndex - 1 + LinkedLevels[i]
        
        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            LevelIndex = s.Serialize<byte>(LevelIndex, name: nameof(LevelIndex));
            InGameLevelIndex = s.Serialize<sbyte>(InGameLevelIndex, name: nameof(InGameLevelIndex));
            UnkData0 = s.SerializeArray<byte>(UnkData0, 2, name: nameof(UnkData0));
            LinkedLevelsCount = s.Serialize<byte>(LinkedLevelsCount, name: nameof(LinkedLevelsCount));
            IsGift = s.Serialize<bool>(IsGift, name: nameof(IsGift));
            LinkedLevelsStartIndex = s.Serialize<sbyte>(LinkedLevelsStartIndex, name: nameof(LinkedLevelsStartIndex));
            UnkData1 = s.SerializeArray<byte>(UnkData1, 59, name: nameof(UnkData1));
            LinkedLevels = s.SerializeArray<byte>(LinkedLevels, LinkedLevelsCount, name: nameof(LinkedLevels));

            // TODO: Parse remaining data

            // Parse data from pointers
            Scene = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }
    }
}