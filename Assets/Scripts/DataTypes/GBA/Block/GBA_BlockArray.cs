namespace R1Engine
{
    public class GBA_BlockArray<T> : GBA_BaseBlock
        where T : GBA_BaseBlock, new()
    {
        public T[] Blocks { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize blocks
            if (Blocks == null)
                Blocks = new T[OffsetTable.OffsetsCount];

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<T>(Blocks[i], name: $"{nameof(Blocks)}[{i}]"));
        }
    }
}