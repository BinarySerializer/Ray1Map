namespace R1Engine
{
    public class GBA_Shanghai_KnotsBlock : R1Serializable
    {
        public long Length { get; set; } // Set before serializing

        public GBC_Knot[] Knots { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Knots = s.SerializeObjectArray<GBC_Knot>(Knots, Length, name: nameof(Knots));
        }
    }
}