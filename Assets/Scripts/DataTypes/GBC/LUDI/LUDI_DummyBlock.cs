using System.Linq;
using BinarySerializer;

namespace R1Engine
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