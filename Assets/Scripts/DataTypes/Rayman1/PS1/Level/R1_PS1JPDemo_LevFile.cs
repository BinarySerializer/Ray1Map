namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1 - Japan Demo)
    /// </summary>
    public class R1_PS1JPDemo_LevFile : R1Serializable
    {
        // All pointers lead to allfix
        public R1_PS1_FontData FontData { get; set; }
        public R1_EventData RaymanEvent { get; set; }

        public Pointer EventsPointer { get; set; }
        public Pointer UnknownEventTablePointer { get; set; }
        public uint EventCount { get; set; }
        public Pointer EventLinkTablePointer { get; set; }
        public uint EvenLinkCount { get; set; }

        public R1_EventData[] Events { get; set; }
        public R1_PS1JPDemoVol3_UnknownEventTableItem[] UnknownEventTable { get; set; }
        public byte[] EventLinkTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize font data
            FontData = s.SerializeObject<R1_PS1_FontData>(FontData, name: nameof(FontData));
            
            // Serialize fixed Rayman event
            RaymanEvent = s.SerializeObject<R1_EventData>(RaymanEvent, name: nameof(RaymanEvent));

            // Serialize event information
            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                UnknownEventTablePointer = s.SerializePointer(UnknownEventTablePointer, name: nameof(UnknownEventTablePointer));
            EventCount = s.Serialize<uint>(EventCount, name: nameof(EventCount));
            EventLinkTablePointer = s.SerializePointer(EventLinkTablePointer, name: nameof(EventLinkTablePointer));
            EvenLinkCount = s.Serialize<uint>(EvenLinkCount, name: nameof(EvenLinkCount));

            // Serialize data from pointers
            s.DoAt(EventsPointer, () => Events = s.SerializeObjectArray<R1_EventData>(Events, EventCount, name: nameof(Events)));

            if (UnknownEventTablePointer != null)
                s.DoAt(UnknownEventTablePointer, () => UnknownEventTable = s.SerializeObjectArray<R1_PS1JPDemoVol3_UnknownEventTableItem>(UnknownEventTable, EventCount, name: nameof(UnknownEventTable)));
            
            s.DoAt(EventLinkTablePointer, () => EventLinkTable = s.SerializeArray<byte>(EventLinkTable, EvenLinkCount, name: nameof(EventLinkTable)));
        }
    }
}