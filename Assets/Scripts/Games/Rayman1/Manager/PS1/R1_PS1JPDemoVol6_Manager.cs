using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map.Rayman1
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

        protected override PS1_MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1_MemoryMappedFile.InvalidPointerMode.Allow;

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
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.EnumerateWorlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory, $"{GetWorldName(w)}*.MAP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray());

        public override string ExeFilePath => "RAY_GAME.EXE";
        public override uint? ExeBaseAddress => 0x80150000 - 0x800;
        protected override PS1_ExecutableConfig GetExecutableConfig => PS1_ExecutableConfig.PS1_JPDemoVol6;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_TileSet GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetTileSetFilePath(context.GetR1Settings());

            // Read the file
            var tileSet = FileFactory.Read<ObjectArray<RGBA5551Color>>(filename, context, (s, x) => x.Pre_Length = s.CurrentLength / 2);

            // Return the tile set
            return new Unity_TileSet(tileSet.Value, TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, PS1VramHelpers.VRAMMode mode)
        {
            // Read palettes
            var pal4 = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetPalettePath(context.GetR1Settings(), 4), context, (y, x) => x.Pre_Length = y.CurrentLength / 2);
            var pal8 = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetPalettePath(context.GetR1Settings(), 8), context, (y, x) => x.Pre_Length = y.CurrentLength / 2);
            var palLettre = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetFontPalettePath(), context, (y, x) => x.Pre_Length = y.CurrentLength / 2);

            // Read the files
            var fixGraphics = FileFactory.Read<Array<byte>>(GetAllfixSpritePath(), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);
            var wldGraphics = FileFactory.Read<Array<byte>>(GetWorldSpritePath(context.GetR1Settings()), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);
            var lvlGraphics = FileFactory.Read<Array<byte>>(GetLevelSpritePath(context.GetR1Settings()), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);

            var vram = PS1VramHelpers.PS1_JPDemoVol6_FillVRAM(pal4.Value, pal8.Value, palLettre.Value, fixGraphics.Value, wldGraphics.Value, lvlGraphics.Value);

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get the file paths
            var mapPath = GetMapFilePath(context.GetR1Settings());
            var levelPath = GetLevelFilePath(context.GetR1Settings());

            // Read the files
            var map = FileFactory.Read<MapData>(mapPath, context);
            var level = FileFactory.Read<PS1_JPDemo_LevFile>(levelPath, context);

            // Load the level
            return await LoadAsync(context, map, level.Objects, level.ObjectLinkTable.Select(x => (ushort)x).ToArray());
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            await base.LoadFilesAsync(context);

            // Get the file paths
            var allfixPath = GetAllfixFilePath();
            var worldPath = GetWorldFilePath(context.GetR1Settings());
            var levelPath = GetLevelFilePath(context.GetR1Settings());
            var mapPath = GetMapFilePath(context.GetR1Settings());
            var tileSetPath = GetTileSetFilePath(context.GetR1Settings());

            // sprites
            await LoadExtraFile(context, GetAllfixSpritePath(), true);
            await LoadExtraFile(context, GetWorldSpritePath(context.GetR1Settings()), true);
            await LoadExtraFile(context, GetLevelSpritePath(context.GetR1Settings()), true);
            await LoadExtraFile(context, GetPalettePath(context.GetR1Settings(), 4), false);
            await LoadExtraFile(context, GetPalettePath(context.GetR1Settings(), 8), false);
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
                return $"{(World)settings.World}/{(World)settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{(World)settings.World}/{(World)settings.World}{settings.Level} - ";

            return $"Unknown/";
        }

        public override async UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames)
        {
            using (var context = new Ray1MapContext(settings))
            {
                // Load files
                await LoadFilesAsync(context);

                // Read level file
                var level = FileFactory.Read<PS1_JPDemo_LevFile>(GetLevelFilePath(context.GetR1Settings()), context);

                // Export
                await ExportMenuSpritesAsync(context, null, outputPath, exportAnimFrames, new PS1_FontData[]
                {
                    level.FontData
                }, new ObjData[]
                {
                    level.RaymanObj
                }, null);
            }
        }

        public override Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData> GetEventTemplates(Context context)
        {
            var level = FileFactory.Read<PS1_JPDemo_LevFile>(GetLevelFilePath(context.GetR1Settings()), context);

            return new Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData>()
            {
                [Unity_ObjectManager_R1.WldObjType.Ray] = level.RaymanObj,
            };
        }

        public override async UniTask<Texture2D> LoadLevelBackgroundAsync(Context context)
        {
            const string bgFilePath = "JUN_F01.R16";

            await LoadExtraFile(context, bgFilePath, true);

            var bg = FileFactory.Read<PS1_VignetteBlockGroup>(bgFilePath, context, onPreSerialize: (s, x) => x.BlockGroupSize = s.CurrentLength / 2);

            return bg.ToTexture(context);
        }
    }
}