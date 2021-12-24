using Cysharp.Threading.Tasks;
using ImageMagick;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBC
{
    public class GBC_R1_Manager : GBC_BaseManager
    {
        public string GetROMFilePath => "ROM.gbc";

        public override int LevelCount => 47;

        public override GameAction[] GetGameActions(GameSettings settings) => base.GetGameActions(settings).Concat(new GameAction[]
        {
            new GameAction("Export Root TileKits", false, true, (input, output) => ExportRootTileKitsAsync(settings, output)),
        }).ToArray();

        public override GBC_LevelList GetLevelList(Context context)
        {
            return FileFactory.Read<GBC_ROM>(GetROMFilePath, context).LevelList;
        }

        public async UniTask ExportRootTileKitsAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                var s = context.Deserializer;
                var rom = FileFactory.Read<GBC_ROM>(GetROMFilePath, context);

                for (int i = 0; i < rom.ReferencesCount; i++)
                {
                    // Get the reference
                    var reference = rom.References[i];

                    if (reference.BlockHeader.Type == GBC_BlockType.TileKit)
                    {
                        var tileKit = s.DoAt(reference.Pointer.GetPointer(), () => s.SerializeObject<GBC_TileKit>(default, name: $"TileKit[{i}]"));
                        var tex = tileKit.GetTileSetTex();

                        for (int j = 0; j < tex.Length; j++)
                            Util.ByteArrayToFile(Path.Combine(outputDir, $"TileKit{i}_Pal{j}.png"), tex[0][j].EncodeToPNG());
                    }
                }
            }
        }

        public override async UniTask ExportBlocksAsync(GameSettings settings, string outputDir, bool export)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBC_ROM>(GetROMFilePath, context);

                for (int i = 0; i < rom.ReferencesCount; i++) {
                    var ptr = rom.References[i].Pointer.GetPointer();
                    var type = rom.References[i].BlockHeader.Type;
                    string path = $"{i} - {type}_0x{ptr.StringFileOffset}";
                    var dir = Path.Combine(outputDir, path);

                    Directory.CreateDirectory(dir);

                    ExportBlocks(context, dir, export, rom.References[i].Pointer.GetPointer());
                }
            }

            Debug.Log("Finished logging blocks");
        }

        public override void ExportVignette(Context context, string outputDir)
        {
            var s = context.Deserializer;
            var rom = FileFactory.Read<GBC_ROM>(GetROMFilePath, context);

            // Enumerate every reference
            for (int i = 0; i < rom.References.Length; i++)
            {
                // Get the reference
                var reference = rom.References[i];

                var dir = Path.Combine(outputDir, $"BlockRoot_{i}");

                if (reference.BlockHeader.Type == GBC_BlockType.Vignette)
                {
                    // We assume it's always an array
                    var vigArray = s.DoAt(reference.Pointer.GetPointer(), () => s.SerializeObject<GBC_BlockArray<GBC_Vignette>>(default, name: "Vignette"));

                    // Export every vignette
                    for (int j = 0; j < vigArray.Blocks.Length; j++)
                        Util.ByteArrayToFile(Path.Combine(dir, $"{j}.png"), vigArray.Blocks[j].ToTexture2D().EncodeToPNG());
                }
                else if (reference.BlockHeader.Type == GBC_BlockType.Video)
                {
                    using (MagickImageCollection collection = new MagickImageCollection()) {
                        // Serialize video block
                        var video = s.DoAt(reference.Pointer.GetPointer(), () => s.SerializeObject<GBC_Video>(default, name: "Video"));

                        for (int j = 0; j < video.Frames.Length; j++) {
                            Texture2D tex = video.Frames[j].ToTexture2D();

                            // Export frame
                            Util.ByteArrayToFile(Path.Combine(dir, "Frames", $"{j}.png"), tex.EncodeToPNG());

                            // Add frame to image collection
                            var img = tex.ToMagickImage();
                            collection.Add(img);
                            collection[j].AnimationDelay = 1;
                            collection[j].AnimationTicksPerSecond = 15;
                            collection[j].Trim();
                            collection[j].GifDisposeMethod = GifDisposeMethod.Background;
                        }

                        // Save gif
                        collection.Write(Path.Combine(dir, $"BlockRoot_{i}.gif"));
                    }
                }
            }

            var levelList = GetLevelList(context);
            for (int i = 0; i < levelList.DependencyTable.DependenciesCount; i++) {
                var level = s.DoAt(levelList.DependencyTable.GetPointer(i), () => {
                    return s.SerializeObject<GBC_Level>(default, name: $"Level[{i}]");
                });
                ExportVignetteReference(level, level.VignetteIntro, Path.Combine(outputDir, "Levels", i.ToString(), "Intro"));
                ExportVignetteReference(level, level.VignetteOutro, Path.Combine(outputDir, "Levels", i.ToString(), "Outro"));
                ExportVignetteReference(level, level.VignetteLevelName, Path.Combine(outputDir, "Levels", i.ToString(), "Name"));
            }

            void ExportVignetteReference(GBC_BaseBlock vigRefParent, GBC_Level.VignetteReference vigRef, string outPath) {
                vigRef.SerializeVignettes(s, vigRefParent);
                if (vigRef.Vignette != null) {
                    if (vigRef.Vignette.Vignettes != null) {
                        for (int j = 0; j < vigRef.Vignette.Vignettes.Length; j++)
                            Util.ByteArrayToFile(Path.Combine(outPath, $"{j}.png"), vigRef.Vignette.Vignettes[j].ToTexture2D().EncodeToPNG());
                    } else {
                        Util.ByteArrayToFile(outPath + ".png", vigRef.Vignette.ToTexture2D().EncodeToPNG());
                    }
                }
            }
        }

        public override Unity_Map[] GetMaps(Context context, GBC_PlayField playField, GBC_Level level) {

            var pal = Util.ConvertAndSplitGBCPalette(level.Scene.TilePalette, transparentIndex: null);
            var tileSetTex = ToTileSetTextureMultiPalette(playField.TileKit.TileData, pal, CellSize, flipY: false);
            int numTiles = playField.TileKit.TileData.Length / 16;
            //Util.ByteArrayToFile(context.BasePath + "test.png", tileSetTex.EncodeToPNG());

            var mapTiles = new MapTile[playField.Width * playField.Height];
            bool hasFGLayer = false;
            Dictionary<int, ushort> vramToTileIndex = new Dictionary<int, ushort>();
            for (int i = 0; i < mapTiles.Length; i++) {
                MapTile t = new MapTile() {
                    // Attributes
                    HorizontalFlip = playField.BGMapAttributes.MapTiles[i].HorizontalFlip,
                    VerticalFlip = playField.BGMapAttributes.MapTiles[i].VerticalFlip,
                    Priority = playField.BGMapAttributes.MapTiles[i].Priority,

                    // Collision
                    CollisionType = playField.Collision.MapTiles[i].CollisionType,
                };
                if (t.Priority) hasFGLayer = true;

                // Determine tile index
                var indexInVRAMSigned = playField.BGMapTileNumbers.MapTiles[i].TileMapY;
                var bank = playField.BGMapAttributes.MapTiles[i].GBC_BankNumber;
                var key = indexInVRAMSigned | (bank << 8);
                if (!vramToTileIndex.ContainsKey(key)) {
                    var vramPointer = 0x9000 + (indexInVRAMSigned > 127 ? (-256 + indexInVRAMSigned) : indexInVRAMSigned) * 16;
                    var vramMap = bank == 1 ? playField.VRAMBank2Map : playField.VRAMBank1Map;
                    bool found = false;
                    foreach (var vramEntry in vramMap) {
                        var startVRAMPointer = vramEntry.VRAMPointer;
                        var endVRAMPointer = startVRAMPointer + vramEntry.TileCount * 16;
                        if (vramPointer >= startVRAMPointer && vramPointer < endVRAMPointer) {
                            vramToTileIndex[key] = (ushort)((vramEntry.TileOffset + (vramPointer - startVRAMPointer)) / 16);
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        vramToTileIndex[key] = 0;
                    }
                }
                var paletteIndex = playField.BGMapAttributes.MapTiles[i].PaletteIndex;
                t.TileMapY = (ushort)(numTiles * paletteIndex + vramToTileIndex[key]);

                mapTiles[i] = t;
            }
            var unityMap = new Unity_Map {
                Width = (ushort)playField.Width,
                Height = (ushort)playField.Height,
                TileSet = new Unity_TileSet[]
                    {
                        new Unity_TileSet(tileSetTex, CellSize),
                    },
                MapTiles = mapTiles.Select(x => new Unity_Tile(x)).ToArray(),
                Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
            };
            if (hasFGLayer) {
                var mapTilesFG = new MapTile[playField.Width * playField.Height];
                for (int i = 0; i < mapTilesFG.Length; i++) {
                    MapTile t = mapTiles[i].Priority ? new MapTile() {
                        HorizontalFlip = mapTiles[i].HorizontalFlip,
                        VerticalFlip = mapTiles[i].VerticalFlip,
                        PaletteIndex = mapTiles[i].PaletteIndex,
                        Priority = mapTiles[i].Priority,
                        TileMapY = (ushort)(mapTiles[i].TileMapY + 1) // + 1 because transparent tile was added to tileset
                    } : new MapTile();
                    mapTilesFG[i] = t;
                }
                var palFG = Util.ConvertAndSplitGBCPalette(level.Scene.TilePalette, transparentIndex: 0);
                var tileSetTexFG = ToTileSetTextureMultiPalette(playField.TileKit.TileData, palFG, CellSize, flipY: false, addTransparentTile: true);
                var fgMap = new Unity_Map {
                    Width = (ushort)playField.Width,
                    Height = (ushort)playField.Height,
                    TileSet = new Unity_TileSet[]
                    {
                        new Unity_TileSet(tileSetTexFG, CellSize),
                    },
                    MapTiles = mapTilesFG.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                    Layer = Unity_Map.MapLayer.Front
                };
                return new Unity_Map[] { unityMap, fgMap };
            } else {
                return new Unity_Map[] { unityMap };
            }
        }

        public Texture2D ToTileSetTextureMultiPalette(byte[] imgData, Color[][] pal, int tileWidth, bool flipY, bool addTransparentTile = false) {
            int numPalettes = pal.Length;
            int wrap = 64;
            int bpp = 2; 
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            int tilesX = Math.Min((addTransparentTile ? 1 : 0) + tilesetLength * numPalettes, wrap);
            int tilesY = Mathf.CeilToInt(((addTransparentTile ? 1 : 0) + tilesetLength * numPalettes) / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth, clear: true);

            for (int p = 0; p < numPalettes; p++) {
                for (int i = 0; i < tilesetLength; i++) {
                    int tileInd = (addTransparentTile ? 1 : 0) + i + p * tilesetLength;
                    int tileY = (tileInd / wrap) * tileWidth;
                    int tileX = (tileInd % wrap) * tileWidth;

                    tex.FillInTile(imgData, i * tileSize, pal[p], Util.TileEncoding.Planar_2bpp, tileWidth, flipY, tileX, tileY);
                }
            }

            tex.Apply();

            return tex;
        }

        public override async UniTask LoadFilesAsync(Context context) => await context.AddLinearFileAsync(GetROMFilePath);
    }
}