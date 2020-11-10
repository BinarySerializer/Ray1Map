namespace R1Engine
{
    public class GBAIsometric_RHR_GraphicsData : R1Serializable
    {
        public uint UInt_00 { get; set; }
        public uint CompressionLookupBufferLength { get; set; }
        public uint TotalLength { get; set; }

        public Pointer CompressionLookupPointer { get; set; }
        public Pointer CompressedDataPointer { get; set; }

        // Parsed
        public byte[] CompressionLookupBuffer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
            CompressionLookupBufferLength = s.Serialize<uint>(CompressionLookupBufferLength, name: nameof(CompressionLookupBufferLength));
            TotalLength = s.Serialize<uint>(TotalLength, name: nameof(TotalLength));

            CompressionLookupPointer = s.SerializePointer(CompressionLookupPointer, name: nameof(CompressionLookupPointer));
            CompressedDataPointer = s.SerializePointer(CompressedDataPointer, name: nameof(CompressedDataPointer));

            // Serialize data from pointers
            CompressionLookupBuffer = s.DoAt(CompressionLookupPointer, () => s.SerializeArray<byte>(CompressionLookupBuffer, CompressionLookupBufferLength, name: nameof(CompressionLookupBuffer)));
        }
    }
}