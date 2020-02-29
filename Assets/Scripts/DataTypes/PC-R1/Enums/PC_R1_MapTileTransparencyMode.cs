namespace R1Engine
{
    /// <summary>
    /// The transparency mode for a <see cref="PC_R1_MapTile"/>
    /// </summary>
    public enum PC_R1_MapTileTransparencyMode
    {
        /// <summary>
        /// Indicates that the cell has no transparency
        /// </summary>
        NoTransparency = 0,

        /// <summary>
        /// Indicates that the cell is fully transparent
        /// </summary>
        FullyTransparent = 1,

        /// <summary>
        /// Indicates that the cell is partially transparent
        /// </summary>
        PartiallyTransparent = 2
    }
}