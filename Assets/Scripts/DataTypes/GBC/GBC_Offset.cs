namespace R1Engine
{
    public class GBC_Offset : R1Serializable
    {
        public ushort UnkFileID { get; set; }
        public ushort FileID { get; set; }
        public ushort BlockID { get; set; }
        public ushort UShort_06 { get; set; } // Padding?

        public GBC_Pointer GBC_Pointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                UnkFileID = s.Serialize<ushort>(UnkFileID, name: nameof(UnkFileID));
                FileID = s.Serialize<ushort>(FileID, name: nameof(FileID));
                BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
                UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            }
            else
            {
                GBC_Pointer = s.SerializeObject<GBC_Pointer>(GBC_Pointer, name: nameof(GBC_Pointer));
            }
        }
    }
}