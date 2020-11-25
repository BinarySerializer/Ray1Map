using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_R1PalmOS_Manager : GBC_BaseManager
    {
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

        public ARGBColor[] GetPalmOS8BitPalette() {
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

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 47).ToArray()), 
        });

        public override GameAction[] GetGameActions(GameSettings settings) => base.GetGameActions(settings).Concat(new GameAction[]
        {
            new GameAction("Export DataBases", false, true, (input, output) => ExportDataBasesAsync(settings, output, false)),
            new GameAction("Export DataBases (Categorized)", false, true, (input, output) => ExportDataBasesAsync(settings, output, true)),
            new GameAction("Export TileSets", false, true, (input, output) => ExportDataBasesAsync(settings, output, false, asTileSet: true)),
        }).ToArray();

        public async UniTask ExportDataBasesAsync(GameSettings settings, string outputDir, bool categorized, bool asTileSet = false)
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

        public override async UniTask<GBC_SceneList> GetSceneListAsync(Context context)
        {
            // Init global offset table
            await InitGlobalOffsetTable(context);

            var allfix = FileFactory.Read<PalmOS_DataFile>(AllfixFilePath, context);
            var s = context.Deserializer;
            return s.DoAt(allfix.Resolve(1), () => s.SerializeObject<GBC_SceneManifest>(default, name: "SceneManfiest")).SceneList;
        }
        public override ARGBColor[] GetTilePalette(GBC_Scene scene) => GetPalmOS8BitPalette();
    }
}