using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan Demo Vol6)
    /// </summary>
    public class R1_PS1JPDemoVol6_Manager : R1_PS1BaseManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 40;

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath() => $"RAY.DTA";
        public string GetAllfixSpritePath() => $"RAY.IMG";

        /// <summary>
        /// Gets the file path for the world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => $"{GetWorldName(settings.R1_World)}.DTA";
        public string GetWorldSpritePath(GameSettings settings) => $"{GetWorldName(settings.R1_World)}.IMG";

        /// <summary>
        /// Gets the file path for the level tile set file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level tile set file path</returns>
        public string GetTileSetFilePath(GameSettings settings) => $"{GetWorldName(settings.R1_World)}_OPT.R16";

        /// <summary>
        /// Gets the file path for the level file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public string GetLevelFilePath(GameSettings settings) => $"{GetWorldName(settings.R1_World)}{settings.Level:00}.DTA";
        public string GetLevelSpritePath(GameSettings settings) => $"{GetWorldName(settings.R1_World)}{settings.Level:00}.IMG";
        public string GetPalettePath(GameSettings settings, int i) => $"{GetWorldName(settings.R1_World)}{i}.PAL";
        public string GetFontPalettePath() => $"LETTRE.PAL";

        /// <summary>
        /// Gets the file path for the level map file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level map file path</returns>
        public string GetMapFilePath(GameSettings settings) => $"{GetWorldName(settings.R1_World)}{settings.Level:00}.MAP";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory, $"{GetWorldName(w)}*.MAP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray());

        public override string ExeFilePath => "RAY_GAME.EXE";
        public override uint? ExeBaseAddress => 0x80150000 - 0x800;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801b791c, 3, R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801b79d0, 1, R1_PS1_FileType.demo_vig),
            new FileTableInfo(0x801b7a0c, 12, R1_PS1_FileType.vab_file),

            new FileTableInfo(0x801b7cdc, 5, R1_PS1_FileType.wld_file), // Jungle

            // Unused
            //new FileTableInfo(0x801B7E08, 5, R1_PS1_FileType.wld_file), // Music
            //new FileTableInfo(0x801B7F34, 5, R1_PS1_FileType.wld_file), // Mountain
            //new FileTableInfo(0x801B8060, 5, R1_PS1_FileType.wld_file), // Image
            //new FileTableInfo(0x801B818C, 5, R1_PS1_FileType.wld_file), // Cave
            //new FileTableInfo(0x801B82B8, 5, R1_PS1_FileType.wld_file), // Cake

            new FileTableInfo(0x801b83e4, 64, R1_PS1_FileType.map_file), // Jungle

            // Unused
            //new FileTableInfo(0x801B92E4, 64, R1_PS1_FileType.map_file), // Music
            //new FileTableInfo(0x801BA1E4, 64, R1_PS1_FileType.map_file), // Mountain
            //new FileTableInfo(0x801BB0E4, 64, R1_PS1_FileType.map_file), // Image
            //new FileTableInfo(0x801BBFE4, 64, R1_PS1_FileType.map_file), // Cave
            //new FileTableInfo(0x801BCEE4, 64, R1_PS1_FileType.map_file), // Cake

            new FileTableInfo(0x801BDDE4, 11, R1_PS1_FileType.fnd_file),
            new FileTableInfo(0x801be078, 11, R1_PS1_FileType.demo_file), // BGI (background data?)

            new FileTableInfo(0x801BE30C, 4, R1_PS1_FileType.trk_file)
        };

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_MapTileMap GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetTileSetFilePath(context.Settings);

            // Read the file
            var tileSet = FileFactory.Read<ObjectArray<RGBA5551Color>>(filename, context, (s, x) => x.Length = s.CurrentLength / 2);

            // Return the tile set
            return new Unity_MapTileMap(tileSet.Value, TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, VRAMMode mode)
        {
            // Read palettes
            var pal4 = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetPalettePath(context.Settings, 4), context, (y, x) => x.Length = y.CurrentLength / 2);
            var pal8 = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetPalettePath(context.Settings, 8), context, (y, x) => x.Length = y.CurrentLength / 2);
            var palLettre = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetFontPalettePath(), context, (y, x) => x.Length = y.CurrentLength / 2);

            // Read the files
            var fixGraphics = FileFactory.Read<Array<byte>>(GetAllfixSpritePath(), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);
            var wldGraphics = FileFactory.Read<Array<byte>>(GetWorldSpritePath(context.Settings), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);
            var lvlGraphics = FileFactory.Read<Array<byte>>(GetLevelSpritePath(context.Settings), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);
            
            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.currentXPage = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * (256 * 2)];
            Array.Copy(fixGraphics.Value, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[fixGraphics.Value.Length - cageSprites.Length];
            Array.Copy(fixGraphics.Value, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            /*byte[] unknown = new byte[128 * 8];
            vram.AddData(unknown, 128);*/
            vram.AddData(cageSprites, 128);
            vram.AddData(allFixSprites, 256);

            vram.AddData(wldGraphics.Value, 256);
            vram.AddData(lvlGraphics.Value, 256);

            int paletteY = 256 - 3; // 480 - 1 page height
            vram.AddDataAt(1, 1, 0, paletteY++, palLettre.Value.SelectMany(c => BitConverter.GetBytes(c.Color5551)).ToArray(), 512);
            vram.AddDataAt(1, 1, 0, paletteY++, pal4.Value.SelectMany(c => BitConverter.GetBytes(c.Color5551)).ToArray(), 512);
            vram.AddDataAt(1, 1, 0, paletteY++, pal8.Value.SelectMany(c => BitConverter.GetBytes(c.Color5551)).ToArray(), 512);
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
            // Get the file paths
            var mapPath = GetMapFilePath(context.Settings);
            var levelPath = GetLevelFilePath(context.Settings);

            // Read the files
            var map = FileFactory.Read<MapData>(mapPath, context);
            var level = FileFactory.Read<R1_PS1JPDemo_LevFile>(levelPath, context);

            // Load the level
            return await LoadAsync(context, map, level.Events, level.EventLinkTable.Select(x => (ushort)x).ToArray(), loadTextures);
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            await base.LoadFilesAsync(context);

            // Get the file paths
            var allfixPath = GetAllfixFilePath();
            var worldPath = GetWorldFilePath(context.Settings);
            var levelPath = GetLevelFilePath(context.Settings);
            var mapPath = GetMapFilePath(context.Settings);
            var tileSetPath = GetTileSetFilePath(context.Settings);

            // sprites
            await LoadExtraFile(context, GetAllfixSpritePath(), true);
            await LoadExtraFile(context, GetWorldSpritePath(context.Settings), true);
            await LoadExtraFile(context, GetLevelSpritePath(context.Settings), true);
            await LoadExtraFile(context, GetPalettePath(context.Settings, 4), false);
            await LoadExtraFile(context, GetPalettePath(context.Settings, 8), false);
            await LoadExtraFile(context, GetFontPalettePath(), true);

            // Load the files
            await LoadExtraFile(context, allfixPath, false);
            await LoadExtraFile(context, worldPath, false);
            await LoadExtraFile(context, levelPath, true);
            await LoadExtraFile(context, mapPath, true);
            await LoadExtraFile(context, tileSetPath, true);
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("JUN_F01.R16"),
            new PS1VignetteFileInfo("LOGO_UBI.R16", 640),
            new PS1VignetteFileInfo("PRES01A.R16", 640),
            new PS1VignetteFileInfo("PRES01B.R16", 640),
        };

        /// <summary>
        /// Gets the base directory name for exporting a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="des">The design to export</param>
        /// <returns>The base directory name</returns>
        protected override string GetExportDirName(GameSettings settings, Unity_ObjGraphics des)
        {
            // Get the file path
            var path = des.FilePath;

            if (path == null)
                throw new Exception("Path can not be null");

            if (path == GetAllfixFilePath())
                return $"Allfix/";
            else if (path == GetWorldFilePath(settings))
                return $"{settings.World}/{settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{settings.World}/{settings.World}{settings.Level} - ";

            return $"Unknown/";
        }

        public override async UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames)
        {
            using (var context = new Context(settings))
            {
                // Load files
                await LoadFilesAsync(context);

                // Read level file
                var level = FileFactory.Read<R1_PS1JPDemo_LevFile>(GetLevelFilePath(context.Settings), context);

                // Export
                await ExportMenuSpritesAsync(context, null, outputPath, exportAnimFrames, new R1_PS1_FontData[]
                {
                    level.FontData
                }, new R1_EventData[]
                {
                    level.RaymanEvent
                }, null);
            }
        }

        public override Dictionary<Unity_ObjectManager_R1.WldObjType, R1_EventData> GetEventTemplates(Context context)
        {
            var level = FileFactory.Read<R1_PS1JPDemo_LevFile>(GetLevelFilePath(context.Settings), context);

            return new Dictionary<Unity_ObjectManager_R1.WldObjType, R1_EventData>()
            {
                [Unity_ObjectManager_R1.WldObjType.Ray] = level.RaymanEvent,
            };
        }

        public override async UniTask<Texture2D> LoadLevelBackgroundAsync(Context context)
        {
            const string bgFilePath = "JUN_F01.R16";

            await LoadExtraFile(context, bgFilePath, true);

            var bg = FileFactory.Read<R1_PS1_VignetteBlockGroup>(bgFilePath, context, onPreSerialize: (s, x) => x.BlockGroupSize = (int)(s.CurrentLength / 2));

            return bg.ToTexture(context);
        }
    }
}