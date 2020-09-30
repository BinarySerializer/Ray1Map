namespace R1Engine
{
    public class R1_R2UnknownAnimData : R1Serializable
    {
        public ushort Ushort_00 { get; set; } // Some index?
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public ushort Ushort_04 { get; set; }
        public sbyte Byte_06 { get; set; }
        public sbyte Byte_07 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            Byte_06 = s.Serialize<sbyte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<sbyte>(Byte_07, name: nameof(Byte_07));
        }
    }
}