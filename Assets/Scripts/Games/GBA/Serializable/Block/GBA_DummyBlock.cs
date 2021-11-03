using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_DummyBlock : GBA_BaseBlock
    {
        public byte[] Data { get; set; }
        public GBA_DummyBlock[] SubBlocks { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
		public override void SerializeOffsetData(SerializerObject s) {
			base.SerializeOffsetData(s);
            if (SubBlocks == null) {
                SubBlocks = new GBA_DummyBlock[OffsetTable.OffsetsCount];
            }
            for (int i = 0; i < OffsetTable.Offsets.Length; i++) {
                s.DoAt(OffsetTable.GetPointer(i), () => {
                    SubBlocks[i] = s.SerializeObject<GBA_DummyBlock>(SubBlocks[i], name: $"{nameof(SubBlocks)}[{i}]");
                });
            }
		}

	}
}