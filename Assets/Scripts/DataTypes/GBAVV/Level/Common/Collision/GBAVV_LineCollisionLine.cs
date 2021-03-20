namespace R1Engine
{
    public class GBAVV_LineCollisionLine : R1Serializable
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Direction { get; set; }
        public Pointer CollisionDataPointer { get; set; }

        // Serialized from pointers
        public GBAVV_Fusion_MapCollisionLineData CollisionData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_BrotherBear)
            {
                X1 = s.Serialize<short>((short)X1, name: nameof(X1));
                Y1 = s.Serialize<short>((short)Y1, name: nameof(Y1));
                X2 = s.Serialize<short>((short)X2, name: nameof(X2));
                Y2 = s.Serialize<short>((short)Y2, name: nameof(Y2));
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