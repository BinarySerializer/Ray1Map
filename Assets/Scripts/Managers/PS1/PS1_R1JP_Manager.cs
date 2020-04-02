using System;
using R1Engine.Serialize;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class PS1_R1JP_Manager : PS1_BaseXXX_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 1;

        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoJP;

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override IList<ARGBColor> GetTileSet(Context context)
        {
            // TODO: Clean this up
            var levelTileSetFileName = GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}{context.Settings.Level:00}.BLC";

            if (FileSystem.FileExists(context.BasePath + levelTileSetFileName))
            {
                ObjectArray<ARGB1555Color> cols = FileFactory.Read<ObjectArray<ARGB1555Color>>(levelTileSetFileName, context, onPreSerialize: (s, x) => x.Length = s.CurrentLength / 2);
                return cols.Value;
            }

            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldJPFile = FileFactory.Read<PS1_R1_WorldFile>(filename, context);

            // Return the tile set
            return worldJPFile.RawTiles;
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            Controller.status = $"Loading allfix";

            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            var allfix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            var world = FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the level
            return await LoadAsync(context, allfix, world, level.MapData, level.EventData, level.TextureBlock);
        }
    }
}