using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ray1Map.PSKlonoa
{
    public abstract class PSKlonoa_DTP_BaseManager : BaseGameManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Levels.Select((x, i) => new GameInfo_World(i, x.Item1, Enumerable.Range(0, x.Item2).ToArray())).ToArray());

        public static bool IncludeDebugInfo => FileSystem.mode != FileSystem.Mode.Web && Settings.ShowDebugInfo;

        public virtual (string, int)[] Levels => new (string, int)[]
        {
            ("FIX", 0),
            ("MENU", 0),
            ("INTRO", 0),

            ("Vision 1-1", 3),
            ("Vision 1-2", 5),
            ("Rongo Lango", 2),

            ("Vision 2-1", 4),
            ("Vision 2-2", 6),
            ("Pamela", 2),

            ("Vision 3-1", 5),
            ("Vision 3-2", 10),
            ("Gelg Bolm", 1),

            ("Vision 4-1", 3), // TODO: 5 in proto
            ("Vision 4-2", 8),
            ("Baladium", 2),

            ("Vision 5-1", 7),
            ("Vision 5-2", 9),
            ("Joka", 1),

            ("Vision 6-1", 8),
            ("Vision 6-2", 8),
            ("Ghadius", 2),

            // Order is different here than in-game
            ("Final Vision 1", 2), // TODO: 1 in proto
            ("Final Vision 2", 3),
            ("Nahatomb", 3),

            ("Extra Vision", 9),
        };

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Extract BIN", false, true, (input, output) => Extract_BINAsync(settings, output, false)),
                new GameAction("Extract BIN (unpack archives)", false, true, (input, output) => Extract_BINAsync(settings, output, true)),
                new GameAction("Extract Code", false, true, (input, output) => Extract_CodeAsync(settings, output)),
                new GameAction("Extract Graphics", false, true, (input, output) => Extract_GraphicsAsync(settings, output)),
                new GameAction("Extract Cutscenes", false, true, (input, output) => Extract_Cutscenes(settings, output)),
            };
        }

        public async UniTask Extract_BINAsync(GameSettings settings, string outputPath, bool unpack)
        {
            using var context = new Ray1MapContext(settings);
            var config = GetKlonoaSettings(settings);
            await LoadFilesAsync(context, config);
            context.AddKlonoaSettings(config);

            // Load the IDX
            var idxData = Load_IDX(context, config);

            var s = context.Deserializer;

            var loader = Loader.Create(context, idxData);

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
                [IDXLoadCommand.FileType.Proto_Archive_MenuSprites_0] = 1,
                [IDXLoadCommand.FileType.Proto_Archive_MenuSprites_1] = 1,
                [IDXLoadCommand.FileType.Proto_Archive_MenuSprites_2] = 1,
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
            using var context = new Ray1MapContext(settings);
            var config = GetKlonoaSettings(settings);
            await LoadFilesAsync(context, config);
            context.AddKlonoaSettings(config);

            // Load the IDX
            var idxData = Load_IDX(context, config);

            var loader = Loader.Create(context, idxData);

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

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - 0x{cmd.FILE_Destination:X8}.dat"), codeFile.Data);
                });
            }
        }

        public async UniTask Extract_GraphicsAsync(GameSettings settings, string outputPath)
        {
            using var context = new Ray1MapContext(settings);
            var config = GetKlonoaSettings(settings);
            await LoadFilesAsync(context, config);
            context.AddKlonoaSettings(config);

            // Load the IDX
            var idxData = Load_IDX(context, config);

            // Create the loader
            var loader = Loader.Create(context, idxData);

            // Enumerate every bin block
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                // Load the BIN
                loader.SwitchBlocks(blockIndex);
                loader.LoadAndProcessBINBlock();

                if (blockIndex >= config.BLOCK_FirstLevel)
                    loader.ProcessLevelData();

                // WORLD MAP SPRITES
                var wldMap = loader.GetLoadedFile<WorldMap_ArchiveFile>();
                if (wldMap != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 1)
                            loader.AddToVRAM(wldMap.Palette2);

                        // Enumerate every frame
                        for (int frameIndex = 0; frameIndex < wldMap.AnimatedSprites.Sprites.Files.Length - 1; frameIndex++)
                            exportSprite(wldMap.AnimatedSprites.Sprites.Files[frameIndex].Textures, $"{blockIndex} - WorldMapSprites", i, frameIndex, -1, -1); // TODO: Correct pal
                    }
                }

                // WORLD MAP TEXTURES
                for (var i = 0; i < wldMap?.SpriteSheets.Files.Length; i++)
                {
                    PS1_TIM tim = wldMap.SpriteSheets.Files[i];

                    exportTex(
                        getTex: () => tim.GetTexture(),
                        blockName: $"{blockIndex} - WorldMapTextures",
                        name: $"{i}");
                }

                // MENU SPRITES
                var menuSprites = loader.GetLoadedFile<MenuSprites_ArchiveFile>();

                var sprites_0 = menuSprites?.Sprites_0;
                var sprites_1 = menuSprites?.Sprites_1;
                var sprites_2 = menuSprites?.Sprites_2;

                if (loader.GameVersion == KlonoaGameVersion.DTP_Prototype_19970717)
                {
                    for (int fileIndex = 0; fileIndex < loader.LoadedFiles[blockIndex].Length; fileIndex++)
                    {
                        switch (loader.IDX.Entries[blockIndex].LoadCommands[fileIndex].FILE_Type)
                        {
                            case IDXLoadCommand.FileType.Proto_Archive_MenuSprites_0:
                                sprites_0 = (Sprites_ArchiveFile)loader.LoadedFiles[blockIndex][fileIndex];
                                break;

                            case IDXLoadCommand.FileType.Proto_Archive_MenuSprites_1:
                                sprites_1 = (Sprites_ArchiveFile)loader.LoadedFiles[blockIndex][fileIndex];
                                break;

                            case IDXLoadCommand.FileType.Proto_Archive_MenuSprites_2:
                                sprites_2 = (Sprites_ArchiveFile)loader.LoadedFiles[blockIndex][fileIndex];
                                break;
                        }
                    }
                }

                for (int frameIndex = 0; frameIndex < sprites_0?.Files.Length - 1; frameIndex++)
                    exportSprite(sprites_0.Files[frameIndex].Textures, $"{blockIndex} - Menu", 0, frameIndex, 960, 442);

                for (int frameIndex = 0; frameIndex < sprites_1?.Files.Length - 1; frameIndex++)
                    exportSprite(sprites_1.Files[frameIndex].Textures, $"{blockIndex} - Menu", 1, frameIndex, 0, 480);

                for (int frameIndex = 0; frameIndex < sprites_2?.Files.Length - 1; frameIndex++)
                    exportSprite(sprites_2.Files[frameIndex].Textures, $"{blockIndex} - Menu", 2, frameIndex, 960, 440);

                for (int frameIndex = 0; frameIndex < menuSprites?.AnimatedSprites.Sprites.Files.Length - 1; frameIndex++)
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

                    exportSprite(menuSprites.AnimatedSprites.Sprites.Files[frameIndex].Textures, $"{blockIndex} - Menu", 3, frameIndex, 0, palY);
                }

                // MENU BACKGROUND TEXTURES
                var menuBg = loader.GetLoadedFile<ArchiveFile<TIM_ArchiveFile>>();
                for (var i = 0; i < menuBg?.Files.Length; i++)
                {
                    var tims = menuBg.Files[i];

                    for (var j = 0; j < tims.Files.Length; j++)
                    {
                        PS1_TIM tim = tims.Files[j];
                        exportTex(
                            getTex: () => tim.GetTexture(onlyFirstTransparent: true),
                            blockName: $"{blockIndex} - MenuBackgrounds",
                            name: $"{i} - {j}");
                    }
                }

                // TIM TEXTURES
                for (int fileIndex = 0; fileIndex < loader.LoadedFiles[blockIndex].Length; fileIndex++)
                {
                    if (!(loader.LoadedFiles[blockIndex][fileIndex] is TIM_ArchiveFile timArchive)) 
                        continue;
                    
                    for (var i = 0; i < timArchive.Files.Length; i++)
                    {
                        PS1_TIM tim = timArchive.Files[i];
                        exportTex(
                            getTex: () => tim.GetTexture(),
                            blockName: $"{blockIndex} - {loader.IDX.Entries[blockIndex].LoadCommands[fileIndex].FILE_Type.ToString().Replace("Archive_TIM_", String.Empty)}",
                            name: $"{fileIndex} - {i}");
                    }
                }

                // GAME OBJECT TEXTURES
                if (loader.LevelData3D != null)
                {
                    for (int sectorIndex = 0; sectorIndex < loader.LevelData3D.SectorGameObjectDefinition.Length; sectorIndex++)
                    {
                        var sectorGameObjects = loader.LevelData3D.SectorGameObjectDefinition[sectorIndex].ObjectsDefinitions;

                        for (int objIndex = 0; objIndex < sectorGameObjects.Length; objIndex++)
                        {
                            var obj = sectorGameObjects[objIndex];

                            if (obj.Data?.TIM != null)
                            {
                                exportTex(
                                    getTex: () => obj.Data?.TIM.GetTexture(),
                                    blockName: $"{blockIndex} - GameObjectTextures",
                                    name: $"{sectorIndex} - {objIndex} - Texture");
                            }
                            
                            if (obj.Data?.TIMArchive != null)
                            {
                                var textures = obj.Data?.TIMArchive;

                                for (int texIndex = 0; texIndex < textures.Files.Length; texIndex++)
                                {
                                    exportTex(
                                        getTex: () => textures.Files[texIndex].Obj.GetTexture(),
                                        blockName: $"{blockIndex} - GameObjectTextures",
                                        name: $"{sectorIndex} - {objIndex} - Textures - {texIndex}");
                                }
                            }

                            if (obj.Data?.TextureAnimation != null)
                            {
                                var texAnim = obj.Data.TextureAnimation;

                                for (int texIndex = 0; texIndex < texAnim.Files.Length; texIndex++)
                                {
                                    exportTex(
                                        getTex: () => texAnim.Files[texIndex].Obj.GetTexture(),
                                        blockName: $"{blockIndex} - GameObjectTextures",
                                        name: $"{sectorIndex} - {objIndex} - TextureAnimation - {texIndex}");
                                }
                            }
                        }
                    }
                }

                // BACKGROUND TEXTURES (TILE SETS)
                var bgPack = loader.BackgroundPack;
                for (var i = 0; i < bgPack?.TIMFiles.Files.Length; i++)
                {
                    var tim = bgPack.TIMFiles.Files[i];
                    exportTex(
                        getTex: () => tim.GetTexture(noPal: true),
                        blockName: $"{blockIndex} - BackgroundTileSets",
                        name: $"{i}");
                }

                // BACKGROUND ANIMATIONS (MAPS)
                if (bgPack != null)
                {
                    var exportedLayers = new HashSet<BackgroundGameObject>();

                    for (int sectorIndex = 0; sectorIndex < bgPack.BackgroundGameObjectsFiles.Files.Length; sectorIndex++)
                    {
                        var objects = bgPack.BackgroundGameObjectsFiles.Files[sectorIndex].Objects.Where(x => !exportedLayers.Contains(x)).ToArray();
                        var objectsLoader = new KlonoaObjectsLoader(loader, 0, null, null);

                        // Load to get the backgrounds with their animations
                        await objectsLoader.LoadAsync(new GameObjectDefinition[0], objects);
                        await objectsLoader.Anim_Manager.LoadTexturesAsync(loader.VRAM);

                        // Export every layer
                        foreach (KlonoaBackgroundLayer layer in objectsLoader.BackgroundLayers)
                        {
                            exportedLayers.Add(layer.Object);
                            export(layer.Frames, layer.Speed, layer.Object.BGDIndex);
                        }
                    }

                    // Export unused layers
                    for (int bgdIndex = 0; bgdIndex < bgPack.BGDFiles.Files.Length; bgdIndex++)
                    {
                        if (exportedLayers.Any(x => x.BGDIndex == bgdIndex))
                            continue;

                        PS1_TIM tim = bgPack.TIMFiles.Files[0];
                        PS1_CEL cel = bgPack.CELFiles.Files[0];
                        PS1_BGD map = bgPack.BGDFiles.Files[bgdIndex];

                        Texture2D tex = loader.VRAM.FillMapTexture(tim, cel, map);

                        export(new Texture2D[]
                        {
                            tex
                        }, 0, bgdIndex);
                    }

                    void export(IList<Texture2D> frames, int speed, int index)
                    {
                        if (frames.Count > 1)
                        {
                            Util.ExportAnimAsGif(frames, speed, false, false, Path.Combine(outputPath, $"{blockIndex} - Backgrounds", $"{index}.gif"));
                        }
                        else
                        {
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - Backgrounds", $"{index}.png"), frames[0].EncodeToPNG());
                        }
                    }
                }

                // SPRITE SETS
                for (int spriteSetIndex = 0; spriteSetIndex < loader.SpriteSets.Length; spriteSetIndex++)
                {
                    var spriteSet = loader.SpriteSets[spriteSetIndex];

                    if (spriteSet == null)
                        continue;

                    // Enumerate every sprite
                    for (int spriteIndex = 0; spriteIndex < spriteSet.Files.Length - 1; spriteIndex++)
                        exportSprite(spriteSet.Files[spriteIndex].Textures, $"{blockIndex} - SpriteSets", spriteSetIndex, spriteIndex, 0, 500);
                }

                // SPRITE SET PLAYER SPRITES
                var spritePack = loader.GetLoadedFile<LevelSpritePack_ArchiveFile>();
                if (spritePack != null)
                {
                    var exportedPlayerSprites = new HashSet<PlayerSprite_File>();

                    var pal = spritePack.PlayerSprites.Files.FirstOrDefault(x => x?.TIM?.Clut != null)?.TIM.Clut.Palette.Select(x => x.GetColor()).ToArray();

                    for (var i = 0; i < spritePack.PlayerSprites.Files.Length; i++)
                    {
                        var file = spritePack.PlayerSprites.Files[i];
                        if (file != null && !exportedPlayerSprites.Contains(file))
                        {
                            exportedPlayerSprites.Add(file);

                            exportTex(() =>
                            {
                                if (file.TIM != null)
                                    return file.TIM.GetTexture(palette: pal);
                                else
                                    return PS1Helpers.GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height,
                                        PS1_TIM.TIM_ColorFormat.BPP_8);
                            }, $"{blockIndex} - PlayerSprites", $"{i}");
                        }
                    }
                }

                // LEVEL MENU SPRITES
                var lvlMenuSprites = loader.GetLoadedFile<LevelMenuSprites_ArchiveFile>();
                if (lvlMenuSprites != null)
                {
                    for (int frameIndex = 0; frameIndex < lvlMenuSprites.Sprites_0.Files.Length; frameIndex++)
                        exportSprite(lvlMenuSprites.Sprites_0.Files[frameIndex].Textures, $"{blockIndex} - Menu", 0, frameIndex, 960, 442);

                    for (int frameIndex = 0; frameIndex < lvlMenuSprites.Sprites_1.Files.Length; frameIndex++)
                        exportSprite(lvlMenuSprites.Sprites_1.Files[frameIndex].Textures, $"{blockIndex} - Menu", 1, frameIndex, -1, -1); // TODO: Correct pal
                }

                // CUTSCENE SPRITES
                var cutscenePack = loader.LevelPack?.CutscenePack;
                if (cutscenePack?.Sprites != null)
                {
                    for (int frameIndex = 0; frameIndex < cutscenePack.Sprites.Files.Length - 1; frameIndex++)
                        exportSprite(cutscenePack.Sprites.Files[frameIndex].Textures, $"{blockIndex} - CutsceneSprites", 0, frameIndex, 0, 500);
                }

                // CUTSCENE CHARACTER NAMES
                if (cutscenePack?.CharacterNamesImgData != null)
                    exportTex(
                        getTex: () => PS1Helpers.GetTexture(imgData: cutscenePack.CharacterNamesImgData.Data, pal: null, width: 0x0C, height: 0x50, colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4), 
                        blockName: $"{blockIndex} - CutsceneCharacterNames",
                        name: $"0");

                // CUTSCENE PLAYER SPRITES
                if (cutscenePack?.PlayerFramesImgData != null)
                {
                    var playerPal = loader.VRAM.GetColors1555(0, 0, 160, 511, 256).Select(x => x.GetColor()).ToArray();

                    for (var i = 0; i < cutscenePack.PlayerFramesImgData.Files.Length; i++)
                    {
                        var file = cutscenePack.PlayerFramesImgData.Files[i];
                        exportTex(
                            getTex: () => PS1Helpers.GetTexture(file.ImgData, playerPal, file.Width, file.Height,
                                PS1_TIM.TIM_ColorFormat.BPP_8), 
                            blockName: $"{blockIndex} - CutscenePlayerSprites",
                            name: $"{i}");
                    }
                }

                // CUTSCENE ANIMATIONS
                if (cutscenePack?.SpriteAnimations != null)
                {
                    var playerPal = loader.VRAM.GetColors1555(0, 0, 160, 511, 256).Select(x => x.GetColor()).ToArray();

                    for (var i = 0; i < cutscenePack.SpriteAnimations.Animations.Length; i++)
                    {
                        var anim = cutscenePack.SpriteAnimations.Animations[i];

                        var isPlayerAnim = (cutscenePack.Cutscenes.SelectMany(x => x.Cutscene_Normal.Instructions).FirstOrDefault(x => x.Type == CutsceneInstruction.InstructionType.SetObj2DAnimation && ((CutsceneInstructionData_SetObjAnimation)x.Data).AnimIndex == i)?.Data as CutsceneInstructionData_SetObjAnimation)?.ObjIndex == 0;

                        var animFrames = GetAnimationFrames(
                            loader: loader, 
                            anim: anim, 
                            sprites: cutscenePack.Sprites, 
                            palX: 0, 
                            palY: 500, 
                            isCutscenePlayer: isPlayerAnim, 
                            playerSprites: cutscenePack.PlayerFramesImgData.Files, 
                            playerPalette: playerPal);

                        if (animFrames.Textures == null)
                            continue;

                        if (animFrames.Textures.Any(x => x == null))
                        {
                            Debug.LogWarning($"At least one animation frame is null");
                            continue;
                        }

                        Util.ExportAnimAsGif(
                            frames: animFrames.Textures, 
                            speeds: anim.Frames.Select(x => (int)x.FrameDelay).ToArray(), 
                            center: false, 
                            trim: false, 
                            filePath: Path.Combine(outputPath, $"{blockIndex} - CutsceneAnimations", $"{i}.gif"),
                            frameOffsets: animFrames.Offsets,
                            uniformSize: true,
                            uniformGravity: ImageMagick.Gravity.Southwest);
                    }
                }

                loader.VRAM.ExportToFile(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"));
            }

            void exportTex(Func<Texture2D> getTex, string blockName, string name)
            {
                try
                {
                    var tex = getTex();

                    if (tex != null)
                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockName}", $"{name}.png"),
                            tex.EncodeToPNG());
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting with ex: {ex}");
                }
            }

            void exportSprite(SpriteTexture[] spriteTextures, string blockName, int setIndex, int frameIndex, int palX, int palY)
            {
                try
                {
                    var tex = GetTexture(spriteTextures, loader.VRAM, palX, palY).Texture;

                    Util.ByteArrayToFile(Path.Combine(outputPath, blockName, $"{setIndex} - {frameIndex}.png"), tex.EncodeToPNG());
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting sprite frame: {ex}");
                }
            }
        }

        public async UniTask Extract_Cutscenes(GameSettings settings, string outputPath)
        {
            using var context = new Ray1MapContext(settings);
            var config = GetKlonoaSettings(settings);
            await LoadFilesAsync(context, config);
            context.AddKlonoaSettings(config);

            // Load the IDX
            var idxData = Load_IDX(context, config);

            var loader = Loader.Create(context, idxData);

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    // Check the file type
                    if (cmd.FILE_Type != IDXLoadCommand.FileType.Archive_LevelPack)
                        return;

                    // Read the data
                    var lvl = loader.LoadBINFile<LevelPack_ArchiveFile>(i);

                    // Make sure it has normal cutscens
                    if (lvl.CutscenePack.Cutscenes == null)
                        return;

                    // Enumerate every cutscene
                    for (var cutsceneIndex = 0; cutsceneIndex < lvl.CutscenePack.Cutscenes.Length; cutsceneIndex++)
                    {
                        Cutscene cutscene = lvl.CutscenePack.Cutscenes[cutsceneIndex];
                        var normalCutscene = CutsceneTextTranslationTables.CutsceneToText(
                            cutscene: cutscene,
                            translationTable: GetCutsceneTranslationTable,
                            includeInstructionIndex: false,
                            normalCutscene: true);

                        File.WriteAllText(Path.Combine(outputPath, $"{blockIndex}_{cutsceneIndex}.txt"), normalCutscene);

                        if (cutscene.Cutscene_Skip != null)
                        {
                            var skipCutscene = CutsceneTextTranslationTables.CutsceneToText(
                                cutscene: cutscene,
                                translationTable: GetCutsceneTranslationTable,
                                includeInstructionIndex: false,
                                normalCutscene: false);

                            File.WriteAllText(Path.Combine(outputPath, $"{blockIndex}_{cutsceneIndex} (skip).txt"), skipCutscene);
                        }
                    }
                });
            }
        }

        public abstract KlonoaSettings_DTP GetKlonoaSettings(GameSettings settings);

        public abstract Dictionary<string, char> GetCutsceneTranslationTable { get; }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            var stopWatch = IncludeDebugInfo ? Stopwatch.StartNew() : null;
            var startupLog = IncludeDebugInfo ? new StringBuilder() : null;

            // Get settings
            GameSettings settings = context.GetR1Settings();
            int lev = settings.World;
            int sector = settings.Level;
            KlonoaSettings_DTP config = GetKlonoaSettings(settings);
            const float scale = 64f;

            // Create the level
            var level = new Unity_Level()
            {
                CellSize = 16,
                FramesPerSecond = 60,
                IsometricData = new Unity_IsometricData
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
            };

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Retrieved settings");

            // Load the files
            await LoadFilesAsync(context, config);

            // Add the settings
            context.AddKlonoaSettings(config);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded files");

            Controller.DetailedState = "Loading IDX";
            await Controller.WaitIfNecessary();

            // Load the IDX
            IDX idxData = Load_IDX(context, config);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded IDX");

            ArchiveFile.AddToParsedArchiveFiles = true;

            Controller.DetailedState = "Loading BIN";
            await Controller.WaitIfNecessary();

            // Create the loader
            var loader = Loader.Create(context, idxData);

            // Only parse the selected sector
            loader.LevelSector = sector;

            var logAction = new Func<string, Task>(async x =>
            {
                Controller.DetailedState = x;
                await Controller.WaitIfNecessary();
            });

            // Load the fixed BIN
            loader.SwitchBlocks(loader.Settings.BLOCK_Fix);
            await loader.FillCacheForBlockReadAsync();
            await loader.LoadAndProcessBINBlockAsync(logAction);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded fixed BIN");

            // Load the level BIN
            loader.SwitchBlocks(lev);
            await loader.FillCacheForBlockReadAsync();
            await loader.LoadAndProcessBINBlockAsync(logAction);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded level BIN");

            Controller.DetailedState = "Loading level data";
            await Controller.WaitIfNecessary();

            // Load hard-coded level data
            loader.ProcessLevelData();

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded hard-coded level data");

            // Load the layers
            await Load_LayersAsync(level, loader, sector, scale);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded layers");

            // Load object manager
            Unity_ObjectManager_PSKlonoa_DTP objManager = await Load_ObjManagerAsync(loader);
            level.ObjManager = objManager;

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded object manager");

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            level.EventData = Load_Objects(loader, sector, scale, objManager);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded objects");

            Controller.DetailedState = "Loading paths";
            await Controller.WaitIfNecessary();

            level.CollisionLines = loader.LevelPack.Sectors[sector].MovementPaths.Files.SelectMany(x => x.Blocks).GetMovementPaths(scale);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded paths");

            var bgClear = loader.BackgroundPack?.BackgroundGameObjectsFiles.Files.
                ElementAtOrDefault(sector)?.Objects.
                Where(x => x.Type == BackgroundGameObject.BackgroundGameObjectType.Clear_Gradient ||
                           x.Type == BackgroundGameObject.BackgroundGameObjectType.Clear).
                Select(x => x.Data_Clear).
                ToArray();

            if (bgClear?.Any() == true)
                level.CameraClear = new Unity_CameraClear(bgClear.First().Entries[0].Color.GetColor());

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded camera clears");

            level.StartIn3D = true;
            MovementPathCamera cam = loader.LevelData3D.MovementPathCameras.PathCameras[sector].FirstOrDefault(x => x.Type == MovementPathCamera.CameraType.Absolute);

            if (cam?.AbsolutePosition != null)
            {
                level.StartPosition = cam.AbsolutePosition.GetPositionVector(scale);
            }
            else
            {
                Debug.Log($"No default camera position defined");
                level.StartPosition = new Vector3(0, 10, -60);
            }

            if (cam?.AbsoluteRotation != null)
                level.StartRotation = cam.AbsoluteRotation.GetQuaternion(true);

            startupLog?.AppendLine($"{stopWatch.ElapsedMilliseconds:0000}ms - Loaded default camera position");

            if (IncludeDebugInfo)
            {
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
            }

            level.PS1_VRAM = loader.VRAM;

            return level;
        }

        public async UniTask Load_LayersAsync(Unity_Level level, Loader loader, int sector, float scale)
        {
            var layers = new List<Unity_Layer>();
            var parent3d = Controller.obj.levelController.editor.layerTiles;

            Controller.DetailedState = "Loading game objects";
            await Controller.WaitIfNecessary();

            var gao_3dObjParent = new GameObject("3D Objects");
            gao_3dObjParent.transform.localPosition = Vector3.zero;
            gao_3dObjParent.transform.localRotation = Quaternion.identity;
            gao_3dObjParent.transform.localScale = Vector3.one;

            // Load the objects first so we get the VRAM animations
            var objectsLoader = new KlonoaObjectsLoader(loader, scale, level.IsometricData, gao_3dObjParent);

            var objects3D = loader.LevelData3D.SectorGameObjectDefinition[sector].ObjectsDefinitions;

            await objectsLoader.LoadAsync(objects3D, loader.BackgroundPack.BackgroundGameObjectsFiles.Files.ElementAtOrDefault(sector)?.Objects);

            if (IncludeDebugInfo)
            {
                Debug.Log($"TEXTURE ANIMATIONS:{Environment.NewLine}" +
                          $"{String.Join(Environment.NewLine, objectsLoader.TextureAnimations.Select(a => $"{(a.UsesSingleRegion ? a.Region.ToString() : String.Join(", ", a.Regions))}"))}");
                Debug.Log($"PALETTE ANIMATIONS:{Environment.NewLine}" +
                          $"{String.Join(Environment.NewLine, objectsLoader.PaletteAnimations.Select(a => $"{(a.UsesSingleRegion ? a.Region.ToString() : String.Join(", ", a.Regions))}"))}");
            }

            // Load tracks

            Controller.DetailedState = "Loading camera tracks";
            await Controller.WaitIfNecessary();

            level.TrackManagers = objectsLoader.CameraAnimations.Select(x => new Unity_TrackManager_PSKlonoaDTP(x, scale)).ToArray();

            if (level.TrackManagers.Length > 1)
                Debug.LogWarning($"More than 1 track manager!");

            Controller.DetailedState = "Loading backgrounds";
            await Controller.WaitIfNecessary();

            // Load gradients
            var backgroundsClear = Load_Layers_Gradients(loader, sector, scale);
            layers.AddRange(backgroundsClear);

            // Load backgrounds
            var backgrounds = Load_Layers_Backgrounds(loader, objectsLoader, scale);
            layers.AddRange(backgrounds);

            Controller.DetailedState = "Loading level geometry";
            await Controller.WaitIfNecessary();

            // Load level object
            var levelObj = Load_Layers_LevelObject(loader, parent3d, objectsLoader, sector, scale);
            layers.Add(levelObj);

            // Add layer for 3D objects last
            layers.Add(new Unity_Layer_GameObject(true, isAnimated: objectsLoader.GetIsAnimated)
            {
                Name = "3D Objects",
                ShortName = $"3DO",
                Graphics = gao_3dObjParent
            });
            gao_3dObjParent.transform.SetParent(parent3d.transform, false);

            // Log some debug info
            if (IncludeDebugInfo)
            {
                Debug.Log($"MAP INFO{Environment.NewLine}" +
                          $"{objectsLoader.Anim_Manager.AnimatedTextures.SelectMany(x => x.Value).Count()} texture animations{Environment.NewLine}" +
                          $"{objectsLoader.ScrollAnimations.Count} UV scroll animations{Environment.NewLine}" +
                          $"Objects:{Environment.NewLine}\t" +
                          $"{String.Join($"{Environment.NewLine}\t", objects3D.Take(objects3D.Length - 1).Select(x => $"{x.Offset}: {(int)x.PrimaryType:00}-{x.SecondaryType:00} ({x.Data?.GlobalGameObjectType})"))}");
            }

            await objectsLoader.Anim_Manager.LoadTexturesAsync(loader.VRAM);

            level.Layers = layers.ToArray();
        }

        public IEnumerable<Unity_Layer> Load_Layers_Gradients(Loader loader, int sector, float scale) {

            // Add background objects
            var bgObjects = loader.BackgroundPack?.BackgroundGameObjectsFiles.Files.
                ElementAtOrDefault(sector)?.Objects;
            var bgClear = bgObjects?.
                Where(x => x.Type == BackgroundGameObject.BackgroundGameObjectType.Clear_Gradient ||
                           x.Type == BackgroundGameObject.BackgroundGameObjectType.Clear).
                Select(x => x.Data_Clear);

            List<Unity_Layer_GameObject> gaoLayers = new List<Unity_Layer_GameObject>();
            int pixelsPerUnit = 16;

            foreach (var clear in bgClear) {
                Mesh m = new Mesh();
                Vector3[] vertices = clear.Entries.Select(e =>
                    new Vector3(
                        ((e.XPos_RelativeObj != -1 ? bgObjects[e.XPos_RelativeObj].XPos : 0) + e.XPos) / pixelsPerUnit,
                        -((e.YPos_RelativeObj != -1 ? bgObjects[e.YPos_RelativeObj].YPos : 0) + e.YPos) / pixelsPerUnit,
                        0)).ToArray();
                Color[] colors = clear.Entries.Select(e =>
                    e.Color.GetColor()).ToArray();
                int[] triangles = new int[] {
                    0, 1, 2, 0, 2, 1,
                    2, 1, 3, 2, 3, 1,
                };

                // Compensate for having gradients relative to the camera
                int screenHeight = 208;
                int screenWidth = 320;
                if ((clear.Entries[0].YPos_RelativeObj == -1) != (clear.Entries[3].YPos_RelativeObj == -1)) {
                    if (clear.Entries[0].YPos_RelativeObj == -1) {
                        vertices[0] = new Vector3(vertices[0].x, vertices[3].y + (float)screenHeight / pixelsPerUnit, vertices[0].z);
                    } else {
                        vertices[3] += new Vector3(0, vertices[0].y, 0);
                    }
                }
                if ((clear.Entries[0].XPos_RelativeObj == -1) != (clear.Entries[3].XPos_RelativeObj == -1)) {
                    if (clear.Entries[0].XPos_RelativeObj == -1) {
                        vertices[0] = new Vector3(vertices[3].x -(float)screenWidth / pixelsPerUnit, vertices[0].y, vertices[0].z);
                    } else {
                        vertices[3] += new Vector3(vertices[0].x, 0, 0);
                    }
                }

                // Use points 0 and 3 only
                colors[1] = colors[0];
                colors[2] = colors[3];
                vertices[1] = new Vector3(vertices[3].x, vertices[0].y, 0);
                vertices[2] = new Vector3(vertices[0].x, vertices[3].y, 0);

                m.SetVertices(vertices);
                m.SetColors(colors);
                m.SetTriangles(triangles, 0);
                m.RecalculateNormals();


                int index = bgObjects.FindItemIndex(clearData => clearData.Data_Clear == clear);
                GameObject gao = new GameObject($"Gradient {index}");
                MeshFilter mf = gao.AddComponent<MeshFilter>();
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                mr.sortingLayerName = "Tiles";
                gao.layer = LayerMask.NameToLayer("Tiles");
                gao.transform.localScale = new Vector3(1, 1, 1f);
                gao.transform.localRotation = Quaternion.identity;
                //gao.transform.localRotation = Quaternion.Euler(90,0,0);
                gao.transform.localPosition = new Vector3(0,0,101); // Puts it right behind the layers
                mf.mesh = m;
                mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
                mf.mesh = m;

                gaoLayers.Add(new Unity_Layer_GameObject(false) {
                    Graphics = gao,
                    Name = $"Gradient {index}",
                    ShortName = $"GR{index}"
                });
            }

            return gaoLayers;
        }

        public IEnumerable<Unity_Layer> Load_Layers_Backgrounds(Loader loader, KlonoaObjectsLoader objectsLoader, float scale)
        {
            // Get the background textures
            var bgLayers = objectsLoader.BackgroundLayers;
            int pixelsPerUnit = 16;
            // Add the backgrounds
            return bgLayers.Select((t, i) => new Unity_Layer_Texture
            {
                Name = $"Background {i}",
                ShortName = $"BG{i}",
                Textures = t.Frames,
                AnimSpeed = t.Speed,
                PositionOffset = new Vector3(t.Object.XPos, -t.Object.YPos, 0) / pixelsPerUnit
            }).Reverse();
        }

        public Unity_Layer Load_Layers_LevelObject(Loader loader, GameObject parent, KlonoaObjectsLoader objectsLoader, int sector, float scale)
        {
            var tmdGameObj = new KlonoaTMDGameObject(
                    tmd: loader.LevelPack.Sectors[sector].LevelModel,
                    vram: loader.VRAM,
                    scale: scale,
                    objectsLoader: objectsLoader,
                    isPrimaryObj: true);

            GameObject obj = tmdGameObj.CreateGameObject("Map", IncludeDebugInfo);

            Bounds levelBounds = loader.LevelPack.Sectors[sector].LevelModel.GetDimensions(scale);

            // Calculate actual level dimensions: switched axes for unity & multiplied by cellSize
            int cellSize = 16;

            //var layerDimensions = new Vector3(size.x, size.z, size.y) * cellSize;
            var layerDimensions = new Rect(
                levelBounds.min.x * cellSize, -levelBounds.max.z * cellSize,
                levelBounds.size.x * cellSize, levelBounds.size.z * cellSize);
            // Correctly center object
            //obj.transform.position = new Vector3(-levelBounds.min.x, 0, -size.z-levelBounds.min.z);
            
            obj.transform.SetParent(parent.transform, false);

            var collisionObj = loader.LevelPack.Sectors[sector].LevelCollisionTriangles.CollisionTriangles.GetCollisionGameObject(scale);
            collisionObj.transform.SetParent(Controller.obj.levelController.editor.layerTypes.transform, false);

            return new Unity_Layer_GameObject(true, isAnimated: tmdGameObj.HasAnimations)
            {
                Name = "Map",
                ShortName = "MAP",
                Graphics = obj,
                Collision = collisionObj,
                Dimensions = layerDimensions,
                DisableGraphicsWhenCollisionIsActive = true
            };
        }

        public async UniTask<Unity_ObjectManager_PSKlonoa_DTP> Load_ObjManagerAsync(Loader loader)
        {
            var spriteSets = new List<Unity_ObjectManager_PSKlonoa_DTP.SpriteSet>();

            // Enumerate each sprite set
            for (var i = 0; i < loader.SpriteSets.Length; i++)
            {
                Controller.DetailedState = $"Loading sprites {i + 1}/{loader.SpriteSets.Length}";
                await Controller.WaitIfNecessary();

                var frames = loader.SpriteSets[i];

                // Skip if null
                if (frames == null)
                    continue;

                // Create the sprites
                var sprites = frames.Files.Take(frames.Files.Length - 1).Select(x => GetTexture(x.Textures, loader.VRAM, 0, 500).Texture.CreateSprite()).ToArray();

                var set = new Unity_ObjectManager_PSKlonoa_DTP.SpriteSet(sprites, Unity_ObjectManager_PSKlonoa_DTP.SpritesType.SpriteSets, i);

                spriteSets.Add(set);
            }

            return new Unity_ObjectManager_PSKlonoa_DTP(loader.Context, spriteSets.ToArray());
        }

        public List<Unity_SpriteObject> Load_Objects(Loader loader, int sector, float scale, Unity_ObjectManager_PSKlonoa_DTP objManager)
        {
            var objects = new List<Unity_SpriteObject>();
            var movementPaths = loader.LevelPack.Sectors[sector].MovementPaths.Files;

            // Add enemies
            foreach (EnemyObject enemyObj in loader.LevelData2D.Enemy_Objects.Where(x => x.GlobalSectorIndex == loader.GlobalSectorIndex))
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

                    objects.Add(new Unity_Object_PSKlonoa_DTP_Enemy(objManager, obj, scale, spriteInfo));
                }
            }

            // Add enemy spawn points
            for (int pathIndex = 0; pathIndex < movementPaths.Length; pathIndex++)
            {
                for (int objIndex = 0; objIndex < loader.LevelData2D.Enemy_ObjectIndexTables.IndexTables[pathIndex].Length; objIndex++)
                {
                    var obj = loader.LevelData2D.Enemy_Objects[loader.LevelData2D.Enemy_ObjectIndexTables.IndexTables[pathIndex][objIndex]];

                    if (obj.GlobalSectorIndex != loader.GlobalSectorIndex)
                        continue;

                    var pos = movementPaths[pathIndex].Blocks.GetPosition(obj.MovementPathSpawnPosition, Vector3.zero, scale);

                    objects.Add(new Unity_Object_Dummy(obj, Unity_ObjectType.Trigger, objLinks: new int[]
                    {
                        objects.OfType<Unity_Object_PSKlonoa_DTP_Enemy>().FindItemIndex(x => x.Object == obj)
                    })
                    {
                        Position = pos,
                    });
                }
            }

            // Add collectibles
            objects.AddRange(loader.LevelData2D.Collectible_Objects.Where(x => x.GlobalSectorIndex == loader.GlobalSectorIndex && x.SecondaryType != -1).Select(x =>
            {
                Vector3 pos;

                // If the path index is -1 then the position is absolute, otherwise it's relative
                if (x.MovementPath == -1)
                    pos = KlonoaHelpers.GetPosition(x.Position.X.Value, x.Position.Y.Value, x.Position.Z.Value, scale);
                else
                    pos = movementPaths[x.MovementPath].Blocks.GetPosition(x.MovementPathPosition, new Vector3(0, x.Position.Y.Value, 0), scale);

                var spriteInfo = GetSprite_Collectible(x);

                if (spriteInfo.SpriteSet == -1 || spriteInfo.SpriteIndex == -1)
                    Debug.LogWarning($"Sprite could not be determined for collectible object of secondary type {x.SecondaryType}");

                return new Unity_Object_PSKlonoa_DTP_Collectible(objManager, x, pos, spriteInfo);
            }));

            // Add scenery objects
            objects.AddRange(loader.LevelData3D.SectorGameObjectDefinition[sector].ObjectsDefinitions.
                Where(x => x.Data?.ScenerySprites != null || x.Data?.LightPositions != null).
                SelectMany(x => (x.Data.LightPositions ?? x.Data.ScenerySprites).Vectors[0]).
                Select(x => new Unity_Object_Dummy(x, Unity_ObjectType.Object)
            {
                Position = KlonoaHelpers.GetPosition(x.X, x.Y, x.Z, scale),
            }));

            var wpIndex = objects.Count;
            // Temporarily add waypoints at each path block to visualize them
            /*objects.AddRange(movementPaths.SelectMany((x, i) => x.Blocks.SelectMany(b => new Unity_Object[]
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
            })));*/

            return objects;
        }

        public IDX Load_IDX(Context context, KlonoaSettings_DTP settings)
        {
            return FileFactory.Read<IDX>(settings.FilePath_IDX, context);
        }

        public (Texture2D Texture, RectInt Rect) GetTexture(SpriteTexture[] spriteTextures, PS1_VRAM vram, int palX, int palY)
        {
            if (spriteTextures?.Any() != true)
                return default;

            var rects = spriteTextures.Select(s => 
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

            for (var i = 0; i < spriteTextures.Length; i++)
            {
                var index = spriteTextures.Length - i - 1;

                var sprite = spriteTextures[index];
                var texPage = sprite.TexturePage;

                try
                {
                    vram.FillTexture(
                        tex: tex,
                        width: sprite.Width,
                        height: sprite.Height,
                        colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4,
                        texX: rects[index].x - minX,
                        texY: rects[index].y - minY,
                        clutX: palX + sprite.PalOffsetX,
                        clutY: palY + sprite.PalOffsetY,
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

            return (tex, new RectInt(minX, minY, width, height));
        }

        public (Texture2D[] Textures, Vector2Int[] Offsets) GetAnimationFrames(Loader loader, SpriteAnimation anim, Sprites_ArchiveFile sprites, int palX, int palY, bool isCutscenePlayer = false, CutscenePlayerSprite_File[] playerSprites = null, Color[] playerPalette = null)
        {
            var textures = new Texture2D[anim.FramesCount];
            var rects = new RectInt?[anim.FramesCount];

            for (int i = 0; i < anim.FramesCount; i++)
            {
                var frame = anim.Frames[i];

                try
                {
                    if (isCutscenePlayer && playerSprites != null && playerPalette != null)
                    {
                        if (frame.PlayerAnimation != 0xFF)
                        {
                            if (frame.PlayerAnimation == 0x99)
                            {
                                var playerSprite = playerSprites[frame.SpriteIndex - 1];

                                textures[i] = PS1Helpers.GetTexture(playerSprite.ImgData, playerPalette, playerSprite.Width,
                                    playerSprite.Height, PS1_TIM.TIM_ColorFormat.BPP_8);
                                rects[i] = new RectInt(frame.XPosition, 0, textures[i].width, textures[i].height);
                            }
                            else
                            {
                                // Animate Klonoa normally - nothing we can do sadly, so set to null
                                textures[i] = null;
                            }

                            continue;
                        }
                    }

                    var sprite = sprites.Files[frame.SpriteIndex];

                    var tex = GetTexture(sprite.Textures, loader.VRAM, palX, palY);
                    textures[i] = tex.Texture;
                    rects[i] = new RectInt(tex.Rect.x + frame.XPosition, tex.Rect.y, tex.Rect.width, tex.Rect.height);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting animation. IsPlayer: {isCutscenePlayer}, Sprite: {frame.SpriteIndex}, PlayerAnimation: {frame.PlayerAnimation}, Frame: {i}, Offset: {frame.Offset}, Exception: {ex}");

                    return default;
                }
            }

            if (rects.All(x => x == null))
                return default;

            var minX = rects.Where(x => x != null).Min(x => x.Value.x);
            var minY = rects.Where(x => x != null).Min(x => x.Value.y);

            return (textures, rects.Select((x, i) => rects[i] == null ? default : new Vector2Int(rects[i].Value.x - minX, rects[i].Value.y - minY)).ToArray());
        }

        public ObjSpriteInfo GetSprite_Enemy(EnemyObject obj)
        {
            // TODO: Some enemies have palette swaps. This is done by modifying the x and y palette offsets (by default they are 0 and 500). For example the shielded Moo enemies in Vision 1-2

            // There are 42 object types (0-41). The graphics index is an index to an array of functions for displaying the graphics. The game
            // normally doesn't directly use the graphics index as it sometimes modifies it, but it appears the value initially set
            // in the object data will always match the correct sprite to show, so we can use that.
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

                35 => new ObjSpriteInfo(0, 81, scale: 2), // Big Moo
                36 => new ObjSpriteInfo(1, 0, scale: 2),
                37 => new ObjSpriteInfo(0, 81, scale: 2), // Big Moo

                39 => new ObjSpriteInfo(14, 0, scale: 2), // Big Moo with shield

                112 => new ObjSpriteInfo(11, 149),
                137 => new ObjSpriteInfo(11, 149, scale: 2),
                _ => new ObjSpriteInfo(-1, -1)
            };
        }

        public ObjSpriteInfo GetSprite_Collectible(CollectibleObject obj)
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

        public async UniTask LoadFilesAsync(Context context, KlonoaSettings_DTP config)
        {
            // The game only loads portions of the BIN at a time
            await context.AddLinearFileAsync(config.FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(config.FilePath_IDX, config.Address_IDX);

            // The exe has to be loaded to read certain data from it
            await context.AddMemoryMappedFile(config.FilePath_EXE, config.Address_EXE, memoryMappedPriority: 0); // Give lower prio to prioritize IDX
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