using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Event command for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_EventCommand : ISerializableFile
    {
        public ushort CodeCount { get; set; }

        public ushort LabelOffsetCount { get; set; }

        public byte[] EventCode { get; set; }

        public ushort[] LabelOffsetTable { get; set; }

        public void Deserialize(Stream stream)
        {
            CodeCount = stream.Read<ushort>();
            LabelOffsetCount = stream.Read<ushort>();

            EventCode = stream.Read<byte>(CodeCount);

            LabelOffsetTable = stream.Read<ushort>(LabelOffsetCount);
        }

        public void Serialize(Stream stream)
        {
            stream.Write(CodeCount);
            stream.Write(LabelOffsetCount);
            stream.Write(EventCode);
            stream.Write(LabelOffsetTable);
        }
    }
}