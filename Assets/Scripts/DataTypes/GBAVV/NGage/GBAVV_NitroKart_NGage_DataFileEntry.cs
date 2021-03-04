namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_DataFileEntry : R1Serializable
    {
        public uint CRC { get; set; }
        public Pointer BlockPointer { get; set; }
        public int BlockLength { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            CRC = s.Serialize<uint>(CRC, name: nameof(CRC));
            BlockPointer = s.SerializePointer(BlockPointer, name: nameof(BlockPointer));
            BlockLength = s.Serialize<int>(BlockLength, name: nameof(BlockLength));
        }
    }
}