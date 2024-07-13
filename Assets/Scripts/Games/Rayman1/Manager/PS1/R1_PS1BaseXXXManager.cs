﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PlayStation.PS1;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PS1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Sprite = BinarySerializer.Ray1.Sprite;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// Base .xxx game manager for PS1
    /// </summary>
    public abstract class R1_PS1BaseXXXManager : R1_PS1BaseManager
    {
        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetWorldName(settings.R1_World)}{settings.Level:00}.XXX";

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"RAY.XXX";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public virtual string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"INI.XXX";

        /// <summary>
        /// Gets the file path for the font file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The font file path</returns>
        public virtual string GetFontFilePath(GameSettings settings) => GetDataPath() + $"LET2.IMG";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetWorldName(settings.R1_World)}.XXX";

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public virtual string GetWorldFolderPath(World world) => GetDataPath() + GetWorldName(world) + "/";

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public virtual string GetDataPath() => "RAY/";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.EnumerateWorlds().Where(w => Directory.Exists(settings.GameDirectory + GetWorldFolderPath(w)))
            .Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"{GetWorldName(w)}**.XXX", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Where(x => x.Length == 5)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).Where(x => x.Maps.Any()).Append(new GameInfo_World(7, new int[]
            {
                0 // Worldmap
            })).ToArray());

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImageAsync(settings, output)),
                new GameAction("Export background sprites", false, true, (input, output) => ExportBackgroundSpritesAsync(settings, output)),
                new GameAction("Test BIN Reader", false, false, (input,output) => TestBinRead(settings))
            }).ToArray();
        }

        public async UniTask TestBinRead(GameSettings settings) {
            using (var context = new Ray1MapContext(settings)) {
                await context.AddLinearFileAsync("disc.bin");
                var binFile = FileFactory.Read<ISO9960_BinFile>(context, "disc.bin");
            }
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("RAY/IMA/CRD/END_01.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC.XXX", 206, 199, 182, 195, 214, 187),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_CLOR.R16", 206),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_MAR.R16 ", 195),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_MOSR.R16", 182),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_RAYR.R16", 187),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_SKOR.R16", 199),
            new PS1VignetteFileInfo("RAY/IMA/CRD/VAC_TOOR.R16", 214),

            // EU only
            new PS1VignetteFileInfo("RAY/IMA/CRD/LANGUE.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/CRD/PIRACY.R16", 320),

            new PS1VignetteFileInfo("RAY/IMA/FND/NWORLD.R16"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF5.XXX"),
            // Ignore US exclusive duplicate
            //new PS1VignetteFileInfo("RAY/IMA/FND/IMGF21.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/JUNF6.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MONF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/MUSF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF4.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF5.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/CAVF6.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/GATF1.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/GATF2.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/GATF3.XXX"),
            new PS1VignetteFileInfo("RAY/IMA/FND/IMGF1.XXX"),

            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_JOE.R16", 162),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_MUS.R16", 159),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR1.R16", 254),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR2.R16", 208),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR3.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR4.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_PR5.R16", 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_RAP.R16", 171),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_TRZ.R16", 178),
            new PS1VignetteFileInfo("RAY/IMA/VIG/CONTINUE.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/FND01.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/FND02.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/LOGO_UBI.R16", 640),
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRE.XXX", 254, 208, 200, 200, 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRESENT.R16", 279),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_01R.R16", 219),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_02R.R16", 231),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_03R.R16", 257),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_04R.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_05R.R16", 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_06R.R16", 203),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_DRK.R16", 168),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_END.R16", 306),

            // JP only
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRES01A.R16", 640),
            new PS1VignetteFileInfo("RAY/IMA/VIG/PRES01B.R16", 640),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_CAK.R16", 203),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_CAV.R16", 146),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_HLO.R16", 320),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_IMG.R16", 200),
            new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_JUN.R16", 219),
            // This one seems broken or using some weird encoding
            //new PS1VignetteFileInfo("RAY/IMA/VIG/VIG_MON.R16", ???),
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

            if (path == GetAllfixFilePath(settings))
                return $"Allfix/";
            else if (path == GetWorldFilePath(settings))
                return $"{(World)settings.World}/{(World)settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{(World)settings.World}/{(World)settings.World}{settings.Level} - ";

            return $"Unknown/";
        }

        public async UniTask ExportPaletteImageAsync(GameSettings settings, string outputPath)
        {
            var spritePals = new List<RGBA5551Color[]>();
            var tilePals = new List<RGBA5551Color[]>();

            void Add(ICollection<RGBA5551Color[]> pals, Clut pal)
            {
                if (pal != null && !pals.Any(x => x.SequenceEqual(pal.Palette)))
                    pals.Add(pal.Palette);
            }

            // Enumerate every world
            foreach (var world in GetLevels(settings).First().Worlds)
            {
                settings.World = world.Index;
                settings.Level = 1;

                using (var context = new Ray1MapContext(settings))
                {
                    // Read the allfix file
                    await LoadExtraFile(context, GetAllfixFilePath(context.GetR1Settings()), false);
                    var allfix = FileFactory.Read<AllfixPack>(context, GetAllfixFilePath(context.GetR1Settings()));

                    // Read the BigRay file
                    await LoadExtraFile(context, GetBigRayFilePath(context.GetR1Settings()), false);
                    var br = FileFactory.Read<BigRayPack>(context, GetBigRayFilePath(context.GetR1Settings()));

                    Add(spritePals, allfix.Palette1);
                    Add(spritePals, allfix.Palette2);
                    Add(spritePals, allfix.Palette3);
                    Add(spritePals, allfix.Palette4);
                    Add(spritePals, allfix.Palette5);
                    Add(spritePals, allfix.Palette6);
                    Add(spritePals, br.Palette1);
                    Add(spritePals, br.Palette2);

                    // Read the world file
                    await LoadExtraFile(context, GetWorldFilePath(context.GetR1Settings()), false);
                    var wld = FileFactory.Read<WorldPack>(context, GetWorldFilePath(context.GetR1Settings()));

                    Add(spritePals, wld.Palette1);
                    Add(spritePals, wld.Palette2);

                    foreach (var tilePal in wld.TilePalettes ?? Array.Empty<Clut>())
                        Add(tilePals, tilePal);
                }
            }

            // Export
            PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), spritePals.Concat(tilePals).SelectMany(x => x).ToArray(), optionalWrap: 256);
        }

        public async UniTask ExportBackgroundSpritesAsync(GameSettings settings, string outputPath)
        {
            HashSet<string> exportedFiles = new();

            // Enumerate every world
            foreach (var world in GetLevels(settings).First().Worlds)
            {
                // Don't export menu
                if (world.Index == 7)
                    break;

                settings.World = world.Index;

                // Enumerate every level
                foreach (var lvl in world.Maps)
                {
                    settings.Level = lvl;

                    try
                    {
                        // Create the context
                        using var context = new Ray1MapContext(settings);

                        // Load the level
                        await LoadFilesAsync(context);
                        await LoadAsync(context);

                        Executable exe = LoadEXE(context);
                        byte bgIndex = exe.PS1_LevelBackgroundIndexTable[settings.World - 1][settings.Level - 1];
                        int fndStartIndex = exe.GetFileTypeIndex(GetExecutableConfig, FileType.fnd_file);

                        if (fndStartIndex == -1)
                            continue;

                        string bgFilePath = exe.PS1_FileTable[fndStartIndex + bgIndex].ProcessedFilePath;

                        if (exportedFiles.Contains(bgFilePath))
                            continue;

                        exportedFiles.Add(bgFilePath);

                        FondPack bg = FileFactory.Read<FondPack>(context, bgFilePath);

                        if (bg.SpriteData == null)
                            continue;
                        
                        VRAM vram = context.GetRequiredStoredObject<VRAM>("vram");
                        bool isJp = context.GetRequiredSettings<Ray1Settings>().EngineVersion == Ray1EngineVersion.PS1_JP;

                        short clutX = (short)(isJp ? 0x40 : 0x300);
                        short clutY = (short)(isJp ? 0x1fc : 0x1f7);

                        for (int i = 0; i < bg.SpriteData.PalettesCount; i++)
                        {
                            var rect = new BinarySerializer.PlayStation.PS1.Rect(clutX, (short)(clutY - i), 0x100, 1);
                            vram.AddPalette(bg.SpriteData.Palettes[i].Palette, rect);
                        }

                        var levelPack = FileFactory.Read<LevelPack>(context, GetLevelFilePath(settings));

                        for (int i = 0; i < bg.SpriteData.SpritesCount; i++)
                        {
                            Sprite sprite = levelPack.BackgroundData.Sprites[i];
                            Texture2D tex = GetSpriteTexture(context, null, sprite);

                            // Export it
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(bgFilePath)}_{i}.png"), tex.EncodeToPNG());
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to export {settings.World}-{settings.Level}: {ex}");
                    }

                    // Unload textures
                    await Resources.UnloadUnusedAssets();
                }
            }
        }

        public override async UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames)
        {
            using (var menuContext = new Ray1MapContext(settings)) 
            {
                using (var bigRayContext = new Ray1MapContext(settings))
                {
                    await LoadFilesAsync(menuContext);
                    await LoadFilesAsync(bigRayContext);

                    // Read the allfix & font files for the menu
                    await LoadExtraFile(menuContext, GetAllfixFilePath(menuContext.GetR1Settings()), false);
                    var fix = FileFactory.Read<AllfixPack>(menuContext, GetAllfixFilePath(menuContext.GetR1Settings()));
                    await LoadExtraFile(menuContext, GetFontFilePath(menuContext.GetR1Settings()), false);

                    // Correct font palette
                    if (settings.EngineVersion == EngineVersion.R1_PS1_JP)
                    {
                        foreach (Alpha font in new[] { fix.AllfixData.Alpha, fix.AllfixData.Alpha2 })
                        {
                            foreach (Sprite imgDescr in font.Sprites)
                            {
                                imgDescr.Clut.ClutY = 509;
                            }
                        }
                    }
                    else
                    {
                        foreach (Alpha font in new[] { fix.AllfixData.Alpha, fix.AllfixData.Alpha2 })
                        {
                            foreach (Sprite imgDescr in font.Sprites)
                            {
                                imgDescr.Clut.ClutY = 492;
                            }
                        }
                    }

                    // Read the BigRay file
                    await LoadExtraFile(bigRayContext, GetBigRayFilePath(bigRayContext.GetR1Settings()), false);
                    var br = bigRayContext.FileExists(GetBigRayFilePath(bigRayContext.GetR1Settings())) ? FileFactory.Read<BigRayPack>(bigRayContext, GetBigRayFilePath(bigRayContext.GetR1Settings())) : null;

                    // Export
                    await ExportMenuSpritesAsync(menuContext, bigRayContext, outputPath, exportAnimFrames, new Alpha[]
                    {
                        fix.AllfixData.Alpha,
                        fix.AllfixData.Alpha2,
                    }, new ObjData[]
                    {
                        fix.AllfixData.Ray,
                        fix.AllfixData.RayLittle,
                        fix.AllfixData.ClockObj,
                        fix.AllfixData.DivObj,
                    }.Concat(fix.AllfixData.MapObj).ToArray(), br?.BigRayData);
                }
            }
        }

        public override async UniTask<Texture2D> LoadLevelBackgroundAsync(Context context)
        {
            var exe = LoadEXE(context);

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                if (exe.PS1_LevelBackgroundIndexTable == null)
                    return null;

                var bgIndex = exe.PS1_LevelBackgroundIndexTable[context.GetR1Settings().World - 1][context.GetR1Settings().Level - 1];
                var fndStartIndex = exe.GetFileTypeIndex(GetExecutableConfig, FileType.fnd_file);

                if (fndStartIndex == -1)
                    return null;

                string bgFilePath = exe.PS1_FileTable[fndStartIndex + bgIndex].ProcessedFilePath;

                await LoadExtraFile(context, bgFilePath, true);

                var bg = FileFactory.Read<FondPack>(context, bgFilePath);

                return bg.Fond.ToTexture(context);
            }
            else
            {
                string bgFilePath = exe.PS1_FileTable[exe.GetFileTypeIndex(GetExecutableConfig, FileType.img_file) + 2].ProcessedFilePath;
                await LoadExtraFile(context, bgFilePath, true);

                return FileFactory.Read<Fond>(context, bgFilePath).ToTexture(context);
            }
        }

        public override Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData> GetEventTemplates(Context context)
        {
            var allfix = FileFactory.Read<AllfixPack>(context, GetAllfixFilePath(context.GetR1Settings())).AllfixData;

            return new Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData>()
            {
                [Unity_ObjectManager_R1.WldObjType.Ray] = allfix.Ray,
                [Unity_ObjectManager_R1.WldObjType.RayLittle] = allfix.RayLittle,
                [Unity_ObjectManager_R1.WldObjType.ClockObj] = allfix.ClockObj,
                [Unity_ObjectManager_R1.WldObjType.DivObj] = allfix.DivObj,
                [Unity_ObjectManager_R1.WldObjType.MapObj] = allfix.MapObj[0],
            };
        }
    }
}