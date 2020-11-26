namespace R1Engine
{
    public abstract class GBC_BaseBlock : R1Serializable 
    {
        public byte[] GBC_Bytes_00 { get; set; }
        public ushort GBC_DataLength { get; set; } // The size of the data in the block
        public GBC_Pointer GBC_DataPointer { get; set; } // Offset to the end of the offset table (using the current memory bank). The game uses this to skip the offset table and start parsing the data.

        public GBC_PalmOS_BlockHeader PalmOS_Header { get; set; }

        public GBC_OffsetTable OffsetTable { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
            {
                GBC_Bytes_00 = s.SerializeArray<byte>(GBC_Bytes_00, 5, name: nameof(GBC_Bytes_00));
                GBC_DataLength = s.Serialize<ushort>(GBC_DataLength, name: nameof(GBC_DataLength));
                GBC_DataPointer = s.SerializeObject<GBC_Pointer>(GBC_DataPointer, onPreSerialize: x => x.HasMemoryBankValue = false, name: nameof(GBC_DataPointer));
            }
            else
            {
                PalmOS_Header = s.SerializeObject<GBC_PalmOS_BlockHeader>(PalmOS_Header, name: nameof(PalmOS_Header));
            }

            OffsetTable = s.SerializeObject<GBC_OffsetTable>(OffsetTable, name: nameof(OffsetTable));
        }
    }
}