namespace R1Engine
{
    public class GBAIsometric_UnkStruct : R1Serializable
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
        }
    }
}