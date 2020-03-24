namespace R1Engine
{
    /// <summary>
    /// Event command for PC
    /// </summary>
    public class PC_EventCommand : R1Serializable
    {
        /// <summary>
        /// The amount of bytes for the commands
        /// </summary>
        public ushort CommandLength { get; set; }

        /// <summary>
        /// The amount of label offsets
        /// </summary>
        public ushort LabelOffsetCount { get; set; }

        /// <summary>
        /// The commands
        /// </summary>
        public byte[] Commands { get; set; }

        /// <summary>
        /// The label offsets
        /// </summary>
        public ushort[] LabelOffsetTable { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer objects</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the lengths
            CommandLength = s.Serialize<ushort>(CommandLength, name: "CommandLength");
            LabelOffsetCount = s.Serialize<ushort>(LabelOffsetCount, name: "LabelOffsetCount");

            // Serialize the commands
            Commands = s.SerializeArray<byte>(Commands, CommandLength, name: "Commands");

            // Serialize the label offsets
            LabelOffsetTable = s.SerializeArray<ushort>(LabelOffsetTable, LabelOffsetCount, name: "LabelOffsetTable");
        }
    }
}