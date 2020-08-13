namespace R1Engine
{
    public class GBA_ActorState : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public byte AnimationIndex { get; set; }
        public byte Byte_05 { get; set; }

        // Related to the struct Byte_07 points to
        public sbyte Byte_06 { get; set; }

        // Some offset index
        public byte Byte_07 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            Byte_06 = s.Serialize<sbyte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
        }
    }
}