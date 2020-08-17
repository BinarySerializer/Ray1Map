namespace R1Engine
{
    /// <summary>
    /// WAV audio file data
    /// </summary>
    public class WAV : R1Serializable
    {
        public byte[] Magic { get; set; }

        /// <summary>
        /// The size of the file after this
        /// </summary>
        public uint FileSize { get; set; }

        public byte[] FileTypeHeader { get; set; }

        public byte[] FormatChunkMarker { get; set; }

        public uint FormatDataLength { get; set; }

        public ushort FormatType { get; set; }

        public ushort ChannelCount { get; set; }

        public uint SampleRate { get; set; }

        public uint ByteRate { get; set; }

        public ushort BlockAlign { get; set; }

        public ushort BitsPerSample { get; set; }

        public byte[] DataChunkHeader { get; set; }

        public uint DataSize { get; set; }

        public byte[] Data { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeArray<byte>(Magic, 4, name: nameof(Magic));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            FileTypeHeader = s.SerializeArray<byte>(FileTypeHeader, 4, name: nameof(FileTypeHeader));
            FormatChunkMarker = s.SerializeArray<byte>(FormatChunkMarker, 4, name: nameof(FormatChunkMarker));
            FormatDataLength = s.Serialize<uint>(FormatDataLength, name: nameof(FormatDataLength));
            FormatType = s.Serialize<ushort>(FormatType, name: nameof(FormatType));
            ChannelCount = s.Serialize<ushort>(ChannelCount, name: nameof(ChannelCount));
            SampleRate = s.Serialize<uint>(SampleRate, name: nameof(SampleRate));
            ByteRate = s.Serialize<uint>(ByteRate, name: nameof(ByteRate));
            BlockAlign = s.Serialize<ushort>(BlockAlign, name: nameof(BlockAlign));
            BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
            DataChunkHeader = s.SerializeArray<byte>(DataChunkHeader, 4, name: nameof(DataChunkHeader));
            DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
            Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
        }
    }
}