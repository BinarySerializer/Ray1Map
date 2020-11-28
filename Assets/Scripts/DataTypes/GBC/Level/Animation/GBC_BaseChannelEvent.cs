namespace R1Engine
{
    public class GBC_BaseChannelEvent : R1Serializable
    {
        public byte EventType { get; set; }
        public byte EventDataSize { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            EventType = s.Serialize<byte>(EventType, name: nameof(EventType));
            EventDataSize = s.Serialize<byte>(EventDataSize, name: nameof(EventDataSize));
        }
    }
}