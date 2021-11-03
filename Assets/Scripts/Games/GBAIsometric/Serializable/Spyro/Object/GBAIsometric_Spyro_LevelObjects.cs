using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_LevelObjects : BinarySerializable
    {
        public ushort LevelID { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex ObjectTableIndex { get; set; }

        // Parsed
        public GBAIsometric_Spyro_ObjectTable ObjectTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));
            ObjectTableIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjectTableIndex, name: nameof(ObjectTableIndex));

            ObjectTable = ObjectTableIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_ObjectTable>(ObjectTable, name: nameof(ObjectTable)));
        }
    }
}