using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_MapCollisionLine : BinarySerializable
    {
        // Normal?
        public short UnkX { get; set; }
        public short UnkY { get; set; }
        public FixedPointInt32 Int_04 { get; set; }

        public short X1 { get; set; }
        public short Y1 { get; set; }
        
        public short X2 { get; set; }
        public short Y2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkX = s.Serialize<short>(UnkX, name: nameof(UnkX));
            UnkY = s.Serialize<short>(UnkY, name: nameof(UnkY));
            Int_04 = s.SerializeObject<FixedPointInt32>(Int_04, x => x.Pre_PointPosition = 10, name: nameof(Int_04));
            X1 = s.Serialize<short>(X1, name: nameof(X1));
            Y1 = s.Serialize<short>(Y1, name: nameof(Y1));
            X2 = s.Serialize<short>(X2, name: nameof(X2));
            Y2 = s.Serialize<short>(Y2, name: nameof(Y2));
        }
    }
}