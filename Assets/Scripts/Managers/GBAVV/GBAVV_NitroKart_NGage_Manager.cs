using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_Manager : IGameManager
    {
        public string ExeFilePath => @"6rac.app";
        public string DataFilePath => @"data.gob";

        public GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[0];

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)),
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
            new GameAction("Export textures", false, true, (input, output) => ExportTexturesAsync(settings, output)),
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputDir, bool includeAbsolutePointer = true)
        {
            await DoAtBlocksAsync(settings, (s, i, offset) =>
            {
                var append = includeAbsolutePointer ? $"_{offset.BlockPointer.AbsoluteOffset:X8}" : String.Empty;

                var bytes = s.SerializeArray<byte>(default, s.CurrentLength, name: $"Block[{i}]");

                Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}{append}.dat"), bytes);
            });
        }

        public async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif)
        {
            await DoAtBlocksAsync(settings, (s, i, offset) =>
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
        public async UniTask ExportTexturesAsync(GameSettings settings, string outputDir)
        {
            Color[] pal = null;

            await DoAtBlocksAsync(settings, (s, i, offset) =>
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

        public async UniTask DoAtBlocksAsync(GameSettings settings, Action<SerializerObject, int, GBAVV_NitroKart_NGage_DataFileEntry> action)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                // Load the data file
                var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

                // Enumerate every block in the offset table
                for (int i = 0; i < data.BlocksCount; i++)
                {
                    // Get the offset
                    var offset = data.DataFileEntries[i];

                    s.DoAt(offset.BlockPointer, () => s.DoEncoded(new BriefLZEncoder(), () =>
                    {
                        action(s, i, offset);
                        s.Goto(s.CurrentPointer.file.StartPointer + s.CurrentLength);
                    }));
                }
            }
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

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context)
        {
            await context.AddLinearSerializedFileAsync(ExeFilePath);
            await context.AddLinearSerializedFileAsync(DataFilePath);
        }
    }
}