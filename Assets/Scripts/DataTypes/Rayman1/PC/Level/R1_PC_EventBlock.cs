namespace R1Engine
{
    /// <summary>
    /// Event data for PC
    /// </summary>
    public class R1_PC_EventBlock : R1Serializable
    {
        /// <summary>
        /// The checksum for the decrypted event block
        /// </summary>
        public byte EventBlockChecksum { get; set; }

        /// <summary>
        /// The number of available events in the map
        /// </summary>
        public ushort EventCount { get; set; }

        /// <summary>
        /// Data table for event linking
        /// </summary>
        public ushort[] EventLinkingTable { get; set; }

        /// <summary>
        /// The events in the map
        /// </summary>
        public R1_EventData[] Events { get; set; }

        /// <summary>
        /// The event commands in the map
        /// </summary>
        public R1_PC_EventCommand[] EventCommands { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            EventBlockChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                // Set the xor key to use for the event block
                s.DoXOR((byte)(s.GameSettings.EngineVersion == EngineVersion.R1_PC || s.GameSettings.EngineVersion == EngineVersion.R1_PocketPC ? 0 : 0x91), () =>
                {
                    // Serialize the event count
                    EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));

                    // Serialize the event linking table
                    EventLinkingTable = s.SerializeArray<ushort>(EventLinkingTable, EventCount, name: nameof(EventLinkingTable));

                    // Serialize the events
                    Events = s.SerializeObjectArray<R1_EventData>(Events, EventCount, name: nameof(Events));

                    // Serialize the event commands
                    EventCommands = s.SerializeObjectArray<R1_PC_EventCommand>(EventCommands, EventCount, name: nameof(EventCommands));
                });
            }, ChecksumPlacement.Before, calculateChecksum: s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit || s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu, name: nameof(EventBlockChecksum));
        }
    }
}