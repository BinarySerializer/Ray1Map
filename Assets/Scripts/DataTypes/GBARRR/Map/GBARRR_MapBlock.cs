namespace R1Engine
{
    public class GBARRR_MapBlock : R1Serializable
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

        public GBARRR_CollisionTypes[] CollisionTypes_8 { get; set; } // 8x8 collision types
        public GBARRR_TileReference[] TileIndices_8 { get; set; } // 8x8 tile indices
        public GBARRR_AlphaTileReference[] AlphaTileIndices_8 { get; set; } // 8x8 alpha tile indices

        public GBARRR_TileReference[] Indices_16 { get; set; } // 16x16 secondary tile indices

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

            if (Type == MapType.Collision)
                CollisionTypes_8 = s.DoAt(Offset + Indices_8Offset, () => s.SerializeObjectArray<GBARRR_CollisionTypes>(CollisionTypes_8, Indices_8Count, name: nameof(CollisionTypes_8)));
            else if (Type == MapType.Tiles)
                TileIndices_8 = s.DoAt(Offset + Indices_8Offset, () => s.SerializeObjectArray<GBARRR_TileReference>(TileIndices_8, Indices_8Count, name: nameof(TileIndices_8)));
            else if (Type == MapType.AlphaBlending)
                AlphaTileIndices_8 = s.DoAt(Offset + Indices_8Offset, () => s.SerializeObjectArray<GBARRR_AlphaTileReference>(AlphaTileIndices_8, Indices_8Count, name: nameof(AlphaTileIndices_8)));

            Indices_16 = s.DoAt(Offset + Indices_16Offset, () => s.SerializeObjectArray<GBARRR_TileReference>(Indices_16, Indices_16Count, name: nameof(Indices_16)));
        }

        public enum MapType
        {
            Collision,
            Tiles,
            AlphaBlending
        }

        public class GBARRR_CollisionTypes : R1Serializable
        {
            public GBARRR_TileCollisionType[] CollisionTypes { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                CollisionTypes = s.SerializeArray<GBARRR_TileCollisionType>(CollisionTypes, 4, name: nameof(CollisionTypes));
            }
        }

        public class GBARRR_TileReference : R1Serializable
        {
            public ushort[] TileIndices { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TileIndices = s.SerializeArray<ushort>(TileIndices, 4, name: nameof(TileIndices));
            }
        }

        public class GBARRR_AlphaTileReference : R1Serializable
        {
            public GBARRR_AlphaTileInfo[] TileInfos { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TileInfos = s.SerializeObjectArray<GBARRR_AlphaTileInfo>(TileInfos, 4, name: nameof(TileInfos));
            }

            public class GBARRR_AlphaTileInfo : R1Serializable
            {
                public ushort TileIndex { get; set; }
                public byte Unk { get; set; } // Either 0 or 15 - palette index? Alpha flag?

                public override void SerializeImpl(SerializerObject s)
                {
                    s.SerializeBitValues<ushort>(bitFunc =>
                    {
                        TileIndex = (ushort)bitFunc(TileIndex, 12, name: nameof(TileIndex));
                        Unk = (byte)bitFunc(Unk, 4, name: nameof(Unk));
                    });
                }
            }
        }
    }
}