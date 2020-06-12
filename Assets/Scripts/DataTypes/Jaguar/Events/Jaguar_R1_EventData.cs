namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventData : R1Serializable
    {
        // Set this before serializing
        public bool IsAlways { get; set; }

        // 0 if invalid event, 1 if valid?
        public ushort IsValid { get; set; }

        // Offsets for the position
        public ushort OffsetX { get; set; }
        public ushort OffsetY { get; set; }

        public uint DESPointer { get; set; }

        // Always 0x05?
        public ushort Unk_0A { get; set; }

        // Some index?
        public ushort Unk_0C { get; set; }

        public ushort AlwaysEventsCount { get; set; }

        public Jaguar_R1_EventData[] AlwaysEvents { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (!IsAlways)
                IsValid = s.Serialize<ushort>(IsValid, name: nameof(IsValid));

            OffsetX = s.Serialize<ushort>(OffsetX, name: nameof(OffsetX));
            OffsetY = s.Serialize<ushort>(OffsetY, name: nameof(OffsetY));
            DESPointer = s.Serialize<uint>(DESPointer, name: nameof(DESPointer));
            Unk_0A = s.Serialize<ushort>(Unk_0A, name: nameof(Unk_0A));
            Unk_0C = s.Serialize<ushort>(Unk_0C, name: nameof(Unk_0C));
            AlwaysEventsCount = s.Serialize<ushort>(AlwaysEventsCount, name: nameof(AlwaysEventsCount));

            if (!IsAlways)
                AlwaysEvents = s.SerializeObjectArray<Jaguar_R1_EventData>(AlwaysEvents, AlwaysEventsCount, e => e.IsAlways = true, name: nameof(AlwaysEvents));
        }
    }
}