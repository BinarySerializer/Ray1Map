namespace R1Engine
{
    public abstract class GBC_BaseBlock : R1Serializable 
    {
        public byte[] GBC_Bytes_00 { get; set; }
        public ushort GBC_BlockLength { get; set; } // The total size of the block
        public ushort GBC_DataOffset { get; set; } // Offset to the end of the offset table (using the current memory bank). The game uses this to skip the offset table and start parsing the data.

        public ushort BlockID { get; set; }
        public ushort Padding { get; set; }
        public GBC_OffsetTable OffsetTable { get; set; } // Not always here

        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
            {
                GBC_Bytes_00 = s.SerializeArray<byte>(GBC_Bytes_00, 5, name: nameof(GBC_Bytes_00));
                GBC_BlockLength = s.Serialize<ushort>(GBC_BlockLength, name: nameof(GBC_BlockLength));
                GBC_DataOffset = s.Serialize<ushort>(GBC_DataOffset, name: nameof(GBC_DataOffset));
            }
            else
            {
                BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
                Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));
            }
        }

        protected void SerializeOffsetTable(SerializerObject s) {
            OffsetTable = s.SerializeObject<GBC_OffsetTable>(OffsetTable, name: nameof(OffsetTable));
        }
    }
}