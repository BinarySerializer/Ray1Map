using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Sparx_MapLayer : BinarySerializable
    {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public short Length { get; set; }
        public string Name { get; set; }
        public ushort[] RowsOffsetTable { get; set; }
        public byte[] MapData { get; set; }

        public BinarySerializer.Nintendo.GBA_MapTile[] GetMap(GBAIsometric_Ice_Sparx_TileSetMap tileSetMap)
        {
            BinarySerializer.Nintendo.GBA_MapTile[] map = new BinarySerializer.Nintendo.GBA_MapTile[Width * 2 * Height * 2];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    byte tileIndex = MapData[y * Width + x];

                    for (int yy = 0; yy < 2; yy++)
                    {
                        for (int xx = 0; xx < 2; xx++) 
                        {
                            map[(y * 2 + yy) * Width * 2 + (x * 2 + xx)] = tileSetMap.Tiles[tileIndex * 4 + (yy * 2 + xx)];
                        }
                    }
                }
            }

            return map;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagicString("KTX", 4);
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Length = s.Serialize<short>(Length, name: nameof(Length));
            s.SerializePadding(1);
            Name = s.SerializeString(Name, 16, name: nameof(Name));
            RowsOffsetTable = s.SerializeArray<ushort>(RowsOffsetTable, Height, name: nameof(RowsOffsetTable));
            MapData = s.SerializeArray<byte>(MapData, Length, name: nameof(MapData));
        }
    }
}