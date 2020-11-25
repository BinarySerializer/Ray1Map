namespace R1Engine
{
    public class GBC_DummyBlock : GBC_BaseBlock
    {
        public byte[] Data { get; set; }
        public GBC_DummyBlock[] SubBlocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
                Data = s.DoAt(Offset, () => s.SerializeArray<byte>(Data, GBC_BlockLength, name: nameof(Data)));

            // Serialize sub-blocks
            if (SubBlocks == null)
                SubBlocks = new GBC_DummyBlock[OffsetTable.OffsetsCount];
            for (int i = 0; i < OffsetTable.Offsets.Length; i++)
            {
                s.DoAt(OffsetTable.GetPointer(i), () => {
                    SubBlocks[i] = s.SerializeObject<GBC_DummyBlock>(SubBlocks[i], name: $"{nameof(SubBlocks)}[{i}]");
                });
            }
        }
    }
}