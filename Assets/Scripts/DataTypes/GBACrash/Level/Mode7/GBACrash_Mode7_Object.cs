namespace R1Engine
{
    public class GBACrash_Mode7_Object : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Int_10 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
            ZPos = s.Serialize<int>(ZPos, name: nameof(ZPos));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
        }
    }
}