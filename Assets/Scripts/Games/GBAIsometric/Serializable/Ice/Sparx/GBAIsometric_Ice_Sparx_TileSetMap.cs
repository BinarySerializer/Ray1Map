using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Sparx_TileSetMap : BinarySerializable
    {
        public BinarySerializer.GBA.MapTile[] Tiles { get; set; } // 4 * 256

        public override void SerializeImpl(SerializerObject s)
        {
            Tiles = s.SerializeObjectArray<BinarySerializer.GBA.MapTile>(Tiles, 4 * 256, name: nameof(Tiles));
        }
    }
}