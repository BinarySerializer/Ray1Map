namespace R1Engine
{
    public class GBC_HeaderChannelEvent : GBC_BaseChannelEvent
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            Data = s.SerializeArray<byte>(Data, EventDataSize, name: nameof(Data));
        }
    }
}