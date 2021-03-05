using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_Manager : IGameManager
    {
        public string ExeFilePath => @"6rac.app";
        public string DataFilePath => @"data.gob";
        public const uint ExeBaseAddress = 0x10000000 - 648;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[0];

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)),
            new GameAction("Export Blocks (with filenames)", false, true, (input, output) => ExportBlocksAsync(settings, output, withFilenames: true)),
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
            new GameAction("Export textures", false, true, (input, output) => ExportTexturesAsync(settings, output)),
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputDir, bool withFilenames = false)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                DoAtBlocks(context, (s, i, offset) =>
                {
                    // Serialize the bytes
                    var bytes = s.SerializeArray<byte>(default, s.CurrentLength, name: $"Block[{i}]");

                    // Export the data
                    Util.ByteArrayToFile(Path.Combine(outputDir, GetBlockExportName(context, i, offset.CRC, withFilenames, true)), bytes);
                });
            }
        }

        public async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                DoAtBlocks(context, (s, i, offset) =>
                {
                    try
                    {
                        // Attempt to parse block as an AnimSet
                        var a = s.SerializeObject<GBAVV_Map2D_AnimSet>(default);

                        // Enumerate every animation
                        for (int j = 0; j < a.AnimationsCount; j++)
                        {
                            var frames = GetAnimFrames(a, j);

                            Util.ExportAnim(
                                frames: frames,
                                speed: a.Animations[j].GetAnimSpeed,
                                center: false,
                                saveAsGif: saveAsGif,
                                outputDir: outputDir,
                                primaryName: $"{i}",
                                secondaryName: $"{j}");
                        }

                    }
                    catch
                    {
                        // Ignore any exceptions
                    }
                });
            }
        }

        public async UniTask ExportTexturesAsync(GameSettings settings, string outputDir)
        {
            Color[] pal = null;

            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                DoAtBlocks(context, (s, i, offset) =>
                {
                    // If the block is 64 bytes long we assume it's a palette
                    if (s.CurrentLength == 64)
                    {
                        pal = Util.ConvertGBAPalette(s.SerializeObjectArray<RGBA5551Color>(default, 32, name: $"Pal[{i}]"), transparentIndex: null);
                    }
                    else
                    {
                        // If we serialized a palette in the previous block we assume this block has the textures
                        if (pal != null && s.CurrentLength % 0x1500 == 0)
                        {
                            var textures = s.SerializeObject<GBAVV_NitroKart_NGage_TexturesCollection>(default, name: $"TexturesCollection[{i}]");

                            for (int j = 0; j < textures.Textures.Length; j++)
                            {
                                var texture = textures.Textures[j];
                                var tex = TextureHelpers.CreateTexture2D(64, 64);

                                for (int y = 0; y < 64; y++)
                                {
                                    for (int x = 0; x < 64; x++)
                                    {
                                        var off = y * 64 + x;
                                        var palIndex = texture.Texture_64px[off] / 2;

                                        // TODO: Fix the textures where the index is too high
                                        if (palIndex >= pal.Length)
                                        {
                                            Debug.LogWarning($"Out of bounds palette index {palIndex} in texture {i}-{j} at offset {off}");
                                            tex.SetPixel(x, 64 - y - 1, Color.clear);
                                        }
                                        else
                                        {
                                            tex.SetPixel(x, 64 - y - 1, pal[palIndex]);
                                        }
                                    }
                                }

                                tex.Apply();

                                Util.ByteArrayToFile(Path.Combine(outputDir, $"{i} - {j}.png"), tex.EncodeToPNG());
                            }
                        }

                        // Remove palette
                        pal = null;
                    }
                });
            }
        }

        public string GetBlockExportName(Context context, int i, uint crc, bool withFilenames, bool includeFileExtension)
        {
            // Get the CRC for every string
            var stringCrc = GetStringCRCs(context);

            var fileName = withFilenames && (stringCrc?.ContainsKey(crc) ?? false) ? stringCrc[crc] : $"{i}.dat";

            if (!includeFileExtension)
                return Path.GetFileNameWithoutExtension(fileName);
            else
                return fileName;
        }
        public void DoAtBlocks(Context context, Action<SerializerObject, int, GBAVV_NitroKart_NGage_DataFileEntry> action)
        {
            var s = context.Deserializer;
            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            // Enumerate every block in the offset table
            for (int i = 0; i < data.BlocksCount; i++)
            {
                DoAtBlock(context, i, () =>
                {
                    action(s, i, data.DataFileEntries[i]);
                    s.Goto(s.CurrentPointer.file.StartPointer + s.CurrentLength);
                });
            }
        }
        public void DoAtBlock(Context context, string filePath, Action action)
        {
            // Load the polynomial data from the exe
            var polynomialData = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context).CRCPolynomialData;

            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            var crc = CalculateCRC(filePath, polynomialData);

            var blockIndex = data.DataFileEntries.FindItemIndex(x => x.CRC == crc);

            if (blockIndex == -1)
                throw new Exception($"File {filePath} could not be found! CRC: {crc}");

            DoAtBlock(context, blockIndex, action);
        }
        public void DoAtBlock(Context context, int i, Action action)
        {
            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            var s = context.Deserializer;

            // Get the offset
            var offset = data.DataFileEntries[i];

            // Do action at block, encoded
            s.DoAt(offset.BlockPointer, () => s.DoEncoded(new BriefLZEncoder(), action, allowLocalPointers: true));
        }

        public Texture2D[] GetAnimFrames(GBAVV_Map2D_AnimSet animSet, int animIndex)
        {
            // Get properties
            var anim = animSet.Animations[animIndex];
            var frames = anim.FrameIndexTable.Select(x => animSet.AnimationFrames[x]).ToArray();
            var pal = Util.ConvertGBAPalette(anim.Palette);

            // Return empty animation if there are no frames
            if (!frames.Any())
                return new Texture2D[0];

            var output = new Texture2D[frames.Length];

            var minX = animSet.GetMinX(animIndex);
            var minY = animSet.GetMinY(animIndex);
            var maxX = frames.Select(x => x.RenderBox.X + x.NitroKart_NGage_Width).Max();
            var maxY = frames.Select(x => x.RenderBox.Y + x.NitroKart_NGage_Height).Max();

            var width = maxX - minX;
            var height = maxY - minY;

            var frameCache = new Dictionary<GBAVV_Map2D_AnimationFrame, Texture2D>();

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                var frame = frames[frameIndex];

                if (!frameCache.ContainsKey(frame))
                {
                    var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                    var offsetX = frame.RenderBox.X - minX;
                    var offsetY = frame.RenderBox.Y - minY;

                    for (int y = 0; y < frame.NitroKart_NGage_Height; y++)
                    {
                        for (int x = 0; x < frame.NitroKart_NGage_Width; x++)
                        {
                            tex.SetPixel(offsetX + x, height - (offsetY + y) - 1, pal[frame.NitroKart_NGage_ImageData[y * frame.NitroKart_NGage_Width + x]]);
                        }
                    }

                    tex.Apply();

                    frameCache.Add(frame, tex);
                }

                output[frameIndex] = frameCache[frame];
            }

            return output;
        }

        public uint CalculateCRC(string str, uint[] polynomialData) 
        {
            // Normalize the file path and get the bytes
            byte[] buffer = Encoding.ASCII.GetBytes(str.ToUpper().Replace('\\', '/'));

            uint crc = 0;

            foreach (var b in buffer)
                crc = polynomialData[(crc ^ b) & 0xFF] ^ (crc >> 8);

            return crc;
        }
        public Dictionary<uint, string> GetStringCRCs(Context context) 
        {
            var polynomialData = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context).CRCPolynomialData;

            const string id = "StringCRCs";

            if (context.GetStoredObject<Dictionary<uint, string>>(id) == null)
                context.StoreObject(id, FilePaths.ToDictionary(x => CalculateCRC(x, polynomialData)));

            return context.GetStoredObject<Dictionary<uint, string>>(id);
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Load the exe
            var exe = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context);

            Debug.Log(CalculateCRC("snd/music.gax", exe.CRCPolynomialData) + " - " + 454916686);
            Debug.Log(CalculateCRC("snd/fx.gax", exe.CRCPolynomialData) + " - " + 3101288717);

            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context)
        {
            await context.AddMemoryMappedFile(ExeFilePath, ExeBaseAddress);
            await context.AddLinearSerializedFileAsync(DataFilePath);
        }

        public string[] FilePaths => new string[]
        {
            @"gfx\projectiles.gfx",
            @"gfx\effects.gfx",
            @"gfx\objects.gfx",
            @"gfx\bananas.gfx",
            @"gfx\HUD.gfx",
            @"gfx\hubWorld.gfx",
            @"gfx\norm.gfx",
            @"gfx\fakeCrash.gfx",
            @"gfx\polar.gfx",
            @"gfx\crash.gfx",
            @"gfx\nash.gfx",
            @"gfx\nTrance.gfx",
            @"gfx\tiny.gfx",
            @"gfx\UI.gfx",
            @"gfx\coco.gfx",
            @"gfx\crunch.gfx",
            @"gfx\dingodile.gfx",
            @"gfx\kongo.gfx",
            @"gfx\nCortex.gfx",
            @"gfx\nGin.gfx",
            @"gfx\nOxide.gfx",
            @"gfx\nTropy.gfx",
            @"gfx\otto.gfx",
            @"gfx\uiLayerA.tex",
            @"gfx\velo.gfx",
            @"flc\nGageLogo.flc",
            @"flc\nGageLogoFrench.flc",
            @"flc\nGageLogoGerman.flc",
            @"flc\nGageLogoItalian.flc",
            @"flc\nGageLogoSpanish.flc",
            @"gfx\dancemove02.gfx",
            @"gfx\dancemove04.gfx",
            @"gfx\dancemove05.gfx",
            @"snd\music.gax",
            @"snd\fx.gax",
            @"gfx\placeholder.tex",
            @"gfx\placeholder.pal",
            @"gfx\uiLayerA.pal",
            @"gfx\cnkaFont.fni",
            @"gfx\cnkaFont.fnc",
            @"gfx\cnkaMonoFont.fnh",
            @"gfx\cnkaMonoFont.fni",
            @"gfx\cnkaMonoFont.fnc",
            @"gfx\cnkaSmallFont.fnh",
            @"gfx\cnkaSmallFont.fni",
            @"gfx\cnkaSmallFont.fnc",
            @"gfx\cnkaSmallMonoFont.fnh",
            @"gfx\cnkaSmallMonoFont.fni",
            @"gfx\cnkaSmallMonoFont.fnc",
            @"gfx\debugFont.fnh",
            @"gfx\debugFont.fni",
            @"gfx\debugFont.fnc",
            @"gfx\smallFont.fnh",
            @"gfx\smallFont.fni",
            @"gfx\smallFont.fnc",
            @"gfx\smallMonoFont.fnh",
            @"gfx\smallMonoFont.fni",
            @"gfx\smallMonoFont.fnc",
            @"gfx\passwordFont.fnh",
            @"gfx\passwordFont.fni",
            @"gfx\passwordFont.fnc",
            @"gfx\terraHub.pvs",
            @"game\terraHub.pop",
            @"gfx\terra01.pvs",
            @"game\terra01.pop",
            @"gfx\terra02.pvs",
            @"game\terra02.pop",
            @"gfx\terra03.pvs",
            @"game\terra03.pop",
            @"gfx\barinHub.pvs",
            @"game\barinHub.pop",
            @"gfx\barin01.pvs",
            @"game\barin01.pop",
            @"gfx\barin02.pvs",
            @"game\barin02.pop",
            @"gfx\barin03.pvs",
            @"game\barin03.pop",
            @"gfx\fenomHub.pvs",
            @"game\fenomHub.pop",
            @"gfx\fenom01.pvs",
            @"game\fenom01.pop",
            @"gfx\fenom02.pvs",
            @"game\fenom02.pop",
            @"gfx\fenom03.pvs",
            @"game\fenom03.pop",
            @"gfx\tekneeHub.pvs",
            @"game\tekneeHub.pop",
            @"gfx\teknee01.pvs",
            @"game\teknee01.pop",
            @"gfx\teknee02.pvs",
            @"game\teknee02.pop",
            @"gfx\teknee03.pvs",
            @"game\teknee03.pop",
            @"gfx\citadelHub.pvs",
            @"game\citadelHub.pop",
            @"gfx\veloBoss.pvs",
            @"game\veloBoss.pop",
            @"gfx\crystalArena01.pvs",
            @"game\crystalArena01.pop",
            @"gfx\crystalArena02.pvs",
            @"game\crystalArena02.pop",
            @"gfx\crystalArena03.pvs",
            @"game\crystalArena03.pop",
            @"gfx\crystalArena04.pvs",
            @"game\crystalArena04.pop",
            @"gfx\battleArena05.pvs",
            @"game\battleArena05.pop",
            @"gfx\battleArena06.pvs",
            @"game\battleArena06.pop",
            @"gfx\battleArena07.pvs",
            @"game\battleArena07.pop",
            @"gfx\battleArena08.pvs",
            @"game\battleArena08.pop",
            @"flc/vuLogo.flc",
            @"flc/vvLogo.flc",
            @"flc/cnkLogo.flc",
            @"flc/garageBkgd.flc",
            @"flc/intro10.flc",
            @"flc/intro01.flc",
            @"flc/intro02.flc",
            @"flc/intro03.flc",
            @"flc/intro04.flc",
            @"flc/intro05.flc",
            @"flc/intro06.flc",
            @"flc/intro07.flc",
            @"flc/intro08.flc",
            @"flc/intro09.flc",
            @"flc/earthBossIntro01.flc",
            @"flc/earthBossIntro02.flc",
            @"flc/earthBossIntro03.flc",
            @"flc/earthBossCrashWin01.flc",
            @"flc/earthBossCrashWin02.flc",
            @"flc/earthBossCrashWin03.flc",
            @"flc/earthBossCrashWin04.flc",
            @"flc/earthBossEvilWin02.flc",
            @"flc/earthBossEvilWin03.flc",
            @"flc/barinBossIntro02.flc",
            @"flc/barinBossIntro03.flc",
            @"flc/barinBossCrashWin01.flc",
            @"flc/barinBossCrashWin02.flc",
            @"flc/barinBossCrashWin03.flc",
            @"flc/barinBossCrashWin04.flc",
            @"flc/barinBossEvilWin01.flc",
            @"flc/barinBossEvilWin02.flc",
            @"flc/fenomBossIntro02.flc",
            @"flc/fenomBossIntro03.flc",
            @"flc/fenomBossIntro04.flc",
            @"flc/fenomBossCrashWin01.flc",
            @"flc/fenomBossCrashWin02.flc",
            @"flc/fenomBossCrashWin03.flc",
            @"flc/fenomBossEvilWin01.flc",
            @"flc/fenomBossEvilWin02.flc",
            @"flc/tekneeBossIntro01.flc",
            @"flc/tekneeBossIntro02.flc",
            @"flc/tekneeBossCrashWin01.flc",
            @"flc/tekneeBossCrashWin02.flc",
            @"flc/tekneeBossCrashWin03.flc",
            @"flc/tekneeBossEvilWin02.flc",
            @"flc/crashEndWin02.flc",
            @"flc/crashEndWin01.flc",
            @"flc/crashEndWin04.flc",
            @"flc/crashEndWin03.flc",
            @"flc/evilEndWin01.flc",
            @"flc/evilEndWin02.flc",
            @"flc/evilEndWin03.flc",
            @"flc/evilEndWin04.flc",
            @"podium.s3d",
            @"warp.s3d",
        };
    }
}