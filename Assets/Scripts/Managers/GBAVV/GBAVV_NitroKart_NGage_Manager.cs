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
    public class GBAVV_NitroKart_NGage_Manager : GBAVV_NitroKart_Manager
    {
        public string ExeFilePath => @"6rac.app";
        public string DataFilePath => @"data.gob";
        public const uint ExeBaseAddress = 0x10000000 - 648;

        public override string[] Languages => new string[]
        {
            "English",
            "French",
            "German",
            "Italian",
            "Spanish",
        };

        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)),
            new GameAction("Export Blocks (with filenames)", false, true, (input, output) => ExportBlocksAsync(settings, output, withFilenames: true)),
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
            new GameAction("Export textures", false, true, (input, output) => ExportTexturesAsync(settings, output)),
            new GameAction("Export FLC as GIF", false, true, (input, output) => ExportFLCAsync(settings, output)),
            new GameAction("Export Fonts", false, true, (input, output) => ExportFontsAsync(settings, output)),
            new GameAction("Export Backgrounds", false, true, (input, output) => ExportBackgroundsAsync(settings, output)),
            new GameAction("Export Music & Sample Data", false, true, (input, output) => ExportMusicAsync(settings, output)),
        };

        public async UniTask ExportMusicAsync(GameSettings settings, string outputPath) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;

                void ExportSample(string directory, string filename, byte[] data, uint sampleRate, ushort channels) {
                    // Create the directory
                    Directory.CreateDirectory(directory);

                    // Create WAV data
                    var formatChunk = new WAVFormatChunk() {
                        ChunkHeader = "fmt ",
                        FormatType = 1,
                        ChannelCount = channels,
                        SampleRate = sampleRate,
                        BitsPerSample = 8,
                    };

                    var wav = new WAV {
                        Magic = "RIFF",
                        FileTypeHeader = "WAVE",
                        Chunks = new WAVChunk[]
                        {
                            formatChunk,
                            new WAVChunk()
                            {
                                ChunkHeader = "data",
                                Data = data
                            }
                        }
                    };

                    formatChunk.ByteRate = (formatChunk.SampleRate * formatChunk.BitsPerSample * formatChunk.ChannelCount) / 8;
                    formatChunk.BlockAlign = (ushort)((formatChunk.BitsPerSample * formatChunk.ChannelCount) / 8);

                    // Get the output path
                    var outputFilePath = Path.Combine(directory, filename + ".wav");

                    // Create and open the output file
                    using (var outputStream = File.Create(outputFilePath)) {
                        // Create a context
                        using (var wavContext = new Context(settings)) {
                            // Create a key
                            const string wavKey = "wav";

                            // Add the file to the context
                            wavContext.AddFile(new StreamFile(wavKey, outputStream, wavContext));

                            // Write the data
                            FileFactory.Write<WAV>(wavKey, wav, wavContext);
                        }
                    }
                }

                void ExportGAX(string directory, GAX2_Song song, ushort channels) {
                    for (int i = 0; i < song.Samples.Length; i++) {
                        var e = song.Samples[i];
                        string outPath = Path.Combine(directory, "Samples");
                        ExportSample(outPath, $"{i}_{e.SampleOffset.AbsoluteOffset:X8}", e.Sample, 15769, channels);
                    }
                    var h = song;
                    if(h.SampleRate == 0) return;
                    // For each entry
                    GAX2_MidiWriter w = new GAX2_MidiWriter();
                    Directory.CreateDirectory(Path.Combine(outputPath, "midi"));
                    w.Write(h, Path.Combine(outputPath, "midi", $"{h.ParsedName}.mid"));

                    GAX2_XMWriter xmw = new GAX2_XMWriter();
                    Directory.CreateDirectory(Path.Combine(outputPath, "xm"));

                    XM xm = xmw.ConvertToXM(h);

                    // Get the output path
                    var outputFilePath = Path.Combine(outputPath, "xm", $"{h.ParsedName}.xm");

                    // Create and open the output file
                    using (var outputStream = File.Create(outputFilePath)) {
                        // Create a context
                        using (var xmContext = new Context(settings)) {
                            xmContext.Log.OverrideLogPath = Path.Combine(outputPath, "xm", $"{h.ParsedName}.txt");
                            // Create a key
                            string xmKey = $"{h.ParsedName}.xm";

                            // Add the file to the context
                            xmContext.AddFile(new StreamFile(xmKey, outputStream, context));

                            // Write the data
                            FileFactory.Write<XM>(xmKey, xm, xmContext);
                        }
                    }

                }

                await LoadFilesAsync(context);
                // Load the data file
                var dataFile = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

                // Load the exe
                var exe = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context, onPreSerialize: (_,e) => e.SerializeGAX = true);

                // Load the rom
                for (int i = 0; i < exe.GAX_Music.Songs.Length; i++) {
                    ExportGAX($"{outputPath}/music/{i}", exe.GAX_Music.Songs[i], 2);
                }
                for (int i = 0; i < exe.GAX_FX.Songs.Length; i++) {
                    ExportGAX($"{outputPath}/fx/{i}", exe.GAX_FX.Songs[i], 1);
                }
                /*uint[] ptrs = s.GameSettings.GameModeSelection == GameModeSelection.RaymanRavingRabbidsGBAUS ? ptrs_us : ptrs_eu;
                foreach (var ptr in ptrs) {
                    s.DoAt(new Pointer(ptr, rom.Offset.file), () => {
                        GAX2_Song h = s.SerializeObject<GAX2_Song>(default, name: "SongHeader");
                        // For each entry
                        GAX2_MidiWriter w = new GAX2_MidiWriter();
                        Directory.CreateDirectory(Path.Combine(outputPath, "midi"));
                        w.Write(h, Path.Combine(outputPath, "midi", $"{h.ParsedName}.mid"));

                        GAX2_XMWriter xmw = new GAX2_XMWriter();
                        Directory.CreateDirectory(Path.Combine(outputPath, "xm"));

                        XM xm = xmw.ConvertToXM(h);

                        // Get the output path
                        var outputFilePath = Path.Combine(outputPath, "xm", $"{h.ParsedName}.xm");

                        // Create and open the output file
                        using (var outputStream = File.Create(outputFilePath)) {
                            // Create a context
                            using (var xmContext = new Context(settings)) {
                                xmContext.Log.OverrideLogPath = Path.Combine(outputPath, "xm", $"{h.ParsedName}.txt");
                                // Create a key
                                string xmKey = $"{h.ParsedName}.xm";

                                // Add the file to the context
                                xmContext.AddFile(new StreamFile(xmKey, outputStream, context));

                                // Write the data
                                FileFactory.Write<XM>(xmKey, xm, xmContext);
                            }
                        }
                    });
                }*/
            }
        }

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
            GBAVV_NitroKart_NGage_PAL pal = null;

            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                // Load the exe
                var exe = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context, onPreSerialize: (s, o) => o.SerializeAllData = true);

                // Get the blend mode for each referenced texture
                var blendModes = new Dictionary<uint, byte>();

                foreach (var lev in exe.LevelInfos)
                {
                    var pvs = lev.PVS;

                    foreach (var tri in pvs.Triangles)
                        blendModes[CalculateCRC($"{pvs.TextureFilePaths[tri.TextureIndex].GetFullPath}.tex", exe.CRCPolynomialData)] = tri.BlendMode;
                }

                DoAtBlocks(context, (s, i, offset) =>
                {
                    // If the block is 64 bytes long we assume it's a palette
                    if (s.CurrentLength == 64)
                    {
                        pal = s.SerializeObject<GBAVV_NitroKart_NGage_PAL>(default, name: $"Pal[{i}]");
                    }
                    else
                    {
                        // If we serialized a palette in the previous block we assume this block has the textures
                        if (pal != null && s.CurrentLength % 0x1500 == 0)
                        {
                            var texFile = s.SerializeObject<GBAVV_NitroKart_NGage_TEX>(default, name: $"TEX[{i}]");

                            byte blendMode = blendModes.TryGetItem(offset.CRC);

                            var textures = LoadTextures(texFile, pal, blendMode, true);

                            for (int j = 0; j < textures.Length; j++)
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"{GetBlockExportName(context, i, offset.CRC, true, false)}{(texFile.Textures.Length > 1 ? $" - {j}" : "")}.png"), textures[j].EncodeToPNG());
                        }

                        // Remove palette
                        pal = null;
                    }
                });
            }
        }

        public async UniTask ExportFLCAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                // Enumerate every .flc file
                foreach (var flcPath in FilePaths.Where(x => x.EndsWith(".flc")))
                {
                    var s = context.Deserializer;
                    var flc = DoAtBlock(context, flcPath, () => s.SerializeObject<FLIC>(default, name: $"{Path.GetFileNameWithoutExtension(flcPath)}"));

                    using (var collection = flc.ToMagickImageCollection())
                    {
                        // Save as gif
                        var path = Path.Combine(outputDir, Path.GetDirectoryName(flcPath), $"{Path.GetFileNameWithoutExtension(flcPath)}.gif");
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        collection.Write(path);
                    }
                }
            }
        }

        public async UniTask ExportFontsAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                // Enumerate every .fni file
                foreach (var fniPath in FilePaths.Where(x => x.EndsWith(".fni")))
                {
                    var pathWithoutExt = Path.Combine(Path.GetDirectoryName(fniPath), Path.GetFileNameWithoutExtension(fniPath));
                    var fncPath = $"{pathWithoutExt}.fnc";

                    var s = context.Deserializer;
                    var imgData = DoAtBlock(context, fniPath, () => s.SerializeArray<byte>(default, s.CurrentLength, name: $"{Path.GetFileNameWithoutExtension(fniPath)}"));
                    var pal = DoAtBlock(context, fncPath, () => s.SerializeObject<GBAVV_NitroKart_NGage_PAL>(default, name: $"{Path.GetFileNameWithoutExtension(fncPath)}"));

                    var palette = pal.Palette.Select((x, i) =>
                    {
                        if (i == 0)
                            return Color.clear;

                        var c = x.GetColor();
                        return new Color(c.r, c.g, c.b);
                    }).ToArray();

                    var sizes = new Dictionary<int, Vector2Int>()
                    {
                        [0x10000] = new Vector2Int(256, 256),
                        [0x8000] = new Vector2Int(128, 256),
                        [0x4000] = new Vector2Int(128, 128),
                    };

                    var size = sizes[imgData.Length];

                    var tex = TextureHelpers.CreateTexture2D(size.x, size.y);

                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            tex.SetPixel(x, tex.height - y - 1, palette[imgData[y * tex.width + x]]);
                        }
                    }

                    tex.Apply();

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{pathWithoutExt}.png"), tex.EncodeToPNG());
                }
            }
        }

        public async UniTask ExportBackgroundsAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                // Enumerate every .rle file
                foreach (var rlePath in FilePaths.Where(x => x.EndsWith(".rle")))
                {
                    var pathWithoutExt = Path.Combine(Path.GetDirectoryName(rlePath), Path.GetFileNameWithoutExtension(rlePath));
                    var palPath = $"{pathWithoutExt}.pal";

                    var s = context.Deserializer;
                    var rle = DoAtBlock(context, rlePath, () => s.SerializeObject<GBAVV_NitroKart_NGage_RLE>(default, name: $"{Path.GetFileNameWithoutExtension(rlePath)}"));
                    var pal = DoAtBlock(context, palPath, () => s.SerializeObject<GBAVV_NitroKart_NGage_PAL>(default, name: $"{Path.GetFileNameWithoutExtension(palPath)}"));
                    
                    Util.ExportAnimAsGif(rle.ToTextures(pal), 4, false, false, Path.Combine(outputDir, $"{pathWithoutExt}.gif"));
                }
            }
        }

        public override void FindDataInROM(SerializerObject s, Pointer offset)
        {
            // Read ROM as a uint array
            var values = s.DoAt(offset, () => s.SerializeArray<uint>(default, s.CurrentLength / 4, name: "Values"));

            // Helper for getting a pointer
            long getPointer(int index) => ExeBaseAddress + index * 4;
            bool isValidPointer(uint value) => value >= ExeBaseAddress && value < ExeBaseAddress + s.CurrentLength;

            // Keep track of found data
            var foundScripts = new List<Tuple<long, string>>();

            // Find scripts by finding the name command which is always the first one
            for (int i = 0; i < values.Length - 2; i++)
            {
                if (values[i] == 9 && values[i + 1] == 7 && isValidPointer(values[i + 2]))
                {
                    foundScripts.Add(new Tuple<long, string>(getPointer(i), s.DoAt(new Pointer(values[i + 2], s.CurrentPointer.file), () => s.SerializeString(default))));
                }
            }

            // Log found data to clipboard
            var str = new StringBuilder();

            str.AppendLine();
            str.AppendLine($"Scripts:");

            foreach (var (p, name) in foundScripts)
                str.AppendLine($"0x{p:X8}, // {name}");

            str.ToString().CopyToClipboard();
        }

        public override void FindObjTypeData(Context context)
        {
            var s = context.Deserializer;

            var str = new StringBuilder();

            var initFunctionPointers = s.DoAt(new Pointer(ObjTypesPointer, s.Context.GetFile(ExeFilePath)), () => s.SerializePointerArray(default, ObjTypesCount));
            var orderedPointers = initFunctionPointers.OrderBy(x => x.AbsoluteOffset).Distinct().ToArray();

            // Enumerate every obj init function
            for (int i = 0; i < initFunctionPointers.Length; i++)
            {
                var nextPointer = orderedPointers.ElementAtOrDefault(orderedPointers.FindItemIndex(x => x == initFunctionPointers[i]) + 1);

                s.DoAt(initFunctionPointers[i], () =>
                {
                    s.Align();

                    var foundPointer = false;

                    // Try and read every int as a pointer until we get a valid one 25 times
                    for (int j = 0; j < 25; j++)
                    {
                        if (nextPointer != null && s.CurrentPointer.AbsoluteOffset >= nextPointer.AbsoluteOffset)
                            break;

                        var p = s.SerializePointer(default, allowInvalid: true);

                        s.DoAt(p, () =>
                        {
                            s.Goto(s.CurrentPointer + 20);
                            var graphicsPointer = s.SerializePointer(default, allowInvalid: true);
                            var gfxPath = s.DoAt(graphicsPointer, () => s.SerializeString(default));

                            if (gfxPath?.EndsWith(".gfx") == true)
                            {
                                str.AppendLine($"0x{p.AbsoluteOffset:X8}, // {i}");
                                foundPointer = true;
                            }
                        });

                        if (foundPointer)
                            return;
                    }

                    // No pointer found...
                    str.AppendLine($"null, // {i}");
                });
            }

            str.ToString().CopyToClipboard();
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

        public IEnumerable<(GBAVV_Map2D_AnimSet, string)> LoadGFX(Context context)
        {
            // Get all file paths
            var paths = FilePaths;

            // Enumerate every .gfx file
            foreach (var gfxPath in paths.Where(x => x.EndsWith(".gfx")))
            {
                var s = context.Deserializer;

                // Parse the animation set
                yield return (DoAtBlock(context, gfxPath, () => s.SerializeObject<GBAVV_Map2D_AnimSet>(default, name: $"AnimSet_{Path.GetFileNameWithoutExtension(gfxPath)}")), gfxPath);
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
        public Unity_ObjectManager_GBAVV.AnimSet[][] LoadAnimSets(IEnumerable<(GBAVV_Map2D_AnimSet, string)> animSets)
        {
            Unity_ObjectManager_GBAVV.AnimSet.Animation convertAnim(GBAVV_Map2D_AnimSet animSet, GBAVV_Map2D_Animation anim, int i) => new Unity_ObjectManager_GBAVV.AnimSet.Animation(
                animFrameFunc: () => GetAnimFrames(animSet, i).Select(frame => frame.CreateSprite()).ToArray(),
                crashAnim: anim,
                xPos: animSet.GetMinX(i),
                yPos: animSet.GetMinY(i)
            );

            Unity_ObjectManager_GBAVV.AnimSet convertAnimSet((GBAVV_Map2D_AnimSet, string) animSet) => new Unity_ObjectManager_GBAVV.AnimSet(animSet.Item1.Animations.Select((anim, i) => convertAnim(animSet.Item1, anim, i)).ToArray(), animSet.Item2);

            return new Unity_ObjectManager_GBAVV.AnimSet[][]
            {
                animSets.Select(convertAnimSet).ToArray()
            };
        }

        public Texture2D[] LoadTextures(GBAVV_NitroKart_NGage_TEX tex, GBAVV_NitroKart_NGage_PAL pal, byte blendMode, bool flipY)
        {
            bool hasTransparentColor = blendMode == 6 || blendMode == 7 || blendMode == 9;
            var palData = pal.Palette.Select(p => p.GetColor()).Select((c, i) => (i == 0 && hasTransparentColor) ? c : new Color(c.r, c.g, c.b, 1f)).ToArray();

            Texture2D[] texs = new Texture2D[tex.Textures.Length];
            for (int i = 0; i < texs.Length; i++)
            {
                var texData = tex.Textures[i].Texture_64px.Select(b => (byte)((b / 2) % palData.Length)).ToArray();
                texs[i] = Util.ToTileSetTexture(texData, palData, Util.TileEncoding.Linear_8bpp, 64, flipY);
            }

            return texs;
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

        public GameObject CreateS3DGameObject(Context context, GBAVV_NitroKart_NGage_S3D s3d) {
            float scale = 4096f;
            Vector3 toVertex(GBAVV_NitroKart_NGage_Vertex v) => new Vector3(v.X / scale, v.Z / scale, v.Y / scale);
            Vector2 toUV(GBAVV_NitroKart_NGage_UV uv) => new Vector2(uv.U / (float)0x80, uv.V / (float)0x80);
            ///var palette = pvs.VertexColorsPalettes.Select(p => p.GetColor()).Select(c => new Color(c.r, c.g, c.b, 1f)).ToArray();


            GameObject gaoParent = new GameObject();
            gaoParent.transform.position = Vector3.zero;

            for (int t = 0; t < s3d.TexturesCount; t++) {
                Dictionary<int, MeshInProgress> meshes = new Dictionary<int, MeshInProgress>();
                Dictionary<int, Texture2D[]> animatedTextures = new Dictionary<int, Texture2D[]>();
                foreach (var tri in s3d.Triangles[t]) {
                    var key = tri.TextureIndex | (tri.BlendMode << 8) | (tri.Flags << 16);
                    if (!meshes.ContainsKey(key)) {
                        var texs = LoadTextures(s3d.Textures[tri.TextureIndex], s3d.Palettes[tri.TextureIndex], tri.BlendMode, false);
                        if (texs.Length > 1) animatedTextures.Add(key, texs);
                        meshes[key] = new MeshInProgress($"Texture:{tri.TextureIndex} - BlendMode:{tri.BlendMode} - Flags:{tri.Flags}", texs[0]);
                    }
                    /*
                     *blend modes:
                        0 - Unlit
                        1 - Vertex Color
                        3 - Transparent (0.5)
                        4 - Hidden (collision)

                        6 - also transparent cutout
                        7 - Transparent cutout
                        8 - Additive
                        9 - Additive and transparent

                     flags:
                        32 = scrolling texture
                     */

                    var m = meshes[key];
                    int vertCount = m.vertices.Count;
                    m.vertices.Add(toVertex(s3d.Vertices[t][tri.Vertex0]));
                    m.vertices.Add(toVertex(s3d.Vertices[t][tri.Vertex1]));
                    m.vertices.Add(toVertex(s3d.Vertices[t][tri.Vertex2]));
                    m.uvs.Add(toUV(tri.UV0));
                    m.uvs.Add(toUV(tri.UV1));
                    m.uvs.Add(toUV(tri.UV2));
                    m.triangles.Add(vertCount + 0);
                    m.triangles.Add(vertCount + 1);
                    m.triangles.Add(vertCount + 2);
                    // Backface
                    m.triangles.Add(vertCount + 0);
                    m.triangles.Add(vertCount + 2);
                    m.triangles.Add(vertCount + 1);
                    /*m.colors.Add(palette[16 * tri.VertexColorPaletteIndex + tri.VertexColorIndex0]);
                    m.colors.Add(palette[16 * tri.VertexColorPaletteIndex + tri.VertexColorIndex1]);
                    m.colors.Add(palette[16 * tri.VertexColorPaletteIndex + tri.VertexColorIndex2]);*/
                }

                // Create GameObjects
                foreach (var k in meshes.Keys) {
                    var blendMode = BitHelpers.ExtractBits(k, 8, 8);
                    var flags = BitHelpers.ExtractBits(k, 16, 16);
                    var curMesh = meshes[k];
                    Mesh unityMesh = new Mesh();
                    unityMesh.SetVertices(curMesh.vertices);
                    unityMesh.SetTriangles(curMesh.triangles, 0);
                    unityMesh.SetColors(curMesh.colors);
                    unityMesh.SetUVs(0, curMesh.uvs);
                    unityMesh.RecalculateNormals();
                    GameObject gao = new GameObject(curMesh.name);
                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                    gao.layer = LayerMask.NameToLayer("3D Collision");
                    gao.transform.SetParent(gaoParent.transform);
                    gao.transform.localScale = Vector3.one;
                    gao.transform.localPosition = Vector3.zero;
                    mf.mesh = unityMesh;
                    switch (blendMode) {
                        case 3:
                            mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentMaterial;
                            break;
                        case 6:
                        case 7:
                            mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial;
                            break;
                        case 8:
                        case 9:
                            mr.material = Controller.obj.levelController.controllerTilemap.unlitAdditiveMaterial;
                            break;
                        default:
                            mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
                            break;
                    }
                    if (curMesh.texture != null) {
                        curMesh.texture.wrapMode = TextureWrapMode.Repeat;
                        mr.material.SetTexture("_MainTex", curMesh.texture);
                    }
                    bool isScroll = BitHelpers.ExtractBits(flags, 1, 5) == 1;
                    if (isScroll || animatedTextures.ContainsKey(k)) {
                        var animTex = gao.AddComponent<AnimatedTextureComponent>();
                        animTex.material = mr.material;
                        if (isScroll) animTex.scrollU = -1f;
                        if (animatedTextures.ContainsKey(k)) {
                            animTex.animatedTextureSpeed = 15;
                            animTex.animatedTextures = animatedTextures[k];
                        }
                    }
                    if (blendMode == 4) { // Collision
                        gao.SetActive(false);
                    }
                }
            }
            return gaoParent;
        }

        public GameObject CreatePVSGameObject(Context context, GBAVV_NitroKart_NGage_PVS pvs) {
            float scale = 8f;
            Vector3 toVertex(GBAVV_NitroKart_NGage_Vertex v) => new Vector3(v.X / scale, v.Z / scale, v.Y / scale);
            Vector2 toUV(GBAVV_NitroKart_NGage_UV uv) => new Vector2(uv.U / (float)0x80, uv.V / (float)0x80);
            var palette = pvs.VertexColorsPalettes.Select(p => p.GetColor()).Select(c => new Color(c.r, c.g, c.b, 1f)).ToArray();

            Dictionary<int, MeshInProgress> meshes = new Dictionary<int, MeshInProgress>();
            Dictionary<int, Texture2D[]> animatedTextures = new Dictionary<int, Texture2D[]>();
            foreach (var tri in pvs.Triangles) {
                var key = tri.TextureIndex | (tri.BlendMode << 8) | (tri.Flags << 16);
                if (!meshes.ContainsKey(key)) {
                    var texs = LoadTextures(pvs.Textures[tri.TextureIndex], pvs.Palettes[tri.TextureIndex], tri.BlendMode, false);
                    if(texs.Length > 1) animatedTextures.Add(key, texs);
                    meshes[key] = new MeshInProgress($"Texture:{tri.TextureIndex} - BlendMode:{tri.BlendMode} - Flags:{tri.Flags}", texs[0]);
                }
                /*
                 *blend modes:
                    0 - Unlit
                    1 - Vertex Color
                    3 - Transparent (0.5)
                    4 - Hidden (collision)

                    6 - also transparent cutout
                    7 - Transparent cutout
                    8 - Additive
                    9 - Additive and transparent

                 flags:
                    32 = scrolling texture
                 */

                var m = meshes[key];
                int vertCount = m.vertices.Count;
                m.vertices.Add(toVertex(pvs.Vertices[tri.Vertex0]));
                m.vertices.Add(toVertex(pvs.Vertices[tri.Vertex1]));
                m.vertices.Add(toVertex(pvs.Vertices[tri.Vertex2]));
                m.uvs.Add(toUV(tri.UV0));
                m.uvs.Add(toUV(tri.UV1));
                m.uvs.Add(toUV(tri.UV2));
                m.triangles.Add(vertCount + 0);
                m.triangles.Add(vertCount + 1);
                m.triangles.Add(vertCount + 2);
                // Backface
                m.triangles.Add(vertCount + 0);
                m.triangles.Add(vertCount + 2);
                m.triangles.Add(vertCount + 1);
                m.colors.Add(palette[16 * tri.VertexColorPaletteIndex + tri.VertexColorIndex0]);
                m.colors.Add(palette[16 * tri.VertexColorPaletteIndex + tri.VertexColorIndex1]);
                m.colors.Add(palette[16 * tri.VertexColorPaletteIndex + tri.VertexColorIndex2]);
            }

            // Create GameObjects
            GameObject gaoParent = new GameObject("Map");
            gaoParent.transform.position = Vector3.zero;
            foreach(var k in meshes.Keys) {
                var blendMode = BitHelpers.ExtractBits(k,8,8);
                var flags = BitHelpers.ExtractBits(k, 16, 16);
                var curMesh = meshes[k];
                Mesh unityMesh = new Mesh();
                unityMesh.SetVertices(curMesh.vertices);
                unityMesh.SetTriangles(curMesh.triangles, 0);
                if (blendMode == 3) {
                    unityMesh.SetColors(curMesh.colors.Select(c => new Color(c.r, c.g, c.b, 0.5f)).ToArray());
                } else {
                    unityMesh.SetColors(curMesh.colors);
                }
                unityMesh.SetUVs(0, curMesh.uvs);
                unityMesh.RecalculateNormals();
                GameObject gao = new GameObject(curMesh.name);

                MeshCollider mc = gao.AddComponent<MeshCollider>();
                Mesh colMesh = new Mesh();
                colMesh.SetVertices(curMesh.vertices);
                colMesh.SetTriangles(curMesh.triangles.Where((x,i) => i % 6 >= 3).ToArray(), 0);
                colMesh.RecalculateNormals();
                mc.sharedMesh = colMesh;


                MeshFilter mf = gao.AddComponent<MeshFilter>();
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                gao.layer = LayerMask.NameToLayer("3D Collision");
                gao.transform.SetParent(gaoParent.transform);
                gao.transform.localScale = Vector3.one;
                gao.transform.localPosition = Vector3.zero;
                mf.mesh = unityMesh;
                switch (blendMode) {
                    case 3:
                        mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentMaterial;
                        break;
                    case 6:
                    case 7:
                        mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial;
                        break;
                    case 8:
                    case 9:
                        mr.material = Controller.obj.levelController.controllerTilemap.unlitAdditiveMaterial;
                        break;
                    default:
                        mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
                        break;
                }
                if (curMesh.texture != null) {
                    curMesh.texture.wrapMode = TextureWrapMode.Repeat;
                    mr.material.SetTexture("_MainTex", curMesh.texture);
                }
                bool isScroll = BitHelpers.ExtractBits(flags, 1, 5) == 1;
                if (isScroll || animatedTextures.ContainsKey(k)) {
                    var animTex = gao.AddComponent<AnimatedTextureComponent>();
                    animTex.material = mr.material;
                    if(isScroll) animTex.scrollU = -1f;
                    if (animatedTextures.ContainsKey(k)) {
                        animTex.animatedTextureSpeed = 15;
                        animTex.animatedTextures = animatedTextures[k];
                    }
                }
                if (blendMode == 4) { // Collision
                    gao.SetActive(false);
                }
            }
            return gaoParent;
        }

        public float GetLevelWidth(GBAVV_NitroKart_NGage_PVS pvs) {
            return pvs.Vertices.Max(v => v.Y);
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Load the data file
            var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

            // Load the exe
            var exe = FileFactory.Read<GBAVV_NitroKart_NGage_ExeFile>(ExeFilePath, context);

            var level = exe.LevelInfos[context.Settings.Level];
            var pop = level.POP;

            var pvs = CreatePVSGameObject(context, level.PVS);
            float levelWidth = GetLevelWidth(level.PVS);
            pvs.transform.position = new Vector3(0,0,-levelWidth / 8f);

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(exe.Scripts);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objManager = new Unity_ObjectManager_GBAVV(context, LoadAnimSets(LoadGFX(context)), null, GBAVV_MapInfo.GBAVV_MapType.Kart, nitroKart_ObjTypeData: exe.NitroKart_ObjTypeData, scripts: exe.Scripts, locPointerTable: loc.Item2);
            objManager.LevelWidthNitroKartNGage = levelWidth;

            var objGroups = new List<(GBAVV_NitroKart_Object[], string)>();

            objGroups.Add((pop.Objects.Objects_Normal, "Normal"));

            if (pop.Objects.Objects_TimeTrial != pop.Objects.Objects_Normal)
                objGroups.Add((pop.Objects.Objects_TimeTrial, "Time Trial"));

            if (pop.Objects.Objects_BossRace != pop.Objects.Objects_Normal)
                objGroups.Add((pop.Objects.Objects_BossRace, "Boss Race"));

            var objects = objGroups.SelectMany((x, i) => x.Item1.Select(o => (Unity_Object)new Unity_Object_GBAVVNitroKart(objManager, o, i))).ToList();

            GameObject gao_3dObjParent = null;

            void replaceObjWith3D(GBAVV_NitroKart_NGage_S3D s3d, int[] objTypes, Vector3? snapToFloorPosition = null)
            {
                var toRemove = new HashSet<Unity_Object>();

                foreach (var o in objects.OfType<Unity_Object_GBAVVNitroKart>().Where(x => objTypes.Contains(x.Object.ObjType)))
                {
                    if (gao_3dObjParent == null) {
                        gao_3dObjParent = new GameObject("3D Objects");
                        gao_3dObjParent.transform.localPosition = Vector3.zero;
                        gao_3dObjParent.transform.localRotation = Quaternion.identity;
                        gao_3dObjParent.transform.localScale = Vector3.one;
                    }
                    var obj = CreateS3DGameObject(context, s3d);
                    obj.transform.SetParent(gao_3dObjParent.transform);
                    const float scale = 8f;
                    var newPosPreConvert = o.Position;
                    var newPos = new Vector3(newPosPreConvert.x / scale, newPosPreConvert.z / scale, -newPosPreConvert.y / scale);
                    obj.transform.position =  newPos;
                    if (snapToFloorPosition.HasValue) {
                        Ray ray = new Ray(newPos + Vector3.up * 100, Vector3.down);
                        var layerMask = 1 << LayerMask.NameToLayer("3D Collision");
                        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
                        if (hits?.Length > 0) {
                            var hit = hits.OrderBy(h => Vector3.Distance(newPos, h.point)).First();
                            obj.transform.position = hit.point + snapToFloorPosition.Value;
                        }
                    }
                    toRemove.Add(o);
                }

                // Remove objects
                foreach (var o in toRemove)
                    objects.Remove(o);
            }

            replaceObjWith3D(exe.S3D_Warp, new ValueRange(0x56, 0x62).EnumerateRange().ToArray(), snapToFloorPosition: Vector3.zero);
            replaceObjWith3D(exe.S3D_Podium, new ValueRange(0x1F, 0x21).EnumerateRange().ToArray(), snapToFloorPosition: Vector3.zero);

            var waypointsGroupIndex = 0;

            void addTrackWaypoints(GBAVV_NitroKart_TrackWaypoint[] waypoints, string groupName, int trackDataIndex, bool snapToFloor = true)
            {
                if (waypoints == null)
                    return;

                if (objGroups.Any(x => x.Item2 == groupName))
                {
                    Vector3 previousPos = Vector3.zero;
                    var objCount = objects.Count;
                    for (int i = 0; i < waypoints.Length; i++) {
                        var w = new Unity_Object_GBAVVNitroKartWaypoint(objManager, waypoints[i], waypointsGroupIndex, trackDataIndex);
                        w.LinkedWayPointIndex = objCount + ((i == waypoints.Length - 1) ? 0 : (i+1));
                        if (snapToFloor) {
                            const float scale = 8f;
                            var posPreConvert = w.Position;
                            var convertedPos = new Vector3(posPreConvert.x, posPreConvert.z, -posPreConvert.y) / scale;
                            Vector3 newPos = convertedPos;
                            Ray ray = new Ray(convertedPos + Vector3.up * 100, Vector3.down);
                            var layerMask = 1 << LayerMask.NameToLayer("3D Collision");
                            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
                            if (hits?.Length > 0) {
                                RaycastHit hit = default;
                                if (i == 0) {
                                    hit = hits.OrderByDescending(h => h.distance).First(); // Take bottom most point
                                } else {
                                    hit = hits.OrderBy(h => Vector3.Distance(previousPos, h.point)/*h.distance*/).First(); // Take point closest to last waypoint
                                }
                                newPos = hit.point;
                                Vector3 backConvertedPos = new Vector3(newPos.x, -newPos.z, newPos.y) * scale;
                                w.Position = backConvertedPos;
                            } else {
                                Vector3 backConvertedPos = new Vector3(newPos.x, -newPos.z, previousPos.y) * scale;
                                w.Position = backConvertedPos;
                            }
                            previousPos = newPos;
                        }
                        objects.Add(w);
                    }
                    waypointsGroupIndex++;
                }
            }

            addTrackWaypoints(pop.TrackData1.TrackWaypoints_Normal, "Normal", 0);
            addTrackWaypoints(pop.TrackData1.TrackWaypoints_TimeTrial, "Time Trial", 0);
            addTrackWaypoints(pop.TrackData1.TrackWaypoints_BossRace, "Boss Race", 0);
            waypointsGroupIndex = 0;
            addTrackWaypoints(pop.TrackData2.TrackWaypoints_Normal, "Normal", 1);
            addTrackWaypoints(pop.TrackData2.TrackWaypoints_TimeTrial, "Time Trial", 1);
            addTrackWaypoints(pop.TrackData2.TrackWaypoints_BossRace, "Boss Race", 1);

            var layers = new List<Unity_Layer>();
            layers.Add(new Unity_Layer_GameObject(true) {
                Name = "Map",
                Graphics = pvs
            });
            if (gao_3dObjParent != null) {
                layers.Add(new Unity_Layer_GameObject(true) {
                    Name = "3D Objects",
                    Graphics = gao_3dObjParent
                });
            }

            for (int i = 0; i < level.ParallaxCount; i++)
            {
                var frames = level.ParallaxData[i].ToTextures(level.ParallaxPalettes[i]);
                layers.Add(new Unity_Layer_Texture
                {
                    Name = $"Parallax {i}",
                    Texture = frames.First(),
                    TextureFrames = frames,
                    AnimSpeed = level.Int_14, // TODO: Is this the speed?
                });
            }

            return new Unity_Level(
                layers: layers.ToArray(),
                /*maps: new Unity_Map[] {
                    new Unity_Map()
                    {
                        Width = 1,
                        Height = 1,
                        TileSet = new Unity_TileSet[]
                        {
                            new Unity_TileSet(8, Color.clear), 
                        },
                        MapTiles = new Unity_Tile[]
                        {
                            new Unity_Tile(new MapTile()), 
                        }
                    }
                },*/
                objManager: objManager,
                eventData: objects,
                cellSize: 8,
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
                    ObjectScale = Vector3.one * 8
                },
                objectGroups: objGroups.Select(x => x.Item2).ToArray(),
                localization: loc.Item1);
        }

        public override UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public override async UniTask LoadFilesAsync(Context context)
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
            @"flc/fakeCrash.flc",
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
            @"gfx\Barin02\start.pal",
            @"gfx\Barin02\start.tex",
            @"gfx\Barin02\garrow.bmp.pal",
            @"gfx\Barin02\garrow.bmp.tex",
            @"gfx\Barin02\B1DSurface03.pal",
            @"gfx\Barin02\B1DSurface03.tex",
            @"gfx\Terra03\HH_wall0a.pal",
            @"gfx\Terra03\HH_wall0a.tex",
            @"gfx\Terra03\TC_surface03.pal",
            @"gfx\Terra03\TC_surface03.tex",
        };

        public override long ObjTypesCount => 113;
        public override uint ObjTypesPointer => 0x1001aa2c;

        public override uint?[] ObjTypesDataPointers => new uint?[]
        {
            0x1008D660, // 0
            0x1008D660, // 1
            0x1008D660, // 2
            null, // 3
            null, // 4
            null, // 5
            null, // 6
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
            null, // 12
            0x1008D660, // 13
            null, // 14
            null, // 15
            0x1008D660, // 16
            0x1008D660, // 17
            0x1008D660, // 18
            0x1008D660, // 19
            0x1008D660, // 20
            0x1008D660, // 21
            0x1008D660, // 22
            0x1008D660, // 23
            0x1008D660, // 24
            0x1008D660, // 25
            0x1008D660, // 26
            0x1008D660, // 27
            0x1008D660, // 28
            0x1008D660, // 29
            0x1008D660, // 30
            null, // 31
            null, // 32
            null, // 33
            0x1006867C, // 34
            0x100686BC, // 35
            0x100686FC, // 36
            0x1006863C, // 37
            0x100684FC, // 38
            0x1006853C, // 39
            0x1006873C, // 40
            0x1006857C, // 41
            0x100685BC, // 42
            0x100685FC, // 43
            0x10087B98, // 44
            0x1008D660, // 45
            0x1008D660, // 46
            0x1008D660, // 47
            0x1008D660, // 48
            0x1008D660, // 49
            0x1008D660, // 50
            0x1008D660, // 51
            0x1008D660, // 52
            0x1008D660, // 53
            0x1008D660, // 54
            0x1008D660, // 55
            0x1008D660, // 56
            0x1008D660, // 57
            0x1008D660, // 58
            0x1008D660, // 59
            0x1008D660, // 60
            0x1008D660, // 61
            0x1008D660, // 62
            0x1008D660, // 63
            0x1008D660, // 64
            0x1008D660, // 65
            0x1008D660, // 66
            0x1008D660, // 67
            0x1008D660, // 68
            0x1008D660, // 69
            0x1008D660, // 70
            0x1008D660, // 71
            0x1008D660, // 72
            null, // 73
            0x10087BF8, // 74
            0x10087C28, // 75
            0x10087C64, // 76
            0x1008D660, // 77
            0x1008D660, // 78
            0x1008D660, // 79
            0x1008D660, // 80
            0x1008D660, // 81
            0x1008D660, // 82
            0x1008D660, // 83
            0x1008D660, // 84
            0x1008D660, // 85
            null, // 86
            null, // 87
            null, // 88
            null, // 89
            null, // 90
            null, // 91
            null, // 92
            null, // 93
            null, // 94
            null, // 95
            null, // 96
            null, // 97
            null, // 98
            null, // 99
            0x1008D444, // 100
            0x1008D480, // 101
            0x1008D4B0, // 102
            0x1008D570, // 103
            0x1008D540, // 104
            0x1008D5A0, // 105
            0x1008D5D0, // 106
            0x1008D600, // 107
            0x1008D630, // 108
            null, // 109
            0x1008D660, // 110
            0x1008D660, // 111
            0x1008D660, // 112
        };

        public override uint[] GraphicsDataPointers => null;

        public override uint[] ScriptPointers => new uint[]
        {
            0x10067804, // script_waitForInputOrTime
            0x10083C54, // movie_intro
            0x10083DC8, // movie_garage
            0x10083E58, // movie_credits
            0x10083F90, // movie_gameIntro
            0x100845CC, // movie_earthBossIntro
            0x100847F8, // movie_earthBossCrashWin
            0x10084AAC, // movie_earthBossEvilWin
            0x10084D00, // movie_barinBossIntro
            0x10084F2C, // movie_barinBossCrashWin
            0x10085180, // movie_barinBossEvilWin
            0x10085450, // movie_fenomBossIntro
            0x10085690, // movie_fenomBossCrashWin
            0x10085914, // movie_fenomBossEvilWin
            0x10085BC8, // movie_tekneeBossIntro
            0x10085DB8, // movie_tekneeBossCrashWin
            0x10085F30, // movie_tekneeBossEvilWin
            0x10086088, // movie_veloBossIntro
            0x100861B4, // movie_veloBossCrashWin
            0x100865E0, // movie_veloBossEvilWin
            0x10086824, // SCRIPT_pagedTextLoop
            0x1008BB7C, // script_license
            0x1008BBBC, // script_intro
            0x1008BBFC, // script_credits
            0x1008BC44, // script_earthBossIntro
            0x1008BC90, // script_earthBossCrashWin
            0x1008BCD8, // script_earthBossEvilWin
            0x1008BD20, // script_barinBossIntro
            0x1008BD6C, // script_barinBossCrashWin
            0x1008BDB4, // script_barinBossEvilWin
            0x1008BDFC, // script_fenomBossIntro
            0x1008BE48, // script_fenomBossCrashWin
            0x1008BE90, // script_fenomBossEvilWin
            0x1008BED8, // script_tekneeBossIntro
            0x1008BF24, // script_tekneeBossCrashWin
            0x1008BF70, // script_tekneeBossEvilWin
            0x1008BFB8, // script_veloBossIntro
            0x1008C000, // script_veloBossCrashWin
            0x1008C048, // script_veloBossEvilWin
        };
    }
}