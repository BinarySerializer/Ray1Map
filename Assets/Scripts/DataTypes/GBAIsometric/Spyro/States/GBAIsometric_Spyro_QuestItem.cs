namespace R1Engine
{
    public class GBAIsometric_Spyro_QuestItem : R1Serializable
    {
        public ushort ObjectType { get; set; }
        public byte Byte_02 { get; set; }
        public byte AnimFrameIndex { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            AnimFrameIndex = s.Serialize<byte>(AnimFrameIndex, name: nameof(AnimFrameIndex));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            s.Serialize<ushort>(default, name: "Padding");
        }
    }
}