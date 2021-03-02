namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_DataFile : R1Serializable
    {
        public int BlocksCount { get; set; }
        public GBAVV_NitroKart_NGage_DataFileEntry[] DataFileEntries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            BlocksCount = s.Serialize<int>(BlocksCount, name: nameof(BlocksCount));
            DataFileEntries = s.SerializeObjectArray<GBAVV_NitroKart_NGage_DataFileEntry>(DataFileEntries, BlocksCount, name: nameof(DataFileEntries));
        }

        public class GBAVV_NitroKart_NGage_DataFileEntry : R1Serializable
        {
            public uint Uint_00 { get; set; }
            public int BlockOffset { get; set; }
            public int BlockLength { get; set; }

            public Pointer Pointer => Offset.file.StartPointer + BlockOffset;

            public override void SerializeImpl(SerializerObject s)
            {
                Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
                BlockOffset = s.Serialize<int>(BlockOffset, name: nameof(BlockOffset));
                BlockLength = s.Serialize<int>(BlockLength, name: nameof(BlockLength));
            }
        }
    }
}