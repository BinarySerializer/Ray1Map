namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventData : R1Serializable
    {
        // Offsets for the position
        public ushort OffsetX { get; set; }
        public ushort OffsetY { get; set; }

        public Pointer DESPointer { get; set; }

        // Always 0x05?
        public ushort Unk_0A { get; set; }

        // Some index?
        public ushort Unk_0C { get; set; }

        // Parsed
        public Jaguar_R1_DESData DESData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            OffsetX = s.Serialize<ushort>(OffsetX, name: nameof(OffsetX));
            OffsetY = s.Serialize<ushort>(OffsetY, name: nameof(OffsetY));
            DESPointer = s.SerializePointer(DESPointer, name: nameof(DESPointer));
            Unk_0A = s.Serialize<ushort>(Unk_0A, name: nameof(Unk_0A));
            Unk_0C = s.Serialize<ushort>(Unk_0C, name: nameof(Unk_0C));

            s.DoAt(DESPointer, () => {
                DESData = s.SerializeObject<Jaguar_R1_DESData>(DESData, name: nameof(DESData));
            });
        }
    }
}