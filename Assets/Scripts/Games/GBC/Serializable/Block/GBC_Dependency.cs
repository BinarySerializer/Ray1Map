using BinarySerializer;

namespace Ray1Map.GBC
{
    public class GBC_Dependency : BinarySerializable
    {
        public LUDI_FileIdentifier FileID { get; set; }
        public LUDI_BlockIdentifier BlockID { get; set; }

        public GBC_Pointer GBC_Pointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_Palm || s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_PocketPC)
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