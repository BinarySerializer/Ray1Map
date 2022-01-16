using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_SpriteMap : BinarySerializable
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public MapTile[] MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, name: nameof(MapData));
        }
    }
}