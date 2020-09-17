namespace R1Engine
{
    public class GBA_BoxTriggerActorData : R1Serializable
    {
        public ushort UShort_00 { get; set; }
        public byte LinkedActor { get; set; }
        public byte Byte_03 { get; set; } // Padding?
        public ushort UShort_04 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            LinkedActor = s.Serialize<byte>(LinkedActor, name: nameof(LinkedActor));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));
        }
    }
}