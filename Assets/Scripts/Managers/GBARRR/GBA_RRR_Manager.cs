using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBA_RRR_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(2, 5).Append(29).ToArray()), // Child
            new GameInfo_World(1, Enumerable.Range(7, 5).ToArray()), // Forest
            new GameInfo_World(2, Enumerable.Range(12, 5).ToArray()), // Organic Cave
            new GameInfo_World(3, Enumerable.Range(17, 6).Append(31).ToArray()), // Sweets
            new GameInfo_World(4, Enumerable.Range(0, 2).Concat(Enumerable.Range(23, 5)).ToArray()), // Dark
            new GameInfo_World(5, Enumerable.Range(30, 1).ToArray()), // Menu

            // Special:
            new GameInfo_World(10, Enumerable.Range(0, 3).ToArray()), // Village (these are all actually level 28, but since there are 3 variants in a separate array it's easier to separate them like this)
            new GameInfo_World(11, Enumerable.Range(0, 3).ToArray()), // Mode7
            new GameInfo_World(12, Enumerable.Range(0, 1).ToArray()), // Unused Mode7
            new GameInfo_World(13, Enumerable.Range(0, 13).ToArray()), // Menu
        });

        public enum GameMode
        {
            Game,
            Village,
            Mode7,
            Mode7Unused,
            Menu
        }

        public static GameMode GetCurrentGameMode(GameSettings s)
        {
            switch (s.World)
            {
                case 10:
                    return GameMode.Village;
                case 11:
                    return GameMode.Mode7;
                case 12:
                    return GameMode.Mode7Unused;
                case 13:
                    return GameMode.Menu;
                default:
                    return GameMode.Game;
            }
        }

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, ExportFlags.Normal)), 
            new GameAction("Export Vignette", false, true, (input, output) => ExportBlocksAsync(settings, output, ExportFlags.Vignette)),
            new GameAction("Export Categorized & Converted Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, ExportFlags.All)),
            new GameAction("Export Mode7 Sprites", false, true, (input, output) => ExportMode7SpritesAsync(settings, output)),
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

                await LoadFilesAsync(context);

                // Load the rom
                var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);
                var pointerTable = PointerTables.GBARRR_PointerTable(s.GameSettings.GameModeSelection, rom.Offset.file);
                Pointer<GAX2_Instrument>[] instruments = null;
                s.DoAt(new Pointer(0x0805C8EC, rom.Offset.file), () => {
                    instruments = s.SerializePointerArray<GAX2_Instrument>(instruments, 156, resolve: true, name: nameof(instruments));
                });
                s.DoAt(pointerTable[GBARRR_Pointer.MusicSampleTable], () => {
                    var sampleTable = s.SerializeObject<GAX2_SampleTable>(default, onPreSerialize: st => st.Length = 141, name: "SampleTable1");
                    string outPath = outputPath + "/MusicSamples/";
                    for (int i = 0; i < sampleTable.Length; i++) {
                        var e = sampleTable.Entries[i];
                        var instr = instruments.FirstOrDefault(ins => ins.Value != null && ins.Value.Sample == i+1);
                        ExportSample(outPath, $"{i}_{e.SampleOffset.AbsoluteOffset:X8}", e.Sample, 15769, 2);
                    }
                });
                s.DoAt(pointerTable[GBARRR_Pointer.SoundEffectSampleTable], () => {
                    var sampleTable = s.SerializeObject<GAX2_SampleTable>(default, onPreSerialize: st => st.Length = 186, name: "SampleTable2");
                    string outPath = outputPath + "/SoundEffects/";
                    for (int i = 0; i < sampleTable.Length; i++) {
                        var e = sampleTable.Entries[i];
                        ExportSample(outPath, $"{i}_{e.SampleOffset.AbsoluteOffset:X8}", e.Sample, 15769, 1);
                    }
                });
                uint[] ptrs_eu = new uint[] {
                    0x083C4DA4,
                    0x083C54CC,
                    0x083C66AC,
                    0x083C769C,
                    0x083C8E24,
                    0x083C9334,
                    0x083CA3D4,
                    0x083CA7E4,
                    0x083CAD88,
                    0x083CB1B4,
                    0x083CBEF0,
                    0x083CC470,
                    0x083CC6F8,
                    0x083CC8D0,
                    0x083CCB24,
                    0x083CCCE0,
                    0x083CCE70,
                    0x083CD080,
                    0x083CD274,
                    0x083CD490,
                    0x083CD6E0,
                    0x083CD8F4,
                    0x083CDB90,
                    0x083CDCCC,
                    0x083CDF3C,
                    0x083CE104,
                    0x083CE2A0,
                    0x083CE4D0,
                    0x083CE6F0,
                    0x083CE890,
                    0x083CEA88,
                    0x083CEC64,
                    0x083CEE70,
                    0x083CF06C,
                    0x083CF290,
                    0x083CF484,
                    0x083CF6E0,
                    0x083CF974,
                    0x083CFBA8,
                    0x083D05A4,
                    0x083D0938,
                    0x083D0F80,
                    0x083D2984,
                    0x083D30E4,
                    0x083D33E8,
                    0x083D37C0,
                    0x083D3F98
                };
                uint[] ptrs_us = new uint[] {
                    0x083C4AC4,
                    0x083C51EC,
                    0x083C63CC,
                    0x083C73BC,
                    0x083C8B44,
                    0x083C9054,
                    0x083CA0F4,
                    0x083CA504,
                    0x083CAAA8,
                    0x083CAED4,
                    0x083CBC10,
                    0x083CC190,
                    0x083CC418,
                    0x083CC5F0,
                    0x083CC844,
                    0x083CCA00,
                    0x083CCB90,
                    0x083CCDA0,
                    0x083CCF94,
                    0x083CD1B0,
                    0x083CD400,
                    0x083CD614,
                    0x083CD8B0,
                    0x083CD9EC,
                    0x083CDC5C,
                    0x083CDE24,
                    0x083CDFC0,
                    0x083CE1F0,
                    0x083CE410,
                    0x083CE5B0,
                    0x083CE7A8,
                    0x083CE984,
                    0x083CEB90,
                    0x083CED8C,
                    0x083CEFB0,
                    0x083CF1A4,
                    0x083CF400,
                    0x083CF694,
                    0x083CF8C8,
                    0x083D02C4,
                    0x083D0658,
                    0x083D0CA0,
                    0x083D26A4,
                    0x083D2E04,
                    0x083D3108,
                    0x083D34E0,
                    0x083D3CB8
                };
                uint[] ptrs = s.GameSettings.GameModeSelection == GameModeSelection.RaymanRavingRabbidsGBAUS ? ptrs_us : ptrs_eu;
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
                }
                /*s.DoAt(pointerTable[GBARRR_Pointer.MusicTable], () => {
                    var musicTable = s.SerializePointerArray<GBARRR_MusicTableEntry>(default, 0x1f, resolve: true, name: "MusicTable");
                    // For each entry
                    GBARRR_MidiWriter w = new GBARRR_MidiWriter();
                    Directory.CreateDirectory(Path.Combine(outputPath, "midi"));
                    for (int i = 0; i < musicTable.Length; i++) {
                        w.Write(musicTable[i].Value,
                            Path.Combine(outputPath, "midi",
                            $"{musicTable[i].Value.ParsedName}.mid"));
                    }
                });*/
            }
        }

        [Flags]
        public enum ExportFlags {
            Normal = 1 << 0,
            Vignette = 1 << 1,
            Graphics = 1 << 2,
            Palettes = 1 << 3,
            LevelBlocks = 1 << 4,
            Tilesets = 1 << 5,
            AdditionalBlocks = 1 << 6,

            All = Normal | Vignette | Graphics | Palettes | LevelBlocks | Tilesets | AdditionalBlocks
        }

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputPath, ExportFlags flags, bool includeAbsolutePointer = true)
        {
            const int vigWidth = 240;
            const int vigHeight = 160;

            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                // Load the rom
                var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);


                var lvlBlocks = rom.LevelInfo.SelectMany(x => new uint[]
                {
                    // Tilesets
                    x.LevelTilesetIndex, x.BG0TilesetIndex, x.FGTilesetIndex, x.BG1TilesetIndex,
                    // Maps & scene
                    x.BG0MapIndex, x.BG1MapIndex, x.CollisionMapIndex, x.LevelMapIndex, x.FGMapIndex, x.ObjectArrayIndex,
                    // Palette
                    x.SpritePaletteIndex
                }).ToArray();

                BaseColor[] pal = flags.HasFlag(ExportFlags.Graphics) ? Util.CreateDummyPalette(256, true, wrap: 16) : null;

                // Helper
                void ExportConvertedMode7Block(string outPath, uint length) {
                    bool exported = false;
                    Pointer blockOff = s.CurrentPointer;

                    // Graphics
                    /*if (!exported && flags.HasFlag(ExportFlags.Graphics)) {
                        s.Goto(blockOff);
                        try {
                            var gb = s.SerializeObject<GBARRR_GraphicsBlock>(default, name: $"GraphicsBlock[{i}]");
                            if (gb.Count != 0) {
                                int tileDataSize = gb.TileData[0].Length;
                                if (gb.TileData.All(td => td.Length == tileDataSize) && Math.Sqrt(tileDataSize * 2) % 1 == 0) {
                                    //gb.TileSize = (uint)Mathf.RoundToInt(Mathf.Sqrt(tileDataSize * 2));
                                    ExportSpriteFrames(gb, pal, 0, Path.Combine(outPath, $"ActorGraphics/{i}_{absoluteOffset:X8}/"));
                                    exported = true;
                                }
                            }
                        } catch (Exception ex) {
                        }
                    }*/

                    // Palettes
                    if (!exported && flags.HasFlag(ExportFlags.Palettes)) {
                        s.Goto(blockOff);
                        if (length == 0x200) {
                            var p = s.SerializeObject<GBARRR_Palette>(default, name: $"Palette");
                            PaletteHelpers.ExportPalette(Path.Combine(outPath, $"Palette.png"), p.Palette.Select(x => new CustomColor(x.Red, x.Green, x.Blue)).ToArray(), optionalWrap: 16);
                            exported = true;
                            if (p != null) pal = p.Palette;
                        }
                    }

                    // Tilesets
                    if (!exported && flags.HasFlag(ExportFlags.Tilesets)) {
                        s.Goto(blockOff);
                        if ((length % 0x20) == 0) {
                            var bytes = s.SerializeArray<byte>(default, length, name: $"Block");
                            try
                            {
                                var ts = LoadTileSet(bytes, true, pal, palCount: 16);
                                Util.ByteArrayToFile(Path.Combine(outPath, $"Tileset_4.png"), ts.Tiles[0].texture.EncodeToPNG());
                                exported = true;
                            } catch (Exception) {
                            }
                            try
                            {
                                var ts = LoadTileSet(bytes, false, pal, palCount: 1);
                                Util.ByteArrayToFile(Path.Combine(outPath, $"Tileset_8.png"), ts.Tiles[0].texture.EncodeToPNG());
                                exported = true;
                            } catch (Exception) {
                            }
                        }
                    }

                    // Binary
                    if (flags.HasFlag(ExportFlags.Normal)) {
                        s.Goto(blockOff);
                        var bytes = s.SerializeArray<byte>(default, length, name: $"Block");

                        Util.ByteArrayToFile(Path.Combine(outPath, $"Binary.dat"), bytes);
                    }
                }

                void ExportMode7Block(Pointer ptr, string path, bool compressed = true, uint length = 0x200) {
                    s.DoAt(ptr, () => {
                        if (compressed) {
                            s.DoEncoded(new RNCEncoder(hasHeader: false), () => {
                                ExportConvertedMode7Block($"{outputPath}/Additional/{path}", s.CurrentLength);
                            });
                        } else {
                            ExportConvertedMode7Block($"{outputPath}/Additional/{path}", length);
                        }
                    });
                }
                void ExportMode7Array(Pointer ptr, string path, int length, bool compressed = true, uint blockLength = 0x200) {
                    s.DoAt(ptr, () => {
                        Pointer[] ptrs = s.SerializePointerArray(null, length, name: path);
                        for (int i = 0; i < length; i++) {
                            ExportMode7Block(ptrs[i], $"{path}/{i}", compressed: compressed, length: blockLength);
                        }
                    });
                }

                if (flags.HasFlag(ExportFlags.AdditionalBlocks)) {
                    var pointerTable = PointerTables.GBARRR_PointerTable(s.GameSettings.GameModeSelection, rom.Offset.file);

                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_Waypoints], nameof(GBARRR_Pointer.Mode7_Waypoints), 3, compressed: false, 0x7D0);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Palette_Mode7Sprites_2], nameof(GBARRR_Pointer.Palette_Mode7Sprites_2), 3, compressed: false);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Palette_Mode7Sprites_1], nameof(GBARRR_Pointer.Palette_Mode7Sprites_1), 3, compressed: false, 0x80);

                    ExportMode7Block(pointerTable[GBARRR_Pointer.Sprites_Compressed_Unk], nameof(GBARRR_Pointer.Sprites_Compressed_Unk));
                    ExportMode7Block(pointerTable[GBARRR_Pointer.Sprites_Compressed_GameOver], nameof(GBARRR_Pointer.Sprites_Compressed_GameOver));
                    ExportMode7Block(pointerTable[GBARRR_Pointer.RNC_2], nameof(GBARRR_Pointer.RNC_2));
                    ExportMode7Block(pointerTable[GBARRR_Pointer.RNC_3], nameof(GBARRR_Pointer.RNC_3));
                    ExportMode7Block(pointerTable[GBARRR_Pointer.Sprites_PauseMenu_Carrot], nameof(GBARRR_Pointer.Sprites_PauseMenu_Carrot));
                    ExportMode7Block(pointerTable[GBARRR_Pointer.Sprites_Compressed_MainMenu], nameof(GBARRR_Pointer.Sprites_Compressed_MainMenu));

                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_Sprites_World], nameof(GBARRR_Pointer.Mode7_Sprites_World), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_CollisionTypesArray], nameof(GBARRR_Pointer.Mode7_CollisionTypesArray), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_Objects], nameof(GBARRR_Pointer.Mode7_Objects), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_Sprites_HUD], nameof(GBARRR_Pointer.Mode7_Sprites_HUD), 3);

                    ExportMode7Block(pointerTable[GBARRR_Pointer.Palette_Mode7Sprites_0], nameof(GBARRR_Pointer.Palette_Mode7Sprites_0), compressed: false);

                    for (int i = 0; i < 15; i++) {
                        // Export palette first so it's cached
                        ExportMode7Array(pointerTable[GBARRR_Pointer.MenuArray] + i * 0xC + 0x8, $"{nameof(GBARRR_Pointer.MenuArray)}/Pal/{i}", 1, compressed: false);
                        try {
                            ExportMode7Array(pointerTable[GBARRR_Pointer.MenuArray] + i * 0xC, $"{nameof(GBARRR_Pointer.MenuArray)}/{i}", 2, compressed: IsMenuCompressed(i));
                        } catch (Exception) { }
                    }
                    pal = Util.CreateDummyPalette(256, true, wrap: 16);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_MapTiles], nameof(GBARRR_Pointer.Mode7_MapTiles), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_BG1Tiles], nameof(GBARRR_Pointer.Mode7_BG1Tiles), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_Bg1Map], nameof(GBARRR_Pointer.Mode7_Bg1Map), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_BG0Tiles], nameof(GBARRR_Pointer.Mode7_BG0Tiles), 3);
                    ExportMode7Array(pointerTable[GBARRR_Pointer.Mode7_BG0Map], nameof(GBARRR_Pointer.Mode7_BG0Map), 3);
                }

                var animTable = GBARRR_Tables.GetAnimBlocks.SelectMany(x => x).ToArray();

                // Enumerate every block in the offset table
                for (uint i = 0; i < rom.OffsetTable.OffsetTableCount; i++)
                {
                    // Get the offset
                    var offset = rom.OffsetTable.OffsetTable[i];

                    var append = includeAbsolutePointer ? $"_{(rom.OffsetTable.Offset + offset.BlockOffset).AbsoluteOffset:X8}" : String.Empty;

                    rom.OffsetTable.DoAtBlock(context, i, size =>
                    {
                        Pointer blockOff = s.CurrentPointer;
                        bool exported = false;
                        string outPath = outputPath;

                        // Level blocks
                        if (flags.HasFlag(ExportFlags.LevelBlocks)) {
                            if (lvlBlocks.Contains(i)) {
                                outPath += "/LevelBlocks/";
                            }
                        }

                        // Vignette
                        if (!exported && flags.HasFlag(ExportFlags.Vignette)) {
                            s.Goto(blockOff);
                            if (size == (256 * 2) + (vigWidth * vigHeight)) {
                                var vig = s.SerializeObject<GBARRR_Vignette>(default, name: $"Vignette[{i}]");

                                var tex = TextureHelpers.CreateTexture2D(vigWidth, vigHeight, true);

                                foreach (var c in vig.Palette)
                                    c.Alpha = 255;

                                var index = 0;
                                for (int y = 0; y < vigHeight; y++) {
                                    for (int x = 0; x < vigWidth; x++) {
                                        tex.SetPixel(x, vigHeight - y - 1, vig.Palette[vig.ImgData[index]].GetColor());

                                        index++;
                                    }
                                }

                                tex.Apply();
                                exported = true;
                                Util.ByteArrayToFile(Path.Combine(outPath, $"Vignette/{i}{append}.png"), tex.EncodeToPNG());
                            }
                        }

                        // Graphics
                        if (!exported && flags.HasFlag(ExportFlags.Graphics) && animTable.Any(x => x.AnimBlockIndex == i)) {
                            s.Goto(blockOff);
                            try {
                                var a = animTable.First(x => x.AnimBlockIndex == i);
                                var gb = s.SerializeObject<GBARRR_GraphicsBlock>(default, name: $"GraphicsBlock[{i}]");
                                if (FixedSpriteSizes.ContainsKey((int)i)) {
                                    gb.AnimationAssemble = FixedSpriteSizes[(int)i];
                                }
                                if (gb.Count != 0) 
                                {
                                    RGBA5551Color[] p = null;
                                    rom.OffsetTable.DoAtBlock(context, a.PalBlockIndex, blockSize => p = s.SerializeObjectArray<RGBA5551Color>(default, 256));
                                    int tileDataSize = gb.TileData[0].Length;
                                    if (gb.TileData.All(td => td.Length == tileDataSize) && Math.Sqrt(tileDataSize * 2) % 1 == 0) {
                                        //gb.TileSize = (uint)Mathf.RoundToInt(Mathf.Sqrt(tileDataSize * 2));
                                        ExportSpriteFrames(gb, p, a.SubPalette, Path.Combine(outPath, $"ActorGraphics/{i}{append}/"), includeAbsolutePointer, 3);
                                        exported = true;
                                    }/* else {
                                        UnityEngine.Debug.Log($"Possible Graphics block {i}: {Math.Sqrt(tileDataSize * 2)} - {tileDataSize}");
                                    }*/
                                }
                            } catch (Exception) {
                            }
                        }

                        // Tilesets
                        if (!exported && flags.HasFlag(ExportFlags.Tilesets)) {
                            s.Goto(blockOff);
                            if (size > 0x200 && ((size - 0x200) % 0x20) == 0) {
                                try {
                                    var tileset = s.SerializeObject<GBARRR_Tileset>(default, onPreSerialize: t => t.BlockSize = size, name: $"Tileset[{i}]");
                                    int length = tileset.Data.Length;
                                    bool is4Bit = (length % 0x40 != 0);
                                    var ts = LoadTileSet(tileset.Data, is4Bit, tileset.Palette, palCount: is4Bit ? 16 : 1);
                                    Util.ByteArrayToFile(Path.Combine(outPath, $"Tilesets/{i}{append}.png"), ts.Tiles[0].texture.EncodeToPNG());
                                    exported = true;
                                } catch (Exception) {
                                }
                            }
                        }

                        // Palettes
                        if (!exported && flags.HasFlag(ExportFlags.Palettes)) {
                            s.Goto(blockOff);
                            if (size == 0x200)
                            {
                                var p = s.SerializeObject<GBARRR_Palette>(default, name: $"Palette[{i}]");
                                PaletteHelpers.ExportPalette(Path.Combine(outPath, $"Palette/{i}{append}.png"), p.Palette.Select(x => new CustomColor(x.Red, x.Green, x.Blue)).ToArray(), optionalWrap: 16);
                                exported = true;
                                if (p != null)
                                    pal = p.Palette;
                            }
                        }

                        // Binary
                        if (!exported && flags.HasFlag(ExportFlags.Normal)) {
                            s.Goto(blockOff);
                            var bytes = s.SerializeArray<byte>(default, size, name: $"Block[{i}]");

                            Util.ByteArrayToFile(Path.Combine(outPath, $"Uncategorized/{i}{append}{(s.CurrentPointer.file is StreamFile ? "_decompressed" : String.Empty)}.dat"), bytes);
                        }
                    });
                }
            }
        }

        public async UniTask<Mode7Data> LoadMode7SpritesAsync(Context context) {
            var s = context.Deserializer;
            var romPath = GetROMFilePath;
            var pointerTable = PointerTables.GBARRR_PointerTable(context.Settings.GameModeSelection, context.GetFile(romPath));

            // Read animation frame indices
            Controller.DetailedState = $"Loading animation frames";
            await Controller.WaitIfNecessary();

            var animationFrameIndicesPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_AnimationFrameIndices], () => s.SerializePointerArray(default, 46, name: "AnimationFrameIndices"));
            ushort[][] animationFrameIndices = new ushort[animationFrameIndicesPointers.Length][];
            Dictionary<Pointer, ushort[]> serializedAnimFrameIndicesPointers = new Dictionary<Pointer, ushort[]>();
            for (int i = 0; i < animationFrameIndices.Length; i++) {
                var ptr = animationFrameIndicesPointers[i];
                if (!serializedAnimFrameIndicesPointers.ContainsKey(ptr)) {
                    Pointer nextPtr = animationFrameIndicesPointers.OrderBy(p => p.AbsoluteOffset).FirstOrDefault(p => p.AbsoluteOffset > ptr.AbsoluteOffset);
                    if (nextPtr == null) {
                        nextPtr = animationFrameIndicesPointers[animationFrameIndicesPointers.Length - 1] + 2; // Last one only has one frame
                    }
                    serializedAnimFrameIndicesPointers[ptr] = s.DoAt(ptr, () => s.SerializeArray<ushort>(default, (nextPtr - ptr) / 2, name: $"{nameof(animationFrameIndices)}[{i}]"));
                }
                animationFrameIndices[i] = serializedAnimFrameIndicesPointers[ptr];
            }

            // Read animation sets, determine frame count of each
            var animationPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_Animations], () => s.SerializePointerArray(default, 49, name: "Animations"));
            GBARRR_Mode7AnimSet[] animSets = new GBARRR_Mode7AnimSet[animationPointers.Length];
            Dictionary<Pointer, GBARRR_Mode7AnimSet> serializedAnimSets = new Dictionary<Pointer, GBARRR_Mode7AnimSet>();
            for (int i = 0; i < animSets.Length; i++) {
                var ptr = animationPointers[i];
                if (!serializedAnimSets.ContainsKey(ptr)) {
                    var animationIndicesWithThisAnimSet = animationPointers.Select((x, j) => x == ptr ? j : -1).Where(x => x != -1 && x < animationFrameIndices.Length).ToArray();
                    var maxFrameIndex = animationIndicesWithThisAnimSet.Select(x => animationFrameIndices[x].Max()).Max();
                    var count = (maxFrameIndex + 1) * 2; // Why times 2?
                    serializedAnimSets[ptr] = s.DoAt(ptr, () => s.SerializeObject<GBARRR_Mode7AnimSet>(default, onPreSerialize: x => x.Length = count, name: $"{nameof(animSets)}[{i}]"));
                }
                animSets[i] = serializedAnimSets[ptr];
            }

            // Mode7
            Controller.DetailedState = $"Loading VRAM configurations (Mode7)";
            await Controller.WaitIfNecessary();
            // Read & export graphics for Mode7
            var hudPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_Sprites_HUD], () => s.SerializePointerArray(default, 3, name: "HUDSpritePointers"));
            var worldPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_Sprites_World], () => s.SerializePointerArray(default, 3, name: "WorldSpritePointers"));
            var raymanPointers = s.DoAt(pointerTable[GBARRR_Pointer.Sprites_Mode7Rayman], () => s.SerializePointerArray(default, 40, name: "RaymanSpritePointers"));
            var lumCountPointers = s.DoAt(pointerTable[GBARRR_Pointer.Sprites_Mode7UI_LumCount], () => s.SerializePointerArray(default, 100, name: "LumCountSpritePointers"));
            var totalLumCountPointers = s.DoAt(pointerTable[GBARRR_Pointer.Sprites_Mode7UI_TotalLumCount], () => s.SerializePointerArray(default, 100, name: "TotalLumCountSpritePointers"));
            var palette2Pointers = s.DoAt(pointerTable[GBARRR_Pointer.Palette_Mode7Sprites_2], () => s.SerializePointerArray(default, 3, name: "Palette2Pointers"));
            var palette1Pointers = s.DoAt(pointerTable[GBARRR_Pointer.Palette_Mode7Sprites_1], () => s.SerializePointerArray(default, 3, name: "Palette1Pointers"));
            GBARRR_Palette palette0 = s.DoAt(pointerTable[GBARRR_Pointer.Palette_Mode7Sprites_0], () => s.SerializeObject<GBARRR_Palette>(default, name: "Palette0"));

            // Read per-frame graphics
            var raymanGraphics = new byte[raymanPointers.Length][];
            for (int f = 0; f < raymanPointers.Length; f++) {
                raymanGraphics[f] = s.DoAt(raymanPointers[f], () => s.SerializeArray<byte>(raymanGraphics[f], 0x800, name: $"{nameof(raymanGraphics)}[{f}]"));
            }
            var lumCountGraphics = new byte[lumCountPointers.Length][];
            for (int f = 0; f < lumCountPointers.Length; f++) {
                lumCountGraphics[f] = s.DoAt(lumCountPointers[f], () => s.SerializeArray<byte>(lumCountGraphics[f], 0x100, name: $"{nameof(lumCountGraphics)}[{f}]"));
            }
            var totalLumCountGraphics = new byte[totalLumCountPointers.Length][];
            for (int f = 0; f < totalLumCountPointers.Length; f++) {
                totalLumCountGraphics[f] = s.DoAt(totalLumCountPointers[f], () => s.SerializeArray<byte>(totalLumCountGraphics[f], 0x100, name: $"{nameof(totalLumCountGraphics)}[{f}]"));
            }
            var vramConfigs = new Dictionary<Mode7Config, Mode7VRAMConfiguration>();

            // Read & export per level 
            {
                int levelIndex = 0; // it's the same data for all 3 levels
                byte[] hudSprites = null;
                s.DoAt(hudPointers[levelIndex], () => {
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => hudSprites = s.SerializeArray<byte>(hudSprites, s.CurrentLength, name: nameof(hudSprites)));
                });
                byte[] worldSprites = null;
                s.DoAt(worldPointers[levelIndex], () => {
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => worldSprites = s.SerializeArray<byte>(worldSprites, s.CurrentLength, name: nameof(worldSprites)));
                });
                var palette2 = s.DoAt(palette2Pointers[levelIndex], () => s.SerializeObject<GBARRR_Palette>(default, name: "Palette2"));
                var palette1 = s.DoAt(palette1Pointers[levelIndex], () => s.SerializeObject<GBARRR_Palette>(default, name: "Palette1"));

                // Create merged palette
                var newPalette = new RGBA5551Color[256];
                Array.Copy(palette2.Palette, newPalette, 0x100);
                Array.Copy(palette1.Palette, newPalette, 0x80);
                Array.Copy(palette0.Palette, newPalette, 0x10);
                //PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"Level_{levelIndex}", $"Palette.png"), newPalette, optionalWrap: 16);

                List<Mode7VRAMEntry> vram = new List<Mode7VRAMEntry>();
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010000, IsPerFrame = true, PerFrameImageData = raymanGraphics });
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010900, IsPerFrame = true, PerFrameImageData = lumCountGraphics });
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010800, IsPerFrame = true, PerFrameImageData = totalLumCountGraphics });
                vram.Add(new Mode7VRAMEntry() { Address = 0x06015000, ImageData = worldSprites });
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010a00, ImageData = hudSprites });
                vramConfigs[Mode7Config.Mode7] = new Mode7VRAMConfiguration() { Palette = newPalette, VRAM = vram.ToArray() };
                //await ExportAllAnimSets(Path.Combine(outputPath, $"Level_{i}"), vram.ToArray(), newPalette);
            }

            // Pause Menu
            Controller.DetailedState = $"Loading VRAM configurations (Pause)";
            await Controller.WaitIfNecessary();
            {
                byte[] menuSprites0 = null;
                s.DoAt(pointerTable[GBARRR_Pointer.Sprites_PauseMenu_Carrot], () => {
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => menuSprites0 = s.SerializeArray<byte>(menuSprites0, s.CurrentLength, name: nameof(menuSprites0)));
                });

                var menuPointers = s.DoAt(pointerTable[GBARRR_Pointer.Sprites_PauseMenu], () => s.SerializePointerArray(default, 12, name: "PauseMenuSpritePointers"));
                var menuGraphics = new byte[menuPointers.Length][];
                for (int f = 0; f < menuPointers.Length; f++) {
                    menuGraphics[f] = s.DoAt(menuPointers[f], () => s.SerializeArray<byte>(menuGraphics[f], 0xC80 * 2, name: $"{nameof(menuGraphics)}[{f}]"));
                }

                List<Mode7VRAMEntry> vram = new List<Mode7VRAMEntry>();
                vram.Add(new Mode7VRAMEntry() { Address = 0x06016900, ImageData = menuSprites0 });
                vram.Add(new Mode7VRAMEntry() { Address = 0x06015000, IsPerFrame = true, PerFrameImageData = menuGraphics });
                GBARRR_Palette palette = s.DoAt(pointerTable[GBARRR_Pointer.Palette_PauseMenuSprites], () => s.SerializeObject<GBARRR_Palette>(default, name: "MenuPalette"));

                //PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"PauseMenu", $"Palette.png"), palette.Palette, optionalWrap: 16);

                vramConfigs[Mode7Config.PauseMenu] = new Mode7VRAMConfiguration() { Palette = palette.Palette, VRAM = vram.ToArray() };
                //await ExportAllAnimSets(Path.Combine(outputPath, $"PauseMenu"), vram.ToArray(), palette.Palette);
            }

            // Game over
            Controller.DetailedState = $"Loading VRAM configurations (Game Over)";
            await Controller.WaitIfNecessary();
            {
                byte[] gameOverCompressedSprites = null;
                s.DoAt(pointerTable[GBARRR_Pointer.Sprites_Compressed_GameOver], () => {
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => gameOverCompressedSprites = s.SerializeArray<byte>(gameOverCompressedSprites, s.CurrentLength, name: nameof(gameOverCompressedSprites)));
                });

                var gameOverPointers = s.DoAt(pointerTable[GBARRR_Pointer.Sprites_GameOver], () => s.SerializePointerArray(default, 47, name: "GameOverSpritePointers"));
                var gameOverGraphics = new byte[gameOverPointers.Length][];
                for (int f = 0; f < gameOverPointers.Length; f++) {
                    gameOverGraphics[f] = s.DoAt(gameOverPointers[f], () => s.SerializeArray<byte>(gameOverGraphics[f], 0x400 * 2, name: $"{nameof(gameOverGraphics)}[{f}]"));
                }

                List<Mode7VRAMEntry> vram = new List<Mode7VRAMEntry>();
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010800, ImageData = gameOverCompressedSprites });
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010000, IsPerFrame = true, PerFrameImageData = gameOverGraphics });
                GBARRR_Palette palette1 = s.DoAt(pointerTable[GBARRR_Pointer.Palette_GameOver1], () => s.SerializeObject<GBARRR_Palette>(default, name: "Palette"));
                GBARRR_Palette palette2 = s.DoAt(pointerTable[GBARRR_Pointer.Palette_GameOver2], () => s.SerializeObject<GBARRR_Palette>(default, name: "Palette"));

                // Create merged palette
                var newPalette = new RGBA5551Color[256];
                Array.Copy(palette1.Palette, newPalette, 0x100);
                Array.Copy(palette2.Palette, 0, newPalette, 0x10, 0x10);
                //PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"GameOver", $"Palette.png"), newPalette, optionalWrap: 16);

                vramConfigs[Mode7Config.GameOver] = new Mode7VRAMConfiguration() { Palette = newPalette, VRAM = vram.ToArray() };
                //await ExportAllAnimSets(Path.Combine(outputPath, $"GameOver"), vram.ToArray(), newPalette);
            }



            // Unk
            Controller.DetailedState = $"Loading VRAM configurations (Unknown)";
            await Controller.WaitIfNecessary();
            {
                byte[] menuSprites0 = null;
                s.DoAt(pointerTable[GBARRR_Pointer.Sprites_Compressed_Unk], () => {
                    //menuSprites0 = s.SerializeArray<byte>(menuSprites0, 0x20, name: nameof(menuSprites0));
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => menuSprites0 = s.SerializeArray<byte>(menuSprites0, s.CurrentLength, name: nameof(menuSprites0)));
                });

                List<Mode7VRAMEntry> vram = new List<Mode7VRAMEntry>();
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010000, ImageData = menuSprites0 });
                GBARRR_Palette palette = s.DoAt(pointerTable[GBARRR_Pointer.Palette_UnkSprites], () => s.SerializeObject<GBARRR_Palette>(default, name: "MenuPalette"));

                //PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"Unk", $"Palette.png"), palette.Palette, optionalWrap: 16);

                vramConfigs[Mode7Config.Unk] = new Mode7VRAMConfiguration() { Palette = palette.Palette, VRAM = vram.ToArray() };
                //await ExportAllAnimSets(Path.Combine(outputPath, $"Unk"), vram.ToArray(), palette.Palette);
            }


            // Main Menu
            Controller.DetailedState = $"Loading VRAM configurations (Main Menu)";
            await Controller.WaitIfNecessary();
            {
                byte[] menuSprites0 = null;
                s.DoAt(pointerTable[GBARRR_Pointer.Sprites_Compressed_MainMenu], () => {
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => menuSprites0 = s.SerializeArray<byte>(menuSprites0, s.CurrentLength, name: nameof(menuSprites0)));
                });

                List<Mode7VRAMEntry> vram = new List<Mode7VRAMEntry>();
                vram.Add(new Mode7VRAMEntry() { Address = 0x06010000, ImageData = menuSprites0 });
                GBARRR_Palette palette = s.DoAt(pointerTable[GBARRR_Pointer.Palette_MainMenuSprites], () => s.SerializeObject<GBARRR_Palette>(default, name: "MenuPalette"));

                //PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"MainMenu", $"Palette.png"), palette.Palette, optionalWrap: 16);

                vramConfigs[Mode7Config.MainMenu] = new Mode7VRAMConfiguration() { Palette = palette.Palette, VRAM = vram.ToArray() };
                //await ExportAllAnimSets(Path.Combine(outputPath, $"MainMenu"), vram.ToArray(), palette.Palette);
            }

            return new Mode7Data() {
                AnimSets = animSets,
                FrameIndices = animationFrameIndices,
                Configurations = vramConfigs
            };
        }



        public Vector2Int[] GetMode7AnimFramePositions(Mode7Data mode7Data, int animIndex, bool mirrored = false) {
            var animSet = mode7Data.AnimSets[animIndex];
            var frameIndices = mode7Data.FrameIndices[animIndex];
            List<Vector2Int> pos = new List<Vector2Int>();
            foreach (var frameInd in frameIndices)
            {
                var frame = animSet.Frames[frameInd * 2 + (mirrored ? 1 : 0)];

                pos.Add(new Vector2Int(frame?.MinXPosition ?? 0, frame?.MinYPosition ?? 0));
            }
            return pos.ToArray();
        }

        public Vector2Int Mode7_GetRaymanStartPosition(int levelIndex) {
            switch (levelIndex) {
                case 0: return new Vector2Int(0xE0, 0x3C8);
                case 1: return new Vector2Int(0xB0, 0x408);
                case 2: return new Vector2Int(0xD8, 0x430);
                default: return Vector2Int.zero;
            }
        }

        public async UniTask ExportMode7SpritesAsync(GameSettings settings, string outputPath) {
            using (var context = new Context(settings)) {
                // Load files
                await LoadFilesAsync(context);
                
                var mode7Data = await LoadMode7SpritesAsync(context);
                var animSets = mode7Data.AnimSets;
                var vramConfigs = mode7Data.Configurations;
                var animationFrameIndices = mode7Data.FrameIndices;
              

                // Export anim set frames
                Dictionary<GBARRR_Mode7AnimSet, Texture2D[]> exportedAnimSets = new Dictionary<GBARRR_Mode7AnimSet, Texture2D[]>();
                for (int a = 0; a < animSets.Length; a++) {
                    if (!exportedAnimSets.ContainsKey(animSets[a])) {
                        Mode7VRAMConfiguration vramConfig = null;
                        Mode7Config config = mode7Data.GetVRAMConfig(a);
                        vramConfig = vramConfigs[config];
                        Texture2D[] texs = Mode7_GetAnimSetFrames(animSets[a], vramConfig.Palette, vramConfig.VRAM, isExport: true, loadMirroredFrames: true);
                        exportedAnimSets[animSets[a]] = texs;
                        if (texs == null) continue;
                        for (int i = 0; i < texs.Length; i++) {
                            if (texs[i] == null) continue;
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"AnimSet_{a}_{config}", $"{i}.png"), texs[i].EncodeToPNG());
                        }
                        await Controller.WaitIfNecessary();
                    }
                }

                // Export the animations themselves
                for (int a = 0; a < animationFrameIndices.Length; a++) {
                    if (!exportedAnimSets.ContainsKey(animSets[a])) continue;
                    var texs = exportedAnimSets[animSets[a]];
                    if(texs == null) continue;

                    using (MagickImageCollection collection = new MagickImageCollection()) {
                        int index = 0;

                        foreach (var frame in animationFrameIndices[a]) {
                            var tex = texs[frame * 2]; // * 2 only if loadMirroredFrames
                            if(tex == null) continue;
                            var img = tex.ToMagickImage();
                            collection.Add(img);
                            collection[index].AnimationDelay = 1;
                            collection[index].AnimationTicksPerSecond = 10;
                            collection[index].Trim();

                            collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                            index++;
                        }

                        // Save gif
                        collection.Write(Path.Combine(outputPath, $"Anim_{a}.gif"));
                    }
                }
            }

        }

        public Texture2D[] Mode7_GetAnimSetFrames(GBARRR_Mode7AnimSet animSet, RGBA5551Color[] palette, IEnumerable<Mode7VRAMEntry> vram, bool isExport = false, bool loadMirroredFrames = false) {
            int minX1 = 0, minY1 = 0, maxX2 = int.MinValue, maxY2 = int.MinValue;
            if (isExport) {
                if (animSet.Frames.Length > 0) {
                    var fs = animSet.Frames.Where(f => f != null && f.Channels.Length > 0);
                    minX1 = fs.Min(f => f.MinXPosition);
                    minY1 = fs.Min(f => f.MinYPosition);
                    maxX2 = fs.Max(f => f.MaxXPosition);
                    maxY2 = fs.Max(f => f.MaxYPosition);
                } else {
                    maxX2 = 0;
                    maxY2 = 0;
                }
            }
            const int tileWidth = 8;
            int tileSize = (tileWidth * tileWidth) / 2;
            const uint baseAddr = 0x06010000;
            var pal = Util.ConvertAndSplitGBAPalette(palette);
            var numPalettes = pal.Length;

            Texture2D[] texs = new Texture2D[animSet.Frames.Length / (loadMirroredFrames ? 1 : 2)];
            for (int i = 0; i < texs.Length; i++) {
                var frame = animSet.Frames[i * (loadMirroredFrames ? 1 : 2)];
                if(frame == null) continue;
                int frameMinX = frame.MinXPosition;
                int frameMinY = frame.MinYPosition;
                int w, h, frameOffsetX, frameOffsetY;
                if (isExport) {
                    frameOffsetX = frameMinX - minX1;
                    frameOffsetY = frameMinY - minY1;
                    w = (maxX2 - minX1);
                    h = (maxY2 - minY1);
                } else {
                    frameOffsetX = 0;
                    frameOffsetY = 0;
                    w = frame.MaxXPosition - frame.MinXPosition;
                    h = frame.MaxYPosition - frame.MinYPosition;
                }
                Texture2D tex = TextureHelpers.CreateTexture2D(w, h, clear: true);

                void addObjToFrame(GBARRR_Mode7AnimationChannel channel) {
                    if(channel.IsEndAttribute) return;
                    int width = channel.XSize;
                    int height = channel.YSize;

                    //var tileIndex = relativeTile;
                    var imageIndex = channel.ImageIndex;
                    var imageAddress = baseAddr + imageIndex * tileSize;

                    // Clamp palette index
                    var palIndex = Mathf.Clamp(channel.PaletteIndex, 0, pal.Length - 1);

                    for (int y = 0; y < height; y++) {
                        for (int x = 0; x < width; x++) {
                            byte[] tileSet = null;
                            int tileOffset = 0;

                            // Find array to use
                            foreach (var vramEntry in vram) {
                                if (imageAddress >= vramEntry.Address) {
                                    var relativeAddr = imageAddress - vramEntry.Address;
                                    if (vramEntry.IsPerFrame) {
                                        int frameIndex = i / (loadMirroredFrames ? 2 : 1);
                                        if(frameIndex >= vramEntry.PerFrameImageData.Length) continue;
                                        if (relativeAddr >= vramEntry.PerFrameImageData[frameIndex].Length) continue;
                                        tileSet = vramEntry.PerFrameImageData[frameIndex];
                                    } else {
                                        if (relativeAddr >= vramEntry.ImageData.Length) continue;
                                        tileSet = vramEntry.ImageData;
                                    }
                                    tileOffset = (int)relativeAddr;
                                }
                            }

                            // Fill in tile
                            if (tileSet != null) {
                                int actualX = ((channel.IsFlippedHorizontally ? (channel.XSize - 1 - x) : x) * CellSize) + channel.XPosition - frameMinX + frameOffsetX;
                                int actualY = ((channel.IsFlippedVertically ? (channel.XSize - 1 - y) : y) * CellSize) + channel.YPosition - frameMinY + frameOffsetY;

                                tex.FillInTile(tileSet, tileOffset, pal[palIndex], Util.TileEncoding.Linear_4bpp, CellSize, true, actualX, actualY,
                                    flipTileX: channel.IsFlippedHorizontally,
                                    flipTileY: channel.IsFlippedVertically,
                                    ignoreTransparent: true);
                            }

                            imageAddress += tileSize;
                        }
                    }
                }

                foreach (var channel in frame.Channels)
                    addObjToFrame(channel);

                tex.Apply();

                texs[i] = tex;
            }
            return texs;
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);
            var gameMode = GetCurrentGameMode(context.Settings);

            var lvl = context.Settings.Level;
            var world = context.Settings.World;

            if (gameMode == GameMode.Village)
            {
                lvl = 28;
                world = 5;
            }

            // Shooting Range 2 should be set to the values of Shooting Range 1
            if (lvl == 31)
            {
                lvl = 29;
                world = 0;
            }

            Debug.Log($"RRR level: {world}-{lvl} ({gameMode})");

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(rom.Localization);

            if (gameMode == GameMode.Mode7)
            {
                var map = new Unity_Map {
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                    Width = 256,
                    Height = 256,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(rom.Mode7_MapTiles, false, rom.Mode7_TilemapPalette), 
                    },
                    MapTiles = rom.Mode7_MapData.Select((x, i) =>
                    {
                        x.CollisionType = rom.Mode7_CollisionTypes[rom.Mode7_CollisionMapData[i]];
                        return new Unity_Tile(x);
                    }).ToArray(),
                };
                var bg0 = new Unity_Map {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 32,
                    Height = 12, // Height is actually 32, but the remaining tiles are always transparent, but with a solid color
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(rom.Mode7_BG0Tiles, false, rom.Mode7_TilemapPalette), 
                    },
                    MapTiles = rom.Mode7_BG0MapData.Take(12 * 32).Select((x, i) => new Unity_Tile(x)).ToArray(),
                };
                var bg1 = new Unity_Map {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 32,
                    Height = 32,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(rom.Mode7_BG1Tiles, false, rom.Mode7_TilemapPalette), 
                    },
                    MapTiles = rom.Mode7_BG1MapData.Select((x, i) => new Unity_Tile(x)).ToArray(),
                };

                var objmanager = new Unity_ObjectManager_GBARRRMode7(context, await LoadGraphicsDataAsync(context));

                var objLength = rom.Mode7_Objects.FindItemIndex(x => x.ObjectType == GBARRR_Mode7Object.Mode7Type.Invalid);
                var mode7Waypoints = rom.Mode7_Waypoints.Select(x => (Unity_Object)new Unity_Object_GBARRRMode7Waypoint(x, objmanager));
                var rayPos = Mode7_GetRaymanStartPosition(lvl);
                var mode7Objects = rom.
                    // Get the objects
                    Mode7_Objects.
                    // Only take the valid ones
                    Take(objLength).
                    // Convert to Unity objects
                    Select(x => (Unity_Object)new Unity_Object_GBARRRMode7(x, objmanager, x.ObjectType == GBARRR_Mode7Object.Mode7Type.Unknown || (int)x.ObjectType > 45)).
                    // Add waypoints
                    Concat(mode7Waypoints).
                    // To list
                    ToList();

                return new Unity_Level(
                    maps: new Unity_Map[]
                    {
                        map, // Put the map first so the backgrounds are visible
                        bg0,
                        bg1,
                    },
                    objManager: objmanager,
                    getCollisionTypeNameFunc: x => ((GBARRR_Mode7TileCollisionType)x).ToString(),
                    getCollisionTypeGraphicFunc: x => ((GBARRR_Mode7TileCollisionType)x).GetCollisionTypeGraphic(),
                    cellSize: CellSize,
                    localization: loc,
                    eventData: mode7Objects,
                    defaultMap: 0,
                    rayman: new Unity_Object_GBARRRMode7(new GBARRR_Mode7Object
                    {
                        ObjectType = GBARRR_Mode7Object.Mode7Type.Unknown,
                        XPosition = (short)rayPos.x,
                        YPosition = (short)rayPos.y
                    }, objmanager, false)
                );
            }

            if (gameMode == GameMode.Menu)
            {
                var mapLevels = GetMenuLevels(context.Settings.Level);
                var maps = new Unity_Map[mapLevels.Length];

                for (int i = 0; i < mapLevels.Length; i++)
                {
                    var menulevel = mapLevels[i];
                    var palIndex = GetMenuPalIndex(menulevel);
                    var size = GetMenuSize(menulevel);

                    maps[i] = new Unity_Map {
                        Type = Unity_Map.MapType.Graphics,
                        Width = size.Width,
                        Height = size.Height,
                        TileSet = new Unity_MapTileMap[]
                        {
                            LoadTileSet(rom.Menu_Tiles[i], palIndex != null, rom.Menu_Palette[i], palIndex ?? 0),
                        },
                        MapTiles = rom.Menu_MapData[i].Select(x => new Unity_Tile(x)).ToArray(),
                    };

                    if (HasMenuAlphaBlending(menulevel))
                    {
                        maps[i].Alpha = 0.5f;
                        maps[i].IsAdditive = true;
                    }
                }

                return new Unity_Level(
                    maps: maps,
                    objManager: new Unity_ObjectManager_GBARRR(context, new Unity_ObjectManager_GBARRR.GraphicsData[0][]),
                    getCollisionTypeNameFunc: x => ((GBARRR_TileCollisionType)x).ToString(),
                    getCollisionTypeGraphicFunc: x => ((GBARRR_TileCollisionType)x).GetCollisionTypeGraphic(),
                    cellSize: CellSize,
                    localization: loc,
                    defaultMap: 0
                );
            }

            if (gameMode == GameMode.Mode7Unused)
            {
                // Create a collision map and copy over to normal map
                var cmap = LoadMap(rom.CollisionMap.MapWidth, rom.CollisionMap.MapHeight, null, rom.CollisionMap, null, 0, false, 0);

                // Create the map
                var map = new Unity_Map {
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                    Width = 256,
                    Height = 256,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(rom.LevelTileset.Data, false, rom.LevelTileset.Palette),
                    },
                    MapTiles = rom.Mode7_MapData.Select((x, i) =>
                    {
                        x.CollisionType = cmap.MapTiles[i].Data.CollisionType;
                        return new Unity_Tile(x);
                    }).ToArray(),
                };

                // Map data appears to be missing for these
                var bg0 = new Unity_Map {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 32,
                    Height = 8,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(rom.BG0TileSet.Data, false, rom.BG0TileSet.Palette),
                    },
                    MapTiles = Enumerable.Range(1, 32 * 8).Select(x => new Unity_Tile(new MapTile()
                    {
                        TileMapY = (ushort)x
                    })).ToArray(),
                };
                var bg1 = new Unity_Map {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 32,
                    Height = 7,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(rom.BG1TileSet.Data, false, rom.BG1TileSet.Palette),
                    },
                    MapTiles = Enumerable.Range(1, 32 * 7).Select(x => new Unity_Tile(new MapTile()
                    {
                        TileMapY = (ushort)x
                    })).ToArray(),
                };

                var o = new Unity_ObjectManager_GBARRR(context, new Unity_ObjectManager_GBARRR.GraphicsData[0][]);

                return new Unity_Level(
                    maps: new Unity_Map[]
                    {
                        map, // Put the map first so the backgrounds are visible
                        bg0,
                        bg1,
                    },
                    objManager: o,
                    eventData: rom.ObjectArray.Objects.Select(x => (Unity_Object)new Unity_Object_GBARRR(x, o)).ToList(),
                    getCollisionTypeNameFunc: x => ((GBARRR_TileCollisionType)x).ToString(),
                    getCollisionTypeGraphicFunc: x => ((GBARRR_TileCollisionType)x).GetCollisionTypeGraphic(),
                    cellSize: CellSize,
                    localization: loc,
                    defaultMap: 0
                );
            }

            Controller.DetailedState = $"Loading tile set";
            await Controller.WaitIfNecessary();

            var bg0Tileset = LoadTileSet(rom.BG0TileSet.Data, true, rom.TilePalette ?? rom.BG0TileSet.Palette, GetBG0Palette(lvl), 1,
                GetBG0AnimTileInfo(lvl, rom.AnimatedPalettes));
            var bg1Tileset = LoadTileSet(rom.BG1TileSet.Data, true, rom.TilePalette ?? rom.BG1TileSet.Palette, GetBG1Palette(lvl), 1);
            var levelTileset = LoadTileSet(rom.LevelTileset.Data, false, rom.TilePalette ?? rom.LevelTileset.Palette);
            var fgTileset = LoadTileSet(rom.FGTileSet.Data, true, rom.TilePalette ?? rom.FGTileSet.Palette, 0, 16,
                GetFGAnimTileInfo(lvl, rom.AnimatedPalettes));

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            var bg0Map = new Unity_Map() {
                Type = Unity_Map.MapType.Graphics,
                Width = 32,
                Height = 32,
                TileSet = new Unity_MapTileMap[] { bg0Tileset },
                MapTiles = rom.BG0Map.MapTiles.Select(x =>
                {
                    // Modify the index for animated tiles
                    x.TileMapY = (ushort)(bg0Tileset.GBARRR_PalOffsets[0] + x.TileMapY);
                    return new Unity_Tile(x);
                }).ToArray()
            };
            var bg1Map = new Unity_Map() {
                Type = Unity_Map.MapType.Graphics,
                Width = 32,
                Height = 32,
                TileSet = new Unity_MapTileMap[] { bg1Tileset },
                MapTiles = rom.BG1Map.MapTiles.Select(x => new Unity_Tile(x)).ToArray()
            };

            var hasFGMap = !(gameMode == GameMode.Village && context.Settings.Level == 2); // Disable rain

            var properties = rom.LevelProperties[lvl];

            var levelMap = LoadMap(rom.LevelMap.MapWidth, rom.LevelMap.MapHeight, rom.LevelMap, rom.CollisionMap, levelTileset, 0, false, 0);
            var fgMap = hasFGMap ? LoadMap(rom.FGMap.MapWidth, rom.FGMap.MapHeight, rom.FGMap, null, fgTileset, properties.AlphaBlendingValue, IsForeground(world, lvl), GetFGPalette(lvl)) : null;
            
            await Controller.WaitIfNecessary();

            var objManager = new Unity_ObjectManager_GBARRR(context, await LoadGraphicsDataAsync(context, rom));

            await Controller.WaitIfNecessary();

            var objects = rom.ObjectArray.Objects.Select(x => (Unity_Object)new Unity_Object_GBARRR(x, objManager));

            if (gameMode == GameMode.Village && context.Settings.Level == 2)
                objects = objects.Where(x => ((Unity_Object_GBARRR)x).Object.ObjectType != GBARRR_ObjectType.Scenery2); // Disable rain

            var objList = objects.ToList();

            // Assign correct animations to objects
            var dict = GetActorGraphicsData(lvl, world);
            foreach (var obj in objList.Cast<Unity_Object_GBARRR>())
            {
                AssignObjectValues(obj.Object, rom, lvl, world);

                if (!dict.ContainsKey(obj.Object.P_GraphicsOffset))
                {
                    if (obj.Object.P_GraphicsOffset != 0) {
                        Debug.LogWarning($"Graphics with offset {obj.Object.P_GraphicsOffset:X8} wasn't loaded!");
                    }
                    obj.AnimationGroupIndex = -1;
                    continue;
                }

                var animBlock = dict[obj.Object.P_GraphicsOffset];
                var animGroup = objManager.GraphicsDatas.FindItemIndex(x => x.Any(y => y.BlockIndex == animBlock));
                var animIndex = objManager.GraphicsDatas[animGroup].FindItemIndex(x => x.BlockIndex == animBlock);

                obj.AnimationGroupIndex = animGroup;
                obj.AnimIndex = animIndex;
            }

            return new Unity_Level(
                maps: hasFGMap ? new Unity_Map[]
                {
                    bg0Map,
                    bg1Map,
                    levelMap,
                    fgMap
                } : new Unity_Map[]
                {
                    bg0Map,
                    bg1Map,
                    levelMap,
                }, 
                objManager: objManager,
                eventData: objList,
                getCollisionTypeNameFunc: x => ((GBARRR_TileCollisionType)x).ToString(),
                getCollisionTypeGraphicFunc: x => ((GBARRR_TileCollisionType)x).GetCollisionTypeGraphic(),
                cellSize: CellSize,
                localization: loc,
                defaultCollisionMap: 2,
                defaultMap: 2
            );
        }


        protected async UniTask<Unity_ObjectManager_GBARRR.GraphicsData[][]> LoadGraphicsDataAsync(Context context, GBARRR_ROM rom)
        {
            SerializerObject s = context.Deserializer;
            var animTable = GBARRR_Tables.GetAnimBlocks;
            var graphicsData = new Unity_ObjectManager_GBARRR.GraphicsData[animTable.Length][];

            for (int animGroup = 0; animGroup < animTable.Length; animGroup++)
            {
                Controller.DetailedState = $"Loading animation group {animGroup + 1}/{animTable.Length}";
                await Controller.WaitIfNecessary();

                graphicsData[animGroup] = new Unity_ObjectManager_GBARRR.GraphicsData[animTable[animGroup].Length];

                for (int animIndex = 0; animIndex < animTable[animGroup].Length; animIndex++)
                {
                    int index = animTable[animGroup][animIndex].AnimBlockIndex;

                    GBARRR_GraphicsBlock graphicsBlock = null;
                    rom.OffsetTable.DoAtBlock(s.Context, index, size => {
                        graphicsBlock = s.SerializeObject<GBARRR_GraphicsBlock>(null, pb => pb.BlockIndex = index, name: "Graphics");
                        if (FixedSpriteSizes.ContainsKey(index)) {
                            graphicsBlock.AnimationAssemble = FixedSpriteSizes[index];
                        }
                    });
                    if (graphicsBlock != null) {
                        int paletteIndex = animTable[animGroup][animIndex].PalBlockIndex;
                        GBARRR_Palette palette = null;
                        rom.OffsetTable.DoAtBlock(context, paletteIndex, size => {
                            palette = s.SerializeObject<GBARRR_Palette>(palette, name: nameof(palette));
                        });
                        Vector2? pivot = null;
                        if (animGroup == 1) // Rayman has a different pivot
                            pivot = new Vector2(0.5f, 0f);

                        // Copy variables to prevent them from being modified before used in the func
                        var f_animGroup = animGroup;
                        var f_animIndex = animIndex;

                        graphicsData[animGroup][animIndex] = new Unity_ObjectManager_GBARRR.GraphicsData(
                            animFrameFunc: () => GetSpriteFrames(graphicsBlock, palette.Palette, animTable[f_animGroup][f_animIndex].SubPalette)
                                .Select(x => x.CreateSprite(pivot: pivot)).ToArray(), 
                            animSpeed: animTable[animGroup][animIndex].AnimSpeed,
                            blockIndex: index);

                    }
                }
            }

            return graphicsData;
        }
        protected async UniTask<Unity_ObjectManager_GBARRRMode7.GraphicsData[]> LoadGraphicsDataAsync(Context context)
        {
            var mode7Data = await LoadMode7SpritesAsync(context);
            var animSets = mode7Data.AnimSets;
            var vramConfigs = mode7Data.Configurations;
            var animationFrameIndices = mode7Data.FrameIndices;

            var graphicsData = new Unity_ObjectManager_GBARRRMode7.GraphicsData[animationFrameIndices.Length];

            // Get textures for each animation set
            Dictionary<GBARRR_Mode7AnimSet, Texture2D[]> animSetTextures = new Dictionary<GBARRR_Mode7AnimSet, Texture2D[]>();
            for (int a = 0; a < animSets.Length; a++)
            {
                if (!animSetTextures.ContainsKey(animSets[a]))
                {
                    Mode7Config config = mode7Data.GetVRAMConfig(a);
                    Mode7VRAMConfiguration vramConfig = vramConfigs[config];
                    Texture2D[] texs = Mode7_GetAnimSetFrames(animSets[a], vramConfig.Palette, vramConfig.VRAM);
                    animSetTextures[animSets[a]] = texs;
                    await Controller.WaitIfNecessary();
                }
            }

            // Add the animations
            for (int a = 0; a < animationFrameIndices.Length; a++)
            {
                if (!animSetTextures.ContainsKey(animSets[a]))
                    continue;

                var allTexs = animSetTextures[animSets[a]];
                var texs = animationFrameIndices[a].Select(x => allTexs?[x]);

                graphicsData[a] = new Unity_ObjectManager_GBARRRMode7.GraphicsData(texs.Select(x => x?.CreateSprite()).ToArray(), 6, GetMode7AnimFramePositions(mode7Data, a));
            }

            return graphicsData;
        }

        protected IEnumerable<Texture2D> GetSpriteFrames(GBARRR_GraphicsBlock spr, BaseColor[] palette, int paletteIndex)
        {
            // For each frame
            Color[] paletteColors = new Color[16];
            for (int i = 0; i < paletteColors.Length; i++) {
                Color c = palette[paletteIndex * 16 + i].GetColor();

                c = i != 0 ? new Color(c.r, c.g, c.b, 1f) : new Color(c.r, c.g, c.b, 0f);
                paletteColors[i] = c;
            }
            int width = (int)spr.TileSize;
            int height = (int)spr.TileSize;
            int frameCount = (int)spr.Count;

            int blockWidth = 1;
            int blockHeight = 1;
            AnimationAssemble.AssembleOrder order = AnimationAssemble.AssembleOrder.Row;
            if (spr.AnimationAssemble != null) {
                blockWidth = spr.AnimationAssemble.Width;
                blockHeight = spr.AnimationAssemble.Height;
                order = spr.AnimationAssemble.Order;
                frameCount = frameCount / (blockWidth * blockHeight);
            }

            for (int frame = 0; frame < frameCount; frame++)
            {
                var tex = TextureHelpers.CreateTexture2D(width * blockWidth, height * blockHeight, false);
                for (int bx = 0; bx < blockWidth; bx++) {
                    for (int by = 0; by < blockHeight; by++) {
                        int i = frame * blockWidth * blockHeight;
                        if (order == AnimationAssemble.AssembleOrder.Column) {
                            i += blockHeight * bx + by;
                        } else {
                            i += blockWidth * by + bx;
                        }
                        for (int y = 0; y < height; y++) {
                            int tileY = y / 8;
                            int inTileY = y % 8;
                            int tileIndexY = tileY * width / 8;
                            int pixelY = (blockHeight * height) - (by * height) - y - 1;
                            for (int x = 0; x < width; x++) {
                                int tileX = x / 8;
                                int inTileX = x % 8;
                                int tileIndex = tileX + tileIndexY;
                                int index = tileIndex * (8 * 4) + (inTileY * 8 + inTileX) / 2;

                                if (index < spr.TileData[i].Length) {
                                    var v = BitHelpers.ExtractBits(spr.TileData[i][index], 4, x % 2 == 0 ? 0 : 4);

                                    tex.SetPixel(x + bx * width, pixelY, paletteColors[v]);
                                } else {
                                    Debug.Log($"{spr.Offset.AbsoluteOffset:X8} - " + spr.TileData[i].Length + " - " + width + " - " + index);
                                }
                            }
                        }
                    }
                }
                tex.Apply();

                yield return tex;
            }
        }

        protected void ExportSpriteFrames(GBARRR_GraphicsBlock spr, BaseColor[] palette, int paletteIndex, string outputDir, bool includeAbsolutePointer, int speed) {
            try 
            {
                var index = 0;

                // For each frame
                foreach (var tex in GetSpriteFrames(spr, palette, paletteIndex))
                {
                    var append = includeAbsolutePointer ? $"_{spr.Offset.AbsoluteOffset:X8}" : String.Empty;
                    var fileName = $"Frames{append}-{speed}/{index}.png";
                    Util.ByteArrayToFile(Path.Combine(outputDir, fileName), tex.EncodeToPNG());
                    index++;
                }
            } catch (Exception ex) {
                Debug.LogError($"Error for GraphicsBlock {spr.Offset.AbsoluteOffset:X8} - Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
            }
        }

        public Unity_Map LoadMap(uint width, uint height, GBARRR_MapBlock mapBlock, GBARRR_MapBlock collisionBlock, Unity_MapTileMap tileset, byte alphaBlending, bool foreground, int palIndex)
        {
            var map = new Unity_Map {
                Type = Unity_Map.MapType.Graphics,
                Width = (ushort)(width * 4), // The game uses 32x32 tiles, made out of 8x8 tiles
                Height = (ushort)(height * 4),
                TileSet = new Unity_MapTileMap[]
                {
                    tileset
                },
                MapTiles = new Unity_Tile[width * 4 * height * 4],
                Layer = foreground ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle
            };
            if (collisionBlock != null) {
                map.Type |= Unity_Map.MapType.Collision;
            }

            if (alphaBlending != 0) {
                map.IsAdditive = true;
                map.Alpha = alphaBlending / 16f;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var actualX = x * 4;
                    var actualY = y * 4;

                    var index_32 = y * width + x;
                    var tile_32 = mapBlock?.Indices_32[index_32];
                    var col_32 = collisionBlock?.Indices_32[index_32];

                    var tiles_16 = tile_32 != null ? mapBlock.Indices_16[tile_32.Value] : null;
                    var col_16 = col_32 != null ? collisionBlock.Indices_16[col_32.Value] : null;

                    setTiles16(0, 0, 0);
                    setTiles16(2, 0, 1);
                    setTiles16(0, 2, 2);
                    setTiles16(2, 2, 3);
                    
                    void setTiles16(int offX, int offY, int index)
                    {
                        var i = tiles_16?.TileIndices[index];
                        var tiles_8 = i != null ? mapBlock?.Tiles_8[i.Value].Tiles : null;
                        var col_8 = col_16 != null ? collisionBlock?.Tiles_8[col_16.TileIndices[index]].Tiles : null;

                        setTileAt(actualX + offX + 0, actualY + offY + 0, tiles_8?[0], col_8?[0].CollisionType, $"{i}-0");
                        setTileAt(actualX + offX + 1, actualY + offY + 0, tiles_8?[1], col_8?[1].CollisionType, $"{i}-1");
                        setTileAt(actualX + offX + 0, actualY + offY + 1, tiles_8?[2], col_8?[2].CollisionType, $"{i}-2");
                        setTileAt(actualX + offX + 1, actualY + offY + 1, tiles_8?[3], col_8?[3].CollisionType, $"{i}-3");
                    }

                    void setTileAt(int tileX, int tileY, MapTile tile, byte? collisionType, string debugString)
                    {
                        var tileMapY = (mapBlock?.Type == GBARRR_MapBlock.MapType.Foreground && tile?.TileMapY > 1) 
                            ? (ushort)(tile?.TileMapY - 2 ?? 0) 
                            : tile?.TileMapY ?? 0;

                        map.MapTiles[tileY * map.Width + tileX] = new Unity_Tile(new MapTile()
                        {
                            TileMapY = (mapBlock?.Type == GBARRR_MapBlock.MapType.Foreground && tile?.TileMapY > 1) 
                                ? (ushort)(tileMapY + tileset.GBARRR_PalOffsets[(palIndex + tile?.PaletteIndex) % 16 ?? 0]) 
                                : tileMapY,
                            CollisionType = collisionType ?? 0,
                            HorizontalFlip = tile?.HorizontalFlip ?? false,
                            VerticalFlip = tile?.VerticalFlip ?? false
                        })
                        {
                            DebugText = debugString
                        };
                    }
                }
            }

            return map;
        }

        public Unity_MapTileMap LoadTileSet(byte[] tilemap, bool is4bit, BaseColor[] palette, int palStart = 0, int palCount = 1, AnimTileInfo[] animtedTileInfos = null)
        {
            animtedTileInfos = animtedTileInfos ?? new AnimTileInfo[0];
            int block_size = is4bit ? 0x20 : 0x40;
            const float texWidth = 256f;
            const float tilesWidth = texWidth / CellSize;
            var tileCount = tilemap.Length / block_size;
            var texHeight = Mathf.CeilToInt(tileCount / tilesWidth) * CellSize;
            List<Unity_AnimatedTile> unityAnimTiles = new List<Unity_AnimatedTile>();
            var totalPalCount = palCount + animtedTileInfos.Sum(x => x.PalCount);
            //UnityEngine.Debug.Log(tileCount + " - " + block_size);
            var palOffsets = new int[palCount];

            // Get the tile-set texture
            var tex = TextureHelpers.CreateTexture2D((int)texWidth, texHeight * totalPalCount);
            var palBlockSize = (tex.width * tex.height / (CellSize * CellSize)) / totalPalCount;

            var currentBlockIndex = 0;

            // Add tiles for all normal palettes
            for (int i = 0; i < palCount; i++)
            {
                fillTiles(palette, (i + palStart) * 16, i);
                palOffsets[i] = i * palBlockSize;
                currentBlockIndex++;
            }

            // Add animated tile data
            foreach (var animTileInfo in animtedTileInfos)
            {
                // Fill in tiles for all animated tile versions (skipping the first one as we already have that from the normal palette)
                for (int i = 0; i < animTileInfo.PalCount; i++)
                    fillTiles(animTileInfo.AnimatedPalette, i * 16, currentBlockIndex + i);

                for (int i = 0; i < tileCount; i++)
                {
                    unityAnimTiles.Add(new Unity_AnimatedTile()
                    {
                        AnimationSpeed = animTileInfo.AnimSpeed,
                        TileIndices = Enumerable.Range(0, animTileInfo.PalCount).Select(x => (currentBlockIndex + x) * palBlockSize + i).ToArray()
                    });
                }

                palOffsets[animTileInfo.TilePalIndex] = currentBlockIndex * palBlockSize;

                currentBlockIndex += animTileInfo.PalCount;
            }

            void fillTiles(BaseColor[] pal, int palOffset, int blockIndex)
            {
                var tileIndex = 0;

                for (int y = 0; y < tex.height; y += CellSize)
                {
                    for (int x = 0; x < tex.width; x += CellSize)
                    {
                        if (tileIndex >= tileCount)
                            break;

                        var offset = tileIndex * block_size;

                        for (int yy = 0; yy < CellSize; yy++)
                        {
                            for (int xx = 0; xx < CellSize; xx++)
                            {
                                Color c;

                                if (is4bit)
                                {
                                    var off = offset + ((yy * CellSize) + xx) / 2;
                                    var relOff = ((yy * CellSize) + xx);
                                    var b = tilemap[off];
                                    b = (byte)BitHelpers.ExtractBits(b, 4, relOff % 2 == 0 ? 0 : 4);
                                    c = pal[palOffset + b].GetColor();
                                    c = new Color(c.r, c.g, c.b, b != 0 ? 1f : 0f);
                                }
                                else
                                {
                                    var b = tilemap[offset + (yy * CellSize) + xx];
                                    c = pal[b].GetColor();
                                    c = new Color(c.r, c.g, c.b, b != 0 ? 1f : 0f);
                                }

                                tex.SetPixel(x + xx, blockIndex * texHeight + y + yy, c);
                            }
                        }

                        tileIndex++;
                    }
                }
            }

            tex.Apply();

            return new Unity_MapTileMap(tex, CellSize)
            {
                AnimatedTiles = unityAnimTiles?.ToArray(),
                GBARRR_PalOffsets = palOffsets
            };
        }

        public IReadOnlyDictionary<string, string[]> LoadLocalization(GBARRR_LocalizationBlock loc)
        {
            var dictionary = new Dictionary<string, string[]>();

            var languages = new string[]
            {
                "English",
                "French",
                "German",
                "Italian",
                "Dutch",
                "Spanish"
            };

            for (int i = 0; i < loc.Strings.Length; i++)
                dictionary.Add(languages[i], loc.Strings[i].Where(x => !String.IsNullOrWhiteSpace(x)).ToArray());

            return dictionary;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);

		#region Hardcoded methods
		public Dictionary<uint, uint> GetActorGraphicsData(int level, int world) {

            //Dictionary<uint, uint> indexDict = new Dictionary<uint, uint>();
            Dictionary<uint, uint> memAddressDict = new Dictionary<uint, uint>();
            uint LoadGraphicsBlock(uint index, uint count, uint tileSize) {
                /*GBARRR_GraphicsBlock obj = null;
                if(indexDict.ContainsKey(index)) return indexDict[index];
                rom.OffsetTable.DoAtBlock(s.Context, index, size => {
                    obj = s.SerializeObject<GBARRR_GraphicsBlock>(null, onPreSerialize: gb => {
                        gb.BlockIndex = (int)index;
                        gb.Count = count;
                        gb.TileSize = tileSize;
                    }, name: "Graphics");
                    if (FixedSpriteSizes.ContainsKey((int)index)) {
                        obj.AnimationAssemble = FixedSpriteSizes[(int)index];
                    }
                });*/
                //indexDict[index] = index;
                memAddressDict[index] = index;
                return index;
            }
            uint LoadGraphicsBlockUnknown(uint index) {
                //indexDict[index] = index;
                memAddressDict[index] = index;
                return index;
            }

            memAddressDict[0x03003F6C] = LoadGraphicsBlock(0x12, 3, 0x10);
            memAddressDict[0x03004310] = LoadGraphicsBlock(0x16, 9, 0x10);
            memAddressDict[0x0300432C] = LoadGraphicsBlock(0x19, 4, 0x10);
            memAddressDict[0x03002928] = LoadGraphicsBlock(0x1a, 0xb, 0x10);
            memAddressDict[0x030024BC] = LoadGraphicsBlock(0x1c, 4, 0x10);
            memAddressDict[0x030028A4] = LoadGraphicsBlock(0x1d, 0xb, 0x10);
            memAddressDict[0x030024F8] = LoadGraphicsBlock(0x1e, 8, 0x20);
            memAddressDict[0x030028A0] = LoadGraphicsBlock(7, 0x70, 0x10);
            memAddressDict[0x03004F88] = LoadGraphicsBlock(0xf, 8, 0x20);
            memAddressDict[0x030025E8] = LoadGraphicsBlock(0x2c, 0xc, 0x10);
            memAddressDict[0x03002EC4] = LoadGraphicsBlock(0x2e, 0xd, 0x20);
            memAddressDict[0x03002954] = LoadGraphicsBlock(0x1f, 1, 0x20);
            memAddressDict[0x030051D4] = LoadGraphicsBlock(0x22, 1, 0x20);
            memAddressDict[0x03005024] = LoadGraphicsBlock(0x24, 1, 0x20);
            memAddressDict[0x03002318] = LoadGraphicsBlock(0x26, 1, 0x20);
            memAddressDict[0x03005194] = LoadGraphicsBlock(0x21, 1, 0x20);
            memAddressDict[0x030024B4] = LoadGraphicsBlock(0x20, 1, 0x20);
            memAddressDict[0x03002890] = LoadGraphicsBlock(0x27, 1, 0x20);
            memAddressDict[0x03004F80] = LoadGraphicsBlock(0x29, 1, 0x20);
            memAddressDict[0x030025D4] = LoadGraphicsBlock(0x28, 1, 0x20);
            memAddressDict[0x03005160] = LoadGraphicsBlock(0x2b, 1, 0x20);
            memAddressDict[0x0300235C] = LoadGraphicsBlock(0x30, 4, 0x10);
            memAddressDict[0x03002918] = LoadGraphicsBlock(0x32, 2, 0x10);
            memAddressDict[0x030023F8] = LoadGraphicsBlock(0x39, 8, 0x20);
            memAddressDict[0x03005080] = LoadGraphicsBlock(0x00000305, 0xc, 0x40);
            memAddressDict[0x03002EF4] = LoadGraphicsBlock(0x00000311, 0xf, 0x20);
            memAddressDict[0x030050E0] = LoadGraphicsBlock(0x00000315, 0xf, 0x20);
            memAddressDict[0x03002E34] = LoadGraphicsBlock(0x314, 0xf, 0x20);
            memAddressDict[0x03002560] = LoadGraphicsBlock(0x00000313, 0xf, 0x20);
            memAddressDict[0x030051C8] = LoadGraphicsBlock(0x00000312, 0xf, 0x20);
            memAddressDict[0x030050B4] = LoadGraphicsBlock(0x00000317, 0xe, 0x40);
            memAddressDict[0x030028D8] = LoadGraphicsBlock(0x318, 0xe, 0x40);
            memAddressDict[0x03004334] = LoadGraphicsBlock(0x00000319, 0xe, 0x40);
            memAddressDict[0x03005170] = LoadGraphicsBlock(0x0000031A, 0xe, 0x40);
            memAddressDict[0x0300308C] = LoadGraphicsBlock(0x0000031B, 0xe, 0x40);
            memAddressDict[0x030028E0] = LoadGraphicsBlock(0x31c, 0xe, 0x40);
            memAddressDict[0x03005234] = LoadGraphicsBlock(0x3b, 9, 0x10);
            memAddressDict[0x03004FA0] = LoadGraphicsBlock(0x3d, 8, 0x20);
            memAddressDict[0x03003F6C] = LoadGraphicsBlock(0x12, 3, 0x10);

            if (level < 0x1d) {
                memAddressDict[0x03005158] = LoadGraphicsBlock(0x0000013F, 4, 0x20);
                memAddressDict[0x03002E2C] = LoadGraphicsBlock(0x324, 3, 0x20);
                memAddressDict[0x03004314] = LoadGraphicsBlock(0x00000322, 3, 0x20);
                memAddressDict[0x0300254C] = LoadGraphicsBlock(0x0000031E, 3, 0x20);
                memAddressDict[0x03002910] = LoadGraphicsBlock(800, 4, 0x20);
                memAddressDict[0x03004F90] = LoadGraphicsBlock(0xe5, 1, 0x10);
                memAddressDict[0x03005208] = LoadGraphicsBlock(0xe6, 4, 0x20);
                memAddressDict[0x03005068] = LoadGraphicsBlock(0x00000327, 4, 0x20);
                memAddressDict[0x03005078] = LoadGraphicsBlock(0x00000325, 4, 0x20);
                memAddressDict[0x03002460] = LoadGraphicsBlock(0x00000326, 4, 0x20);
                memAddressDict[0x0300401C] = LoadGraphicsBlock(0x138, 4, 0x10);
                memAddressDict[0x0300248C] = LoadGraphicsBlock(0x00000139, 4, 0x10);
                memAddressDict[0x03002374] = LoadGraphicsBlock(0x0000013B, 4, 0x10);
                memAddressDict[0x03005064] = LoadGraphicsBlock(0x0000013D, 4, 0x10);
                memAddressDict[0x030025A4] = LoadGraphicsBlock(0x00000307, 8, 0x20);
                memAddressDict[0x03003080] = LoadGraphicsBlock(0x00000309, 8, 0x40);
                memAddressDict[0x0300502C] = LoadGraphicsBlock(0x0000030A, 8, 0x40);
                memAddressDict[0x03005108] = LoadGraphicsBlock(0x0000030B, 8, 0x40);
                memAddressDict[0x03004250] = LoadGraphicsBlock(0xf8, 1, 0x10);
                memAddressDict[0x03004F6C] = LoadGraphicsBlock(0x180, 4, 0x40);
                memAddressDict[0x03002520] = LoadGraphicsBlock(0x182, 2, 0x10);
            }
            if (level == 0x1b) {
                memAddressDict[0x03002EF8] = LoadGraphicsBlock(0x368, 4, 0x40);
                memAddressDict[0x03002440] = LoadGraphicsBlock(0x00000369, 9, 0x40);
                memAddressDict[0x030024E8] = LoadGraphicsBlock(0x0000036B, 5, 0x40);
                memAddressDict[0x03002538] = LoadGraphicsBlock(0x36c, 5, 0x40);
                memAddressDict[0x030022F0] = LoadGraphicsBlock(0x0000036D, 6, 0x40);
                memAddressDict[0x03004244] = LoadGraphicsBlock(0x0000036E, 5, 0x40);
                memAddressDict[0x03002528] = LoadGraphicsBlock(0x0000036F, 5, 0x40);
                memAddressDict[0x03002930] = LoadGraphicsBlock(0x370, 1, 0x40);
                memAddressDict[0x030022EC] = LoadGraphicsBlock(0x00000371, 1, 0x40);
                memAddressDict[0x03002EF0] = LoadGraphicsBlock(0x00000373, 1, 0x40);
                memAddressDict[0x03004044] = LoadGraphicsBlock(0x378, 3, 0x40);
                memAddressDict[0x030051C4] = LoadGraphicsBlock(0x374, 1, 0x40);
                memAddressDict[0x03002E70] = LoadGraphicsBlock(0x00000375, 1, 0x40);
                memAddressDict[0x03002884] = LoadGraphicsBlock(0x00000376, 1, 0x40);
                memAddressDict[0x03002888] = LoadGraphicsBlock(0x00000377, 1, 0x40);
                memAddressDict[0x0300509C] = LoadGraphicsBlock(0x0000037A, 1, 0x40);
                memAddressDict[0x0300256C] = LoadGraphicsBlock(0x0000037B, 8, 0x40);
                memAddressDict[0x03003F7C] = LoadGraphicsBlock(0x37c, 8, 0x40);
                memAddressDict[0x030023E0] = LoadGraphicsBlock(0x0000037E, 3, 0x40);
                memAddressDict[0x0300519C] = LoadGraphicsBlock(0x0000037F, 2, 0x40);
                memAddressDict[0x03004398] = LoadGraphicsBlock(0x380, 3, 0x40);
                memAddressDict[0x03004028] = LoadGraphicsBlock(0x00000381, 2, 0x40);
                memAddressDict[0x03004394] = LoadGraphicsBlock(0x00000382, 3, 0x40);
                memAddressDict[0x030025E0] = LoadGraphicsBlock(0x00000383, 2, 0x40);
                memAddressDict[0x03002DF0] = LoadGraphicsBlock(900, 3, 0x40);
                memAddressDict[0x03002948] = LoadGraphicsBlock(0x00000385, 2, 0x40);
                memAddressDict[0x0300522C] = LoadGraphicsBlock(0x00000386, 3, 0x40);
                memAddressDict[0x030022D4] = LoadGraphicsBlock(0x00000387, 2, 0x40);
                memAddressDict[0x03004FA8] = LoadGraphicsBlock(0x388, 4, 0x40);
            }
            if (level == 1 || level == 6 || level == 11 || level == 16 || level == 22) {
                memAddressDict[0x0300290C] = LoadGraphicsBlock(0x00000291, 9, 0x40);
                memAddressDict[0x03003FF0] = LoadGraphicsBlock(0x00000292, 9, 0x40);
                memAddressDict[0x03004F7C] = LoadGraphicsBlock(0x0000034F, 8, 0x40);
                memAddressDict[0x03004360] = LoadGraphicsBlock(0x00000351, 8, 0x20);
                memAddressDict[0x03002DF4] = LoadGraphicsBlock(0x00000353, 7, 0x20);
                memAddressDict[0x030023DC] = LoadGraphicsBlock(0x00000355, 3, 0x20);
                memAddressDict[0x03002564] = LoadGraphicsBlock(0x00000357, 4, 0x20);
                memAddressDict[0x03005250] = LoadGraphicsBlock(0x00000359, 1, 0x40);
                memAddressDict[0x030025B4] = LoadGraphicsBlock(0x0000035A, 2, 0x40);
                memAddressDict[0x03004FD4] = LoadGraphicsBlock(0x0000035B, 2, 0x40);
                memAddressDict[0x03002E98] = LoadGraphicsBlock(0x0000035D, 7, 0x40);
                memAddressDict[0x03002480] = LoadGraphicsBlock(0x0000035E, 8, 0x40);
                memAddressDict[0x030051B4] = LoadGraphicsBlock(0x0000035F, 2, 0x40);
                memAddressDict[0x03004F78] = LoadGraphicsBlock(0x360, 3, 0x40);
                memAddressDict[0x0300231C] = LoadGraphicsBlock(0x00000361, 0xe, 0x40);
                memAddressDict[0x0300289C] = LoadGraphicsBlock(0x00000363, 8, 0x40);
                memAddressDict[0x03002E38] = LoadGraphicsBlock(0x364, 8, 0x40);
                memAddressDict[0x0300291C] = LoadGraphicsBlock(0x00000366, 6, 0x40);
                memAddressDict[0x03004040] = LoadGraphicsBlock(0x000002C3, 2, 0x20);
                memAddressDict[0x03005220] = LoadGraphicsBlock(0x000002C1, 2, 0x40);
            }
            memAddressDict[0x03002924] = LoadGraphicsBlock(0x118, 0xf, 0x40);
            memAddressDict[0x0300507C] = LoadGraphicsBlock(0x00000119, 0xf, 0x40);
            memAddressDict[0x030030B0] = LoadGraphicsBlock(0x000002F1, 2, 0x20);
            memAddressDict[0x03002DE4] = LoadGraphicsBlock(0x000002F9, 10, 0x20);
            memAddressDict[0x03002DF8] = LoadGraphicsBlock(0x000002FB, 0x2d, 0x10);
            memAddressDict[0x030028D0] = LoadGraphicsBlock(0x41, 1, 0x10);
            memAddressDict[0x030028CC] = LoadGraphicsBlock(0x43, 1, 0x10);

            memAddressDict[0x030022C0] = LoadGraphicsBlock(0x3f, 1, 0x10);
            memAddressDict[0x0300234C] = LoadGraphicsBlock(0x00000149, 1, 0x20);
            memAddressDict[0x030022B8] = LoadGraphicsBlock(0x0000014B, 0x16, 0x20);
            memAddressDict[0x03004020] = LoadGraphicsBlock(0x00000302, 7, 0x40);


            if (level != 6 && level != 0xb && level != 0x10 && level != 0x16 && level != 0x1b) {
                if (level == 0x1c || level == 1) {
                    memAddressDict[0x030022C0] = LoadGraphicsBlock(0x3f, 1, 0x10);
                    memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                    memAddressDict[0x0300372C] = LoadGraphicsBlock(0x0000019D, 0x1d, 0x40);
                    memAddressDict[0x03005204] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                    memAddressDict[0x030028C4] = LoadGraphicsBlock(0x19e, 0x21, 0x40);
                    memAddressDict[0x0300258C] = LoadGraphicsBlock(0x0000019F, 10, 0x40);
                    memAddressDict[0x03004378] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                    memAddressDict[0x03002F0C] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                } else {
                    memAddressDict[0x03002324] = LoadGraphicsBlock(0x00000141, 8, 0x40);
                    memAddressDict[0x030051F8] = LoadGraphicsBlock(0x142, 8, 0x40);
                    memAddressDict[0x030025DC] = LoadGraphicsBlock(0x00000143, 3, 0x40);
                    memAddressDict[0x03004358] = LoadGraphicsBlock(0x144, 5, 0x40);
                    memAddressDict[0x03004264] = LoadGraphicsBlock(0x00000145, 8, 0x40);
                    memAddressDict[0x03002368] = LoadGraphicsBlock(0x146, 8, 0x40);
                    memAddressDict[0x03003FF4] = LoadGraphicsBlock(0x00000147, 6, 0x40);
                    memAddressDict[0x0300372C] = LoadGraphicsBlock(0x196, 9, 0x40);
                    memAddressDict[0x03005204] = LoadGraphicsBlock(0x00000197, 0xb, 0x40);
                    memAddressDict[0x030028C4] = LoadGraphicsBlock(0x198, 7, 0x40);
                    memAddressDict[0x0300258C] = LoadGraphicsBlock(0x00000199, 10, 0x40);
                    memAddressDict[0x03004378] = LoadGraphicsBlock(0x19a, 6, 0x40);
                    memAddressDict[0x03002F0C] = LoadGraphicsBlock(0x0000019B, 8, 0x40);
                }
                if (world < 6) {
                    switch (world) {
                        case 0:
                            if (level != 0x1d && level != 0x1f) {
                                memAddressDict[0x03002958] = LoadGraphicsBlock(0x00000187, 1, 0x40);
                                memAddressDict[0x030050F0] = LoadGraphicsBlock(0x0000018B, 1, 0x40);
                                memAddressDict[0x03002E04] = LoadGraphicsBlock(0x00000189, 7, 0x40);
                                memAddressDict[0x03004FB8] = LoadGraphicsBlock(0x00000151, 10, 0x40);
                                memAddressDict[0x030023BC] = LoadGraphicsBlock(0x1a4, 6, 0x40);
                                memAddressDict[0x030024A4] = LoadGraphicsBlock(0x000001A5, 4, 0x40);
                                memAddressDict[0x030024C8] = LoadGraphicsBlock(0x000002C5, 6, 0x40);
                                memAddressDict[0x03002EBC] = LoadGraphicsBlock(0x174, 5, 0x20);
                                memAddressDict[0x03004354] = LoadGraphicsBlock(0x00000272, 8, 0x40);
                                memAddressDict[0x03003F54] = LoadGraphicsBlock(0x274, 8, 0x40);
                                memAddressDict[0x030022D8] = LoadGraphicsBlock(0x00000159, 0xe, 0x20);
                                memAddressDict[0x03003FC8] = LoadGraphicsBlock(0x00000281, 0x29, 0x40);
                                memAddressDict[0x0300290C] = LoadGraphicsBlock(0x00000291, 9, 0x40);
                                memAddressDict[0x03003FF0] = LoadGraphicsBlock(0x00000292, 9, 0x40);
                                memAddressDict[0x03002E40] = LoadGraphicsBlock(0x0000028B, 0xc, 0x40);
                                memAddressDict[0x030030A4] = LoadGraphicsBlock(0x0000028D, 0xc, 0x20);
                                memAddressDict[0x0300514C] = LoadGraphicsBlock(0x0000028F, 8, 0x40);
                                memAddressDict[0x03002540] = LoadGraphicsBlock(0x00000296, 0x16, 0x20);
                                memAddressDict[0x0300232C] = LoadGraphicsBlock(0x294, 0x1f, 0x20);
                                memAddressDict[0x03004024] = LoadGraphicsBlock(0x298, 0xe, 0x20);
                                memAddressDict[0x03004388] = LoadGraphicsBlock(0x00000299, 0x18, 0x20);
                                memAddressDict[0x03003FC0] = LoadGraphicsBlock(0x000001A7, 10, 0x40);
                                memAddressDict[0x030024E0] = LoadGraphicsBlock(0x1a8, 0x13, 0x40);
                                memAddressDict[0x030022BC] = LoadGraphicsBlock(0x000001A9, 6, 0x40);
                                memAddressDict[0x030051D0] = LoadGraphicsBlock(0x1aa, 0x12, 0x40);
                                memAddressDict[0x03004FD0] = LoadGraphicsBlock(0x000001AB, 7, 0x40);
                                memAddressDict[0x030028F8] = LoadGraphicsBlock(0x000001AD, 4, 0x40);

                                memAddressDict[0x0300524C] = LoadGraphicsBlock(0x1af, 8, 0x40);
                                memAddressDict[0x0300521C] = LoadGraphicsBlock(0x000001B5, 9, 0x40);
                                memAddressDict[0x03004348] = LoadGraphicsBlock(0x1b4, 3, 0x40);
                                memAddressDict[0x03005164] = LoadGraphicsBlock(0x000001B3, 0x10, 0x40);
                                memAddressDict[0x030022D0] = LoadGraphicsBlock(0x1b0, 4, 0x40);
                                memAddressDict[0x030025D0] = LoadGraphicsBlock(0x1b2, 10, 0x40);

                                memAddressDict[0x03002E68] = LoadGraphicsBlock(0x000001B7, 0xd, 0x40);
                                memAddressDict[0x030030AC] = LoadGraphicsBlock(0x1ba, 6, 0x40);
                                memAddressDict[0x030029B4] = LoadGraphicsBlock(0x000001BB, 9, 0x40);
                                memAddressDict[0x0300520C] = LoadGraphicsBlock(0x1b8, 0xb, 0x40);
                                memAddressDict[0x03002360] = LoadGraphicsBlock(0x000001B9, 0xb, 0x40);
                                memAddressDict[0x03003F64] = LoadGraphicsBlock(0x1bc, 5, 0x40);
                                memAddressDict[0x030051CC] = LoadGraphicsBlock(0x000001BD, 2, 0x40);

                                memAddressDict[0x03003F78] = LoadGraphicsBlock(0x000001C7, 0xb, 0x40);
                                memAddressDict[0x030051A4] = LoadGraphicsBlock(0x1c8, 0xc, 0x40);
                                memAddressDict[0x03002EE4] = LoadGraphicsBlock(0x000001C9, 0xc, 0x40);
                                memAddressDict[0x030050AC] = LoadGraphicsBlock(0x1ca, 10, 0x40);
                                memAddressDict[0x03005110] = LoadGraphicsBlock(0x000001CB, 0x10, 0x40);
                                memAddressDict[0x03002ED8] = LoadGraphicsBlock(0x1cc, 0x23, 0x40);
                                memAddressDict[0x030050F4] = LoadGraphicsBlock(0x000001CD, 0x1b, 0x40);

                                memAddressDict[0x03002378] = LoadGraphicsBlock(0x000001BF, 0xb, 0x40);
                                memAddressDict[0x030023A0] = LoadGraphicsBlock(0x000001C1, 9, 0x40);
                                memAddressDict[0x030025A8] = LoadGraphicsBlock(0x1c2, 10, 0x40);
                                memAddressDict[0x030025C8] = LoadGraphicsBlock(0x000001C3, 7, 0x40);
                                memAddressDict[0x030022E8] = LoadGraphicsBlock(0x1c4, 10, 0x40);
                                memAddressDict[0x03002E48] = LoadGraphicsBlock(0x1c0, 0x10, 0x40);
                                memAddressDict[0x03003FAC] = LoadGraphicsBlock(0x000001C5, 4, 0x40);

                                memAddressDict[0x030029C8] = LoadGraphicsBlock(0x1d0, 0x10, 0x40);
                                memAddressDict[0x03002F04] = LoadGraphicsBlock(0x000001CF, 0x19, 0x40);
                                memAddressDict[0x03002950] = LoadGraphicsBlock(0x000001D1, 10, 0x40);
                                memAddressDict[0x03002E8C] = LoadGraphicsBlock(0x1d4, 2, 0x40);
                                memAddressDict[0x03002940] = LoadGraphicsBlock(0x1d2, 5, 0x40);
                                memAddressDict[0x0300288C] = LoadGraphicsBlock(0x000001D3, 10, 0x40);
                            }
                            memAddressDict[0x03005150] = LoadGraphicsBlock(0x000002E1, 6, 0x40);
                            memAddressDict[0x03003730] = LoadGraphicsBlock(0x000002E2, 8, 0x40);
                            memAddressDict[0x03003FBC] = LoadGraphicsBlock(0x2e4, 6, 0x40);
                            memAddressDict[0x030023C0] = LoadGraphicsBlock(0x000002E5, 8, 0x40);
                            memAddressDict[0x03002E9C] = LoadGraphicsBlock(0x000002E7, 6, 0x40);
                            memAddressDict[0x03004FF4] = LoadGraphicsBlock(0x2e8, 8, 0x40);
                            memAddressDict[0x0300431C] = LoadGraphicsBlock(0x000002ED, 2, 0x20);
                            memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                            memAddressDict[0x030030B0] = LoadGraphicsBlock(0x000002F1, 2, 0x20);
                            memAddressDict[0x03004018] = LoadGraphicsBlock(0x000002F3, 6, 0x40);
                            memAddressDict[0x03005244] = LoadGraphicsBlock(0x2f4, 1, 0x40);
                            memAddressDict[0x030051E4] = LoadGraphicsBlock(0x000002F5, 8, 0x40);
                            memAddressDict[0x030051BC] = LoadGraphicsBlock(0x000002F7, 6, 0x20);
                            break;
                        case 1:
                            memAddressDict[0x03004270] = LoadGraphicsBlock(0x17c, 4, 0x40);
                            memAddressDict[0x030023CC] = LoadGraphicsBlock(0x00000265, 0xc, 0x20);
                            memAddressDict[0x030028B4] = LoadGraphicsBlock(0x00000266, 6, 0x10);
                            memAddressDict[0x030023F0] = LoadGraphicsBlock(0x268, 6, 0x20);
                            memAddressDict[0x03002DEC] = LoadGraphicsBlock(0x26c, 4, 0x10);
                            memAddressDict[0x03002E5C] = LoadGraphicsBlock(0x0000026D, 4, 0x10);
                            memAddressDict[0x03004FC8] = LoadGraphicsBlock(0x0000026E, 4, 0x10);
                            memAddressDict[0x03002370] = LoadGraphicsBlock(0x17e, 7, 0x40);
                            memAddressDict[0x03004FF0] = LoadGraphicsBlock(0x00000183, 1, 0x40);
                            memAddressDict[0x030024DC] = LoadGraphicsBlock(0x00000185, 1, 0x10);
                            memAddressDict[0x03002908] = LoadGraphicsBlock(0x0000014F, 10, 0x40);
                            memAddressDict[0x03002EB0] = LoadGraphicsBlock(0x304, 7, 0x20);
                            memAddressDict[0x03002904] = LoadGraphicsBlock(0x00000193, 0xb, 0x40);
                            memAddressDict[0x030051F0] = LoadGraphicsBlock(0x194, 7, 0x40);
                            memAddressDict[0x03005188] = LoadGraphicsBlock(0x000002A9, 0x14, 0x40);
                            memAddressDict[0x03002300] = LoadGraphicsBlock(0x000002A7, 0x18, 0x20);
                            memAddressDict[0x03002E58] = LoadGraphicsBlock(0x0000029F, 0x15, 0x20);
                            memAddressDict[0x030042F8] = LoadGraphicsBlock(0x1d6, 5, 0x40);
                            memAddressDict[0x030023FC] = LoadGraphicsBlock(0x000001D7, 6, 0x40);
                            memAddressDict[0x030051DC] = LoadGraphicsBlock(0x1d8, 0xb, 0x40);
                            memAddressDict[0x03004FE8] = LoadGraphicsBlock(0x000001D9, 10, 0x40);
                            memAddressDict[0x03002354] = LoadGraphicsBlock(0x1da, 0x15, 0x40);
                            memAddressDict[0x03004F68] = LoadGraphicsBlock(0x000001DB, 6, 0x40);

                            memAddressDict[0x03002F14] = LoadGraphicsBlock(0x000001E9, 10, 0x40);
                            memAddressDict[0x0300293C] = LoadGraphicsBlock(0x1ea, 10, 0x40);
                            memAddressDict[0x03002920] = LoadGraphicsBlock(0x000001EB, 10, 0x40);
                            memAddressDict[0x030023EC] = LoadGraphicsBlock(0x000001ED, 8, 0x40);
                            memAddressDict[0x03004368] = LoadGraphicsBlock(0x1ec, 9, 0x40);

                            memAddressDict[0x03002568] = LoadGraphicsBlock(0x1f0, 6, 0x40);
                            memAddressDict[0x03002E50] = LoadGraphicsBlock(0x000001F1, 0x10, 0x40);
                            memAddressDict[0x03002F18] = LoadGraphicsBlock(0x000001F3, 0x10, 0x40);
                            memAddressDict[0x0300433C] = LoadGraphicsBlock(0x000001EF, 7, 0x40);
                            memAddressDict[0x03002504] = LoadGraphicsBlock(500, 9, 0x40);
                            memAddressDict[0x0300438C] = LoadGraphicsBlock(0x1f2, 7, 0x40);
                            memAddressDict[0x03005224] = LoadGraphicsBlock(0x000001F7, 0x10, 0x40);
                            memAddressDict[0x030043A0] = LoadGraphicsBlock(0x1f8, 6, 0x40);
                            memAddressDict[0x0300503C] = LoadGraphicsBlock(0x1f6, 0x10, 0x40);
                            memAddressDict[0x0300511C] = LoadGraphicsBlock(0x1fa, 2, 0x40);

                            memAddressDict[0x030025E4] = LoadGraphicsBlock(0x000001E5, 6, 0x40);
                            memAddressDict[0x030024FC] = LoadGraphicsBlock(0x1e4, 3, 0x40);
                            memAddressDict[0x030030C4] = LoadGraphicsBlock(0x000001E1, 10, 0x40);
                            memAddressDict[0x03004FB0] = LoadGraphicsBlock(0x000001DF, 7, 0x40);
                            memAddressDict[0x030051C0] = LoadGraphicsBlock(0x1e0, 3, 0x40);
                            memAddressDict[0x03002EE0] = LoadGraphicsBlock(0x1e2, 8, 0x40);
                            memAddressDict[0x03002870] = LoadGraphicsBlock(0x000001DD, 3, 0x40);
                            memAddressDict[0x030023B8] = LoadGraphicsBlock(0x000001E3, 5, 0x40);
                            memAddressDict[0x030051A0] = LoadGraphicsBlock(0x1de, 5, 0x40);
                            memAddressDict[0x030022B0] = LoadGraphicsBlock(0x000001E7, 4, 0x40);

                            memAddressDict[0x03002378] = LoadGraphicsBlock(0x000001BF, 0xb, 0x40);
                            memAddressDict[0x030023A0] = LoadGraphicsBlock(0x000001C1, 9, 0x40);
                            memAddressDict[0x030025A8] = LoadGraphicsBlock(0x1c2, 10, 0x40);
                            memAddressDict[0x030025C8] = LoadGraphicsBlock(0x000001C3, 7, 0x40);
                            memAddressDict[0x030022E8] = LoadGraphicsBlock(0x1c4, 10, 0x40);
                            memAddressDict[0x03002E48] = LoadGraphicsBlock(0x1c0, 0x10, 0x40);
                            memAddressDict[0x03003FAC] = LoadGraphicsBlock(0x000001C5, 4, 0x40);

                            memAddressDict[0x03004318] = LoadGraphicsBlock(0x1fc, 0xe, 0x40);
                            memAddressDict[0x03005128] = LoadGraphicsBlock(0x000001FD, 0x18, 0x40);

                            memAddressDict[0x030029B8] = LoadGraphicsBlock(0x000001FF, 10, 0x40);
                            memAddressDict[0x03005248] = LoadGraphicsBlock(0x200, 5, 0x40);

                            memAddressDict[0x030029C8] = LoadGraphicsBlock(0x1d0, 0x10, 0x40);
                            memAddressDict[0x03002F04] = LoadGraphicsBlock(0x000001CF, 0x19, 0x40);
                            memAddressDict[0x03002950] = LoadGraphicsBlock(0x000001D1, 10, 0x40);
                            memAddressDict[0x03002E8C] = LoadGraphicsBlock(0x1d4, 2, 0x40);
                            memAddressDict[0x03002940] = LoadGraphicsBlock(0x1d2, 5, 0x40);
                            memAddressDict[0x0300288C] = LoadGraphicsBlock(0x000001D3, 10, 0x40);
                            break;
                        case 2:
                            memAddressDict[0x0300292C] = LoadGraphicsBlock(0x000002BA, 4, 0x40);
                            memAddressDict[0x03002900] = LoadGraphicsBlock(0x000002BB, 8, 0x40);
                            memAddressDict[0x030030CC] = LoadGraphicsBlock(700, 2, 0x40);
                            memAddressDict[0x03003F70] = LoadGraphicsBlock(0x000002BD, 4, 0x40);
                            memAddressDict[0x03002ED0] = LoadGraphicsBlock(0x00000203, 8, 0x40);
                            memAddressDict[0x03003F50] = LoadGraphicsBlock(0x00000205, 0xb, 0x40);
                            memAddressDict[0x03002E88] = LoadGraphicsBlock(0x204, 8, 0x40);
                            memAddressDict[0x0300510C] = LoadGraphicsBlock(0x00000202, 0xb, 0x40);
                            memAddressDict[0x0300251C] = LoadGraphicsBlock(0x00000206, 6, 0x40);

                            memAddressDict[0x030022E0] = LoadGraphicsBlock(0x0000020A, 3, 0x40);
                            memAddressDict[0x03002458] = LoadGraphicsBlock(0x208, 4, 0x40);
                            memAddressDict[0x030042E8] = LoadGraphicsBlock(0x00000209, 0xb, 0x40);
                            memAddressDict[0x03004FCC] = LoadGraphicsBlock(0x0000020B, 7, 0x40);
                            memAddressDict[0x03005028] = LoadGraphicsBlock(0x20c, 0xb, 0x40);
                            memAddressDict[0x03002588] = LoadGraphicsBlock(0x0000020D, 3, 0x40);
                            memAddressDict[0x03004F74] = LoadGraphicsBlock(0x0000020F, 4, 0x40);

                            memAddressDict[0x03004F94] = LoadGraphicsBlock(0x00000217, 0xe, 0x40);
                            memAddressDict[0x03002310] = LoadGraphicsBlock(0x218, 9, 0x40);
                            memAddressDict[0x03005070] = LoadGraphicsBlock(0x0000021B, 0x11, 0x40);
                            memAddressDict[0x03002EE8] = LoadGraphicsBlock(0x00000219, 0x14, 0x40);
                            memAddressDict[0x03005034] = LoadGraphicsBlock(0x0000021A, 0xb, 0x40);

                            memAddressDict[0x030025CC] = LoadGraphicsBlock(0x00000211, 0x14, 0x40);
                            memAddressDict[0x03002340] = LoadGraphicsBlock(0x00000213, 9, 0x40);
                            memAddressDict[0x03003098] = LoadGraphicsBlock(0x214, 0x11, 0x40);
                            memAddressDict[0x03002474] = LoadGraphicsBlock(0x00000215, 0x16, 0x40);

                            memAddressDict[0x03002464] = LoadGraphicsBlock(0x0000021E, 8, 0x40);
                            memAddressDict[0x03002EDC] = LoadGraphicsBlock(0x0000021F, 0x11, 0x40);
                            memAddressDict[0x030051E8] = LoadGraphicsBlock(0x0000021D, 0x13, 0x40);
                            memAddressDict[0x030051F4] = LoadGraphicsBlock(0x220, 0xb, 0x40);
                            memAddressDict[0x030029C4] = LoadGraphicsBlock(0x00000221, 9, 0x40);
                            memAddressDict[0x03002944] = LoadGraphicsBlock(0x00000222, 8, 0x40);

                            memAddressDict[0x03005140] = LoadGraphicsBlock(0x178, 4, 0x40);
                            memAddressDict[0x03004FE0] = LoadGraphicsBlock(0x17a, 7, 0x40);
                            memAddressDict[0x030030B4] = LoadGraphicsBlock(0x000002CA, 0x30, 0x20);
                            memAddressDict[0x03005220] = LoadGraphicsBlock(0x000002C1, 2, 0x40);
                            memAddressDict[0x03003088] = LoadGraphicsBlock(0x0000030D, 8, 0x20);
                            memAddressDict[0x030025B8] = LoadGraphicsBlock(0x0000030E, 8, 0x20);
                            memAddressDict[0x030024B8] = LoadGraphicsBlock(0x0000030F, 8, 0x40);
                            memAddressDict[0x0300435C] = LoadGraphicsBlock(0x000002A3, 1, 0x40);
                            memAddressDict[0x0300245C] = LoadGraphicsBlock(0x000002A5, 1, 0x40);
                            memAddressDict[0x030030C8] = LoadGraphicsBlock(0x2a0, 0x16, 0x20);
                            memAddressDict[0x03002E58] = LoadGraphicsBlock(0x0000029F, 0x15, 0x20);

                            break;
                        case 3:
                            if (level != 0x1d && level != 0x1f) {
                                memAddressDict[0x03004354] = LoadGraphicsBlock(0x00000272, 8, 0x40);
                                memAddressDict[0x03003F54] = LoadGraphicsBlock(0x274, 8, 0x40);
                                memAddressDict[0x03003FC8] = LoadGraphicsBlock(0x00000281, 0x29, 0x40);
                                memAddressDict[0x0300523C] = LoadGraphicsBlock(0x0000018D, 1, 0x40);
                                memAddressDict[0x03002444] = LoadGraphicsBlock(0x00000153, 10, 0x40);
                                memAddressDict[0x03005200] = LoadGraphicsBlock(0x0000018F, 7, 0x40);
                                memAddressDict[0x03002334] = LoadGraphicsBlock(0x270, 1, 0x40);
                                memAddressDict[0x030050A0] = LoadGraphicsBlock(0x00000155, 1, 0x40);
                                memAddressDict[0x03004040] = LoadGraphicsBlock(0x000002C3, 2, 0x20);
                                memAddressDict[0x030028DC] = LoadGraphicsBlock(0x2cc, 0x1e, 0x20);
                                memAddressDict[0x03002ED4] = LoadGraphicsBlock(0x00000157, 0xe, 0x20);
                                memAddressDict[0x030042EC] = LoadGraphicsBlock(0x000002B2, 8, 0x40);
                                memAddressDict[0x03003720] = LoadGraphicsBlock(0x0000015B, 2, 0x40);
                                memAddressDict[0x03003F5C] = LoadGraphicsBlock(0x000002AB, 0x1b, 0x20);
                                memAddressDict[0x03005188] = LoadGraphicsBlock(0x000002A9, 0x14, 0x40);
                                memAddressDict[0x03005190] = LoadGraphicsBlock(0x224, 6, 0x40);
                                memAddressDict[0x030022FC] = LoadGraphicsBlock(0x00000225, 7, 0x40);
                                memAddressDict[0x03004FC0] = LoadGraphicsBlock(0x00000226, 7, 0x40);
                                memAddressDict[0x030051FC] = LoadGraphicsBlock(0x00000227, 0x10, 0x40);
                                memAddressDict[0x030023A8] = LoadGraphicsBlock(0x228, 0xf, 0x40);
                                memAddressDict[0x030028FC] = LoadGraphicsBlock(0x0000022A, 8, 0x40);
                                memAddressDict[0x030050E8] = LoadGraphicsBlock(0x0000022B, 8, 0x40);
                                memAddressDict[0x03003F84] = LoadGraphicsBlock(0x00000229, 6, 0x40);
                                memAddressDict[0x030023D0] = LoadGraphicsBlock(0x0000022D, 4, 0x40);

                                memAddressDict[0x030023B0] = LoadGraphicsBlock(0x247, 8, 0x40);
                                memAddressDict[0x0300239C] = LoadGraphicsBlock(0x0000024A, 0xd, 0x40);
                                memAddressDict[0x03004F70] = LoadGraphicsBlock(0x00000249, 8, 0x40);
                                memAddressDict[0x03005038] = LoadGraphicsBlock(0x248, 8, 0x40);
                                memAddressDict[0x030029D0] = LoadGraphicsBlock(0x0000024B, 4, 0x40);
                                memAddressDict[0x03003738] = LoadGraphicsBlock(0x24c, 4, 0x40);

                                memAddressDict[0x03003F74] = LoadGraphicsBlock(0x00000239, 0x10, 0x40);
                                memAddressDict[0x03003090] = LoadGraphicsBlock(0x0000023A, 0xd, 0x40);
                                memAddressDict[0x03005054] = LoadGraphicsBlock(0x0000023E, 9, 0x40);
                                memAddressDict[0x030043A4] = LoadGraphicsBlock(0x0000023D, 6, 0x40);
                                memAddressDict[0x0300506C] = LoadGraphicsBlock(0x0000023F, 7, 0x40);
                                memAddressDict[0x030025C0] = LoadGraphicsBlock(0x23c, 0xe, 0x40);

                                memAddressDict[0x030028E8] = LoadGraphicsBlock(0x00000241, 8, 0x40);
                                memAddressDict[0x030050FC] = LoadGraphicsBlock(0x00000242, 9, 0x40);
                                memAddressDict[0x030023E8] = LoadGraphicsBlock(0x00000243, 8, 0x40);
                                memAddressDict[0x030028E4] = LoadGraphicsBlock(0x244, 10, 0x40);
                                memAddressDict[0x03005214] = LoadGraphicsBlock(0x00000245, 0x16, 0x40);

                                memAddressDict[0x03002498] = LoadGraphicsBlock(0x00000231, 0x1c, 0x40);
                                memAddressDict[0x0300257C] = LoadGraphicsBlock(0x00000232, 0x12, 0x40);
                                memAddressDict[0x0300233C] = LoadGraphicsBlock(0x234, 0xc, 0x40);
                                memAddressDict[0x03002488] = LoadGraphicsBlock(0x00000235, 0xd, 0x40);
                                memAddressDict[0x03005098] = LoadGraphicsBlock(0x00000233, 0x10, 0x40);
                                memAddressDict[0x03003FD8] = LoadGraphicsBlock(0x00000237, 1, 0x40);

                                memAddressDict[0x03002378] = LoadGraphicsBlock(0x000001BF, 0xb, 0x40);
                                memAddressDict[0x030023A0] = LoadGraphicsBlock(0x000001C1, 9, 0x40);
                                memAddressDict[0x030025A8] = LoadGraphicsBlock(0x1c2, 10, 0x40);
                                memAddressDict[0x030025C8] = LoadGraphicsBlock(0x000001C3, 7, 0x40);
                                memAddressDict[0x030022E8] = LoadGraphicsBlock(0x1c4, 10, 0x40);
                                memAddressDict[0x03002E48] = LoadGraphicsBlock(0x1c0, 0x10, 0x40);
                                memAddressDict[0x03003FAC] = LoadGraphicsBlock(0x000001C5, 4, 0x40);
                            }
                            memAddressDict[0x03005150] = LoadGraphicsBlock(0x000002E1, 6, 0x40);
                            memAddressDict[0x03003730] = LoadGraphicsBlock(0x000002E2, 8, 0x40);
                            memAddressDict[0x03003FBC] = LoadGraphicsBlock(0x2e4, 6, 0x40);
                            memAddressDict[0x030023C0] = LoadGraphicsBlock(0x000002E5, 8, 0x40);
                            memAddressDict[0x03002E9C] = LoadGraphicsBlock(0x000002E7, 6, 0x40);
                            memAddressDict[0x03004FF4] = LoadGraphicsBlock(0x2e8, 8, 0x40);
                            memAddressDict[0x0300431C] = LoadGraphicsBlock(0x000002ED, 2, 0x20);
                            memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                            memAddressDict[0x030030B0] = LoadGraphicsBlock(0x000002F1, 2, 0x20);
                            memAddressDict[0x03004018] = LoadGraphicsBlock(0x000002F3, 6, 0x40);
                            memAddressDict[0x03005244] = LoadGraphicsBlock(0x2f4, 1, 0x40);
                            memAddressDict[0x030051E4] = LoadGraphicsBlock(0x000002F5, 8, 0x40);
                            memAddressDict[0x030051BC] = LoadGraphicsBlock(0x000002F7, 6, 0x20);
                            break;
                        case 4:
                            if (level != 1) {
                                memAddressDict[0x0300403C] = LoadGraphicsBlock(0x000002B3, 2, 0x20);
                                memAddressDict[0x03004F9C] = LoadGraphicsBlock(0x000002C6, 8, 0x40);
                                memAddressDict[0x03004390] = LoadGraphicsBlock(0x000002C7, 0x21, 0x20);
                                memAddressDict[0x03004FFC] = LoadGraphicsBlock(0x2c8, 0x18, 0x20);
                                memAddressDict[0x030050B0] = LoadGraphicsBlock(0x000002DF, 9, 0x40);
                                memAddressDict[0x0300439C] = LoadGraphicsBlock(0x000002DE, 4, 0x40);
                                memAddressDict[0x0300292C] = LoadGraphicsBlock(0x000002B5, 4, 0x40);
                                memAddressDict[0x03002900] = LoadGraphicsBlock(0x000002B6, 8, 0x40);
                                memAddressDict[0x030030CC] = LoadGraphicsBlock(0x000002B7, 2, 0x40);
                                memAddressDict[0x03003F70] = LoadGraphicsBlock(0x2b8, 4, 0x40);
                                memAddressDict[0x03005088] = LoadGraphicsBlock(0x13e, 4, 0x20);
                                memAddressDict[0x03005228] = LoadGraphicsBlock(0x000002BF, 2, 0x20);
                                memAddressDict[0x03003FCC] = LoadGraphicsBlock(0x00000191, 7, 0x40);
                                memAddressDict[0x03005114] = LoadGraphicsBlock(0x0000027A, 8, 0x40);
                                memAddressDict[0x03005090] = LoadGraphicsBlock(0x0000027B, 8, 0x40);
                                memAddressDict[0x030042F4] = LoadGraphicsBlock(0x27c, 8, 0x40);
                                memAddressDict[0x03004034] = LoadGraphicsBlock(0x0000027D, 8, 0x40);
                                memAddressDict[0x030023A4] = LoadGraphicsBlock(0x0000027E, 8, 0x40);
                                memAddressDict[0x03002500] = LoadGraphicsBlock(0x0000027F, 8, 0x40);
                                memAddressDict[0x030051B8] = LoadGraphicsBlock(0x00000283, 0xd, 0x40);
                                memAddressDict[0x03004330] = LoadGraphicsBlock(0x00000285, 6, 0x40);
                                memAddressDict[0x03002580] = LoadGraphicsBlock(0x00000287, 6, 0x40);
                                memAddressDict[0x03004338] = LoadGraphicsBlock(0x00000289, 0x1d, 0x40);
                                memAddressDict[0x0300232C] = LoadGraphicsBlock(0x294, 0x1f, 0x20);
                                memAddressDict[0x03002E58] = LoadGraphicsBlock(0x0000029F, 0x15, 0x20);
                                memAddressDict[0x0300501C] = LoadGraphicsBlock(0x0000024E, 0x15, 0x40);
                                memAddressDict[0x030028C0] = LoadGraphicsBlock(0x0000024F, 6, 0x40);
                                memAddressDict[0x03004320] = LoadGraphicsBlock(0x00000251, 6, 0x40);
                                memAddressDict[0x03003FFC] = LoadGraphicsBlock(0x250, 6, 0x40);
                                memAddressDict[0x03005144] = LoadGraphicsBlock(0x00000252, 8, 0x40);

                                memAddressDict[0x03002DE0] = LoadGraphicsBlock(0x0000025E, 6, 0x40);
                                memAddressDict[0x03002EC8] = LoadGraphicsBlock(0x25c, 0x17, 0x40);
                                memAddressDict[0x03002338] = LoadGraphicsBlock(0x0000025F, 0xc, 0x40);
                                memAddressDict[0x03004FF8] = LoadGraphicsBlock(0x0000025D, 8, 0x40);

                                memAddressDict[0x03002464] = LoadGraphicsBlock(0x0000021E, 8, 0x40);
                                memAddressDict[0x03002EDC] = LoadGraphicsBlock(0x0000021F, 0x11, 0x40);
                                memAddressDict[0x030051E8] = LoadGraphicsBlock(0x0000021D, 0x13, 0x40);
                                memAddressDict[0x030051F4] = LoadGraphicsBlock(0x220, 0xb, 0x40);

                                memAddressDict[0x03002ED0] = LoadGraphicsBlock(0x00000203, 8, 0x40);
                                memAddressDict[0x03003F50] = LoadGraphicsBlock(0x00000205, 0xb, 0x40);
                                memAddressDict[0x03002E88] = LoadGraphicsBlock(0x204, 8, 0x40);
                                memAddressDict[0x0300510C] = LoadGraphicsBlock(0x00000202, 0xb, 0x40);
                                memAddressDict[0x0300251C] = LoadGraphicsBlock(0x00000206, 6, 0x40);

                                memAddressDict[0x030024F0] = LoadGraphicsBlock(0x254, 0x15, 0x40);
                                memAddressDict[0x03002364] = LoadGraphicsBlock(0x00000255, 9, 0x40);
                                memAddressDict[0x03004FEC] = LoadGraphicsBlock(0x00000257, 0xc, 0x40);
                                memAddressDict[0x03002E7C] = LoadGraphicsBlock(0x00000256, 7, 0x40);
                                memAddressDict[0x030030C0] = LoadGraphicsBlock(600, 0xd, 0x40);
                                memAddressDict[0x03005060] = LoadGraphicsBlock(0x0000025A, 3, 0x40);
                            }
                            memAddressDict[0x03004340] = LoadGraphicsBlock(0x000002AD, 4, 0x40);
                            memAddressDict[0x03004010] = LoadGraphicsBlock(0x000002AE, 8, 0x40);
                            memAddressDict[0x030025AC] = LoadGraphicsBlock(0x2b0, 7, 0x40);
                            memAddressDict[0x03002514] = LoadGraphicsBlock(0x176, 1, 0x40);
                            memAddressDict[0x03002544] = LoadGraphicsBlock(0x00000261, 4, 0x20);
                            memAddressDict[0x030023B4] = LoadGraphicsBlock(0x00000263, 0x14, 0x20);
                            memAddressDict[0x030050EC] = LoadGraphicsBlock(0x0000026A, 8, 0x20);
                            memAddressDict[0x03004308] = LoadGraphicsBlock(0x0000026B, 4, 0x20);
                            memAddressDict[0x03003728] = LoadGraphicsBlock(0x00000269, 0xe, 0x40);

                            if (level == 1) {
                                memAddressDict[0x03002534] = LoadGraphicsBlock(0x0000033A, 0xe, 0x40);
                                memAddressDict[0x0300516C] = LoadGraphicsBlock(0x00000339, 7, 0x40);
                                memAddressDict[0x03003FC4] = LoadGraphicsBlock(0x0000029B, 8, 0x40);
                                memAddressDict[0x03005044] = LoadGraphicsBlock(0x29c, 9, 0x40);
                                memAddressDict[0x03002DFC] = LoadGraphicsBlock(0x0000029D, 8, 0x40);

                                //Hardcoded
                                LoadGraphicsBlockUnknown(812);
                            }
                            break;
                        case 5:
                            memAddressDict[0x03002520] = LoadGraphicsBlock(0x182, 2, 0x10);
                            memAddressDict[0x03004270] = LoadGraphicsBlock(0x17c, 4, 0x40);
                            memAddressDict[0x030023CC] = LoadGraphicsBlock(0x00000265, 0xc, 0x20);
                            memAddressDict[0x030028B4] = LoadGraphicsBlock(0x00000266, 6, 0x10);
                            memAddressDict[0x030023F0] = LoadGraphicsBlock(0x268, 6, 0x20);
                            memAddressDict[0x03002DEC] = LoadGraphicsBlock(0x26c, 4, 0x10);
                            memAddressDict[0x03002E5C] = LoadGraphicsBlock(0x0000026D, 4, 0x10);
                            memAddressDict[0x03004FC8] = LoadGraphicsBlock(0x0000026E, 4, 0x10);
                            memAddressDict[0x03002370] = LoadGraphicsBlock(0x17e, 7, 0x40);
                            memAddressDict[0x03004FF0] = LoadGraphicsBlock(0x00000183, 1, 0x40);
                            memAddressDict[0x030024DC] = LoadGraphicsBlock(0x00000185, 1, 0x10);
                            memAddressDict[0x03004248] = LoadGraphicsBlock(0x00000276, 9, 0x40);
                            memAddressDict[0x03002348] = LoadGraphicsBlock(0x2d0, 1, 0x40);
                            memAddressDict[0x03004324] = LoadGraphicsBlock(0x2d4, 1, 0x40);
                            memAddressDict[0x03003724] = LoadGraphicsBlock(0x2d8, 1, 0x40);
                            memAddressDict[0x03002574] = LoadGraphicsBlock(0x2dc, 1, 0x40);
                            memAddressDict[0x030022F4] = LoadGraphicsBlock(0x278, 8, 0x10);
                            memAddressDict[0x0300518C] = LoadGraphicsBlock(0x0000015D, 5, 0x40);
                            memAddressDict[0x03005100] = LoadGraphicsBlock(0x00000161, 5, 0x20);
                            memAddressDict[0x03002E94] = LoadGraphicsBlock(0x0000015F, 5, 0x20);
                            memAddressDict[0x030050A4] = LoadGraphicsBlock(0x00000163, 5, 0x40);
                            memAddressDict[0x03003F60] = LoadGraphicsBlock(0x00000167, 5, 0x20);
                            memAddressDict[0x030029CC] = LoadGraphicsBlock(0x00000165, 5, 0x20);
                            memAddressDict[0x03002F1C] = LoadGraphicsBlock(0x172, 0x16, 0x40);
                            memAddressDict[0x03004260] = LoadGraphicsBlock(0x00000171, 0x16, 0x20);
                            memAddressDict[0x03002EB4] = LoadGraphicsBlock(0x170, 0x16, 0x10);

                            memAddressDict[0x03003F58] = LoadGraphicsBlock(/*0x03000010 + 0xb +*/ 0x00000169, 8, 0x40); // is this right?
                            break;
                    }
                    LoadGraphicsBlock(0x10, 1, 0x20); // What is this? It loads at the end of every world block and I have no idea at what address
                }
            } else {
                memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                memAddressDict[0x0300372C] = LoadGraphicsBlock(0x0000019D, 0x1d, 0x40);
                memAddressDict[0x03005204] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                memAddressDict[0x030028C4] = LoadGraphicsBlock(0x19e, 0x21, 0x40);
                memAddressDict[0x0300258C] = LoadGraphicsBlock(0x0000019F, 10, 0x40);
                memAddressDict[0x03004378] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                memAddressDict[0x03002F0C] = LoadGraphicsBlock(0x000001A1, 10, 0x40);

                // Hardcoded
                LoadGraphicsBlockUnknown(828);
            }

            // Load extra blocks
            for (uint i = 145; i <= 230; i++) { // Rayman
                LoadGraphicsBlockUnknown(i);
            }
            for (uint i = 232; i <= 246; i++) { // Gangster
                LoadGraphicsBlockUnknown(i);
            }
            for (uint i = 248; i <= 263; i++) { // Punk
                LoadGraphicsBlockUnknown(i);
            }
            for (uint i = 265; i <= 278; i++) { // Granny
                LoadGraphicsBlockUnknown(i);
            }
            for (uint i = 280; i <= 281; i++) { // Granny carrot
                LoadGraphicsBlockUnknown(i);
            }
            for (uint i = 283; i <= 295; i++) { // Rocker
                LoadGraphicsBlockUnknown(i);
            }
            for (uint i = 297; i <= 311; i++) { // Disco
                LoadGraphicsBlockUnknown(i);
            }

            return memAddressDict;
        }

        public void AssignObjectValues(GBARRR_Object obj, GBARRR_ROM rom, int level, int world) {
            if(level == 0x1b) world = 5;
            if(world > 5) return;
            GBARRR_GraphicsTableEntry graphicsEntry = rom.GraphicsTable0[world].LastOrDefault(e => e.Key * 2 == obj.Ushort_0C);
            if (graphicsEntry == null) return;
            obj.P_GraphicsIndex = graphicsEntry.Value;
            obj.P_SpriteSize = rom.GraphicsTable1[world][obj.P_GraphicsIndex];
            obj.P_FrameCount = rom.GraphicsTable2[world][obj.P_GraphicsIndex];

            switch (world) {
                case 0:
					// 2 is always a big thing. We'll check that later
					#region World 0
					if (obj.P_GraphicsIndex == 3) {
                        obj.P_FunctionPointer = 0x08037B9D;
                    }
                    if (obj.P_GraphicsIndex == 4) {
                        obj.P_FunctionPointer = 0x08037CA1;
                    }
                    if (obj.P_GraphicsIndex == 5) {
                        obj.P_GraphicsOffset = 0x03005158;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 6) {
                        obj.P_FunctionPointer = 0x0804DC65;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 8) {
                        obj.P_GraphicsOffset = 0x0300401C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 9) {
                        obj.P_GraphicsOffset = 0x0300248C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 10) {
                        obj.P_GraphicsOffset = 0x03002374;
                        obj.P_PaletteIndex = 0x13a;
                    }
                    if (obj.P_GraphicsIndex == 0xb) {
                        obj.P_GraphicsOffset = 0x03005064;
                        obj.P_PaletteIndex = 0x13c;
                    }
                    if (obj.P_GraphicsIndex == 0x18) {
                        // Boss
                        obj.P_GraphicsOffset = 828; // Hardcoded
                        obj.P_PaletteIndex = 0x0000033B;
                        obj.P_Field0E = 0;
                        obj.P_Field08 = 0;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        //*(undefined4*)0x03005370 = 3;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x32) {
                        obj.P_GraphicsOffset = 0x0300290C;
                        obj.P_PaletteIndex = 0x290;
                        obj.P_SpriteSize = 0x40;
                    }
                    if (obj.P_GraphicsIndex == 0x1c) {
                        obj.P_GraphicsOffset = 0x03003FF0;
                        obj.P_PaletteIndex = 0x290;
                        obj.P_SpriteSize = 0x40;
                    }
                    if (obj.P_GraphicsIndex == 0x1e) {
                        obj.P_GraphicsOffset = 0x03002E40;
                        obj.P_PaletteIndex = 0x0000028A;
                        obj.P_SpriteSize = 0x40;
                    }
                    if (obj.P_GraphicsIndex == 0x43) {
                        obj.P_GraphicsOffset = 0x030030A4;
                        obj.P_PaletteIndex = 0x28c;
                        obj.P_SpriteSize = 0x20;
                    }
                    if (obj.P_GraphicsIndex == 0x1d) {
                        obj.P_GraphicsOffset = 0x0300514C;
                        obj.P_PaletteIndex = 0x0000028E;
                        obj.P_SpriteSize = 0x40;
                    }
                    if (obj.P_GraphicsIndex == 0x46) {
                        obj.P_GraphicsOffset = 0x03002540;
                        obj.P_PaletteIndex = 0x00000295;
                        obj.P_SpriteSize = 0x20;
                    }
                    if (obj.P_GraphicsIndex == 0x44) {
                        obj.P_GraphicsOffset = 0x03004024;
                        obj.P_PaletteIndex = 0x00000297;
                        obj.P_SpriteSize = 0x20;
                    }
                    if (obj.P_GraphicsIndex == 0x45) {
                        obj.P_GraphicsOffset = 0x03004388;
                        obj.P_PaletteIndex = 0x00000297;
                        obj.P_SpriteSize = 0x20;
                    }
                    if (obj.P_GraphicsIndex == 0x1f) {
                        obj.P_GraphicsOffset = 0x0300232C;
                        obj.P_PaletteIndex = 0x00000293;
                        obj.P_SpriteSize = 0x20;
                    }
                    if (obj.P_GraphicsIndex == 0x28) {
                        obj.P_GraphicsOffset = 0x030023BC;
                        obj.P_PaletteIndex = 0x000001A3;
                    }
                    if (obj.P_GraphicsIndex == 0x29) {
                        obj.P_GraphicsOffset = 0x030024C8;
                        obj.P_PaletteIndex = 0x2c4;
                    }
                    if (obj.P_GraphicsIndex == 0x2a) {
                        obj.P_GraphicsOffset = 0x030051D0;
                        obj.P_PaletteIndex = 0x1a6;
                        obj.P_SpriteHeight = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x2b) {
                        obj.P_GraphicsOffset = 0x0300524C;
                        obj.P_PaletteIndex = 0x1ae;
                        obj.P_SpriteHeight = 1;
                    }
                    if (obj.P_GraphicsIndex == 0x2c) {
                        obj.P_GraphicsOffset = 0x03002E68;
                        obj.P_PaletteIndex = 0x1b6;
                        obj.P_SpriteHeight = 2;
                    }
                    if (obj.P_GraphicsIndex == 0x2d) {
                        obj.P_GraphicsOffset = 0x03002378;
                        obj.P_PaletteIndex = 0x1be;
                        obj.P_SpriteHeight = 4;
                    }
                    if (obj.P_GraphicsIndex == 0x30) {
                        obj.P_GraphicsOffset = 0x03002958;
                        obj.P_PaletteIndex = 0x186;
                    }
                    if (obj.P_GraphicsIndex == 0x31) {
                        obj.P_GraphicsOffset = 0x030050F0;
                        obj.P_PaletteIndex = 0x18a;
                    }
                    if (obj.P_GraphicsIndex == 0x2e) {
                        obj.P_GraphicsOffset = 0x03002F0C;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 0x00000195;
                        obj.P_Field12 = 6;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        //*(Actor**)0x03004278 = actor;
                    }
                    if (obj.P_GraphicsIndex == 0x33) {
                        obj.P_GraphicsOffset = 0x03003F78;
                        obj.P_PaletteIndex = 0x1c6;
                        obj.P_SpriteHeight = 3;
                    }
                    if (obj.P_GraphicsIndex == 0x3f) {
                        obj.P_GraphicsOffset = 0x030029C8;
                        obj.P_PaletteIndex = 0x1ce;
                        obj.P_SpriteHeight = 5;
                    }
                    if (obj.P_GraphicsIndex == 0x34) {
                        obj.P_GraphicsOffset = 0x03002324;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x35) {
                        obj.P_GraphicsOffset = 0x03004264;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x36) {
                        obj.P_GraphicsOffset = 0x03004354;
                        obj.P_PaletteIndex = 0x00000271;
                    }
                    if (obj.P_GraphicsIndex == 0x37) {
                        obj.P_GraphicsOffset = 0x03003F54;
                        obj.P_PaletteIndex = 0x00000273;
                    }
                    if (obj.P_GraphicsIndex == 0x38) {
                        obj.P_GraphicsOffset = 0x03003FC8;
                        obj.P_PaletteIndex = 0x280;
                    }
                    if (obj.P_GraphicsIndex == 0x39) {
                        //0x030052AC = 0x030052AC + 1;
                        if (level == 0x1d) {
                            obj.P_GraphicsOffset = 0x03005150;
                            obj.P_PaletteIndex = 0x2e0;
                        } else {
                            obj.P_GraphicsOffset = 0x03003FBC;
                            obj.P_PaletteIndex = 0x2e3;
                        }
                        obj.P_Field12 = 0;
                        obj.P_Field2C = 0x50;
                    }
                    if (obj.P_GraphicsIndex == 0x3a) {
                        obj.P_GraphicsOffset = 0x03002E9C;
                        obj.P_PaletteIndex = 0x2e6;
                        obj.P_Field12 = 0;
                        obj.P_Field2C = 0x50;
                    }
                    if (obj.P_GraphicsIndex == 0x3b) {
                        obj.P_GraphicsOffset = 0x03002E04;
                        obj.P_PaletteIndex = 0x188;
                    }
                    if (obj.P_GraphicsIndex == 0x3c) {
                        obj.P_GraphicsOffset = 0x03004FB8;
                        obj.P_PaletteIndex = 0x150;
                    }
                    if (obj.P_GraphicsIndex == 0x3e) {
                        obj.P_GraphicsOffset = 0x03004F6C;
                        obj.P_PaletteIndex = 0x17f;
                    }
                    if (obj.P_GraphicsIndex == 0x40) {
                        obj.P_GraphicsOffset = 0x03002EBC;
                        obj.P_PaletteIndex = 0x173;
                        obj.P_Field10 = obj.RuntimeYPosition;
                    }
                    if (obj.P_GraphicsIndex == 0x41) {
                        obj.P_GraphicsOffset = 0x030022D8;
                        obj.P_PaletteIndex = 0x158;
                    }
                    if (obj.P_GraphicsIndex == 0x42) {
                        obj.P_GraphicsOffset = 0x030022B8;
                        obj.P_OtherGraphicsOffset = 0x0300234C;
                        obj.P_Field20 = 100;
                        obj.P_PaletteIndex = 0x148;
                    }
					#endregion
					break;
                case 1:
                    // 2 is always a big thing. We'll check that later
                    #region World 1
                    if (obj.P_GraphicsIndex == 3) {
                        obj.P_FunctionPointer = 0x08037B9D;
                    }
                    if (obj.P_GraphicsIndex == 4) {
                        obj.P_FunctionPointer = 0x08037CA1;
                    }
                    if (obj.P_GraphicsIndex == 5) {
                        obj.P_GraphicsOffset = 0x03005158;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 6) {
                        obj.P_FunctionPointer = 0x0804DC65;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 8) {
                        obj.P_GraphicsOffset = 0x0300401C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 9) {
                        obj.P_GraphicsOffset = 0x0300248C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 10) {
                        obj.P_GraphicsOffset = 0x03002374;
                        obj.P_PaletteIndex = 0x13a;
                    }
                    if (obj.P_GraphicsIndex == 0xb) {
                        obj.P_GraphicsOffset = 0x03005064;
                        obj.P_PaletteIndex = 0x13c;
                    }
                    if (obj.P_GraphicsIndex == 0x10) {
                        obj.P_GraphicsOffset = 0x030028B4;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x11) {
                        obj.P_GraphicsOffset = 0x03002DEC;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x12) {
                        obj.P_GraphicsOffset = 0x03002E5C;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x13) {
                        obj.P_GraphicsOffset = 0x03004FC8;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x18) {
                        // Boss
                        obj.P_GraphicsOffset = 828; // Hardcoded
                        obj.P_PaletteIndex = 0x0000033B;
                        obj.P_Field0E = 0;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        obj.P_SpriteWidth = 0xff;
                        //*(undefined4*)0x03005370 = 3;
                    }
                    if (obj.P_GraphicsIndex == 0x28) {
                        obj.P_GraphicsOffset = 0x030042F8;
                        obj.P_PaletteIndex = 0x000001D5;
                        obj.P_SpriteHeight = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x29) {
                        obj.P_GraphicsOffset = 0x03002F14;
                        obj.P_PaletteIndex = 0x1e8;
                        obj.P_SpriteHeight = 1;
                    }
                    if (obj.P_GraphicsIndex == 0x2a) {
                        obj.P_GraphicsOffset = 0x03002568;
                        obj.P_PaletteIndex = 0x1ee;
                        obj.P_SpriteHeight = 2;
                    }
                    if (obj.P_GraphicsIndex == 0x2b) {
                        obj.P_GraphicsOffset = 0x030051A0;
                        obj.P_PaletteIndex = 0x1dc;
                        obj.P_FrameCount = 5;
                        obj.P_Field34 = obj.P_Field12;
                    }
                    if (obj.P_GraphicsIndex == 0x2c) {
                        obj.P_GraphicsOffset = 0x03002378;
                        obj.P_PaletteIndex = 0x1be;
                        obj.P_SpriteHeight = 4;
                    }
                    if (obj.P_GraphicsIndex == 0x2d) {
                        obj.P_GraphicsOffset = 0x03005128;
                        obj.P_PaletteIndex = 0x000001FB;
                        obj.P_SpriteHeight = 5;
                    }
                    if (obj.P_GraphicsIndex == 0x33) {
                        obj.P_GraphicsOffset = 0x03005248;
                        obj.P_PaletteIndex = 0x1fe;
                        obj.P_SpriteHeight = 6;
                    }
                    if (obj.P_GraphicsIndex == 0x32) {
                        obj.P_GraphicsOffset = 0x030029C8;
                        obj.P_PaletteIndex = 0x1ce;
                        obj.P_SpriteHeight = 7;
                    }
                    if (obj.P_GraphicsIndex == 0x30) {
                        obj.P_GraphicsOffset = 0x03004270;
                        obj.P_PaletteIndex = 0x0000017B;
                    }
                    if (obj.P_GraphicsIndex == 0x34) {
                        obj.P_GraphicsOffset = 0x03002324;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x35) {
                        obj.P_GraphicsOffset = 0x03004264;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x3a) {
                        obj.P_GraphicsOffset = 0x03004F6C;
                        obj.P_PaletteIndex = 0x0000017F;
                    }
                    if (obj.P_GraphicsIndex == 0x3b) {
                        obj.P_GraphicsOffset = 0x03002370;
                        obj.P_PaletteIndex = 0x0000017D;
                    }
                    if (obj.P_GraphicsIndex == 0x3c) {
                        obj.P_GraphicsOffset = 0x030023CC;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x3d) {
                        obj.P_GraphicsOffset = 0x030023F0;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x37) {
                        obj.P_GraphicsOffset = 0x03002904;
                        obj.P_PaletteIndex = 0x192;
                    }
                    if (obj.P_GraphicsIndex == 0x38) {
                        obj.P_GraphicsOffset = 0x03004FF0;
                        obj.P_PaletteIndex = 0x0000017B;
                    }
                    if (obj.P_GraphicsIndex == 0x39) {
                        obj.P_GraphicsOffset = 0x03002908;
                        obj.P_Field14 = 0;
                        obj.P_PaletteIndex = 0x14e;
                    }
                    if (obj.P_GraphicsIndex == 0x31) {
                        obj.P_GraphicsOffset = 0x03005188;
                        obj.P_PaletteIndex = 0x2a8;
                    }
                    if (obj.P_GraphicsIndex == 0x3e) {
                        obj.P_GraphicsOffset = 0x03002300;
                        obj.P_PaletteIndex = 0x000002A6;
                    }
                    if (obj.P_GraphicsIndex == 0x3f) {
                        obj.P_GraphicsOffset = 0x03002E58;
                        obj.P_PaletteIndex = 0x0000029E;
                    }
                    #endregion
                    break;
                case 2:
                    // 2 is always a big thing. We'll check that later
                    #region World 2

                    if (obj.P_GraphicsIndex == 3) {
                        obj.P_FunctionPointer = 0x08037B9D; // Trigger (level completed)
                    }
                    if (obj.P_GraphicsIndex == 4) {
                        obj.P_FunctionPointer = 0x08037CA1; // Trigger (back to map)
                    }
                    if (obj.P_GraphicsIndex == 5) {
                        obj.P_GraphicsOffset = 0x03005158;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 6) {
                        obj.P_FunctionPointer = 0x0804DC65;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 0x18) {
                        // Boss
                        obj.P_GraphicsOffset = 828; // Hardcoded
                        obj.P_PaletteIndex = 0x0000033B;
                        obj.P_Field0E = 0;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        obj.P_SpriteWidth = 0xff;
                        //*(undefined4*)0x03005370 = 4;
                    }
                    if (obj.P_GraphicsIndex == 0x34) {
                        obj.P_GraphicsOffset = 0x03002324;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x35) {
                        obj.P_GraphicsOffset = 0x03004264;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 8) {
                        obj.P_GraphicsOffset = 0x0300401C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 9) {
                        obj.P_GraphicsOffset = 0x0300248C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 10) {
                        obj.P_GraphicsOffset = 0x03002374;
                        obj.P_PaletteIndex = 0x13a;
                    }
                    if (obj.P_GraphicsIndex == 0xb) {
                        obj.P_GraphicsOffset = 0x03005064;
                        obj.P_PaletteIndex = 0x13c;
                    }
                    if (obj.P_GraphicsIndex == 0x28) {
                        obj.P_GraphicsOffset = 0x03002ED0;
                        obj.P_PaletteIndex = 0x00000201;
                        obj.P_SpriteHeight = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x29) {
                        obj.P_GraphicsOffset = 0x030022E0;
                        obj.P_PaletteIndex = 0x00000207;
                        obj.P_SpriteHeight = 1;
                        obj.P_Field0E = 0x65;
                    }
                    if (obj.P_GraphicsIndex == 0x2a) {
                        obj.P_GraphicsOffset = 0x03004F94;
                        obj.P_PaletteIndex = 0x00000216;
                        obj.P_SpriteHeight = 2;
                    }
                    if (obj.P_GraphicsIndex == 0x2b) {
                        obj.P_GraphicsOffset = 0x030025CC;
                        obj.P_PaletteIndex = 0x210;
                        obj.P_SpriteHeight = 3;
                    }
                    if (obj.P_GraphicsIndex == 0x2c) {
                        obj.P_GraphicsOffset = 0x03002464;
                        obj.P_PaletteIndex = 0x21c;
                        obj.P_SpriteHeight = 4;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x30) {
                        obj.P_GraphicsOffset = 0x03005140;
                        obj.P_PaletteIndex = 0x00000177;
                    }
                    if (obj.P_GraphicsIndex == 0x36) {
                        obj.P_GraphicsOffset = 0x03004FE0;
                        obj.P_PaletteIndex = 0x00000179;
                    }
                    if (obj.P_GraphicsIndex == 0x3b) {
                        obj.P_GraphicsOffset = 0x0300292C;
                        obj.P_PaletteIndex = 0x000002B9;
                    }
                    if (obj.P_GraphicsIndex == 0x3c) {
                        obj.P_GraphicsOffset = 0x030024B8;
                        obj.P_PaletteIndex = 0x30c;
                    }
                    if (obj.P_GraphicsIndex == 0x3d) {
                        obj.P_GraphicsOffset = 0x03004F6C;
                        obj.P_PaletteIndex = 0x0000017F;
                    }
                    if (obj.P_GraphicsIndex == 0x3e) {
                        obj.P_GraphicsOffset = 0x03005220;
                        obj.P_PaletteIndex = 0x2c0;
                    }
                    if (obj.P_GraphicsIndex == 0x40) {
                        obj.P_GraphicsOffset = 0x030030B4;
                        obj.P_PaletteIndex = 0x000002C9;
                    }
                    if (obj.P_GraphicsIndex == 0x41) {
                        obj.P_GraphicsOffset = 0x03003088;
                        obj.P_PaletteIndex = 0x30c;
                    }
                    if (obj.P_GraphicsIndex == 0x42) {
                        obj.P_GraphicsOffset = 0x030025B8;
                        obj.P_PaletteIndex = 0x30c;
                    }
                    if (obj.P_GraphicsIndex == 0x31) {
                        obj.P_GraphicsOffset = 0x0300435C;
                        obj.P_PaletteIndex = 0x000002A2;
                    }
                    if (obj.P_GraphicsIndex == 0x32) {
                        obj.P_GraphicsOffset = 0x0300245C;
                        obj.P_PaletteIndex = 0x2a4;
                    }
                    if (obj.P_GraphicsIndex == 0x43) {
                        obj.P_GraphicsOffset = 0x030030C8;
                        obj.P_PaletteIndex = 0x000002A1;
                    }
                    if (obj.P_GraphicsIndex == 0x44) {
                        obj.P_GraphicsOffset = 0x03002e58;
                        obj.P_PaletteIndex = 0x0000029E;
                    }
                    #endregion
                    break;
                case 3:
                    // 2 is always a big thing. We'll check that later
                    #region World 3

                    if (obj.P_GraphicsIndex == 3) {
                        obj.P_FunctionPointer = 0x08037B9D;
                    }
                    if (obj.P_GraphicsIndex == 4) {
                        obj.P_FunctionPointer = 0x08037CA1;
                    }
                    if (obj.P_GraphicsIndex == 5) {
                        obj.P_GraphicsOffset = 0x03005158;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 6) {
                        obj.P_FunctionPointer = 0x0804DC65;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 8) {
                        obj.P_GraphicsOffset = 0x0300401C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 9) {
                        obj.P_GraphicsOffset = 0x0300248C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 10) {
                        obj.P_GraphicsOffset = 0x03002374;
                        obj.P_PaletteIndex = 0x13a;
                    }
                    if (obj.P_GraphicsIndex == 0xb) {
                        obj.P_GraphicsOffset = 0x03005064;
                        obj.P_PaletteIndex = 0x13c;
                    }
                    if (obj.P_GraphicsIndex == 0x18) {
                        // Boss
                        obj.P_GraphicsOffset = 828; // Hardcoded
                        obj.P_PaletteIndex = 0x0000033B;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        obj.P_SpriteHeight = 0;
                        //actor.P_Field0E = actor.RuntimeXPosition << 2;
                        obj.P_Field2E = (short)0x00000341;
                        obj.P_FrameCount = 0xb;
                        obj.P_RuntimeAnimFrame = 1;
                        obj.P_Field10 = 1;
                        obj.P_Field14 = 200;
                    }
                    if (obj.P_GraphicsIndex == 0x28) {
                        obj.P_GraphicsOffset = 0x03005190;
                        obj.P_PaletteIndex = 0x00000223;
                        obj.P_SpriteHeight = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x29) {
                        obj.P_GraphicsOffset = 0x030023B0;
                        obj.P_PaletteIndex = 0x00000246;
                        obj.P_SpriteHeight = 1;
                    }
                    if (obj.P_GraphicsIndex == 0x2a) {
                        obj.P_GraphicsOffset = 0x03002498;
                        obj.P_PaletteIndex = 0x230;
                        obj.P_SpriteHeight = 4;
                    }
                    if (obj.P_GraphicsIndex == 0x2b) {
                        obj.P_GraphicsOffset = 0x03003F74;
                        obj.P_PaletteIndex = 0x238;
                        obj.P_SpriteHeight = 2;
                    }
                    if (obj.P_GraphicsIndex == 0x2c) {
                        obj.P_GraphicsOffset = 0x030028E8;
                        obj.P_PaletteIndex = 0x240;
                        obj.P_SpriteHeight = 3;
                    }
                    if (obj.P_GraphicsIndex == 0x2d) {
                        obj.P_GraphicsOffset = 0x03002378;
                        obj.P_PaletteIndex = 0x1be;
                        obj.P_SpriteHeight = 5;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x38) {
                        obj.P_GraphicsOffset = 0x03004354;
                        obj.P_PaletteIndex = 0x00000271;
                    }
                    if (obj.P_GraphicsIndex == 0x39) {
                        obj.P_GraphicsOffset = 0x03003F54;
                        obj.P_PaletteIndex = 0x00000273;
                    }
                    if (obj.P_GraphicsIndex == 0x32) {
                        obj.P_GraphicsOffset = 0x03003FC8;
                        obj.P_PaletteIndex = 0x280;
                    }
                    if (obj.P_GraphicsIndex == 0x33) {
                        obj.P_GraphicsOffset = 0x03005200;
                        obj.P_PaletteIndex = 0x18e;
                    }
                    if (obj.P_GraphicsIndex == 0x34) {
                        obj.P_GraphicsOffset = 0x03002324;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x35) {
                        obj.P_GraphicsOffset = 0x03004264;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x36) {
                        obj.P_GraphicsOffset = 0x03002444;
                        obj.P_PaletteIndex = 0x152;
                    }
                    if (obj.P_GraphicsIndex == 0x37) {
                        obj.P_GraphicsOffset = 0x030050A0;
                        obj.P_PaletteIndex = 0x154;
                    }
                    if (obj.P_GraphicsIndex == 0x3b) {
                        obj.P_GraphicsOffset = 0x03003720;
                        obj.P_PaletteIndex = 0x15a;
                    }
                    if (obj.P_GraphicsIndex == 0x30) {
                        obj.P_GraphicsOffset = 0x0300523C;
                        obj.P_PaletteIndex = 0x18c;
                    }
                    if (obj.P_GraphicsIndex == 0x3c) {
                        obj.P_GraphicsOffset = 0x03002334;
                        obj.P_PaletteIndex = 0x0000026F;
                    }
                    if (obj.P_GraphicsIndex == 0x3d) {
                        obj.P_GraphicsOffset = 0x03004F6C;
                        obj.P_PaletteIndex = 0x0000017F;
                    }
                    if (obj.P_GraphicsIndex == 0x3e) {
                        obj.P_GraphicsOffset = 0x030042EC;
                        obj.P_PaletteIndex = 0x000002B1;
                    }
                    if (obj.P_GraphicsIndex == 0x40) {
                        obj.P_GraphicsOffset = 0x03004040;
                        obj.P_PaletteIndex = 0x000002C2;
                    }
                    if (obj.P_GraphicsIndex == 0x41) {
                        obj.P_GraphicsOffset = 0x030028DC;
                        obj.P_PaletteIndex = 0x000002CB;
                    }
                    if (obj.P_GraphicsIndex == 0x42) {
                        obj.P_GraphicsOffset = 0x03002ED4;
                        obj.P_PaletteIndex = 0x156;
                    }
                    if (obj.P_GraphicsIndex == 0x31) {
                        obj.P_GraphicsOffset = 0x03005188;
                        obj.P_PaletteIndex = 0x2a8;
                    }
                    if (obj.P_GraphicsIndex == 0x43) {
                        obj.P_GraphicsOffset = 0x03003F5C;
                        obj.P_PaletteIndex = 0x000002AA;
                    }
                    #endregion
                    break;
                case 4:
                    // 2 is always a big thing. We'll check that later
                    #region World 4

                    if (obj.P_GraphicsIndex == 3) {
                        obj.P_FunctionPointer = 0x08037B9D;
                    }
                    if (obj.P_GraphicsIndex == 4) {
                        obj.P_FunctionPointer = 0x08037CA1;
                    }
                    if (obj.P_GraphicsIndex == 5) {
                        obj.P_GraphicsOffset = 0x03005158;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 6) {
                        obj.P_FunctionPointer = 0x0804DC65;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 8) {
                        obj.P_GraphicsOffset = 0x0300401C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 9) {
                        obj.P_GraphicsOffset = 0x0300248C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 10) {
                        obj.P_GraphicsOffset = 0x03002374;
                        obj.P_PaletteIndex = 0x13a;
                    }
                    if (obj.P_GraphicsIndex == 0xb) {
                        obj.P_GraphicsOffset = 0x03005064;
                        obj.P_PaletteIndex = 0x13c;
                    }
                    if (obj.P_GraphicsIndex == 0x18) {
                        // Boss
                        obj.P_GraphicsOffset = 828; // Hardcoded
                        obj.P_PaletteIndex = 0x0000033B;
                        obj.P_Field0E = 0;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 0x28) {
                        if (level == 0) {
                            obj.P_GraphicsOffset = 0x030051B8;
                            obj.P_FrameCount = 0xd;
                            obj.P_PaletteIndex = 0x00000282;
                        } else {
                            if (level == 0x18) {
                                obj.P_Field34 = obj.P_Field34 | 0x200;
                            } else {
                                obj.P_GraphicsOffset = 0x03002ED0;
                                obj.P_PaletteIndex = 0x00000201;
                                obj.P_SpriteHeight = 4;
                            }
                        }
                    }
                    if (obj.P_GraphicsIndex == 0x29) {
                        obj.P_GraphicsOffset = 0x0300501C;
                        obj.P_PaletteIndex = 0x0000024D;
                        obj.P_SpriteHeight = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x2a) {
                        if (level == 0) {
                            obj.P_GraphicsOffset = 0x03004338;
                            obj.P_FrameCount = 0x1d;
                            obj.P_PaletteIndex = 0x288;
                        } else {
                            if (level == 0x18) {
                                obj.P_Field34 = obj.P_Field34 | 0x200;
                            } else {
                                obj.P_GraphicsOffset = 0x03002464;
                                obj.P_PaletteIndex = 0x21c;
                                obj.P_SpriteHeight = 2;
                            }
                        }
                    }
                    if (obj.P_GraphicsIndex == 0x2b) {
                        if (level == 0) {
                            obj.P_GraphicsOffset = 0x03004330;
                            obj.P_FrameCount = 6;
                            obj.P_PaletteIndex = 0x284;
                        } else {
                            if (level == 0x18) {
                                obj.P_Field34 = obj.P_Field34 | 0x200;
                            } else {
                                obj.P_GraphicsOffset = 0x03002DE0;
                                obj.P_PaletteIndex = 0x0000025B;
                                obj.P_SpriteHeight = 1;
                            }
                        }
                    }
                    if (obj.P_GraphicsIndex == 0x2c) {
                        if (level == 0) {
                            obj.P_GraphicsOffset = 0x03002580;
                            obj.P_FrameCount = 6;
                            obj.P_PaletteIndex = 0x00000286;
                        } else {
                            if (level == 0x18) {
                                obj.P_Field34 = obj.P_Field34 | 0x200;
                            } else {
                                obj.P_GraphicsOffset = 0x030024F0;
                                obj.P_PaletteIndex = 0x00000253;
                                obj.P_SpriteHeight = 3;
                            }
                        }
                    }
                    if (obj.P_GraphicsIndex == 0x2e) {
                        obj.P_GraphicsOffset = 0x03002F0C;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 0x00000195;
                        obj.P_Field12 = 6;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        //*(Actor**)0x03004278 = actor;
                    }
                    if (obj.P_GraphicsIndex == 0x30) {
                        obj.P_GraphicsOffset = 0x03002514;
                        obj.P_PaletteIndex = 0x00000175;
                    }
                    if (obj.P_GraphicsIndex == 0x31) {
                        switch (obj.Ushort_0E) {
                            case 1: obj.P_GraphicsOffset = 0x03005090; break;
                            case 2: obj.P_GraphicsOffset = 0x030042f4; break;
                            case 3: obj.P_GraphicsOffset = 0x03004034; break;
                            case 4: obj.P_GraphicsOffset = 0x030023a4; break;
                            case 5: obj.P_GraphicsOffset = 0x03002500; break;
                            case 0:
                            default: obj.P_GraphicsOffset = 0x03005114; break;
                        }
                        obj.P_PaletteIndex = 0x00000279;
                    }
                    if (obj.P_GraphicsIndex == 0x32) {
                        obj.P_GraphicsOffset = 0x03003FCC;
                        obj.P_PaletteIndex = 400;
                    }
                    if (obj.P_GraphicsIndex == 0x33) {
                        obj.P_GraphicsOffset = 0x03004F9C;
                        obj.P_PaletteIndex = 0x00000262;
                        if (obj.P_Field12 == 0x96) {
                            if (level == 0) {
                                obj.P_Field34 = obj.P_Field34 | 0x200;
                            } else {
                                obj.P_FrameCount = 1;
                            }
                        }
                    }
                    if (obj.P_GraphicsIndex == 0x34) {
                        obj.P_GraphicsOffset = 0x03002324;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x35) {
                        obj.P_GraphicsOffset = 0x03004264;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x36) {
                        obj.P_GraphicsOffset = 0x0300439C;
                        obj.P_PaletteIndex = 0x000002DD;
                    }
                    if (obj.P_GraphicsIndex == 0x37) {
                        obj.P_GraphicsOffset = 0x030050B0;
                        obj.P_PaletteIndex = 0x000002DD;
                    }
                    if (obj.P_GraphicsIndex == 0x38) {
                        obj.P_GraphicsOffset = 0x03004340;
                        obj.P_OtherGraphicsOffset = 0x03004010;
                        obj.P_PaletteIndex = 0x2ac;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x39) {
                        obj.P_GraphicsOffset = 0x030025AC;
                        obj.P_PaletteIndex = 0x000002AF;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x3a) {
                        obj.P_GraphicsOffset = 0x03005080;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 0x3b) {
                        obj.P_GraphicsOffset = 0x0300292C;
                        obj.P_PaletteIndex = 0x2b4;
                    }
                    if (obj.P_GraphicsIndex == 0x3c) {
                        obj.P_GraphicsOffset = 0x030022B8;
                        obj.P_OtherGraphicsOffset = 0x0300234C;
                        obj.P_Field14 = 0x03004020;
                        obj.P_Field0E = 4;
                        obj.P_Field10 = 0x40;
                        obj.P_Field20 = 100;
                        obj.P_PaletteIndex = 0x148;
                    }
                    if (obj.P_GraphicsIndex == 0x3d) {
                        obj.P_GraphicsOffset = 0x03002E24;
                        obj.P_PaletteIndex = 4;
                    }
                    if (obj.P_GraphicsIndex == 0x3e) {
                        obj.P_GraphicsOffset = 0x03002544;
                        obj.P_PaletteIndex = 0x260;
                    }
                    if (obj.P_GraphicsIndex == 0x3f) {
                        obj.P_GraphicsOffset = 0x030023B4;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x40) {
                        obj.P_GraphicsOffset = 0x030050EC;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x41) {
                        obj.P_GraphicsOffset = 0x03004308;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x42) {
                        obj.P_GraphicsOffset = 0x0300403C;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x44) {
                        obj.P_GraphicsOffset = 0x03004390;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x45) {
                        obj.P_GraphicsOffset = 0x03004FFC;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x27) {
                        // Boss Prison
                        obj.P_GraphicsOffset = 812; // Hardcoded
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        obj.P_FrameCount = 5;
                        obj.P_Field2E = 0x32c;
                        obj.P_PaletteIndex = 0x328;
                        obj.P_Field34 = obj.P_Field34 | 1;
                        //*(undefined4*)0x03005370 = 3;
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x47) {
                        obj.P_GraphicsOffset = 0x03005088;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 0x48) {
                        obj.P_GraphicsOffset = 0x03005228;
                        obj.P_PaletteIndex = 0x000002BE;
                    }
                    if (obj.P_GraphicsIndex == 0x4c) {
                        obj.P_GraphicsOffset = 0x0300516C;
                        obj.P_PaletteIndex = 0x338;
                        obj.P_Field12 = (uint)obj.RuntimeXPosition;
                        obj.P_Field0E = (uint)obj.RuntimeYPosition;
                    }
                    if (obj.P_GraphicsIndex == 0x4d) {
                        obj.P_GraphicsOffset = 0x03002534;
                        obj.P_PaletteIndex = 0x338;
                        obj.P_Field12 = (uint)obj.RuntimeXPosition;
                        obj.P_Field0E = (uint)obj.RuntimeYPosition;
                    }
                    if (obj.P_GraphicsIndex == 0x4e) {
                        obj.P_GraphicsOffset = 0x03003728;
                        obj.P_PaletteIndex = 0x00000262;
                    }
                    if (obj.P_GraphicsIndex == 0x4f) {
                        obj.P_GraphicsOffset = 0x03004F6C;
                        obj.P_PaletteIndex = 0x0000017F;
                    }
                    if (obj.P_GraphicsIndex == 0x4a) {
                        obj.P_GraphicsOffset = 0x0300232C;
                        obj.P_PaletteIndex = 0x293;
                    }
                    if (obj.P_GraphicsIndex == 0x4b) {
                        obj.P_GraphicsOffset = 0x03002E58;
                        obj.P_PaletteIndex = 0x0000029E;
                    }
                    #endregion
                    break;
                case 5:
                    // 2 is always a big thing. We'll check that later
                    #region World 5

                    if (obj.P_GraphicsIndex == 3) {
                        obj.P_FunctionPointer = 0x08037b9d;
                    }
                    if (obj.P_GraphicsIndex == 4) {
                        obj.P_FunctionPointer = 0x08037ca1;
                    }
                    if (obj.P_GraphicsIndex == 5) {
                        obj.P_GraphicsOffset = 0x03005158;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 6) {
                        obj.P_FunctionPointer = 0x0804dc65;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 8) {
                        obj.P_GraphicsOffset = 0x0300401C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 9) {
                        obj.P_GraphicsOffset = 0x0300248C;
                        obj.P_PaletteIndex = 2;
                    }
                    if (obj.P_GraphicsIndex == 10) {
                        obj.P_GraphicsOffset = 0x03002374;
                        obj.P_PaletteIndex = 0x13a;
                    }
                    if (obj.P_GraphicsIndex == 0xb) {
                        obj.P_GraphicsOffset = 0x03005064;
                        obj.P_PaletteIndex = 0x13c;
                    }
                    if (obj.P_GraphicsIndex == 0x10) {
                        obj.P_GraphicsOffset = 0x030028B4;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x11) {
                        obj.P_GraphicsOffset = 0x03002DEC;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x12) {
                        obj.P_GraphicsOffset = 0x03002E5C;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x13) {
                        obj.P_GraphicsOffset = 0x03004FC8;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x28) {
                        obj.P_GraphicsOffset = 0x030024E8;
                        obj.P_PaletteIndex = 0x0000036A;
                    }
                    if (obj.P_GraphicsIndex == 0x29) {
                        obj.P_GraphicsOffset = 0x030022F0;
                        obj.P_PaletteIndex = 0x0000036A;
                    }
                    if (obj.P_GraphicsIndex == 0x2a) {
                        obj.P_GraphicsOffset = 0x03002528;
                        obj.P_PaletteIndex = 0x0000036A;
                    }
                    if (obj.P_GraphicsIndex == 0x2b) {
                        obj.P_GraphicsOffset = 0x03003F7C;
                        obj.P_PaletteIndex = 0x0000036A;
                    }
                    if (obj.P_GraphicsIndex == 0x2c) {
                        obj.P_GraphicsOffset = 0x03002EF0;
                        obj.P_PaletteIndex = 0x00000372;
                    }
                    if (obj.P_GraphicsIndex == 0x30) {
                        obj.P_GraphicsOffset = 0x03004270;
                        obj.P_PaletteIndex = 0x0000017B;
                    }
                    if (obj.P_GraphicsIndex == 0x2e) {
                        obj.P_GraphicsOffset = 0x03002F0C;
                        obj.P_FrameCount = 10;
                        obj.P_PaletteIndex = 0x19c;
                        obj.P_Field12 = 6;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                        //0x03004278 = actor;
                    }
                    if (obj.P_GraphicsIndex == 0x34) {
                        obj.P_GraphicsOffset = 0x03002324;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x35) {
                        obj.P_GraphicsOffset = 0x03004264;
                        obj.P_FrameCount = 8;
                        obj.P_PaletteIndex = 2;
                        obj.P_Field12 = 0;
                    }
                    if (obj.P_GraphicsIndex == 0x3a) {
                        obj.P_GraphicsOffset = 0x03004F6C;
                        obj.P_PaletteIndex = 0x0000017F;
                    }
                    if (obj.P_GraphicsIndex == 0x3b) {
                        obj.P_GraphicsOffset = 0x03002370;
                        obj.P_PaletteIndex = 0x0000017D;
                    }
                    if (obj.P_GraphicsIndex == 0x3c) {
                        obj.P_GraphicsOffset = 0x030023CC;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x3d) {
                        obj.P_GraphicsOffset = 0x030023F0;
                        obj.P_PaletteIndex = 0x264;
                    }
                    if (obj.P_GraphicsIndex == 0x38) {
                        obj.P_GraphicsOffset = 0x03004FF0;
                        obj.P_PaletteIndex = 0x0000017B;
                    }
                    if (obj.P_GraphicsIndex == 0x39) {
                        obj.P_GraphicsOffset = 0x03002908;
                        obj.P_Field14 = 0;
                        obj.P_PaletteIndex = 0x14e;
                    }
                    if (obj.P_GraphicsIndex == 0x3e) {
                        obj.P_GraphicsOffset = 0x03004248;
                        obj.P_PaletteIndex = 0x00000275;
                    }
                    if (obj.P_GraphicsIndex == 0xf) {
                        obj.P_GraphicsOffset = 0x030022F4;
                        obj.P_PaletteIndex = 0x00000277;
                        /*iVar12 = GetSomeIndex(0x16);
                        if (iVar12 != 0) {
                            actor.P_AnimIndex = 1;
                        }*/
                    }
                    obj.P_GraphicsIndex = obj.P_GraphicsIndex;
                    if (obj.P_GraphicsIndex == 0x37) {
                        obj.P_GraphicsOffset = 0x0300518C;
                        obj.P_PaletteIndex = 0x15c;
                    }
                    if (obj.P_GraphicsIndex == 0x3f) {
                        obj.P_GraphicsOffset = 0x03002E94;
                        obj.P_PaletteIndex = 0x15e;
                    }
                    if (obj.P_GraphicsIndex == 0x40) {
                        obj.P_GraphicsOffset = 0x03005100;
                        obj.P_PaletteIndex = 0x160;
                    }
                    if (obj.P_GraphicsIndex == 0x27) {
                        obj.P_GraphicsOffset = 0x03002EB4;
                        obj.P_PaletteIndex = 0x0000016F;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 0x36) {
                        obj.P_GraphicsOffset = 0x03002F1C;
                        obj.P_PaletteIndex = 0x0000016F;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    if (obj.P_GraphicsIndex == 0x41) {
                        obj.P_GraphicsOffset = 0x03004260;
                        obj.P_PaletteIndex = 0x0000016F;
                        obj.P_Field34 = obj.P_Field34 | 0x200;
                    }
                    #endregion
                    break;
            }

            // Ly
            if (obj.P_GraphicsIndex == 0x2e && world < 5 && (level == 1 || level == 6 || level == 0xb || level == 0x10 || level == 0x16 || level == 0x1b)) {
                obj.P_GraphicsOffset = 0x03002F0C;
                obj.P_FrameCount = 10;
                obj.P_PaletteIndex = 0x19c;
                obj.P_Field12 = 6;
                obj.P_Field34 = obj.P_Field34 | 0x200;
                //0x03004278 = actor;
            }
            // Rayman
            if (obj.P_GraphicsIndex == 2) {
                obj.P_GraphicsOffset = 148;
                obj.P_PaletteIndex = 144;
                if (level == 0) {
                    obj.P_GraphicsOffset = 226;
                }
            }
        }

        protected int GetFGPalette(int level)
        {
            switch (level)
            {
                case 12:
                    return 13;

                default:
                    return 14;
            }
        }
        protected int GetBG0Palette(int level)
        {
            switch (level)
            {
                case 21:
                case 30:
                    return 13;

                default:
                    return 15;
            }
        }
        protected int GetBG1Palette(int level)
        {
            switch (level)
            {
                case 30:
                    return 15;

                case 0:
                case 1:
                case 24:
                case 25:
                case 26:
                case 27:
                    return 12;

                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 28:
                    return 14;
                
                default:
                    return 13;
            }
        }

        protected bool IsForeground(int world, int level) => !(world == 3 || level == 30 || level == 27);

        public Size GetMenuSize(int level)
        {
            switch (level)
            {
                case 0:
                    return new Size(32, 8);

                default:
                    return new Size(30, 20);
            }
        }
        protected int? GetMenuPalIndex(int level)
        {
            switch (level)
            {
                case 0:
                    return 15;
                
                case 4:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 14:
                    return 0;

                default:
                    return null;
            }
        }

        public int[] GetMenuLevels(int index)
        {
            switch (index)
            {
                case 0:
                    return new int[] {0, 1, 4}; // Menu

                case 1:
                    return new int[] {0, 2, 4}; // Credits

                case 2:
                    return new int[] {0, 3, 4}; // Options

                default:
                    return new int[] { index + 2 };
            }
        }

        public bool IsMenuCompressed(int level) => !(level == 5 || level == 6 || level == 13);

        public bool HasMenuAlphaBlending(int level) => level == 4;

        public class Size
        {
            public Size(ushort width, ushort height)
            {
                Width = width;
                Height = height;
            }

            public ushort Width { get; }
            public ushort Height { get; }
        }

        public AnimTileInfo[] GetBG0AnimTileInfo(int level, BaseColor[][] animtedPalettes)
        {
            switch (level)
            {
                case 16:
                    return new AnimTileInfo[]
                    {
                        new AnimTileInfo(animtedPalettes[4], 5, palCount: 15), 
                    };

                default:
                    return null;
            }
        }
        public AnimTileInfo[] GetFGAnimTileInfo(int level, BaseColor[][] animtedPalettes)
        {
            switch (level)
            {
                // Dark
                case 0:
                case 1:
                case 24:
                case 25:
                    return new AnimTileInfo[]
                    {
                        new AnimTileInfo(animtedPalettes[0], 4, 13), // Fire
                        new AnimTileInfo(animtedPalettes[1], 2, 14), // Green toxic waste
                    };
                case 23:
                case 26:
                    return new AnimTileInfo[]
                    {
                        new AnimTileInfo(animtedPalettes[3], 8, 13), // Blue light
                    };

                // Organic cave
                case 13:
                case 14:
                case 15:
                case 16:
                    return new AnimTileInfo[]
                    {
                        new AnimTileInfo(animtedPalettes[2], 5, 13), // Pink light on walls
                    };

                default:
                    return null;
            }
        }

        public class AnimTileInfo
        {
            public AnimTileInfo(BaseColor[] animatedPalette, int animSpeed, int tilePalIndex = 0, int palCount = 16)
            {
                AnimatedPalette = animatedPalette;
                AnimSpeed = animSpeed;
                TilePalIndex = tilePalIndex;
                PalCount = palCount;
            }

            public BaseColor[] AnimatedPalette { get; }
            public int AnimSpeed { get; }
            public int TilePalIndex { get; }
            public int PalCount { get; }
        }

        public class AnimationAssemble {
            public AnimationAssemble(ushort width, ushort height, AssembleOrder order = AssembleOrder.Row) {
                Width = width;
                Height = height;
                Order = order;
            }

            public ushort Width { get; }
            public ushort Height { get; }
            public AssembleOrder Order { get; }
            public enum AssembleOrder { Column, Row }
        }

        public class Mode7VRAMEntry {
            public uint Address { get; set; }
            public bool IsPerFrame { get; set; }
            public byte[] ImageData { get; set; }
            public byte[][] PerFrameImageData { get; set; }
        }

        public class Mode7VRAMConfiguration {
            public Mode7VRAMEntry[] VRAM { get; set; }
            public RGBA5551Color[] Palette { get; set; }
        }

        public class Mode7Data {
            public Dictionary<Mode7Config, Mode7VRAMConfiguration> Configurations { get; set; }
            public ushort[][] FrameIndices { get; set; }
            public GBARRR_Mode7AnimSet[] AnimSets { get; set; }



            public Mode7Config GetVRAMConfig(int animIndex) {
                Mode7Config config = Mode7Config.Mode7;
                if (AnimSets[animIndex] == AnimSets[0] || AnimSets[animIndex] == AnimSets[1] || AnimSets[animIndex] == AnimSets[3] ||
                    AnimSets[animIndex] == AnimSets[12] || AnimSets[animIndex] == AnimSets[13]) {
                    config = Mode7Config.Mode7;
                } else if (AnimSets[animIndex] == AnimSets[19]) {
                    config = Mode7Config.MainMenu;
                } else if (AnimSets[animIndex] == AnimSets[40] || AnimSets[animIndex] == AnimSets[41]) {
                    config = Mode7Config.GameOver;
                } else if (AnimSets[animIndex] == AnimSets[43] || AnimSets[animIndex] == AnimSets[44]) {
                    config = Mode7Config.PauseMenu;
                } else if (AnimSets[animIndex] == AnimSets[45]) {
                    config = Mode7Config.Unk;
                }
                return config;
            }
        }
        public enum Mode7Config {
            Mode7,
            MainMenu,
            PauseMenu,
            GameOver,
            Unk
        }

        public Dictionary<int, AnimationAssemble> FixedSpriteSizes => new Dictionary<int, AnimationAssemble>() {
            [18]  = new AnimationAssemble(3, 1),
            [331] = new AnimationAssemble(1, 2),
            [711] = new AnimationAssemble(1, 3),
            [712] = new AnimationAssemble(1, 3),
            [714] = new AnimationAssemble(1, 3),
            [716] = new AnimationAssemble(1, 3),
            [749] = new AnimationAssemble(2, 1),
            [753] = new AnimationAssemble(2, 1),

            // Boss Prison
            [809] = new AnimationAssemble(5, 5),
            [810] = new AnimationAssemble(5, 5),
            [811] = new AnimationAssemble(5, 5),
            [812] = new AnimationAssemble(5, 5),
            [813] = new AnimationAssemble(5, 5),
            [814] = new AnimationAssemble(5, 5),
            [815] = new AnimationAssemble(5, 5),
            [816] = new AnimationAssemble(5, 5),
            [817] = new AnimationAssemble(5, 5),
            [818] = new AnimationAssemble(5, 5),
            [819] = new AnimationAssemble(5, 5),
            [820] = new AnimationAssemble(5, 5),
            [821] = new AnimationAssemble(5, 5),
            [822] = new AnimationAssemble(5, 5),
            [823] = new AnimationAssemble(5, 5),

            // Bunny boss
            [828] = new AnimationAssemble(5, 5),
            [829] = new AnimationAssemble(5, 5),
            [830] = new AnimationAssemble(5, 5),
            [831] = new AnimationAssemble(5, 5),
            [832] = new AnimationAssemble(5, 5),
            [833] = new AnimationAssemble(5, 5),
            [834] = new AnimationAssemble(5, 5),
            [835] = new AnimationAssemble(5, 5),
            [836] = new AnimationAssemble(5, 5),
            [837] = new AnimationAssemble(5, 5),
            [838] = new AnimationAssemble(5, 5),
            [839] = new AnimationAssemble(5, 5),
            [840] = new AnimationAssemble(5, 5),
            [841] = new AnimationAssemble(5, 5),
            [842] = new AnimationAssemble(5, 5),
            [843] = new AnimationAssemble(5, 5),
            [844] = new AnimationAssemble(5, 5),
            [845] = new AnimationAssemble(5, 5),
        };
		#endregion
	}
}