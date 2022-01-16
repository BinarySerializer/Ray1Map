using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_LevelObjects : BinarySerializable
    {
        public ushort LevelID { get; set; }
        public GBAIsometric_IceDragon_ResourceRef ObjectTableIndex { get; set; }

        // Parsed
        public GBAIsometric_Spyro_ObjectTable ObjectTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));
            ObjectTableIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(ObjectTableIndex, name: nameof(ObjectTableIndex));

            ObjectTableIndex.DoAt(size => ObjectTable = s.SerializeObject<GBAIsometric_Spyro_ObjectTable>(ObjectTable, name: nameof(ObjectTable)));
        }
    }
}