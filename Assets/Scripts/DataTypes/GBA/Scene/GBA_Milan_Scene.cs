namespace R1Engine
{
    public class GBA_Milan_Scene : GBA_BaseBlock
    {
        public ushort Ushort_00 { get; set; }

        // Parsed from offsets
        public GBA_PlayField PlayField { get; set; }
        public GBA_Milan_ActorsBlock ActorsBlock { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            PlayField = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));
            ActorsBlock = s.DoAt(OffsetTable.GetPointer(1), () => s.SerializeObject<GBA_Milan_ActorsBlock>(ActorsBlock, name: nameof(ActorsBlock)));

            // Block 4 is captors
        }
    }
}