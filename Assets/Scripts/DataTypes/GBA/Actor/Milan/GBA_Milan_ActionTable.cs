namespace R1Engine
{
    public class GBA_Milan_ActionTable : GBA_BaseBlock
    {
        // Parsed from offsets
        public GBA_Milan_ActionBlock[] ActionBlocks { get; set; }

        public override void SerializeBlock(SerializerObject s) { }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (ActionBlocks == null)
                ActionBlocks = new GBA_Milan_ActionBlock[OffsetTable.OffsetsCount];

            for (int i = 0; i < ActionBlocks.Length; i++)
                ActionBlocks[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_Milan_ActionBlock>(ActionBlocks[i], name: $"{nameof(ActionBlocks)}[{i}]"));
        }
    }
}