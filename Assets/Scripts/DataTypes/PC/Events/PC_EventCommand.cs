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
            // Get the xor key to use for the command
            byte eveXor = (byte)(deserializer.GameSettings.GameMode == GameMode.RaymanPC ? 0 : 145);

            CodeCount = deserializer.Read<ushort>(eveXor);
            LabelOffsetCount = deserializer.Read<ushort>(eveXor);

            EventCode = deserializer.ReadArray<byte>(CodeCount, eveXor);

            LabelOffsetTable = deserializer.ReadArray<ushort>(LabelOffsetCount, eveXor);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            // Get the xor key to use for the command
            byte eveXor = (byte)(serializer.GameSettings.GameMode == GameMode.RaymanPC ? 0 : 145);

            serializer.Write(CodeCount, eveXor);
            serializer.Write(LabelOffsetCount, eveXor);
            serializer.Write(EventCode, eveXor);
            serializer.Write(LabelOffsetTable, eveXor);
        }
    }
}