using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_ListEvents : BinarySerializable
    {
        public EVE_Track Track { get; set; } // Set before serializing
        public uint EventsCount { get; set; }

        public EVE_Event[] Events { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            EventsCount = s.Serialize<uint>(EventsCount, name: nameof(EventsCount));

			// Serialize each event separately. They need to be able to access the first event in the list during serialization
			if (Events == null)
                Events = new EVE_Event[EventsCount];

            for (int i = 0; i < Events.Length; i++)
            {
                Events[i] = s.SerializeObject<EVE_Event>(Events[i], x =>
                {
                    x.ListEvents = this;
                    x.Index = i;
                }, name: $"{nameof(Events)}[{i}]");
            }
        }
    }
}