using System;
using BinarySerializer;


namespace R1Engine
{
    public class GBARRR_OffsetTable : BinarySerializable
    {
        public uint OffsetTableCount { get; set; }
        public GBARRR_OffsetTableEntry[] OffsetTable { get; set; }

        public void DoAtBlock(Context context, long index, Action<uint> action)
        {
            var offset = OffsetTable[index];
            var pointer = Offset + offset.BlockOffset;
            var s = context.Deserializer;

            if (offset.Header == GBARRR_OffsetTableEntryHeader.Compressed)
                s.DoAt(pointer, () => s.DoEncoded(new LZSSEncoder(offset.BlockSize), () => action(s.CurrentLength)));
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