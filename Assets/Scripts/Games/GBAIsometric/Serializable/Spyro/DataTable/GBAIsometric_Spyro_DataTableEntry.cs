using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_DataTableEntry : BinarySerializable
    {
        public Pointer DataPointer { get; set; }

        public uint DataLength { get; set; } // Uncompressed size

        public CompressionType Compression { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));
            Compression = s.Serialize<CompressionType>(Compression, name: nameof(Compression));
        }

        public enum CompressionType : uint
        {
            None = 0,
            Huffman = 1,
            LZSS = 2,
            RL = 3
        }
    }
}