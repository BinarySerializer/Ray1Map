namespace R1Engine
{
    public class GBACrash_Mode7_Object : R1Serializable
    {
        public byte ObjType_Normal { get; set; }
        public byte ObjType_TimeAttack0 { get; set; }
        public byte ObjType_TimeAttack1 { get; set; }
        public byte Byte_03 { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Int_10 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType_Normal = s.Serialize<byte>(ObjType_Normal, name: nameof(ObjType_Normal));
            ObjType_TimeAttack0 = s.Serialize<byte>(ObjType_TimeAttack0, name: nameof(ObjType_TimeAttack0));
            ObjType_TimeAttack1 = s.Serialize<byte>(ObjType_TimeAttack1, name: nameof(ObjType_TimeAttack1));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            XPos = s.Serialize<int>(XPos, name: nameof(XPos));
            YPos = s.Serialize<int>(YPos, name: nameof(YPos));
            ZPos = s.Serialize<int>(ZPos, name: nameof(ZPos));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
        }
    }
}