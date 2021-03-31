using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro_LevelNameInfo : BinarySerializable
    {
        public ushort LevelID { get; set; }
        public GBAIsometric_LocIndex PrimaryName { get; set; }
        public GBAIsometric_LocIndex SecondaryName { get; set; }
        public uint Uint_08 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));
            PrimaryName = s.SerializeObject<GBAIsometric_LocIndex>(PrimaryName, name: nameof(PrimaryName));
            SecondaryName = s.SerializeObject<GBAIsometric_LocIndex>(SecondaryName, name: nameof(SecondaryName));
            s.Serialize<ushort>(default, name: "Padding");
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
        }
    }
}