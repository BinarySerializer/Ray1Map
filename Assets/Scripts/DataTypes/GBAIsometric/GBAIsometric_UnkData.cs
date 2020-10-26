namespace R1Engine
{
    public class GBAIsometric_UnkData : R1Serializable
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, 12, name: nameof(Data));
        }
    }
}