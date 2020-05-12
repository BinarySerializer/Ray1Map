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
        /// Pointer to the compressed map data
        /// </summary>
        public Pointer MapDataPointer { get; set; }

        // Leads to compressed data
        public Pointer Pointer_08 { get; set; }

        // ?
        public Pointer Pointer_0B { get; set; }

        public Pointer TilePalettePointer { get; set; }

        public byte[] Unk_10 { get; set; }

        #endregion

        #region Parsed from Pointers

        // TODO: Parse from compressed data
        /// <summary>
        /// The map data
        /// </summary>
        public GBA_R1_Map MapData { get; set; }

        /// <summary>
        /// The 10 available tile palettes
        /// </summary>
        public ARGB1555Color[][] TilePalettes { get; set; }

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
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0B = s.SerializePointer(Pointer_0B, name: nameof(Pointer_0B));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            Unk_10 = s.SerializeArray<byte>(Unk_10, 8, name: nameof(Unk_10));

            // Parse from pointers
            s.DoAt(TilePalettePointer, () =>
            {
                if (TilePalettes == null)
                    TilePalettes = new ARGB1555Color[10][];

                for (int i = 0; i < TilePalettes.Length; i++)
                    TilePalettes[i] = s.SerializeObjectArray<ARGB1555Color>(TilePalettes[i], 16, name: $"{nameof(TilePalettes)}[{i}]");
            });
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