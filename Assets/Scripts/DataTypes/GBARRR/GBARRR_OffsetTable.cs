using System;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBARRR_OffsetTable : R1Serializable
    {
        public uint OffsetTableCount { get; set; }
        public GBARRR_OffsetTableEntry[] OffsetTable { get; set; }

        public void DoAtBlock(Context context, long index, Action<uint> action)
        {
            var offset = OffsetTable[index];
            var pointer = Offset + offset.BlockOffset;
            var s = context.Deserializer;

            var header = s.DoAt(pointer, () => s.Serialize<uint>(default, name: "BlockHeaderCheck"));

            if (header == 0x1234567)
                s.DoAt(pointer, () => s.DoEncoded(new LZSSEncoder(offset.BlockSize), () => action(offset.BlockSize)));
            else
                s.DoAt(pointer, () => action(offset.BlockSize));
        }

        public override void SerializeImpl(SerializerObject s)
        {
            OffsetTableCount = s.Serialize<uint>(OffsetTableCount, name: nameof(OffsetTableCount));
            OffsetTable = s.SerializeObjectArray<GBARRR_OffsetTableEntry>(OffsetTable, OffsetTableCount, name: nameof(OffsetTable));
        }
    }
}