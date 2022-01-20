using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_Object : BinarySerializable
    {
        public short ObjectType { get; set; }
        public ushort Ushort_02 { get; set; } // Unused?
        public FixedPointInt32 XPosition { get; set; }
        public FixedPointInt32 YPosition { get; set; }
        public FixedPointInt32 Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<short>(ObjectType, name: nameof(ObjectType));
            Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            XPosition = s.SerializeObject<FixedPointInt32>(XPosition, x => x.Pre_PointPosition = 10, name: nameof(XPosition));
            YPosition = s.SerializeObject<FixedPointInt32>(YPosition, x => x.Pre_PointPosition = 10, name: nameof(YPosition));
            Height = s.SerializeObject<FixedPointInt32>(Height, x => x.Pre_PointPosition = 10, name: nameof(Height));
        }
    }
}