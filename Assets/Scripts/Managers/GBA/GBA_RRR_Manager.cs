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
        /*
         * [0] = new Dictionary<int, string>() {
                [3] = "Wailing jail",
                [4] = "Boss Prison",
            },
            [1] = new Dictionary<int, string>() {
                [5] = "Child's play",
                [6] = "The kids' hamlet",
                [7] = "The toy chase",
                [8] = "Toy box",
                [9] = "Celestial castle",
            },
            [2] = new Dictionary<int, string>() {
                [10] = "Dream forest",
                [11] = "The leafy valley",
                [12] = "Colonial jungle",
                [13] = "The lush mountaintops",
                [14] = "Hidden burrow",
            },
            [3] = new Dictionary<int, string>() {
                [15] = "Stomach circuit",
                [16] = "Gastric rivers",
                [17] = "Living cavern",
                [18] = "Swallowed treasures",
                [19] = "The sticky lair",
            },
            [4] = new Dictionary<int, string>() {
                [20] = "The desert of desserts",
                [21] = "Cake race",
                [22] = "The sweet islands",
                [23] = "Not a piece of cake!",
                [24] = "Tart tunnels",
                [25] = "Ginger-bunny-bread",
            },
            [5] = new Dictionary<int, string>() {
                [26] = "Filthy corridors",
                [27] = "Agony jails",
                [28] = "Infernal escape",
                [29] = "Spikes and yikes!",
                [30] = "The rabbids' lair",
            },
            [6] = new Dictionary<int, string>() {
                [0] = "Village 0",
                [1] = "Village 1",
                [2] = "Village 2",
                [31] = "Village 3",
                [32] = "Shooting Range 1",
                [33] = "Title Screen",
                [34] = "Shooting Range 2",
            },
         * */

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)), 
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                var baseOffset = context.GetFile(GetROMFilePath).StartPointer + 0x722374;
                
                s.Goto(baseOffset);

                var length = s.Serialize<uint>(default);

                for (int i = 0; i < length; i++)
                {
                    s.Serialize<uint>(default);
                    var blockSize = s.Serialize<uint>(default);
                    var blockOffset = s.Serialize<uint>(default);
                    s.Serialize<uint>(default);

                    var blockPointer = baseOffset + blockOffset;

                    var block = s.DoAt(blockPointer, () => s.SerializeArray<byte>(default, blockSize));

                    if (block.Length > 4 && block[0] == 0x67 && block[1] == 0x45 && block[2] == 0x23 && block[3] == 0x01) {
                        s.DoAt(blockPointer, () => {
                            s.DoEncoded(new LZSSEncoder(blockSize), () => 
                            {
                                const int width = 240;
                                const int height = 160;

                                // Check if it's a vignette...
                                if (s.CurrentLength == (256 * 2) + (width * height))
                                {
                                    // Serialize palette
                                    var pal = s.SerializeObjectArray<ARGB1555Color>(default, 256).Select(x =>
                                    {
                                        var c = x.GetColor();
                                        c.a = 1;
                                        return c;
                                    }).ToArray();

                                    var tex = TextureHelpers.CreateTexture2D(width, height, true);

                                    for (int y = 0; y < height; y++)
                                    {
                                        for (int x = 0; x < width; x++)
                                        {
                                            var b = s.Serialize<byte>(default);

                                            tex.SetPixel(x, height - y - 1, pal[b]);
                                        }
                                    }

                                    tex.Apply();

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}_decompressed.png"), tex.EncodeToPNG());
                                }
                                else
                                {
                                    block = s.SerializeArray<byte>(default, s.CurrentLength);
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}_decompressed.dat"), block);
                                }
                            });
                        });
                    } else {
                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}.dat"), block);
                    }
                }
            }
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);

            Controller.DetailedState = $"Loading tile set";
            await Controller.WaitIfNecessary();

            var tilemap = LoadTileSet(rom.TileMap, false);
            var alphaBlendingTilemap = LoadTileSet(rom.AlphaTileMap, true);

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            var primaryMap = LoadMap(rom.LevelMap, rom.CollisionMap, tilemap);
            var alphaMap = LoadMap(rom.AlphaBlendingMap, null, alphaBlendingTilemap);

            await Controller.WaitIfNecessary();

            return new Unity_Level(
                maps: new Unity_Map[]
                {
                    primaryMap,
                    alphaMap
                }, 
                objManager: new Unity_ObjectManager_GBARRR(context),
                eventData: rom.LevelScene.Actors.Select(x => (Unity_Object)new Unity_Object_GBARRR(x)).ToList(),
                getCollisionTypeGraphicFunc: x => ((GBARRR_TileCollisionType)x).GetCollisionTypeGraphic(),
                cellSize: CellSize,
                localization: LoadLocalization(rom.Localization),
                defaultCollisionMap: 0,
                defaultMap: 1
            );
        }

        public Unity_Map LoadMap(GBARRR_MapBlock mapBlock, GBARRR_MapBlock collisionBlock, Unity_MapTileMap tilemap)
        {
            var map = new Unity_Map
            {
                Width = (ushort)(mapBlock.MapWidth * 4), // The game uses 32x32 tiles, made out of 8x8 tiles
                Height = (ushort)(mapBlock.MapHeight * 4),
                TileSetWidth = 1,
                TileSet = new Unity_MapTileMap[]
                {
                    tilemap
                },
                MapTiles = new Unity_Tile[mapBlock.MapWidth * 4 * mapBlock.MapHeight * 4]
            };

            // TODO: Fix this
            if (mapBlock.Type == GBARRR_MapBlock.MapType.AlphaBlending) {
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
                        var tilemapX = (mapBlock.Type == GBARRR_MapBlock.MapType.AlphaBlending && tile.TileMapX > 1) ? (ushort)(tile.TileMapX - 2) : tile.TileMapX;
                        map.MapTiles[tileY * map.Width + tileX] = new Unity_Tile(new MapTile()
                        {
                            /*TileMapX = mapBlock.Type == GBARRR_MapBlock.MapType.AlphaBlending 
                                // TODO: Which palette to use? 0xD works in map 0 for some things.
                                ? (ushort)(tile.TileMapX + 0xD * (tilemap.Tiles.Length / 16)) 
                                : tile.TileMapX,*/
                            TileMapX = (mapBlock.Type == GBARRR_MapBlock.MapType.AlphaBlending && tile.TileMapX > 1) ? (ushort)(tilemapX + (0xE - tile.PaletteIndex) * (tilemap.Tiles.Length / 16)) : tilemapX,
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

        public Unity_MapTileMap LoadTileSet(GBARRR_TileMap tilemap, bool is4bit)
        {
            int block_size = is4bit ? 0x20 : 0x40;
            const float texWidth = 256f;
            var tilesWidth = texWidth / CellSize;
            var tileCount = tilemap.Data.Length / block_size;
            var palCount = is4bit ? 16 : 1; // Duplicate tiles for every palette if 4-bit
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
                                    c = tilemap.Palette[p * 16 + b].GetColor();
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

        public void SaveLevel(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}