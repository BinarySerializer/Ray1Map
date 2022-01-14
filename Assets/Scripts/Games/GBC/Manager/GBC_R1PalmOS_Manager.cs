﻿using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBC
{
    public class GBC_R1PalmOS_Manager : GBC_BaseManager
    {
        public override int LevelCount => 47;
        public string AllfixFilePath => "worldmap.pdb";
        public string[] GetAllDataPaths(Context c) {
            return new string[] {
                c.GetR1Settings().GameModeSelection == GameModeSelection.RaymanGBCPalmOSColor || c.GetR1Settings().GameModeSelection == GameModeSelection.RaymanGBCPalmOSColorDemo ? "palmcolormenu" : "menu",
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
        }

        public static BaseColor[] GetPalmOS4BitPalette() => PaletteHelpers.CreateDummyPalette(16, firstTransparent: false).Reverse().ToArray();

        public static BaseColor[] GetPalmOS8BitPalette() {
            BaseColor[] pal = new BaseColor[256];
            int palIndex = 0;

            for (int r = 0; r < 6; r++) {
                for (int b = 0; b < 3; b++) {
                    for (int g = 0; g < 6; g++) {
                        pal[palIndex++] = new CustomColor(
                            (byte)(0xFF - r * 0x33) / 255f,
                            (byte)(0xFF - g * 0x33) / 255f,
                            (byte)(0xFF - b * 0x33) / 255f);
                    }
                }
            }
            for (int r = 0; r < 6; r++) {
                for (int b = 0; b < 3; b++) {
                    for (int g = 0; g < 6; g++) {
                        pal[palIndex++] = new CustomColor(
                            (byte)(0xFF - r * 0x33) / 255f,
                            (byte)(0xFF - g * 0x33) / 255f,
                            (byte)(0xFF - (b + 3) * 0x33) / 255f);
                    }
                }
            }
            for (int i = 2; i < 16; i++) {
                if (i % 3 == 0) continue;
                byte b = (byte)(0x11 * i);
                pal[palIndex++] = new CustomColor(b / 255f, b / 255f, b / 255f);
            }
            pal[palIndex++] = new CustomColor(0xC0 / 255, 0xC0 / 255, 0xC0 / 255);
            pal[palIndex++] = new CustomColor(0x80 / 255, 0x00 / 255, 0x00 / 255);
            pal[palIndex++] = new CustomColor(0x80 / 255, 0x00 / 255, 0x80 / 255);
            pal[palIndex++] = new CustomColor(0x00 / 255, 0x80 / 255, 0x00 / 255);
            pal[palIndex++] = new CustomColor(0x00 / 255, 0x80 / 255, 0x80 / 255);
            for (int i = palIndex; i < 256; i++) {
                pal[palIndex++] = new CustomColor(0, 0, 0);
            }
            return pal;
        }

        public override GameAction[] GetGameActions(GameSettings settings) => base.GetGameActions(settings).Concat(new GameAction[]
        {
            new GameAction("Export Databases", false, true, (input, output) => ExportDataBasesAsync(settings, output)),
        }).ToArray();

        void ExportVignette(GBC_PalmOS_Vignette vignette, string outputPath) {
            if (vignette == null) throw new Exception("Not a vignette");
            if (vignette.Width > 0x1000 || vignette.Height > 0x1000 || vignette.Width == 0 || vignette.Height == 0) {
                throw new Exception("Not a vignette");
            }
            int w = (int)vignette.Width;
            int h = (int)vignette.Height;

            var palette = vignette.BPP == 8 ? GetPalmOS8BitPalette() : GetPalmOS4BitPalette();

            Texture2D tex = TextureHelpers.CreateTexture2D(w, h);
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    int ind = y * w + x;
                    if (vignette.BPP == 8) {
                        int col = vignette.Data[ind];
                        tex.SetPixel(x, h - 1 - y, palette[col].GetColor());
                    } else {
                        int col = vignette.Data[ind / 2];
                        if (ind % 2 == 0) {
                            col = BitHelpers.ExtractBits(col, 4, 4);
                        } else {
                            col = BitHelpers.ExtractBits(col, 4, 0);
                        }
                        tex.SetPixel(x, h - 1 - y, palette[col].GetColor());
                    }
                }
            }
            tex.Apply();
            Util.ByteArrayToFile(outputPath, tex.EncodeToPNG());
        }

        public async UniTask ExportDataBasesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                var s = context.Deserializer;

                foreach (var filePath in Directory.GetFiles(context.BasePath, "*", SearchOption.TopDirectoryOnly))
                {
                    var ext = Path.GetExtension(filePath);
                    var type = ext == ".pdb" ? Palm_Database.DatabaseType.PDB : (ext == ".prc" ? Palm_Database.DatabaseType.PRC : (Palm_Database.DatabaseType?)null);

                    if (type == null)
                        continue;

                    var relPath = Path.GetFileName(filePath);
                    await context.AddLinearFileAsync(relPath, Endian.Big);

                    if (type == Palm_Database.DatabaseType.PDB) {
                        var dataFile = FileFactory.Read<LUDI_PalmOS_DataFile>(context, relPath);
                        ExportLUDIDataFile(dataFile, s, Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath)));
                    } else {
                        var dataBase = FileFactory.Read<Palm_Database>(context, relPath, (so, pd) => pd.Type = type.Value);

                        for (int i = 0; i < dataBase.RecordsCount; i++) {
                            var record = dataBase.Records[i];
                            var name = type == Palm_Database.DatabaseType.PRC ? $"{record.Name}_{record.ID}" : $"{i}";

                            string filename = $"{name}.bin";
                            var bytes = s.DoAt(record.DataPointer, () => s.SerializeArray<byte>(default, record.Length, name: $"Record[{i}]"));
                            Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), bytes);
                        }
                    }
                }
            }
        }

        public override void ExportVignette(Context context, string outputDir)
        {
            var s = context.Deserializer;

            var path = GetAllDataPaths(context).First(x => x.Contains("menu"));

            var dataBase = FileFactory.Read<Palm_Database>(context, path + ".pdb", (so, pd) => pd.Type = Palm_Database.DatabaseType.PDB);

            for (int i = 0; i < dataBase.RecordsCount; i++)
            {
                var record = dataBase.Records[i];
                bool exported = false;

                try
                {
                    var vignette = s.DoAt(record.DataPointer, () => s.SerializeObject<LUDI_CompressedBlock<GBC_PalmOS_Vignette>>(default, name: "Vignette"));
                    ExportVignette(vignette.Value, Path.Combine(outputDir, path, $"{i}.png"));
                    exported = true;
                }
                catch (Exception) { }

                if (!exported)
                {
                    try
                    {
                        var vignette = s.DoAt(record.DataPointer, () => s.SerializeObject<LUDI_UncompressedBlock<GBC_PalmOS_Vignette>>(default, name: "Vignette"));
                        ExportVignette(vignette.Value, Path.Combine(outputDir, path, $"{i}.png"));
                    }
                    catch (Exception) { }
                }
            }
        }

        public async UniTask InitGlobalOffsetTable(Context context) {
            LUDI_GlobalOffsetTable globalOffsetTable = new LUDI_GlobalOffsetTable();
            List<LUDI_BaseDataFile> dataFiles = new List<LUDI_BaseDataFile>();
            foreach (var path in GetAllDataPaths(context)) {
                var fullPath = $"{path}.pdb";
                if (await context.AddLinearFileAsync(fullPath, Endian.Big) != null)
                    dataFiles.Add(FileFactory.Read<LUDI_PalmOS_DataFile>(context, fullPath));
            }
            globalOffsetTable.Files = dataFiles.ToArray();
            context.StoreObject<LUDI_GlobalOffsetTable>(GlobalOffsetTableKey, globalOffsetTable);
        }

        public override GBC_LevelList GetLevelList(Context context)
        {
            var allfix = FileFactory.Read<LUDI_PalmOS_DataFile>(context, AllfixFilePath);
            var s = context.Deserializer;
            return s.DoAt(allfix.Resolve(1), () => s.SerializeObject<GBC_LevelManifest>(default, name: "LevelManifest")).LevelList;
        }

		public override Unity_Map[] GetMaps(Context context, GBC_PlayField playField, GBC_Level level) {

            bool greyScale = context.GetR1Settings().GameModeSelection == GameModeSelection.RaymanGBCPalmOSGreyscale || context.GetR1Settings().GameModeSelection == GameModeSelection.RaymanGBCPalmOSGreyscaleDemo;
            Util.TileEncoding encoding = greyScale ? Util.TileEncoding.Linear_4bpp_ReverseOrder : Util.TileEncoding.Linear_8bpp;
            Color[] pal = (greyScale ? GetPalmOS4BitPalette() : GetPalmOS8BitPalette()).Select(x => x.GetColor()).ToArray();
            var tileSetTex = Util.ToTileSetTexture(playField.TileKit.TileData, pal, encoding, CellSize, flipY: false);

            var maps = new Unity_Map[]
            {
                new Unity_Map
                {
                    Width = (ushort)playField.Width,
                    Height = (ushort)playField.Height,
                    TileSet = new Unity_TileSet[]
                    {
                        new Unity_TileSet(tileSetTex, CellSize),
                    },
                    MapTiles = playField.MapTiles.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                }
            };
            return maps;
        }

        public override async UniTask LoadFilesAsync(Context context) => await InitGlobalOffsetTable(context);
    }
}