namespace R1Engine
{
    /// <summary>
    /// Editor history item for the Ray1Map editor
    /// </summary>
    public class Ray1MapEditorHistoryItem
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="modifiedTiles">The tiles which have been modified</param>
        public Ray1MapEditorHistoryItem(Common_Tile[] modifiedTiles)
        {
            ModifiedTiles = modifiedTiles;
        }

        /// <summary>
        /// The tiles which have been modified
        /// </summary>
        public Common_Tile[] ModifiedTiles { get; }
    }
}