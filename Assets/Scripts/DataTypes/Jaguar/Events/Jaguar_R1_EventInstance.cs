namespace R1Engine
{
    /// <summary>
    /// Data necessary to spawn an instance of an event for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventInstance : R1Serializable
    {
        // Offsets for the position
        public ushort OffsetX { get; set; }
        public ushort OffsetY { get; set; }

        public Pointer EventDefinitionPointer { get; set; }

        // Always 0x05?
        public ushort Unk_0A { get; set; }

        // Some index?
        public ushort Unk_0C { get; set; }

        // Parsed
        public Jaguar_R1_EventDefinition EventDefinition { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            OffsetX = s.Serialize<ushort>(OffsetX, name: nameof(OffsetX));
            OffsetY = s.Serialize<ushort>(OffsetY, name: nameof(OffsetY));
            EventDefinitionPointer = s.SerializePointer(EventDefinitionPointer, name: nameof(EventDefinitionPointer));
            Unk_0A = s.Serialize<ushort>(Unk_0A, name: nameof(Unk_0A));
            Unk_0C = s.Serialize<ushort>(Unk_0C, name: nameof(Unk_0C));

            s.DoAt(EventDefinitionPointer, () => {
                EventDefinition = s.SerializeObject<Jaguar_R1_EventDefinition>(EventDefinition, name: nameof(EventDefinition));
            });
        }
    }
}