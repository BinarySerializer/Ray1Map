using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBC_R1_Manager : GBC_BaseManager
    {
        public string GetROMFilePath => "ROM.gbc";

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 47).ToArray()), 
        });

        public override async UniTask<GBC_SceneList> GetSceneListAsync(Context context)
        {
            var file = await context.AddLinearSerializedFileAsync(GetROMFilePath);
            var pointerTable = PointerTables.GBC_PointerTable(context.Settings.GameModeSelection, file);
            var s = context.Deserializer;
            return s.DoAt(pointerTable[GBC_R1_Pointer.SceneList], () => s.SerializeObject<GBC_SceneList>(default, name: "SceneList"));
        }

        public override Unity_Map[] GetMaps(Context context, GBC_Map map, GBC_Scene scene) {

            var pal = Util.ConvertAndSplitGBCPalette(map.Palette, firstTransparent: false);
            var tileSetTex = ToTileSetTextureMultiPalette(map.TileKit.TileData, pal, CellSize, flipY: false);
            int numTiles = map.TileKit.TileData.Length / 16;
            //Util.ByteArrayToFile(context.BasePath + "test.png", tileSetTex.EncodeToPNG());

            var mapTiles = new MapTile[map.Width * map.Height];
            bool hasFGLayer = false;
            Dictionary<int, ushort> vramToTileIndex = new Dictionary<int, ushort>();
            for (int i = 0; i < mapTiles.Length; i++) {
                MapTile t = new MapTile() {
                    // Attributes
                    HorizontalFlip = map.BGMapAttributes.MapTiles[i].HorizontalFlip,
                    VerticalFlip = map.BGMapAttributes.MapTiles[i].VerticalFlip,
                    GBC_Priority = map.BGMapAttributes.MapTiles[i].GBC_Priority,

                    // Collision
                    CollisionType = map.Collision.MapTiles[i].CollisionType,
                };
                if (t.GBC_Priority == 1) hasFGLayer = true;

                // Determine tile index
                var indexInVRAMSigned = map.BGMapTileNumbers.MapTiles[i].TileMapY;
                var bank = map.BGMapAttributes.MapTiles[i].GBC_BankNumber;
                var key = indexInVRAMSigned | (bank << 8);
                if (!vramToTileIndex.ContainsKey(key)) {
                    var vramPointer = 0x9000 + (indexInVRAMSigned > 127 ? (-256 + indexInVRAMSigned) : indexInVRAMSigned) * 16;
                    var vramMap = bank == 1 ? map.VRAMBank2Map : map.VRAMBank1Map;
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
                var paletteIndex = map.BGMapAttributes.MapTiles[i].PaletteIndex;
                t.TileMapY = (ushort)(numTiles * paletteIndex + vramToTileIndex[key]);

                mapTiles[i] = t;
            }
            var unityMap = new Unity_Map {
                Width = (ushort)map.Width,
                Height = (ushort)map.Height,
                TileSet = new Unity_MapTileMap[]
                    {
                        new Unity_MapTileMap(tileSetTex, CellSize),
                    },
                MapTiles = mapTiles.Select(x => new Unity_Tile(x)).ToArray(),
                Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
            };
            if (hasFGLayer) {
                var mapTilesFG = new MapTile[map.Width * map.Height];
                for (int i = 0; i < mapTilesFG.Length; i++) {
                    MapTile t = mapTiles[i].GBC_Priority == 1 ? new MapTile() {
                        HorizontalFlip = mapTiles[i].HorizontalFlip,
                        VerticalFlip = mapTiles[i].VerticalFlip,
                        PaletteIndex = mapTiles[i].PaletteIndex,
                        GBC_Priority = mapTiles[i].GBC_Priority,
                        TileMapY = (ushort)(mapTiles[i].TileMapY + 1) // + 1 because transparent tile was added to tileset
                    } : new MapTile();
                    mapTilesFG[i] = t;
                }
                var palFG = Util.ConvertAndSplitGBCPalette(map.Palette, firstTransparent: true);
                var tileSetTexFG = ToTileSetTextureMultiPalette(map.TileKit.TileData, palFG, CellSize, flipY: false, addTransparentTile: true);
                var fgMap = new Unity_Map {
                    Width = (ushort)map.Width,
                    Height = (ushort)map.Height,
                    TileSet = new Unity_MapTileMap[]
                    {
                        new Unity_MapTileMap(tileSetTexFG, CellSize),
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


    }
}