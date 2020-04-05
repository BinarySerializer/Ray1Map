using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (Saturn)
    /// </summary>
    public class Satun_R1_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        // TODO: Fix
        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoR2PS1;

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(World world) => GetWorldName(world) + "/";

        /// <summary>
        /// Gets the map file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The map file path</returns>
        public string GetMapFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}00{context.Settings.Level}.XMP";

        /// <summary>
        /// Gets the tile-set palette file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set palette file path</returns>
        public string GetTileSetPaletteFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}.PAL";

        /// <summary>
        /// Gets the tile-set palette index table file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set palette index table file path</returns>
        public string GetTileSetPaletteIndexTableFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}_01.PLT";

        /// <summary>
        /// Gets the tile-set file path
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile-set file path</returns>
        public string GetTileSetFilePath(Context context) => GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}_01.BIT";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"*.XMP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(5)))
            .ToArray())).ToArray();

        public override async Task LoadExtraFile(Context context, string path)
        {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = FileInfo;

            // TODO: This should be memory mapped!
            var file = new LinearSerializedFile(context)
            {
                filePath = path,
                Endianness = BinaryFile.Endian.Big
            };

            context.AddFile(file);
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Common_Tileset GetTileSet(Context context) 
        {
            // Read the files
            var tileSetPalette = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetTileSetPaletteFilePath(context), context, onPreSerialize: (s, x) => x.Length = s.CurrentLength / 2);
            var tileSetPaletteIndexTable = FileFactory.Read<Array<byte>>(GetTileSetPaletteIndexTableFilePath(context), context, onPreSerialize: (s, x) => x.Length = s.CurrentLength);
            var tileSet = FileFactory.Read<BIT>(GetTileSetFilePath(context), context, onPreSerialize: (s, b) =>
            {
                b.PAL = tileSetPalette;
                b.PLT = tileSetPaletteIndexTable;
            });

            // Get the tile-set texture
            var tex = tileSet.ToTexture(CellSize * TileSetWidth);

            // TODO: Is this needed?
            // Add transparency
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    var color = tex.GetPixel(x, y);

                    if (color.r == 0 && color.g == 0 && color.b == 0)
                        tex.SetPixel(x, y, new Color(color.r, color.g, color.b, 0f));
                }
            }
            tex.Apply();

            return new Common_Tileset(tex, CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override PS1_VRAM FillVRAM(Context context) => throw new NotImplementedException();

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Get the paths
            var tileSetPaletteFilePath = GetTileSetPaletteFilePath(context);
            var tileSetPaletteIndexTableFilePath = GetTileSetPaletteIndexTableFilePath(context);
            var tileSetFilePath = GetTileSetFilePath(context);
            var mapFilePath = GetMapFilePath(context);

            // Load the files
            await LoadExtraFile(context, tileSetPaletteFilePath);
            await LoadExtraFile(context, tileSetPaletteIndexTableFilePath);
            await LoadExtraFile(context, tileSetFilePath);
            await LoadExtraFile(context, mapFilePath);

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapFilePath, context);

            // Load the level
            return await LoadAsync(context, map, null, null);
        }
    }
}