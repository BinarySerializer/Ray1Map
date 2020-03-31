using System;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 40;

        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoDemoVol3;

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath() => $"RAY.FXS";

        /// <summary>
        /// Gets the file path for the world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => $"RAY.WL{(settings.World == World.Jungle ? 1 : 2)}";

        /// <summary>
        /// Gets the file path for the level file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => $"RAY.LV{(settings.World == World.Jungle ? 1 : 2)}";
        
        /// <summary>
        /// Gets the file path for the level tile set file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level tile set file path</returns>
        public virtual string GetTileSetFilePath(GameSettings settings) => $"_{GetWorldName(settings.World)}_{settings.Level:00}.R16";

        /// <summary>
        /// Gets the file path for the level map file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level map file path</returns>
        public virtual string GetMapFilePath(GameSettings settings) => $"_{GetWorldName(settings.World)}_{settings.Level:00}.MAP";

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public override string GetWorldName(World world) => base.GetWorldName(world).Substring(1);

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory, $"_{GetWorldName(w)}_*.MAP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(4)))
            .ToArray())).ToArray();

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override IList<ARGBColor> GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetTileSetFilePath(context.Settings);

            // Read the file
            var tileSet = FileFactory.Read<ObjectArray<RGB555Color>>(filename, context, (s, x) => x.Length = s.CurrentLength / 2);

            // Return the tile set
            return tileSet.Value;
        }

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {

            };
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Get the file paths
            var allfixPath = GetAllfixFilePath();
            var worldPath = GetWorldFilePath(context.Settings);
            var levelPath = GetLevelFilePath(context.Settings);
            var mapPath = GetMapFilePath(context.Settings);
            var tileSetPath = GetTileSetFilePath(context.Settings);
            
            // Load the files
            await LoadExtraFile(context, allfixPath);
            await LoadExtraFile(context, worldPath);
            await LoadExtraFile(context, levelPath);
            await LoadExtraFile(context, mapPath);
            await LoadExtraFile(context, tileSetPath);

            // Read the files
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);
            var lvl = FileFactory.Read<PS1_R1JPDemoVol3_LevFile>(levelPath, context);

            // Load the level
            return await LoadAsync(context, null, null, map, null, null);
        }
    }
}