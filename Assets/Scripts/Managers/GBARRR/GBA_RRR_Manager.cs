using Cysharp.Threading.Tasks;
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

        // TODO: There are actually 36 maps + 1 single layer map - are the other ones menus?
        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(3, 2).ToArray()),
            new GameInfo_World(1, Enumerable.Range(5, 5).ToArray()),
            new GameInfo_World(2, Enumerable.Range(10, 5).ToArray()),
            new GameInfo_World(3, Enumerable.Range(15, 5).ToArray()),
            new GameInfo_World(4, Enumerable.Range(20, 6).ToArray()),
            new GameInfo_World(5, Enumerable.Range(26, 5).ToArray()),
            new GameInfo_World(6, Enumerable.Range(0, 3).Concat(Enumerable.Range(31, 4)).ToArray()),
        });

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, false)), 
            new GameAction("Export Vignette", false, true, (input, output) => ExportBlocksAsync(settings, output, true)), 
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputPath, bool exportVignette)
        {
            const int vigWidth = 240;
            const int vigHeight = 160;

            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                // Load the rom
                var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);

                // Enumerate every block in the offset table
                for (int i = 0; i < rom.OffsetTable.OffsetTableCount; i++)
                {
                    // Get the offset
                    var offset = rom.OffsetTable.OffsetTable[i];

                    var absoluteOffset = (rom.OffsetTable.Offset + offset.BlockOffset).AbsoluteOffset;

                    rom.OffsetTable.DoAtBlock(context, i, size =>
                    {
                        if (size == (256 * 2) + (vigWidth * vigHeight) && exportVignette)
                        {
                            var vig = s.SerializeObject<GBARRR_Vignette>(default, name: $"Vignette[{i}]");

                            var tex = TextureHelpers.CreateTexture2D(vigWidth, vigHeight, true);

                            foreach (var c in vig.Palette)
                                c.Alpha = 255;

                            var index = 0;
                            for (int y = 0; y < vigHeight; y++)
                            {
                                for (int x = 0; x < vigWidth; x++)
                                {
                                    tex.SetPixel(x, vigHeight - y - 1, vig.Palette[vig.ImgData[index]].GetColor());
                                    
                                    index++;
                                }
                            }

                            tex.Apply();

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{absoluteOffset:X8}.png"), tex.EncodeToPNG());
                        }
                        else if (!exportVignette)
                        {
                            var bytes = s.SerializeArray<byte>(default, size, name: $"Block[{i}]");

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{absoluteOffset:X8}{(s.CurrentPointer.file is StreamFile ? "_decompressed" : String.Empty)}.dat"), bytes);
                        }
                    });
                }
            }
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);

            Controller.DetailedState = $"Loading tile set";
            await Controller.WaitIfNecessary();

            var bg0Tileset = LoadTileSet(rom.BG0TileSet, true, 15, 1);
            var bg1Tileset = LoadTileSet(rom.BG1TileSet, true, 12, 1);
            var levelTileset = LoadTileSet(rom.LevelTileset, false);
            var fgTileset = LoadTileSet(rom.FGTileSet, true, 0, 16); // TODO: Only serialize palettes 12, 13 and 14?

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            var bg0Map = new Unity_Map()
            {
                Width = 32,
                Height = 32,
                TileSetWidth = 1,
                TileSet = new Unity_MapTileMap[] { bg0Tileset },
                MapTiles = rom.BG0Map.MapTiles.Select(x => new Unity_Tile(x)).ToArray()
            };
            var bg1Map = new Unity_Map()
            {
                Width = 32,
                Height = 32,
                TileSetWidth = 1,
                TileSet = new Unity_MapTileMap[] { bg1Tileset },
                MapTiles = rom.BG1Map.MapTiles.Select(x => new Unity_Tile(x)).ToArray()
            };
            var levelMap = LoadMap(rom.LevelMap, rom.CollisionMap, levelTileset);
            var fgMap = LoadMap(rom.FGMap, null, fgTileset);

            await Controller.WaitIfNecessary();

            return new Unity_Level(
                maps: new Unity_Map[]
                {
                    bg0Map,
                    bg1Map,
                    levelMap,
                    fgMap
                }, 
                objManager: new Unity_ObjectManager_GBARRR(context),
                eventData: rom.LevelScene.Actors.Select(x => (Unity_Object)new Unity_Object_GBARRR(x)).ToList(),
                getCollisionTypeGraphicFunc: x => ((GBARRR_TileCollisionType)x).GetCollisionTypeGraphic(),
                cellSize: CellSize,
                localization: LoadLocalization(rom.Localization),
                defaultCollisionMap: 2,
                defaultMap: 2
            );
        }

        public Unity_Map LoadMap(GBARRR_MapBlock mapBlock, GBARRR_MapBlock collisionBlock, Unity_MapTileMap tileset)
        {
            var map = new Unity_Map
            {
                Width = (ushort)(mapBlock.MapWidth * 4), // The game uses 32x32 tiles, made out of 8x8 tiles
                Height = (ushort)(mapBlock.MapHeight * 4),
                TileSetWidth = 1,
                TileSet = new Unity_MapTileMap[]
                {
                    tileset
                },
                MapTiles = new Unity_Tile[mapBlock.MapWidth * 4 * mapBlock.MapHeight * 4]
            };

            // TODO: Correct alpha level and only do this on maps which use it
            if (mapBlock.Type == GBARRR_MapBlock.MapType.Foreground) {
                map.IsAdditive = true;
                map.Alpha = 0.5f;
            }

            for (int y = 0; y < mapBlock.MapHeight; y++)
            {
                for (int x = 0; x < mapBlock.MapWidth; x++)
                {
                    var actualX = x * 4;
                    var actualY = y * 4;

                    var index_32 = y * mapBlock.MapWidth + x;
                    var tile_32 = mapBlock.Indices_32[index_32];
                    var col_32 = collisionBlock?.Indices_32[index_32];

                    var tiles_16 = mapBlock.Indices_16[tile_32];
                    var col_16 = col_32 != null ? collisionBlock.Indices_16[col_32.Value] : null;

                    setTiles16(0, 0, 0);
                    setTiles16(2, 0, 1);
                    setTiles16(0, 2, 2);
                    setTiles16(2, 2, 3);
                    
                    void setTiles16(int offX, int offY, int index)
                    {
                        var i = tiles_16.TileIndices[index];
                        var tiles_8 = mapBlock.Tiles_8[i].Tiles;
                        var col_8 = col_16 != null ? collisionBlock?.Tiles_8[col_16.TileIndices[index]].Tiles : null;

                        setTileAt(actualX + offX + 0, actualY + offY + 0, tiles_8[0], col_8?[0].CollisionType, $"{i}-0");
                        setTileAt(actualX + offX + 1, actualY + offY + 0, tiles_8[1], col_8?[1].CollisionType, $"{i}-1");
                        setTileAt(actualX + offX + 0, actualY + offY + 1, tiles_8[2], col_8?[2].CollisionType, $"{i}-2");
                        setTileAt(actualX + offX + 1, actualY + offY + 1, tiles_8[3], col_8?[3].CollisionType, $"{i}-3");
                    }

                    void setTileAt(int tileX, int tileY, MapTile tile, byte? collisionType, string debugString)
                    {
                        var tilemapX = (mapBlock.Type == GBARRR_MapBlock.MapType.Foreground && tile.TileMapX > 1) ? (ushort)(tile.TileMapX - 2) : tile.TileMapX;
                        map.MapTiles[tileY * map.Width + tileX] = new Unity_Tile(new MapTile()
                        {
                            /*TileMapX = mapBlock.Type == GBARRR_MapBlock.MapType.AlphaBlending 
                                // TODO: Which palette to use? 0xD works in map 0 for some things.
                                ? (ushort)(tile.TileMapX + 0xD * (tilemap.Tiles.Length / 16)) 
                                : tile.TileMapX,*/
                            TileMapX = (mapBlock.Type == GBARRR_MapBlock.MapType.Foreground && tile.TileMapX > 1) ? (ushort)(tilemapX + ((0xE + tile.PaletteIndex) % 16) * (tileset.Tiles.Length / 16)) : tilemapX,
                            CollisionType = collisionType ?? 0,
                            HorizontalFlip = tile.HorizontalFlip,
                            VerticalFlip = tile.VerticalFlip
                        })
                        {
                            DebugText = debugString
                        };
                    }
                }
            }

            return map;
        }

        public Unity_MapTileMap LoadTileSet(GBARRR_Tileset tilemap, bool is4bit, int palStart = 0, int palCount = 1)
        {
            int block_size = is4bit ? 0x20 : 0x40;
            const float texWidth = 256f;
            const float tilesWidth = texWidth / CellSize;
            var tileCount = tilemap.Data.Length / block_size;
            var texHeight = Mathf.CeilToInt(tileCount / tilesWidth) * CellSize;
            //UnityEngine.Debug.Log(tileCount + " - " + block_size);

            // Get the tile-set texture
            var tex = TextureHelpers.CreateTexture2D((int)texWidth, texHeight * palCount);

            for (int p = 0; p < palCount; p++)
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

                                if (is4bit) {
                                    var off = offset + ((yy * CellSize) + xx) / 2;
                                    var relOff = ((yy * CellSize) + xx);
                                    var b = tilemap.Data[off];
                                    b = (byte)BitHelpers.ExtractBits(b, 4, relOff % 2 == 0 ? 0 : 4);
                                    c = tilemap.Palette[(p + palStart) * 16 + b].GetColor();
                                    c = new Color(c.r, c.g, c.b, b != 0 ? 1f : 0f);
                                }
                                else
                                {
                                    var b = tilemap.Data[offset + (yy * CellSize) + xx];
                                    c = tilemap.Palette[b].GetColor();
                                    c = new Color(c.r, c.g, c.b, b != 0 ? 1f : 0f);
                                }

                                tex.SetPixel(x + xx, p * texHeight + y + yy, c);
                            }
                        }

                        tileIndex++;
                    }
                }
            }

            tex.Apply();

            return new Unity_MapTileMap(tex, CellSize);
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
    }
}