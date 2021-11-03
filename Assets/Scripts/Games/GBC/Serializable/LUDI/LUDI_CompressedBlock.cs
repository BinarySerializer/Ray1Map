using BinarySerializer;

namespace Ray1Map.GBC
{
    public class LUDI_CompressedBlock<T> : LUDI_BaseBlock
        where T : BinarySerializable, new()
    {
        public uint TotalBlockSize { get; set; }
        public uint DecompressedSize { get; set; }
        public uint CompressedSize { get; set; }
        public T Value { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            TotalBlockSize = s.Serialize<uint>(TotalBlockSize, name: nameof(TotalBlockSize));
            DecompressedSize = s.Serialize<uint>(DecompressedSize, name: nameof(DecompressedSize));
            CompressedSize = s.Serialize<uint>(CompressedSize, name: nameof(CompressedSize));
            s.DoEncoded(new Lzo1xEncoder(CompressedSize, DecompressedSize), () => {
                Value = s.SerializeObject<T>(Value, name: nameof(Value));
            }, Endian.Little);
        }
    }
}