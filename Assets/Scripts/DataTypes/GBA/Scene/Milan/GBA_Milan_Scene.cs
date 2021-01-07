namespace R1Engine
{
    public class GBA_Milan_Scene : GBA_BaseBlock
    {
        public ushort Ushort_00 { get; set; }

        // Parsed from offsets
        public GBA_PlayField PlayField { get; set; }
        public GBA_Milan_ActorsBlock ActorsBlock { get; set; }
        public GBA_Palette TomClancy_TilePalette { get; set; }
        public GBA_Palette TomClancy_ObjPalette { get; set; }
        public GBA_DummyBlock Block_02 { get; set; }
        public GBA_DummyBlock Block_03 { get; set; }
        public GBA_Milan_ActorsBlock CaptorsBlock { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            var blockIndex = 0;

            PlayField = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));

            if (s.GameSettings.EngineVersion == EngineVersion.GBA_TomClancysRainbowSixRogueSpear)
            {
                TomClancy_TilePalette = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_Palette>(TomClancy_TilePalette, name: nameof(TomClancy_TilePalette)));
                TomClancy_ObjPalette = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_Palette>(TomClancy_ObjPalette, name: nameof(TomClancy_ObjPalette)));
            }

            ActorsBlock = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_Milan_ActorsBlock>(ActorsBlock, name: nameof(ActorsBlock)));

            Block_02 = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_DummyBlock>(Block_02, name: nameof(Block_02)));
            Block_03 = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_DummyBlock>(Block_03, name: nameof(Block_03)));

            CaptorsBlock = s.DoAt(OffsetTable.GetPointer(blockIndex++), () => s.SerializeObject<GBA_Milan_ActorsBlock>(CaptorsBlock, x => x.IsCaptor = true, name: nameof(CaptorsBlock)));
        }
    }
}