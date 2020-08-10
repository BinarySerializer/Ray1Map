namespace R1Engine
{
    // First  level is at 0x082E7288
    // Second level is at 0x08362544
    // 0x03000e20 has pointer to this struct for the current level during runtime

    public class GBA_R3_PlayField2D : GBA_R3_BaseBlock
    {
        #region PlayField Data

        public bool IsMode7 { get; set; }

        public byte TileMap { get; set; }

        // Seems to determine the tilemap for BG_0
        public byte Unk_02 { get; set; }
        
        public byte Unk_03 { get; set; }

        // For BG_0 parallax scrolling?
        // 0-4 (isn't it 0-3?)
        public byte ClusterCount { get; set; }

        // 0-5
        public byte LayerCount { get; set; }
        
        public byte[] ClusterTable { get; set; }
        public byte[] LayerTable { get; set; }

        #endregion

        #region Parsed

        public GBA_R3_OffsetTable OffsetTable { get; set; }

        public GBA_R3_Cluster[] Clusters { get; set; }
        public GBA_R3_TileLayer[] Layers { get; set; }
        public GBA_R3_TileMap Tilemap { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            IsMode7 = s.Serialize<bool>(IsMode7, name: nameof(IsMode7));

            // Mode7 maps have a different structure
            if (IsMode7)
                return;

            TileMap = s.Serialize<byte>(TileMap, name: nameof(TileMap));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
            ClusterCount = s.Serialize<byte>(ClusterCount, name: nameof(ClusterCount));
            LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));
            ClusterTable = s.SerializeArray<byte>(ClusterTable, 4, name: nameof(ClusterTable));
            LayerTable = s.SerializeArray<byte>(LayerTable, 6, name: nameof(LayerTable));

            s.Align();

            // Serialize offset table
            OffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(OffsetTable, name: nameof(OffsetTable));

            if (Clusters == null)
                Clusters = new GBA_R3_Cluster[ClusterCount];

            // Serialize layers
            for (int i = 0; i < ClusterCount; i++)
                Clusters[i] = s.DoAt(OffsetTable.GetPointer(ClusterTable[i], true), () => s.SerializeObject<GBA_R3_Cluster>(Clusters[i], name: $"{nameof(Clusters)}[{i}]"));

            if (Layers == null)
                Layers = new GBA_R3_TileLayer[LayerCount];

            // Serialize layers
            for (int i = 0; i < LayerCount; i++)
                Layers[i] = s.DoAt(OffsetTable.GetPointer(LayerTable[i], true), () => s.SerializeObject<GBA_R3_TileLayer>(Layers[i], name: $"{nameof(Layers)}[{i}]"));

            // Serialize tilemap
            Tilemap = s.DoAt(OffsetTable.GetPointer(TileMap, true), () => s.SerializeObject<GBA_R3_TileMap>(Tilemap, name: nameof(Tilemap)));
        }

        #endregion
    }
}