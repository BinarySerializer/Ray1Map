namespace R1Engine
{
    public class GBA_BoxTriggerActorData : R1Serializable
    {
        public ushort UShort_00 { get; set; }
        public byte LinkedActor { get; set; }
        public byte Byte_03 { get; set; } // Padding?
        public ushort UShort_04 { get; set; }

        // PoP and beyond
        public byte Type { get; set; }
        public byte UnkBits { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }
        public byte Byte_06 { get; set; }
        public byte Byte_07 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia) {
                Type = s.Serialize<byte>(Type, name: nameof(Type));
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));

                s.SerializeBitValues<byte>(bitFunc => {
                    LinkedActor = (byte)bitFunc(LinkedActor, 6, name: nameof(LinkedActor));
                    UnkBits = (byte)bitFunc(UnkBits, 2, name: nameof(UnkBits));
                });
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
                Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
                Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
            } else {
                UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
                LinkedActor = s.Serialize<byte>(LinkedActor, name: nameof(LinkedActor));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));
            }
        }
    }
}