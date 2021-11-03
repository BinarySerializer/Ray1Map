using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_BGMapBlock : BinarySerializable
    {
        public byte[] UnkData { get; set; }

        public MapTile[] MapTiles { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UnkData = s.SerializeArray<byte>(UnkData, 180, name: nameof(UnkData));
            MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, 32 * 32, name: nameof(MapTiles), onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Foreground);
        }
    }
}