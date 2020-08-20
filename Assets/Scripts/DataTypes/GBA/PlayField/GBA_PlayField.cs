namespace R1Engine
{
    public class GBA_PlayField : GBA_BaseBlock
    {
        #region PlayField Data

        // Indicates if the PlayField is of type 2D or Mode7
        public bool IsMode7 { get; set; }

        public byte TileKitOffsetIndex { get; set; }

        public byte BGTileTableOffsetIndex { get; set; }
        
        public byte Unk_03 { get; set; }

        // For BG_0 parallax scrolling?
        // 0-4 (isn't it 0-3?)
        public byte ClusterCount { get; set; }

        // 0-5
        public byte LayerCount { get; set; }
        
        public byte[] ClusterTable { get; set; }
        public byte[] LayerTable { get; set; }

        // Prince of Persia
        public byte[] UnkBytes1 { get; set; }
        public byte[] UnkBytes2 { get; set; }

        // Batman: Vengeance
        public byte TilePaletteIndex { get; set; }

        #endregion

        #region Parsed

        public GBA_TileKit TileKit { get; set; }

        public GBA_BGTileTable BGTileTable { get; set; }

        public GBA_Cluster[] Clusters { get; set; }
        public GBA_TileLayer[] Layers { get; set; }

        public byte[] ClusterData { get; set; }
        public GBA_Batman_TileLayer[] BatmanLayers { get; set; }

        // Batman: Vengeance
        public GBA_Palette TilePalette { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia) {
                UnkBytes1 = s.SerializeArray<byte>(UnkBytes1, 2, name: nameof(UnkBytes1));
                BGTileTableOffsetIndex = s.Serialize<byte>(BGTileTableOffsetIndex, name: nameof(BGTileTableOffsetIndex));
                TileKitOffsetIndex = s.Serialize<byte>(TileKitOffsetIndex, name: nameof(TileKitOffsetIndex));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
                UnkBytes2 = s.SerializeArray<byte>(UnkBytes2, 3, name: nameof(UnkBytes2));
            } else if(s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                TilePaletteIndex = s.Serialize<byte>(TilePaletteIndex, name: nameof(TilePaletteIndex));
            } else {
                IsMode7 = s.Serialize<bool>(IsMode7, name: nameof(IsMode7));
                TileKitOffsetIndex = s.Serialize<byte>(TileKitOffsetIndex, name: nameof(TileKitOffsetIndex));
                BGTileTableOffsetIndex = s.Serialize<byte>(BGTileTableOffsetIndex, name: nameof(BGTileTableOffsetIndex));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
            }

            if (!IsMode7)
                ClusterCount = s.Serialize<byte>(ClusterCount, name: nameof(ClusterCount));
            
            LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));

            if (s.GameSettings.EngineVersion != EngineVersion.GBA_BatmanVengeance) {
                
                if (!IsMode7)
                    ClusterTable = s.SerializeArray<byte>(ClusterTable, 4, name: nameof(ClusterTable));
                
                LayerTable = s.SerializeArray<byte>(LayerTable, IsMode7 ? 8 : 6, name: nameof(LayerTable));

                // TODO: Mode7 has more data
            } else {
                UnkBytes1 = s.SerializeArray<byte>(UnkBytes1, 1, name: nameof(UnkBytes1));
                ClusterData = s.SerializeArray<byte>(ClusterData, 0x40, name: nameof(ClusterData)); // 4 of 0x10
                BatmanLayers = s.SerializeObjectArray<GBA_Batman_TileLayer>(BatmanLayers, LayerCount, name: nameof(BatmanLayers));
            }
        }

        public override void SerializeOffsetData(SerializerObject s) {
            if (!IsMode7 && Clusters == null) Clusters = new GBA_Cluster[ClusterCount];
            if (Layers == null) Layers = new GBA_TileLayer[LayerCount];
            if (s.GameSettings.EngineVersion != EngineVersion.GBA_BatmanVengeance) {

                if (!IsMode7) {
                    for (int i = 0; i < ClusterCount; i++) {
                        Clusters[i] = s.DoAt(OffsetTable.GetPointer(ClusterTable[i]), () => s.SerializeObject<GBA_Cluster>(Clusters[i], name: $"{nameof(Clusters)}[{i}]"));
                    }
                }

                // Serialize layers
                for (int i = 0; i < LayerCount; i++) {
                    Layers[i] = s.DoAt(OffsetTable.GetPointer(LayerTable[i]), () => s.SerializeObject<GBA_TileLayer>(Layers[i], name: $"{nameof(Layers)}[{i}]"));

                    if (!IsMode7) Layers[i].Cluster = Clusters[Layers[i].ClusterIndex];
                }

                // Serialize tilemap
                TileKit = s.DoAt(OffsetTable.GetPointer(TileKitOffsetIndex), () => s.SerializeObject<GBA_TileKit>(TileKit, name: nameof(TileKit)));

                // Serialize tilemap
                BGTileTable = s.DoAt(OffsetTable.GetPointer(BGTileTableOffsetIndex), () => s.SerializeObject<GBA_BGTileTable>(BGTileTable, name: nameof(BGTileTable)));
            } else {
                // Serialize tile palette
                TilePalette = s.DoAt(OffsetTable.GetPointer(TilePaletteIndex), () => s.SerializeObject<GBA_Palette>(TilePalette, name: nameof(TilePalette)));

                // Serialize layers
                for (int i = 0; i < LayerCount; i++) {
                    s.DoAt(OffsetTable.GetPointer(BatmanLayers[i].LayerID), () => {
                        Layers[i] = s.SerializeObject<GBA_TileLayer>(Layers[i], onPreSerialize: l => {
                            l.IsCompressed = BatmanLayers[i].IsCompressed;
                            l.StructType = BatmanLayers[i].IsCollisionBlock ? GBA_TileLayer.TileLayerStructTypes.Collision : GBA_TileLayer.TileLayerStructTypes.Map2D;
                            l.Width = BatmanLayers[i].Width;
                            l.Height = BatmanLayers[i].Height;
                        }, name: $"{nameof(Layers)}[{i}]");
                    });
                }
            }
        }

        #endregion

        public class GBA_Batman_TileLayer : R1Serializable {
            public byte LayerID { get; set; }
            public bool IsCollisionBlock { get; set; }
            public bool IsCompressed { get; set; }
            public byte Unk_03 { get; set; }

            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public override void SerializeImpl(SerializerObject s) {
                LayerID = s.Serialize<byte>(LayerID, name: nameof(LayerID));
                IsCollisionBlock = s.Serialize<bool>(IsCollisionBlock, name: nameof(IsCollisionBlock));

                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
            }
		}
	}
}