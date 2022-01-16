using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_ResourceEntry : BinarySerializable
    {
        public Pointer DataPointer { get; set; }

        public uint DataLength { get; set; } // Uncompressed size

        public GBAIsometric_IceDragon_CompressionType Compression { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));
            Compression = s.Serialize<GBAIsometric_IceDragon_CompressionType>(Compression, name: nameof(Compression));
        }
    }
}