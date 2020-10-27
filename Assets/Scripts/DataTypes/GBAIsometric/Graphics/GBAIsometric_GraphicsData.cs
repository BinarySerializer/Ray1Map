namespace R1Engine
{
    public class GBAIsometric_GraphicsData : R1Serializable
    {
        public int Int_00 { get; set; }
        public int CompressionLookupBufferLength { get; set; }
        public int Int_0C { get; set; }

        public Pointer CompressionLookupPointer { get; set; }
        public Pointer CompressedDataPointer { get; set; }

        // Parsed
        public byte[] CompressionLookUpBuffer { get; set; } // Graphics data? Compressed?

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            CompressionLookupBufferLength = s.Serialize<int>(CompressionLookupBufferLength, name: nameof(CompressionLookupBufferLength));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));

            CompressionLookupPointer = s.SerializePointer(CompressionLookupPointer, name: nameof(CompressionLookupPointer));
            CompressedDataPointer = s.SerializePointer(CompressedDataPointer, name: nameof(CompressedDataPointer));

            // Serialize data from pointers
            CompressionLookUpBuffer = s.DoAt(CompressionLookupPointer, () => s.SerializeArray<byte>(CompressionLookUpBuffer, CompressionLookupBufferLength, name: nameof(CompressionLookUpBuffer)));
        }
    }
}