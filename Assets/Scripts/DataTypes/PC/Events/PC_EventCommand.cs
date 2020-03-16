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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            // Get the xor key to use for the command
            byte eveXor = (byte)(serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 145);

            serializer.Serialize(nameof(CodeCount), eveXor);
            serializer.Serialize(nameof(LabelOffsetCount), eveXor);

            serializer.SerializeArray<byte>(nameof(EventCode), CodeCount, eveXor);

            serializer.SerializeArray<ushort>(nameof(LabelOffsetTable), LabelOffsetCount, eveXor);
        }
    }
}