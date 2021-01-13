namespace R1Engine
{
    public class GBACrash_Mode7_Object : R1Serializable
    {
        public byte ObjType_0 { get; set; }
        public byte ObjType_1 { get; set; }
        public byte ObjType_2 { get; set; }
        public byte Byte_03 { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Int_10 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType_0 = s.Serialize<byte>(ObjType_0, name: nameof(ObjType_0));
            ObjType_1 = s.Serialize<byte>(ObjType_1, name: nameof(ObjType_1));
            ObjType_2 = s.Serialize<byte>(ObjType_2, name: nameof(ObjType_2));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
            ZPos = s.Serialize<int>(ZPos, name: nameof(ZPos));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
        }
    }
}