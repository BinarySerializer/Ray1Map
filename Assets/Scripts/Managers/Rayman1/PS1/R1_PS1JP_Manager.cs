using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class R1_PS1JP_Manager : R1_PS1BaseXXXManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 1;

        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => PS1FileInfo.fileInfoJP;

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        public string GetSpecialTileSetPath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetWorldName(settings.R1_World)}{settings.Level:00}.BLC";

        /// <summary>
        /// Gets the tile set colors to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set colors to use</returns>
        public IList<ARGBColor> GetTileSetColors(Context context)
        {
            var levelTileSetFileName = GetSpecialTileSetPath(context.Settings);

            if (FileSystem.FileExists(context.BasePath + levelTileSetFileName))
            {
                ObjectArray<ARGB1555Color> cols = FileFactory.Read<ObjectArray<ARGB1555Color>>(levelTileSetFileName, context, onPreSerialize: (s, x) => x.Length = s.CurrentLength / 2);
                return cols.Value;
            }

            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldJPFile = FileFactory.Read<R1_PS1_WorldFile>(filename, context);

            // Return the tile set
            return worldJPFile.RawTiles;
        }

        public override string GetExeFilePath => "PSX.EXE";

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_MapTileMap GetTileSet(Context context) => new Unity_MapTileMap(GetTileSetColors(context), TileSetWidth, Settings.CellSize);

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, VRAMMode mode)
        {
            // TODO: Support BigRay + font

            // Read the files
            var allFix = mode != VRAMMode.BigRay ? FileFactory.Read<R1_PS1_AllfixFile>(GetAllfixFilePath(context.Settings), context) : null;
            var world = mode == VRAMMode.Level ? FileFactory.Read<R1_PS1_WorldFile>(GetWorldFilePath(context.Settings), context) : null;
            var levelTextureBlock = mode == VRAMMode.Level ? FileFactory.Read<R1_PS1_LevFile>(GetLevelFilePath(context.Settings), context).TextureBlock : null;
            var bigRay = mode == VRAMMode.BigRay ? FileFactory.Read<R1_PS1_BigRayFile>(GetBigRayFilePath(context.Settings), context) : null;
            var font = mode == VRAMMode.Menu ? FileFactory.Read<Array<byte>>(GetFontFilePath(context.Settings), context, (s, o) => o.Length = s.CurrentLength) : null;

            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.currentXPage = 5;

            // Reserve spot for tiles in vram
            if (mode == VRAMMode.Level)
            {
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
            }

            if (mode != VRAMMode.BigRay)
            {
                // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
                // storing the first bit of sprites we load as 1x2
                byte[] cageSprites = new byte[(128 * 3) * (256 * 2)];
                Array.Copy(allFix.TextureBlock, 0, cageSprites, 0, cageSprites.Length);
                byte[] allFixSprites = new byte[allFix.TextureBlock.Length - cageSprites.Length];
                Array.Copy(allFix.TextureBlock, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
                vram.AddData(cageSprites, (128 * 3));
                vram.AddData(allFixSprites, 256);
            }

            if (mode == VRAMMode.Level)
            {
                vram.AddData(world.TextureBlock, 256);
                vram.AddData(levelTextureBlock, 256);
            }
            else if (mode == VRAMMode.Menu)
            {
                vram.AddDataAt(10, 1, 0, 80, font.Value, 256);
            }
            else if (mode == VRAMMode.BigRay)
            {
                vram.AddDataAt(10, 0, 0, 0, bigRay.TextureBlock, 256);
            }

            int paletteY = 224; // 480 - 1 page height
            if (mode != VRAMMode.BigRay)
            {
                vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

                paletteY += 26;
                vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

                if (mode == VRAMMode.Level)
                {
                    vram.AddDataAt(1, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                    vram.AddDataAt(1, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                }
                else
                {
                    vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                    vram.AddDataAt(1, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                }
            }
            else
            {
                paletteY += 31;

                // BigRay
                vram.AddDataAt(1, 1, 0, paletteY++, bigRay.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(1, 1, 0, paletteY++, bigRay.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            }

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading allfix";

            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            FileFactory.Read<R1_PS1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.DetailedState = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            FileFactory.Read<R1_PS1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.DetailedState = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<R1_PS1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load special tile set file
            await LoadExtraFile(context, GetSpecialTileSetPath(context.Settings));

            // Load the exe
            await context.AddLinearSerializedFileAsync(GetExeFilePath);

            // Load the level
            return await LoadAsync(context, level.MapData, level.EventData.Events, level.EventData.EventLinkingTable.Select(x => (ushort)x).ToArray(), loadTextures);
        }

        public override uint? TypeZDCOffset => 0x98308;
        public override uint? ZDCDataOffset => 0x97308;
        public override uint? EventFlagsOffset => 0x96B08;
    }
}