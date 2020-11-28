namespace R1Engine
{
    public class GBC_Level : GBC_BaseBlock
    {
        public byte LevelIndex { get; set; }
        public byte[] UnkData0 { get; set; }
        public byte LinkedLevelsCount { get; set; }
        public byte[] UnkData1 { get; set; } // This data is identical across GBC and PalmOS, so most likely no 16/32-bit values
        public byte[] LinkedLevels { get; set; } // exit block index = exit + 1
        
        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            LevelIndex = s.Serialize<byte>(LevelIndex, name: nameof(LevelIndex));
            UnkData0 = s.SerializeArray<byte>(UnkData0, 3, name: nameof(UnkData0));
            LinkedLevelsCount = s.Serialize<byte>(LinkedLevelsCount, name: nameof(LinkedLevelsCount));
            UnkData1 = s.SerializeArray<byte>(UnkData1, 61, name: nameof(UnkData0));
            LinkedLevels = s.SerializeArray<byte>(LinkedLevels, LinkedLevelsCount, name: nameof(LinkedLevels));

            // TODO: Parse remaining data

            // Parse data from pointers
            Scene = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }
    }
}