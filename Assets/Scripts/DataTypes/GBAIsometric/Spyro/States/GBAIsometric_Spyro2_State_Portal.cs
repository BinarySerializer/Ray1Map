using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro2_State_Portal : BinarySerializable
    {
        public ushort ObjectType { get; set; }
        public ushort SpawnerObjectType { get; set; }
        public ushort LevelID { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            SpawnerObjectType = s.Serialize<ushort>(SpawnerObjectType, name: nameof(SpawnerObjectType));
            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));
            s.Serialize<ushort>(default, name: "Padding");
        }
    }
}