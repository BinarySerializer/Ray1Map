namespace R1Engine
{
    public class GBACrash_Isometric_CollisionTile : R1Serializable
    {
        public FixedPointInt Height { get; set; }
        public int TypeIndex { get; set; }
        public FixedPointInt Int_08 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Height = s.SerializeObject<FixedPointInt>(Height, name: nameof(Height));
            TypeIndex = s.Serialize<int>(TypeIndex, name: nameof(TypeIndex));
            Int_08 = s.SerializeObject<FixedPointInt>(Int_08, name: nameof(Int_08));
        }
    }
}