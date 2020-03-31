namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_LevFile : R1Serializable
    {
        // All pointers lead to allfix
        public Pointer UnknownPointer1 { get; set; }
        public Pointer UnknownPointer2 { get; set; }
        public uint Unk1 { get; set; }
        public Pointer UnknownPointer3 { get; set; }
        public Pointer UnknownPointer4 { get; set; }
        public Pointer UnknownPointer5 { get; set; }
        public Pointer UnknownPointer6 { get; set; }
        public byte[] Unk2 { get; set; }

        public Pointer EventsPointer { get; set; }
        public Pointer UnknownEventTablePointer { get; set; }
        public uint EventCount { get; set; }
        public Pointer EventLinkTablePointer { get; set; }
        public uint EvenLinkCount { get; set; }

        public PS1_R1JPDemoVol3_Event[] Events { get; set; }
        public PS1_R1JPDemoVol3_UnknownEventTableItem[] UnknownEventTable { get; set; }
        public byte[] EventLinkTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            UnknownPointer1 = s.SerializePointer(UnknownPointer1, name: nameof(UnknownPointer1));
            UnknownPointer2 = s.SerializePointer(UnknownPointer2, name: nameof(UnknownPointer2));
            Unk1 = s.Serialize(Unk1, name: nameof(Unk1));
            UnknownPointer3 = s.SerializePointer(UnknownPointer3, name: nameof(UnknownPointer3));
            UnknownPointer4 = s.SerializePointer(UnknownPointer4, name: nameof(UnknownPointer4));
            UnknownPointer5 = s.SerializePointer(UnknownPointer5, name: nameof(UnknownPointer5));
            UnknownPointer6 = s.SerializePointer(UnknownPointer6, name: nameof(UnknownPointer6));
            Unk2 = s.SerializeArray(Unk2, 128, name: nameof(Unk1));

            // Serialize event information
            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));
            UnknownEventTablePointer = s.SerializePointer(UnknownEventTablePointer, name: nameof(UnknownEventTablePointer));
            EventCount = s.Serialize(EventCount, name: nameof(EventCount));
            EventLinkTablePointer = s.SerializePointer(EventLinkTablePointer, name: nameof(EventLinkTablePointer));
            EvenLinkCount = s.Serialize(EvenLinkCount, name: nameof(EvenLinkCount));

            // Serialize data from pointers
            s.DoAt(EventsPointer, () =>
            {
                Events = s.SerializeObjectArray<PS1_R1JPDemoVol3_Event>(Events, EventCount, name: nameof(Events));
            });
            s.DoAt(UnknownEventTablePointer, () =>
            {
                UnknownEventTable = s.SerializeObjectArray<PS1_R1JPDemoVol3_UnknownEventTableItem>(UnknownEventTable, EventCount, name: nameof(UnknownEventTable));
            });
            s.DoAt(EventLinkTablePointer, () =>
            {
                EventLinkTable = s.SerializeArray<byte>(EventLinkTable, EvenLinkCount, name: nameof(EventLinkTable));
            });
        }
    }
}