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

        public KeyValuePair<string, int>[] Levels => new KeyValuePair<string, int>[]
        {
            new KeyValuePair<string, int>("music", 4),
            // TODO: Remaining levels
        };

        public string AllfixFilePath => "worldmap.pdb";

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

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Levels.Select((x, i) => new GameInfo_World(i, Enumerable.Range(0, x.Value).ToArray())).ToArray());

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
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

                            var tileSet = s.DoAt(record.DataPointer, () => s.SerializeObject<GBC_PalmOS_UncompressedBlock<GBC_PalmOS_TileSet>>(default, name: "TileSet"));

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

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var pal = GetPalmOS8BitPalette().Select(x => x.GetColor()).ToArray();

            var worldFile = $"{Levels[context.Settings.World].Key}.pdb";

            await context.AddLinearSerializedFileAsync(worldFile, BinaryFile.Endian.Big);
            await context.AddLinearSerializedFileAsync(AllfixFilePath, BinaryFile.Endian.Big);

            var world = FileFactory.Read<Palm_Database>(worldFile, context, onPreSerialize: (so, pd) => pd.Type = Palm_Database.DatabaseType.PDB);
            var allfix = FileFactory.Read<Palm_Database>(AllfixFilePath, context, onPreSerialize: (so, pd) => pd.Type = Palm_Database.DatabaseType.PDB);

            var s = context.Deserializer;

            var tileSet = s.DoAt(allfix.Records[294].DataPointer, () => s.SerializeObject<GBC_PalmOS_UncompressedBlock<GBC_PalmOS_TileSet>>(default, name: "TileSet")).Value;

            var playField = s.DoAt(world.Records[context.Settings.Level].DataPointer, () => s.SerializeObject<GBC_PalmOS_UncompressedBlock<GBC_PalmOS_PlayField>>(default, name: "PlayField")).Value;

            var map = s.DoAt(world.Records[playField.MapIndex].DataPointer, () => s.SerializeObject<GBC_PalmOS_UncompressedBlock<GBC_PalmOS_Map>>(default, name: "Map")).Value;

            var tileSetTex = Util.ToTileSetTexture(tileSet.TileData, pal, true, CellSize, flipY: false);

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