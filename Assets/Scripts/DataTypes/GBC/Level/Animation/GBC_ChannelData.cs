namespace R1Engine
{
    public class GBC_ChannelData : GBC_BaseBlock
    {
        public GBC_ChannelEvent CountEvent { get; set; }
        public GBC_ChannelEvent HeaderEvent { get; set; }
        public GBC_ChannelEvent[] Events { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            CountEvent = s.SerializeObject<GBC_ChannelEvent>(CountEvent, name: nameof(CountEvent));
            HeaderEvent = s.SerializeObject<GBC_ChannelEvent>(HeaderEvent, name: nameof(HeaderEvent));
            Events = s.SerializeObjectArray<GBC_ChannelEvent>(Events, CountEvent.EventType - 2, name: nameof(Events));
        }
    }
}