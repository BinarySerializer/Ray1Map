using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro2_State_ChallengePortal : BinarySerializable
    {
        public ushort ObjectType { get; set; }
        public ushort SpawnerObjectType { get; set; }
        public byte LevelID_0 { get; set; }
        public byte LevelID_1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            SpawnerObjectType = s.Serialize<ushort>(SpawnerObjectType, name: nameof(SpawnerObjectType));
            LevelID_0 = s.Serialize<byte>(LevelID_0, name: nameof(LevelID_0));
            LevelID_1 = s.Serialize<byte>(LevelID_1, name: nameof(LevelID_1));
            s.Serialize<ushort>(default, name: "Padding");
        }
    }
}