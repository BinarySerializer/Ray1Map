using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Isometric_TargetObject : BinarySerializable
    {
        public GBAVV_Isometric_TargetObjType ObjType { get; set; }
        public FixedPointInt32 XPos { get; set; }
        public FixedPointInt32 YPos { get; set; }
        public FixedPointInt32 TargetXPos { get; set; }
        public FixedPointInt32 TargetYPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<GBAVV_Isometric_TargetObjType>(ObjType, name: nameof(ObjType));
            XPos = s.SerializeObject<FixedPointInt32>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt32>(YPos, name: nameof(YPos));
            TargetXPos = s.SerializeObject<FixedPointInt32>(TargetXPos, name: nameof(TargetXPos));
            TargetYPos = s.SerializeObject<FixedPointInt32>(TargetYPos, name: nameof(TargetYPos));
        }

        public enum GBAVV_Isometric_TargetObjType
        {
            None = -1,
            Invalid = 0,
            Barrel = 1,
            Laser = 2
        }
    }
}