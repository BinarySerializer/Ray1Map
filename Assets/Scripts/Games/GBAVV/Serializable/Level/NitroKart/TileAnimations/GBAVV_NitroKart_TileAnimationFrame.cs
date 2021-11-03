using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_TileAnimationFrame : BinarySerializable
    {
        public int TilesCount { get; set; } // Set before serializing

        public int Speed { get; set; }
        public byte[] TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Speed = s.Serialize<int>(Speed, name: nameof(Speed));
            TileSet = s.SerializeArray<byte>(TileSet, 0x20 * TilesCount, name: nameof(TileSet));
        }
    }
}