namespace R1Engine
{
    public class WAVFormatChunk : WAVChunk
    {
        public ushort FormatType { get; set; }

        public ushort ChannelCount { get; set; }

        public uint SampleRate { get; set; }

        public uint ByteRate { get; set; }

        public ushort BlockAlign { get; set; }

        public ushort BitsPerSample { get; set; }

        protected override void SerializeChunk(SerializerObject s)
        {
            FormatType = s.Serialize<ushort>(FormatType, name: nameof(FormatType));
            ChannelCount = s.Serialize<ushort>(ChannelCount, name: nameof(ChannelCount));
            SampleRate = s.Serialize<uint>(SampleRate, name: nameof(SampleRate));
            ByteRate = s.Serialize<uint>(ByteRate, name: nameof(ByteRate));
            BlockAlign = s.Serialize<ushort>(BlockAlign, name: nameof(BlockAlign));
            BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
        }
    }
}