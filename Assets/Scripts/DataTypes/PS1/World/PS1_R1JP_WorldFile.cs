namespace R1Engine
{
    /// <summary>
    /// World data for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class PS1_R1JP_WorldFile : PS1_R1_BaseFile
    {
        /// <summary>
        /// The pointer to the first block
        /// </summary>
        public Pointer FirstBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the second block
        /// </summary>
        public Pointer SecondBlockPointer => BlockPointers[1];

        /// <summary>
        /// The pointer to the event texture block
        /// </summary>
        public Pointer EventTexturesBlockPointer => BlockPointers[2];

        /// <summary>
        /// The pointer to the fourth block
        /// </summary>
        public Pointer FourthBlockPointer => BlockPointers[3];

        /// <summary>
        /// The pointer to the fifth block
        /// </summary>
        public Pointer UnknownPaletteBlockPointer => BlockPointers[4];

        /// <summary>
        /// The pointer to the tiles block
        /// </summary>
        public Pointer TilesBlockPointer => BlockPointers[5];

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FirstBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] SecondBlock { get; set; }

        public byte[] TextureBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FourthBlock { get; set; }

        public RGB555Color[] UnknownPalette { get; set; }

        /// <summary>
        /// The tile set
        /// </summary>
        public PS1_R1_RawTileSet TileSet { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            // HEADER

            base.SerializeImpl(s);

            // BLOCK 1
            s.DoAt(FirstBlockPointer, () => {
                FirstBlock = s.SerializeArray<byte>(FirstBlock, SecondBlockPointer - s.CurrentPointer, name: nameof(FirstBlock));
            });

            // BLOCK 2
            s.DoAt(SecondBlockPointer, () => {
                SecondBlock = s.SerializeArray<byte>(SecondBlock, EventTexturesBlockPointer - s.CurrentPointer, name: nameof(SecondBlock));
            });

            // EVENT TEXTURES
            s.DoAt(EventTexturesBlockPointer, () => {
                TextureBlock = s.SerializeArray<byte>(TextureBlock, FourthBlockPointer - s.CurrentPointer, name: nameof(TextureBlock));
            });

            // BLOCK 4
            s.DoAt(FourthBlockPointer, () => {
                FourthBlock = s.SerializeArray<byte>(FourthBlock, UnknownPaletteBlockPointer - s.CurrentPointer, name: nameof(FourthBlock));
            });

            // UNKNOWN PALETTE
            s.DoAt(UnknownPaletteBlockPointer, () => {
                UnknownPalette = s.SerializeObjectArray<RGB555Color>(UnknownPalette, 256, name: nameof(UnknownPalette));
            });

            // TILES
            s.DoAt(TilesBlockPointer, () => {
                // Get the tile count
                int tileCount = TileSet?.TilesArrayLength ?? (int)((FileSize - s.CurrentPointer.FileOffset) / 2);

                // Serialize the tiles
                TileSet = s.SerializeObject(TileSet, x => x.TilesArrayLength = tileCount, name: nameof(TileSet));
            });
        }
    }
}