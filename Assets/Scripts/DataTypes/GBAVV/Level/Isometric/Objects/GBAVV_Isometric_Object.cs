using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Isometric_Object : BinarySerializable
    {
        public GBAVV_Isometric_ObjType ObjType { get; set; }
        public GBAVV_Isometric_ObjType ObjType_TimeTrial { get; set; }
        public FixedPointInt32 XPos { get; set; }
        public FixedPointInt32 YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<GBAVV_Isometric_ObjType>(ObjType, name: nameof(ObjType));
            ObjType_TimeTrial = s.Serialize<GBAVV_Isometric_ObjType>(ObjType_TimeTrial, name: nameof(ObjType_TimeTrial));
            XPos = s.SerializeObject<FixedPointInt32>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt32>(YPos, name: nameof(YPos));
        }

        public enum GBAVV_Isometric_ObjType
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
            Crate_QuestionMark_Multiplayer = 15,
            Crate_Nitro_Multiplayer = 16,
            LaserGate_Left = 17,
            LaserGate_Right = 18,
            LaserGate_Left_Flipped = 19,
            LaserGate_Right_Flipped = 20,
            TutorialMessage = 21,
        }
    }
}