namespace R1Engine
{
    public class GBAVV_TileSet : R1Serializable
    {
        public uint TileSetLength { get; set; }
        public byte[] TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetLength = s.Serialize<uint>(TileSetLength, name: nameof(TileSetLength));
            TileSet = s.SerializeArray<byte>(TileSet, TileSetLength, name: nameof(TileSet));
        }
    }
}