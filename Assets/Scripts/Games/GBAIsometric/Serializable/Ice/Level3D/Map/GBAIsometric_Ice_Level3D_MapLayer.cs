using BinarySerializer;
using BinarySerializer.Nintendo.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_MapLayer : BinarySerializable
    {
        public bool HasGroups => Bits_00_00 != 0;

        public byte Bits_00_00 { get; set; } // If not 0 there is no group data
        public bool Is16Bit { get; set; } // Always true
        public bool Bits_00_03 { get; set; } // Always false
        public byte Bits_00_04 { get; set; } // More flags

        public bool Bits_01_00 { get; set; }
        public byte CharacterBaseBlock { get; set; }
        public bool Bits_01_03 { get; set; }
        public byte Bits_01_04 { get; set; }

        public byte Prio { get; set; } // More flags

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public byte TileGroupWidth { get; set; } // In pixels
        public byte TileGroupHeight { get; set; } // In pixels
        public ushort TileGroupsCount { get; set; }

        public BinarySerializer.Nintendo.GBA.MapTile[][] TileGroups { get; set; }
        public ushort[] GroupMap { get; set; }

        public BinarySerializer.Nintendo.GBA.MapTile[] TileMap { get; set; }

        public BinarySerializer.Nintendo.GBA.MapTile[] GetFullMap()
        {
            if (!HasGroups)
                return TileMap;

            var map = new BinarySerializer.Nintendo.GBA.MapTile[Width * Height];

            int groupWidth = TileGroupWidth / Constants.TileSize;
            int groupHeight = TileGroupHeight / Constants.TileSize;

            int groupMapWidth = Width / groupWidth;
            int groupMapHeight = Height / groupHeight;

            for (int y = 0; y < groupMapHeight; y++)
            {
                for (int x = 0; x < groupMapWidth; x++)
                {
                    BinarySerializer.Nintendo.GBA.MapTile[] group = TileGroups[GroupMap[y * groupMapWidth + x]];

                    for (int gy = 0; gy < groupHeight; gy++)
                    {
                        for (int gx = 0; gx < groupWidth; gx++)
                        {
                            map[(y * groupHeight + gy) * Width + (x * groupWidth + gx)] = group[gy * groupWidth + gx];
                        }
                    }
                }
            }

            return map;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoEncoded(new BinarySerializer.Nintendo.GBA.LZSSEncoder(), () =>
            {
                s.DoBits<byte>(b =>
                {
                    Bits_00_00 = b.SerializeBits<byte>(Bits_00_00, 2, name: nameof(Bits_00_00));
                    Is16Bit = b.SerializeBits<bool>(Is16Bit, 1, name: nameof(Is16Bit));
                    Bits_00_03 = b.SerializeBits<bool>(Bits_00_03, 1, name: nameof(Bits_00_03));
                    Bits_00_04 = b.SerializeBits<byte>(Bits_00_04, 4, name: nameof(Bits_00_04));
                });

                s.DoBits<byte>(b =>
                {
                    Bits_01_00 = b.SerializeBits<bool>(Bits_01_00, 1, name: nameof(Bits_01_00));
                    CharacterBaseBlock = b.SerializeBits<byte>(CharacterBaseBlock, 2, name: nameof(CharacterBaseBlock));
                    Bits_01_03 = b.SerializeBits<bool>(Bits_01_03, 1, name: nameof(Bits_01_03));
                    Bits_01_04 = b.SerializeBits<byte>(Bits_01_04, 2, name: nameof(Bits_01_04));
                    b.SerializePadding(2, logIfNotNull: true);
                });

                Prio = s.Serialize<byte>(Prio, name: nameof(Prio));
                s.SerializePadding(1, logIfNotNull: true);

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                TileGroupWidth = s.Serialize<byte>(TileGroupWidth, name: nameof(TileGroupWidth));
                TileGroupHeight = s.Serialize<byte>(TileGroupHeight, name: nameof(TileGroupHeight));
                TileGroupsCount = s.Serialize<ushort>(TileGroupsCount, name: nameof(TileGroupsCount));

                if (HasGroups)
                {
                    TileGroups ??= new BinarySerializer.Nintendo.GBA.MapTile[TileGroupsCount][];
                    int groupLength = (TileGroupWidth / Constants.TileSize) * (TileGroupHeight / Constants.TileSize);

                    for (int i = 0; i < TileGroups.Length; i++)
                        TileGroups[i] = s.SerializeIntoArray<BinarySerializer.Nintendo.GBA.MapTile>(TileGroups[i], groupLength, Is16Bit ? BinarySerializer.Nintendo.GBA.MapTile.SerializeInto_Regular : BinarySerializer.Nintendo.GBA.MapTile.SerializeInto_Affine, name: $"{nameof(TileGroups)}[{i}]");

                    GroupMap = s.SerializeArray<ushort>(GroupMap, (Width * Height) / groupLength, name: nameof(GroupMap));
                }
                else
                {
                    TileMap = s.SerializeIntoArray<BinarySerializer.Nintendo.GBA.MapTile>(TileMap, Width * Height,
                        Is16Bit || Bits_00_00 != 0 ? BinarySerializer.Nintendo.GBA.MapTile.SerializeInto_Regular : BinarySerializer.Nintendo.GBA.MapTile.SerializeInto_Affine, name: nameof(TileMap));
                }
            });
        }
    }
}