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

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 26).ToArray()), 
        });

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
                                primaryName: $"{GetBlockExportName(context, i, offset.CRC, true, false)}",
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
                        pal = Util.ConvertGBAPalette(s.SerializeObject<GBAVV_NitroKart_NGage_PAL>(default, name: $"Pal[{i}]").Palette, transparentIndex: null);
                    }
                    else
                    {
                        // If we serialized a palette in the previous block we assume this block has the textures
                        if (pal != null && s.CurrentLength % 0x1500 == 0)
                        {
                            var textures = s.SerializeObject<GBAVV_NitroKart_NGage_TEX>(default, name: $"TEX[{i}]");

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

                                var t = $"{i}";
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"{GetBlockExportName(context, i, offset.CRC, true, false)}{(textures.Textures.Length > 1 ? $" - {j}" : "")}.png"), tex.EncodeToPNG());
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
                return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
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
                DoAtBlock<object>(context, i, () =>
                {
                    action(s, i, data.DataFileEntries[i]);
                    s.Goto(s.CurrentPointer.file.StartPointer + s.CurrentLength);
                    return default;
                });
            }
        }
        public T DoAtBlock<T>(Context context, string filePath, Func<T> func)
        {
            // Load the polynomial data from the exe
            var polynomialData = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context).CRCPolynomialData;

            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            var crc = CalculateCRC(filePath, polynomialData);

            var blockIndex = data.DataFileEntries.FindItemIndex(x => x.CRC == crc);

            if (blockIndex == -1)
            {
                Debug.LogWarning($"File {filePath} could not be found! CRC: {crc}");
                return default;
            }

            return DoAtBlock(context, blockIndex, func);
        }
        public T DoAtBlock<T>(Context context, int i, Func<T> func)
        {
            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            var s = context.Deserializer;

            // Get the offset
            var offset = data.DataFileEntries[i];

            // Do action at block, encoded
            return s.DoAt(offset.BlockPointer, () =>
            {
                T result = default;
                s.DoEncoded(new BriefLZEncoder(), () => result = func(), allowLocalPointers: true);
                return result;
            });
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
            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            // Load the exe
            var exe = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context);

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
            @"gfx\dancemove07.gfx",
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
            @"gfx\Terra01\TempStartline.tex",
            @"gfx\Terra01\TempStartline.pal",
            @"gfx\Terra02\E2_D_Surface01_m.tex",
            @"gfx\Terra02\E2_D_Surface01_m.pal",
            @"gfx\Terra02\E2_D_Surface04.tex",
            @"gfx\Terra02\E2_D_Surface04.pal",
            @"gfx\Terra02\E2_D_Surface03_M.tex",
            @"gfx\Terra02\E2_D_Surface03_M.pal",
            @"gfx\Terra02\E2_D_Surface02_S.tex",
            @"gfx\Terra02\E2_D_Surface02_S.pal",
            @"gfx\Terra01\E2_Moss02.tex",
            @"gfx\Terra01\E2_Moss02.pal",
            @"gfx\Terra02\E2_D_Surface02_S1.tex",
            @"gfx\Terra02\E2_D_Surface02_S1.pal",
            @"gfx\Terra02\E2_D_Surface03_MS.tex",
            @"gfx\Terra02\E2_D_Surface03_MS.pal",
            @"gfx\Terra02\E2_Komodo01.tex",
            @"gfx\Terra02\E2_Komodo01.pal",
            @"gfx\Terra02\E2_Column01.tex",
            @"gfx\Terra02\E2_Column01.pal",
            @"gfx\Terra02\E2_Column02.tex",
            @"gfx\Terra02\E2_Column02.pal",
            @"gfx\Terra01\E1_Bridge01.tex",
            @"gfx\Terra01\E1_Bridge01.pal",
            @"gfx\Terra01\garrow.tex",
            @"gfx\Terra01\garrow.pal",
            @"gfx\Terra01\E1_DirtGrassTrim.tex",
            @"gfx\Terra01\E1_DirtGrassTrim.pal",
            @"gfx\Terra01\E1_ground.tex",
            @"gfx\Terra01\E1_ground.pal",
            @"gfx\Terra01\E1_dirtGrassTransition.tex",
            @"gfx\Terra01\E1_dirtGrassTransition.pal",
            @"gfx\Terra01\E1_D_Surface_M.tex",
            @"gfx\Terra01\E1_D_Surface_M.pal",
            @"gfx\Terra01\E1_SandGrassTrim.tex",
            @"gfx\Terra01\E1_SandGrassTrim.pal",
            @"gfx\Terra01\E1_dirtSandTransition.tex",
            @"gfx\Terra01\E1_dirtSandTransition.pal",
            @"gfx\Terra01\E1_WaterTrim.tex",
            @"gfx\Terra01\E1_WaterTrim.pal",
            @"gfx\Terra01\E1_Water01_TR.tex",
            @"gfx\Terra01\E1_Water01_TR.pal",
            @"gfx\Terra01\E1_Rock01.tex",
            @"gfx\Terra01\E1_Rock01.pal",
            @"gfx\Terra01\E1_Totem02.tex",
            @"gfx\Terra01\E1_Totem02.pal",
            @"gfx\Terra01\E1_PALM.tex",
            @"gfx\Terra01\E1_PALM.pal",
            @"gfx\Terra01\olive.tex",
            @"gfx\Terra01\olive.pal",
            @"gfx\Terra01\aqua.tex",
            @"gfx\Terra01\aqua.pal",
            @"gfx\Terra02\E2_TreeTrunk02.tex",
            @"gfx\Terra02\E2_TreeTrunk02.pal",
            @"gfx\Terra02\E2_TreeTrunk.tex",
            @"gfx\Terra02\E2_TreeTrunk.pal",
            @"gfx\Terra02\E2_TreeLeaves.tex",
            @"gfx\Terra02\E2_TreeLeaves.pal",
            @"gfx\Terra02\TempStartline.tex",
            @"gfx\Terra02\TempStartline.pal",
            @"gfx\Terra02\garrow.tex",
            @"gfx\Terra02\garrow.pal",
            @"gfx\Terra02\E2_bridge02.tex",
            @"gfx\Terra02\E2_bridge02.pal",
            @"gfx\Terra02\E1_Bridge01.tex",
            @"gfx\Terra02\E1_Bridge01.pal",
            @"gfx\Terra02\E2_Moss02.tex",
            @"gfx\Terra02\E2_Moss02.pal",
            @"gfx\Terra02\E2_Rock02.tex",
            @"gfx\Terra02\E2_Rock02.pal",
            @"gfx\Terra02\E2_Rock01.tex",
            @"gfx\Terra02\E2_Rock01.pal",
            @"gfx\Terra02\rope.tex",
            @"gfx\Terra02\rope.pal",
            @"gfx\Terra03\TC_surface01.tex",
            @"gfx\Terra03\TC_surface01.pal",
            @"gfx\Terra03\TC_surface02.tex",
            @"gfx\Terra03\TC_surface02.pal",
            @"gfx\Terra03\garrow.tex",
            @"gfx\Terra03\garrow.pal",
            @"gfx\Terra03\HH_floor2.tex",
            @"gfx\Terra03\HH_floor2.pal",
            @"gfx\Terra03\JT_trans01.tex",
            @"gfx\Terra03\JT_trans01.pal",
            @"gfx\Terra03\newCenterTrans.tex",
            @"gfx\Terra03\newCenterTrans.pal",
            @"gfx\Terra03\jungle_road.tex",
            @"gfx\Terra03\jungle_road.pal",
            @"gfx\Terra03\TempStartline.tex",
            @"gfx\Terra03\TempStartline.pal",
            @"gfx\Terra03\TC_sidewall.tex",
            @"gfx\Terra03\TC_sidewall.pal",
            @"gfx\Terra03\HH_headear.tex",
            @"gfx\Terra03\HH_headear.pal",
            @"gfx\Terra03\T_leafcluster02_A.tex",
            @"gfx\Terra03\T_leafcluster02_A.pal",
            @"gfx\Terra03\T_leafcluster02_B.tex",
            @"gfx\Terra03\T_leafcluster02_B.pal",
            @"gfx\Terra03\E2_Moss02.tex",
            @"gfx\Terra03\E2_Moss02.pal",
            @"gfx\Terra03\E3_torch.tex",
            @"gfx\Terra03\E3_torch.pal",
            @"gfx\Terra03\fire.tex",
            @"gfx\Terra03\fire.pal",
            @"gfx\Terra03\HH_wall0aORG.tex",
            @"gfx\Terra03\HH_wall0aORG.pal",
            @"gfx\Barin01\start.tex",
            @"gfx\Barin01\start.pal",
            @"gfx\Barin01\garrow.bmp.tex",
            @"gfx\Barin01\garrow.bmp.pal",
            @"gfx\Barin02\B2_D_Surface10.tex",
            @"gfx\Barin02\B2_D_Surface10.pal",
            @"gfx\Barin02\B2_D_Surface01A.tex",
            @"gfx\Barin02\B2_D_Surface01A.pal",
            @"gfx\Barin02\B2_SeaHorse01.tex",
            @"gfx\Barin02\B2_SeaHorse01.pal",
            @"gfx\Barin02\B2_Surface01.tex",
            @"gfx\Barin02\B2_Surface01.pal",
            @"gfx\Barin01\B1DSurface03.tex",
            @"gfx\Barin01\B1DSurface03.pal",
            @"gfx\Barin02\B2_SurfaceICE_02.tex",
            @"gfx\Barin02\B2_SurfaceICE_02.pal",
            @"gfx\Barin02\B2_SurfaceICE_01.tex",
            @"gfx\Barin02\B2_SurfaceICE_01.pal",
            @"gfx\Barin02\B2_StoneWall02.tex",
            @"gfx\Barin02\B2_StoneWall02.pal",
            @"gfx\Barin01\B1DSurface03a.tex",
            @"gfx\Barin01\B1DSurface03a.pal",
            @"gfx\Barin01\B1DSurfaceICES1.tex",
            @"gfx\Barin01\B1DSurfaceICES1.pal",
            @"gfx\Barin01\B1DSurfaceICEM2.tex",
            @"gfx\Barin01\B1DSurfaceICEM2.pal",
            @"gfx\Barin01\B1DSurface04.tex",
            @"gfx\Barin01\B1DSurface04.pal",
            @"gfx\Barin01\B1DSurface04c.tex",
            @"gfx\Barin01\B1DSurface04c.pal",
            @"gfx\Barin01\B1lava02f.tex",
            @"gfx\Barin01\B1lava02f.pal",
            @"gfx\Barin01\B1lava02a.tex",
            @"gfx\Barin01\B1lava02a.pal",
            @"gfx\Barin01\B1crystal1c.tex",
            @"gfx\Barin01\B1crystal1c.pal",
            @"gfx\Barin01\B1MetalTree.tex",
            @"gfx\Barin01\B1MetalTree.pal",
            @"gfx\Barin01\B1Concrete2.tex",
            @"gfx\Barin01\B1Concrete2.pal",
            @"gfx\Barin01\B1DSurface04c1.tex",
            @"gfx\Barin01\B1DSurface04c1.pal",
            @"gfx\Barin01\B1LavaRock.tex",
            @"gfx\Barin01\B1LavaRock.pal",
            @"gfx\Barin02\B2_Snow01.tex",
            @"gfx\Barin02\B2_Snow01.pal",
            @"gfx\Barin02\B2_CaveColumn03.tex",
            @"gfx\Barin02\B2_CaveColumn03.pal",
            @"gfx\Barin02\B2_StoneWall05_B.tex",
            @"gfx\Barin02\B2_StoneWall05_B.pal",
            @"gfx\Barin02\B2_StoneWall05.tex",
            @"gfx\Barin02\B2_StoneWall05.pal",
            @"gfx\Barin02\B2_D_Surface10a.tex",
            @"gfx\Barin02\B2_D_Surface10a.pal",
            @"gfx\Barin02\B2_Crystal01.tex",
            @"gfx\Barin02\B2_Crystal01.pal",
            @"gfx\Barin03\B3_D_Surface01_S.tex",
            @"gfx\Barin03\B3_D_Surface01_S.pal",
            @"gfx\Barin03\B3_D_Surface01_M.tex",
            @"gfx\Barin03\B3_D_Surface01_M.pal",
            @"gfx\Barin03\start.tex",
            @"gfx\Barin03\start.pal",
            @"gfx\Barin03\garrow.bmp.tex",
            @"gfx\Barin03\garrow.bmp.pal",
            @"gfx\Barin03\B3_MetalWall07.tex",
            @"gfx\Barin03\B3_MetalWall07.pal",
            @"gfx\Barin03\B3_D_Surface02a.tex",
            @"gfx\Barin03\B3_D_Surface02a.pal",
            @"gfx\Barin03\B3_D_Surface01_W.tex",
            @"gfx\Barin03\B3_D_Surface01_W.pal",
            @"gfx\Barin03\B3_RockWall01.tex",
            @"gfx\Barin03\B3_RockWall01.pal",
            @"gfx\Barin03\B3_Wreckage01.tex",
            @"gfx\Barin03\B3_Wreckage01.pal",
            @"gfx\Barin03\B3_MetalWall05.tex",
            @"gfx\Barin03\B3_MetalWall05.pal",
            @"gfx\Barin03\B3_Light01.tex",
            @"gfx\Barin03\B3_Light01.pal",
            @"gfx\Barin03\B3_D_Surface01_Z.tex",
            @"gfx\Barin03\B3_D_Surface01_Z.pal",
            @"gfx\Barin03\B3_SeaRock01.tex",
            @"gfx\Barin03\B3_SeaRock01.pal",
            @"gfx\Barin03\B3_Crystal01.tex",
            @"gfx\Barin03\B3_Crystal01.pal",
            @"gfx\Barin03\B3_MetalBar01.tex",
            @"gfx\Barin03\B3_MetalBar01.pal",
            @"gfx\Barin03\ringGreen.tex",
            @"gfx\Barin03\ringGreen.pal",
            @"gfx\Fenom03\F3_rock02.tex",
            @"gfx\Fenom03\F3_rock02.pal",
            @"gfx\Fenom03\F3_D_Surface03.tex",
            @"gfx\Fenom03\F3_D_Surface03.pal",
            @"gfx\Fenom03\F3_D_Surface04.tex",
            @"gfx\Fenom03\F3_D_Surface04.pal",
            @"gfx\Fenom03\F3_D_Surface01.tex",
            @"gfx\Fenom03\F3_D_Surface01.pal",
            @"gfx\Fenom03\F3_ivory.tex",
            @"gfx\Fenom03\F3_ivory.pal",
            @"gfx\Fenom01\F1_D_Surface_S.tex",
            @"gfx\Fenom01\F1_D_Surface_S.pal",
            @"gfx\Fenom01\F1_D_Surface_M.tex",
            @"gfx\Fenom01\F1_D_Surface_M.pal",
            @"gfx\Fenom01\garrow.bmp.tex",
            @"gfx\Fenom01\garrow.bmp.pal",
            @"gfx\Fenom01\F1_Surface_Sand.tex",
            @"gfx\Fenom01\F1_Surface_Sand.pal",
            @"gfx\Fenom01\start.tex",
            @"gfx\Fenom01\start.pal",
            @"gfx\Fenom01\F1_ground.tex",
            @"gfx\Fenom01\F1_ground.pal",
            @"gfx\Fenom01\fence.tex",
            @"gfx\Fenom01\fence.pal",
            @"gfx\Fenom01\F1_Rock08.tex",
            @"gfx\Fenom01\F1_Rock08.pal",
            @"gfx\Fenom01\F1_Rock06.tex",
            @"gfx\Fenom01\F1_Rock06.pal",
            @"gfx\Fenom01\F1_Rock07.tex",
            @"gfx\Fenom01\F1_Rock07.pal",
            @"gfx\Fenom01\F1_cactus1.tex",
            @"gfx\Fenom01\F1_cactus1.pal",
            @"gfx\Fenom01\F1_Clock04.tex",
            @"gfx\Fenom01\F1_Clock04.pal",
            @"gfx\Fenom01\F1_Clock06.tex",
            @"gfx\Fenom01\F1_Clock06.pal",
            @"gfx\Fenom01\F1_Clock01.tex",
            @"gfx\Fenom01\F1_Clock01.pal",
            @"gfx\Fenom01\f1_shed01.tex",
            @"gfx\Fenom01\f1_shed01.pal",
            @"gfx\Fenom02\start.tex",
            @"gfx\Fenom02\start.pal",
            @"gfx\Fenom02\F2_D_Surface02.tex",
            @"gfx\Fenom02\F2_D_Surface02.pal",
            @"gfx\Fenom02\F2_D_Surface01.tex",
            @"gfx\Fenom02\F2_D_Surface01.pal",
            @"gfx\Fenom02\F2_D_Surface01_B.tex",
            @"gfx\Fenom02\F2_D_Surface01_B.pal",
            @"gfx\Fenom02\F2_D_Surface05.tex",
            @"gfx\Fenom02\F2_D_Surface05.pal",
            @"gfx\Fenom02\garrow.bmp.tex",
            @"gfx\Fenom02\garrow.bmp.pal",
            @"gfx\Teknee02\T2_MetalWall04.tex",
            @"gfx\Teknee02\T2_MetalWall04.pal",
            @"gfx\Fenom02\F2_Column04.tex",
            @"gfx\Fenom02\F2_Column04.pal",
            @"gfx\Fenom02\vit.tex",
            @"gfx\Fenom02\vit.pal",
            @"gfx\Fenom02\F2_D_Surface06.tex",
            @"gfx\Fenom02\F2_D_Surface06.pal",
            @"gfx\Fenom02\T1_Pipesign.tex",
            @"gfx\Fenom02\T1_Pipesign.pal",
            @"gfx\Fenom02\F2_Wall01.tex",
            @"gfx\Fenom02\F2_Wall01.pal",
            @"gfx\Fenom02\F2_Wall08.tex",
            @"gfx\Fenom02\F2_Wall08.pal",
            @"gfx\Fenom02\F2_Flag.tex",
            @"gfx\Fenom02\F2_Flag.pal",
            @"gfx\Fenom02\F2_Glass.tex",
            @"gfx\Fenom02\F2_Glass.pal",
            @"gfx\Fenom02\F2_WoodArrow.tex",
            @"gfx\Fenom02\F2_WoodArrow.pal",
            @"gfx\Fenom03\garrow.bmp.tex",
            @"gfx\Fenom03\garrow.bmp.pal",
            @"gfx\Fenom03\F3_D_Surface08.tex",
            @"gfx\Fenom03\F3_D_Surface08.pal",
            @"gfx\Fenom03\start.tex",
            @"gfx\Fenom03\start.pal",
            @"gfx\Fenom03\F3_cuckoo_rock2.tex",
            @"gfx\Fenom03\F3_cuckoo_rock2.pal",
            @"gfx\Fenom03\E1_liaf2_T.tex",
            @"gfx\Fenom03\E1_liaf2_T.pal",
            @"gfx\Teknee02\T2_Surface04.tex",
            @"gfx\Teknee02\T2_Surface04.pal",
            @"gfx\Teknee02\T2_Surface05.tex",
            @"gfx\Teknee02\T2_Surface05.pal",
            @"gfx\Teknee03\T3_Metalpipe01.tex",
            @"gfx\Teknee03\T3_Metalpipe01.pal",
            @"gfx\Teknee03\T3_MetalWall07.tex",
            @"gfx\Teknee03\T3_MetalWall07.pal",
            @"gfx\Teknee03\T3_MetalWall06.tex",
            @"gfx\Teknee03\T3_MetalWall06.pal",
            @"gfx\Teknee03\T3_MetalWall09.tex",
            @"gfx\Teknee03\T3_MetalWall09.pal",
            @"gfx\Teknee03\T3_Light04.tex",
            @"gfx\Teknee03\T3_Light04.pal",
            @"gfx\Teknee03\T3_MetalWall01.tex",
            @"gfx\Teknee03\T3_MetalWall01.pal",
            @"gfx\Teknee03\T3_TempWindow02.tex",
            @"gfx\Teknee03\T3_TempWindow02.pal",
            @"gfx\Teknee03\T3_D_Surface03.tex",
            @"gfx\Teknee03\T3_D_Surface03.pal",
            @"gfx\Teknee03\T3_D_Surface03b.tex",
            @"gfx\Teknee03\T3_D_Surface03b.pal",
            @"gfx\porterTest\UI_Crown.tex",
            @"gfx\porterTest\UI_Crown.pal",
            @"gfx\porterTest\UI_Gate1.tex",
            @"gfx\porterTest\UI_Gate1.pal",
            @"gfx\Teknee01\T1_D_Surface_G.tex",
            @"gfx\Teknee01\T1_D_Surface_G.pal",
            @"gfx\Teknee01\garrow.bmp.tex",
            @"gfx\Teknee01\garrow.bmp.pal",
            @"gfx\Teknee01\T1_D_Surface_G1.tex",
            @"gfx\Teknee01\T1_D_Surface_G1.pal",
            @"gfx\Teknee01\T1_D_Surface_M.tex",
            @"gfx\Teknee01\T1_D_Surface_M.pal",
            @"gfx\Teknee01\T1_Metalpipe_05.tex",
            @"gfx\Teknee01\T1_Metalpipe_05.pal",
            @"gfx\Teknee01\treadTransition.tex",
            @"gfx\Teknee01\treadTransition.pal",
            @"gfx\Teknee01\T1_D_Surface_S.tex",
            @"gfx\Teknee01\T1_D_Surface_S.pal",
            @"gfx\Teknee01\T1_ground.tex",
            @"gfx\Teknee01\T1_ground.pal",
            @"gfx\Teknee01\start.tex",
            @"gfx\Teknee01\start.pal",
            @"gfx\Teknee01\T1_Rack02.tex",
            @"gfx\Teknee01\T1_Rack02.pal",
            @"gfx\Teknee01\T1_MetalWall_02.tex",
            @"gfx\Teknee01\T1_MetalWall_02.pal",
            @"gfx\Teknee01\T1_Back02.tex",
            @"gfx\Teknee01\T1_Back02.pal",
            @"gfx\Teknee01\T1_Concrete1.tex",
            @"gfx\Teknee01\T1_Concrete1.pal",
            @"gfx\Teknee01\T1_Pipesign.tex",
            @"gfx\Teknee01\T1_Pipesign.pal",
            @"gfx\Teknee01\T1_MetalWall_03.tex",
            @"gfx\Teknee01\T1_MetalWall_03.pal",
            @"gfx\Teknee01\T1_Rack01.tex",
            @"gfx\Teknee01\T1_Rack01.pal",
            @"gfx\Teknee01\T1_Metalpipe_02.tex",
            @"gfx\Teknee01\T1_Metalpipe_02.pal",
            @"gfx\Teknee02\T2_D_Surface04.tex",
            @"gfx\Teknee02\T2_D_Surface04.pal",
            @"gfx\Teknee02\garrow.bmp.tex",
            @"gfx\Teknee02\garrow.bmp.pal",
            @"gfx\Teknee02\start.tex",
            @"gfx\Teknee02\start.pal",
            @"gfx\Teknee02\T2_MetalWall02.tex",
            @"gfx\Teknee02\T2_MetalWall02.pal",
            @"gfx\Teknee02\T2_MetalWall06.tex",
            @"gfx\Teknee02\T2_MetalWall06.pal",
            @"gfx\Teknee02\t2_light01.tex",
            @"gfx\Teknee02\t2_light01.pal",
            @"gfx\Teknee02\T2_D_Surface05.tex",
            @"gfx\Teknee02\T2_D_Surface05.pal",
            @"gfx\Teknee02\T2_MetalWall14.tex",
            @"gfx\Teknee02\T2_MetalWall14.pal",
            @"gfx\Teknee02\T2_MetalWall12.tex",
            @"gfx\Teknee02\T2_MetalWall12.pal",
            @"gfx\Teknee02\RoadSignArrow.tex",
            @"gfx\Teknee02\RoadSignArrow.pal",
            @"gfx\Teknee03\start.tex",
            @"gfx\Teknee03\start.pal",
            @"gfx\Teknee03\garrow.bmp.tex",
            @"gfx\Teknee03\garrow.bmp.pal",
            @"gfx\Teknee03\T2_Surface04.tex",
            @"gfx\Teknee03\T2_Surface04.pal",
            @"gfx\Teknee03\T2_Surface05.tex",
            @"gfx\Teknee03\T2_Surface05.pal",
            @"gfx\Teknee03\T3_MetalWall08.tex",
            @"gfx\Teknee03\T3_MetalWall08.pal",
            @"gfx\Teknee03\T3_D_Surface04.tex",
            @"gfx\Teknee03\T3_D_Surface04.pal",
            @"gfx\Teknee03\T3_Light02.tex",
            @"gfx\Teknee03\T3_Light02.pal",
            @"gfx\Teknee03\lightning.tex",
            @"gfx\Teknee03\lightning.pal",
            @"gfx\Barin01\E2_Komodo02.tex",
            @"gfx\Barin01\E2_Komodo02.pal",
            @"gfx\Fenom01\F1_Sand_Rock_Sand.tex",
            @"gfx\Fenom01\F1_Sand_Rock_Sand.pal",
            @"gfx\Terra03\CY_Ground_S.tex",
            @"gfx\Terra03\CY_Ground_S.pal",
            @"gfx\Teknee02\T2_MetalWall01.tex",
            @"gfx\Teknee02\T2_MetalWall01.pal",
            @"gfx\Terra02\parallax0.pal",
            @"gfx\Terra02\parallax0.rle",
            @"gfx\Terra02\parallax1.pal",
            @"gfx\Terra02\parallax1.rle",
            @"gfx\Terra02\parallax2.pal",
            @"gfx\Terra02\parallax2.rle",
            @"gfx\Terra01\parallax0.pal",
            @"gfx\Terra01\parallax0.rle",
            @"gfx\Terra01\parallax1.pal",
            @"gfx\Terra01\parallax1.rle",
            @"gfx\Terra01\parallax2.pal",
            @"gfx\Terra01\parallax2.rle",
            @"gfx\Terra03\parallax0.pal",
            @"gfx\Terra03\parallax0.rle",
            @"gfx\Terra03\parallax1.pal",
            @"gfx\Terra03\parallax1.rle",
            @"gfx\Terra03\parallax2.pal",
            @"gfx\Terra03\parallax2.rle",
            @"gfx\Barin02\parallax0.pal",
            @"gfx\Barin02\parallax0.rle",
            @"gfx\Barin02\parallax1.pal",
            @"gfx\Barin02\parallax1.rle",
            @"gfx\Barin02\parallax2.pal",
            @"gfx\Barin02\parallax2.rle",
            @"gfx\Barin01\parallax0.pal",
            @"gfx\Barin01\parallax0.rle",
            @"gfx\Barin01\parallax1.pal",
            @"gfx\Barin01\parallax1.rle",
            @"gfx\Barin01\parallax2.pal",
            @"gfx\Barin01\parallax2.rle",
            @"gfx\Barin03\parallax0.pal",
            @"gfx\Barin03\parallax0.rle",
            @"gfx\Barin03\parallax1.pal",
            @"gfx\Barin03\parallax1.rle",
            @"gfx\Barin03\parallax2.pal",
            @"gfx\Barin03\parallax2.rle",
            @"gfx\Fenom03\parallax0.pal",
            @"gfx\Fenom03\parallax0.rle",
            @"gfx\Fenom03\parallax1.pal",
            @"gfx\Fenom03\parallax1.rle",
            @"gfx\Fenom03\parallax2.pal",
            @"gfx\Fenom03\parallax2.rle",
            @"gfx\Fenom01\parallax0.pal",
            @"gfx\Fenom01\parallax0.rle",
            @"gfx\Fenom01\parallax1.pal",
            @"gfx\Fenom01\parallax1.rle",
            @"gfx\Fenom01\parallax2.pal",
            @"gfx\Fenom01\parallax2.rle",
            @"gfx\Fenom02\parallax0.pal",
            @"gfx\Fenom02\parallax0.rle",
            @"gfx\Fenom02\parallax1.pal",
            @"gfx\Fenom02\parallax1.rle",
            @"gfx\Fenom02\parallax2.pal",
            @"gfx\Fenom02\parallax2.rle",
            @"gfx\Teknee03\parallax0.pal",
            @"gfx\Teknee03\parallax0.rle",
            @"gfx\Teknee03\parallax1.pal",
            @"gfx\Teknee03\parallax1.rle",
            @"gfx\Teknee03\parallax2.pal",
            @"gfx\Teknee03\parallax2.rle",
            @"gfx\Teknee01\parallax0.pal",
            @"gfx\Teknee01\parallax0.rle",
            @"gfx\Teknee01\parallax1.pal",
            @"gfx\Teknee01\parallax1.rle",
            @"gfx\Teknee01\parallax2.pal",
            @"gfx\Teknee01\parallax2.rle",
            @"gfx\Teknee02\parallax0.pal",
            @"gfx\Teknee02\parallax0.rle",
            @"gfx\Teknee02\parallax1.pal",
            @"gfx\Teknee02\parallax1.rle",
            @"gfx\Teknee02\parallax2.pal",
            @"gfx\Teknee02\parallax2.rle",
            @"gfx\veloBoss\parallax0.pal",
            @"gfx\veloBoss\parallax0.rle",
            @"gfx\veloBoss\parallax1.pal",
            @"gfx\veloBoss\parallax1.rle",
            @"gfx\veloBoss\parallax2.pal",
            @"gfx\veloBoss\parallax2.rle",
            @"gfx\Barin02\start.pal",
            @"gfx\Barin02\start.tex",
            @"gfx\Barin02\garrow.bmp.pal",
            @"gfx\Barin02\garrow.bmp.tex",
            @"gfx\Barin02\B1DSurface03.pal",
            @"gfx\Barin02\B1DSurface03.tex",
        };
    }
}