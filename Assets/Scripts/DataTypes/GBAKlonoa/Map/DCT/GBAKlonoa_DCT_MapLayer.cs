using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_DCT_MapLayer : BinarySerializable
    {
        public byte Byte_00 { get; set; }
        public byte TileLength { get; set; } // 0x20 for 4-bit and 0x40 for 8-bit
        public bool Is8Bit => TileLength == 0x40;
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort TileSetLength { get; set; }
        public Pointer TileSetPointer { get; set; }
        public Pointer MapPointer { get; set; }
        public byte[] Bytes_10 { get; set; }

        // Serialized from pointers
        public MapTile[] Map { get; set; }
        public byte[] TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            TileLength = s.Serialize<byte>(TileLength, name: nameof(TileLength));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            MapPointer = s.SerializePointer(MapPointer, name: nameof(MapPointer));
            Bytes_10 = s.SerializeArray<byte>(Bytes_10, 4, name: nameof(Bytes_10));

            s.DoAt(TileSetPointer, () => s.DoEncoded(new GBAKlonoa_DCT_Encoder(), () => TileSet = s.SerializeArray<byte>(TileSet, TileSetLength * TileLength, name: nameof(TileSet))));
            s.DoAt(MapPointer, () => s.DoEncoded(new GBAKlonoa_DCT_Encoder(), () => Map = s.SerializeObjectArray<MapTile>(Map, Width * Height, x => x.Pre_GBAKlonoa_Is8Bit = Is8Bit, name: nameof(Map))));
        }
    }
}