using System;

namespace R1Engine
{
    /// <summary>
    /// A map block for Rayman 3 (GBA)
    /// </summary>
    public class GBA_TileLayer : GBA_BaseBlock
    {
        public bool IsCollisionBlock { get; set; }

        public bool IsCompressed { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // 0-3
        public byte LayerID { get; set; }

        // 0-3
        public byte ClusterIndex { get; set; }

        public bool ShouldSetBGAlphaBlending { get; set; }
        public byte Unk_0B { get; set; }

        public byte Unk_0C { get; set; }

        // Backgrounds are 8bpp, tilemaps are 4bpp
        public bool Is8bpp { get; set; }

        // Always 0? Padding?
        public ushort Unk5 { get; set; }

        // The tile indexes
        public ushort[] MapData { get; set; }
        public GBA_TileCollisionType[] CollisionData { get; set; }

        // Batman
        public GBA_TileMap Tilemap { get; set; }


        public override void SerializeImpl(SerializerObject s) {

            if (s.GameSettings.EngineVersion == EngineVersion.BatmanVengeanceGBA) {
                if (!IsCollisionBlock) {
                    Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                    Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                    ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));
                    // TODO: figure out what this is. One of these
                    Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));

                    if (IsCompressed) {
                        s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData)));
                    } else {
                        MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData));
                    }
                    // Serialize tilemap
                    Tilemap = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_TileMap>(Tilemap, name: nameof(Tilemap)));

                } else {
                    if (IsCompressed) {
                        s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
                    } else {
                        CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData));
                    }
                }
            } else {
                IsCollisionBlock = s.Serialize<bool>(IsCollisionBlock, name: nameof(IsCollisionBlock));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));

                if (!IsCompressed)
                    throw new Exception("Non-compressed data is currently not supported");

                Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                if (!IsCollisionBlock) {
                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                    ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));
                    ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                    Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Is8bpp = s.Serialize<bool>(Is8bpp, name: nameof(Is8bpp));
                    Unk5 = s.Serialize<ushort>(Unk5, name: nameof(Unk5));

                    // TODO: It seems the compressed block contains more data than just the tile indexes for BG_2 & 3?
                    if (s.GameSettings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || s.GameSettings.EngineVersion == EngineVersion.StarWarsGBA) {
                        s.DoEncoded(new HuffmanEncoder(), () => s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData))));
                    } else {
                        s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData)));
                    }
                } else {
                    if (s.GameSettings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || s.GameSettings.EngineVersion == EngineVersion.StarWarsGBA) {
                        s.DoEncoded(new HuffmanEncoder(), () => s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData))));
                    } else {
                        s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
                    }
                }
            }
            s.Align();
        }
    }
}