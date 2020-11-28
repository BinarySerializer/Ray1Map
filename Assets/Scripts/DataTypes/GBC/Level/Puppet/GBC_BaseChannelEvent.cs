namespace R1Engine
{
    public class GBC_BaseChannelEvent : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte DataSize { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            DataSize = s.Serialize<byte>(DataSize, name: nameof(DataSize));
        }
    }
}