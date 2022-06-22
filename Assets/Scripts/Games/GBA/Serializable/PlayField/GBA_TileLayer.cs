using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using UnityEngine;

namespace Ray1Map.GBA
{
    /// <summary>
    /// A map block for GBA
    /// </summary>
    public class GBA_TileLayer : GBA_BaseBlock
    {
        public Type StructType { get; set; }

        public bool IsCompressed { get; set; }
        public bool Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // 0-3 for 2D, 0-1 for Mode7
        public byte LayerID { get; set; }

        public byte ClusterIndex { get; set; }

        public bool ShouldSetBGAlphaBlending { get; set; }
        public sbyte AlphaBlending_Coeff { get; set; }

        public bool UsesTileKitDirectly { get; set; }

        // 0-3 for Mode7
        public byte Priority { get; set; }

        public byte Unk_0C { get; set; }
        public byte Unk_0D { get; set; }
        public byte Unk_0E { get; set; }
        public byte Unk_0F { get; set; }

        public byte TileKitIndex { get; set; }

        public GBA_ColorMode ColorMode { get; set; }

        public byte Mode7_10 { get; set; }
        public byte Mode7_11 { get; set; }
        public byte Mode7_12 { get; set; }
        public byte Mode7_13 { get; set; }
        public byte Mode7_14 { get; set; }

        public byte[] Mode7Data { get; set; }
        public MapTile[] MapData { get; set; }
        public GBA_TileCollisionType[] CollisionData { get; set; }
        public byte[] UnkBytes { get; set; }

        // Batman
        public GBA_TileKit TileKit { get; set; }

        // Shanghai
        public ushort Shanghai_CollisionValue1 { get; set; }
        public ushort Shanghai_CollisionValue2 { get; set; }
        public byte[] Shanghai_MapIndices_8 { get; set; }
        public ushort[] Shanghai_MapIndices_16 { get; set; }
        public MapTile[] Shanghai_MapTiles { get; set; }

        // Milan
        public GBA_3DCollision[] CollisionData3D { get; set; }

        // Parsed
        public GBA_Cluster Cluster { get; set; }
        public GBA_ClusterBlock ClusterBlock { get; set; }

        public override void SerializeBlock(SerializerObject s) {

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                if (StructType != Type.Collision) {
                    Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                    Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                    ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));
                    // TODO: figure out what this is. One of these
                    UnkBytes = s.SerializeArray<byte>(UnkBytes, 5, name: nameof(UnkBytes));
                    ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                    Unk_0E = s.Serialize<byte>(Unk_0E, name: nameof(Unk_0E));
                    ColorMode = s.Serialize<GBA_ColorMode>(ColorMode, name: nameof(ColorMode));
                }
            }
            else if (s.GetR1Settings().GBA_IsMilan)
            {
                ColorMode = GBA_ColorMode.Color4bpp;

                // Serialize cluster
                Cluster = s.DoAt(ShanghaiOffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Cluster>(Cluster, name: nameof(Cluster)));

                IStreamEncoder encoder = null;

                switch (Cluster.Milan_MapCompressionType)
                {
                    case GBA_Cluster.Milan_CompressionType.Collision_RL:
                    case GBA_Cluster.Milan_CompressionType.Map_RL:
                        encoder = new RLEEncoder();
                        break;

                    case GBA_Cluster.Milan_CompressionType.Collsiion_LZSS:
                    case GBA_Cluster.Milan_CompressionType.Map_LZSS:
                        encoder = new BinarySerializer.Nintendo.GBA.LZSSEncoder();
                        break;
                }

                // Go to the map data
                s.Goto(ShanghaiOffsetTable.GetPointer(2));

                if (StructType == Type.Layer2D)
                {
                    s.DoEncoded(encoder, () => Shanghai_MapIndices_16 = s.SerializeArray<ushort>(Shanghai_MapIndices_16, ((Width + Width % 2) * (Height + Height % 2)) / 4, name: nameof(Shanghai_MapIndices_16)));

                    var end = s.CurrentPointer;

                    // Go to the map tiles
                    s.Goto(ShanghaiOffsetTable.GetPointer(1));

                    s.DoEncoded(encoder, () => Shanghai_MapTiles = s.SerializeObjectArray<MapTile>(Shanghai_MapTiles, (Shanghai_MapIndices_16.Max(x => BitHelpers.ExtractBits(x, 12, 0)) + 1) * 4, name: nameof(Shanghai_MapTiles)));

                    s.Goto(end);
                }
                else // Collision
                {
                    s.DoEncoded(encoder, () => SerializeTileMap(s));
                }

                s.Align();

                // Return to avoid serializing the map tile array normally
                return;
            }
            else if (s.GetR1Settings().GBA_IsShanghai)
            {
                ColorMode = GBA_ColorMode.Color4bpp;
                IsCompressed = false;

                if (StructType == Type.Layer2D)
                {
                    // Serialize cluster
                    Cluster = s.DoAt(ShanghaiOffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Cluster>(Cluster, name: nameof(Cluster)));

                    // Go to the map data
                    s.Goto(ShanghaiOffsetTable.GetPointer(2));

                    // If the map tile size is not 0 the map is split into a 32x8 index array and 8x8 map tiles
                    if (Cluster.Shanghai_MapTileSize != 0)
                    {
                        var indexArrayLength = Mathf.CeilToInt(Width / 4f) * Height;

                        // The tile size is either 1 or 2 bytes
                        if (Cluster.Shanghai_MapTileSize == 1)
                            Shanghai_MapIndices_8 = s.SerializeArray<byte>(Shanghai_MapIndices_8, indexArrayLength, name: nameof(Shanghai_MapIndices_8));
                        else
                            Shanghai_MapIndices_16 = s.SerializeArray<ushort>(Shanghai_MapIndices_16, indexArrayLength, name: nameof(Shanghai_MapIndices_16));

                        var indexArray = Cluster.Shanghai_MapTileSize == 1 ? Shanghai_MapIndices_8.Select(x => (ushort)x).ToArray() : Shanghai_MapIndices_16;

                        Shanghai_MapTiles = s.SerializeObjectArray<MapTile>(Shanghai_MapTiles, (indexArray.Max() + 1) * 4, name: nameof(Shanghai_MapTiles));

                        // Return to avoid serializing the map tile array normally
                        return;
                    }
                }
                else
                {
                    // Go to the collision data
                    s.Goto(ShanghaiOffsetTable.GetPointer(0));

                    // Serialize header properties
                    Shanghai_CollisionValue1 = s.Serialize<ushort>(Shanghai_CollisionValue1, name: nameof(Shanghai_CollisionValue1));
                    Width = s.Serialize<ushort>(Width, name: nameof(Width));
                    Height = s.Serialize<ushort>(Height, name: nameof(Height));
                    Shanghai_CollisionValue2 = s.Serialize<ushort>(Shanghai_CollisionValue2, name: nameof(Shanghai_CollisionValue2));
                }
            }
            else 
            {
                StructType = s.Serialize<Type>(StructType, name: nameof(StructType));

                if (StructType != Type.TextLayerMode7)
                    IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                else
                    Unk_01 = s.Serialize<bool>(Unk_01, name: nameof(Unk_01));

                Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                switch (StructType)
                {
                    case Type.TextLayerMode7:
                        LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                        ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                        AlphaBlending_Coeff = s.Serialize<sbyte>(AlphaBlending_Coeff, name: nameof(AlphaBlending_Coeff));
                        UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x14, name: nameof(UnkBytes));
                        ColorMode = s.Serialize<GBA_ColorMode>(ColorMode, name: nameof(ColorMode));
                        // 21 bytes
                        // Prio is 0x1D
                        // ColorMode is 0x1F
                        // Width & height seems duplicates again (is it actually width and height?)

                        break;

                    case Type.RotscaleLayerMode7:
                        LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                        ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                        AlphaBlending_Coeff = s.Serialize<sbyte>(AlphaBlending_Coeff, name: nameof(AlphaBlending_Coeff));
                        // The game hard-codes the color mode
                        ColorMode = GBA_ColorMode.Color8bpp;

                        Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                        Unk_0D = s.Serialize<byte>(Unk_0D, name: nameof(Unk_0D));
                        Unk_0E = s.Serialize<byte>(Unk_0E, name: nameof(Unk_0E));
                        Unk_0F = s.Serialize<byte>(Unk_0F, name: nameof(Unk_0F));
                        Mode7_10 = s.Serialize<byte>(Mode7_10, name: nameof(Mode7_10));
                        Mode7_11 = s.Serialize<byte>(Mode7_11, name: nameof(Mode7_11));
                        Mode7_12 = s.Serialize<byte>(Mode7_12, name: nameof(Mode7_12));
                        Mode7_13 = s.Serialize<byte>(Mode7_13, name: nameof(Mode7_13));
                        Mode7_14 = s.Serialize<byte>(Mode7_14, name: nameof(Mode7_14));
                        break;

                    case Type.SplinterCellZoom:
                        if (LayerID < 2) {
                            Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                            Unk_0D = s.Serialize<byte>(Unk_0D, name: nameof(Unk_0D));
                            Unk_0E = s.Serialize<byte>(Unk_0E, name: nameof(Unk_0E));
                            Unk_0F = s.Serialize<byte>(Unk_0F, name: nameof(Unk_0F));
                            if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                                ColorMode = GBA_ColorMode.Color8bpp;
                            } else {
                                ColorMode = GBA_ColorMode.Color4bpp;
                            }
                        } else {
                            ColorMode = GBA_ColorMode.Color8bpp;
                        }
                        UsesTileKitDirectly = true;
                        break;
                    case Type.PoP:
                        Unk_0C = s.Serialize<byte>(Unk_0C, name: nameof(Unk_0C));
                        Unk_0D = s.Serialize<byte>(Unk_0D, name: nameof(Unk_0D));
                        Unk_0E = s.Serialize<byte>(Unk_0E, name: nameof(Unk_0E));
                        Unk_0F = s.Serialize<byte>(Unk_0F, name: nameof(Unk_0F));
                        ColorMode = GBA_ColorMode.Color8bpp;
                        break;
                    case Type.Layer2D:
                        LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                        ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));
                        ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                        AlphaBlending_Coeff = s.Serialize<sbyte>(AlphaBlending_Coeff, name: nameof(AlphaBlending_Coeff));

                        UsesTileKitDirectly = s.Serialize<bool>(UsesTileKitDirectly, name: nameof(UsesTileKitDirectly));
                        ColorMode = s.Serialize<GBA_ColorMode>(ColorMode, name: nameof(ColorMode));
                        TileKitIndex = s.Serialize<byte>(TileKitIndex, name: nameof(TileKitIndex));
                        Unk_0F = s.Serialize<byte>(Unk_0F, name: nameof(Unk_0F));
                        break;
                }
            }

            if (StructType != Type.TextLayerMode7)
            {
                if (!IsCompressed)
                    SerializeTileMap(s);
                else if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_PrinceOfPersia && StructType != Type.PoP)
                    s.DoEncoded(new HuffmanEncoder(), () => s.DoEncoded(new BinarySerializer.Nintendo.GBA.LZSSEncoder(), () => SerializeTileMap(s)));
                else
                    s.DoEncoded(new BinarySerializer.Nintendo.GBA.LZSSEncoder(), () => SerializeTileMap(s));
                s.Align();
            }
        }

        protected void SerializeTileMap(SerializerObject s) {
            switch (StructType) {
                case Type.Layer2D:
                case Type.SplinterCellZoom:
                case Type.PoP:
                    MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, onPreSerialize: m => {
                        if (!UsesTileKitDirectly) {
                            if (TileKitIndex == 1) {
                                m.GBATileType = MapTile.GBA_TileType.FGTile;
                            } else {
                                m.GBATileType = MapTile.GBA_TileType.BGTile;
                            }
                        }
                        m.Is8Bpp = ColorMode == GBA_ColorMode.Color8bpp;
                    }, name: nameof(MapData));
                    break;
                case Type.RotscaleLayerMode7:
                    Mode7Data = s.SerializeArray<byte>(Mode7Data, Width * Height, name: nameof(Mode7Data));
                    break;
                case Type.Collision:
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_TombRaiderTheProphecy || 
                        s.GetR1Settings().EngineVersion == EngineVersion.GBA_TomClancysRainbowSixRogueSpear) {
                        CollisionData3D = s.SerializeObjectArray<GBA_3DCollision>(CollisionData3D, Width * Height, name: nameof(CollisionData3D));
                    } else {
                        CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData));
                    }
                    break;
            }
        }

		public override void SerializeOffsetData(SerializerObject s) {
			base.SerializeOffsetData(s);
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                if(StructType != Type.Collision)
                    // Serialize tilemap
                    TileKit = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_TileKit>(TileKit, name: nameof(TileKit)));

            }
        }

		public enum Type : byte
        {
            Layer2D = 0,
            Collision = 1,
            RotscaleLayerMode7 = 2,
            TextLayerMode7 = 3,
            SplinterCellZoom = 4,
            PoP = 5
        }

        public override long GetShanghaiOffsetTableLength => StructType == Type.Layer2D || Context.GetR1Settings().GBA_IsMilan ? 3 : 1;
    }
}