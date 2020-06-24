namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1 - Japan Demo)
    /// </summary>
    public class PS1_R1JPDemo_LevFile : R1Serializable
    {
        // All pointers lead to allfix
        public Pointer FontImageDescriptorsPointer { get; set; }
        public Pointer FontImageBufferPointer { get; set; }
        public uint NumFontImageDescriptors { get; set; }
        public Pointer UnknownImageDesciptorsPointer { get; set; } // 0x100 image descriptors
        public Pointer UnknownPointer4 { get; set; }
        public Pointer UnknownImageBufferPointer { get; set; } // UnknownImageDescriptors describes this
        public Pointer UnknownPointer6 { get; set; }
        public byte[] Unk2 { get; set; }

        public Pointer EventsPointer { get; set; }
        public Pointer UnknownEventTablePointer { get; set; }
        public uint EventCount { get; set; }
        public Pointer EventLinkTablePointer { get; set; }
        public uint EvenLinkCount { get; set; }

        public EventData[] Events { get; set; }
        public PS1_R1JPDemoVol3_UnknownEventTableItem[] UnknownEventTable { get; set; }
        public byte[] EventLinkTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            FontImageDescriptorsPointer = s.SerializePointer(FontImageDescriptorsPointer, name: nameof(FontImageDescriptorsPointer));
            FontImageBufferPointer = s.SerializePointer(FontImageBufferPointer, name: nameof(FontImageBufferPointer));
            NumFontImageDescriptors = s.Serialize<uint>(NumFontImageDescriptors, name: nameof(NumFontImageDescriptors));
            UnknownImageDesciptorsPointer = s.SerializePointer(UnknownImageDesciptorsPointer, name: nameof(UnknownImageDesciptorsPointer));
            UnknownPointer4 = s.SerializePointer(UnknownPointer4, name: nameof(UnknownPointer4));
            UnknownImageBufferPointer = s.SerializePointer(UnknownImageBufferPointer, name: nameof(UnknownImageBufferPointer));
            UnknownPointer6 = s.SerializePointer(UnknownPointer6, name: nameof(UnknownPointer6));
            Unk2 = s.SerializeArray<byte>(Unk2, 128, name: nameof(Unk2));

            // Serialize event information
            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
                UnknownEventTablePointer = s.SerializePointer(UnknownEventTablePointer, name: nameof(UnknownEventTablePointer));
            EventCount = s.Serialize<uint>(EventCount, name: nameof(EventCount));
            EventLinkTablePointer = s.SerializePointer(EventLinkTablePointer, name: nameof(EventLinkTablePointer));
            EvenLinkCount = s.Serialize<uint>(EvenLinkCount, name: nameof(EvenLinkCount));

            // Serialize data from pointers
            s.DoAt(EventsPointer, () =>
            {
                Events = s.SerializeObjectArray<EventData>(Events, EventCount, name: nameof(Events));
            });

            if (UnknownEventTablePointer != null)
            {
                s.DoAt(UnknownEventTablePointer, () =>
                {
                    UnknownEventTable = s.SerializeObjectArray<PS1_R1JPDemoVol3_UnknownEventTableItem>(UnknownEventTable, EventCount, name: nameof(UnknownEventTable));
                });
            }
            
            s.DoAt(EventLinkTablePointer, () =>
            {
                EventLinkTable = s.SerializeArray<byte>(EventLinkTable, EvenLinkCount, name: nameof(EventLinkTable));
            });
        }
    }
}