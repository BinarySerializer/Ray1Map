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
            CodeCount = s.Serialize(CodeCount, name: "CodeCount");
            LabelOffsetCount = s.Serialize(LabelOffsetCount, name: "LabelOffsetCount");

            EventCode = s.SerializeArray(EventCode, CodeCount, name: "EventCode");

            LabelOffsetTable = s.SerializeArray(LabelOffsetTable, LabelOffsetCount, name: "LabelOffsetTable");
        }
    }
}