using R1Engine.Serialize;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class PS1_R1JP_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 1;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override IList<ARGBColor> GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldJPFile = FileFactory.Read<PS1_R1JP_WorldFile>(filename, context);

            // Return the tile set
            return worldJPFile.TileSet.Tiles;
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            var allfix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            var world = FileFactory.Read<PS1_R1JP_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the level
            return await LoadAsync(context, allfix, null, level.MapData, level.EventData, level.TextureBlock);
        }
    }
}