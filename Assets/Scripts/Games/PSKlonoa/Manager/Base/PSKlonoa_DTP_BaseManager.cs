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

        private static bool IncludeDebugInfo => FileSystem.mode != FileSystem.Mode.Web && Settings.ShowDebugInfo;

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
            ("Final Vision & Nahatomb 1", 2), // TODO: 1 in proto
            ("Final Vision & Nahatomb 2", 3),
            ("Final Vision & Nahatomb 3", 3),

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
                    var tim = wldMap.SpriteSheets.Files[i];

                    exportTex(
                        getTex: () => GetTexture(tim),
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
                        var tim = tims.Files[j];
                        exportTex(
                            getTex: () => GetTexture(tim, onlyFirstTransparent: true),
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
                        var tim = timArchive.Files[i];
                        exportTex(
                            getTex: () => GetTexture(tim),
                            blockName: $"{blockIndex} - {loader.IDX.Entries[blockIndex].LoadCommands[fileIndex].FILE_Type.ToString().Replace("Archive_TIM_", String.Empty)}",
                            name: $"{fileIndex} - {i}");
                    }
                }

                // GAME OBJECT TEXTURES
                if (loader.LevelData3D != null)
                {
                    for (int sectorIndex = 0; sectorIndex < loader.LevelData3D.SectorGameObjects3D.Length; sectorIndex++)
                    {
                        var sectorGameObjects = loader.LevelData3D.SectorGameObjects3D[sectorIndex].Objects;

                        for (int objIndex = 0; objIndex < sectorGameObjects.Length; objIndex++)
                        {
                            var obj = sectorGameObjects[objIndex];

                            if (obj.Data_TIM != null)
                            {
                                exportTex(
                                    getTex: () => GetTexture(obj.Data_TIM),
                                    blockName: $"{blockIndex} - GameObjectTextures",
                                    name: $"{sectorIndex} - {objIndex} - Texture");
                            }
                            
                            if (obj.Data_TIMArchive != null)
                            {
                                var textures = obj.Data_TIMArchive;

                                for (int texIndex = 0; texIndex < textures.Files.Length; texIndex++)
                                {
                                    exportTex(
                                        getTex: () => GetTexture(textures.Files[texIndex]),
                                        blockName: $"{blockIndex} - GameObjectTextures",
                                        name: $"{sectorIndex} - {objIndex} - Textures - {texIndex}");
                                }
                            }

                            if (obj.Data_TextureAnimation != null)
                            {
                                var texAnim = obj.Data_TextureAnimation;

                                for (int texIndex = 0; texIndex < texAnim.Files.Length; texIndex++)
                                {
                                    exportTex(
                                        getTex: () => GetTexture(texAnim.Files[texIndex]),
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
                        getTex: () => GetTexture(tim, noPal: true),
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
                        var objectsLoader = new PSKlonoa_DTP_GameObjectsLoader(this, loader, 0, null, new GameObject3D[0], objects);

                        // Load to get the backgrounds with their animations
                        await objectsLoader.LoadAsync();
                        await objectsLoader.Anim_Manager.LoadTexturesAsync(loader.VRAM);

                        // Export every layer
                        foreach (PSKlonoa_DTP_GameObjectsLoader.BackgroundLayer layer in objectsLoader.BG_Layers)
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

                        var tex = GetTexture(loader, bgPack, new BackgroundGameObject()
                        {
                            Type = BackgroundGameObject.BackgroundGameObjectType.BackgroundLayer_19,
                            BGDIndex = bgdIndex,
                            CELIndex = 0,
                        });

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
                                    return GetTexture(file.TIM, palette: pal);
                                else
                                    return GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height,
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
                        getTex: () => GetTexture(imgData: cutscenePack.CharacterNamesImgData.Data, pal: null, width: 0x0C, height: 0x50, colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4), 
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
                            getTex: () => GetTexture(file.ImgData, playerPal, file.Width, file.Height,
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

                        var isPlayerAnim = (cutscenePack.Cutscenes.SelectMany(x => x.Cutscene_Normal.Instructions).FirstOrDefault(x => x.Type == CutsceneInstruction.InstructionType.SetObjAnimation && ((CutsceneInstructionData_SetObjAnimation)x.Data).AnimIndex == i)?.Data as CutsceneInstructionData_SetObjAnimation)?.ObjIndex == 0;

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

                PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), loader.VRAM);
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

            Load_MovementPaths(level, loader, sector, scale);

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
            var objectsLoader = new PSKlonoa_DTP_GameObjectsLoader(this, loader, scale, gao_3dObjParent, loader.LevelData3D.SectorGameObjects3D[sector].Objects, loader.BackgroundPack.BackgroundGameObjectsFiles.Files.ElementAtOrDefault(sector)?.Objects);
            await objectsLoader.LoadAsync();

            if (IncludeDebugInfo)
            {
                Debug.Log($"TEXTURE ANIMATIONS:{Environment.NewLine}" +
                          $"{String.Join(Environment.NewLine, objectsLoader.Anim_TextureAnimations.Select(a => $"{(a.UsesSingleRegion ? a.Region.ToString() : String.Join(", ", a.Regions))}"))}");
                Debug.Log($"PALETTE ANIMATIONS:{Environment.NewLine}" +
                          $"{String.Join(Environment.NewLine, objectsLoader.Anim_PaletteAnimations.Select(a => $"{(a.UsesSingleRegion ? a.Region.ToString() : String.Join(", ", a.Regions))}"))}");
            }

            // Load tracks

            Controller.DetailedState = "Loading camera tracks";
            await Controller.WaitIfNecessary();

            // TODO: Add cutscene tracks too
            level.TrackManagers = objectsLoader.GameObj_CameraAnimations.Select(x => new Unity_TrackManager_PSKlonoaDTP(x, scale)).ToArray();

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
            layers.Add(new Unity_Layer_GameObject(true, isAnimated: objectsLoader.GameObj_IsAnimated)
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
                          $"{objectsLoader.Anim_ScrollAnimations.Count} UV scroll animations{Environment.NewLine}" +
                          $"Objects:{Environment.NewLine}\t" +
                          $"{String.Join($"{Environment.NewLine}\t", objectsLoader.GameObjects3D.Take(objectsLoader.GameObjects3D.Length - 1).Select(x => $"{x.Offset}: {(int)x.PrimaryType:00}-{x.SecondaryType:00} ({x.GlobalGameObjectType})"))}");
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

        public IEnumerable<Unity_Layer> Load_Layers_Backgrounds(Loader loader, PSKlonoa_DTP_GameObjectsLoader objectsLoader, float scale)
        {
            // Get the background textures
            var bgLayers = objectsLoader.BG_Layers;
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

        public Unity_Layer Load_Layers_LevelObject(Loader loader, GameObject parent, PSKlonoa_DTP_GameObjectsLoader objectsLoader, int sector, float scale)
        {
            GameObject obj;
            bool isAnimated;

            (obj, isAnimated) = CreateGameObject(
                tmd: loader.LevelPack.Sectors[sector].LevelModel, 
                loader: loader, 
                scale: scale, 
                name: "Map", 
                objectsLoader: objectsLoader, 
                isPrimaryObj: true);

            var levelBounds = PSKlonoaHelpers.GetDimensions(loader.LevelPack.Sectors[sector].LevelModel, scale);

            // Calculate actual level dimensions: switched axes for unity & multiplied by cellSize
            var cellSize = 16;
            //var layerDimensions = new Vector3(size.x, size.z, size.y) * cellSize;
            var layerDimensions = new Rect(
                levelBounds.min.x * cellSize, -levelBounds.max.z * cellSize,
                levelBounds.size.x * cellSize, levelBounds.size.z * cellSize);
            // Correctly center object
            //obj.transform.position = new Vector3(-levelBounds.min.x, 0, -size.z-levelBounds.min.z);
            
            obj.transform.SetParent(parent.transform, false);

            var collisionObj = CreateCollisionGameObject(loader.LevelPack.Sectors[sector].LevelCollisionTriangles.CollisionTriangles, scale);
            collisionObj.transform.SetParent(Controller.obj.levelController.editor.layerTypes.transform, false);

            return new Unity_Layer_GameObject(true, isAnimated: isAnimated)
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
                    var spriteInfo = PSKlonoaHelpers.GetSprite_Enemy(obj);

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

                    var pos = PSKlonoaHelpers.GetPosition(movementPaths[pathIndex].Blocks, obj.MovementPathSpawnPosition, Vector3.zero, scale);

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
                    pos = PSKlonoaHelpers.GetPosition(x.Position.X.Value, x.Position.Y.Value, x.Position.Z.Value, scale);
                else
                    pos = PSKlonoaHelpers.GetPosition(movementPaths[x.MovementPath].Blocks, x.MovementPathPosition, new Vector3(0, x.Position.Y.Value, 0), scale);

                var spriteInfo = PSKlonoaHelpers.GetSprite_Collectible(x);

                if (spriteInfo.SpriteSet == -1 || spriteInfo.SpriteIndex == -1)
                    Debug.LogWarning($"Sprite could not be determined for collectible object of secondary type {x.SecondaryType}");

                return new Unity_Object_PSKlonoa_DTP_Collectible(objManager, x, pos, spriteInfo);
            }));

            // Add scenery objects
            objects.AddRange(loader.LevelData3D.SectorGameObjects3D[sector].Objects.
                Where(x => x.Data_ScenerySprites != null || x.Data_LightPositions != null).
                SelectMany(x => (x.Data_LightPositions ?? x.Data_ScenerySprites).Vectors[0]).
                Select(x => new Unity_Object_Dummy(x, Unity_ObjectType.Object)
            {
                Position = PSKlonoaHelpers.GetPosition(x.X, x.Y, x.Z, scale),
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

        public void Load_MovementPaths(Unity_Level level, Loader loader, int sector, float scale)
        {
            var lines = new List<Unity_CollisionLine>();
            const float verticalAdjust = 0.2f;
            var up = new Vector3(0, 0, verticalAdjust);

            foreach (var path in loader.LevelPack.Sectors[sector].MovementPaths.Files)
            {
                foreach (var pathBlock in path.Blocks)
                {
                    var origin = PSKlonoaHelpers.GetPosition(pathBlock.XPos, pathBlock.YPos, pathBlock.ZPos, scale)
                                 + up;
                    var end = PSKlonoaHelpers.GetPosition(
                                  x: pathBlock.XPos + pathBlock.DirectionX * pathBlock.BlockLength, 
                                  y: pathBlock.YPos + pathBlock.DirectionY * pathBlock.BlockLength, 
                                  z: pathBlock.ZPos + pathBlock.DirectionZ * pathBlock.BlockLength, 
                                  scale: scale)
                              + up;

                    lines.Add(new Unity_CollisionLine(origin, end) { is3D = true, UnityWidth = 0.5f });
                }
            }

            level.CollisionLines = lines.ToArray();
        }

        public GameObject CreateCollisionGameObject(CollisionTriangle[] collisionTriangles, float scale)
        {
            Vector3 toVertex(int x, int y, int z) => new Vector3(x / scale, -y / scale, z / scale);

            var obj = new GameObject("Collision");
            obj.transform.position = Vector3.zero;
            var collidersParent = new GameObject("Collision - Colliders");
            collidersParent.transform.position = Vector3.zero;

            var defaultColor = new Color(88 / 255f, 98 / 255f, 115 / 255f);

            foreach (CollisionTriangle c in collisionTriangles)
            {
                Mesh unityMesh = new Mesh();

                var vertices = new Vector3[]
                {
                    toVertex(c.X1, c.Y1, c.Z1),
                    toVertex(c.X2, c.Y2, c.Z2),
                    toVertex(c.X3, c.Y3, c.Z3),

                    toVertex(c.X1, c.Y1, c.Z1),
                    toVertex(c.X3, c.Y3, c.Z3),
                    toVertex(c.X2, c.Y2, c.Z2),
                };

                unityMesh.SetVertices(vertices);

                var color = new Color(BitHelpers.ExtractBits((int)c.Type, 8, 0) / 255f, BitHelpers.ExtractBits((int)c.Type, 8, 8) / 255f, BitHelpers.ExtractBits((int)c.Type, 8, 16) / 255f);
                unityMesh.SetColors(Enumerable.Repeat(color, vertices.Length).ToArray());

                unityMesh.SetTriangles(Enumerable.Range(0, vertices.Length).ToArray(), 0);

                unityMesh.RecalculateNormals();

                GameObject gao = new GameObject($"Collision Triangle {c.Offset}");

                MeshFilter mf = gao.AddComponent<MeshFilter>();
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                gao.layer = LayerMask.NameToLayer("3D Collision");
                gao.transform.SetParent(obj.transform, false);
                gao.transform.localScale = Vector3.one;
                gao.transform.localPosition = Vector3.zero;
                mf.mesh = unityMesh;

                mr.material = Controller.obj.levelController.controllerTilemap.isometricCollisionMaterial;


                // Add Collider GameObject
                GameObject gaoc = new GameObject($"Collision Triangle {c.Offset} - Collider");
                MeshCollider mc = gaoc.AddComponent<MeshCollider>();
                mc.sharedMesh = unityMesh;
                gaoc.layer = LayerMask.NameToLayer("3D Collision");
                gaoc.transform.SetParent(collidersParent.transform);
                gaoc.transform.localScale = Vector3.one;
                gaoc.transform.localPosition = Vector3.zero;
                var col3D = gaoc.AddComponent<Unity_Collision3DBehaviour>();
                col3D.Type = $"{c.Type:X8}";

            }

            return obj;
        }

        public (GameObject, bool) CreateGameObject(
            PS1_TMD tmd, 
            Loader loader, 
            float scale, 
            string name, 
            PSKlonoa_DTP_GameObjectsLoader objectsLoader, 
            bool isPrimaryObj,
            ModelAnimation_ArchiveFile[] animations = null, 
            AnimSpeed animSpeed = null, 
            AnimLoopMode animLoopMode = AnimLoopMode.Repeat,
            ArchiveFile<ModelBoneAnimation_ArchiveFile> boneAnimations = null)
        {
            bool isAnimated = false;

            GameObject gaoParent = new GameObject(name);
            gaoParent.transform.position = Vector3.zero;

            var vramTextures = new HashSet<PS1VRAMTexture>();
            var vramTexturesLookup = new Dictionary<PS1_TMD_Packet, PS1VRAMTexture>();

            // Get texture bounds
            foreach (PS1_TMD_Packet packet in tmd.Objects.SelectMany(x => x.Primitives).Where(x => x.Mode.TME))
            {
                var tex = new PS1VRAMTexture(packet.TSB, packet.CBA, packet.UV);

                var overlappingTex = vramTextures.FirstOrDefault(x => x.HasOverlap(tex));

                if (isPrimaryObj && packet.UV.Any(x => objectsLoader.Anim_ScrollAnimations.SelectMany(a => a.UVOffsets).Contains((int)(x.Offset.FileOffset - tmd.Objects[0].Offset.FileOffset))))
                {
                    tex.ExpandWithUVScroll();
                }


                if (overlappingTex != null)
                {
                    overlappingTex.ExpandWithBounds(tex);
                    vramTexturesLookup.Add(packet, overlappingTex);
                }
                else
                {
                    vramTextures.Add(tex);
                    vramTexturesLookup.Add(packet, tex);
                }
            }

            // Create textures
            foreach (PS1VRAMTexture vramTex in vramTextures)
            {
                // Create the default texture
                vramTex.SetTexture(vramTex.GetTexture(loader.VRAM));

                // Check if the texture is animated
                var vramAnims = objectsLoader.Anim_GetAnimationsFromRegion(vramTex.TextureRegion, vramTex.PaletteRegion).ToArray();

                if (!vramAnims.Any()) 
                    continue;
                
                var animatedTexture = new PS1VRAMAnimatedTexture(vramTex.Bounds.width, vramTex.Bounds.height, true, tex =>
                {
                    vramTex.GetTexture(loader.VRAM, tex);
                }, vramAnims);
                
                objectsLoader.Anim_Manager.AddAnimatedTexture(animatedTexture);
                vramTex.SetAnimatedTexture(animatedTexture);
            }

            // Create each object
            for (var objIndex = 0; objIndex < tmd.Objects.Length; objIndex++)
            {
                var obj = tmd.Objects[objIndex];

                // Helper methods
                Vector3 toVertex(PS1_TMD_Vertex v) => new Vector3(v.X / scale, -v.Y / scale, v.Z / scale);
                Vector3 toNormal(PS1_TMD_Normal n) => new Vector3(n.X, -n.Y , n.Z);
                int getBoneForVertex(int vertexIndex) {
                    if(obj.Bones == null) return -1;
                    return obj.Bones.FindItemIndex(b => b.VerticesIndex <= vertexIndex && b.VerticesIndex + b.VerticesCount > vertexIndex) + 1;
                }

                GameObject gameObject = new GameObject($"Object_{objIndex} Offset:{obj.Offset}");

                gameObject.transform.SetParent(gaoParent.transform, false);
                gameObject.transform.localScale = Vector3.one;

                // Init bones
                bool hasBones = obj.BonesCount > 0;
                Transform[] bones = null;
                Matrix4x4[] bindPoses = null;
                if (hasBones) {
                    bones = new Transform[obj.Bones.Length + 1];
                    for (int i = 0; i < bones.Length; i++) {
                        var b = new GameObject($"Bone {i}");
                        b.transform.SetParent(gameObject.transform);
                        bones[i] = b.transform;
                    }
                    // Init Root bone
                    {
                        var b = bones[0];
                        b.transform.localPosition = Vector3.zero;
                        b.transform.localRotation = Quaternion.identity;
                        b.transform.localScale = Vector3.one;
                    }
                    // Init other bones
                    for (int i = 0; i < obj.Bones.Length; i++) {
                        var b = bones[i+1];
                        b.transform.SetParent(bones[obj.Bones[i].ParentIndex]);
                        b.transform.localPosition = Vector3.zero;
                        b.transform.localRotation = Quaternion.identity;
                        b.transform.localScale = Vector3.one;
                    }

                    bindPoses = new Matrix4x4[bones.Length];
                    for (int i = 0; i < bindPoses.Length; i++) {
                        bindPoses[i] = bones[i].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;
                    }

                    if (boneAnimations != null)
                    {
                        // TODO: Support multiple animations
                        ModelBoneAnimation_ArchiveFile anim = boneAnimations.Files[10];

                        for (int boneIndex = 0; boneIndex < anim.Rotations.BonesCount; boneIndex++)
                        {
                            short frameCount = anim.Rotations.FramesCount;

                            var animComponent = gameObject.AddComponent<AnimatedTransformComponent>();
                            animComponent.animatedTransform = bones[boneIndex + 1];

                            if (animSpeed != null)
                                animComponent.speed = animSpeed;

                            animComponent.loopMode = animLoopMode;

                            var rotX = anim.Rotations.GetValues(boneIndex * 3 + 0);
                            var rotY = anim.Rotations.GetValues(boneIndex * 3 + 1);
                            var rotZ = anim.Rotations.GetValues(boneIndex * 3 + 2);

                            var positions = anim.Positions.Vectors.
                                Select(x => PSKlonoaHelpers.GetPositionVector(x[boneIndex], Vector3.zero, scale)).
                                ToArray();
                            var rotations = Enumerable.Range(0, frameCount).
                                Select(x => PSKlonoaHelpers.GetQuaternion(rotX[x], rotY[x], rotZ[x])).
                                ToArray();

                            animComponent.frames = new AnimatedTransformComponent.Frame[frameCount];

                            for (int i = 0; i < frameCount; i++)
                            {
                                animComponent.frames[i] = new AnimatedTransformComponent.Frame()
                                {
                                    Position = positions[i],
                                    Rotation = rotations[i],
                                    Scale = Vector3.one,
                                };
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Object has bones but no animation");
                    }
                }

                var isTransformAnimated = ApplyTransform(
                    gameObj: gameObject, 
                    transforms: animations, 
                    scale: scale, 
                    objIndex: objIndex, 
                    animSpeed: animSpeed?.CloneAnimSpeed(), 
                    animLoopMode: animLoopMode);

                if (isTransformAnimated)
                    isAnimated = true;

                // Add each primitive
                for (var packetIndex = 0; packetIndex < obj.Primitives.Length; packetIndex++)
                {
                    var packet = obj.Primitives[packetIndex];

                    //if (!packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.LGT))
                    //    Debug.LogWarning($"Packet has light source");

                    if (packet.Mode.Code != PS1_TMD_PacketMode.PacketModeCODE.Polygon)
                    {
                        if (packet.Mode.Code != 0)
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

                    if (normals == null) 
                        unityMesh.RecalculateNormals();

                    if (hasBones) {
                        BoneWeight[] weights = packet.Vertices.Select(x => new BoneWeight() {
                            boneIndex0 = getBoneForVertex(x),
                            weight0 = 1f
                        }).ToArray();
                        unityMesh.boneWeights = weights;
                        unityMesh.bindposes = bindPoses;
                    }

                    GameObject gao = new GameObject($"Packet_{packetIndex} Offset:{packet.Offset} Flags:{packet.Flags}");

                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    gao.layer = LayerMask.NameToLayer("3D Collision");
                    gao.transform.SetParent(gameObject.transform, false);
                    gao.transform.localScale = Vector3.one;
                    gao.transform.localPosition = Vector3.zero;
                    mf.sharedMesh = unityMesh;

                    Material mat = null;
                    if (packet.Mode.ABE)
                        mat = new Material(Controller.obj.levelController.controllerTilemap.unlitAdditiveMaterial);
                    else
                        mat = new Material(Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial);

                    if (hasBones) {
                        SkinnedMeshRenderer smr = gao.AddComponent<SkinnedMeshRenderer>();
                        smr.sharedMaterial = mat;
                        smr.sharedMesh = unityMesh;
                        smr.bones = bones;
                        smr.rootBone = bones[0];
                    } else {
                        MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                        mr.sharedMaterial = mat;
                    }
                    // Add texture
                    if (packet.Mode.TME)
                    {
                        var tex = vramTexturesLookup[packet];
                        mat.SetTexture("_MainTex", tex.Texture);

                        unityMesh.SetUVs(0, packet.UV.Select((uv, i) =>
                        {
                            var u = uv.U - tex.Bounds.x;
                            var v = uv.V - tex.Bounds.y;
                            if (i % 2 == 1) u += 1;
                            if (i >= 2) v += 1;

                            return new Vector2(u / (float)(tex.Bounds.width), v / (float)(tex.Bounds.height));
                        }).ToArray());

                        if (tex.AnimatedTexture != null)
                        {
                            isAnimated = true;
                            var animTex = gao.AddComponent<AnimatedTextureComponent>();
                            animTex.material = mat;
                            animTex.animatedTextures = tex.AnimatedTexture.Textures;
                            animTex.speed = new AnimSpeed_FrameDelay(tex.AnimatedTexture.Speed);
                        }

                        if (IncludeDebugInfo)
                            gao.name = $"{packet.Offset}: {objIndex}-{packetIndex} TX: {packet.TSB.TX}, TY:{packet.TSB.TY}, F:{packet.Flags}, ABE:{packet.Mode.ABE}, TGE:{packet.Mode.TGE}, ABR: {packet.TSB.ABR}, IsAnimated: {tex.AnimatedTexture != null}";

                        // Check for UV scroll animations
                        if (isPrimaryObj && packet.UV.Any(x => objectsLoader.Anim_ScrollAnimations.SelectMany(a => a.UVOffsets).Contains((int)(x.Offset.FileOffset - tmd.Objects[0].Offset.FileOffset))))
                        {
                            isAnimated = true;
                            var animTex = gao.AddComponent<AnimatedTextureComponent>();
                            animTex.material = mat;
                            animTex.scrollV = -2f * 60f / (tex?.Bounds.height ?? 256);
                        }
                    }
                }
            }

            return (gaoParent, isAnimated);
        }

        public bool ApplyTransform(GameObject gameObj, ModelAnimation_ArchiveFile[] transforms, float scale, int objIndex = 0, AnimSpeed animSpeed = null, AnimLoopMode animLoopMode = AnimLoopMode.Repeat)
        {
            if (transforms?.Any() == true && transforms[0].Positions.Vectors[0].Length == 1)
                objIndex = 0;

            if (transforms != null && transforms.Any() && transforms[0].Positions.ObjectsCount > objIndex)
            {
                gameObj.transform.localPosition = PSKlonoaHelpers.GetPositionVector(transforms[0].Positions.Vectors[0][objIndex], null, scale);
                gameObj.transform.localRotation = PSKlonoaHelpers.GetQuaternion(transforms[0].Rotations.Vectors[0][objIndex]);
            }
            else
            {
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.transform.localRotation = Quaternion.identity;
            }

            if (transforms?.FirstOrDefault()?.Positions.Vectors.Length > 1)
            {
                var mtComponent = gameObj.AddComponent<AnimatedTransformComponent>();
                mtComponent.animatedTransform = gameObj.transform;

                if (animSpeed != null)
                    mtComponent.speed = animSpeed;

                mtComponent.loopMode = animLoopMode;

                var positions = transforms.
                    SelectMany(x => x.Positions.Vectors).
                    Select(x => x.Length > objIndex ? PSKlonoaHelpers.GetPositionVector(x[objIndex], null, scale) : (Vector3?)null).
                    ToArray();
                var rotations = transforms.
                    SelectMany(x => x.Rotations.Vectors).
                    Select(x => x.Length > objIndex ? PSKlonoaHelpers.GetQuaternion(x[objIndex]) : (Quaternion?)null).
                    ToArray();

                var frameCount = Math.Max(positions.Length, rotations.Length);
                mtComponent.frames = new AnimatedTransformComponent.Frame[frameCount];

                for (int i = 0; i < frameCount; i++)
                {
                    mtComponent.frames[i] = new AnimatedTransformComponent.Frame()
                    {
                        Position = positions[i] ?? Vector3.zero,
                        Rotation = rotations[i] ?? Quaternion.identity,
                        Scale = Vector3.one,
                        IsHidden = positions[i] == null || rotations[i] == null,
                    };
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public IDX Load_IDX(Context context, KlonoaSettings_DTP settings)
        {
            return FileFactory.Read<IDX>(settings.FilePath_IDX, context);
        }

        public Texture2D GetTexture(PS1_TIM tim, bool flipTextureY = true, Color[] palette = null, bool onlyFirstTransparent = false, bool noPal = false)
        {
            if (tim.Region.XPos == 0 && tim.Region.YPos == 0)
                return null;

            var pal = noPal ? null : palette ?? tim.Clut?.Palette?.Select(x => x.GetColor()).ToArray();

            if (onlyFirstTransparent && pal != null)
                for (int i = 0; i < pal.Length; i++)
                    pal[i].a = i == 0 ? 0 : 1;

            return GetTexture(tim.ImgData, pal, tim.Region.Width, tim.Region.Height, tim.ColorFormat, flipTextureY);
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
                    PSKlonoaHelpers.FillTextureFromVRAM(
                        tex: tex,
                        vram: vram,
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

        public (Texture2D[], int) GetBackgroundFrames(Loader loader, PSKlonoa_DTP_GameObjectsLoader objectsLoader, BackgroundPack_ArchiveFile bg, BackgroundGameObject layer)
        {
            var celIndex = layer.CELIndex;
            var bgIndex = layer.BGDIndex;

            var tim = bg.TIMFiles.Files[celIndex];
            var cel = bg.CELFiles.Files[celIndex];
            var map = bg.BGDFiles.Files[bgIndex];

            bool is8bit = tim.ColorFormat == PS1_TIM.TIM_ColorFormat.BPP_8;
            int palLength = (is8bit ? 256 : 16) * 2;

            var anims = new HashSet<PS1VRAMAnimation>();

            if (objectsLoader.Anim_BGPaletteAnimations.Any())
            {
                foreach (var clut in map.Map.Select(x => cel.Cells[x]).Select(x => x.ClutX | x.ClutY << 6).Distinct())
                {
                    var region = new RectInt((clut & 0x3F) * 16 * 2, clut >> 6, palLength, 1);

                    foreach (var anim in objectsLoader.Anim_GetBGAnimationsFromRegion(region))
                        anims.Add(anim);

                    if (anims.Count == objectsLoader.Anim_BGPaletteAnimations.Count)
                        break;
                }
            }

            if (!anims.Any())
                return (new Texture2D[]
                {
                    GetTexture(loader, bg, layer)
                }, 0);

            var width = map.MapWidth * map.CellWidth;
            var height = map.MapHeight * map.CellHeight;

            var animatedTex = new PS1VRAMAnimatedTexture(width, height, true, tex =>
            {
                GetTexture(loader, bg, layer, tex);
            }, anims.ToArray());

            objectsLoader.Anim_Manager.AddAnimatedTexture(animatedTex);

            return (animatedTex.Textures, animatedTex.Speed);
        }

        public Texture2D GetTexture(Loader loader, BackgroundPack_ArchiveFile bg, BackgroundGameObject layer, Texture2D tex = null)
        {
            var celIndex = layer.CELIndex;
            var bgIndex = layer.BGDIndex;

            var tim = bg.TIMFiles.Files[celIndex];
            var cel = bg.CELFiles.Files[celIndex];
            var map = bg.BGDFiles.Files[bgIndex];

            tex ??= TextureHelpers.CreateTexture2D(map.MapWidth * map.CellWidth, map.MapHeight * map.CellHeight, clear: true);

            for (int mapY = 0; mapY < map.MapHeight; mapY++)
            {
                for (int mapX = 0; mapX < map.MapWidth; mapX++)
                {
                    var cellIndex = map.Map[mapY * map.MapWidth + mapX];

                    if (cellIndex == 0xFF)
                        continue;

                    var cell = cel.Cells[cellIndex];

                    if (cell.ABE)
                        Debug.LogWarning($"CEL ABE flag is set!");

                    PSKlonoaHelpers.FillTextureFromVRAM(
                        tex: tex,
                        vram: loader.VRAM,
                        width: map.CellWidth,
                        height: map.CellHeight,
                        colorFormat: tim.ColorFormat,
                        texX: mapX * map.CellWidth,
                        texY: mapY * map.CellHeight,
                        clutX: cell.ClutX * 16,
                        clutY: cell.ClutY,
                        texturePageOriginX: tim.Region.XPos,
                        texturePageOriginY: tim.Region.YPos,
                        texturePageOffsetX: cell.XOffset,
                        texturePageOffsetY: cell.YOffset);
                }
            }

            tex.Apply();

            return tex;
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

                                textures[i] = GetTexture(playerSprite.ImgData, playerPalette, playerSprite.Width,
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

        public async UniTask LoadFilesAsync(Context context, KlonoaSettings_DTP config)
        {
            // The game only loads portions of the BIN at a time
            await context.AddLinearFileAsync(config.FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(config.FilePath_IDX, config.Address_IDX);

            // The exe has to be loaded to read certain data from it
            await context.AddMemoryMappedFile(config.FilePath_EXE, config.Address_EXE, memoryMappedPriority: 0); // Give lower prio to prioritize IDX
        }

        public class PS1VRAMTexture
        {
            public PS1VRAMTexture(PS1_TSB tsb, PS1_CBA cba, PS1_TMD_UV[] uvs)
            {
                TSB = tsb;
                CBA = cba;

                int xMin = uvs.Min(x => x.U);
                int xMax = uvs.Max(x => x.U) + 1;
                int yMin = uvs.Min(x => x.V);
                int yMax = uvs.Max(x => x.V) + 1;
                int w = xMax - xMin;
                int h = yMax - yMin;

                Bounds = new RectInt(xMin, yMin, w, h);

                bool is8bit = TSB.TP == PS1_TSB.TexturePageTP.CLUT_8Bit;
                TextureRegion = new RectInt(TSB.TX * PS1_VRAM.PageWidth + Bounds.x / (is8bit ? 1 : 2), TSB.TY * PS1_VRAM.PageHeight + Bounds.y, Bounds.width / (is8bit ? 1 : 2), Bounds.height);

                int palLength = (is8bit ? 256 : 16) * 2;
                PaletteRegion = new RectInt(CBA.ClutX * 2 * 16, CBA.ClutY, palLength, 1);
            }

            public PS1_TSB TSB { get; }
            public PS1_CBA CBA { get; }
            public RectInt Bounds { get; protected set; }
            public RectInt TextureRegion { get; }
            public RectInt PaletteRegion { get; }
            public Texture2D Texture { get; protected set; }
            public PS1VRAMAnimatedTexture AnimatedTexture { get; protected set; }

            public bool HasOverlap(PS1VRAMTexture b)
            {
                if (b.TSB.TP != TSB.TP ||
                    b.TSB.TX != TSB.TX ||
                    b.TSB.TY != TSB.TY ||
                    b.CBA.ClutX != CBA.ClutX ||
                    b.CBA.ClutY != CBA.ClutY)
                    return false;

                return Bounds.Overlaps(b.Bounds);
            }

            public Texture2D GetTexture(PS1_VRAM vram, Texture2D tex = null)
            {
                PS1_TIM.TIM_ColorFormat colFormat = TSB.TP switch
                {
                    PS1_TSB.TexturePageTP.CLUT_4Bit => PS1_TIM.TIM_ColorFormat.BPP_4,
                    PS1_TSB.TexturePageTP.CLUT_8Bit => PS1_TIM.TIM_ColorFormat.BPP_8,
                    PS1_TSB.TexturePageTP.Direct_15Bit => PS1_TIM.TIM_ColorFormat.BPP_16,
                    _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {TSB.TP}")
                };

                tex ??= TextureHelpers.CreateTexture2D(Bounds.width, Bounds.height, clear: true);
                tex.wrapMode = TextureWrapMode.Repeat;

                PSKlonoaHelpers.FillTextureFromVRAM(
                    tex: tex,
                    vram: vram,
                    width: Bounds.width,
                    height: Bounds.height,
                    colorFormat: colFormat,
                    texX: 0,
                    texY: 0,
                    clutX: CBA.ClutX * 16,
                    clutY: CBA.ClutY,
                    texturePageX: TSB.TX,
                    texturePageY: TSB.TY,
                    texturePageOffsetX: Bounds.x,
                    texturePageOffsetY: Bounds.y,
                    flipY: true);

                tex.Apply();

                return tex;
            }

            public void SetTexture(Texture2D tex)
            {
                Texture = tex;
            }

            public void SetAnimatedTexture(PS1VRAMAnimatedTexture tex)
            {
                AnimatedTexture = tex;
            }

            public void ExpandWithBounds(PS1VRAMTexture b)
            {
                var minX = Math.Min(Bounds.x, b.Bounds.x);
                var minY = Math.Min(Bounds.y, b.Bounds.y);
                var maxX = Math.Max(Bounds.x + Bounds.width, b.Bounds.x + b.Bounds.width);
                var maxY = Math.Max(Bounds.y + Bounds.height, b.Bounds.y + b.Bounds.height);
                Bounds = new RectInt(
                    minX,
                    minY,
                    maxX - minX,
                    maxY - minY);
            }

            public void ExpandWithUVScroll() {
                Bounds = new RectInt(
                    Bounds.x,
                    0,
                    Bounds.width,
                    192);

            }
        }
    }
}