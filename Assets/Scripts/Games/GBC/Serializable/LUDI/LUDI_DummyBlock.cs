using BinarySerializer;

namespace Ray1Map.GBC
{
    public class LUDI_DummyBlock : LUDI_BaseBlock
    {
        public byte[] Data { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }
}