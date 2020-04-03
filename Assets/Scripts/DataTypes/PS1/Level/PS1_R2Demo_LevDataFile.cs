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

        public Pointer UnkPointer1 { get; set; }
        public Pointer UnkPointer2 { get; set; }

        public byte[] Unk3 { get; set; }

        /// <summary>
        /// The event count
        /// </summary>
        public ushort EventCount { get; set; }

        public byte[] Unk4 { get; set; }

        public Pointer UnkPointer3 { get; set; }
        public Pointer UnkPointer4 { get; set; }
        public Pointer UnkPointer5 { get; set; }
        public Pointer UnkPointer6 { get; set; }

        public ushort[] EventLinkTable { get; set; }
        
        public PS1_R2Demo_Event[] Events { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<int>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<int>(Unk2, name: nameof(Unk2));

            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));

            UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: nameof(UnkPointer2));

            Unk3 = s.SerializeArray<byte>(Unk3, 30, name: nameof(Unk3));

            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));
            
            Unk4 = s.SerializeArray<byte>(Unk4, 24, name: nameof(Unk4));
            
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));
            UnkPointer4 = s.SerializePointer(UnkPointer4, name: nameof(UnkPointer4));
            UnkPointer5 = s.SerializePointer(UnkPointer5, name: nameof(UnkPointer5));
            UnkPointer6 = s.SerializePointer(UnkPointer6, name: nameof(UnkPointer6));

            EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, EventCount, name: nameof(EventLinkTable));

            s.DoAt(EventsPointer, () =>
            {
                // TODO: Are these part of the first event?
                var dummy = s.SerializeArray(new byte[0], 12);

                Events = s.SerializeObjectArray<PS1_R2Demo_Event>(Events, EventCount, name: nameof(Events));
            });
        }
    }
}