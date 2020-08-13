using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A map block for Rayman 3 (GBA)
    /// </summary>
    public class GBA_TileLayer : GBA_BaseBlock
    {
        public TileLayerStructTypes StructType { get; set; }

        public bool IsCompressed { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // 0-3 for 2D, 0-1 for Mode7
        public byte LayerID { get; set; }

        // 0-3 for 2D
        public byte ClusterIndex { get; set; }

        public bool ShouldSetBGAlphaBlending { get; set; }

        // Related to BG Alpha Blending
        public sbyte Unk_0B { get; set; }

        public byte Unk_0C { get; set; }

        // Backgrounds are 8bpp, tilemaps are 4bpp
        public bool Is8bpp { get; set; }

        // 0-3 for Mode7
        public byte Priority { get; set; }

        public byte Unk_0E { get; set; }

        // Is this same as Is8bpp? This is ColorMode for Mode7, Is8bpp is used right now for 2D
        public byte ColorMode { get; set; }

        public byte Mode7_10 { get; set; }
        public byte Mode7_11 { get; set; }
        public byte Mode7_12 { get; set; }
        public byte Mode7_13 { get; set; }
        public byte Mode7_14 { get; set; }

        public byte[] Mode7Data { get; set; }
        public MapTile[] MapData { get; set; }
        public GBA_TileCollisionType[] CollisionData { get; set; }

        // Batman
        public GBA_TileKit Tilemap { get; set; }


        public override void SerializeImpl(SerializerObject s) {

            if (s.GameSettings.EngineVersion == EngineVersion.BatmanVengeanceGBA) {
                if (StructType != TileLayerStructTypes.Collision) {
                    Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                    Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                    ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));
                    // TODO: figure out what this is. One of these
                    Unk_0B = s.Serialize<sbyte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Unk_0B = s.Serialize<sbyte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Unk_0B = s.Serialize<sbyte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                    Unk_0B = s.Serialize<sbyte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));

                    if (IsCompressed) {
                        s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, name: nameof(MapData)));
                    } else {
                        MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, name: nameof(MapData));
                    }
                    // Serialize tilemap
                    Tilemap = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_TileKit>(Tilemap, name: nameof(Tilemap)));

                } else {
                    if (IsCompressed) {
                        s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
                    } else {
                        CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData));
                    }
                }
            } else {
                StructType = s.Serialize<TileLayerStructTypes>(StructType, name: nameof(StructType));

                if ((byte)StructType > 2)
                {
                    Debug.LogWarning($"TileLayer type {StructType} is not supported");
                    return;
                }

                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));

                if (!IsCompressed)
                    throw new Exception("Non-compressed data is currently not supported");

                Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                if (StructType != TileLayerStructTypes.Collision) {
                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));

                    if (StructType != TileLayerStructTypes.Mode7)
                        ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));

                    ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                    Unk_0B = s.Serialize<sbyte>(Unk_0B, name: nameof(Unk_0B));
                    Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));

                    if (StructType == TileLayerStructTypes.Mode7)
                    {
                        Priority = s.Serialize<byte>(Priority, name: nameof(Priority));
                        Is8bpp = true;
                    }
                    else
                        Is8bpp = s.Serialize<bool>(Is8bpp, name: nameof(Is8bpp));

                    Unk_0E = s.Serialize<byte>(Unk_0E, name: nameof(Unk_0E));
                    ColorMode = s.Serialize<byte>(ColorMode, name: nameof(ColorMode));

                    if (StructType == TileLayerStructTypes.Mode7)
                    {
                        Mode7_10 = s.Serialize<byte>(Mode7_10, name: nameof(Mode7_10));
                        Mode7_11 = s.Serialize<byte>(Mode7_11, name: nameof(Mode7_11));
                        Mode7_12 = s.Serialize<byte>(Mode7_12, name: nameof(Mode7_12));
                        Mode7_13 = s.Serialize<byte>(Mode7_13, name: nameof(Mode7_13));
                        Mode7_14 = s.Serialize<byte>(Mode7_14, name: nameof(Mode7_14));
                    }

                    // TODO: It seems the compressed block contains more data than just the tile indexes for BG_2 & 3?
                    if (s.GameSettings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || s.GameSettings.EngineVersion == EngineVersion.StarWarsGBA) {
                        s.DoEncoded(new HuffmanEncoder(), () => s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, name: nameof(MapData))));
                    } else {
                        s.DoEncoded(new LZSSEncoder(), () =>
                        {
                            if (StructType == TileLayerStructTypes.Map2D)
                                MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, name: nameof(MapData));
                            else
                                Mode7Data = s.SerializeArray<byte>(Mode7Data, Width * Height, name: nameof(Mode7Data));
                        });
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

        public enum TileLayerStructTypes : byte
        {
            Map2D = 0,
            Collision = 1,
            Mode7 = 2
        }
    }
}