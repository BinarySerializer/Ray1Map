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
    }
}