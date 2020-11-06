namespace R1Engine
{
    public class GBAIsometric_Spyro_LocDecompress : R1Serializable
    {
        public byte b0 { get; set; }
        public byte b1 { get; set; }
        public byte b2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            b0 = s.Serialize<byte>(b0, name: nameof(b0));
            b1 = s.Serialize<byte>(b1, name: nameof(b1));
            b2 = s.Serialize<byte>(b2, name: nameof(b2));
        }
    }
}