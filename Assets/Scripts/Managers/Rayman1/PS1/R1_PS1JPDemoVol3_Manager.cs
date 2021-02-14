using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class R1_PS1JPDemoVol3_Manager : R1_PS1BaseManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 40;

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath() => $"RAY.FXS";

        public string GetPalettePath(GameSettings settings, int i) => $"RAY{i}_{(settings.R1_World == R1_World.Jungle ? 1 : 2)}.PAL";

        /// <summary>
        /// Gets the file path for the world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => $"RAY.WL{(settings.R1_World == R1_World.Jungle ? 1 : 2)}";

        /// <summary>
        /// Gets the file path for the level file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => $"RAY.LV{(settings.R1_World == R1_World.Jungle ? 1 : 2)}";
        
        /// <summary>
        /// Gets the file path for the level tile set file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level tile set file path</returns>
        public virtual string GetTileSetFilePath(GameSettings settings) => $"_{GetWorldName(settings.R1_World)}_{settings.Level:00}.R16";

        /// <summary>
        /// Gets the file path for the level map file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level map file path</returns>
        public virtual string GetMapFilePath(GameSettings settings) => $"_{GetWorldName(settings.R1_World)}_{settings.Level:00}.MAP";

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public override string GetWorldName(R1_World world) => base.GetWorldName(world).Substring(1);

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory, $"_{GetWorldName(w)}_*.MAP", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(4)))
            .ToArray())).ToArray());

        public override string ExeFilePath => "RAY.EXE";
        public override uint? ExeBaseAddress => 0x80180000 - 0x800;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801b5f3c, 1, R1_PS1_FileType.pal_file),
            new FileTableInfo(0x801b5f74, 4, R1_PS1_FileType.demo_vig),
            new FileTableInfo(0x801b6054, 1, R1_PS1_FileType.img_file),
            new FileTableInfo(0x801b608c, 1, R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801b60c4, 0xb, R1_PS1_FileType.demo_w1),
            new FileTableInfo(0x801b6594, 0xb, R1_PS1_FileType.demo_w2),
            new FileTableInfo(0x801b67fc, 4, R1_PS1_FileType.fnd_file),
            new FileTableInfo(0x801b68fc, 1, R1_PS1_FileType.demo_track)
        };

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_TileSet GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetTileSetFilePath(context.Settings);

            // Read the file
            var tileSet = FileFactory.Read<ObjectArray<RGBA5551Color>>(filename, context, (s, x) => x.Length = s.CurrentLength / 2);

            // Return the tile set
            return new Unity_TileSet(tileSet.Value, TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, VRAMMode mode)
        {
            // We don't need to emulate the v-ram for this version
            return;// null;
        }

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="imgBuffer">The image buffer, if available</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public override Texture2D GetSpriteTexture(Context context, byte[] imgBuffer, R1_ImageDescriptor s)
        {
            if (s.ImageType != 2 && s.ImageType != 3) 
                return null;

            // Ignore dummy sprites
            if (s.IsDummySprite())
                return null;

            // Get the image properties
            var width = s.Width;
            var height = s.Height;
            var offset = s.ImageBufferOffset;

            var pal4 = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetPalettePath(context.Settings, 4), context, (y, x) => x.Length = 256);
            var pal8 = FileFactory.Read<ObjectArray<RGBA5551Color>>(GetPalettePath(context.Settings, 8), context, (y, x) => x.Length = 256);

            // Select correct palette
            var palette = s.ImageType == 3 ? pal8.Value : pal4.Value;
            var paletteOffset = 16 * s.Unknown1;

            // Create the texture
            Texture2D tex = TextureHelpers.CreateTexture2D(width, height);

            // Set every pixel
            if (s.ImageType == 3)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var paletteIndex = imgBuffer[offset + width * y + x];

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteIndex].GetColor());
                    }
                }
            }
            else if (s.ImageType == 2)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int actualX = (s.ImageOffsetInPageX + x) / 2;
                        var paletteIndex = imgBuffer[offset + (width * y + x) / 2];
                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteOffset + paletteIndex].GetColor());
                    }
                }
            }

            // Apply the changes
            tex.Apply();

            // Return the texture
            return tex;
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
            var levelPath = GetLevelFilePath(context.Settings);
            var mapPath = GetMapFilePath(context.Settings);

            // Load the files
            await LoadFilesAsync(context);

            // Read the files
            var map = FileFactory.Read<MapData>(mapPath, context);
            var lvl = FileFactory.Read<R1_PS1JPDemo_LevFile>(levelPath, context);

            // Load the level
            return await LoadAsync(context, map, lvl.Events, lvl.EventLinkTable.Select(x => (ushort)x).ToArray(), loadTextures);
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
            var pal4Path = GetPalettePath(context.Settings, 4);
            var pal8Path = GetPalettePath(context.Settings, 8);

            // Load the files
            await LoadExtraFile(context, allfixPath, false);
            await LoadExtraFile(context, worldPath, false);
            await LoadExtraFile(context, levelPath, true);
            await LoadExtraFile(context, mapPath, true);
            await LoadExtraFile(context, tileSetPath, true);
            await LoadExtraFile(context, pal4Path, true);
            await LoadExtraFile(context, pal8Path, true);
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("JUN_F02.R16"), 
            new PS1VignetteFileInfo("MON_F2W.R16"), 
            new PS1VignetteFileInfo("VIG_0P.R16", 260), 
            new PS1VignetteFileInfo("VIG_1P.R16", 320), 
            new PS1VignetteFileInfo("VIG_02P.R16", 257), 
            new PS1VignetteFileInfo("VIG_7P.R16", 320), 
            new PS1VignetteFileInfo("WORLD.R16", 320), 
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
            var exe = FileFactory.Read<R1_PS1_Executable>(ExeFilePath, context);

            var bgIndex = context.Settings.R1_World == R1_World.Jungle ? 0 : 2;
            var fndStartIndex = exe.GetFileTypeIndex(this, R1_PS1_FileType.fnd_file);

            if (fndStartIndex == -1)
                return null;

            var bgFilePath = exe.FileTable[fndStartIndex + bgIndex].ProcessedFilePath;

            await LoadExtraFile(context, bgFilePath, true);

            var bg = FileFactory.Read<R1_PS1_VignetteBlockGroup>(bgFilePath, context, onPreSerialize: (s, x) => x.BlockGroupSize = (int)(s.CurrentLength / 2));

            return bg.ToTexture(context);
        }

        /*

        World info table: (x, y, up, down, left, right)
        36 00 B5 00 02 00 01 00 01 00 02 00 // Jungle
        5E 00 7E 00 02 00 01 00 07 00 03 00 // Music
        A3 00 5D 00 03 00 03 00 02 00 05 00 // Mountain
        1C 01 A2 00 06 00 04 00 05 00 06 00 // Image
        E6 00 73 00 03 00 04 00 03 00 04 00 // Cave
        2D 01 55 00 06 00 04 00 04 00 06 00 // Cake
        23 00 5F 00 07 00 02 00 07 00 02 00 // Present (Breakout?)

        */
    }
}