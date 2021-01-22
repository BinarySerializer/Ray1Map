namespace R1Engine
{
    public class GBACrash_Isometric_Position : R1Serializable
    {
        public FixedPointInt XPos { get; set; }
        public FixedPointInt YPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.SerializeObject<FixedPointInt>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt>(YPos, name: nameof(YPos));
        }
    }
}