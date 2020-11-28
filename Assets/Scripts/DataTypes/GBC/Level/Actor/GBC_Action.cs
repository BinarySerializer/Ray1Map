namespace R1Engine
{
    public class GBC_Action : R1Serializable {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte AnimIndex { get; set; } // Block index to use in GBC_Puppet. 0 = no animation?

        public override void SerializeImpl(SerializerObject s) {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            AnimIndex = s.Serialize<byte>(AnimIndex, name: nameof(AnimIndex));
        }
    }
}