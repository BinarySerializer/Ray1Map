namespace R1Engine
{
    public class GBACrash_Isometric_Object : R1Serializable
    {
        public GBACrash_Isometric_ObjType ObjType { get; set; }
        public GBACrash_Isometric_ObjType ObjType_TimeTrial { get; set; }
        public FixedPointInt XPos { get; set; }
        public FixedPointInt YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<GBACrash_Isometric_ObjType>(ObjType, name: nameof(ObjType));
            ObjType_TimeTrial = s.Serialize<GBACrash_Isometric_ObjType>(ObjType_TimeTrial, name: nameof(ObjType_TimeTrial));
            XPos = s.SerializeObject<FixedPointInt>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt>(YPos, name: nameof(YPos));
        }

        public enum GBACrash_Isometric_ObjType
        {
            None = -1,
            Invalid = 0,
            // 1-5 are crates, max is 0x15
        }
    }
}