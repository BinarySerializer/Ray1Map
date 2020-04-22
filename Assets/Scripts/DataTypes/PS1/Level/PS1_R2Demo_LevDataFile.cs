namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_LevDataFile : R1Serializable
    {
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }

        /// <summary>
        /// Pointer to the events
        /// </summary>
        public Pointer EventsPointer { get; set; }

        public Pointer AlwaysEventsPointer { get; set; }
        public Pointer ImageDescriptorsPointer { get; set; }
        public byte[] Unk3 { get; set; }

        /// <summary>
        /// The event count
        /// </summary>
        public ushort EventCount { get; set; }

        public ushort EventCount2 { get; set; }
        public ushort UShort_36 { get; set; }
        public ushort UShort_38 { get; set; }
        public ushort UShort_3A { get; set; }
        public ushort NumImageDescriptors { get; set; }
        public ushort UShort_3E { get; set; }
        public ushort UShort_40 { get; set; }
        public ushort UShort_42 { get; set; }
        public ushort UShort_44 { get; set; }
        public ushort UShort_46 { get; set; }
        public uint UInt_48 { get; set; }

        public Pointer UnkPointer3 { get; set; }
        public Pointer UnkPointer4 { get; set; }
        public Pointer UnkPointer5 { get; set; }
        public Pointer UnkPointer6 { get; set; }

        public ushort[] EventLinkTable { get; set; }
        
        public PS1_R2Demo_Event[] Events { get; set; }
        public PS1_R2Demo_Event[] AlwaysEvents { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<int>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<int>(Unk2, name: nameof(Unk2));

            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));

            AlwaysEventsPointer = s.SerializePointer(AlwaysEventsPointer, name: nameof(AlwaysEventsPointer));
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));

            Unk3 = s.SerializeArray<byte>(Unk3, 30, name: nameof(Unk3));

            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));

            EventCount2 = s.Serialize<ushort>(EventCount2, name: nameof(EventCount2));
            UShort_36 = s.Serialize<ushort>(UShort_36, name: nameof(UShort_36));
            UShort_38 = s.Serialize<ushort>(UShort_38, name: nameof(UShort_38));
            UShort_3A = s.Serialize<ushort>(UShort_3A, name: nameof(UShort_3A));
            NumImageDescriptors = s.Serialize<ushort>(NumImageDescriptors, name: nameof(NumImageDescriptors));
            UShort_3E = s.Serialize<ushort>(UShort_3E, name: nameof(UShort_3E));
            UShort_40 = s.Serialize<ushort>(UShort_40, name: nameof(UShort_40));
            UShort_42 = s.Serialize<ushort>(UShort_42, name: nameof(UShort_42));
            UShort_44 = s.Serialize<ushort>(UShort_44, name: nameof(UShort_44));
            UShort_46 = s.Serialize<ushort>(UShort_46, name: nameof(UShort_46));
            UInt_48 = s.Serialize<uint>(UInt_48, name: nameof(UInt_48));
            
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));
            UnkPointer4 = s.SerializePointer(UnkPointer4, name: nameof(UnkPointer4));
            UnkPointer5 = s.SerializePointer(UnkPointer5, name: nameof(UnkPointer5));
            UnkPointer6 = s.SerializePointer(UnkPointer6, name: nameof(UnkPointer6));

            EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, EventCount, name: nameof(EventLinkTable));

            s.DoAt(EventsPointer, () =>
            {
                Events = s.SerializeObjectArray<PS1_R2Demo_Event>(Events, EventCount, name: nameof(Events));
            });
            s.DoAt(AlwaysEventsPointer, () => {
                AlwaysEvents = s.SerializeObjectArray<PS1_R2Demo_Event>(AlwaysEvents, EventCount2, name: nameof(AlwaysEvents));
            });
        }
    }
}