namespace R1Engine
{
    public class GBA_PlayField : GBA_BaseBlock
    {
        #region PlayField Data

        public Type StructType { get; set; }

        public byte TileKitOffsetIndex { get; set; }

        public byte BGTileTableOffsetIndex { get; set; }

        public byte LayerStartIndex { get; set; }

        public byte TileKitCount { get; set; } = 1;

        public byte Unk_03 { get; set; }

        // 0-4 (isn't it 0-3?)
        public byte ClusterCount { get; set; }

        // 0-5
        public byte LayerCount { get; set; }
        
        public byte[] ClusterTable { get; set; }
        public byte[] LayerTable { get; set; }

        // Size in bytes
        public ushort Mode7TilesSize { get; set; }

        public bool Mode7IsCompressed { get; set; }
        public byte Mode7Unk { get; set; }

        public MapTile[] Mode7Tiles { get; set; }

        // Prince of Persia
        public byte[] UnkBytes1 { get; set; }
        public byte[] UnkBytes2 { get; set; }
        public byte[] UnkBytes3 { get; set; }

        // Batman: Vengeance
        public byte TilePaletteIndex { get; set; }

        #endregion

        #region Parsed

        public GBA_TileKit[] TileKits { get; set; }

        public GBA_BGTileTable BGTileTable { get; set; }
        public GBA_BGTileTable FGTileTable { get; set; }

        public GBA_Cluster[] Clusters { get; set; }
        public GBA_ClusterBlock[] ClusterBlocks { get; set; }
        public GBA_TileLayer[] Layers { get; set; }

        public GBA_Batman_TileLayer[] BatmanLayers { get; set; }
        public GBA_Batman_TileLayer Shanghai_UILayer { get; set; }

        // Batman: Vengeance
        public GBA_Palette TilePalette { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia)
            {
                StructType = s.Serialize<Type>(StructType, name: nameof(StructType));
                UnkBytes1 = s.SerializeArray<byte>(UnkBytes1, 1, name: nameof(UnkBytes1));
                BGTileTableOffsetIndex = s.Serialize<byte>(BGTileTableOffsetIndex, name: nameof(BGTileTableOffsetIndex));
                TileKitOffsetIndex = s.Serialize<byte>(TileKitOffsetIndex, name: nameof(TileKitOffsetIndex));
                TileKitCount = s.Serialize<byte>(TileKitCount, name: nameof(TileKitCount));
                if (StructType == Type.PlayFieldPoP) {
                    UnkBytes2 = s.SerializeArray<byte>(UnkBytes2, 0x13, name: nameof(UnkBytes2));
                    LayerCount = 2;
                } else {
                    UnkBytes2 = s.SerializeArray<byte>(UnkBytes2, 3, name: nameof(UnkBytes2));
                }
            } 
            else if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) 
            {
                TilePaletteIndex = s.Serialize<byte>(TilePaletteIndex, name: nameof(TilePaletteIndex));
            } 
            else if (s.GameSettings.GBA_IsCommon)
            {
                StructType = s.Serialize<Type>(StructType, name: nameof(StructType));
                TileKitOffsetIndex = s.Serialize<byte>(TileKitOffsetIndex, name: nameof(TileKitOffsetIndex));
                BGTileTableOffsetIndex = s.Serialize<byte>(BGTileTableOffsetIndex, name: nameof(BGTileTableOffsetIndex));
                if (StructType == Type.PlayFieldZoom) {
                    LayerStartIndex = s.Serialize<byte>(LayerStartIndex, name: nameof(LayerStartIndex));
                    LayerCount = 4;
                    LayerTable = s.SerializeArray<byte>(LayerTable, 4, name: nameof(LayerTable));
                } else {
                    Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
                }
            }
            if (StructType != Type.PlayFieldZoom && StructType != Type.PlayFieldPoP) {

                if (StructType == Type.PlayField2D && s.GameSettings.EngineVersion > EngineVersion.GBA_TomClancysRainbowSixRogueSpear)
                    ClusterCount = s.Serialize<byte>(ClusterCount, name: nameof(ClusterCount));

                LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));

                if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
                {
                    UnkBytes1 = s.SerializeArray<byte>(UnkBytes1, 1, name: nameof(UnkBytes1));
                    Clusters = s.SerializeObjectArray<GBA_Cluster>(Clusters, 4, name: nameof(Clusters));
                    BatmanLayers = s.SerializeObjectArray<GBA_Batman_TileLayer>(BatmanLayers, LayerCount, name: nameof(BatmanLayers));
                }
                else if (s.GameSettings.GBA_IsShanghai || s.GameSettings.GBA_IsMilan)
                {
                    s.SerializeArray(new byte[3], 3, name: "Padding");
                    BatmanLayers = s.SerializeObjectArray<GBA_Batman_TileLayer>(BatmanLayers, LayerCount - (s.GameSettings.GBA_IsMilan ? 1 : 0), name: nameof(BatmanLayers));

                    // TODO: Parse the UI layer
                    Shanghai_UILayer = s.SerializeObject<GBA_Batman_TileLayer>(Shanghai_UILayer, name: nameof(Shanghai_UILayer));
                }
                else
                {
                    if (StructType == Type.PlayField2D)
                        ClusterTable = s.SerializeArray<byte>(ClusterTable, 4, name: nameof(ClusterTable));

                    LayerTable = s.SerializeArray<byte>(LayerTable, 6, name: nameof(LayerTable));

                    if (StructType == Type.PlayFieldMode7)
                    {
                        s.Serialize<byte>(default, name: "Padding");

                        Mode7TilesSize = s.Serialize<ushort>(Mode7TilesSize, name: nameof(Mode7TilesSize));
                        Mode7IsCompressed = s.Serialize<bool>(Mode7IsCompressed, name: nameof(Mode7IsCompressed));
                        Mode7Unk = s.Serialize<byte>(Mode7Unk, name: nameof(Mode7Unk));
                        if (Mode7IsCompressed)
                        {
                            s.DoEncoded(new GBA_LZSSEncoder(), () => Mode7Tiles = s.SerializeObjectArray<MapTile>(
                                Mode7Tiles, Mode7TilesSize / 2,
                                onPreSerialize: x =>
                                {
                                    x.GBATileType = MapTile.GBA_TileType.Mode7Tile;
                                    x.Is8Bpp = true;
                                }, name: nameof(Mode7Tiles)));
                        }
                        else
                        {
                            Mode7Tiles = s.SerializeObjectArray<MapTile>(Mode7Tiles, Mode7TilesSize / 2,
                                onPreSerialize: x =>
                                {
                                    x.GBATileType = MapTile.GBA_TileType.Mode7Tile;
                                    x.Is8Bpp = true;
                                }, name: nameof(Mode7Tiles));
                        }
                    }
                }
            }
        }

        public override void SerializeOffsetData(SerializerObject s) 
        {
            var layerCount = LayerCount - (s.GameSettings.GBA_IsMilan ? 1 : 0);

            if (StructType == Type.PlayField2D && Clusters == null) ClusterBlocks = new GBA_ClusterBlock[ClusterCount];
            if (Layers == null) Layers = new GBA_TileLayer[layerCount];
            if (s.GameSettings.GBA_IsShanghai ||s.GameSettings.GBA_IsMilan || s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
            {
                // Serialize tile palette
                if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
                    TilePalette = s.DoAt(OffsetTable.GetPointer(TilePaletteIndex), () => s.SerializeObject<GBA_Palette>(TilePalette, name: nameof(TilePalette)));

                // Serialize layers
                for (int i = 0; i < layerCount; i++)
                {
                    // Serialize tile layer
                    s.DoAt(OffsetTable.GetPointer(BatmanLayers[i].Index_TileLayer), () =>
                    {
                        Layers[i] = s.SerializeObject<GBA_TileLayer>(Layers[i], onPreSerialize: l =>
                        {
                            l.IsCompressed = BatmanLayers[i].IsCompressed;
                            l.StructType = BatmanLayers[i].IsCollisionBlock ? GBA_TileLayer.Type.Collision : GBA_TileLayer.Type.Layer2D;
                            l.Width = BatmanLayers[i].Width;
                            l.Height = BatmanLayers[i].Height;
                            l.LayerID = BatmanLayers[i].LayerID;

                            // Layer 2 for Milan is always the shadow layer which should be transparent
                            if (s.GameSettings.GBA_IsMilan && BatmanLayers[i].LayerID == 2)
                            {
                                l.ShouldSetBGAlphaBlending = true;
                                l.AlphaBlending_Coeff = 8; // Is this value correct?
                            }
                        }, name: $"{nameof(Layers)}[{i}]");
                    });

                    // Serialize tile kit
                    if (s.GameSettings.EngineVersion != EngineVersion.GBA_BatmanVengeance && BatmanLayers[i].Index_TileKit != 0xFF)
                        Layers[i].TileKit = s.DoAt(OffsetTable.GetPointer(BatmanLayers[i].Index_TileKit), () => s.SerializeObject<GBA_TileKit>(Layers[i].TileKit, name: $"{nameof(GBA_TileLayer.TileKit)}[{i}]"));
                }
            }
            else
            {
                if (StructType == Type.PlayField2D)
                {
                    for (int i = 0; i < ClusterCount; i++)
                    {
                        ClusterBlocks[i] = s.DoAt(OffsetTable.GetPointer(ClusterTable[i], isRelativeOffset: IsGCNBlock),
                            () =>
                            {
                                return s.SerializeObject<GBA_ClusterBlock>(ClusterBlocks[i],
                                    onPreSerialize: c => c.IsGCNBlock = IsGCNBlock, name: $"{nameof(ClusterBlocks)}[{i}]");
                            });
                    }
                }

                // Serialize layers
                if (StructType == Type.PlayFieldZoom)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Layers[i] = s.DoAt(OffsetTable.GetPointer(LayerStartIndex + i),
                            () => s.SerializeObject<GBA_TileLayer>(Layers[i], onPreSerialize: l => l.LayerID = (byte) i,
                                name: $"{nameof(Layers)}[{i}]"));
                    }
                }
                else if (StructType == Type.PlayFieldPoP)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Layers[i] = s.DoAt(OffsetTable.GetPointer(LayerStartIndex + i),
                            () => s.SerializeObject<GBA_TileLayer>(Layers[i], onPreSerialize: l => l.LayerID = (byte) i,
                                name: $"{nameof(Layers)}[{i}]"));
                    }
                }
                else
                {
                    for (int i = 0; i < LayerCount; i++)
                    {
                        Layers[i] = s.DoAt(OffsetTable.GetPointer(LayerTable[i], isRelativeOffset: IsGCNBlock),
                            () =>
                            {
                                return s.SerializeObject<GBA_TileLayer>(Layers[i],
                                    onPreSerialize: l => l.IsGCNBlock = IsGCNBlock, name: $"{nameof(Layers)}[{i}]");
                            });

                        if (StructType == Type.PlayField2D) Layers[i].ClusterBlock = ClusterBlocks[Layers[i].ClusterIndex];
                    }
                }

                // Serialize tilemap
                if (TileKits == null) TileKits = new GBA_TileKit[TileKitCount];
                for (int i = 0; i < TileKitCount; i++)
                {
                    TileKits[i] = s.DoAt(OffsetTable.GetPointer(TileKitOffsetIndex + i),
                        () => s.SerializeObject<GBA_TileKit>(TileKits[i], name: $"{nameof(TileKits)}[{i}]"));
                }

                // This game has no BGTileTable
                if (s.GameSettings.EngineVersion != EngineVersion.GBA_SplinterCell_NGage)
                {
                    // Serialize tilemap
                    BGTileTable = s.DoAt(OffsetTable.GetPointer(BGTileTableOffsetIndex, isRelativeOffset: IsGCNBlock),
                        () =>
                        {
                            return s.SerializeObject<GBA_BGTileTable>(BGTileTable, onPreSerialize: b =>
                            {
                                b.HasExtraData = StructType == Type.PlayFieldPoP;
                                b.IsGCNBlock = IsGCNBlock;
                            }, name: nameof(BGTileTable));
                        });

                    if (s.GameSettings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia &&
                        StructType != Type.PlayFieldPoP)
                    {
                        FGTileTable = s.DoAt(OffsetTable.GetPointer(BGTileTableOffsetIndex + 1),
                            () => s.SerializeObject<GBA_BGTileTable>(FGTileTable, name: nameof(FGTileTable)));
                    }
                }
            }
        }

		public override int GetOffsetTableLengthGCN(SerializerObject s) {
			return TileKitOffsetIndex + 1;
        }

		#endregion

		public class GBA_Batman_TileLayer : R1Serializable {
            public byte LayerID { get; set; }
            public bool IsCollisionBlock { get; set; }
            public bool IsCompressed { get; set; }
            public byte Byte_03 { get; set; }

            public byte Index_TileLayer { get; set; }
            public byte Index_TileKit { get; set; }

            public ushort Width { get; set; }
            public ushort Height { get; set; }

            public override void SerializeImpl(SerializerObject s) 
            {
                if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
                {
                    Index_TileLayer = s.Serialize<byte>(Index_TileLayer, name: nameof(Index_TileLayer));
                    IsCollisionBlock = s.Serialize<bool>(IsCollisionBlock, name: nameof(IsCollisionBlock));

                    IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                    Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                }
                else if (s.GameSettings.GBA_IsShanghai || s.GameSettings.GBA_IsMilan)
                {
                    Index_TileLayer = s.Serialize<byte>(Index_TileLayer, name: nameof(Index_TileLayer));
                    LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                    Index_TileKit = s.Serialize<byte>(Index_TileKit, name: nameof(Index_TileKit));

                    if (s.GameSettings.GBA_IsShanghai)
                    {
                        IsCollisionBlock = s.Serialize<bool>(IsCollisionBlock, name: nameof(IsCollisionBlock));
                    }
                    else
                    {
                        Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                        IsCollisionBlock = LayerID == 4;
                    }
                }

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
            }
        }

        public enum Type : byte {
            PlayField2D = 0,
            PlayFieldMode7 = 1,
            PlayFieldZoom = 2,
            PlayFieldPoP = 3
        }
    }
}