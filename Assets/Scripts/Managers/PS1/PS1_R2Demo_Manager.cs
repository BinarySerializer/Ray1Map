using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        // TODO: Fix this
        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoUS;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Enumerable.Range(1, w == World.Jungle ? 4 : 0).ToArray())).ToArray();

        /// <summary>
        /// Gets the name for the specified map
        /// </summary>
        /// <param name="map">The map</param>
        /// <returns>The map name</returns>
        public virtual string GetMapName(int map)
        {
            switch (map)
            {
                case 1:
                    return "PL1";

                case 2:
                    return "PL2";

                case 3:
                    return "FD1";

                case 4:
                    return "FD2";

                default:
                    throw new ArgumentOutOfRangeException(nameof(map));
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override IList<ARGBColor> GetTileSet(Context context)
        {
            // TODO: Move these to methods to avoid hard-coding
            var tileSetPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.RAW";
            var palettePath = $"JUNGLE/{GetMapName(context.Settings.Level)}.PAL";

            // TODO: Serialize these as actual files - for the tiles use PS1_R1_RawTileSet and for palettes we should make a generic palette class where the generic is the color type
            var tileSet = File.ReadAllBytes(context.BasePath + tileSetPath);
            var palette = File.ReadAllBytes(context.BasePath + palettePath);

            return tileSet.Select(x => new ARGB1555Color()
            {
                Color1555 = BitConverter.ToUInt16(palette, x * 2)
            }).ToArray();
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // TODO: Move this to method to avoid hard-coding
            var mapPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.MPU";

            // TODO: Use memory mapped file
            LinearSerializedFile file = new LinearSerializedFile(context)
            {
                filePath = mapPath,
            };
            context.AddFile(file);

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);

            // Load the level
            return await LoadAsync(context, null, null, map, null, null);
        }
    }
}