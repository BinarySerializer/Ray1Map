using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Map2D_TileSetBlock : GBAVV_BaseBlock
    {
        public byte[] TileSet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            TileSet = s.SerializeArray<byte>(TileSet, BlockLength, name: nameof(TileSet));
        }
    }
}