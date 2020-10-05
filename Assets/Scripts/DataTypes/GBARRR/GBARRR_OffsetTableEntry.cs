namespace R1Engine
{
    public class GBARRR_OffsetTableEntry : R1Serializable
    {
        public uint Unk1 { get; set; }
        public uint BlockSize { get; set; }
        public uint BlockOffset { get; set; }
        public uint Unk2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            BlockOffset = s.Serialize<uint>(BlockOffset, name: nameof(BlockOffset));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
        }
    }
}