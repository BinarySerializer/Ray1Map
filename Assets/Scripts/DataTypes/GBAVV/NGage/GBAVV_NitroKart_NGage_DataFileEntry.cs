namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_DataFileEntry : R1Serializable
    {
        public uint Uint_00 { get; set; }
        public Pointer BlockPointer { get; set; }
        public int BlockLength { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            BlockPointer = s.SerializePointer(BlockPointer, name: nameof(BlockPointer));
            BlockLength = s.Serialize<int>(BlockLength, name: nameof(BlockLength));
        }
    }
}