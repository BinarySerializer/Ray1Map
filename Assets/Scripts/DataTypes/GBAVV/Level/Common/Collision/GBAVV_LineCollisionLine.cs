using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_LineCollisionLine : BinarySerializable
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public FixedPointInt FixedPoint_X1 { get; set; }
        public FixedPointInt FixedPoint_Y1 { get; set; }
        public FixedPointInt FixedPoint_X2 { get; set; }
        public FixedPointInt FixedPoint_Y2 { get; set; }
        public int Direction { get; set; }
        public Pointer CollisionDataPointer { get; set; }

        // Helpers
        public float GetX1 => FixedPoint_X1?.AsFloat ?? X1;
        public float GetY1 => FixedPoint_Y1?.AsFloat ?? Y1;
        public float GetX2 => FixedPoint_X2?.AsFloat ?? X2;
        public float GetY2 => FixedPoint_Y2?.AsFloat ?? Y2;

        // Serialized from pointers
        public GBAVV_Fusion_MapCollisionLineData CollisionData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_BrotherBear)
            {
                X1 = s.Serialize<short>((short)X1, name: nameof(X1));
                Y1 = s.Serialize<short>((short)Y1, name: nameof(Y1));
                X2 = s.Serialize<short>((short)X2, name: nameof(X2));
                Y2 = s.Serialize<short>((short)Y2, name: nameof(Y2));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_UltimateSpiderMan || 
                     (s.GetR1Settings().EngineVersion >= EngineVersion.GBAVV_OverTheHedge && s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_ShrekTheThird))
            {
                FixedPoint_X1 = s.SerializeObject<FixedPointInt>(FixedPoint_X1, x => x.PointPosition = 8, name: nameof(FixedPoint_X1));
                FixedPoint_Y1 = s.SerializeObject<FixedPointInt>(FixedPoint_Y1, x => x.PointPosition = 8, name: nameof(FixedPoint_Y1));
                FixedPoint_X2 = s.SerializeObject<FixedPointInt>(FixedPoint_X2, x => x.PointPosition = 8, name: nameof(FixedPoint_X2));
                FixedPoint_Y2 = s.SerializeObject<FixedPointInt>(FixedPoint_Y2, x => x.PointPosition = 8, name: nameof(FixedPoint_Y2));
            }
            else
            {
                X1 = s.Serialize<int>(X1, name: nameof(X1));
                Y1 = s.Serialize<int>(Y1, name: nameof(Y1));
                X2 = s.Serialize<int>(X2, name: nameof(X2));
                Y2 = s.Serialize<int>(Y2, name: nameof(Y2));
            }
            Direction = s.Serialize<int>(Direction, name: nameof(Direction));
            CollisionDataPointer = s.SerializePointer(CollisionDataPointer, name: nameof(CollisionDataPointer));

            CollisionData = s.DoAt(CollisionDataPointer, () => s.SerializeObject<GBAVV_Fusion_MapCollisionLineData>(CollisionData, x => x.IsSingleValue = Direction != 3 && Direction != 4, name: nameof(CollisionData)));
        }
    }
}