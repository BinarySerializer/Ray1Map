namespace R1Engine
{
    public class GBAIsometric_Spyro_TileSet : R1Serializable
    {
        public long BlockSize { get; set; }

        public int Region { get; set; } // Each region has 512 tiles
        public int RegionOffset { get; set; }
        public byte[] TileData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<uint>(bitFunc =>
            {
                RegionOffset = bitFunc(RegionOffset, 14, name: nameof(RegionOffset));
                Region = bitFunc(Region, 2, name: nameof(Region));
            });
            TileData = s.SerializeArray<byte>(TileData, BlockSize - 4, name: nameof(TileData));
        }
    }
}