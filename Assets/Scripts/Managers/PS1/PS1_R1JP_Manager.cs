using R1Engine.Serialize;
using System;
using System.Collections.Generic;
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
        /// Gets the path for the special tile set file to use if one is available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetSpecialTileSetPathIfExists(Context context)
        {
            var path = GetWorldFolderPath(context.Settings.World) + $"{GetWorldName(context.Settings.World)}{context.Settings.Level:00}.BLC";

            return FileSystem.FileExists(context.BasePath + path) ? path : null;
        }

        /// <summary>
        /// Gets the tile set colors to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set colors to use</returns>
        public IList<ARGBColor> GetTileSetColors(Context context)
        {
            var levelTileSetFileName = GetSpecialTileSetPathIfExists(context);

            if (levelTileSetFileName != null)
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
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Common_Tileset GetTileSet(Context context) => new Common_Tileset(GetTileSetColors(context), TileSetWidth, Settings.CellSize);

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override void FillVRAM(Context context)
        {
            // Read the files
            var allFix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);
            var world = FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);
            var levelTextureBlock = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context).TextureBlock;

            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.currentXPage = 5;

            // Reserve spot for tiles in vram
            IList<ARGBColor> tiles = GetTileSetColors(context);
            int tilesetHeight = tiles.Count / 256;
            const int tilesetWidth = 4 * 128;
            int tilesetPage = (16 - 4); // Max pages - tileset width
            while (tilesetHeight > 0)
            {
                int thisPageHeight = Math.Min(tilesetHeight, 2 * 256);
                vram.ReserveBlock(tilesetPage * 128, (2 * 256) - thisPageHeight, tilesetWidth, thisPageHeight);
                tilesetHeight -= thisPageHeight;
                tilesetPage -= 4;
            }

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[(128 * 3) * (256 * 2)];
            Array.Copy(allFix.TextureBlock, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[allFix.TextureBlock.Length - cageSprites.Length];
            Array.Copy(allFix.TextureBlock, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            vram.AddData(cageSprites, (128 * 3));
            vram.AddData(allFixSprites, 256);

            vram.AddData(world.TextureBlock, 256);
            vram.AddData(levelTextureBlock, 256);

            int paletteY = 224; // 480 - 1 page height
            vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            paletteY += 26;
            vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(1, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(1, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading allfix";

            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load special tile set file if available
            var levelTileSetFileName = GetSpecialTileSetPathIfExists(context);

            if (levelTileSetFileName != null)
                await LoadExtraFile(context, levelTileSetFileName);

            // Load the level
            return await LoadAsync(context, level.MapData, level.EventData.Events, level.EventData.EventLinkingTable.Select(x => (ushort)x).ToArray(), loadTextures);
        }
    }
}