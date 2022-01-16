using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_TileSet : BinarySerializable
    {
        public long BlockSize { get; set; }

        public int Region { get; set; } // Each region has 512 tiles
        public int RegionOffset { get; set; }
        public byte[] TileData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<uint>(b =>
            {
                RegionOffset = b.SerializeBits<int>(RegionOffset, 14, name: nameof(RegionOffset));
                Region = b.SerializeBits<int>(Region, 2, name: nameof(Region));
            });
            TileData = s.SerializeArray<byte>(TileData, BlockSize - 4, name: nameof(TileData));
        }
    }
}