namespace R1Engine
{
    public class Editor_MapTile : EditorWrapper<MapTile>
    {
        public Editor_MapTile(MapTile tileData) : base(tileData) { }

        /// <summary>
        /// The palette index, between 1 and 3
        /// </summary>
        public int PaletteIndex { get; set; } = 1;

        public Editor_MapTile CloneObj() => new Editor_MapTile(Data.CloneObj())
        {
            DebugText = DebugText,
            HasPendingEdits = HasPendingEdits,
            PaletteIndex = PaletteIndex
        };
    }
}