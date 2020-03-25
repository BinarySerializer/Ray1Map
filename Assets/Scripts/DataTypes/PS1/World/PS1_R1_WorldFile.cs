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
        /// The pointer to the palette block
        /// </summary>
        public Pointer PaletteBlockPointer => BlockPointers[6];

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
        /// The tiles palette index table
        /// </summary>
        public byte[] TilesIndexTable { get; set; }

        /// <summary>
        /// The tile color palettes
        /// </summary>
        public ARGB1555Color[][] TileColorPalettes { get; set; }

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
                DataBlock = s.SerializeArray<byte>(DataBlock, SecondBlockPointer - s.CurrentPointer, name: "DataBlock");
            });

            // BLOCK 2
            s.DoAt(SecondBlockPointer, () => {
                SecondBlock = s.SerializeArray<byte>(SecondBlock, TextureBlockPointer - s.CurrentPointer, name: "SecondBlock");
            });

            // TEXTURE BLOCK
            s.DoAt(TextureBlockPointer, () => {
                TextureBlock = s.SerializeArray<byte>(TextureBlock, EventPalette1BlockPointer - s.CurrentPointer, name: "TextureBlock");
            });

            // EVENT PALETTE 1
            s.DoAt(EventPalette1BlockPointer, () => {
                EventPalette1 = s.SerializeObjectArray<ARGB1555Color>(EventPalette1, 256, name: "EventPalette1");
            });

            // EVENT PALETTE 2
            s.DoAt(EventPalette2BlockPointer, () => {
                EventPalette2 = s.SerializeObjectArray<ARGB1555Color>(EventPalette2, 256, name: "EventPalette2");
            });

            // TILES
            s.DoAt(TilesBlockPointer, () => {
                // Read the tiles index table
                TilesIndexTable = s.SerializeArray<byte>(TilesIndexTable, PaletteBlockPointer - TilesBlockPointer, name: "TilesIndexTable");
            });

            // TILE PALETTES
            s.DoAt(PaletteBlockPointer, () => {
                // TODO: Find a better way to know the number of palettes
                uint numPalettes = (uint)(PaletteIndexBlockPointer - PaletteBlockPointer) / (256 * 2);
                if (TileColorPalettes == null)
                {
                    TileColorPalettes = new ARGB1555Color[numPalettes][];
                }
                for (int i = 0; i < TileColorPalettes.Length; i++)
                {
                    TileColorPalettes[i] = s.SerializeObjectArray<ARGB1555Color>(TileColorPalettes[i], 256, name: "TileColorPalettes[" + i + "]");
                }
            });

            // TILE PALETTE ASSIGN
            s.DoAt(PaletteIndexBlockPointer, () => {
                // Read the palette index table
                TilePaletteIndexTable = s.SerializeArray<byte>(TilePaletteIndexTable, FileSize - PaletteIndexBlockPointer.FileOffset, name: "TilePaletteIndexTable");
            });
        }

        #endregion
    }
}