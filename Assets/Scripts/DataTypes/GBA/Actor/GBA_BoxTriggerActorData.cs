namespace R1Engine
{
    public class GBA_BoxTriggerActorData : GBA_BaseBlock
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte LinkedActor { get; set; }
        public byte Byte_03 { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            LinkedActor = s.Serialize<byte>(LinkedActor, name: nameof(LinkedActor));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
        }
    }
}