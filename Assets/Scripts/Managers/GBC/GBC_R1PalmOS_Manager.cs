using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_R1PalmOS_Manager : IGameManager
    {
        public const int CellSize = 8;
        public const string GlobalOffsetTableKey = "GlobalOffsetTable";

        public string AllfixFilePath => "worldmap.pdb";
        public string[] AllDataPaths { get; } = new string[] {
            "palmcolormenu",
            "worldmap",
            "jungle1",
            "jungle2",
            "jungle3",
            "jungle4",
            "music",
            "mount",
            "cave",
            "dark",

            // Other data files. Don't load automatically if loading is slow
            "IDRegister",
            "IDUnlock",
            "save",
            "save1"
        };

        public static ARGBColor[] GetPalmOS8BitPalette() {
            ARGBColor[] pal = new ARGBColor[256];
            int palIndex = 0;

            for (int r = 0; r < 6; r++) {
                for (int b = 0; b < 3; b++) {
                    for (int g = 0; g < 6; g++) {
                        pal[palIndex++] = new ARGBColor(
                            (byte)(0xFF - r * 0x33),
                            (byte)(0xFF - g * 0x33),
                            (byte)(0xFF - b * 0x33));
                    }
                }
            }
            for (int r = 0; r < 6; r++) {
                for (int b = 0; b < 3; b++) {
                    for (int g = 0; g < 6; g++) {
                        pal[palIndex++] = new ARGBColor(
                            (byte)(0xFF - r * 0x33),
                            (byte)(0xFF - g * 0x33),
                            (byte)(0xFF - (b+3) * 0x33));
                    }
                }
            }
            for (int i = 2; i < 16; i++) {
                if(i % 3 == 0) continue;
                byte b = (byte)(0x11 * i);
                pal[palIndex++] = new ARGBColor(b,b,b);
            }
            pal[palIndex++] = new ARGBColor(0xC0, 0xC0, 0xC0);
            pal[palIndex++] = new ARGBColor(0x80, 0x00, 0x00);
            pal[palIndex++] = new ARGBColor(0x80, 0x00, 0x80);
            pal[palIndex++] = new ARGBColor(0x00, 0x80, 0x00);
            pal[palIndex++] = new ARGBColor(0x00, 0x80, 0x80);
            for (int i = palIndex; i < 256; i++) {
                pal[palIndex++] = new ARGBColor(0,0,0);
            }
            return pal;
        }

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 47).ToArray()), 
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Log Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, false)),
            //new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, true)),
            new GameAction("Export Data", false, true, (input, output) => ExportDataAsync(settings, output, false)),
            new GameAction("Export Data (Categorized)", false, true, (input, output) => ExportDataAsync(settings, output, true)),
            new GameAction("Export TileSets", false, true, (input, output) => ExportDataAsync(settings, output, false, asTileSet: true)),
        };

        public async UniTask ExportDataAsync(GameSettings settings, string outputDir, bool categorized, bool asTileSet = false)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                foreach (var filePath in Directory.GetFiles(context.BasePath, "*", SearchOption.TopDirectoryOnly))
                {
                    var ext = Path.GetExtension(filePath);
                    var type = ext == ".pdb" ? Palm_Database.DatabaseType.PDB : (ext == ".prc" ? Palm_Database.DatabaseType.PRC : (Palm_Database.DatabaseType?)null);

                    if (type == null)
                        continue;

                    var relPath = Path.GetFileName(filePath);
                    await context.AddLinearSerializedFileAsync(relPath, BinaryFile.Endian.Big);

                    var dataBase = FileFactory.Read<Palm_Database>(relPath, context, (so, pd) => pd.Type = type.Value);

                    var palette8Bit = GetPalmOS8BitPalette();
                    var palette4Bit = Util.CreateDummyPalette(16, firstTransparent: false);

                    for (int i = 0; i < dataBase.RecordsCount; i++)
                    {
                        var record = dataBase.Records[i];
                        var name = type == Palm_Database.DatabaseType.PRC ? $"{record.Name}_{record.ID}" : $"{i}";
                        bool exported = false;
                        if (categorized && filePath.Contains("palmcolormenu")) 
                        {
                            if (!exported) {
                                try {
                                    GBC_PalmOS_CompressedBlock<GBC_PalmOS_Vignette> vignette = null;
                                    s.DoAt(record.DataPointer, () => {
                                        vignette = s.SerializeObject<GBC_PalmOS_CompressedBlock<GBC_PalmOS_Vignette>>(default, name: nameof(vignette));
                                    });
                                    if (vignette == null) throw new Exception("Not a vignette");
                                    if (vignette.Value.Width > 0x1000 || vignette.Value.Height > 0x1000 || vignette.Value.Width == 0 || vignette.Value.Height == 0) {
                                        throw new Exception("Not a vignette");
                                    }
                                    int w = (int)vignette.Value.Width;
                                    int h = (int)vignette.Value.Height;
                                    Texture2D tex = TextureHelpers.CreateTexture2D(w, h);
                                    for (int y = 0; y < h; y++) {
                                        for (int x = 0; x < w; x++) {
                                            int ind = y * w + x;
                                            if (vignette.Value.BitDepth == 8) {
                                                int col = vignette.Value.Data[ind];
                                                tex.SetPixel(x, h - 1 - y, palette8Bit[col].GetColor());
                                            } else {
                                                int col = vignette.Value.Data[ind / 2];
                                                if (ind % 2 == 0) {
                                                    col = BitHelpers.ExtractBits(col, 4, 0);
                                                } else {
                                                    col = BitHelpers.ExtractBits(col, 4, 4);
                                                }
                                                tex.SetPixel(x, h - 1 - y, palette4Bit[col].GetColor());
                                            }
                                        }
                                    }
                                    tex.Apply();
                                    exported = true;
                                    string filename = $"Vignette/{name}.png";
                                    Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), tex.EncodeToPNG());
                                } catch (Exception ex) { }
                            }
                            if (!exported) {
                                try {
                                    GBC_PalmOS_UncompressedBlock<GBC_PalmOS_Vignette> vignette = null;
                                    s.DoAt(record.DataPointer, () => {
                                        vignette = s.SerializeObject<GBC_PalmOS_UncompressedBlock<GBC_PalmOS_Vignette>>(default, name: nameof(vignette));
                                    });
                                    if (vignette == null) throw new Exception("Not a vignette");
                                    if (vignette.Value.Width > 0x1000 || vignette.Value.Height > 0x1000 || vignette.Value.Width == 0 || vignette.Value.Height == 0) {
                                        throw new Exception("Not a vignette");
                                    }
                                    int w = (int)vignette.Value.Width;
                                    int h = (int)vignette.Value.Height;
                                    Texture2D tex = TextureHelpers.CreateTexture2D(w, h);
                                    for (int y = 0; y < h; y++) {
                                        for (int x = 0; x < w; x++) {
                                            int ind = y * w + x;
                                            if (vignette.Value.BitDepth == 8) {
                                                int col = vignette.Value.Data[ind];
                                                tex.SetPixel(x, h - 1 - y, palette8Bit[col].GetColor());
                                            } else {
                                                int col = vignette.Value.Data[ind / 2];
                                                if (ind % 2 == 0) {
                                                    col = BitHelpers.ExtractBits(col, 4, 0);
                                                } else {
                                                    col = BitHelpers.ExtractBits(col, 4, 4);
                                                }
                                                tex.SetPixel(x, h - 1 - y, palette4Bit[col].GetColor());
                                            }
                                        }
                                    }
                                    tex.Apply();
                                    exported = true;
                                    string filename = $"Vignette/{name}.png";
                                    Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), tex.EncodeToPNG());
                                } catch (Exception ex) { }
                            }
                            if (!exported) {
                                s.Goto(record.DataPointer);
                                var bytes = s.DoAt(record.DataPointer, () => s.SerializeArray<byte>(default, record.Length, name: $"Record[{i}]"));
                                string filename = $"Uncategorized/{name}.bin";
                                Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), bytes);
                            }
                        } 
                        else if (asTileSet)
                        {
                            if (record.Length <= 12)
                                continue;

                            var isValid = s.DoAt(record.DataPointer, () =>
                            {
                                s.Serialize<uint>(default); // Header
                                var unk = s.Serialize<uint>(default); // ?
                                var count = s.Serialize<uint>(default); // Count

                                return count * 0x40 + 12 == record.Length;
                            });

                            if (!isValid)
                                continue;

                            var tileSet = s.DoAt(record.DataPointer, () => s.SerializeObject<GBC_PalmOS_UncompressedBlock<GBC_TileKit>>(default, name: "TileSet"));

                            var tex = Util.ToTileSetTexture(tileSet.Value.TileData, palette8Bit.Select(x => x.GetColor()).ToArray(), true, 8, true, wrap: 16);
                            
                            Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), $"{name}.png"), tex.EncodeToPNG());
                        }
                        else
                        {
                            string filename = $"{name}.bin";
                            var bytes = s.DoAt(record.DataPointer, () => s.SerializeArray<byte>(default, record.Length, name: $"Record[{i}]"));
                            Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), bytes);
                        }
                    }
                }
            }
        }

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputDir, bool export)
        {
            using (var context = new Context(settings))
            {
                // Init global offset table
                await InitGlobalOffsetTable(context);

                // Get the deserializer
                var s = context.Deserializer;

                var references = new Dictionary<Pointer, HashSet<Pointer>>();

                using (var logFile = File.Create(Path.Combine(outputDir, "GBC_Blocks_Log-Map.txt")))
                {
                    using (var writer = new StreamWriter(logFile))
                    {
                        // Load the ROM
                        await LoadFilesAsync(context);

                        // Load the data block
                        var dataFile = FileFactory.Read<PalmOS_DataFile>(AllfixFilePath, context);

                        var indentLevel = 0;
                        GBC_DummyBlock[] blocks = new GBC_DummyBlock[dataFile.OffsetTable.NumEntries];

                        var blockIndex = 0;
                        foreach (var fileKey in dataFile.OffsetTable.EntriesDictionary.Keys)
                        {
                            try
                            {
                                blocks[blockIndex] = s.DoAt(dataFile.Resolve(fileKey), () => s.SerializeObject<GBC_DummyBlock>(blocks[blockIndex], name: $"{nameof(blocks)}[{blockIndex}]"));
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                            blockIndex++;
                        }

                        void ExportBlocks(GBC_DummyBlock block, int index, string path)
                        {
                            indentLevel++;

                            if (export)
                            {
                                //Util.ByteArrayToFile(outputDir + "/blocks/" + path + "/" + block.Offset.StringFileOffset + ".bin", block.Data);
                            }

                            //writer.WriteLine($"{block.Offset}:{new string(' ', indentLevel * 2)}[{index}] Offsets: {block.OffsetTable.OffsetsCount} - BlockSize: {block.BlockSize}");
                            writer.WriteLine($"{block.Offset}:{new string(' ', indentLevel * 2)}[{index}] Offsets: {block.OffsetTable.OffsetsCount}");

                            // Handle every block offset in the table
                            for (int i = 0; i < block.SubBlocks.Length; i++)
                            {
                                if (block.SubBlocks[i] == null)
                                    continue;

                                if (!references.ContainsKey(block.SubBlocks[i].Offset))
                                    references[block.SubBlocks[i].Offset] = new HashSet<Pointer>();

                                references[block.SubBlocks[i].Offset].Add(block.Offset);

                                // Export
                                ExportBlocks(block.SubBlocks[i], i, path + "/" + (i + " - " + block.SubBlocks[i].Offset.StringFileOffset));
                            }

                            indentLevel--;
                        }

                        for (int i = 0; i < blocks.Length; i++)
                        {
                            await UniTask.WaitForEndOfFrame();
                            ExportBlocks(blocks[i], i, (i + " - " + blocks[i].Offset.StringFileOffset));
                        }
                    }
                }

                // Log references
                using (var logFile = File.Create(Path.Combine(outputDir, "GBC_Blocks_Log-References.txt")))
                {
                    using (var writer = new StreamWriter(logFile))
                    {
                        foreach (var r in references.OrderBy(x => x.Key))
                        {
                            writer.WriteLine($"{r.Key,-30}: {String.Join(", ", r.Value.Select(x => $"{x.AbsoluteOffset:X8}"))}");
                        }
                    }
                }
            }

            Debug.Log("Finished logging blocks");
        }

        public async UniTask InitGlobalOffsetTable(Context context) {
            GBC_GlobalOffsetTable globalOffsetTable = new GBC_GlobalOffsetTable();
            List<GBC_BaseDataFile> dataFiles = new List<GBC_BaseDataFile>();
            foreach (var path in AllDataPaths) {
                var fullPath = $"{path}.pdb";
                await context.AddLinearSerializedFileAsync(fullPath, BinaryFile.Endian.Big);
                dataFiles.Add(FileFactory.Read<PalmOS_DataFile>(fullPath, context));
            }
            globalOffsetTable.Files = dataFiles.ToArray();
            context.StoreObject<GBC_GlobalOffsetTable>(GlobalOffsetTableKey, globalOffsetTable);
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var pal = GetPalmOS8BitPalette().Select(x => x.GetColor()).ToArray();
            
            // Init global offset table
            await InitGlobalOffsetTable(context);

            var s = context.Deserializer;
            var allfix = FileFactory.Read<PalmOS_DataFile>(AllfixFilePath, context);

            var manifest = s.DoAt(allfix.Resolve(1), () => s.SerializeObject<GBC_SceneManifest>(default, name: "SceneManfiest"));

            // Log unused data blocks in offset tables
            var notParsedBlocks = GBC_OffsetTable.OffsetTables.Where(x => x.UsedOffsets.Any(y => !y)).ToArray();
            if (notParsedBlocks.Any())
                Debug.Log($"The following blocks were never parsed:{Environment.NewLine}" + String.Join(Environment.NewLine, notParsedBlocks.Select(y => $"[{y.Offset}]:" + String.Join(", ", y.UsedOffsets.Select((o, i) => new
                {
                    Obj = o,
                    Index = i
                }).Where(o => !o.Obj).Select(o => o.Index.ToString())))));

            var playField = manifest.SceneList.Scene.PlayField;
            var map = playField.Map;

            var tileSetTex = Util.ToTileSetTexture(map.TileKit.TileData, pal, true, CellSize, flipY: false);

            var maps = new Unity_Map[]
            {
                new Unity_Map
                {
                    Width = (ushort)map.Width,
                    Height = (ushort)map.Height,
                    TileSet = new Unity_MapTileMap[]
                    {
                        new Unity_MapTileMap(tileSetTex, CellSize), 
                    },
                    MapTiles = map.MapTiles.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                }
            };

            return new Unity_Level(
                maps: maps, 
                objManager: new Unity_ObjectManager(context),
                cellSize: CellSize,
                getCollisionTypeGraphicFunc: x => ((GBC_TileCollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBC_TileCollisionType)x).ToString());
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public UniTask LoadFilesAsync(Context context) => UniTask.CompletedTask;
    }
}