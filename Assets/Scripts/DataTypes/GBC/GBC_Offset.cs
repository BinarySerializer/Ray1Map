namespace R1Engine
{
    public class GBC_Offset : R1Serializable
    {
        public ushort UnkFileIndex { get; set; }
        public ushort FileIndex { get; set; }
        public ushort BlockIndex { get; set; }
        public ushort UShort_06 { get; set; } // Padding?

        public GBC_Pointer GBC_Pointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                UnkFileIndex = s.Serialize<ushort>(UnkFileIndex, name: nameof(UnkFileIndex));
                FileIndex = s.Serialize<ushort>(FileIndex, name: nameof(FileIndex));
                BlockIndex = s.Serialize<ushort>(BlockIndex, name: nameof(BlockIndex));
                UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            }
            else
            {
                GBC_Pointer = s.SerializeObject<GBC_Pointer>(GBC_Pointer, name: nameof(GBC_Pointer));
            }
        }
    }
}