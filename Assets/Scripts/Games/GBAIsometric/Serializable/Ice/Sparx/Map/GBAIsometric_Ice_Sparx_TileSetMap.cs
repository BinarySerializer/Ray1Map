using BinarySerializer;
using BinarySerializer.Nintendo.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Sparx_TileSetMap : BinarySerializable
    {
        public long Pre_TilesCount { get; set; } = 256;

        public BinarySerializer.Nintendo.GBA.MapTile[] Tiles { get; set; } // 4 * 256

        public override void SerializeImpl(SerializerObject s)
        {
            Tiles = s.SerializeObjectArray<BinarySerializer.Nintendo.GBA.MapTile>(Tiles, 4 * Pre_TilesCount, name: nameof(Tiles));
        }
    }
}