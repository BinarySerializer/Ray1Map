namespace R1Engine
{
    /// <summary>
    /// World data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_WorldFile : PS1_R1_BaseFile
    {
        #region File Pointers

        /// <summary>
        /// The pointer to the first block
        /// </summary>
        public Pointer DataBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the second block
        /// </summary>
        public Pointer SecondBlockPointer => BlockPointers[1];

        /// <summary>
        /// The pointer to the texture block
        /// </summary>
        public Pointer TextureBlockPointer => BlockPointers[2];

        /// <summary>
        /// The pointer to the event palette 1 block
        /// </summary>
        public Pointer EventPalette1BlockPointer => BlockPointers[3];

        /// <summary>
        /// The pointer to the event palette 2 block
        /// </summary>
        public Pointer EventPalette2BlockPointer => BlockPointers[4];

        /// <summary>
        /// The pointer to the tiles block
        /// </summary>
        public Pointer TilesBlockPointer => BlockPointers[5];

        /// <summary>
        /// The pointer to the tile palette block
        /// </summary>
        public Pointer TilePaletteBlockPointer => BlockPointers[6];

        /// <summary>
        /// The pointer to the palette index block
        /// </summary>
        public Pointer PaletteIndexBlockPointer => BlockPointers[7];

        #endregion

        #region Block Data

        /// <summary>
        /// The data block
        /// </summary>
        public byte[] DataBlock { get; set; }

        public byte[] SecondBlock { get; set; }

        /// <summary>
        /// The texture block
        /// </summary>
        public byte[] TextureBlock { get; set; }

        /// <summary>
        /// The event palette
        /// </summary>
        public ARGB1555Color[] EventPalette1 { get; set; }

        /// <summary>
        /// The event palette
        /// </summary>
        public ARGB1555Color[] EventPalette2 { get; set; }

        /// <summary>
        /// The raw tiles (JP)
        /// </summary>
        public ObjectArray<ARGB1555Color> RawTiles { get; set; }

        /// <summary>
        /// The paletted tiles (EU/US)
        /// </summary>
        public byte[] PalettedTiles { get; set; }

        /// <summary>
        /// The tile color palettes
        /// </summary>
        public ARGB1555Color[][] TilePalettes { get; set; }

        /// <summary>
        /// The tile palette index table
        /// </summary>
        public byte[] TilePaletteIndexTable { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // HEADER
            base.SerializeImpl(s);

            // DATA BLOCK
            s.DoAt(DataBlockPointer, () => {
                DataBlock = s.SerializeArray<byte>(DataBlock, SecondBlockPointer - s.CurrentPointer, name: nameof(DataBlock));
            });

            // BLOCK 2
            s.DoAt(SecondBlockPointer, () => {
                SecondBlock = s.SerializeArray<byte>(SecondBlock, TextureBlockPointer - s.CurrentPointer, name: nameof(SecondBlock));
            });

            // TEXTURE BLOCK
            s.DoAt(TextureBlockPointer, () => {
                TextureBlock = s.SerializeArray<byte>(TextureBlock, EventPalette1BlockPointer - s.CurrentPointer, name: nameof(TextureBlock));
            });

            // EVENT PALETTE 1
            s.DoAt(EventPalette1BlockPointer, () => {
                EventPalette1 = s.SerializeObjectArray<ARGB1555Color>(EventPalette1, 256, name: nameof(EventPalette1));
            });

            // EVENT PALETTE 2
            s.DoAt(EventPalette2BlockPointer, () => {
                EventPalette2 = s.SerializeObjectArray<ARGB1555Color>(EventPalette2, 256, name: nameof(EventPalette2));
            });

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1)
            {
                // TILES
                s.DoAt(TilesBlockPointer, () => {
                    // Read the tiles which use a palette
                    PalettedTiles = s.SerializeArray<byte>(PalettedTiles, TilePaletteBlockPointer - TilesBlockPointer, name: nameof(PalettedTiles));
                });

                // TILE PALETTES
                s.DoAt(TilePaletteBlockPointer, () => {
                    // TODO: Find a better way to know the number of palettes
                    uint numPalettes = (uint)(PaletteIndexBlockPointer - TilePaletteBlockPointer) / (256 * 2);
                    if (TilePalettes == null)
                    {
                        TilePalettes = new ARGB1555Color[numPalettes][];
                    }
                    for (int i = 0; i < TilePalettes.Length; i++)
                    {
                        TilePalettes[i] = s.SerializeObjectArray<ARGB1555Color>(TilePalettes[i], 256, name: nameof(TilePalettes) + "[" + i + "]");
                    }
                });

                // TILE PALETTE ASSIGN
                s.DoAt(PaletteIndexBlockPointer, () => {
                    // Read the palette index table
                    TilePaletteIndexTable = s.SerializeArray<byte>(TilePaletteIndexTable, FileSize - PaletteIndexBlockPointer.FileOffset, name: nameof(TilePaletteIndexTable));
                });
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JP)
            {
                // TILES
                s.DoAt(TilesBlockPointer, () => {
                    // Get the tile count
                    uint tileCount = RawTiles?.Length ?? ((FileSize - s.CurrentPointer.FileOffset) / 2);

                    // Serialize the tiles
                    RawTiles = s.SerializeObject<ObjectArray<ARGB1555Color>>(RawTiles, x => x.Length = tileCount);
                });
            }

        }

        #endregion
    }
}