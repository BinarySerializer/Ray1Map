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
            Crate_Normal = 1,
            Crate_QuestionMark = 2,
            Crate_Nitro = 3,
            Crate_Life = 4,
            Crate_Checkpoint = 5,
            Crate_NitroSwitch = 6,
            TimeTrialClock = 7,
            Crate_Time1 = 8,
            Crate_Time2 = 9,
            Crate_Time3 = 10,
            Wumpa = 11,
            Crystal = 12,
            Gem = 13,
            Exit = 14,
            Type_15 = 15,
            Type_16 = 16,
            LaserGate_Left = 17,
            LaserGate_Right = 18,
            Type_19 = 19,
            Type_20 = 20,
            TutorialMessage = 21,
        }
    }
}