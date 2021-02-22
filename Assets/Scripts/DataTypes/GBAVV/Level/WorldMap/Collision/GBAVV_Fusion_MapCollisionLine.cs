namespace R1Engine
{
    public class GBAVV_Fusion_MapCollisionLine : R1Serializable
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Direction { get; set; }
        public Pointer Pointer_14 { get; set; } // Types?

        public override void SerializeImpl(SerializerObject s)
        {
            X1 = s.Serialize<int>(X1, name: nameof(X1));
            Y1 = s.Serialize<int>(Y1, name: nameof(Y1));
            X2 = s.Serialize<int>(X2, name: nameof(X2));
            Y2 = s.Serialize<int>(Y2, name: nameof(Y2));
            Direction = s.Serialize<int>(Direction, name: nameof(Direction));
            Pointer_14 = s.SerializePointer(Pointer_14, name: nameof(Pointer_14));
        }
    }
}