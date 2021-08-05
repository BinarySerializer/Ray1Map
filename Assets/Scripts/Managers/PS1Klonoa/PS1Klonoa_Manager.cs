using BinarySerializer;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer.KlonoaDTP;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class PS1Klonoa_Manager : BaseGameManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Levels.Select((x, i) => new GameInfo_World(i, x.Item1, Enumerable.Range(0, x.Item2).ToArray())).ToArray());

        public virtual (string, int)[] Levels => new (string, int)[]
        {
            ("FIX", 0),
            ("MENU", 0),
            ("CODE", 0),

            ("Vision 1-1", 3),
            ("Vision 1-2", 5),
            ("Rongo Lango", 2),

            ("Vision 2-1", 4),
            ("Vision 2-2", 6),
            ("Pamela", 2),

            ("Vision 3-1", 5),
            ("Vision 3-2", 10),
            ("Gelg Bolm", 1),

            ("Vision 4-1", 3),
            ("Vision 4-2", 8),
            ("Baladium", 2),

            ("Vision 5-1", 7),
            ("Vision 5-2", 9),
            ("Joka", 1),

            ("Vision 6-1", 8),
            ("Vision 6-2", 8),
            ("", 2),
            ("", 2),
            ("", 3),
            ("", 3),
            ("Klonoa's Grand Gale Strategy", 9),
        };

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Extract BIN", false, true, (input, output) => Extract_BINAsync(settings, output, false)),
                new GameAction("Extract BIN (unpack archives)", false, true, (input, output) => Extract_BINAsync(settings, output, true)),
                new GameAction("Extract Code", false, true, (input, output) => Extract_CodeAsync(settings, output)),
                new GameAction("Extract Graphics", false, true, (input, output) => Extract_GraphicsAsync(settings, output)),
                new GameAction("Extract Backgrounds", false, true, (input, output) => Extract_BackgroundsAsync(settings, output)),
                new GameAction("Extract Sprites", false, true, (input, output) => Extract_SpriteFramesAsync(settings, output)),
                new GameAction("Extract ULZ blocks", false, true, (input, output) => Extract_ULZAsync(settings, output)),
            };
        }

        public async UniTask Extract_BINAsync(GameSettings settings, string outputPath, bool unpack)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var s = context.Deserializer;

            var loader = Loader.Create(context, idxData, config);

            var archiveDepths = new Dictionary<IDXLoadCommand.FileType, int>()
            {
                [IDXLoadCommand.FileType.Unknown] = 0,

                [IDXLoadCommand.FileType.Archive_TIM_Generic] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SongsText] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SaveText] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SpriteSheets] = 1,

                [IDXLoadCommand.FileType.OA05] = 0,
                [IDXLoadCommand.FileType.SEQ] = 0,

                [IDXLoadCommand.FileType.Archive_BackgroundPack] = 2,

                [IDXLoadCommand.FileType.FixedSprites] = 1,
                [IDXLoadCommand.FileType.Archive_SpritePack] = 1,
                [IDXLoadCommand.FileType.Archive_LevelMenuSprites] = 2,
                
                [IDXLoadCommand.FileType.Archive_LevelPack] = 1,

                [IDXLoadCommand.FileType.Archive_WorldMap] = 1,
                
                [IDXLoadCommand.FileType.Archive_MenuSprites] = 2,
                [IDXLoadCommand.FileType.Font] = 0,
                [IDXLoadCommand.FileType.Archive_MenuBackgrounds] = 2,
                
                [IDXLoadCommand.FileType.Archive_Unk0] = 1,
                [IDXLoadCommand.FileType.Unk1] = 0,

                [IDXLoadCommand.FileType.Code] = 0,
                [IDXLoadCommand.FileType.CodeNoDest] = 0,
            };

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    var type = cmd.FILE_Type;

                    if (unpack)
                    {
                        var archiveDepth = archiveDepths[type];

                        if (archiveDepth > 0)
                        {
                            // Be lazy and hard-code instead of making some recursive loop
                            if (archiveDepth == 1)
                            {
                                var archive = loader.LoadBINFile<RawData_ArchiveFile>(i);

                                for (int j = 0; j < archive.Files.Length; j++)
                                {
                                    var file = archive.Files[j];

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"{j}.bin"), file.Data);
                                }
                            }
                            else if (archiveDepth == 2)
                            {
                                var archives = loader.LoadBINFile<ArchiveFile<RawData_ArchiveFile>>(i);

                                for (int a = 0; a < archives.Files.Length; a++)
                                {
                                    var archive = archives.Files[a];

                                    for (int j = 0; j < archive.Files.Length; j++)
                                    {
                                        var file = archive.Files[j];

                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"{a}_{j}.bin"), file.Data);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception($"Unsupported archive depth");
                            }

                            return;
                        }
                    }

                    // Read the raw data
                    var data = s.SerializeArray<byte>(null, cmd.FILE_Length);

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"Data.bin"), data);
                });
            }
        }

        public async UniTask Extract_CodeAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    // TODO: Also export no-destination code blocks

                    if (cmd.FILE_Type != IDXLoadCommand.FileType.Code)
                        return;
                    
                    var codeFile = loader.LoadBINFile<RawData_File>(i);

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - 0x{cmd.GetFileDestinationAddress(loader):X8}.dat"), codeFile.Data);
                });
            }
        }

        public async UniTask Extract_GraphicsAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    var index = 0;

                    switch (cmd.FILE_Type)
                    {
                        case IDXLoadCommand.FileType.Code:
                        case IDXLoadCommand.FileType.CodeNoDest:
                            loader.ProcessBINFile(i);
                            break;

                        case IDXLoadCommand.FileType.Archive_TIM_Generic:
                        case IDXLoadCommand.FileType.Archive_TIM_SongsText:
                        case IDXLoadCommand.FileType.Archive_TIM_SaveText:
                        case IDXLoadCommand.FileType.Archive_TIM_SpriteSheets:

                            // Read the data
                            TIM_ArchiveFile timFiles = loader.LoadBINFile<TIM_ArchiveFile>(i);

                            foreach (var tim in timFiles.Files)
                            {
                                export(() => GetTexture(tim), cmd.FILE_Type.ToString().Replace("Archive_TIM_", String.Empty));
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_WorldMap:

                            // Read the data
                            var f = loader.LoadBINFile<WorldMap_ArchiveFile>(i);

                            foreach (var tim in f.SpriteSheets.Files)
                            {
                                export(() => GetTexture(tim), "WorldMap");
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_BackgroundPack:

                            // Read the data
                            var bgPack = loader.LoadBINFile<BackgroundPack_ArchiveFile>(i);

                            foreach (var tim in bgPack.TIMFiles.Files)
                            {
                                export(() => GetTexture(tim), "Background");
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_MenuBackgrounds:

                            // Read the data
                            var timArchives = loader.LoadBINFile<ArchiveFile<TIM_ArchiveFile>>(i);

                            var timsIndex = 0;
                            foreach (var tims in timArchives.Files)
                            {
                                foreach (var tim in tims.Files)
                                {
                                    export(() => GetTexture(tim, onlyFirstTransparent: true), $"MenuBackground {timsIndex}");
                                    index++;
                                }

                                timsIndex++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_SpritePack:

                            // Read the data
                            LevelSpritePack_ArchiveFile spritePack = loader.LoadBINFile<LevelSpritePack_ArchiveFile>(i);

                            var exportedPlayerSprites = new HashSet<PlayerSprite_File>();

                            var pal = spritePack.PlayerSprites.Files.FirstOrDefault(x => x?.TIM?.Clut != null)?.TIM.Clut.Palette.Select(x => x.GetColor()).ToArray();

                            foreach (var file in spritePack.PlayerSprites.Files)
                            {
                                if (file != null && !exportedPlayerSprites.Contains(file))
                                {
                                    exportedPlayerSprites.Add(file);

                                    export(() =>
                                    {
                                        if (file.TIM != null)
                                            return GetTexture(file.TIM, palette: pal);
                                        else
                                            return GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height, PS1_TIM.TIM_ColorFormat.BPP_8);
                                    }, "PlayerSprites");
                                }

                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_LevelPack:

                            // Need to process the data for the object 3D export
                            loader.ProcessBINFile(i);

                            // TODO: Remove try/catch
                            try
                            {
                                // Read the data
                                var lvlPack = loader.LoadBINFile<LevelPack_ArchiveFile>(i);

                                var cutscenePack = lvlPack.CutscenePack;

                                if (cutscenePack != null)
                                {
                                    export(() => GetTexture(cutscenePack.CharacterNamesImgData.Data, null, 0x0C, 0x50, PS1_TIM.TIM_ColorFormat.BPP_4), "CharacterNames");

                                    foreach (var file in cutscenePack.File_2.Files)
                                    {
                                        export(() => GetTexture(file.ImgData, null, file.Width / 2, file.Height, PS1_TIM.TIM_ColorFormat.BPP_8), "CutsceneFrames");

                                        index++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning(ex);
                            }
                            break;
                    }

                    void export(Func<Texture2D> getTex, string type) => exportTex(getTex, type, $"{i} - {index}");
                });

                if (blockIndex < loader.Config.BLOCK_FirstLevel)
                    continue;

                // Process code level data
                loader.ProcessLevelData();

                var exported = new HashSet<BinarySerializable>();

                for (int sector = 0; sector < loader.LevelData3D.SectorModifiers.Length; sector++)
                {
                    var modifiers = loader.LevelData3D.SectorModifiers[sector];

                    for (int modifierIndex = 0; modifierIndex < modifiers.Modifiers.Length; modifierIndex++)
                    {
                        var modifier = modifiers.Modifiers[modifierIndex];

                        if (modifier.DataFiles == null)
                            continue;

                        foreach (var dataFile in modifier.DataFiles)
                        {
                            if (dataFile.TIM != null)
                            {
                                if (exported.Contains(dataFile.TIM))
                                    continue;

                                exported.Add(dataFile.TIM);

                                exportTex(() => GetTexture(dataFile.TIM), "Obj3D", $"{sector} - {modifierIndex}");
                            }
                            else if (dataFile.TextureAnimation != null)
                            {
                                if (exported.Contains(dataFile.TextureAnimation))
                                    continue;

                                exported.Add(dataFile.TextureAnimation);

                                for (var timIndex = 0; timIndex < dataFile.TextureAnimation.Files.Length; timIndex++)
                                {
                                    var tim = dataFile.TextureAnimation.Files[timIndex];
                                    exportTex(() => GetTexture(tim), "Obj3D", $"{sector} - {modifierIndex} - {timIndex}");
                                }
                            }
                        }
                    }
                }

                void exportTex(Func<Texture2D> getTex, string type, string name)
                {
                    try
                    {
                        var tex = getTex();

                        if (tex != null)
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {type}", $"{name}.png"),
                                tex.EncodeToPNG());
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting with ex: {ex}");
                    }
                }
            }
        }

        public async UniTask Extract_SpriteFramesAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Load allfix
            loader.SwitchBlocks(0);
            loader.LoadAndProcessBINBlock();

            // Extract world map sprites with both palettes
            for (int i = 0; i < 2; i++)
            {
                var wld = loader.WorldMap;

                if (i == 1)
                    loader.AddToVRAM(wld.Palette2);

                // Enumerate every frame
                for (int frameIndex = 0; frameIndex < wld.AnimatedSprites.Sprites.Files.Length - 1; frameIndex++)
                    export(wld.AnimatedSprites.Sprites.Files[frameIndex].Textures, $"WorldMap", i, frameIndex, -1, -1); // TODO: Correct pal
            }

            PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_WorldMap.png"), loader.VRAM);

            // Load menu
            loader.SwitchBlocks(1);
            loader.LoadAndProcessBINBlock();

            // Extract menu sprites
            for (int frameIndex = 0; frameIndex < loader.MenuSprites.Sprites_0.Files.Length - 1; frameIndex++)
                export(loader.MenuSprites.Sprites_0.Files[frameIndex].Textures, $"Menu", 0, frameIndex, 960, 442);

            for (int frameIndex = 0; frameIndex < loader.MenuSprites.Sprites_1.Files.Length - 1; frameIndex++)
                export(loader.MenuSprites.Sprites_1.Files[frameIndex].Textures, $"Menu", 1, frameIndex, 0, 480);

            for (int frameIndex = 0; frameIndex < loader.MenuSprites.Sprites_2.Files.Length - 1; frameIndex++)
                export(loader.MenuSprites.Sprites_2.Files[frameIndex].Textures, $"Menu", 2, frameIndex, 960, 440);

            for (int frameIndex = 0; frameIndex < loader.MenuSprites.AnimatedSprites.Sprites.Files.Length - 1; frameIndex++)
            {
                int palY;

                if (frameIndex < 120)
                    palY = 490;
                else if (frameIndex < 166)
                    palY = 480;
                else if (frameIndex < 178)
                    palY = 490;
                else if (frameIndex < 182)
                    palY = -1; // TODO: Correct pal
                else
                    palY = 480;

                export(loader.MenuSprites.AnimatedSprites.Sprites.Files[frameIndex].Textures, $"Menu", 3, frameIndex, 0, palY);
            }

            PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_Menu.png"), loader.VRAM);

            // Enumerate every entry
            for (var blockIndex = 2; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Load the BIN
                loader.LoadAndProcessBINBlock();

                // Enumerate every set of frames
                for (int framesSet = 0; framesSet < loader.SpriteFrames.Length; framesSet++)
                {
                    var spriteFrames = loader.SpriteFrames[framesSet];

                    if (spriteFrames == null)
                        continue;

                    // Enumerate every frame
                    for (int frameIndex = 0; frameIndex < spriteFrames.Files.Length - 1; frameIndex++)
                        export(spriteFrames.Files[frameIndex].Textures, $"{blockIndex}", framesSet, frameIndex, 0, 500);
                }

                if (loader.LevelMenuSprites != null)
                {
                    for (int frameIndex = 0; frameIndex < loader.LevelMenuSprites.Sprites_0.Files.Length; frameIndex++)
                        export(loader.LevelMenuSprites.Sprites_0.Files[frameIndex].Textures, $"{blockIndex} - Menu", 0, frameIndex, 960, 442);

                    for (int frameIndex = 0; frameIndex < loader.LevelMenuSprites.Sprites_1.Files.Length; frameIndex++)
                        export(loader.LevelMenuSprites.Sprites_1.Files[frameIndex].Textures, $"{blockIndex} - Menu", 1, frameIndex, -1, -1); // TODO: Correct pal
                }

                PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), loader.VRAM);
            }

            void export(SpriteTexture[] spriteTextures, string blockName, int setIndex, int frameIndex, int palX, int palY)
            {
                try
                {
                    var tex = GetTexture(spriteTextures, loader.VRAM, palX, palY);

                    Util.ByteArrayToFile(Path.Combine(outputPath, blockName, $"{setIndex} - {frameIndex}.png"), tex.EncodeToPNG());
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting sprite frame: {ex}");
                }
            }
        }

        public async UniTask Extract_BackgroundsAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    try
                    {
                        // Check the file type
                        if (cmd.FILE_Type != IDXLoadCommand.FileType.Archive_BackgroundPack)
                            return;

                        // Read the data
                        var bg = loader.LoadBINFile<BackgroundPack_ArchiveFile>(i);

                        var exportedMaps = new bool[bg.BGDFiles.Files.Length];

                        // Export for each sector
                        foreach (var modifiersFile in bg.BackgroundModifiersFiles.Files)
                            export(modifiersFile.Modifiers.Where(x => !x.IsLayer || !exportedMaps[x.BGDIndex]).ToArray());

                        // Export unreferenced backgrounds
                        for (int j = 0; j < exportedMaps.Length; j++)
                        {
                            if (!exportedMaps[j])
                            {
                                export(new BackgroundModifierObject[]
                                {
                                    new BackgroundModifierObject
                                    {
                                        Type = BackgroundModifierObject.BackgroundModifierType.BackgroundLayer_19,
                                        BGDIndex = j,
                                        CELIndex = 0,
                                    }
                                });
                            }
                        }

                        void export(BackgroundModifierObject[] modifiers)
                        {
                            Texture2D[][] textures;
                            int animSpeed;

                            (textures, animSpeed) = GetBackgrounds(loader, bg, modifiers);

                            for (int texIndex = 0; texIndex < textures.Length; texIndex++)
                            {
                                var bgTex = textures[texIndex];
                                var bgIndex = modifiers.Where(x => x.IsLayer).ElementAt(texIndex).BGDIndex;

                                exportedMaps[bgIndex] = true;

                                if (bgTex.Length > 1)
                                {
                                    Util.ExportAnimAsGif(bgTex, animSpeed * 2, false, false, Path.Combine(outputPath, $"{blockIndex} - {i} - {bgIndex}.gif"));
                                }
                                else
                                {
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - {bgIndex}.png"), bgTex[0].EncodeToPNG());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting with ex: {ex}");
                    }
                });

                PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), loader.VRAM);
            }
        }

        public async UniTask Extract_ULZAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);

            await LoadFilesAsync(context);

            var s = context.Deserializer;

            s.Goto(context.GetFile(Loader.FilePath_BIN).StartPointer);

            while (s.CurrentFileOffset < s.CurrentLength)
            {
                var v = s.Serialize<int>(default);

                if (v != 0x1A7A6C55)
                    continue;

                var offset = s.CurrentPointer - 4;

                s.DoAt(offset, () =>
                {
                    try
                    {
                        var bytes = s.DoEncoded(new ULZEncoder(), () => s.SerializeArray<byte>(default, s.CurrentLength));

                        Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{offset.AbsoluteOffset:X8}.bin"), bytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error decompressing at {offset} with ex: {ex}");
                    }
                });
            }
        }

        public LoaderConfiguration GetLoaderConfig(GameSettings settings)
        {
            // TODO: Support other versions
            return new LoaderConfiguration_DTP_US();
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            var stopWatch = Stopwatch.StartNew();
            var startupLog = new StringBuilder();

            // Get settings
            GameSettings settings = context.GetR1Settings();
            int lev = settings.World;
            int sector = settings.Level;
            LoaderConfiguration config = GetLoaderConfig(settings);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Retrieved settings");

            // Load the files
            await LoadFilesAsync(context, config);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded files");

            Controller.DetailedState = "Loading IDX";
            await Controller.WaitIfNecessary();

            // Load the IDX
            IDX idxData = Load_IDX(context);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded IDX");

            ArchiveFile.AddToParsedArchiveFiles = true;

            Controller.DetailedState = "Loading BIN";
            await Controller.WaitIfNecessary();

            // Create the loader
            var loader = Loader.Create(context, idxData, config);

            // Only parse the selected sector
            loader.LevelSector = sector;

            var logAction = new Func<string, Task>(async x =>
            {
                Controller.DetailedState = x;
                await Controller.WaitIfNecessary();
            });

            // Load the fixed BIN
            loader.SwitchBlocks(loader.Config.BLOCK_Fix);
            await loader.LoadAndProcessBINBlockAsync(logAction);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded fixed BIN");

            // Load the level BIN
            loader.SwitchBlocks(lev);
            await loader.LoadAndProcessBINBlockAsync(logAction);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded level BIN");

            Controller.DetailedState = "Loading level data";
            await Controller.WaitIfNecessary();

            // Load hard-coded level data
            loader.ProcessLevelData();

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded hard-coded level data");

            const float scale = 64f;

            // Load the layers
            Unity_Layer[] layers = await Load_LayersAsync(loader, sector, scale);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded layers");

            // Load object manager
            Unity_ObjectManager_PS1Klonoa objManager = await Load_ObjManagerAsync(loader);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded object manager");

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            List<Unity_Object> objects = Load_Objects(loader, sector, scale, objManager);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded objects");

            Controller.DetailedState = "Loading paths";
            await Controller.WaitIfNecessary();

            var paths = Load_MovementPaths(loader, sector, scale);

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded paths");

            Unity_CameraClear camClear = null;
            var bgClear = loader.BackgroundPack?.BackgroundModifiersFiles.Files[sector].Modifiers.Where(x => x.Type == BackgroundModifierObject.BackgroundModifierType.Clear).Select(x => x.Data_20).ToArray();

            // TODO: Fully support camera clearing with gradient and multiple clear zones
            if (bgClear?.Any() == true)
                camClear = new Unity_CameraClear(bgClear.First().Entries[0].Color.GetColor());

            startupLog.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded camera clears");


            var str = new StringBuilder();

            foreach (var archive in ArchiveFile.ParsedArchiveFiles)
            {
                for (int i = 0; i < archive.Value.Length; i++)
                {
                    if (!archive.Value[i])
                        str.AppendLine($"{archive.Key.Offset}: ({archive.Key.GetType().Name}) File #{i}");
                }
            }

            Debug.Log($"Unparsed BIN files:{Environment.NewLine}" +
                      $"{str}");

            stopWatch.Stop();

            Debug.Log($"Startup in {stopWatch.ElapsedMilliseconds:0000}ms{Environment.NewLine}" +
                          $"{startupLog}");

            return new Unity_Level(
                layers: layers,
                cellSize: 16,
                objManager: objManager,
                eventData: objects,
                framesPerSecond: 30,
                collisionLines: paths,
                isometricData: new Unity_IsometricData
                {
                    CollisionWidth = 0,
                    CollisionHeight = 0,
                    TilesWidth = 0,
                    TilesHeight = 0,
                    Collision = null,
                    Scale = Vector3.one,
                    ViewAngle = Quaternion.Euler(90, 0, 0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * 1
                },
                ps1Vram: loader.VRAM,
                cameraClear: camClear);
        }

        public async UniTask<Unity_Layer[]> Load_LayersAsync(Loader loader, int sector, float scale)
        {
            var modifiers = loader.LevelData3D.SectorModifiers[sector].Modifiers;
            var texAnimations = modifiers.
                Where(x => x.PrimaryType == PrimaryObjectType.Modifier_3D_41).
                SelectMany(x => x.DataFiles).
                Where(x => x.TextureAnimation != null).
                Select(x => x.TextureAnimation.Files).
                ToArray();
            var uvScrollAnimations = modifiers.
                Where(x => x.PrimaryType == PrimaryObjectType.Modifier_3D_41).
                SelectMany(x => x.DataFiles).
                Where(x => x.UVScrollAnimation != null).
                SelectMany(x => x.UVScrollAnimation.UVOffsets).
                ToArray();

            var layers = new List<Unity_Layer>();

            Controller.DetailedState = "Loading backgrounds";
            await Controller.WaitIfNecessary();

            // Load backgrounds
            if (loader.BackgroundPack != null)
            {
                var bgModifiers = loader.BackgroundPack.BackgroundModifiersFiles.Files[sector].Modifiers;

                Texture2D[][] bgTextures;
                int bgAnimSpeed;

                // Get the background textures
                (bgTextures, bgAnimSpeed) = GetBackgrounds(loader, loader.BackgroundPack, bgModifiers);

                // Add the backgrounds
                layers.AddRange(bgTextures.Select((t, i) => new Unity_Layer_Texture
                {
                    Name = $"Background {i}", 
                    ShortName = $"BG{i}", 
                    Textures = t, 
                    AnimSpeed = bgAnimSpeed,
                }));
            }

            Controller.DetailedState = "Loading level geometry";
            await Controller.WaitIfNecessary();

            GameObject obj;
            bool isAnimated;

            (obj, isAnimated) = CreateGameObject(loader.LevelPack.Sectors[sector].LevelModel, loader, scale, "Map", texAnimations, uvScrollAnimations);

            var levelBounds = GetDimensions(loader.LevelPack.Sectors[sector].LevelModel, scale);

            // Calculate actual level dimensions: switched axes for unity & multiplied by cellSize
            var size = levelBounds.size;
            var cellSize = 16;
            var layerDimensions = new Vector3(size.x, size.z, size.y) * cellSize;

            // Correctly center object
            //obj.transform.position = new Vector3(-levelBounds.min.x, 0, -size.z-levelBounds.min.z);

            var parent3d = Controller.obj.levelController.editor.layerTiles.transform;
            layers.Add(new Unity_Layer_GameObject(true, isAnimated: isAnimated)
            {
                Name = "Map",
                ShortName = "MAP",
                Graphics = obj,
                Collision = null,
                Dimensions = layerDimensions,
                DisableGraphicsWhenCollisionIsActive = true
            });
            obj.transform.SetParent(parent3d);

            Controller.DetailedState = "Loading 3D objects";
            await Controller.WaitIfNecessary();

            GameObject gao_3dObjParent = null;

            bool isObjAnimated = false;

            foreach (var modifier in modifiers)
            {
                if (modifier.PrimaryType == PrimaryObjectType.None || modifier.PrimaryType == PrimaryObjectType.Invalid)
                    continue;

                if (modifier.PrimaryType == PrimaryObjectType.Modifier_3D_40 || modifier.PrimaryType == PrimaryObjectType.Modifier_3D_41)
                {
                    bool animated;

                    (gao_3dObjParent, animated) = Load_ModifierObject(loader, modifier, gao_3dObjParent, scale, texAnimations);

                    if (animated)
                        isObjAnimated = true;
                }
                else
                {
                    Debug.LogWarning($"Skipped unsupported modifier object of primary type {modifier.PrimaryType}");
                }
            }

            Debug.Log($"MAP INFO{Environment.NewLine}" +
                      $"{texAnimations.Length} texture animations{Environment.NewLine}" +
                      $"{uvScrollAnimations.Length} UV scroll animations{Environment.NewLine}" +
                      $"Modifiers:{Environment.NewLine}\t" +
                      $"{String.Join($"{Environment.NewLine}\t", modifiers.Take(modifiers.Length - 1).Select(x => $"{x.Offset}: {(int)x.PrimaryType:00}-{x.SecondaryType:00} {String.Join(", ", x.DataFiles?.Select(d => d.DeterminedType.ToString()) ?? new string[0])}"))}");

            if (gao_3dObjParent != null)
            {
                layers.Add(new Unity_Layer_GameObject(true, isAnimated: isObjAnimated)
                {
                    Name = "3D Objects",
                    ShortName = $"3DO",
                    Graphics = gao_3dObjParent
                });
                gao_3dObjParent.transform.SetParent(parent3d);
            }

            Controller.DetailedState = "Loading collision";
            await Controller.WaitIfNecessary();

            // TODO: Load collision

            return layers.ToArray();
        }

        public (GameObject, bool) Load_ModifierObject(Loader loader, ModifierObject modifier, GameObject gao_3dObjParent, float scale, PS1_TIM[][] texAnimations)
        {
            bool isObjAnimated = false;

            // Some objects only have a single position without a rotation
            var pos = modifier.DataFiles.FirstOrDefault(x => x.Position != null)?.Position;
            
            // Objects can have transform info. This is 2-dimensional as it can effect individual objects in the TMD and have multiple "frames"
            // obj positions/rotations. We assume that if it effects multiple objects it does not animate and is only for their relative positions.
            var objTransform = modifier.DataFiles.FirstOrDefault(x => x.Transform?.Info?.ObjectsCount > 1)?.Transform;

            var transform = modifier.DataFiles.FirstOrDefault(x => x.Transform != null && !(x.Transform?.Info?.ObjectsCount > 1))?.Transform;

            var transformPositions = transform?.Positions.Positions;
            var transformRotations = transform?.Rotations.Rotations;

            // The minecart has multiple transforms with animations for how it travels, so we combine them
            if (transform == null)
            {
                var transforms = modifier.DataFiles.FirstOrDefault(x => x.Transforms != null)?.Transforms;

                transformPositions = transforms?.Files.SelectMany(x => x.Positions.Positions).ToArray();
                transformRotations = transforms?.Files.SelectMany(x => x.Rotations.Rotations).ToArray();
            }

            var tmdFiles = modifier.DataFiles.Where(x => x.TMD != null).Select(x => x.TMD);

            var constantRotation = modifier.PrimaryType == PrimaryObjectType.Modifier_3D_41
                ? ModifierObjectsRotations.TryGetItem(loader.BINBlock)?.TryGetItem(modifier.SecondaryType)
                : null;
            var posOffset = modifier.PrimaryType == PrimaryObjectType.Modifier_3D_41
                ? ModifierObjectsPositionOffsets.TryGetItem(loader.BINBlock)?.TryGetItem(modifier.SecondaryType)
                : null;

            var index = 0;

            foreach (var tmd in tmdFiles)
            {
                if (gao_3dObjParent == null)
                {
                    gao_3dObjParent = new GameObject("3D Objects");
                    gao_3dObjParent.transform.localPosition = Vector3.zero;
                    gao_3dObjParent.transform.localRotation = Quaternion.identity;
                    gao_3dObjParent.transform.localScale = Vector3.one;
                }

                GameObject gameObj;
                bool isAnimated;

                (gameObj, isAnimated) = CreateGameObject(
                    tmd: tmd,
                    loader: loader,
                    scale: scale,
                    name: $"Object3D Offset:{modifier.Offset} Index:{index} Type:{modifier.PrimaryType}-{modifier.SecondaryType}",
                    texAnimations: texAnimations,
                    scrollUVs: new int[0],
                    positions: objTransform?.Positions.Positions[0].Select(x => GetPositionVector(x, Vector3.zero, scale)).ToArray(),
                    rotations: objTransform?.Rotations.Rotations[0].Select(x => GetQuaternion(x)).ToArray());

                if (isAnimated)
                    isObjAnimated = true;

                Vector3 defaultPos = Vector3.zero;
                Quaternion defaultRotation = Quaternion.identity;

                if (pos != null)
                {
                    defaultPos = GetPositionVector(pos, posOffset, scale);
                }
                else if (transformPositions != null)
                {
                    defaultPos = GetPositionVector(transformPositions[0][0], posOffset, scale);
                    defaultRotation = GetQuaternion(transformRotations[0][0]);
                }

                gameObj.transform.localPosition = defaultPos;
                gameObj.transform.localRotation = defaultRotation;
                gameObj.transform.SetParent(gao_3dObjParent.transform);

                if (transformPositions?.Length > 1)
                {
                    var mtComponent = gameObj.AddComponent<AnimatedTransformComponent>();
                    mtComponent.animatedTransform = gameObj.transform;
                    var positions = transformPositions?.Select(x => GetPositionVector(x[0], posOffset, scale)).ToArray() ?? new Vector3[0];
                    var rotations = transformRotations?.Select(x => GetQuaternion(x[0])).ToArray() ?? new Quaternion[0];
                    var frameCount = Math.Max(positions.Length, rotations.Length);
                    mtComponent.frames = new AnimatedTransformComponent.Frame[frameCount];
                    for (int i = 0; i < frameCount; i++)
                    {
                        mtComponent.frames[i] = new AnimatedTransformComponent.Frame()
                        {
                            Position = positions.Length > i ? positions[i] : defaultPos,
                            Rotation = rotations.Length > i ? rotations[i] : defaultRotation,
                            Scale = Vector3.one
                        };
                    }
                }

                if (constantRotation != null)
                {
                    var mtComponent = gameObj.AddComponent<AnimatedTransformComponent>();
                    mtComponent.animatedTransform = gameObj.transform;

                    var rotationPerFrame = constantRotation.Value.Item2;
                    var count = 0x1000 / rotationPerFrame;

                    mtComponent.frames = new AnimatedTransformComponent.Frame[count];

                    for (int i = 0; i < count; i++)
                    {
                        var degrees = GetRotationInDegrees(i * rotationPerFrame);

                        mtComponent.frames[i] = new AnimatedTransformComponent.Frame()
                        {
                            Position = defaultPos,
                            Rotation = defaultRotation * Quaternion.Euler(
                                x: degrees * constantRotation.Value.Item1.x,
                                y: degrees * constantRotation.Value.Item1.y,
                                z: degrees * constantRotation.Value.Item1.z),
                            Scale = Vector3.one
                        };
                    }
                }

                index++;
            }

            return (gao_3dObjParent, isObjAnimated);
        }

        public async UniTask<Unity_ObjectManager_PS1Klonoa> Load_ObjManagerAsync(Loader loader)
        {
            var frameSets = new List<Unity_ObjectManager_PS1Klonoa.SpriteSet>();

            // Enumerate each frame set
            for (var i = 0; i < loader.SpriteFrames.Length; i++)
            {
                Controller.DetailedState = $"Loading sprites {i + 1}/{loader.SpriteFrames.Length}";
                await Controller.WaitIfNecessary();

                var frames = loader.SpriteFrames[i];

                // Skip if null
                if (frames == null)
                    continue;

                // Create the frame textures
                var frameTextures = frames.Files.Take(frames.Files.Length - 1).Select(x => GetTexture(x.Textures, loader.VRAM, 0, 500).CreateSprite()).ToArray();

                frameSets.Add(new Unity_ObjectManager_PS1Klonoa.SpriteSet(frameTextures, i));
            }

            return new Unity_ObjectManager_PS1Klonoa(loader.Context, frameSets.ToArray());
        }

        public List<Unity_Object> Load_Objects(Loader loader, int sector, float scale, Unity_ObjectManager_PS1Klonoa objManager)
        {
            var objects = new List<Unity_Object>();
            var movementPaths = loader.LevelPack.Sectors[sector].MovementPaths.Files;

            // Add enemies
            foreach (EnemyObject enemyObj in loader.LevelData2D.EnemyObjects.Where(x => x.GlobalSectorIndex == loader.GlobalSectorIndex))
            {
                // Add the enemy object
                addEnemyObj(enemyObj);

                // Add spawned objects from portals
                if (enemyObj.Data is EnemyData_02 data_02)
                {
                    foreach (var spawnObject in data_02.SpawnObjects)
                        addEnemyObj(spawnObject);
                }

                void addEnemyObj(EnemyObject obj)
                {
                    var spriteInfo = GetSprite_Enemy(obj);

                    if (spriteInfo.SpriteSet == -1 || spriteInfo.SpriteIndex == -1)
                        Debug.LogWarning($"Sprite could not be determined for enemy object of secondary type {obj.SecondaryType} and graphics index {obj.GraphicsIndex}");

                    objects.Add(new Unity_Object_PS1Klonoa_Enemy(objManager, obj, scale, spriteInfo));
                }
            }

            // Add enemy spawn points
            for (int pathIndex = 0; pathIndex < movementPaths.Length; pathIndex++)
            {
                for (int objIndex = 0; objIndex < loader.LevelData2D.EnemyObjectIndexTables.IndexTables[pathIndex].Length; objIndex++)
                {
                    var obj = loader.LevelData2D.EnemyObjects[loader.LevelData2D.EnemyObjectIndexTables.IndexTables[pathIndex][objIndex]];

                    if (obj.GlobalSectorIndex != loader.GlobalSectorIndex)
                        continue;

                    var pos = GetPosition(movementPaths[pathIndex].Blocks, obj.MovementPathSpawnPosition, Vector3.zero, scale);

                    objects.Add(new Unity_Object_Dummy(obj, Unity_Object.ObjectType.Trigger, objLinks: new int[]
                    {
                        objects.OfType<Unity_Object_PS1Klonoa_Enemy>().FindItemIndex(x => x.Object == obj)
                    })
                    {
                        Position = pos,
                    });
                }
            }

            // Add collectibles
            objects.AddRange(loader.LevelData2D.CollectibleObjects.Where(x => x.GlobalSectorIndex == loader.GlobalSectorIndex && x.SecondaryType != -1).Select(x =>
            {
                Vector3 pos;

                // If the path index is -1 then the position is absolute, otherwise it's relative
                if (x.MovementPath == -1)
                    pos = GetPosition(x.XPos.Value, x.YPos.Value, x.ZPos.Value, scale);
                else
                    pos = GetPosition(movementPaths[x.MovementPath].Blocks, x.MovementPathPosition, new Vector3(0, x.YPos.Value, 0), scale);

                var spriteInfo = GetSprite_Collectible(x);

                if (spriteInfo.SpriteSet == -1 || spriteInfo.SpriteIndex == -1)
                    Debug.LogWarning($"Sprite could not be determined for collectible object of secondary type {x.SecondaryType}");

                return new Unity_Object_PS1Klonoa_Collectible(objManager, x, pos, spriteInfo);
            }));

            // Add scenery objects
            objects.AddRange(loader.LevelData3D.SectorModifiers[sector].Modifiers.Where(x => x.DataFiles != null).SelectMany(x => x.DataFiles).Where(x => x.ScenerySprites != null).SelectMany(x => x.ScenerySprites.Positions).Select(x => new Unity_Object_Dummy(x, Unity_Object.ObjectType.Object)
            {
                Position = GetPosition(x.XPos, x.YPos, x.ZPos, scale),
            }));

            var wpIndex = objects.Count;
            // Temporarily add waypoints at each path block to visualize them
            objects.AddRange(movementPaths.SelectMany((x, i) => x.Blocks.SelectMany(b => new Unity_Object[]
            {
                new Unity_Object_Dummy(b, Unity_Object.ObjectType.Waypoint, $"Path: {i}", objLinks: new int[]
                {
                    ++wpIndex
                })
                {
                    Position = GetPosition(b.XPos, b.YPos, b.ZPos, scale),
                },
                new Unity_Object_Dummy(b, Unity_Object.ObjectType.Waypoint, $"Path: {i}", objLinks: new []
                {
                    (wpIndex++) - 1
                })
                {
                    Position = GetPosition(
                        x: b.XPos + b.DirectionX * b.BlockLength,
                        y: b.YPos + b.DirectionY * b.BlockLength,
                        z: b.ZPos + b.DirectionZ * b.BlockLength,
                        scale: scale),
                }
            })));

            return objects;
        }

        public Unity_CollisionLine[] Load_MovementPaths(Loader loader, int sector, float scale)
        {
            var lines = new List<Unity_CollisionLine>();

            foreach (var path in loader.LevelPack.Sectors[sector].MovementPaths.Files)
            {
                foreach (var pathBlock in path.Blocks)
                {
                    var origin = GetPosition(pathBlock.XPos, pathBlock.YPos, pathBlock.ZPos, scale);
                    var end = GetPosition(
                        x: pathBlock.XPos + pathBlock.DirectionX * pathBlock.BlockLength, 
                        y: pathBlock.YPos + pathBlock.DirectionY * pathBlock.BlockLength, 
                        z: pathBlock.ZPos + pathBlock.DirectionZ * pathBlock.BlockLength, 
                        scale: scale);

                    lines.Add(new Unity_CollisionLine(origin, end));
                }
            }

            return lines.ToArray();
        }

        public (GameObject, bool) CreateGameObject(PS1_TMD tmd, Loader loader, float scale, string name, PS1_TIM[][] texAnimations, int[] scrollUVs, Vector3[] positions = null, Quaternion[] rotations = null)
        {
            bool isAnimated = false;
            var textureCache = new Dictionary<int, Texture2D>();
            var textureAnimCache = new Dictionary<long, Texture2D[]>();

            GameObject gaoParent = new GameObject(name);
            gaoParent.transform.position = Vector3.zero;

            // Create each object
            for (var objIndex = 0; objIndex < tmd.Objects.Length; objIndex++)
            {
                var obj = tmd.Objects[objIndex];

                // Helper methods
                Vector3 toVertex(PS1_TMD_Vertex v) => new Vector3(v.X / scale, -v.Y / scale, v.Z / scale);
                Vector3 toNormal(PS1_TMD_Normal n) => new Vector3(n.X, -n.Y , n.Z);
                Vector2 toUV(PS1_TMD_UV uv) => new Vector2(uv.U / 255f, uv.V / 255f);

                RectInt getRect(PS1_TMD_UV[] uv)
                {
                    int xMin = uv.Min(x => x.U);
                    int xMax = uv.Max(x => x.U) + 1;
                    int yMin = uv.Min(x => x.V);
                    int yMax = uv.Max(x => x.V) + 1;
                    int w = xMax - xMin;
                    int h = yMax - yMin;

                    return new RectInt(xMin, yMin, w, h);
                }

                GameObject gameObject = new GameObject($"Object_{objIndex} Offset:{obj.Offset}");

                gameObject.transform.SetParent(gaoParent.transform);
                gameObject.transform.localScale = Vector3.one;

                gameObject.transform.localPosition = positions?[objIndex] ?? Vector3.zero;
                gameObject.transform.rotation = rotations?[objIndex] ?? Quaternion.identity;

                // Add each primitive
                for (var packetIndex = 0; packetIndex < obj.Primitives.Length; packetIndex++)
                {
                    var packet = obj.Primitives[packetIndex];

                    //if (!packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.LGT))
                    //    Debug.LogWarning($"Packet has light source");

                    // TODO: Implement other types
                    if (packet.Mode.Code != PS1_TMD_PacketMode.PacketModeCODE.Polygon)
                    {
                        Debug.LogWarning($"Skipped packet with code {packet.Mode.Code}");
                        continue;
                    }

                    Mesh unityMesh = new Mesh();

                    var vertices = packet.Vertices.Select(x => toVertex(obj.Vertices[x])).ToArray();

                    Vector3[] normals = null;

                    if (packet.Normals != null) 
                    {
                        normals = packet.Normals.Select(x => toNormal(obj.Normals[x])).ToArray();
                        if(normals.Length == 1)
                            normals = Enumerable.Repeat(normals[0], vertices.Length).ToArray();
                    }
                    int[] triangles;

                    // Set vertices
                    unityMesh.SetVertices(vertices);

                    // Set normals
                    if (normals != null) 
                        unityMesh.SetNormals(normals);

                    if (packet.Mode.IsQuad)
                    {
                        if (packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.FCE))
                        {
                            triangles = new int[]
                            {
                                // Lower left triangle
                                0, 1, 2, 0, 2, 1,
                                // Upper right triangle
                                3, 2, 1, 3, 1, 2,
                            };
                        }
                        else
                        {
                            triangles = new int[]
                            {
                                // Lower left triangle
                                0, 1, 2,
                                // Upper right triangle
                                3, 2, 1,
                            };
                        }
                    }
                    else
                    {
                        if (packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.FCE))
                        {
                            triangles = new int[]
                            {
                                0, 1, 2, 0, 2, 1,
                            };
                        }
                        else
                        {
                            triangles = new int[]
                            {
                                0, 1, 2,
                            };
                        }
                    }

                    unityMesh.SetTriangles(triangles, 0);

                    var colors = packet.RGB.Select(x => x.Color.GetColor()).ToArray();

                    if (colors.Length == 1)
                        colors = Enumerable.Repeat(colors[0], vertices.Length).ToArray();

                    unityMesh.SetColors(colors);

                    if (packet.UV != null)
                        unityMesh.SetUVs(0, packet.UV.Select(toUV).ToArray());

                    if(normals == null) unityMesh.RecalculateNormals();

                    GameObject gao = new GameObject($"Packet_{packetIndex} Offset:{packet.Offset} Flags:{packet.Flags}");

                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                    gao.layer = LayerMask.NameToLayer("3D Collision");
                    gao.transform.SetParent(gameObject.transform);
                    gao.transform.localScale = Vector3.one;
                    gao.transform.localPosition = Vector3.zero;
                    mf.mesh = unityMesh;

                    if (packet.Mode.ABE)
                        mr.material = Controller.obj.levelController.controllerTilemap.unlitAdditiveMaterial;
                    else
                        mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial;

                    // Add texture
                    if (packet.Mode.TME)
                    {
                        var rect = getRect(packet.UV);

                        var key = packet.CBA.ClutX | packet.CBA.ClutY << 6 | packet.TSB.TX << 16 | packet.TSB.TY << 24;
                        long animKey = (long) key | (long) rect.x << 32 | (long) rect.y << 40;

                        if (!textureAnimCache.ContainsKey(animKey))
                        {
                            PS1_TIM[] foundAnim = null;

                            // Check if the texture region falls within an animated area
                            foreach (var anim in texAnimations)
                            {
                                foreach (var tim in anim)
                                {
                                    // Check the page
                                    if ((tim.XPos * 2) / PS1_VRAM.PageWidth == packet.TSB.TX &&
                                        tim.YPos / PS1_VRAM.PageHeight == packet.TSB.TY)
                                    {
                                        var is4Bit = tim.ColorFormat == PS1_TIM.TIM_ColorFormat.BPP_4;

                                        var timRect = new RectInt(
                                            xMin: ((tim.XPos * 2) % PS1_VRAM.PageWidth) * (is4Bit ? 2 : 1),
                                            yMin: (tim.YPos % PS1_VRAM.PageHeight),
                                            width: tim.Width * 2 * (is4Bit ? 2 : 1),
                                            height: tim.Height);

                                        // Check page offset
                                        if (rect.Overlaps(timRect))
                                        {
                                            foundAnim = anim;
                                            break;
                                        }
                                    }
                                }

                                if (foundAnim != null)
                                    break;
                            }

                            if (foundAnim != null)
                            {
                                var textures = new Texture2D[foundAnim.Length];

                                for (int i = 0; i < textures.Length; i++)
                                {
                                    loader.AddToVRAM(foundAnim[i]);
                                    textures[i] = GetTexture(packet, loader.VRAM);
                                }

                                textureAnimCache.Add(animKey, textures);
                            }
                            else
                            {
                                textureAnimCache.Add(animKey, null);
                            }
                        }

                        if (!textureCache.ContainsKey(key))
                            textureCache.Add(key, GetTexture(packet, loader.VRAM));

                        var t = textureCache[key];

                        var animTextures = textureAnimCache[animKey];

                        if (animTextures != null)
                        {
                            isAnimated = true;
                            var animTex = gao.AddComponent<AnimatedTextureComponent>();
                            animTex.material = mr.material;
                            animTex.animatedTextureSpeed = 2; // TODO: Is this correct?
                            animTex.animatedTextures = animTextures;
                        }

                        t.wrapMode = TextureWrapMode.Repeat;
                        mr.material.SetTexture("_MainTex", t);
                        mr.name = $"{objIndex}-{packetIndex} TX: {packet.TSB.TX}, TY:{packet.TSB.TY}, X:{rect.x}, Y:{rect.y}, W:{rect.width}, H:{rect.height}, F:{packet.Flags}, ABE:{packet.Mode.ABE}, TGE:{packet.Mode.TGE}, UVOffset:{packet.UV.First().Offset.FileOffset - tmd.Offset.FileOffset}";

                        // Check for UV scroll animations
                        if (packet.UV.Any(x => scrollUVs.Contains((int)(x.Offset.FileOffset - tmd.Offset.FileOffset))))
                        {
                            isAnimated = true;
                            var animTex = gao.AddComponent<AnimatedTextureComponent>();
                            animTex.material = mr.material;
                            animTex.scrollV = -0.5f;
                        }
                    }
                }
            }

            return (gaoParent, isAnimated);
        }

        public Bounds GetDimensions(PS1_TMD tmd, float scale) {
            Bounds b = new Bounds();
            var verts = tmd.Objects.SelectMany(x => x.Vertices).ToArray();
            var min = new Vector3(verts.Min(v => v.X), verts.Min(v => v.Y), verts.Min(v => v.Z)) / scale;
            var max = new Vector3(verts.Max(v => v.X), verts.Max(v => v.Y), verts.Max(v => v.Z)) / scale;
            var center = Vector3.Lerp(min, max, 0.5f);

            return new Bounds(center, max-min);
        }

        Vector3 GetPositionVector(ObjPosition pos, Vector3? posOffset, float scale)
        {
            if (posOffset == null)
                return new Vector3(pos.XPos / scale, -pos.YPos / scale, pos.ZPos / scale);
            else
                return new Vector3((pos.XPos + posOffset.Value.x) / scale, -(pos.YPos + posOffset.Value.y) / scale, (pos.ZPos + posOffset.Value.z) / scale);
        }

        public float GetRotationInDegrees(int value)
        {
            if (value > 0x800)
                value -= 0x1000;

            return value * (360f / 0x1000);
        }

        public Quaternion GetQuaternion(ObjRotation rot)
        {
            return GetQuaternion(rot.RotationX, rot.RotationY, rot.RotationZ);
        }

        public Quaternion GetQuaternion(int rotX, int rotY, int rotZ)
        {
            return Quaternion.Euler(
                x: -GetRotationInDegrees(rotX),
                y: GetRotationInDegrees(rotY),
                z: -(GetRotationInDegrees(rotZ) - GetRotationInDegrees(rotX)));
        }

        public IDX Load_IDX(Context context)
        {
            return FileFactory.Read<IDX>(Loader.FilePath_IDX, context);
        }

        public void FillTextureFromVRAM(
            Texture2D tex,
            PS1_VRAM vram,
            int width, int height,
            PS1_TIM.TIM_ColorFormat colorFormat,
            int texX, int texY,
            int clutX, int clutY,
            int texturePageOriginX, int texturePageOriginY,
            int texturePageOffsetX, int texturePageOffsetY,
            int texturePageX = 0, int texturePageY = 0,
            bool flipX = false, bool flipY = false,
            bool useDummyPal = false)
        {
            var dummyPal = useDummyPal ? Util.CreateDummyPalette(colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8 ? 256 : 16) : null;

            texturePageOriginX *= 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte paletteIndex;

                    if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + texturePageOffsetX + x,
                            texturePageOriginY + texturePageOffsetY + y);
                    }
                    else if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_4)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + (texturePageOffsetX + x) / 2,
                            texturePageOriginY + texturePageOffsetY + y);

                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);
                    }
                    else
                    {
                        throw new Exception($"Non-supported color format");
                    }

                    // Get the color from the palette
                    var c = useDummyPal ? dummyPal[paletteIndex] : vram.GetColor1555(0, 0, clutX + paletteIndex, clutY);

                    if (c.Alpha == 0)
                        continue;

                    var texOffsetX = flipX ? width - x - 1 : x;
                    var texOffsetY = flipY ? height - y - 1 : y;

                    // Set the pixel
                    tex.SetPixel(texX + texOffsetX, tex.height - (texY + texOffsetY) - 1, c.GetColor());
                }
            }
        }

        public Texture2D GetTexture(PS1_TIM tim, bool flipTextureY = true, Color[] palette = null, bool onlyFirstTransparent = false)
        {
            if (tim.XPos == 0 && tim.YPos == 0)
                return null;

            var pal = palette ?? tim.Clut?.Palette?.Select(x => x.GetColor()).ToArray();

            if (onlyFirstTransparent && pal != null)
                for (int i = 0; i < pal.Length; i++)
                    pal[i].a = i == 0 ? 0 : 1;

            return GetTexture(tim.ImgData, pal, tim.Width, tim.Height, tim.ColorFormat, flipTextureY);
        }

        public Texture2D GetTexture(PS1_TMD_Packet packet, PS1_VRAM vram)
        {
            if (!packet.Mode.TME)
                throw new Exception($"Packet has no texture");

            PS1_TIM.TIM_ColorFormat colFormat = packet.TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => PS1_TIM.TIM_ColorFormat.BPP_4,
                PS1_TSB.TexturePageTP.CLUT_8Bit => PS1_TIM.TIM_ColorFormat.BPP_8,
                PS1_TSB.TexturePageTP.Direct_15Bit => PS1_TIM.TIM_ColorFormat.BPP_16,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {packet.TSB.TP}")
            };
            int width = packet.TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => 256,
                PS1_TSB.TexturePageTP.CLUT_8Bit => 128,
                PS1_TSB.TexturePageTP.Direct_15Bit => 64,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {packet.TSB.TP}")
            };

            var tex = TextureHelpers.CreateTexture2D(width, 256, clear: true);

            FillTextureFromVRAM(
                tex: tex,
                vram: vram,
                width: width,
                height: 256,
                colorFormat: colFormat,
                texX: 0,
                texY: 0,
                clutX: packet.CBA.ClutX * 16,
                clutY: packet.CBA.ClutY,
                texturePageX: packet.TSB.TX,
                texturePageY: packet.TSB.TY,
                texturePageOriginX: 0,
                texturePageOriginY: 0,
                texturePageOffsetX: 0,
                texturePageOffsetY: 0,
                flipY: true);

            tex.Apply();

            return tex;
        }

        public Texture2D GetTexture(byte[] imgData, Color[] pal, int width, int height, PS1_TIM.TIM_ColorFormat colorFormat, bool flipTextureY = true)
        {
            Util.TileEncoding encoding;

            int palLength;

            switch (colorFormat)
            {
                case PS1_TIM.TIM_ColorFormat.BPP_4:
                    width *= 2 * 2;
                    encoding = Util.TileEncoding.Linear_4bpp;
                    palLength = 16;
                    break;

                case PS1_TIM.TIM_ColorFormat.BPP_8:
                    width *= 2;
                    encoding = Util.TileEncoding.Linear_8bpp;
                    palLength = 256;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            pal ??= Util.CreateDummyPalette(palLength).Select(x => x.GetColor()).ToArray();

            var tex = TextureHelpers.CreateTexture2D(width, height);

            tex.FillRegion(
                imgData: imgData,
                imgDataOffset: 0,
                pal: pal,
                encoding: encoding,
                regionX: 0,
                regionY: 0,
                regionWidth: tex.width,
                regionHeight: tex.height,
                flipTextureY: flipTextureY);

            return tex;
        }

        public Texture2D GetTexture(SpriteTexture[] sprites, PS1_VRAM vram, int palX, int palY)
        {
            var rects = sprites.Select(s => 
                new RectInt(
                    xMin: s.FlipX ? s.XPos - s.Width - 1 : s.XPos, 
                    yMin: s.FlipY ? s.YPos - s.Height - 1 : s.YPos, 
                    width: s.Width, 
                    height: s.Height)).ToArray();

            var minX = rects.Min(x => x.x);
            var minY = rects.Min(x => x.y);
            var maxX = rects.Max(x => x.x + x.width);
            var maxY = rects.Max(x => x.y + x.height);

            var width = maxX - minX;
            var height = maxY - minY;

            var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

            for (var i = 0; i < sprites.Length; i++)
            {
                var index = sprites.Length - i - 1;

                var sprite = sprites[index];
                var texPage = sprite.TexturePage;

                try
                {
                    FillTextureFromVRAM(
                        tex: tex,
                        vram: vram,
                        width: sprite.Width,
                        height: sprite.Height,
                        colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4,
                        texX: rects[index].x - minX,
                        texY: rects[index].y - minY,
                        clutX: palX + sprite.PalOffsetX,
                        clutY: palY + sprite.PalOffsetY,
                        texturePageOriginX: 0,
                        texturePageOriginY: 0,
                        texturePageX: texPage % 16,
                        texturePageY: texPage / 16,
                        texturePageOffsetX: sprite.TexturePageOffsetX,
                        texturePageOffsetY: sprite.TexturePageOffsetY,
                        flipX: sprite.FlipX,
                        flipY: sprite.FlipY,
                        useDummyPal: palX == -1 || palY == -1);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error filling sprite texture data: {ex.Message}");
                }
            }

            tex.Apply();

            return tex;
        }

        public (Texture2D[][], int) GetBackgrounds(Loader loader, BackgroundPack_ArchiveFile bg, BackgroundModifierObject[] modifiers)
        {
            // Get the modifiers
            var layers = modifiers.Where(x => x.IsLayer).ToArray();
            var palScrolls = modifiers.Where(x => x.Type == BackgroundModifierObject.BackgroundModifierType.PaletteScroll).ToArray();

            // Get the amount of frames each animation will have. We assume if there are multiple palette scroll modifiers that they all share
            // the same length and speed (which they do in Klonoa)
            var framesLength = palScrolls.Any() ? palScrolls.First().Data_23.Length : 1;
            var animSpeed = palScrolls.Any() ? palScrolls.First().Data_23.Speed : 0;

            // Create the textures array for the backgrounds and their frames
            var textures = new Texture2D[layers.Length][];

            for (int i = 0; i < textures.Length; i++)
                textures[i] = new Texture2D[framesLength];

            // Load VRAM data
            for (int tileSetIndex = 0; tileSetIndex < bg.TIMFiles.Files.Length; tileSetIndex++)
            {
                var tim = bg.TIMFiles.Files[tileSetIndex];

                // The game hard-codes this
                if (tileSetIndex == 0)
                {
                    tim.Clut.XPos = 0x130;
                    tim.Clut.YPos = 0x1F0;
                    tim.Clut.Width = 0x10;
                    tim.Clut.Height = 0x10;
                }
                else if (tileSetIndex == 1)
                {
                    tim.XPos = 0x1C0;
                    tim.YPos = 0x100;
                    tim.Width = 0x40;
                    tim.Height = 0x100;

                    tim.Clut.XPos = 0x120;
                    tim.Clut.YPos = 0x1F0;
                    tim.Clut.Width = 0x10;
                    tim.Clut.Height = 0x10;
                }

                loader.AddToVRAM(tim);
            }

            // Create the background frames
            for (int frameIndex = 0; frameIndex < framesLength; frameIndex++)
            {
                // Update VRAM with each palette scroll modifier
                if (frameIndex > 0)
                {
                    foreach (var palMod in palScrolls.Select(x => x.Data_23))
                    {
                        var pal = loader.VRAM.GetPixels8(0, 0, palMod.XPosition * 2, palMod.YPosition, 32);

                        var firstColor_0 = pal[0];
                        var firstColor_1 = pal[1];
                        
                        var index = palMod.StartIndex;
                        var endIndex = index + palMod.Length;
                        
                        do
                        {
                            pal[index * 2] = pal[(index + 1) * 2];
                            pal[index * 2 + 1] = pal[(index + 1) * 2 + 1];

                            index += 1;
                        } while (index < endIndex);

                        pal[(endIndex - 1) * 2] = firstColor_0;
                        pal[(endIndex - 1) * 2 + 1] = firstColor_1;

                        loader.VRAM.AddDataAt(0, 0, palMod.XPosition * 2, palMod.YPosition, pal, 32, 1);
                    }
                }

                // Create the frame for each background
                for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
                {
                    var celIndex = layers[layerIndex].CELIndex;
                    var bgIndex = layers[layerIndex].BGDIndex;

                    var tim = bg.TIMFiles.Files[celIndex];
                    var cel = bg.CELFiles.Files[celIndex];
                    var map = bg.BGDFiles.Files[bgIndex];

                    var tex = TextureHelpers.CreateTexture2D(map.MapWidth * map.CellWidth, map.MapHeight * map.CellHeight, clear: true);

                    for (int mapY = 0; mapY < map.MapHeight; mapY++)
                    {
                        for (int mapX = 0; mapX < map.MapWidth; mapX++)
                        {
                            var cellIndex = map.Map[mapY * map.MapWidth + mapX];

                            if (cellIndex == 0xFF)
                                continue;

                            var cell = cel.Cells[cellIndex];

                            FillTextureFromVRAM(
                                tex: tex,
                                vram: loader.VRAM,
                                width: map.CellWidth,
                                height: map.CellHeight,
                                colorFormat: tim.ColorFormat,
                                texX: mapX * map.CellWidth,
                                texY: mapY * map.CellHeight,
                                clutX: cell.ClutX * 16,
                                clutY: cell.ClutY,
                                texturePageOriginX: tim.XPos,
                                texturePageOriginY: tim.YPos,
                                texturePageOffsetX: cell.XOffset,
                                texturePageOffsetY: cell.YOffset);
                        }
                    }

                    tex.Apply();

                    textures[layerIndex][frameIndex] = tex;
                }
            }

            return (textures, animSpeed);
        }
        
        public async UniTask LoadFilesAsync(Context context, LoaderConfiguration config)
        {
            // The game only loads portions of the BIN at a time
            await context.AddLinearSerializedFileAsync(Loader.FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(Loader.FilePath_IDX, 0x80010000);

            // The exe has to be loaded to read certain data from it
            await context.AddMemoryMappedFile(config.FilePath_EXE, config.Address_EXE, priority: 0); // Give lower prio to prioritize IDX
        }

        // [BINBlock][SecondaryType (for type 41)]
        // All speeds are estimations and may not be accurate
        public Dictionary<int, Dictionary<int, (Vector3Int, int)?>> ModifierObjectsRotations { get; } = new Dictionary<int, Dictionary<int, (Vector3Int, int)?>>()
        {
            [3] = new Dictionary<int, (Vector3Int, int)?>()
            {
                [1] = (new Vector3Int(0, 1, 0), 292), // Wind
                [5] = (new Vector3Int(0, 0, 1), 60), // Small windmill
                [6] = (new Vector3Int(0, 0, 1), 16), // Big windmill
            }
        };

        // [BINBlock][SecondaryType (for type 41)]
        // Some objects offset their positions
        public Dictionary<int, Dictionary<int, Vector3Int>> ModifierObjectsPositionOffsets { get; } = new Dictionary<int, Dictionary<int, Vector3Int>>()
        {
            [3] = new Dictionary<int, Vector3Int>()
            {
                [1] = new Vector3Int(0, 182, 0), // Wind
            }
        };

        public static ObjSpriteInfo GetSprite_Enemy(EnemyObject obj)
        {
            // TODO: Some enemies have palette swaps. This is done by modifying the x and y palette offsets (by default they are 0 and 500). For example the shielded Moo enemies in Vision 1-2

            // There are 42 object types (0-41). The graphics index is an index to an array of functions for displaying the graphics. The game
            // normally doesn't directly use the graphics index as it sometimes modifies it, but it appears the value initially set in the object
            // data will always match the correct sprite to show, so we can use that.
            var graphicsIndex = obj.GraphicsIndex;

            // Usually the graphics index matches the sprite set index (minus 1), but some are special cases, and since we don't want to show the
            // first sprite we hard-code this. Ideally we would animate them, but that is sadly entirely hard-coded :(
            return graphicsIndex switch
            {
                01 => new ObjSpriteInfo(0, 81), // Moo
                02 => new ObjSpriteInfo(1, 0),
                03 => new ObjSpriteInfo(2, 36), // Pinkie

                05 => new ObjSpriteInfo(4, 0), // Portal
                06 => new ObjSpriteInfo(5, 12),
                07 => new ObjSpriteInfo(6, 36), // Flying Moo
                08 => new ObjSpriteInfo(7, 16),
                09 => new ObjSpriteInfo(8, 4), // Spiker
                10 => new ObjSpriteInfo(9, 68),
                11 => new ObjSpriteInfo(10, 4),
                12 => new ObjSpriteInfo(11, 72),
                13 => new ObjSpriteInfo(12, 54),
                14 => new ObjSpriteInfo(13, 24),
                15 => new ObjSpriteInfo(14, 0), // Moo with shield
                16 => new ObjSpriteInfo(15, 154),
                17 => new ObjSpriteInfo(16, 0), // Moo with spiky shield
                18 => new ObjSpriteInfo(17, 0),
                19 => new ObjSpriteInfo(18, 8),
                20 => new ObjSpriteInfo(19, 28),
                21 => new ObjSpriteInfo(20, 0),

                23 => new ObjSpriteInfo(22, 44),
                24 => new ObjSpriteInfo(23, 76),
                25 => new ObjSpriteInfo(24, 0), // Big spiky ball
                26 => new ObjSpriteInfo(25, 36),
                
                28 => new ObjSpriteInfo(27, 118),
                29 => new ObjSpriteInfo(28, 165),
                30 => new ObjSpriteInfo(29, 41),
                31 => new ObjSpriteInfo(30, 157),
                32 => new ObjSpriteInfo(31, 16),

                35 => new ObjSpriteInfo(14, 0, scale: 2), // Big Moo
                36 => new ObjSpriteInfo(1, 0, scale: 2),
                37 => new ObjSpriteInfo(0, 81, scale: 2), // Big Moo

                39 => new ObjSpriteInfo(14, 0, scale: 2), // Big Moo with shield

                112 => new ObjSpriteInfo(11, 149),
                137 => new ObjSpriteInfo(11, 149, scale: 2),
                _ => new ObjSpriteInfo(-1, -1)
            };
        }

        public static ObjSpriteInfo GetSprite_Collectible(CollectibleObject obj)
        {
            switch (obj.SecondaryType)
            {
                // Switch
                case 1:
                    return new ObjSpriteInfo(68, 10);

                // Dream Stone
                case 2:
                    return obj.Ushort_14 == 0 ? new ObjSpriteInfo(68, 0) : new ObjSpriteInfo(68, 5);

                // Heart, life
                case 3:
                case 4:
                    return obj.Short_0E switch
                    {
                        3 => new ObjSpriteInfo(68, 30),
                        4 => new ObjSpriteInfo(68, 22),
                        15 => new ObjSpriteInfo(68, 57),
                        _ => new ObjSpriteInfo(-1, -1)
                    };

                // Bubble
                case 5:
                case 6:
                case 16:
                case 17:
                    return obj.Short_0E switch
                    {
                        5 => new ObjSpriteInfo(68, 42), // Checkpoint
                        9 => new ObjSpriteInfo(68, 43), // Item
                        13 => new ObjSpriteInfo(68, 44), // x2
                        _ => new ObjSpriteInfo(-1, -1)
                    };

                // Nagapoko Egg
                case 8:
                case 9:
                    return new ObjSpriteInfo(68, 76);

                // Bouncy spring
                case 10:
                    return new ObjSpriteInfo(21, 2);

                // Colored orb (Vision 5-1)
                case 15:
                    return new ObjSpriteInfo(68, 81 + (6 * (obj.Ushort_14 - 2)));

                default:
                    return new ObjSpriteInfo(-1, -1);
            }
        }

        public static Vector3 GetPosition(float x, float y, float z, float scale) => new Vector3(x / scale, -z / scale, -y / scale);

        public static Vector3 GetPosition(MovementPathBlock[] path, int position, Vector3 relativePos, float scale)
        {
            var blockIndex = 0;
            int blockPosOffset;

            if (position < 0)
            {
                blockIndex = 0;
                blockPosOffset = position;
            }
            else
            {
                var iVar6 = 0;

                do
                {
                    var iVar2 = path[blockIndex].BlockLength;

                    if (iVar2 == 0x7ffe)
                    {
                        blockIndex = 0;
                    }
                    else
                    {
                        if (iVar2 == 0x7fff)
                        {
                            iVar6 -= path[blockIndex - 1].BlockLength;
                            break;
                        }

                        iVar6 += iVar2;
                        blockIndex++;
                    }
                } while (iVar6 <= position);

                iVar6 -= position;

                blockIndex--;

                if (iVar6 < 0)
                    blockPosOffset = -iVar6;
                else
                    blockPosOffset = path[blockIndex].BlockLength - iVar6;
            }

            var block = path[blockIndex];

            float xPos = block.XPos + block.DirectionX * blockPosOffset + relativePos.x;
            float yPos = block.YPos + block.DirectionY * blockPosOffset + relativePos.y;
            float zPos = block.ZPos + block.DirectionZ * blockPosOffset + relativePos.z;

            return GetPosition(xPos, yPos, zPos, scale);
        }

        public class ObjSpriteInfo
        {
            public ObjSpriteInfo(int spriteSet, int spriteIndex, int scale = 1, int palOffsetX = 0, int palOffsetY = 500)
            {
                SpriteSet = spriteSet;
                SpriteIndex = spriteIndex;
                Scale = scale;
                PalOffsetX = palOffsetX;
                PalOffsetY = palOffsetY;
            }

            public int SpriteSet { get; }
            public int SpriteIndex { get; }
            public int Scale { get; }
            public int PalOffsetX { get; }
            public int PalOffsetY { get; }
        }
    }
}