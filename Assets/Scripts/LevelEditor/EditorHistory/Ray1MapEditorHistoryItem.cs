namespace Ray1Map
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
        public Ray1MapEditorHistoryItem(Ray1MapEditorHistoryTile[] modifiedTiles)
        {
            ModifiedTiles = modifiedTiles;
        }

        /// <summary>
        /// The tiles which have been modified
        /// </summary>
        public Ray1MapEditorHistoryTile[] ModifiedTiles { get; }
    }
}