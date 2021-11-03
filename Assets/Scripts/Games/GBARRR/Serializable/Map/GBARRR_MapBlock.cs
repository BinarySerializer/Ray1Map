using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_MapBlock : BinarySerializable
    {
        public MapType Type { get; set; }

        public uint Indices_8Offset { get; set; }
        public uint Indices_8Count { get; set; }
        public uint Indices_16Offset { get; set; }
        public uint Indices_16Count { get; set; }
        public uint MapWidth { get; set; }
        public uint MapHeight { get; set; }
        public uint MapDataLength { get; set; }

        public ushort[] Indices_32 { get; set; } // 32x32 tiles

        public GBARRR_MapTiles[] Tiles_8 { get; set; } // 8x8 tile indices

        public GBARRR_TileReferences[] Indices_16 { get; set; } // 16x16 secondary tile indices

        public override void SerializeImpl(SerializerObject s)
        {
            Indices_8Offset = s.Serialize<uint>(Indices_8Offset, name: nameof(Indices_8Offset));
            Indices_8Count = s.Serialize<uint>(Indices_8Count, name: nameof(Indices_8Count));

            Indices_16Offset = s.Serialize<uint>(Indices_16Offset, name: nameof(Indices_16Offset));
            Indices_16Count = s.Serialize<uint>(Indices_16Count, name: nameof(Indices_16Count));

            MapWidth = s.Serialize<uint>(MapWidth, name: nameof(MapWidth));
            MapHeight = s.Serialize<uint>(MapHeight, name: nameof(MapHeight));
            MapDataLength = s.Serialize<uint>(MapDataLength, name: nameof(MapDataLength));

            Indices_32 = s.SerializeArray<ushort>(Indices_32, MapWidth * MapHeight, name: nameof(Indices_32));

            Tiles_8 = s.DoAt(Offset + Indices_8Offset, () => s.SerializeObjectArray<GBARRR_MapTiles>(Tiles_8, Indices_8Count, name: nameof(Tiles_8), onPreSerialize: x => x.Type = Type));

            Indices_16 = s.DoAt(Offset + Indices_16Offset, () => s.SerializeObjectArray<GBARRR_TileReferences>(Indices_16, Indices_16Count, name: nameof(Indices_16)));

            // Move to end of block so block size can be checked correctly
            s.Goto(Offset + Indices_16Offset + Indices_16Count * 4 * 2);
        }

        public enum MapType
        {
            Collision,
            Tiles,
            Foreground,
            Mode7Tiles,
            Menu
        }

        public class GBARRR_TileReferences : BinarySerializable
        {
            public ushort[] TileIndices { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TileIndices = s.SerializeArray<ushort>(TileIndices, 4, name: nameof(TileIndices));
            }
        }

        public class GBARRR_MapTiles : BinarySerializable
        {
            public MapType Type { get; set; }

            public MapTile[] Tiles { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Tiles = s.SerializeObjectArray<MapTile>(Tiles, 4, name: nameof(Tiles), onPreSerialize: x => x.GBARRRType = Type);
            }
        }
    }
}