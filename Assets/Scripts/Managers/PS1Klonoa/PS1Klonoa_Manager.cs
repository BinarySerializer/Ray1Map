using BinarySerializer;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class PS1Klonoa_Manager : BaseGameManager
    {
        public const string FilePath_BIN = "FILE.BIN";
        public const string FilePath_IDX = "FILE.IDX";

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 26).ToArray())
        });

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Extract BIN", false, true, (input, output) => Extract_BINAsync(settings, output, false)),
                new GameAction("Extract BIN (unpack archives)", false, true, (input, output) => Extract_BINAsync(settings, output, true)),
                new GameAction("Extract TIM", false, true, (input, output) => Extract_TIMAsync(settings, output)),
                new GameAction("Extract Backgrounds", false, true, (input, output) => Extract_BackgroundsAsync(settings, output)),
                new GameAction("Extract Sprites", false, true, (input, output) => Extract_SpriteFramesAsync(settings, output)),
                new GameAction("Extract ULZ blocks", false, true, (input, output) => Extract_ULZAsync(settings, output)),
            };
        }

        public async UniTask Extract_BINAsync(GameSettings settings, string outputPath, bool unpack)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var s = context.Deserializer;
            
            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Process each BIN file
                ProcessBINFiles(context, entry, (cmd, i) =>
                {
                    var ext = PS1Klonoa_IDXLoadCommand.FileExtensions[cmd.FILE_Type];

                    if (unpack)
                    {
                        var type = cmd.FILE_Type;
                        var archiveDepth = PS1Klonoa_IDXLoadCommand.ArchiveDepths[type];

                        if (archiveDepth > 0)
                        {
                            // Be lazy and hard-code instead of making some recursive loop
                            if (archiveDepth == 1)
                            {
                                var archive = Load_BINFile<PS1Klonoa_ArchiveFile_RawData>(context, cmd, blockIndex, i);

                                for (int j = 0; j < archive.Files.Length; j++)
                                {
                                    var file = archive.Files[j];

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"{j}{ext}"), file.Data);
                                }
                            }
                            else if (archiveDepth == 2)
                            {
                                var archives = Load_BINFile<PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile_RawData>>(context, cmd, blockIndex, i);

                                for (int a = 0; a < archives.Files.Length; a++)
                                {
                                    var archive = archives.Files[a];

                                    for (int j = 0; j < archive.Files.Length; j++)
                                    {
                                        var file = archive.Files[j];

                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"{a}_{j}{ext}"), file.Data);
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

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"DATA.{ext}"), data);
                });
            }
        }

        public async UniTask Extract_TIMAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Process each BIN file
                ProcessBINFiles(context, entry, (cmd, i) =>
                {
                    // Check the file type
                    if (cmd.FILE_Type == PS1Klonoa_IDXLoadCommand.FileType.Archive_TIM)
                    {
                        // Read the data
                        PS1Klonoa_ArchiveFile_TIM timFiles = Load_BINFile<PS1Klonoa_ArchiveFile_TIM>(context, cmd, blockIndex, i);

                        var index = 0;

                        foreach (var tim in timFiles.Files)
                        {
                            try
                            {
                                var tex = GetTexture(tim);

                                if (tex != null)
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} - {index}.png"),
                                        tex.EncodeToPNG());
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Error exporting with ex: {ex}");
                            }

                            index++;
                        }
                    }
                    else if (cmd.FILE_Type == PS1Klonoa_IDXLoadCommand.FileType.Archive_SpritePack)
                    {
                        // Read the data
                        PS1Klonoa_ArchiveFile_LevelSpritePack spritePack = Load_BINFile<PS1Klonoa_ArchiveFile_LevelSpritePack>(context, cmd, blockIndex, i);

                        var exported = new HashSet<PS1Klonoa_File_PlayerSprite>();

                        var index = 0;

                        var pal = spritePack.PlayerSprites.Files.FirstOrDefault(x => x?.TIM?.Clut != null)?.TIM.Clut.Palette.Select(x => x.GetColor()).ToArray();

                        foreach (var file in spritePack.PlayerSprites.Files)
                        {
                            if (file != null && !exported.Contains(file))
                            {
                                exported.Add(file);

                                try
                                {
                                    Texture2D tex;

                                    if (file.TIM != null)
                                        tex = GetTexture(file.TIM, palette: pal);
                                    else
                                        tex = GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height, PS1_TIM.TIM_ColorFormat.BPP_8);

                                    if (tex != null)
                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} - {index} (SpritePack).png"),
                                            tex.EncodeToPNG());
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogWarning($"Error exporting with ex: {ex}");
                                }
                            }

                            index++;
                        }
                    }
                });
            }
        }

        public async UniTask Extract_SpriteFramesAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var data = new GameData();

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Load the BIN
                LoadGameData(context, entry, data, blockIndex);

                // Enumerate every set of frames
                for (int framesSet = 0; framesSet < data.SpriteFrames.Length; framesSet++)
                {
                    var spriteFrames = data.SpriteFrames[framesSet];

                    if (spriteFrames == null)
                        continue;

                    // Enumerate every frame
                    for (int frameIndex = 0; frameIndex < spriteFrames.Files.Length; frameIndex++)
                    {
                        try
                        {
                            var sprites = spriteFrames.Files[frameIndex].Sprites;

                            foreach (var s in sprites)
                            {
                                if (s.FlipX)
                                    s.XPos = (short)(s.XPos - s.Width - 1);
                                if (s.FlipY)
                                    s.YPos = (short)(s.YPos - s.Height - 1);
                            }

                            var minX = sprites.Min(x => x.XPos);
                            var minY = sprites.Min(x => x.YPos);
                            var maxX = sprites.Max(x => x.XPos + x.Width);
                            var maxY = sprites.Max(x => x.YPos + x.Height);

                            var width = maxX - minX;
                            var height = maxY - minY;

                            var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                            foreach (var sprite in sprites)
                            {
                                var texPage = sprite.TexturePage;

                                FillTextureFromVRAM(
                                    tex: tex,
                                    vram: data.VRAM,
                                    width: sprite.Width,
                                    height: sprite.Height,
                                    colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4,
                                    texX: sprite.XPos - minX,
                                    texY: sprite.YPos - minY,
                                    clutX: sprite.PalOffsetX,
                                    clutY: 500 + sprite.PalOffsetY,
                                    texturePageOriginX: 64 * (texPage % 16),
                                    texturePageOriginY: 128 * (texPage / 16), // TODO: Fix this
                                    texturePageOffsetX: sprite.TexturePageOffsetX,
                                    texturePageOffsetY: sprite.TexturePageOffsetY,
                                    flipX: sprite.FlipX,
                                    flipY: sprite.FlipY);
                            }

                            tex.Apply();

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{framesSet} - {frameIndex}.png"), tex.EncodeToPNG());
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Error exporting sprite frame: {ex}");
                        }
                    }
                }

                try
                {
                    PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), data.VRAM);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting VRAM: {ex}");
                }
            }
        }

        public async UniTask Extract_BackgroundsAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                var vram = new PS1_VRAM();

                // Process each BIN file
                ProcessBINFiles(context, entry, (cmd, i) =>
                {
                    try
                    {
                        // Check the file type
                        if (cmd.FILE_Type != PS1Klonoa_IDXLoadCommand.FileType.Archive_BackgroundPack)
                            return;

                        // Read the data
                        var bg = Load_BINFile<PS1Klonoa_ArchiveFile_BackgroundPack>(context, cmd, blockIndex, i);

                        // TODO: Some maps use different textures! How do we find the index? For now export all variants
                        for (int tileSetIndex = 0; tileSetIndex < bg.TIMFiles.Files.Length; tileSetIndex++)
                        {
                            var tim = bg.TIMFiles.Files[tileSetIndex];
                            var cel = bg.CELFiles.Files[tileSetIndex];

                            // The game hard-codes this
                            if (tileSetIndex == 1)
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

                            AddToVRAM(vram, tim);

                            for (int j = 0; j < bg.BGDFiles.Files.Length; j++)
                            {
                                var map = bg.BGDFiles.Files[j];

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
                                            vram: vram, 
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

                                Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - {j}_{tileSetIndex}.png"), tex.EncodeToPNG());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting with ex: {ex}");
                    }
                });

                PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), vram);
            }
        }

        public async UniTask Extract_ULZAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);

            await LoadFilesAsync(context);

            var s = context.Deserializer;

            s.Goto(context.GetFile(FilePath_BIN).StartPointer);

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
                        var bytes = s.DoEncoded(new ULZ_Encoder(), () => s.SerializeArray<byte>(default, s.CurrentLength));

                        Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{offset.AbsoluteOffset:X8}.bin"), bytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error decompressing at {offset} with ex: {ex}");
                    }
                });
            }
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            var settings = context.GetR1Settings();

            Controller.DetailedState = "Loading IDX";
            await Controller.WaitIfNecessary();

            // Load the IDX
            var idxData = Load_IDX(context);

            // Get the entry
            var entry = idxData.Entries[settings.Level];

            Controller.DetailedState = "Loading BIN";
            await Controller.WaitIfNecessary();

            // Create the game data
            var data = new GameData();

            // TODO: Load fixed first (and menu?)

            // Load the game data
            LoadGameData(context, entry, data, settings.Level);

            throw new NotImplementedException();
        }

        public PS1Klonoa_IDX Load_IDX(Context context)
        {
            return FileFactory.Read<PS1Klonoa_IDX>(FilePath_IDX, context);
        }

        public void ProcessBINFiles(Context context, PS1Klonoa_IDXEntry entry, Action<PS1Klonoa_IDXLoadCommand, int> action)
        {
            var s = context.Deserializer;
            var binFile = context.GetFile(FilePath_BIN);

            for (int cmdIndex = 0; cmdIndex < entry.LoadCommands.Length; cmdIndex++)
            {
                var cmd = entry.LoadCommands[cmdIndex];

                if (cmd.Type == 1)
                {
                    s.Goto(binFile.StartPointer + cmd.BIN_Offset);
                }
                else if (cmd.Type == 2)
                {
                    var p = s.CurrentPointer;
                    binFile.AddRegion(p.FileOffset, cmd.FILE_Length, $"File_{cmdIndex}");
                    action(cmd, cmdIndex);
                    s.Goto(p + cmd.FILE_Length);
                }
            }
        }

        public void LoadGameData(Context context, PS1Klonoa_IDXEntry entry, GameData data, int blockIndex)
        {
            data.BackgroundPack = null;
            data.OA05 = null;
            data.LevelPack = null;

            ProcessBINFiles(context, entry, (cmd, i) =>
            {
                // Load the file
                var binFile = Load_BINFile(context, cmd, blockIndex, i);

                switch (binFile)
                {
                    // Copy the TIM files data to VRAM
                    case PS1Klonoa_ArchiveFile_TIM file:
                        
                        foreach (PS1_TIM tim in file.Files)
                            AddToVRAM(data.VRAM, tim);
                        
                        break;

                    // Save for later
                    case PS1Klonoa_File_OA05 file:
                        data.OA05 = file;
                        break;

                    // Save for later and copy the TIM files data to VRAM
                    case PS1Klonoa_ArchiveFile_BackgroundPack file:
                        
                        data.BackgroundPack = file;

                        foreach (PS1_TIM tim in file.TIMFiles.Files)
                            AddToVRAM(data.VRAM, tim);

                        break;

                    case PS1Klonoa_ArchiveFile_Unk0 file:
                        // TODO: Use file
                        break;

                    // The fixed sprites are always the last set of sprite frames
                    case PS1Klonoa_ArchiveFile_Sprites file:
                        data.SpriteFrames[69] = file;
                        break;

                    // Add the level sprite frames
                    case PS1Klonoa_ArchiveFile_LevelSpritePack file:
                        
                        for (int j = 0; j < 69; j++)
                            data.SpriteFrames[j] = file.Sprites[j];
                        
                        break;

                    // Save for later
                    case PS1Klonoa_ArchiveFile_LevelPack file:
                        data.LevelPack = file;
                        break;
                }
            });
        }

        public PS1Klonoa_BaseFile Load_BINFile(Context context, PS1Klonoa_IDXLoadCommand cmd, int blockIndex, int cmdIndex)
        {
            switch (cmd.FILE_Type)
            {
                case PS1Klonoa_IDXLoadCommand.FileType.Archive_TIM:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_TIM>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.OA05:
                    return Load_BINFile<PS1Klonoa_File_OA05>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.SEQ:
                    // Ignore sequenced music data
                    return null;

                case PS1Klonoa_IDXLoadCommand.FileType.Code:
                    // Ignore compiled code
                    return null;

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_BackgroundPack:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_BackgroundPack>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_Unk0:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_Unk0>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.FixedSprites:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_Sprites>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_SpritePack:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_LevelSpritePack>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_LevelPack:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_LevelPack>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_Unk4:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_RawData>(context, cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Unknown:
                default:
                    Debug.LogWarning($"Unsupported file format for file at command {cmdIndex} parsed at 0x{cmd.FILE_FunctionPointer:X8}");
                    return null;
            }
        }

        public T Load_BINFile<T>(Context context, PS1Klonoa_IDXLoadCommand cmd, int blockIndex, int cmdIndex)
            where T : PS1Klonoa_BaseFile, new()
        {
            var s = context.Deserializer;

            return s.SerializeObject<T>(null, x =>
            {
                x.Pre_FileSize = cmd.FILE_Length;
                x.Pre_IsCompressed = false;
            }, name: $"BIN_File_{blockIndex}_{cmdIndex}");
        }

        public void AddToVRAM(PS1_VRAM vram, PS1_TIM tim)
        {
            // Add the palette if available
            if (tim.Clut != null)
                vram.AddPalette(tim.Clut.Palette, 0, 0, tim.Clut.XPos * 2, tim.Clut.YPos, tim.Clut.Width * 2, tim.Clut.Height);
            
            // Add the image data
            if (!(tim.XPos == 0 && tim.YPos == 0) && tim.Width != 0 && tim.Height != 0)
                vram.AddDataAt(0, 0, tim.XPos * 2, tim.YPos, tim.ImgData, tim.Width * 2, tim.Height);
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
                        paletteIndex = vram.GetPixel8(0, 0, texturePageOriginX + texturePageOffsetX + x, texturePageOriginY + texturePageOffsetY + y);
                    }
                    else if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_4)
                    {
                        int actualX = texturePageOriginX + (texturePageOffsetX + x) / 2;
                        paletteIndex = vram.GetPixel8(0, 0, actualX, texturePageOriginY + texturePageOffsetY + y);

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

        public Texture2D GetTexture(PS1_TIM tim, bool flipTextureY = true, Color[] palette = null)
        {
            if (tim.XPos == 0 && tim.YPos == 0)
                return null;

            var pal = palette ?? tim.Clut?.Palette?.Select(x => x.GetColor()).ToArray();

            return GetTexture(tim.ImgData, pal, tim.Width, tim.Height, tim.ColorFormat, flipTextureY);
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

        public override async UniTask LoadFilesAsync(Context context)
        {
            // The game only loads portions of the BIN at a time
            await context.AddLinearSerializedFileAsync(FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(FilePath_IDX, 0x80010000);
        }

        public class GameData
        {
            public GameData()
            {
                VRAM = new PS1_VRAM();
                SpriteFrames = new PS1Klonoa_ArchiveFile_Sprites[70];
            }

            public PS1_VRAM VRAM { get; }
            public PS1Klonoa_ArchiveFile_Sprites[] SpriteFrames { get; }
            public PS1Klonoa_ArchiveFile_BackgroundPack BackgroundPack { get; set; }
            public PS1Klonoa_File_OA05 OA05 { get; set; }
            public PS1Klonoa_ArchiveFile_LevelPack LevelPack { get; set; }
        }
    }
}