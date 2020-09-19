namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2LevDataFile : R1Serializable
    {
        #region Level Data

        public int Unk1 { get; set; }
        public int Unk2 { get; set; }

        /// <summary>
        /// Pointer to the events
        /// </summary>
        public Pointer EventsPointer { get; set; }

        /// <summary>
        /// Pointer to the always events
        /// </summary>
        public Pointer AlwaysEventsPointer { get; set; }

        /// <summary>
        /// Pointer to the allfix image descriptors
        /// </summary>
        public Pointer FixImageDescriptorsPointer { get; set; }
        
        public byte[] Unk3 { get; set; }

        /// <summary>
        /// The event count
        /// </summary>
        public ushort EventCount { get; set; }

        /// <summary>
        /// The always event count
        /// </summary>
        public ushort AlwaysEventsCount { get; set; }

        public ushort UShort_36 { get; set; }
        public ushort UShort_38 { get; set; }
        public ushort UShort_3A { get; set; }

        /// <summary>
        /// The number of allfix image descriptors
        /// </summary>
        public ushort NumFixImageDescriptors { get; set; }

        public ushort UShort_3E { get; set; }
        public ushort UShort_40 { get; set; }
        public ushort UShort_42 { get; set; }
        public ushort UShort_44 { get; set; }
        public ushort UShort_46 { get; set; }
        public uint UInt_48 { get; set; }

        public Pointer ZDCPointer { get; set; }
        public Pointer UnkPointer4 { get; set; }
        public Pointer UnkPointer5 { get; set; }
        public Pointer UnkPointer6 { get; set; }

        /// <summary>
        /// The event link table. This does not include the always events.
        /// </summary>
        public ushort[] EventLinkTable { get; set; }

        #endregion

        #region Parsed Data

        /// <summary>
        /// The events
        /// </summary>
        public R1_R2EventData[] Events { get; set; }
        
        /// <summary>
        /// The always events. These do not have map positions.
        /// </summary>
        public R1_R2EventData[] AlwaysEvents { get; set; }

        /// <summary>
        /// The allfix image descriptors
        /// </summary>
        public R1_ImageDescriptor[] FixImageDescriptors { get; set; }

        public R1_ZDCData[] ZDC { get; set; }

        #endregion

        #region Public Methods

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
            FixImageDescriptorsPointer = s.SerializePointer(FixImageDescriptorsPointer, name: nameof(FixImageDescriptorsPointer));

            Unk3 = s.SerializeArray<byte>(Unk3, 30, name: nameof(Unk3));

            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));

            AlwaysEventsCount = s.Serialize<ushort>(AlwaysEventsCount, name: nameof(AlwaysEventsCount));
            UShort_36 = s.Serialize<ushort>(UShort_36, name: nameof(UShort_36));
            UShort_38 = s.Serialize<ushort>(UShort_38, name: nameof(UShort_38));
            UShort_3A = s.Serialize<ushort>(UShort_3A, name: nameof(UShort_3A));
            NumFixImageDescriptors = s.Serialize<ushort>(NumFixImageDescriptors, name: nameof(NumFixImageDescriptors));
            UShort_3E = s.Serialize<ushort>(UShort_3E, name: nameof(UShort_3E));
            UShort_40 = s.Serialize<ushort>(UShort_40, name: nameof(UShort_40));
            UShort_42 = s.Serialize<ushort>(UShort_42, name: nameof(UShort_42));
            UShort_44 = s.Serialize<ushort>(UShort_44, name: nameof(UShort_44));
            UShort_46 = s.Serialize<ushort>(UShort_46, name: nameof(UShort_46));
            UInt_48 = s.Serialize<uint>(UInt_48, name: nameof(UInt_48));
            
            ZDCPointer = s.SerializePointer(ZDCPointer, name: nameof(ZDCPointer));
            UnkPointer4 = s.SerializePointer(UnkPointer4, name: nameof(UnkPointer4));
            UnkPointer5 = s.SerializePointer(UnkPointer5, name: nameof(UnkPointer5));
            UnkPointer6 = s.SerializePointer(UnkPointer6, name: nameof(UnkPointer6));

            EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, EventCount, name: nameof(EventLinkTable));

            s.DoAt(FixImageDescriptorsPointer, () => FixImageDescriptors = s.SerializeObjectArray<R1_ImageDescriptor>(FixImageDescriptors, NumFixImageDescriptors, name: nameof(FixImageDescriptors)));

            s.DoAt(EventsPointer, () => Events = s.SerializeObjectArray<R1_R2EventData>(Events, EventCount, name: nameof(Events)));
            s.DoAt(AlwaysEventsPointer, () => AlwaysEvents = s.SerializeObjectArray<R1_R2EventData>(AlwaysEvents, AlwaysEventsCount, name: nameof(AlwaysEvents)));

            // TODO: Is there a length?
            s.DoAt(ZDCPointer, () => ZDC = s.SerializeObjectArray<R1_ZDCData>(ZDC, 237, name: nameof(ZDC)));
        }

        #endregion
    }
}