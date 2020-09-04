namespace R1Engine
{
    public class GBA_UnkActorStruct : R1Serializable
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, 16, name: nameof(Data));
        }
    }
}