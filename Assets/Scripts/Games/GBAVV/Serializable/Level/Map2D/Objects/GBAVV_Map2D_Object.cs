using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Map2D_Object : BinarySerializable
    {
        public short ObjType { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public ushort ObjParamsIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<short>(ObjType, name: nameof(ObjType));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            ObjParamsIndex = s.Serialize<ushort>(ObjParamsIndex, name: nameof(ObjParamsIndex));
        }
    }
}