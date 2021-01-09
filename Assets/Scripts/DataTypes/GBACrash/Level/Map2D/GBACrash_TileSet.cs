namespace R1Engine
{
    public class GBACrash_TileSet : GBACrash_BaseBlock
    {
        public byte[] TileSet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            TileSet = s.SerializeArray<byte>(TileSet, BlockLength, name: nameof(TileSet));
        }
    }
}