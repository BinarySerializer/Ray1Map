using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_R1PocketPC_Manager : GBC_BaseManager
    {
        public override int LevelCount => 47;
        public string AllfixFilePath => "worldmap.dat";
        public string[] GetAllDataPaths(Context c) {
            return new string[] {
                ((c.Settings.GameModeSelection == GameModeSelection.RaymanGBCPocketPC_LandscapeIPAQ) ? "ipaqmenu" : "menu"),
                "worldmap",
                "jungle1",
                "jungle2",
                "jungle3",
                "jungle4",
                "music",
                "mount",
                "cave",
                "dark",

                // PocketPC exclusive
                "intro",
                "outro",
                "sound",

                // Other data files. Don't load automatically if loading is slow
                "IDRegister",
                "IDUnlock",
                "save",
                "save1"
            };
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

            var palette = vignette.BPP == 8 ? Util.CreateDummyPalette(256, firstTransparent: false) : Util.CreateDummyPalette(16, firstTransparent: false).Reverse().ToArray();

            Texture2D tex = TextureHelpers.CreateTexture2D(w, h);
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    int ind = y * w + x;
                    if (vignette.BPP == 16) {
                        BaseColor col = vignette.DataPPC[ind];
                        tex.SetPixel(x, h - 1 - y, col.GetColor());
                    } else if (vignette.BPP == 8) {
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

        public override void ExportVignette(Context context, string outputDir)
        {
            var s = context.Deserializer;

            var path = GetAllDataPaths(context).First(x => x.Contains("menu"));

            var dataFile = FileFactory.Read<LUDI_PocketPC_DataFile>(path + ".dat", context);

            for (int i = 0; i < dataFile.BlockCount; i++)
            {
                ushort blockID = dataFile.OffsetTable.Entries[i].BlockID;
                Pointer blockPtr = dataFile.Resolve(blockID);

                if (blockPtr == null) 
                    continue;

                bool exported = false;

                try
                {
                    var vignette = s.DoAt(blockPtr, () => s.SerializeObject<LUDI_CompressedBlock<GBC_PalmOS_Vignette>>(default, name: "Vignette"));
                    ExportVignette(vignette.Value, Path.Combine(outputDir, path, $"{blockID}.png"));
                    exported = true;
                }
                catch (Exception)
                {
                    s.Goto(blockPtr);
                }

                if (!exported)
                {
                    try
                    {
                        var vignette = s.DoAt(blockPtr, () => s.SerializeObject<LUDI_UncompressedBlock<GBC_PalmOS_Vignette>>(default, name: "Vignette"));
                        ExportVignette(vignette.Value, Path.Combine(outputDir, path, $"{blockID}.png"));
                    }
                    catch (Exception)
                    {
                        s.Goto(blockPtr);
                    }
                }
            }
        }

        public async UniTask ExportDataBasesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                foreach (var filePath in Directory.GetFiles(context.BasePath, "*.dat", SearchOption.TopDirectoryOnly))
                {
                    var relPath = Path.GetFileName(filePath);
                    await context.AddLinearSerializedFileAsync(relPath, BinaryFile.Endian.Little);
                    var dataFile = FileFactory.Read<LUDI_PocketPC_DataFile>(relPath, context);

                    for (int i = 0; i < dataFile.BlockCount; i++)
                    {
                        ushort blockID = dataFile.OffsetTable.Entries[i].BlockID;
                        Pointer blockPtr = dataFile.Resolve(blockID);
                        uint blockLength = dataFile.GetLength(blockID) ?? 0;

                        if(blockPtr == null) 
                            continue;

                        var name = $"{blockID}_{blockPtr.StringFileOffset}";

                        string filename = $"{name}.bin";
                        var bytes = s.DoAt(blockPtr, () => s.SerializeArray<byte>(default, blockLength, name: $"Record[{blockID}]"));
                        Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), filename), bytes);
                    }
                }
            }
        }

        public async UniTask InitGlobalOffsetTable(Context context) {
            LUDI_GlobalOffsetTable globalOffsetTable = new LUDI_GlobalOffsetTable();
            List<LUDI_BaseDataFile> dataFiles = new List<LUDI_BaseDataFile>();
            foreach (var path in GetAllDataPaths(context)) {
                var fullPath = $"{path}.dat";
                await context.AddLinearSerializedFileAsync(fullPath, BinaryFile.Endian.Little);
                dataFiles.Add(FileFactory.Read<LUDI_PocketPC_DataFile>(fullPath, context));
            }
            globalOffsetTable.Files = dataFiles.ToArray();
            context.StoreObject<LUDI_GlobalOffsetTable>(GlobalOffsetTableKey, globalOffsetTable);
        }

        public override GBC_LevelList GetSceneList(Context context)
        {
            var allfix = FileFactory.Read<LUDI_PocketPC_DataFile>(AllfixFilePath, context);
            var s = context.Deserializer;
            return s.DoAt(allfix.Resolve(1), () => s.SerializeObject<GBC_LevelManifest>(default, name: "SceneManfiest")).LevelList;
        }

		public override Unity_Map[] GetMaps(Context context, GBC_PlayField playField, GBC_Level level) {
            var tileSetTex = Util.ToTileSetTexture(playField.TileKit.TileDataPocketPC, CellSize, flipY: false);

            var maps = new Unity_Map[]
            {
                new Unity_Map
                {
                    Width = (ushort)playField.Width,
                    Height = (ushort)playField.Height,
                    TileSet = new Unity_MapTileMap[]
                    {
                        new Unity_MapTileMap(tileSetTex, CellSize),
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