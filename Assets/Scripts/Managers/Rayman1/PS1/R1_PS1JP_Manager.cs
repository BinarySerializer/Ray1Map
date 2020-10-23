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

        public override string ExeFilePath => "PSX.EXE";
        public override uint? ExeBaseAddress => 0x80128000 - 0x800;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_MapTileMap GetTileSet(Context context)
        {
            if (context.Settings.R1_World == R1_World.Menu)
                return new Unity_MapTileMap(Settings.CellSize);

            return new Unity_MapTileMap(GetTileSetColors(context), TileSetWidth, Settings.CellSize);
        }

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
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings), false);
            FileFactory.Read<R1_PS1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            R1_PS1_EventBlock eventBlock = null;
            MapData mapData;

            if (context.Settings.R1_World != R1_World.Menu)
            {
                Controller.DetailedState = $"Loading world file";

                await Controller.WaitIfNecessary();

                // Read the world file
                await LoadExtraFile(context, GetWorldFilePath(context.Settings), false);
                FileFactory.Read<R1_PS1_WorldFile>(GetWorldFilePath(context.Settings), context);

                Controller.DetailedState = $"Loading map data";

                // Read the level data
                await LoadExtraFile(context, GetLevelFilePath(context.Settings), true);
                var level = FileFactory.Read<R1_PS1_LevFile>(GetLevelFilePath(context.Settings), context);

                eventBlock = level.EventData;
                mapData = level.MapData;

                // Load special tile set file
                await LoadExtraFile(context, GetSpecialTileSetPath(context.Settings), true);
            }
            else
            {
                await LoadExtraFile(context, GetFontFilePath(context.Settings), false);

                mapData = MapData.GetEmptyMapData(384 / Settings.CellSize, 288 / Settings.CellSize);
            }

            // Load the level
            return await LoadAsync(context, mapData, eventBlock?.Events, eventBlock?.EventLinkingTable.Select(x => (ushort)x).ToArray(), loadTextures);
        }

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x98308;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x97308;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x96B08;
        public override uint? LevelBackgroundIndexTableOffset => ExeBaseAddress + 0x99B58;
        public override uint? WorldInfoOffset => ExeBaseAddress + 0x98BD0;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c1770,3,R1_PS1_FileType.img_file),
            new FileTableInfo(0x801c17dc,2,R1_PS1_FileType.ldr_file),
            new FileTableInfo(0x801c1824,6,R1_PS1_FileType.vdo_file),
            new FileTableInfo(0x801c18fc,0x31,R1_PS1_FileType.trk_file),
            new FileTableInfo(0x801c1fe0,5,R1_PS1_FileType.pre_file),
            new FileTableInfo(0x801c2094,6,R1_PS1_FileType.crd_file),
            new FileTableInfo(0x801c216c,6,R1_PS1_FileType.gam_file),
            new FileTableInfo(0x801c2244,6,R1_PS1_FileType.vig_wld_file),
            new FileTableInfo(0x801c231c,6,R1_PS1_FileType.wld_file),
            new FileTableInfo(0x801c23f4,0x7e,R1_PS1_FileType.map_file),
            new FileTableInfo(0x801c35ac,8,R1_PS1_FileType.blc_file),
            new FileTableInfo(0x801c36cc,0x1f,R1_PS1_FileType.fnd_file),
            new FileTableInfo(0x801c3b28,6,R1_PS1_FileType.vab_file),
            new FileTableInfo(0x801c3c00,2,R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801c3c48,1,R1_PS1_FileType.ini_file),
        };
    }
}