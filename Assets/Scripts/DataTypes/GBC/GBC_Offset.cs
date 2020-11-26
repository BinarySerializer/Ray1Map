namespace R1Engine
{
    public class GBC_Offset : R1Serializable
    {
        public LUDI_FileIdentifier FileID { get; set; }
        public LUDI_BlockIdentifier BlockID { get; set; }

        public GBC_Pointer GBC_Pointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                FileID = s.SerializeObject<LUDI_FileIdentifier>(FileID, name: nameof(FileID));
                BlockID = s.SerializeObject<LUDI_BlockIdentifier>(BlockID, name: nameof(BlockID));
            }
            else
            {
                GBC_Pointer = s.SerializeObject<GBC_Pointer>(GBC_Pointer, name: nameof(GBC_Pointer));
            }
        }
    }
}