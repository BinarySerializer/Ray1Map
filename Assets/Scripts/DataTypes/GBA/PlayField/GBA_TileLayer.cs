using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A map block for GBA
    /// </summary>
    public class GBA_TileLayer : GBA_BaseBlock
    {
        public TileLayerStructTypes StructType { get; set; }

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

        // Parsed
        public GBA_Cluster Cluster { get; set; }

        public override void SerializeBlock(SerializerObject s) {

            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                if (StructType != TileLayerStructTypes.Collision) {
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
            } else {
                StructType = s.Serialize<TileLayerStructTypes>(StructType, name: nameof(StructType));

                if (StructType != TileLayerStructTypes.TextLayerMode7)
                    IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                else
                    Unk_01 = s.Serialize<bool>(Unk_01, name: nameof(Unk_01));

                Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                if (StructType != TileLayerStructTypes.Collision) {
                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));

                    if (StructType == TileLayerStructTypes.Layer2D)
                        ClusterIndex = s.Serialize<byte>(ClusterIndex, name: nameof(ClusterIndex));

                    ShouldSetBGAlphaBlending = s.Serialize<bool>(ShouldSetBGAlphaBlending, name: nameof(ShouldSetBGAlphaBlending));
                    AlphaBlending_Coeff = s.Serialize<sbyte>(AlphaBlending_Coeff, name: nameof(AlphaBlending_Coeff));

                    switch (StructType)
                    {
                        case TileLayerStructTypes.TextLayerMode7:
                            // 21 bytes
                            // Prio is 0x1D
                            // ColorMode is 0x1F
                            // Width & height seems duplicates again (is it actually width and height?)

                            break;

                        case TileLayerStructTypes.RotscaleLayerMode7:
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

                        case TileLayerStructTypes.Layer2D:
                            UsesTileKitDirectly = s.Serialize<bool>(UsesTileKitDirectly, name: nameof(UsesTileKitDirectly));
                            ColorMode = s.Serialize<GBA_ColorMode>(ColorMode, name: nameof(ColorMode));
                            Unk_0E = s.Serialize<byte>(Unk_0E, name: nameof(Unk_0E));
                            Unk_0F = s.Serialize<byte>(Unk_0F, name: nameof(Unk_0F));
                            break;
                    }
                }
            }

            if (StructType != TileLayerStructTypes.TextLayerMode7)
            {
                if (!IsCompressed)
                    SerializeTileMap(s);
                else if (s.GameSettings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia)
                    s.DoEncoded(new HuffmanEncoder(), () => s.DoEncoded(new GBA_LZSSEncoder(), () => SerializeTileMap(s)));
                else
                    s.DoEncoded(new GBA_LZSSEncoder(), () => SerializeTileMap(s));
                s.Align();
            }
        }

        protected void SerializeTileMap(SerializerObject s) {
            switch (StructType) {
                case TileLayerStructTypes.Layer2D:
                    MapData = s.SerializeObjectArray<MapTile>(MapData, Width * Height, onPreSerialize: m => {
                        if (!UsesTileKitDirectly) {
                            if (Unk_0E == 1) {
                                m.GBATileType = MapTile.GBA_TileType.FGTile;
                            } else {
                                m.GBATileType = MapTile.GBA_TileType.BGTile;
                            }
                        }
                        m.Is8Bpp = ColorMode == GBA_ColorMode.Color8bpp;
                    }, name: nameof(MapData));
                    break;
                case TileLayerStructTypes.RotscaleLayerMode7:
                    Mode7Data = s.SerializeArray<byte>(Mode7Data, Width * Height, name: nameof(Mode7Data));
                    break;
                case TileLayerStructTypes.Collision:
                    CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData));
                    break;
            }
        }

		public override void SerializeOffsetData(SerializerObject s) {
			base.SerializeOffsetData(s);
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                if(StructType != TileLayerStructTypes.Collision)
                    // Serialize tilemap
                    TileKit = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_TileKit>(TileKit, name: nameof(TileKit)));

            }
        }

		public enum TileLayerStructTypes : byte
        {
            Layer2D = 0,
            Collision = 1,
            RotscaleLayerMode7 = 2,
            TextLayerMode7 = 3,
        }
    }
}