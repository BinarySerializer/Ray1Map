using BinarySerializer;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.KlonoaDTP;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class PS1Klonoa_Manager : BaseGameManager
    {
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

            var loader = PS1Klonoa_Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Process each BIN file
                loader.ProcessBINFiles(entry, (cmd, i) =>
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
                                var archive = loader.Load_BINFile<PS1Klonoa_ArchiveFile_RawData>(cmd, blockIndex, i);

                                for (int j = 0; j < archive.Files.Length; j++)
                                {
                                    var file = archive.Files[j];

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"{j}{ext}"), file.Data);
                                }
                            }
                            else if (archiveDepth == 2)
                            {
                                var archives = loader.Load_BINFile<PS1Klonoa_ArchiveFile<PS1Klonoa_ArchiveFile_RawData>>(cmd, blockIndex, i);

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

            var loader = PS1Klonoa_Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Process each BIN file
                loader.ProcessBINFiles(entry, (cmd, i) =>
                {
                    // Check the file type
                    if (cmd.FILE_Type == PS1Klonoa_IDXLoadCommand.FileType.Archive_TIM)
                    {
                        // Read the data
                        PS1Klonoa_ArchiveFile_TIM timFiles = loader.Load_BINFile<PS1Klonoa_ArchiveFile_TIM>(cmd, blockIndex, i);

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
                        PS1Klonoa_ArchiveFile_LevelSpritePack spritePack = loader.Load_BINFile<PS1Klonoa_ArchiveFile_LevelSpritePack>(cmd, blockIndex, i);

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

            var loader = PS1Klonoa_Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Load the BIN
                loader.Load_BIN(entry, blockIndex);

                // Enumerate every set of frames
                for (int framesSet = 0; framesSet < loader.SpriteFrames.Length; framesSet++)
                {
                    var spriteFrames = loader.SpriteFrames[framesSet];

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
                                    vram: loader.VRAM,
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
                    PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), loader.VRAM);
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

            var loader = PS1Klonoa_Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                var vram = new PS1_VRAM();

                // Process each BIN file
                loader.ProcessBINFiles(entry, (cmd, i) =>
                {
                    try
                    {
                        // Check the file type
                        if (cmd.FILE_Type != PS1Klonoa_IDXLoadCommand.FileType.Archive_BackgroundPack)
                            return;

                        // Read the data
                        var bg = loader.Load_BINFile<PS1Klonoa_ArchiveFile_BackgroundPack>(cmd, blockIndex, i);

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

                            loader.AddToVRAM(tim);

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

            s.Goto(context.GetFile(PS1Klonoa_Loader.FilePath_BIN).StartPointer);

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

            // Create the loader
            var loader = PS1Klonoa_Loader.Create(context);

            // TODO: Load fixed first

            // Load the BIN
            loader.Load_BIN(entry, settings.Level);

            throw new NotImplementedException();
        }

        public PS1Klonoa_IDX Load_IDX(Context context)
        {
            return FileFactory.Read<PS1Klonoa_IDX>(PS1Klonoa_Loader.FilePath_IDX, context);
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
            await context.AddLinearSerializedFileAsync(PS1Klonoa_Loader.FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(PS1Klonoa_Loader.FilePath_IDX, 0x80010000);
        }
    }
}