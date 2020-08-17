namespace R1Engine
{
    public class Unity_Tile : Unity_BaseEditorElement<MapTile>
    {
        public Unity_Tile(MapTile tileData) : base(tileData) { }

        /// <summary>
        /// The palette index, between 1 and 3
        /// </summary>
        public int PaletteIndex { get; set; } = 1;

        public Unity_Tile CloneObj() => new Unity_Tile(Data.CloneObj())
        {
            DebugText = DebugText,
            HasPendingEdits = HasPendingEdits,
            PaletteIndex = PaletteIndex
        };
    }
}