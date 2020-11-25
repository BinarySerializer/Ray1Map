namespace R1Engine
{
    public class GBC_DummyBlock : GBC_BaseBlock
    {
        public GBC_DummyBlock[] SubBlocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Check to make sure the offset table count is reasonable
            var isValid = s.DoAt(s.CurrentPointer, () =>
            {
                var count = s.Serialize<uint>(default, name: "OffsetsCount");

                return count < 50;
            });

            if (!isValid)
            {
                OffsetTable = new GBC_OffsetTable();
                SubBlocks = new GBC_DummyBlock[0];
                return;
            }

            SerializeOffsetTable(s);

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