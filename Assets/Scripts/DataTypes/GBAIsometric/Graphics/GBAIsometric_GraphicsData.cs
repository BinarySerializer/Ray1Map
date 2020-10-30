namespace R1Engine
{
    public class GBAIsometric_GraphicsData : R1Serializable
    {
        public int Int_00 { get; set; }
        public int CompressionLookupBufferLength { get; set; }
        public int Int_08 { get; set; }

        public Pointer CompressionLookupPointer { get; set; }
        public Pointer CompressedDataPointer { get; set; }

        // Parsed
        public byte[] CompressionLookupBuffer { get; set; } // Graphics data? Compressed?

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            CompressionLookupBufferLength = s.Serialize<int>(CompressionLookupBufferLength, name: nameof(CompressionLookupBufferLength));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));

            CompressionLookupPointer = s.SerializePointer(CompressionLookupPointer, name: nameof(CompressionLookupPointer));
            CompressedDataPointer = s.SerializePointer(CompressedDataPointer, name: nameof(CompressedDataPointer));

            // Serialize data from pointers
            CompressionLookupBuffer = s.DoAt(CompressionLookupPointer, () => s.SerializeArray<byte>(CompressionLookupBuffer, CompressionLookupBufferLength, name: nameof(CompressionLookupBuffer)));
        }
    }
}