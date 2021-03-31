using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro_UnkStruct : BinarySerializable
    {
        public ushort ID { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<ushort>(ID, name: nameof(ID));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            s.Serialize<ushort>(default, "Padding");
        }
    }
}