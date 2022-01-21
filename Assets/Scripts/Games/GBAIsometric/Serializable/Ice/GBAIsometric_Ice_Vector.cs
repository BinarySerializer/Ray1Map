using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Vector : BinarySerializable
    {
        public GBAIsometric_Ice_Vector() { }

        public GBAIsometric_Ice_Vector(int x, int y, int height)
        {
            X = new FixedPointInt32() { Value = x, Pre_PointPosition = 10 };
            Y = new FixedPointInt32() { Value = y, Pre_PointPosition = 10 };
            Height = new FixedPointInt32() { Value = height, Pre_PointPosition = 10 };
        }

        public GBAIsometric_Ice_Vector(FixedPointInt32 x, FixedPointInt32 y, FixedPointInt32 height)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public FixedPointInt32 X { get; set; }
        public FixedPointInt32 Y { get; set; }
        public FixedPointInt32 Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.SerializeObject<FixedPointInt32>(X, x => x.Pre_PointPosition = 10, name: nameof(X));
            Y = s.SerializeObject<FixedPointInt32>(Y, x => x.Pre_PointPosition = 10, name: nameof(Y));
            Height = s.SerializeObject<FixedPointInt32>(Height, x => x.Pre_PointPosition = 10, name: nameof(Height));
        }
    }
}