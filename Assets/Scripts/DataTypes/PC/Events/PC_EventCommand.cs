namespace R1Engine
{
    /// <summary>
    /// Event command for PC
    /// </summary>
    public class PC_EventCommand : IBinarySerializable
    {
        public ushort CodeCount { get; set; }

        public ushort LabelOffsetCount { get; set; }

        public byte[] EventCode { get; set; }

        public ushort[] LabelOffsetTable { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            CodeCount = deserializer.Read<ushort>();
            LabelOffsetCount = deserializer.Read<ushort>();

            EventCode = deserializer.Read<byte>(CodeCount);

            LabelOffsetTable = deserializer.Read<ushort>(LabelOffsetCount);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Write(CodeCount);
            serializer.Write(LabelOffsetCount);
            serializer.Write(EventCode);
            serializer.Write(LabelOffsetTable);
        }
    }
}