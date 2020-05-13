using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_Level : R1Serializable
    {
        #region Level Data

        /// <summary>
        /// Pointer to the tiles
        /// </summary>
        public Pointer TilesPointer { get; set; }

        /// <summary>
        /// Pointer to the compressed map data. Gets copied to 0x02002230 during runtime.
        /// </summary>
        public Pointer MapDataPointer { get; set; }

        /// <summary>
        /// Pointer to the compressed tile palette index table.
        /// </summary>
        public Pointer TilePaletteIndicesPointer { get; set; }

        /// <summary>
        /// Pointer to the tile header data (2 bytes per tile)
        /// </summary>
        public Pointer TileBlockIndicesPointer { get; set; }

        /// <summary>
        /// Pointer to the tile palettes
        /// </summary>
        public Pointer TilePalettePointer { get; set; }

        public byte[] Unk_10 { get; set; }

        // 1 << 0: Compress map data
        // 1 << 1: Compress tile palette indices
        public uint CompressionFlags { get; set; }

        #endregion

        #region Parsed from Pointers

        // TODO: Parse from compressed data
        /// <summary>
        /// The map data
        /// </summary>
        public GBA_R1_Map MapData { get; set; }

        /// <summary>
        /// The 10 available tile palettes (16 colors each)
        /// </summary>
        public ARGB1555Color[] TilePalettes { get; set; }

        public byte[] TilePaletteIndices { get; set; }
        public ushort[] TileBlockIndices { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize values
            TilesPointer = s.SerializePointer(TilesPointer, name: nameof(TilesPointer));
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            TilePaletteIndicesPointer = s.SerializePointer(TilePaletteIndicesPointer, name: nameof(TilePaletteIndicesPointer));
            TileBlockIndicesPointer = s.SerializePointer(TileBlockIndicesPointer, name: nameof(TileBlockIndicesPointer));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            Unk_10 = s.SerializeArray<byte>(Unk_10, 4, name: nameof(Unk_10));
            CompressionFlags = s.Serialize<uint>(CompressionFlags, name: nameof(CompressionFlags));
        }

        public void SerializeLevelData(SerializerObject s) {
            s.DoAt(MapDataPointer, () => {
                if ((CompressionFlags & 1) == 1) {
                    s.DoEncoded(new LZSSEncoder(), () => {
                        MapData = s.SerializeObject<GBA_R1_Map>(MapData, name: nameof(MapData));
                    });
                } else {
                    MapData = s.SerializeObject<GBA_R1_Map>(MapData, name: nameof(MapData));
                }
            });
            s.DoAt(TilePaletteIndicesPointer, () => {
                if ((CompressionFlags & 2) == 2) {
                    s.DoEncoded(new LZSSEncoder(), () => {
                        TilePaletteIndices = s.SerializeArray<byte>(TilePaletteIndices, s.CurrentLength, name: nameof(TilePaletteIndices));
                    });
                } else {
                    uint numTileBlocks = (TilePaletteIndicesPointer.AbsoluteOffset - TileBlockIndicesPointer.AbsoluteOffset) / 2;
                    TilePaletteIndices = s.SerializeArray<byte>(TilePaletteIndices, numTileBlocks, name: nameof(TilePaletteIndices));
                }
            });
            s.DoAt(TileBlockIndicesPointer, () => {
                TileBlockIndices = s.SerializeArray<ushort>(TileBlockIndices, TilePaletteIndices.Length, name: nameof(TileBlockIndices));
            });
            s.DoAt(TilePalettePointer, () => TilePalettes = s.SerializeObjectArray<ARGB1555Color>(TilePalettes, 10 * 16, name: nameof(TilePalettes)));

        }

        #endregion
    }

    /*
     
       v202A598 = *(_DWORD *)(28 * mapIndex + 0x85485B4);
       Pointer_04 = *(_DWORD *)(28 * mapIndex + 0x85485B8);
       if ( *(_BYTE *)(28 * mapIndex + 0x85485CC) & 1 )
       {
           sub_48428();
           Pointer_04 = 33563184;
       }
       v20305E8 = Pointer_04 + 4;
       v8 = *(_DWORD *)(28 * mapIndex + 0x85485BC);
       if ( *(_BYTE *)(28 * mapIndex + 0x85485CC) & 2 )
       {
           sub_48428();
           v8 = 33725136;
       }
       sub_38C(Pointer_04, v8, *(_DWORD *)(28 * mapIndex + 0x85485C0), v5, v17);
       v20305E0 = v202A5A0;
       v20305E2 = v202A578;
       v9 = v202A578 * v202A5A0;
       v20305E4 = v202A578 * v202A5A0;
       if ( v18 )
       {
           sub_44AA4(v9);
           v10 = sub_FF7C(v202BF10 | 1);
           v11 = sub_2E0F8(v10);
           v12 = sub_2D3C4(v11);
           v9 = sub_10334(v12);
       }
       sub_3F8(v9);
       v13 = *(_DWORD *)(4 * v17 + 0x8153980);
       v20011D8 = *(_BYTE *)(*(_BYTE *)(28 * mapIndex + 0x85485CB) + v13);
       if ( !*(_DWORD *)(36 * v20011D8 + 0x86D4D60) )
           v20011D8 = *(_BYTE *)(*(_BYTE *)(28 * mapIndex + 0x85485CA) + v13);
       if ( v20011D8 == 25 )
           v20011D8 = 26;
       sub_4841C(*(_DWORD *)(28 * mapIndex + 0x85485C4), 83886080, 80);
     
     
     */
}