using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.IO;
using UnityEngine;

namespace R1Engine
{
    public class GBC_R1PalmOS_Manager : IGameManager
    {

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

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[0]);

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Data", false, true, (input, output) => ExportDataAsync(settings, output, false)),
            new GameAction("Export Data (Categorized)", false, true, (input, output) => ExportDataAsync(settings, output, true)),
        };

        public async UniTask ExportDataAsync(GameSettings settings, string outputDir, bool categorized)
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
                        if (categorized && filePath.Contains("palmcolormenu")) {
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
                        } else {
                            string filename = $"{name}.bin";
                            var bytes = s.DoAt(record.DataPointer, () => s.SerializeArray<byte>(default, record.Length, name: $"Record[{i}]"));
                            Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), bytes);
                        }
                    }
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) => throw new NotImplementedException();

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public UniTask LoadFilesAsync(Context context) => UniTask.CompletedTask;
    }
}