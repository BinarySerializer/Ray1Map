namespace R1Engine
{
    /// <summary>
    /// Event command for PC
    /// </summary>
    public class PC_EventCommand : R1Serializable
    {
        public ushort CodeCount { get; set; }

        public ushort LabelOffsetCount { get; set; }

        public byte[] EventCode { get; set; }

        public ushort[] LabelOffsetTable { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            CodeCount = s.Serialize<ushort>(CodeCount, name: "CodeCount");
            LabelOffsetCount = s.Serialize<ushort>(LabelOffsetCount, name: "LabelOffsetCount");

            EventCode = s.SerializeArray<byte>(EventCode, CodeCount, name: "EventCode");

            LabelOffsetTable = s.SerializeArray<ushort>(LabelOffsetTable, LabelOffsetCount, name: "LabelOffsetTable");
        }
    }
}