namespace R1Engine
{
    public class GBC_PalmOS_CompressedBlock<T> : R1Serializable 
        where T : R1Serializable, new()
    {
        public LUDI_BlockHeader Header { get; set; }
        public uint BlockSize { get; set; }
        public uint DecompressedSize { get; set; }
        public uint CompressedSize { get; set; }
        public T Value { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Header = s.SerializeObject<LUDI_BlockHeader>(Header, name: nameof(Header));
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            DecompressedSize = s.Serialize<uint>(DecompressedSize, name: nameof(DecompressedSize));
            CompressedSize = s.Serialize<uint>(CompressedSize, name: nameof(CompressedSize));
            s.DoEncoded(new Lzo1xEncoder(CompressedSize, DecompressedSize), () => {
                Value = s.SerializeObject<T>(Value, name: nameof(Value));
            }, R1Engine.Serialize.BinaryFile.Endian.Little);
        }
    }
}