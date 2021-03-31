using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Isometric_TargetObject : BinarySerializable
    {
        public GBAVV_Isometric_TargetObjType ObjType { get; set; }
        public FixedPointInt XPos { get; set; }
        public FixedPointInt YPos { get; set; }
        public FixedPointInt TargetXPos { get; set; }
        public FixedPointInt TargetYPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<GBAVV_Isometric_TargetObjType>(ObjType, name: nameof(ObjType));
            XPos = s.SerializeObject<FixedPointInt>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt>(YPos, name: nameof(YPos));
            TargetXPos = s.SerializeObject<FixedPointInt>(TargetXPos, name: nameof(TargetXPos));
            TargetYPos = s.SerializeObject<FixedPointInt>(TargetYPos, name: nameof(TargetYPos));
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