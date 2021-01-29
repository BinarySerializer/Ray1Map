namespace R1Engine
{
    public class GBAVV_Mode7_Object : R1Serializable
    {
        public byte ObjType_Normal { get; set; }
        public byte ObjType_TimeTrial { get; set; }
        public byte ObjType_Unknown { get; set; }
        public byte Byte_03 { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Int_10 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType_Normal = s.Serialize<byte>(ObjType_Normal, name: nameof(ObjType_Normal));
            ObjType_TimeTrial = s.Serialize<byte>(ObjType_TimeTrial, name: nameof(ObjType_TimeTrial));
            ObjType_Unknown = s.Serialize<byte>(ObjType_Unknown, name: nameof(ObjType_Unknown));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
            ZPos = s.Serialize<int>(ZPos, name: nameof(ZPos));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
        }
    }
}