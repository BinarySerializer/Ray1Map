using BinarySerializer;

namespace R1Engine
{
    public class GBARRR_OffsetTableEntry : BinarySerializable
    {
        public GBARRR_OffsetTableEntryHeader Header { get; set; }
        public uint BlockSize { get; set; }
        public uint BlockOffset { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.Serialize<GBARRR_OffsetTableEntryHeader>(Header, name: nameof(Header));
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            BlockOffset = s.Serialize<uint>(BlockOffset, name: nameof(BlockOffset));
            s.Serialize<uint>(0, name: "Padding");
        }
    }
}