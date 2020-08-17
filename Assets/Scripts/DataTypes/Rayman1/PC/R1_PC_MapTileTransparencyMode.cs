namespace R1Engine
{
    // NOTE: 0 and 1 are flipped in the files. But since the value there is irrelevant we only use the memory format for this enum.

    /// <summary>
    /// The transparency mode for a map tile
    /// </summary>
    public enum R1_PC_MapTileTransparencyMode : byte 
    {
        /// <summary>
        /// Indicates that the cell is fully transparent
        /// </summary>
        FullyTransparent = 0,

        /// <summary>
        /// Indicates that the cell has no transparency
        /// </summary>
        NoTransparency = 1,

        /// <summary>
        /// Indicates that the cell is partially transparent
        /// </summary>
        PartiallyTransparent = 2
    }
}