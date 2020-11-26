namespace R1Engine
{
    public class GBC_ActorLink : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte ActorIndex { get; set; }
        public byte Byte_02 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            ActorIndex = s.Serialize<byte>(ActorIndex, name: nameof(ActorIndex));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
        }
    }
}