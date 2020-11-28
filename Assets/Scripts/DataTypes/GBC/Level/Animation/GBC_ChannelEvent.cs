namespace R1Engine
{
    public class GBC_ChannelEvent : R1Serializable
    {
        public byte EventType { get; set; }
        public byte EventDataSize { get; set; }
        public byte[] EventData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            EventType = s.Serialize<byte>(EventType, name: nameof(EventType));
            EventDataSize = s.Serialize<byte>(EventDataSize, name: nameof(EventDataSize));
            EventData = s.SerializeArray<byte>(EventData, EventDataSize, name: nameof(EventData));
        }
    }
}