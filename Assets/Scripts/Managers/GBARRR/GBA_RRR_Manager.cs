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
        public GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[]
        {
            new GameInfo_Volume(GameMode.Game.ToString(), new GameInfo_World[]
            {
                new GameInfo_World(0, Enumerable.Range(2, 5).Append(29).ToArray()), // Child
                new GameInfo_World(1, Enumerable.Range(7, 5).ToArray()), // Forest
                new GameInfo_World(2, Enumerable.Range(12, 5).ToArray()), // Organic Cave
                new GameInfo_World(3, Enumerable.Range(17, 6).Append(31).ToArray()), // Sweets
                new GameInfo_World(4, Enumerable.Range(0, 2).Concat(Enumerable.Range(23, 5)).ToArray()), // Dark
                new GameInfo_World(5, Enumerable.Range(30, 1).ToArray()), // Menu
            }),
            new GameInfo_Volume(GameMode.Village.ToString(), new GameInfo_World[]
            {
                new GameInfo_World(5, Enumerable.Range(0, 3).ToArray()), // These are all actually level 28, but since there are 3 variants in a separate array it's easier to separate them like this 
            }),
            new GameInfo_Volume(GameMode.Mode7.ToString(), new GameInfo_World[]
            {
                new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
                new GameInfo_World(2, Enumerable.Range(1, 1).ToArray()),
                new GameInfo_World(3, Enumerable.Range(2, 1).ToArray()),
            }), 
        };

        public enum GameMode
        {
            Game,
            Village,
            Mode7
        }

        public static GameMode GetCurrentGameMode(GameSettings s) => (GameMode)Enum.Parse(typeof(GameMode), s.EduVolume);

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, ExportFlags.Normal)), 
            new GameAction("Export Vignette", false, true, (input, output) => ExportBlocksAsync(settings, output, ExportFlags.Vignette)),
            new GameAction("Export Categorized & Converted Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, ExportFlags.All)),
            new GameAction("Export Actor Graphics", false, true, (input, output) => ExportGraphicsAsync(settings, output)),
        };

        public async UniTask ExportGraphicsAsync(GameSettings settings, string outputPath) {

            using (var context = new Context(settings)) {
                var s = context.Deserializer;

                await LoadFilesAsync(context);
                
                // Load the rom
                var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);

                for (int w = 0; w < 6; w++) {
                    for (int l = 0; l < 0x1d; l++) {
                        var dict = LoadActorGraphics(s, rom, l, w);
                        for (int i = 0; i < rom.GraphicsTable0[w].Length; i++) {
                            var act = new GBARRR_Actor() {
                                Ushort_0C = (ushort)(rom.GraphicsTable0[w][i].Key * 2)
                            };
                            AssignActorValues(act, rom, l, w);
                            if (act.P_GraphicsOffset != 0) {
                                if (!dict.ContainsKey(act.P_GraphicsOffset)) continue;
                                act.GraphicsBlock = dict[act.P_GraphicsOffset];
                                /*    UnityEngine.Debug.Log("Graphics with offset " + string.Format("{0:X8}", act.P_GraphicsOffset) + " weren't loaded!");
                                } else {
                                    act.GraphicsBlock = dict[act.P_GraphicsOffset];
                                }*/
                                if (act.P_PaletteIndex < 16) {
                                    // UnityEngine.Debug.Log(act.P_PaletteIndex);
                                    // Use rom.spritepalette to test for now
                                    ExportSpriteFrames(act.GraphicsBlock, rom.SpritePalette, (int)act.P_PaletteIndex, outputPath + $"/{w}/{l}/rompalette/");
                                } else {
                                    rom.OffsetTable.DoAtBlock(context, act.P_PaletteIndex, (size) => {
                                        act.Palette = s.SerializeObject<GBARRR_Palette>(act.Palette, name: nameof(act.Palette));
                                        ExportSpriteFrames(act.GraphicsBlock, act.Palette.Palette, 0, outputPath + $"/{w}/{l}/blockpalette/");
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        [Flags]
        public enum ExportFlags {
            Normal = 1 << 0,
            Vignette = 1 << 1,
            Graphics = 1 << 2,
            Palettes = 1 << 3,
            LevelBlocks = 1 << 4,
            LevelTilesets = 1 << 5,
            Tilesets = 1 << 6,

            All = Normal | Vignette | Graphics | Palettes | LevelBlocks | Tilesets
        }

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputPath, ExportFlags flags)
        {
            const int vigWidth = 240;
            const int vigHeight = 160;

            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                // Load the rom
                var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);

                ARGBColor[] pal = flags.HasFlag(ExportFlags.Graphics) ? Util.CreateDummyPalette(16, true) : null;

                var lvlBlocks = rom.LevelInfo.SelectMany(x => new uint[]
                {
                    // Tilesets
                    x.LevelTilesetIndex, x.BG0TilesetIndex, x.FGTilesetIndex, x.BG1TilesetIndex,
                    // Maps & scene
                    x.BG0MapIndex, x.BG1MapIndex, x.CollisionMapIndex, x.LevelMapIndex, x.FGMapIndex, x.SceneIndex,
                    // Palette
                    x.SpritePaletteIndex
                }).ToArray();

                // Enumerate every block in the offset table
                for (uint i = 0; i < rom.OffsetTable.OffsetTableCount; i++)
                {
                    // Get the offset
                    var offset = rom.OffsetTable.OffsetTable[i];

                    var absoluteOffset = (rom.OffsetTable.Offset + offset.BlockOffset).AbsoluteOffset;

                    rom.OffsetTable.DoAtBlock(context, i, size =>
                    {
                        Pointer blockOff = s.CurrentPointer;
                        bool exported = false;

                        // Level blocks
                        if (!exported && flags.HasFlag(ExportFlags.LevelBlocks)) {
                            if (lvlBlocks.Contains(i)) {
                                exported = true;
                                var bytes = s.SerializeArray<byte>(default, size, name: $"Block[{i}]");

                                Util.ByteArrayToFile(Path.Combine(outputPath, $"LevelBlocks/{i}_{absoluteOffset:X8}{(s.CurrentPointer.file is StreamFile ? "_decompressed" : String.Empty)}.dat"), bytes);
                            }
                        }

                        // Vignette
                        if (!exported && flags.HasFlag(ExportFlags.Vignette)) {
                            s.Goto(blockOff);
                            if (size == (256 * 2) + (vigWidth * vigHeight)) {
                                var vig = s.SerializeObject<GBARRR_Vignette>(default, name: $"Vignette[{i}]");

                                var tex = TextureHelpers.CreateTexture2D(vigWidth, vigHeight, true);

                                foreach (var c in vig.Palette)
                                    c.Alpha = 255;

                                var index = 0;
                                for (int y = 0; y < vigHeight; y++) {
                                    for (int x = 0; x < vigWidth; x++) {
                                        tex.SetPixel(x, vigHeight - y - 1, vig.Palette[vig.ImgData[index]].GetColor());

                                        index++;
                                    }
                                }

                                tex.Apply();
                                exported = true;
                                Util.ByteArrayToFile(Path.Combine(outputPath, $"Vignette/{i}_{absoluteOffset:X8}.png"), tex.EncodeToPNG());
                            }
                        }

                        // Graphics
                        if (!exported && flags.HasFlag(ExportFlags.Graphics)) {
                            s.Goto(blockOff);
                            try {
                                var gb = s.SerializeObject<GBARRR_GraphicsBlock>(default, name: $"GraphicsBlock[{i}]");
                                if (gb.Count != 0) {
                                    int tileDataSize = gb.TileData[0].Length;
                                    if (gb.TileData.All(td => td.Length == tileDataSize) && Math.Sqrt(tileDataSize * 2) % 1 == 0) {
                                        //gb.TileSize = (uint)Mathf.RoundToInt(Mathf.Sqrt(tileDataSize * 2));
                                        ExportSpriteFrames(gb, pal, 0, Path.Combine(outputPath, $"ActorGraphics/{i}_{absoluteOffset:X8}/"));
                                        exported = true;
                                    }/* else {
                                        UnityEngine.Debug.Log($"Possible Graphics block {i}: {Math.Sqrt(tileDataSize * 2)} - {tileDataSize}");
                                    }*/
                                }
                            } catch (Exception ex) {
                            }
                        }

                        // Tilesets
                        if (!exported && flags.HasFlag(ExportFlags.Tilesets)) {
                            s.Goto(blockOff);
                            if (size > 0x200 && ((size - 0x200) % 0x20) == 0) {
                                try {
                                    var tileset = s.SerializeObject<GBARRR_Tileset>(default, onPreSerialize: t => t.BlockSize = size, name: $"Tileset[{i}]");
                                    int length = tileset.Data.Length;
                                    bool is4Bit = (length % 0x40 != 0);
                                    var ts = LoadTileSet(tileset.Data, is4Bit, tileset.Palette, palCount: is4Bit ? 16 : 1);
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"Tilesets/{i}_{absoluteOffset:X8}.png"), ts.Tiles[0].texture.EncodeToPNG());
                                    exported = true;
                                } catch (Exception ex) {
                                }
                            }
                        }

                        // Palettes
                        if (!exported && flags.HasFlag(ExportFlags.Palettes)) {
                            s.Goto(blockOff);
                            if (size == 0x200)
                            {
                                var p = s.SerializeObject<GBARRR_Palette>(default, name: $"Palette[{i}]");
                                PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"Palette/{i}_{absoluteOffset:X8}.png"), p.Palette.Select(x => new ARGBColor(x.Red, x.Green, x.Blue)).ToArray(), optionalWrap: 16);
                                exported = true;
                            }
                        }

                        // Binary
                        if (!exported && flags.HasFlag(ExportFlags.Normal)) {
                            s.Goto(blockOff);
                            var bytes = s.SerializeArray<byte>(default, size, name: $"Block[{i}]");

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"Uncategorized/{i}_{absoluteOffset:X8}{(s.CurrentPointer.file is StreamFile ? "_decompressed" : String.Empty)}.dat"), bytes);
                        }
                    });
                }
            }
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBARRR_ROM>(GetROMFilePath, context);

            var lvl = context.Settings.Level;
            var world = context.Settings.World;

            if (GetCurrentGameMode(context.Settings) == GameMode.Village)
                lvl = 28;

            Debug.Log($"RRR level: {world}-{lvl} ({GetCurrentGameMode(context.Settings)})");

            Controller.DetailedState = $"Loading tile set";
            await Controller.WaitIfNecessary();

            var bg0Tileset = LoadTileSet(rom.BG0TileSet.Data, true, rom.TilePalette ?? rom.BG0TileSet.Palette, 15, 1);
            var bg1Tileset = LoadTileSet(rom.BG1TileSet.Data, true, rom.TilePalette ?? rom.BG1TileSet.Palette, 12, 1);
            var levelTileset = LoadTileSet(rom.LevelTileset.Data, false, rom.TilePalette ?? rom.LevelTileset.Palette);
            var fgTileset = LoadTileSet(rom.FGTileSet.Data, true, rom.TilePalette ?? rom.FGTileSet.Palette, 0, 16); // TODO: Only serialize palettes 12, 13 and 14?

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
            var levelMap = LoadMap(rom.LevelMap, rom.CollisionMap, levelTileset, world);
            var fgMap = LoadMap(rom.FGMap, null, fgTileset, world);

            await Controller.WaitIfNecessary();

            SerializerObject s = context.Deserializer;
            var dict = LoadActorGraphics(s, rom, lvl, world);
            var graphicsData = new List<Unity_ObjectManager_GBARRR.GraphicsData>();

            var actorIndex = 0;

            // Enumerate every actor
            foreach (var act in rom.LevelScene.Actors)
            {
                Controller.DetailedState = $"Loading actor {actorIndex + 1}/{rom.LevelScene.Actors.Length}";
                await Controller.WaitIfNecessary();

                AssignActorValues(act, rom, lvl, world);

                if (act.P_GraphicsOffset != 0)
                {
                    if (!dict.ContainsKey(act.P_GraphicsOffset))
                        Debug.LogWarning($"Graphics with offset {act.P_GraphicsOffset:X8} wasn't loaded!");
                    else
                        act.GraphicsBlock = dict[act.P_GraphicsOffset];

                    if (act.GraphicsBlock != null)
                    {
                        if (act.P_PaletteIndex < 16)
                        {
                            graphicsData.Add(new Unity_ObjectManager_GBARRR.GraphicsData(act.P_GraphicsOffset,
                                GetSpriteFrames(act.GraphicsBlock, rom.SpritePalette, (int) act.P_PaletteIndex)
                                    .Select(x => x.CreateSprite()).ToArray()));
                        }
                        else
                        {
                            rom.OffsetTable.DoAtBlock(context, act.P_PaletteIndex,
                                size => act.Palette =
                                    s.SerializeObject<GBARRR_Palette>(act.Palette, name: nameof(act.Palette)));

                            graphicsData.Add(new Unity_ObjectManager_GBARRR.GraphicsData(act.P_GraphicsOffset,
                                GetSpriteFrames(act.GraphicsBlock, act.Palette.Palette, 0).Select(x => x.CreateSprite())
                                    .ToArray()));
                        }
                    }
                }

                actorIndex++;
            }

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(rom.Localization);

            var objManager = new Unity_ObjectManager_GBARRR(context, graphicsData.ToArray());

            await Controller.WaitIfNecessary();

            return new Unity_Level(
                maps: new Unity_Map[]
                {
                    bg0Map,
                    bg1Map,
                    levelMap,
                    fgMap
                }, 
                objManager: objManager,
                eventData: rom.LevelScene.Actors.Select(x => (Unity_Object)new Unity_Object_GBARRR(x, objManager)).ToList(),
                getCollisionTypeGraphicFunc: x => ((GBARRR_TileCollisionType)x).GetCollisionTypeGraphic(),
                cellSize: CellSize,
                localization: loc,
                defaultCollisionMap: 2,
                defaultMap: 2
            );
        }

        protected IEnumerable<Texture2D> GetSpriteFrames(GBARRR_GraphicsBlock spr, ARGBColor[] palette, int paletteIndex)
        {
            // For each frame
            int width = (int)spr.TileSize;
            int height = (int)spr.TileSize;
            for (int i = 0; i < spr.Count; i++)
            {
                var tex = TextureHelpers.CreateTexture2D(width, height, true);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int tileX = x / 8;
                        int tileY = y / 8;
                        int inTileX = x % 8;
                        int inTileY = y % 8;
                        int tileIndex = tileX + (tileY * width / 8);
                        int index = tileIndex * (8 * 4) + (inTileY * 8 + inTileX) / 2;

                        if (index < spr.TileData[i].Length)
                        {
                            var v = BitHelpers.ExtractBits(spr.TileData[i][index], 4, x % 2 == 0 ? 0 : 4);

                            Color c = palette[paletteIndex * 16 + v].GetColor();

                            c = v != 0 ? new Color(c.r, c.g, c.b, 1f) : new Color(c.r, c.g, c.b, 0f);

                            tex.SetPixel(x, height - y - 1, c);
                        }
                        else
                        {
                            Debug.Log($"{spr.Offset.AbsoluteOffset:X8} - " + spr.TileData[i].Length + " - " + width + " - " + index);
                        }
                    }
                }
                tex.Apply();

                yield return tex;
            }
        }

        protected void ExportSpriteFrames(GBARRR_GraphicsBlock spr, ARGBColor[] palette, int paletteIndex, string outputDir) {
            try 
            {
                var index = 0;

                // For each frame
                foreach (var tex in GetSpriteFrames(spr, palette, paletteIndex))
                {
                    var fileName = $"Sprites_{spr.Offset.AbsoluteOffset:X8}_Pal{paletteIndex}/{index}.png";
                    Util.ByteArrayToFile(Path.Combine(outputDir, fileName), tex.EncodeToPNG());
                    index++;
                }
            } catch (Exception ex) {
                Debug.LogError($"Error for GraphicsBlock {spr.Offset.AbsoluteOffset:X8} - Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
            }
        }

        public Unity_Map LoadMap(GBARRR_MapBlock mapBlock, GBARRR_MapBlock collisionBlock, Unity_MapTileMap tileset, int world)
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

            // TODO: Correct alpha level
            if (mapBlock.Type == GBARRR_MapBlock.MapType.Foreground && (world == 2 || world == 3 || world == 4)) {
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

        public Unity_MapTileMap LoadTileSet(byte[] tilemap, bool is4bit, ARGB1555Color[] palette, int palStart = 0, int palCount = 1)
        {
            int block_size = is4bit ? 0x20 : 0x40;
            const float texWidth = 256f;
            const float tilesWidth = texWidth / CellSize;
            var tileCount = tilemap.Length / block_size;
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
                                    var b = tilemap[off];
                                    b = (byte)BitHelpers.ExtractBits(b, 4, relOff % 2 == 0 ? 0 : 4);
                                    c = palette[(p + palStart) * 16 + b].GetColor();
                                    c = new Color(c.r, c.g, c.b, b != 0 ? 1f : 0f);
                                }
                                else
                                {
                                    var b = tilemap[offset + (yy * CellSize) + xx];
                                    c = palette[b].GetColor();
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

        public Dictionary<uint, GBARRR_GraphicsBlock> LoadActorGraphics(SerializerObject s, GBARRR_ROM rom, int level, int world) {

            Dictionary<uint, GBARRR_GraphicsBlock> indexDict = new Dictionary<uint, GBARRR_GraphicsBlock>();
            GBARRR_GraphicsBlock LoadGraphicsBlock(uint index, uint count, uint tileSize) {
                GBARRR_GraphicsBlock obj = null;
                if(indexDict.ContainsKey(index)) return indexDict[index];
                rom.OffsetTable.DoAtBlock(s.Context, index, size => {
                    obj = s.SerializeObject<GBARRR_GraphicsBlock>(null, onPreSerialize: gb => {
                        gb.Count = count;
                        gb.TileSize = tileSize;
                    }, name: "Graphics");
                });
                indexDict[index] = obj;
                return obj;
            }

            Dictionary<uint, GBARRR_GraphicsBlock> memAddressDict = new Dictionary<uint, GBARRR_GraphicsBlock>();
            memAddressDict[0x03003F6C] = LoadGraphicsBlock(0x12, 3, 0x10);
            memAddressDict[0x03004310] = LoadGraphicsBlock(0x16, 9, 0x10);
            memAddressDict[0x0300432C] = LoadGraphicsBlock(0x19, 4, 0x10);
            memAddressDict[0x03002928] = LoadGraphicsBlock(0x1a, 0xb, 0x10);
            memAddressDict[0x030024BC] = LoadGraphicsBlock(0x1c, 4, 0x10);
            memAddressDict[0x030028A4] = LoadGraphicsBlock(0x1d, 0xb, 0x10);
            memAddressDict[0x030024F8] = LoadGraphicsBlock(0x1e, 8, 0x20);
            memAddressDict[0x030028A0] = LoadGraphicsBlock(7, 0x70, 0x10);
            memAddressDict[0x03004F88] = LoadGraphicsBlock(0xf, 8, 0x20);
            memAddressDict[0x030025E8] = LoadGraphicsBlock(0x2c, 0xc, 0x10);
            memAddressDict[0x03002EC4] = LoadGraphicsBlock(0x2e, 0xd, 0x20);
            memAddressDict[0x03002954] = LoadGraphicsBlock(0x1f, 1, 0x20);
            memAddressDict[0x030051D4] = LoadGraphicsBlock(0x22, 1, 0x20);
            memAddressDict[0x03005024] = LoadGraphicsBlock(0x24, 1, 0x20);
            memAddressDict[0x03002318] = LoadGraphicsBlock(0x26, 1, 0x20);
            memAddressDict[0x03005194] = LoadGraphicsBlock(0x21, 1, 0x20);
            memAddressDict[0x030024B4] = LoadGraphicsBlock(0x20, 1, 0x20);
            memAddressDict[0x03002890] = LoadGraphicsBlock(0x27, 1, 0x20);
            memAddressDict[0x03004F80] = LoadGraphicsBlock(0x29, 1, 0x20);
            memAddressDict[0x030025D4] = LoadGraphicsBlock(0x28, 1, 0x20);
            memAddressDict[0x03005160] = LoadGraphicsBlock(0x2b, 1, 0x20);
            memAddressDict[0x0300235C] = LoadGraphicsBlock(0x30, 4, 0x10);
            memAddressDict[0x03002918] = LoadGraphicsBlock(0x32, 2, 0x10);
            memAddressDict[0x030023F8] = LoadGraphicsBlock(0x39, 8, 0x20);
            memAddressDict[0x03005080] = LoadGraphicsBlock(0x00000305, 0xc, 0x40);
            memAddressDict[0x03002EF4] = LoadGraphicsBlock(0x00000311, 0xf, 0x20);
            memAddressDict[0x030050E0] = LoadGraphicsBlock(0x00000315, 0xf, 0x20);
            memAddressDict[0x03002E34] = LoadGraphicsBlock(0x314, 0xf, 0x20);
            memAddressDict[0x03002560] = LoadGraphicsBlock(0x00000313, 0xf, 0x20);
            memAddressDict[0x030051C8] = LoadGraphicsBlock(0x00000312, 0xf, 0x20);
            memAddressDict[0x030050B4] = LoadGraphicsBlock(0x00000317, 0xe, 0x40);
            memAddressDict[0x030028D8] = LoadGraphicsBlock(0x318, 0xe, 0x40);
            memAddressDict[0x03004334] = LoadGraphicsBlock(0x00000319, 0xe, 0x40);
            memAddressDict[0x03005170] = LoadGraphicsBlock(0x0000031A, 0xe, 0x40);
            memAddressDict[0x0300308C] = LoadGraphicsBlock(0x0000031B, 0xe, 0x40);
            memAddressDict[0x030028E0] = LoadGraphicsBlock(0x31c, 0xe, 0x40);
            memAddressDict[0x03005234] = LoadGraphicsBlock(0x3b, 9, 0x10);
            memAddressDict[0x03004FA0] = LoadGraphicsBlock(0x3d, 8, 0x20);
            memAddressDict[0x03003F6C] = LoadGraphicsBlock(0x12, 3, 0x10);

            if (level < 0x1d) {
                memAddressDict[0x03005158] = LoadGraphicsBlock(0x0000013F, 4, 0x20);
                memAddressDict[0x03002E2C] = LoadGraphicsBlock(0x324, 3, 0x20);
                memAddressDict[0x03004314] = LoadGraphicsBlock(0x00000322, 3, 0x20);
                memAddressDict[0x0300254C] = LoadGraphicsBlock(0x0000031E, 3, 0x20);
                memAddressDict[0x03002910] = LoadGraphicsBlock(800, 4, 0x20);
                memAddressDict[0x03004F90] = LoadGraphicsBlock(0xe5, 1, 0x10);
                memAddressDict[0x03005208] = LoadGraphicsBlock(0xe6, 4, 0x20);
                memAddressDict[0x03005068] = LoadGraphicsBlock(0x00000327, 4, 0x20);
                memAddressDict[0x03005078] = LoadGraphicsBlock(0x00000325, 4, 0x20);
                memAddressDict[0x03002460] = LoadGraphicsBlock(0x00000326, 4, 0x20);
                memAddressDict[0x0300401C] = LoadGraphicsBlock(0x138, 4, 0x10);
                memAddressDict[0x0300248C] = LoadGraphicsBlock(0x00000139, 4, 0x10);
                memAddressDict[0x03002374] = LoadGraphicsBlock(0x0000013B, 4, 0x10);
                memAddressDict[0x03005064] = LoadGraphicsBlock(0x0000013D, 4, 0x10);
                memAddressDict[0x030025A4] = LoadGraphicsBlock(0x00000307, 8, 0x20);
                memAddressDict[0x03003080] = LoadGraphicsBlock(0x00000309, 8, 0x40);
                memAddressDict[0x0300502C] = LoadGraphicsBlock(0x0000030A, 8, 0x40);
                memAddressDict[0x03005108] = LoadGraphicsBlock(0x0000030B, 8, 0x40);
                memAddressDict[0x03004250] = LoadGraphicsBlock(0xf8, 1, 0x10);
                memAddressDict[0x03004F6C] = LoadGraphicsBlock(0x180, 4, 0x40);
                memAddressDict[0x03002520] = LoadGraphicsBlock(0x182, 2, 0x10);
            }
            if (level == 0x1b) {
                memAddressDict[0x03002EF8] = LoadGraphicsBlock(0x368, 4, 0x40);
                memAddressDict[0x03002440] = LoadGraphicsBlock(0x00000369, 9, 0x40);
                memAddressDict[0x030024E8] = LoadGraphicsBlock(0x0000036B, 5, 0x40);
                memAddressDict[0x03002538] = LoadGraphicsBlock(0x36c, 5, 0x40);
                memAddressDict[0x030022F0] = LoadGraphicsBlock(0x0000036D, 6, 0x40);
                memAddressDict[0x03004244] = LoadGraphicsBlock(0x0000036E, 5, 0x40);
                memAddressDict[0x03002528] = LoadGraphicsBlock(0x0000036F, 5, 0x40);
                memAddressDict[0x03002930] = LoadGraphicsBlock(0x370, 1, 0x40);
                memAddressDict[0x030022EC] = LoadGraphicsBlock(0x00000371, 1, 0x40);
                memAddressDict[0x03002EF0] = LoadGraphicsBlock(0x00000373, 1, 0x40);
                memAddressDict[0x03004044] = LoadGraphicsBlock(0x378, 3, 0x40);
                memAddressDict[0x030051C4] = LoadGraphicsBlock(0x374, 1, 0x40);
                memAddressDict[0x03002E70] = LoadGraphicsBlock(0x00000375, 1, 0x40);
                memAddressDict[0x03002884] = LoadGraphicsBlock(0x00000376, 1, 0x40);
                memAddressDict[0x03002888] = LoadGraphicsBlock(0x00000377, 1, 0x40);
                memAddressDict[0x0300509C] = LoadGraphicsBlock(0x0000037A, 1, 0x40);
                memAddressDict[0x0300256C] = LoadGraphicsBlock(0x0000037B, 8, 0x40);
                memAddressDict[0x03003F7C] = LoadGraphicsBlock(0x37c, 8, 0x40);
                memAddressDict[0x030023E0] = LoadGraphicsBlock(0x0000037E, 3, 0x40);
                memAddressDict[0x0300519C] = LoadGraphicsBlock(0x0000037F, 2, 0x40);
                memAddressDict[0x03004398] = LoadGraphicsBlock(0x380, 3, 0x40);
                memAddressDict[0x03004028] = LoadGraphicsBlock(0x00000381, 2, 0x40);
                memAddressDict[0x03004394] = LoadGraphicsBlock(0x00000382, 3, 0x40);
                memAddressDict[0x030025E0] = LoadGraphicsBlock(0x00000383, 2, 0x40);
                memAddressDict[0x03002DF0] = LoadGraphicsBlock(900, 3, 0x40);
                memAddressDict[0x03002948] = LoadGraphicsBlock(0x00000385, 2, 0x40);
                memAddressDict[0x0300522C] = LoadGraphicsBlock(0x00000386, 3, 0x40);
                memAddressDict[0x030022D4] = LoadGraphicsBlock(0x00000387, 2, 0x40);
                memAddressDict[0x03004FA8] = LoadGraphicsBlock(0x388, 4, 0x40);
            }
            if (level == 1 || level == 6 || level == 11 || level == 16 || level == 22) {
                memAddressDict[0x0300290C] = LoadGraphicsBlock(0x00000291, 9, 0x40);
                memAddressDict[0x03003FF0] = LoadGraphicsBlock(0x00000292, 9, 0x40);
                memAddressDict[0x03004F7C] = LoadGraphicsBlock(0x0000034F, 8, 0x40);
                memAddressDict[0x03004360] = LoadGraphicsBlock(0x00000351, 8, 0x20);
                memAddressDict[0x03002DF4] = LoadGraphicsBlock(0x00000353, 7, 0x20);
                memAddressDict[0x030023DC] = LoadGraphicsBlock(0x00000355, 3, 0x20);
                memAddressDict[0x03002564] = LoadGraphicsBlock(0x00000357, 4, 0x20);
                memAddressDict[0x03005250] = LoadGraphicsBlock(0x00000359, 1, 0x40);
                memAddressDict[0x030025B4] = LoadGraphicsBlock(0x0000035A, 2, 0x40);
                memAddressDict[0x03004FD4] = LoadGraphicsBlock(0x0000035B, 2, 0x40);
                memAddressDict[0x03002E98] = LoadGraphicsBlock(0x0000035D, 7, 0x40);
                memAddressDict[0x03002480] = LoadGraphicsBlock(0x0000035E, 8, 0x40);
                memAddressDict[0x030051B4] = LoadGraphicsBlock(0x0000035F, 2, 0x40);
                memAddressDict[0x03004F78] = LoadGraphicsBlock(0x360, 3, 0x40);
                memAddressDict[0x0300231C] = LoadGraphicsBlock(0x00000361, 0xe, 0x40);
                memAddressDict[0x0300289C] = LoadGraphicsBlock(0x00000363, 8, 0x40);
                memAddressDict[0x03002E38] = LoadGraphicsBlock(0x364, 8, 0x40);
                memAddressDict[0x0300291C] = LoadGraphicsBlock(0x00000366, 6, 0x40);
                memAddressDict[0x03004040] = LoadGraphicsBlock(0x000002C3, 2, 0x20);
                memAddressDict[0x03005220] = LoadGraphicsBlock(0x000002C1, 2, 0x40);
            }
            memAddressDict[0x03002924] = LoadGraphicsBlock(0x118, 0xf, 0x40);
            memAddressDict[0x0300507C] = LoadGraphicsBlock(0x00000119, 0xf, 0x40);
            memAddressDict[0x030030B0] = LoadGraphicsBlock(0x000002F1, 2, 0x20);
            memAddressDict[0x03002DE4] = LoadGraphicsBlock(0x000002F9, 10, 0x20);
            memAddressDict[0x03002DF8] = LoadGraphicsBlock(0x000002FB, 0x2d, 0x10);
            memAddressDict[0x030028D0] = LoadGraphicsBlock(0x41, 1, 0x10);
            memAddressDict[0x030028CC] = LoadGraphicsBlock(0x43, 1, 0x10);

            memAddressDict[0x030022C0] = LoadGraphicsBlock(0x3f, 1, 0x10);
            memAddressDict[0x0300234C] = LoadGraphicsBlock(0x00000149, 1, 0x20);
            memAddressDict[0x030022B8] = LoadGraphicsBlock(0x0000014B, 0x16, 0x20);
            memAddressDict[0x03004020] = LoadGraphicsBlock(0x00000302, 7, 0x40);


            if (level != 6 && level != 0xb && level != 0x10 && level != 0x16 && level != 0x1b) {
                if (level == 0x1c || level == 1) {
                    memAddressDict[0x030022C0] = LoadGraphicsBlock(0x3f, 1, 0x10);
                    memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                    memAddressDict[0x0300372C] = LoadGraphicsBlock(0x0000019D, 0x1d, 0x40);
                    memAddressDict[0x03005204] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                    memAddressDict[0x030028C4] = LoadGraphicsBlock(0x19e, 0x21, 0x40);
                    memAddressDict[0x0300258C] = LoadGraphicsBlock(0x0000019F, 10, 0x40);
                    memAddressDict[0x03004378] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                    memAddressDict[0x03002F0C] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
                } else {
                    memAddressDict[0x03002324] = LoadGraphicsBlock(0x00000141, 8, 0x40);
                    memAddressDict[0x030051F8] = LoadGraphicsBlock(0x142, 8, 0x40);
                    memAddressDict[0x030025DC] = LoadGraphicsBlock(0x00000143, 3, 0x40);
                    memAddressDict[0x03004358] = LoadGraphicsBlock(0x144, 5, 0x40);
                    memAddressDict[0x03004264] = LoadGraphicsBlock(0x00000145, 8, 0x40);
                    memAddressDict[0x03002368] = LoadGraphicsBlock(0x146, 8, 0x40);
                    memAddressDict[0x03003FF4] = LoadGraphicsBlock(0x00000147, 6, 0x40);
                    memAddressDict[0x0300372C] = LoadGraphicsBlock(0x196, 9, 0x40);
                    memAddressDict[0x03005204] = LoadGraphicsBlock(0x00000197, 0xb, 0x40);
                    memAddressDict[0x030028C4] = LoadGraphicsBlock(0x198, 7, 0x40);
                    memAddressDict[0x0300258C] = LoadGraphicsBlock(0x00000199, 10, 0x40);
                    memAddressDict[0x03004378] = LoadGraphicsBlock(0x19a, 6, 0x40);
                    memAddressDict[0x03002F0C] = LoadGraphicsBlock(0x0000019B, 8, 0x40);
                }
                if (world < 6) {
                    switch (world) {
                        case 0:
                            if (level != 0x1d && level != 0x1f) {
                                memAddressDict[0x03002958] = LoadGraphicsBlock(0x00000187, 1, 0x40);
                                memAddressDict[0x030050F0] = LoadGraphicsBlock(0x0000018B, 1, 0x40);
                                memAddressDict[0x03002E04] = LoadGraphicsBlock(0x00000189, 7, 0x40);
                                memAddressDict[0x03004FB8] = LoadGraphicsBlock(0x00000151, 10, 0x40);
                                memAddressDict[0x030023BC] = LoadGraphicsBlock(0x1a4, 6, 0x40);
                                memAddressDict[0x030024A4] = LoadGraphicsBlock(0x000001A5, 4, 0x40);
                                memAddressDict[0x030024C8] = LoadGraphicsBlock(0x000002C5, 6, 0x40);
                                memAddressDict[0x03002EBC] = LoadGraphicsBlock(0x174, 5, 0x20);
                                memAddressDict[0x03004354] = LoadGraphicsBlock(0x00000272, 8, 0x40);
                                memAddressDict[0x03003F54] = LoadGraphicsBlock(0x274, 8, 0x40);
                                memAddressDict[0x030022D8] = LoadGraphicsBlock(0x00000159, 0xe, 0x20);
                                memAddressDict[0x03003FC8] = LoadGraphicsBlock(0x00000281, 0x29, 0x40);
                                memAddressDict[0x0300290C] = LoadGraphicsBlock(0x00000291, 9, 0x40);
                                memAddressDict[0x03003FF0] = LoadGraphicsBlock(0x00000292, 9, 0x40);
                                memAddressDict[0x03002E40] = LoadGraphicsBlock(0x0000028B, 0xc, 0x40);
                                memAddressDict[0x030030A4] = LoadGraphicsBlock(0x0000028D, 0xc, 0x20);
                                memAddressDict[0x0300514C] = LoadGraphicsBlock(0x0000028F, 8, 0x40);
                                memAddressDict[0x03002540] = LoadGraphicsBlock(0x00000296, 0x16, 0x20);
                                memAddressDict[0x0300232C] = LoadGraphicsBlock(0x294, 0x1f, 0x20);
                                memAddressDict[0x03004024] = LoadGraphicsBlock(0x298, 0xe, 0x20);
                                memAddressDict[0x03004388] = LoadGraphicsBlock(0x00000299, 0x18, 0x20);
                                memAddressDict[0x03003FC0] = LoadGraphicsBlock(0x000001A7, 10, 0x40);
                                memAddressDict[0x030024E0] = LoadGraphicsBlock(0x1a8, 0x13, 0x40);
                                memAddressDict[0x030022BC] = LoadGraphicsBlock(0x000001A9, 6, 0x40);
                                memAddressDict[0x030051D0] = LoadGraphicsBlock(0x1aa, 0x12, 0x40);
                                memAddressDict[0x03004FD0] = LoadGraphicsBlock(0x000001AB, 7, 0x40);
                                memAddressDict[0x030028F8] = LoadGraphicsBlock(0x000001AD, 4, 0x40);

                                memAddressDict[0x0300524C] = LoadGraphicsBlock(0x1af, 8, 0x40);
                                memAddressDict[0x0300521C] = LoadGraphicsBlock(0x000001B5, 9, 0x40);
                                memAddressDict[0x03004348] = LoadGraphicsBlock(0x1b4, 3, 0x40);
                                memAddressDict[0x03005164] = LoadGraphicsBlock(0x000001B3, 0x10, 0x40);
                                memAddressDict[0x030022D0] = LoadGraphicsBlock(0x1b0, 4, 0x40);
                                memAddressDict[0x030025D0] = LoadGraphicsBlock(0x1b2, 10, 0x40);

                                memAddressDict[0x03002E68] = LoadGraphicsBlock(0x000001B7, 0xd, 0x40);
                                memAddressDict[0x030030AC] = LoadGraphicsBlock(0x1ba, 6, 0x40);
                                memAddressDict[0x030029B4] = LoadGraphicsBlock(0x000001BB, 9, 0x40);
                                memAddressDict[0x0300520C] = LoadGraphicsBlock(0x1b8, 0xb, 0x40);
                                memAddressDict[0x03002360] = LoadGraphicsBlock(0x000001B9, 0xb, 0x40);
                                memAddressDict[0x03003F64] = LoadGraphicsBlock(0x1bc, 5, 0x40);
                                memAddressDict[0x030051CC] = LoadGraphicsBlock(0x000001BD, 2, 0x40);

                                memAddressDict[0x03003F78] = LoadGraphicsBlock(0x000001C7, 0xb, 0x40);
                                memAddressDict[0x030051A4] = LoadGraphicsBlock(0x1c8, 0xc, 0x40);
                                memAddressDict[0x03002EE4] = LoadGraphicsBlock(0x000001C9, 0xc, 0x40);
                                memAddressDict[0x030050AC] = LoadGraphicsBlock(0x1ca, 10, 0x40);
                                memAddressDict[0x03005110] = LoadGraphicsBlock(0x000001CB, 0x10, 0x40);
                                memAddressDict[0x03002ED8] = LoadGraphicsBlock(0x1cc, 0x23, 0x40);
                                memAddressDict[0x030050F4] = LoadGraphicsBlock(0x000001CD, 0x1b, 0x40);

                                memAddressDict[0x03002378] = LoadGraphicsBlock(0x000001BF, 0xb, 0x40);
                                memAddressDict[0x030023A0] = LoadGraphicsBlock(0x000001C1, 9, 0x40);
                                memAddressDict[0x030025A8] = LoadGraphicsBlock(0x1c2, 10, 0x40);
                                memAddressDict[0x030025C8] = LoadGraphicsBlock(0x000001C3, 7, 0x40);
                                memAddressDict[0x030022E8] = LoadGraphicsBlock(0x1c4, 10, 0x40);
                                memAddressDict[0x03002E48] = LoadGraphicsBlock(0x1c0, 0x10, 0x40);
                                memAddressDict[0x03003FAC] = LoadGraphicsBlock(0x000001C5, 4, 0x40);

                                memAddressDict[0x030029C8] = LoadGraphicsBlock(0x1d0, 0x10, 0x40);
                                memAddressDict[0x03002F04] = LoadGraphicsBlock(0x000001CF, 0x19, 0x40);
                                memAddressDict[0x03002950] = LoadGraphicsBlock(0x000001D1, 10, 0x40);
                                memAddressDict[0x03002E8C] = LoadGraphicsBlock(0x1d4, 2, 0x40);
                                memAddressDict[0x03002940] = LoadGraphicsBlock(0x1d2, 5, 0x40);
                                memAddressDict[0x0300288C] = LoadGraphicsBlock(0x000001D3, 10, 0x40);
                            }
                            memAddressDict[0x03005150] = LoadGraphicsBlock(0x000002E1, 6, 0x40);
                            memAddressDict[0x03003730] = LoadGraphicsBlock(0x000002E2, 8, 0x40);
                            memAddressDict[0x03003FBC] = LoadGraphicsBlock(0x2e4, 6, 0x40);
                            memAddressDict[0x030023C0] = LoadGraphicsBlock(0x000002E5, 8, 0x40);
                            memAddressDict[0x03002E9C] = LoadGraphicsBlock(0x000002E7, 6, 0x40);
                            memAddressDict[0x03004FF4] = LoadGraphicsBlock(0x2e8, 8, 0x40);
                            memAddressDict[0x0300431C] = LoadGraphicsBlock(0x000002ED, 2, 0x20);
                            memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                            memAddressDict[0x030030B0] = LoadGraphicsBlock(0x000002F1, 2, 0x20);
                            memAddressDict[0x03004018] = LoadGraphicsBlock(0x000002F3, 6, 0x40);
                            memAddressDict[0x03005244] = LoadGraphicsBlock(0x2f4, 1, 0x40);
                            memAddressDict[0x030051E4] = LoadGraphicsBlock(0x000002F5, 8, 0x40);
                            memAddressDict[0x030051BC] = LoadGraphicsBlock(0x000002F7, 6, 0x20);
                            break;
                        case 1:
                            memAddressDict[0x03004270] = LoadGraphicsBlock(0x17c, 4, 0x40);
                            memAddressDict[0x030023CC] = LoadGraphicsBlock(0x00000265, 0xc, 0x20);
                            memAddressDict[0x030028B4] = LoadGraphicsBlock(0x00000266, 6, 0x10);
                            memAddressDict[0x030023F0] = LoadGraphicsBlock(0x268, 6, 0x20);
                            memAddressDict[0x03002DEC] = LoadGraphicsBlock(0x26c, 4, 0x10);
                            memAddressDict[0x03002E5C] = LoadGraphicsBlock(0x0000026D, 4, 0x10);
                            memAddressDict[0x03004FC8] = LoadGraphicsBlock(0x0000026E, 4, 0x10);
                            memAddressDict[0x03002370] = LoadGraphicsBlock(0x17e, 7, 0x40);
                            memAddressDict[0x03004FF0] = LoadGraphicsBlock(0x00000183, 1, 0x40);
                            memAddressDict[0x030024DC] = LoadGraphicsBlock(0x00000185, 1, 0x10);
                            memAddressDict[0x03002908] = LoadGraphicsBlock(0x0000014F, 10, 0x40);
                            memAddressDict[0x03002EB0] = LoadGraphicsBlock(0x304, 7, 0x20);
                            memAddressDict[0x03002904] = LoadGraphicsBlock(0x00000193, 0xb, 0x40);
                            memAddressDict[0x030051F0] = LoadGraphicsBlock(0x194, 7, 0x40);
                            memAddressDict[0x03005188] = LoadGraphicsBlock(0x000002A9, 0x14, 0x40);
                            memAddressDict[0x03002300] = LoadGraphicsBlock(0x000002A7, 0x18, 0x20);
                            memAddressDict[0x03002E58] = LoadGraphicsBlock(0x0000029F, 0x15, 0x20);
                            memAddressDict[0x030042F8] = LoadGraphicsBlock(0x1d6, 5, 0x40);
                            memAddressDict[0x030023FC] = LoadGraphicsBlock(0x000001D7, 6, 0x40);
                            memAddressDict[0x030051DC] = LoadGraphicsBlock(0x1d8, 0xb, 0x40);
                            memAddressDict[0x03004FE8] = LoadGraphicsBlock(0x000001D9, 10, 0x40);
                            memAddressDict[0x03002354] = LoadGraphicsBlock(0x1da, 0x15, 0x40);
                            memAddressDict[0x03004F68] = LoadGraphicsBlock(0x000001DB, 6, 0x40);

                            memAddressDict[0x03002F14] = LoadGraphicsBlock(0x000001E9, 10, 0x40);
                            memAddressDict[0x0300293C] = LoadGraphicsBlock(0x1ea, 10, 0x40);
                            memAddressDict[0x03002920] = LoadGraphicsBlock(0x000001EB, 10, 0x40);
                            memAddressDict[0x030023EC] = LoadGraphicsBlock(0x000001ED, 8, 0x40);
                            memAddressDict[0x03004368] = LoadGraphicsBlock(0x1ec, 9, 0x40);

                            memAddressDict[0x03002568] = LoadGraphicsBlock(0x1f0, 6, 0x40);
                            memAddressDict[0x03002E50] = LoadGraphicsBlock(0x000001F1, 0x10, 0x40);
                            memAddressDict[0x03002F18] = LoadGraphicsBlock(0x000001F3, 0x10, 0x40);
                            memAddressDict[0x0300433C] = LoadGraphicsBlock(0x000001EF, 7, 0x40);
                            memAddressDict[0x03002504] = LoadGraphicsBlock(500, 9, 0x40);
                            memAddressDict[0x0300438C] = LoadGraphicsBlock(0x1f2, 7, 0x40);
                            memAddressDict[0x03005224] = LoadGraphicsBlock(0x000001F7, 0x10, 0x40);
                            memAddressDict[0x030043A0] = LoadGraphicsBlock(0x1f8, 6, 0x40);
                            memAddressDict[0x0300503C] = LoadGraphicsBlock(0x1f6, 0x10, 0x40);
                            memAddressDict[0x0300511C] = LoadGraphicsBlock(0x1fa, 2, 0x40);

                            memAddressDict[0x030025E4] = LoadGraphicsBlock(0x000001E5, 6, 0x40);
                            memAddressDict[0x030024FC] = LoadGraphicsBlock(0x1e4, 3, 0x40);
                            memAddressDict[0x030030C4] = LoadGraphicsBlock(0x000001E1, 10, 0x40);
                            memAddressDict[0x03004FB0] = LoadGraphicsBlock(0x000001DF, 7, 0x40);
                            memAddressDict[0x030051C0] = LoadGraphicsBlock(0x1e0, 3, 0x40);
                            memAddressDict[0x03002EE0] = LoadGraphicsBlock(0x1e2, 8, 0x40);
                            memAddressDict[0x03002870] = LoadGraphicsBlock(0x000001DD, 3, 0x40);
                            memAddressDict[0x030023B8] = LoadGraphicsBlock(0x000001E3, 5, 0x40);
                            memAddressDict[0x030051A0] = LoadGraphicsBlock(0x1de, 5, 0x40);
                            memAddressDict[0x030022B0] = LoadGraphicsBlock(0x000001E7, 4, 0x40);

                            memAddressDict[0x03002378] = LoadGraphicsBlock(0x000001BF, 0xb, 0x40);
                            memAddressDict[0x030023A0] = LoadGraphicsBlock(0x000001C1, 9, 0x40);
                            memAddressDict[0x030025A8] = LoadGraphicsBlock(0x1c2, 10, 0x40);
                            memAddressDict[0x030025C8] = LoadGraphicsBlock(0x000001C3, 7, 0x40);
                            memAddressDict[0x030022E8] = LoadGraphicsBlock(0x1c4, 10, 0x40);
                            memAddressDict[0x03002E48] = LoadGraphicsBlock(0x1c0, 0x10, 0x40);
                            memAddressDict[0x03003FAC] = LoadGraphicsBlock(0x000001C5, 4, 0x40);

                            memAddressDict[0x03004318] = LoadGraphicsBlock(0x1fc, 0xe, 0x40);
                            memAddressDict[0x03005128] = LoadGraphicsBlock(0x000001FD, 0x18, 0x40);

                            memAddressDict[0x030029B8] = LoadGraphicsBlock(0x000001FF, 10, 0x40);
                            memAddressDict[0x03005248] = LoadGraphicsBlock(0x200, 5, 0x40);

                            memAddressDict[0x030029C8] = LoadGraphicsBlock(0x1d0, 0x10, 0x40);
                            memAddressDict[0x03002F04] = LoadGraphicsBlock(0x000001CF, 0x19, 0x40);
                            memAddressDict[0x03002950] = LoadGraphicsBlock(0x000001D1, 10, 0x40);
                            memAddressDict[0x03002E8C] = LoadGraphicsBlock(0x1d4, 2, 0x40);
                            memAddressDict[0x03002940] = LoadGraphicsBlock(0x1d2, 5, 0x40);
                            memAddressDict[0x0300288C] = LoadGraphicsBlock(0x000001D3, 10, 0x40);
                            break;
                        case 2:
                            memAddressDict[0x0300292C] = LoadGraphicsBlock(0x000002BA, 4, 0x40);
                            memAddressDict[0x03002900] = LoadGraphicsBlock(0x000002BB, 8, 0x40);
                            memAddressDict[0x030030CC] = LoadGraphicsBlock(700, 2, 0x40);
                            memAddressDict[0x03003F70] = LoadGraphicsBlock(0x000002BD, 4, 0x40);
                            memAddressDict[0x03002ED0] = LoadGraphicsBlock(0x00000203, 8, 0x40);
                            memAddressDict[0x03003F50] = LoadGraphicsBlock(0x00000205, 0xb, 0x40);
                            memAddressDict[0x03002E88] = LoadGraphicsBlock(0x204, 8, 0x40);
                            memAddressDict[0x0300510C] = LoadGraphicsBlock(0x00000202, 0xb, 0x40);
                            memAddressDict[0x0300251C] = LoadGraphicsBlock(0x00000206, 6, 0x40);

                            memAddressDict[0x030022E0] = LoadGraphicsBlock(0x0000020A, 3, 0x40);
                            memAddressDict[0x03002458] = LoadGraphicsBlock(0x208, 4, 0x40);
                            memAddressDict[0x030042E8] = LoadGraphicsBlock(0x00000209, 0xb, 0x40);
                            memAddressDict[0x03004FCC] = LoadGraphicsBlock(0x0000020B, 7, 0x40);
                            memAddressDict[0x03005028] = LoadGraphicsBlock(0x20c, 0xb, 0x40);
                            memAddressDict[0x03002588] = LoadGraphicsBlock(0x0000020D, 3, 0x40);
                            memAddressDict[0x03004F74] = LoadGraphicsBlock(0x0000020F, 4, 0x40);

                            memAddressDict[0x03004F94] = LoadGraphicsBlock(0x00000217, 0xe, 0x40);
                            memAddressDict[0x03002310] = LoadGraphicsBlock(0x218, 9, 0x40);
                            memAddressDict[0x03005070] = LoadGraphicsBlock(0x0000021B, 0x11, 0x40);
                            memAddressDict[0x03002EE8] = LoadGraphicsBlock(0x00000219, 0x14, 0x40);
                            memAddressDict[0x03005034] = LoadGraphicsBlock(0x0000021A, 0xb, 0x40);

                            memAddressDict[0x030025CC] = LoadGraphicsBlock(0x00000211, 0x14, 0x40);
                            memAddressDict[0x03002340] = LoadGraphicsBlock(0x00000213, 9, 0x40);
                            memAddressDict[0x03003098] = LoadGraphicsBlock(0x214, 0x11, 0x40);
                            memAddressDict[0x03002474] = LoadGraphicsBlock(0x00000215, 0x16, 0x40);

                            memAddressDict[0x03002464] = LoadGraphicsBlock(0x0000021E, 8, 0x40);
                            memAddressDict[0x03002EDC] = LoadGraphicsBlock(0x0000021F, 0x11, 0x40);
                            memAddressDict[0x030051E8] = LoadGraphicsBlock(0x0000021D, 0x13, 0x40);
                            memAddressDict[0x030051F4] = LoadGraphicsBlock(0x220, 0xb, 0x40);
                            memAddressDict[0x030029C4] = LoadGraphicsBlock(0x00000221, 9, 0x40);
                            memAddressDict[0x03002944] = LoadGraphicsBlock(0x00000222, 8, 0x40);

                            memAddressDict[0x03005140] = LoadGraphicsBlock(0x178, 4, 0x40);
                            memAddressDict[0x03004FE0] = LoadGraphicsBlock(0x17a, 7, 0x40);
                            memAddressDict[0x030030B4] = LoadGraphicsBlock(0x000002CA, 0x30, 0x20);
                            memAddressDict[0x03005220] = LoadGraphicsBlock(0x000002C1, 2, 0x40);
                            memAddressDict[0x03003088] = LoadGraphicsBlock(0x0000030D, 8, 0x20);
                            memAddressDict[0x030025B8] = LoadGraphicsBlock(0x0000030E, 8, 0x20);
                            memAddressDict[0x030024B8] = LoadGraphicsBlock(0x0000030F, 8, 0x40);
                            memAddressDict[0x0300435C] = LoadGraphicsBlock(0x000002A3, 1, 0x40);
                            memAddressDict[0x0300245C] = LoadGraphicsBlock(0x000002A5, 1, 0x40);
                            memAddressDict[0x030030C8] = LoadGraphicsBlock(0x2a0, 0x16, 0x20);
                            memAddressDict[0x03002E58] = LoadGraphicsBlock(0x0000029F, 0x15, 0x20);
                            break;
                        case 3:
                            if (level != 0x1d && level != 0x1f) {
                                memAddressDict[0x03004354] = LoadGraphicsBlock(0x00000272, 8, 0x40);
                                memAddressDict[0x03003F54] = LoadGraphicsBlock(0x274, 8, 0x40);
                                memAddressDict[0x03003FC8] = LoadGraphicsBlock(0x00000281, 0x29, 0x40);
                                memAddressDict[0x0300523C] = LoadGraphicsBlock(0x0000018D, 1, 0x40);
                                memAddressDict[0x03002444] = LoadGraphicsBlock(0x00000153, 10, 0x40);
                                memAddressDict[0x03005200] = LoadGraphicsBlock(0x0000018F, 7, 0x40);
                                memAddressDict[0x03002334] = LoadGraphicsBlock(0x270, 1, 0x40);
                                memAddressDict[0x030050A0] = LoadGraphicsBlock(0x00000155, 1, 0x40);
                                memAddressDict[0x03004040] = LoadGraphicsBlock(0x000002C3, 2, 0x20);
                                memAddressDict[0x030028DC] = LoadGraphicsBlock(0x2cc, 0x1e, 0x20);
                                memAddressDict[0x03002ED4] = LoadGraphicsBlock(0x00000157, 0xe, 0x20);
                                memAddressDict[0x030042EC] = LoadGraphicsBlock(0x000002B2, 8, 0x40);
                                memAddressDict[0x03003720] = LoadGraphicsBlock(0x0000015B, 2, 0x40);
                                memAddressDict[0x03003F5C] = LoadGraphicsBlock(0x000002AB, 0x1b, 0x20);
                                memAddressDict[0x03005188] = LoadGraphicsBlock(0x000002A9, 0x14, 0x40);
                                memAddressDict[0x03005190] = LoadGraphicsBlock(0x224, 6, 0x40);
                                memAddressDict[0x030022FC] = LoadGraphicsBlock(0x00000225, 7, 0x40);
                                memAddressDict[0x03004FC0] = LoadGraphicsBlock(0x00000226, 7, 0x40);
                                memAddressDict[0x030051FC] = LoadGraphicsBlock(0x00000227, 0x10, 0x40);
                                memAddressDict[0x030023A8] = LoadGraphicsBlock(0x228, 0xf, 0x40);
                                memAddressDict[0x030028FC] = LoadGraphicsBlock(0x0000022A, 8, 0x40);
                                memAddressDict[0x030050E8] = LoadGraphicsBlock(0x0000022B, 8, 0x40);
                                memAddressDict[0x03003F84] = LoadGraphicsBlock(0x00000229, 6, 0x40);
                                memAddressDict[0x030023D0] = LoadGraphicsBlock(0x0000022D, 4, 0x40);

                                memAddressDict[0x030023B0] = LoadGraphicsBlock(0x247, 8, 0x40);
                                memAddressDict[0x0300239C] = LoadGraphicsBlock(0x0000024A, 0xd, 0x40);
                                memAddressDict[0x03004F70] = LoadGraphicsBlock(0x00000249, 8, 0x40);
                                memAddressDict[0x03005038] = LoadGraphicsBlock(0x248, 8, 0x40);
                                memAddressDict[0x030029D0] = LoadGraphicsBlock(0x0000024B, 4, 0x40);
                                memAddressDict[0x03003738] = LoadGraphicsBlock(0x24c, 4, 0x40);

                                memAddressDict[0x03003F74] = LoadGraphicsBlock(0x00000239, 0x10, 0x40);
                                memAddressDict[0x03003090] = LoadGraphicsBlock(0x0000023A, 0xd, 0x40);
                                memAddressDict[0x03005054] = LoadGraphicsBlock(0x0000023E, 9, 0x40);
                                memAddressDict[0x030043A4] = LoadGraphicsBlock(0x0000023D, 6, 0x40);
                                memAddressDict[0x0300506C] = LoadGraphicsBlock(0x0000023F, 7, 0x40);
                                memAddressDict[0x030025C0] = LoadGraphicsBlock(0x23c, 0xe, 0x40);

                                memAddressDict[0x030028E8] = LoadGraphicsBlock(0x00000241, 8, 0x40);
                                memAddressDict[0x030050FC] = LoadGraphicsBlock(0x00000242, 9, 0x40);
                                memAddressDict[0x030023E8] = LoadGraphicsBlock(0x00000243, 8, 0x40);
                                memAddressDict[0x030028E4] = LoadGraphicsBlock(0x244, 10, 0x40);
                                memAddressDict[0x03005214] = LoadGraphicsBlock(0x00000245, 0x16, 0x40);

                                memAddressDict[0x03002498] = LoadGraphicsBlock(0x00000231, 0x1c, 0x40);
                                memAddressDict[0x0300257C] = LoadGraphicsBlock(0x00000232, 0x12, 0x40);
                                memAddressDict[0x0300233C] = LoadGraphicsBlock(0x234, 0xc, 0x40);
                                memAddressDict[0x03002488] = LoadGraphicsBlock(0x00000235, 0xd, 0x40);
                                memAddressDict[0x03005098] = LoadGraphicsBlock(0x00000233, 0x10, 0x40);
                                memAddressDict[0x03003FD8] = LoadGraphicsBlock(0x00000237, 1, 0x40);

                                memAddressDict[0x03002378] = LoadGraphicsBlock(0x000001BF, 0xb, 0x40);
                                memAddressDict[0x030023A0] = LoadGraphicsBlock(0x000001C1, 9, 0x40);
                                memAddressDict[0x030025A8] = LoadGraphicsBlock(0x1c2, 10, 0x40);
                                memAddressDict[0x030025C8] = LoadGraphicsBlock(0x000001C3, 7, 0x40);
                                memAddressDict[0x030022E8] = LoadGraphicsBlock(0x1c4, 10, 0x40);
                                memAddressDict[0x03002E48] = LoadGraphicsBlock(0x1c0, 0x10, 0x40);
                                memAddressDict[0x03003FAC] = LoadGraphicsBlock(0x000001C5, 4, 0x40);
                            }
                            memAddressDict[0x03005150] = LoadGraphicsBlock(0x000002E1, 6, 0x40);
                            memAddressDict[0x03003730] = LoadGraphicsBlock(0x000002E2, 8, 0x40);
                            memAddressDict[0x03003FBC] = LoadGraphicsBlock(0x2e4, 6, 0x40);
                            memAddressDict[0x030023C0] = LoadGraphicsBlock(0x000002E5, 8, 0x40);
                            memAddressDict[0x03002E9C] = LoadGraphicsBlock(0x000002E7, 6, 0x40);
                            memAddressDict[0x03004FF4] = LoadGraphicsBlock(0x2e8, 8, 0x40);
                            memAddressDict[0x0300431C] = LoadGraphicsBlock(0x000002ED, 2, 0x20);
                            memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
                            memAddressDict[0x030030B0] = LoadGraphicsBlock(0x000002F1, 2, 0x20);
                            memAddressDict[0x03004018] = LoadGraphicsBlock(0x000002F3, 6, 0x40);
                            memAddressDict[0x03005244] = LoadGraphicsBlock(0x2f4, 1, 0x40);
                            memAddressDict[0x030051E4] = LoadGraphicsBlock(0x000002F5, 8, 0x40);
                            memAddressDict[0x030051BC] = LoadGraphicsBlock(0x000002F7, 6, 0x20);
                            break;
                        case 4:
                            if (level != 1) {
                                memAddressDict[0x0300403C] = LoadGraphicsBlock(0x000002B3, 2, 0x20);
                                memAddressDict[0x03004F9C] = LoadGraphicsBlock(0x000002C6, 8, 0x40);
                                memAddressDict[0x03004390] = LoadGraphicsBlock(0x000002C7, 0x21, 0x20);
                                memAddressDict[0x03004FFC] = LoadGraphicsBlock(0x2c8, 0x18, 0x20);
                                memAddressDict[0x030050B0] = LoadGraphicsBlock(0x000002DF, 9, 0x40);
                                memAddressDict[0x0300439C] = LoadGraphicsBlock(0x000002DE, 4, 0x40);
                                memAddressDict[0x0300292C] = LoadGraphicsBlock(0x000002B5, 4, 0x40);
                                memAddressDict[0x03002900] = LoadGraphicsBlock(0x000002B6, 8, 0x40);
                                memAddressDict[0x030030CC] = LoadGraphicsBlock(0x000002B7, 2, 0x40);
                                memAddressDict[0x03003F70] = LoadGraphicsBlock(0x2b8, 4, 0x40);
                                memAddressDict[0x03005088] = LoadGraphicsBlock(0x13e, 4, 0x20);
                                memAddressDict[0x03005228] = LoadGraphicsBlock(0x000002BF, 2, 0x20);
                                memAddressDict[0x03003FCC] = LoadGraphicsBlock(0x00000191, 7, 0x40);
                                memAddressDict[0x03005114] = LoadGraphicsBlock(0x0000027A, 8, 0x40);
                                memAddressDict[0x03005090] = LoadGraphicsBlock(0x0000027B, 8, 0x40);
                                memAddressDict[0x030042F4] = LoadGraphicsBlock(0x27c, 8, 0x40);
                                memAddressDict[0x03004034] = LoadGraphicsBlock(0x0000027D, 8, 0x40);
                                memAddressDict[0x030023A4] = LoadGraphicsBlock(0x0000027E, 8, 0x40);
                                memAddressDict[0x03002500] = LoadGraphicsBlock(0x0000027F, 8, 0x40);
                                memAddressDict[0x030051B8] = LoadGraphicsBlock(0x00000283, 0xd, 0x40);
                                memAddressDict[0x03004330] = LoadGraphicsBlock(0x00000285, 6, 0x40);
                                memAddressDict[0x03002580] = LoadGraphicsBlock(0x00000287, 6, 0x40);
                                memAddressDict[0x03004338] = LoadGraphicsBlock(0x00000289, 0x1d, 0x40);
                                memAddressDict[0x0300232C] = LoadGraphicsBlock(0x294, 0x1f, 0x20);
                                memAddressDict[0x03002E58] = LoadGraphicsBlock(0x0000029F, 0x15, 0x20);
                                memAddressDict[0x0300501C] = LoadGraphicsBlock(0x0000024E, 0x15, 0x40);
                                memAddressDict[0x030028C0] = LoadGraphicsBlock(0x0000024F, 6, 0x40);
                                memAddressDict[0x03004320] = LoadGraphicsBlock(0x00000251, 6, 0x40);
                                memAddressDict[0x03003FFC] = LoadGraphicsBlock(0x250, 6, 0x40);
                                memAddressDict[0x03005144] = LoadGraphicsBlock(0x00000252, 8, 0x40);

                                memAddressDict[0x03002DE0] = LoadGraphicsBlock(0x0000025E, 6, 0x40);
                                memAddressDict[0x03002EC8] = LoadGraphicsBlock(0x25c, 0x17, 0x40);
                                memAddressDict[0x03002338] = LoadGraphicsBlock(0x0000025F, 0xc, 0x40);
                                memAddressDict[0x03004FF8] = LoadGraphicsBlock(0x0000025D, 8, 0x40);

                                memAddressDict[0x03002464] = LoadGraphicsBlock(0x0000021E, 8, 0x40);
                                memAddressDict[0x03002EDC] = LoadGraphicsBlock(0x0000021F, 0x11, 0x40);
                                memAddressDict[0x030051E8] = LoadGraphicsBlock(0x0000021D, 0x13, 0x40);
                                memAddressDict[0x030051F4] = LoadGraphicsBlock(0x220, 0xb, 0x40);

                                memAddressDict[0x03002ED0] = LoadGraphicsBlock(0x00000203, 8, 0x40);
                                memAddressDict[0x03003F50] = LoadGraphicsBlock(0x00000205, 0xb, 0x40);
                                memAddressDict[0x03002E88] = LoadGraphicsBlock(0x204, 8, 0x40);
                                memAddressDict[0x0300510C] = LoadGraphicsBlock(0x00000202, 0xb, 0x40);
                                memAddressDict[0x0300251C] = LoadGraphicsBlock(0x00000206, 6, 0x40);

                                memAddressDict[0x030024F0] = LoadGraphicsBlock(0x254, 0x15, 0x40);
                                memAddressDict[0x03002364] = LoadGraphicsBlock(0x00000255, 9, 0x40);
                                memAddressDict[0x03004FEC] = LoadGraphicsBlock(0x00000257, 0xc, 0x40);
                                memAddressDict[0x03002E7C] = LoadGraphicsBlock(0x00000256, 7, 0x40);
                                memAddressDict[0x030030C0] = LoadGraphicsBlock(600, 0xd, 0x40);
                                memAddressDict[0x03005060] = LoadGraphicsBlock(0x0000025A, 3, 0x40);
                            }
                            memAddressDict[0x03004340] = LoadGraphicsBlock(0x000002AD, 4, 0x40);
                            memAddressDict[0x03004010] = LoadGraphicsBlock(0x000002AE, 8, 0x40);
                            memAddressDict[0x030025AC] = LoadGraphicsBlock(0x2b0, 7, 0x40);
                            memAddressDict[0x03002514] = LoadGraphicsBlock(0x176, 1, 0x40);
                            memAddressDict[0x03002544] = LoadGraphicsBlock(0x00000261, 4, 0x20);
                            memAddressDict[0x030023B4] = LoadGraphicsBlock(0x00000263, 0x14, 0x20);
                            memAddressDict[0x030050EC] = LoadGraphicsBlock(0x0000026A, 8, 0x20);
                            memAddressDict[0x03004308] = LoadGraphicsBlock(0x0000026B, 4, 0x20);
                            memAddressDict[0x03003728] = LoadGraphicsBlock(0x00000269, 0xe, 0x40);

                            if (level == 1) {
                                memAddressDict[0x03002534] = LoadGraphicsBlock(0x0000033A, 0xe, 0x40);
                                memAddressDict[0x0300516C] = LoadGraphicsBlock(0x00000339, 7, 0x40);
                                memAddressDict[0x03003FC4] = LoadGraphicsBlock(0x0000029B, 8, 0x40);
                                memAddressDict[0x03005044] = LoadGraphicsBlock(0x29c, 9, 0x40);
                                memAddressDict[0x03002DFC] = LoadGraphicsBlock(0x0000029D, 8, 0x40);
                            }
                            break;
                        case 5:
                            memAddressDict[0x03002520] = LoadGraphicsBlock(0x182, 2, 0x10);
                            memAddressDict[0x03004270] = LoadGraphicsBlock(0x17c, 4, 0x40);
                            memAddressDict[0x030023CC] = LoadGraphicsBlock(0x00000265, 0xc, 0x20);
                            memAddressDict[0x030028B4] = LoadGraphicsBlock(0x00000266, 6, 0x10);
                            memAddressDict[0x030023F0] = LoadGraphicsBlock(0x268, 6, 0x20);
                            memAddressDict[0x03002DEC] = LoadGraphicsBlock(0x26c, 4, 0x10);
                            memAddressDict[0x03002E5C] = LoadGraphicsBlock(0x0000026D, 4, 0x10);
                            memAddressDict[0x03004FC8] = LoadGraphicsBlock(0x0000026E, 4, 0x10);
                            memAddressDict[0x03002370] = LoadGraphicsBlock(0x17e, 7, 0x40);
                            memAddressDict[0x03004FF0] = LoadGraphicsBlock(0x00000183, 1, 0x40);
                            memAddressDict[0x030024DC] = LoadGraphicsBlock(0x00000185, 1, 0x10);
                            memAddressDict[0x03004248] = LoadGraphicsBlock(0x00000276, 9, 0x40);
                            memAddressDict[0x03002348] = LoadGraphicsBlock(0x2d0, 1, 0x40);
                            memAddressDict[0x03004324] = LoadGraphicsBlock(0x2d4, 1, 0x40);
                            memAddressDict[0x03003724] = LoadGraphicsBlock(0x2d8, 1, 0x40);
                            memAddressDict[0x03002574] = LoadGraphicsBlock(0x2dc, 1, 0x40);
                            memAddressDict[0x030022F4] = LoadGraphicsBlock(0x278, 8, 0x10);
                            memAddressDict[0x0300518C] = LoadGraphicsBlock(0x0000015D, 5, 0x40);
                            memAddressDict[0x03005100] = LoadGraphicsBlock(0x00000161, 5, 0x20);
                            memAddressDict[0x03002E94] = LoadGraphicsBlock(0x0000015F, 5, 0x20);
                            memAddressDict[0x030050A4] = LoadGraphicsBlock(0x00000163, 5, 0x40);
                            memAddressDict[0x03003F60] = LoadGraphicsBlock(0x00000167, 5, 0x20);
                            memAddressDict[0x030029CC] = LoadGraphicsBlock(0x00000165, 5, 0x20);
                            memAddressDict[0x03002F1C] = LoadGraphicsBlock(0x172, 0x16, 0x40);
                            memAddressDict[0x03004260] = LoadGraphicsBlock(0x00000171, 0x16, 0x20);
                            memAddressDict[0x03002EB4] = LoadGraphicsBlock(0x170, 0x16, 0x10);

                            memAddressDict[0x03003F58] = LoadGraphicsBlock(/*0x03000010 + 0xb +*/ 0x00000169, 8, 0x40); // is this right?
                            break;
                    }
                    LoadGraphicsBlock(0x10, 1, 0x20); // What is this? It loads at the end of every world block and I have no idea at what address
                }
            }
            memAddressDict[0x03002E78] = LoadGraphicsBlock(0x000002EF, 7, 0x20);
            memAddressDict[0x0300372C] = LoadGraphicsBlock(0x0000019D, 0x1d, 0x40);
            memAddressDict[0x03005204] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
            memAddressDict[0x030028C4] = LoadGraphicsBlock(0x19e, 0x21, 0x40);
            memAddressDict[0x0300258C] = LoadGraphicsBlock(0x0000019F, 10, 0x40);
            memAddressDict[0x03004378] = LoadGraphicsBlock(0x000001A1, 10, 0x40);
            memAddressDict[0x03002F0C] = LoadGraphicsBlock(0x000001A1, 10, 0x40);

            return memAddressDict;
        }

        public void AssignActorValues(GBARRR_Actor actor, GBARRR_ROM rom, int level, int world) {
            if(level == 0x1b) world = 5;
            if(world > 5) return;
            GBARRR_GraphicsTableEntry graphicsEntry = rom.GraphicsTable0[world].LastOrDefault(e => e.Key * 2 == actor.Ushort_0C);
            if (graphicsEntry == null) return;
            actor.P_GraphicsIndex = graphicsEntry.Value;
            actor.P_30 = rom.GraphicsTable1[world][actor.P_GraphicsIndex];
            actor.P_AnimIndex = rom.GraphicsTable2[world][actor.P_GraphicsIndex];

            switch (world) {
                case 0:
					// 2 is always a big thing. We'll check that later
					#region World 0
					if (actor.P_GraphicsIndex == 3) {
                        actor.P_FunctionPointer = 0x08037B9D;
                    }
                    if (actor.P_GraphicsIndex == 4) {
                        actor.P_FunctionPointer = 0x08037CA1;
                    }
                    if (actor.P_GraphicsIndex == 5) {
                        actor.P_GraphicsOffset = 0x03005158;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 6) {
                        actor.P_FunctionPointer = 0x0804DC65;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 8) {
                        actor.P_GraphicsOffset = 0x0300401C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 9) {
                        actor.P_GraphicsOffset = 0x0300248C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 10) {
                        actor.P_GraphicsOffset = 0x03002374;
                        actor.P_PaletteIndex = 0x13a;
                    }
                    if (actor.P_GraphicsIndex == 0xb) {
                        actor.P_GraphicsOffset = 0x03005064;
                        actor.P_PaletteIndex = 0x13c;
                    }
                    if (actor.P_GraphicsIndex == 0x18) {
                        actor.P_PaletteIndex = 0x0000033B;
                        actor.P_Field0E = 0;
                        actor.P_Field08 = 0;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        //*(undefined4*)0x03005370 = 3;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x32) {
                        actor.P_GraphicsOffset = 0x0300290C;
                        actor.P_PaletteIndex = 0x290;
                        actor.P_30 = 0x40;
                    }
                    if (actor.P_GraphicsIndex == 0x1c) {
                        actor.P_GraphicsOffset = 0x03003FF0;
                        actor.P_PaletteIndex = 0x290;
                        actor.P_30 = 0x40;
                    }
                    if (actor.P_GraphicsIndex == 0x1e) {
                        actor.P_GraphicsOffset = 0x03002E40;
                        actor.P_PaletteIndex = 0x0000028A;
                        actor.P_30 = 0x40;
                    }
                    if (actor.P_GraphicsIndex == 0x43) {
                        actor.P_GraphicsOffset = 0x030030A4;
                        actor.P_PaletteIndex = 0x28c;
                        actor.P_30 = 0x20;
                    }
                    if (actor.P_GraphicsIndex == 0x1d) {
                        actor.P_GraphicsOffset = 0x0300514C;
                        actor.P_PaletteIndex = 0x0000028E;
                        actor.P_30 = 0x40;
                    }
                    if (actor.P_GraphicsIndex == 0x46) {
                        actor.P_GraphicsOffset = 0x03002540;
                        actor.P_PaletteIndex = 0x00000295;
                        actor.P_30 = 0x20;
                    }
                    if (actor.P_GraphicsIndex == 0x44) {
                        actor.P_GraphicsOffset = 0x03004024;
                        actor.P_PaletteIndex = 0x00000297;
                        actor.P_30 = 0x20;
                    }
                    if (actor.P_GraphicsIndex == 0x45) {
                        actor.P_GraphicsOffset = 0x03004388;
                        actor.P_PaletteIndex = 0x00000297;
                        actor.P_30 = 0x20;
                    }
                    if (actor.P_GraphicsIndex == 0x1f) {
                        actor.P_GraphicsOffset = 0x0300232C;
                        actor.P_PaletteIndex = 0x00000293;
                        actor.P_30 = 0x20;
                    }
                    if (actor.P_GraphicsIndex == 0x28) {
                        actor.P_GraphicsOffset = 0x030023BC;
                        actor.P_PaletteIndex = 0x000001A3;
                    }
                    if (actor.P_GraphicsIndex == 0x29) {
                        actor.P_GraphicsOffset = 0x030024C8;
                        actor.P_PaletteIndex = 0x2c4;
                    }
                    if (actor.P_GraphicsIndex == 0x2a) {
                        actor.P_GraphicsOffset = 0x030051D0;
                        actor.P_PaletteIndex = 0x1a6;
                        actor.P_SpriteHeight = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x2b) {
                        actor.P_GraphicsOffset = 0x0300524C;
                        actor.P_PaletteIndex = 0x1ae;
                        actor.P_SpriteHeight = 1;
                    }
                    if (actor.P_GraphicsIndex == 0x2c) {
                        actor.P_GraphicsOffset = 0x03002E68;
                        actor.P_PaletteIndex = 0x1b6;
                        actor.P_SpriteHeight = 2;
                    }
                    if (actor.P_GraphicsIndex == 0x2d) {
                        actor.P_GraphicsOffset = 0x03002378;
                        actor.P_PaletteIndex = 0x1be;
                        actor.P_SpriteHeight = 4;
                    }
                    if (actor.P_GraphicsIndex == 0x30) {
                        actor.P_GraphicsOffset = 0x03002958;
                        actor.P_PaletteIndex = 0x186;
                    }
                    if (actor.P_GraphicsIndex == 0x31) {
                        actor.P_GraphicsOffset = 0x030050F0;
                        actor.P_PaletteIndex = 0x18a;
                    }
                    if (actor.P_GraphicsIndex == 0x2e) {
                        actor.P_GraphicsOffset = 0x03002F0C;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 0x00000195;
                        actor.P_Field12 = 6;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        //*(Actor**)0x03004278 = actor;
                    }
                    if (actor.P_GraphicsIndex == 0x33) {
                        actor.P_GraphicsOffset = 0x03003F78;
                        actor.P_PaletteIndex = 0x1c6;
                        actor.P_SpriteHeight = 3;
                    }
                    if (actor.P_GraphicsIndex == 0x3f) {
                        actor.P_GraphicsOffset = 0x030029C8;
                        actor.P_PaletteIndex = 0x1ce;
                        actor.P_SpriteHeight = 5;
                    }
                    if (actor.P_GraphicsIndex == 0x34) {
                        actor.P_GraphicsOffset = 0x03002324;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x35) {
                        actor.P_GraphicsOffset = 0x03004264;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x36) {
                        actor.P_GraphicsOffset = 0x03004354;
                        actor.P_PaletteIndex = 0x00000271;
                    }
                    if (actor.P_GraphicsIndex == 0x37) {
                        actor.P_GraphicsOffset = 0x03003F54;
                        actor.P_PaletteIndex = 0x00000273;
                    }
                    if (actor.P_GraphicsIndex == 0x38) {
                        actor.P_GraphicsOffset = 0x03003FC8;
                        actor.P_PaletteIndex = 0x280;
                    }
                    if (actor.P_GraphicsIndex == 0x39) {
                        //0x030052AC = 0x030052AC + 1;
                        if (level == 0x1d) {
                            actor.P_GraphicsOffset = 0x03005150;
                            actor.P_PaletteIndex = 0x2e0;
                        } else {
                            actor.P_GraphicsOffset = 0x03003FBC;
                            actor.P_PaletteIndex = 0x2e3;
                        }
                        actor.P_Field12 = 0;
                        actor.P_Field2C = 0x50;
                    }
                    if (actor.P_GraphicsIndex == 0x3a) {
                        actor.P_GraphicsOffset = 0x03002E9C;
                        actor.P_PaletteIndex = 0x2e6;
                        actor.P_Field12 = 0;
                        actor.P_Field2C = 0x50;
                    }
                    if (actor.P_GraphicsIndex == 0x3b) {
                        actor.P_GraphicsOffset = 0x03002E04;
                        actor.P_PaletteIndex = 0x188;
                    }
                    if (actor.P_GraphicsIndex == 0x3c) {
                        actor.P_GraphicsOffset = 0x03004FB8;
                        actor.P_PaletteIndex = 0x150;
                    }
                    if (actor.P_GraphicsIndex == 0x3e) {
                        actor.P_GraphicsOffset = 0x03004F6C;
                        actor.P_PaletteIndex = 0x17f;
                    }
                    if (actor.P_GraphicsIndex == 0x40) {
                        actor.P_GraphicsOffset = 0x03002EBC;
                        actor.P_PaletteIndex = 0x173;
                        actor.P_Field10 = actor.RuntimeYPosition;
                    }
                    if (actor.P_GraphicsIndex == 0x41) {
                        actor.P_GraphicsOffset = 0x030022D8;
                        actor.P_PaletteIndex = 0x158;
                    }
                    if (actor.P_GraphicsIndex == 0x42) {
                        actor.P_GraphicsOffset = 0x030022B8;
                        actor.P_Field18 = 0x0300234C;
                        actor.P_Field20 = 100;
                        actor.P_PaletteIndex = 0x148;
                    }
					#endregion
					break;
                case 1:
                    // 2 is always a big thing. We'll check that later
                    #region World 1
                    if (actor.P_GraphicsIndex == 3) {
                        actor.P_FunctionPointer = 0x08037B9D;
                    }
                    if (actor.P_GraphicsIndex == 4) {
                        actor.P_FunctionPointer = 0x08037CA1;
                    }
                    if (actor.P_GraphicsIndex == 5) {
                        actor.P_GraphicsOffset = 0x03005158;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 6) {
                        actor.P_FunctionPointer = 0x0804DC65;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 8) {
                        actor.P_GraphicsOffset = 0x0300401C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 9) {
                        actor.P_GraphicsOffset = 0x0300248C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 10) {
                        actor.P_GraphicsOffset = 0x03002374;
                        actor.P_PaletteIndex = 0x13a;
                    }
                    if (actor.P_GraphicsIndex == 0xb) {
                        actor.P_GraphicsOffset = 0x03005064;
                        actor.P_PaletteIndex = 0x13c;
                    }
                    if (actor.P_GraphicsIndex == 0x10) {
                        actor.P_GraphicsOffset = 0x030028B4;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x11) {
                        actor.P_GraphicsOffset = 0x03002DEC;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x12) {
                        actor.P_GraphicsOffset = 0x03002E5C;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x13) {
                        actor.P_GraphicsOffset = 0x03004FC8;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x18) {
                        actor.P_PaletteIndex = 0x0000033B;
                        actor.P_Field0E = 0;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        actor.P_SpriteWidth = 0xff;
                        //*(undefined4*)0x03005370 = 3;
                    }
                    if (actor.P_GraphicsIndex == 0x28) {
                        actor.P_GraphicsOffset = 0x030042F8;
                        actor.P_PaletteIndex = 0x000001D5;
                        actor.P_SpriteHeight = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x29) {
                        actor.P_GraphicsOffset = 0x03002F14;
                        actor.P_PaletteIndex = 0x1e8;
                        actor.P_SpriteHeight = 1;
                    }
                    if (actor.P_GraphicsIndex == 0x2a) {
                        actor.P_GraphicsOffset = 0x03002568;
                        actor.P_PaletteIndex = 0x1ee;
                        actor.P_SpriteHeight = 2;
                    }
                    if (actor.P_GraphicsIndex == 0x2b) {
                        actor.P_GraphicsOffset = 0x030051A0;
                        actor.P_PaletteIndex = 0x1dc;
                        actor.P_AnimIndex = 5;
                        actor.P_Field34 = actor.P_Field12;
                    }
                    if (actor.P_GraphicsIndex == 0x2c) {
                        actor.P_GraphicsOffset = 0x03002378;
                        actor.P_PaletteIndex = 0x1be;
                        actor.P_SpriteHeight = 4;
                    }
                    if (actor.P_GraphicsIndex == 0x2d) {
                        actor.P_GraphicsOffset = 0x03005128;
                        actor.P_PaletteIndex = 0x000001FB;
                        actor.P_SpriteHeight = 5;
                    }
                    if (actor.P_GraphicsIndex == 0x33) {
                        actor.P_GraphicsOffset = 0x03005248;
                        actor.P_PaletteIndex = 0x1fe;
                        actor.P_SpriteHeight = 6;
                    }
                    if (actor.P_GraphicsIndex == 0x32) {
                        actor.P_GraphicsOffset = 0x030029C8;
                        actor.P_PaletteIndex = 0x1ce;
                        actor.P_SpriteHeight = 7;
                    }
                    if (actor.P_GraphicsIndex == 0x30) {
                        actor.P_GraphicsOffset = 0x03004270;
                        actor.P_PaletteIndex = 0x0000017B;
                    }
                    if (actor.P_GraphicsIndex == 0x34) {
                        actor.P_GraphicsOffset = 0x03002324;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x35) {
                        actor.P_GraphicsOffset = 0x03004264;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x3a) {
                        actor.P_GraphicsOffset = 0x03004F6C;
                        actor.P_PaletteIndex = 0x0000017F;
                    }
                    if (actor.P_GraphicsIndex == 0x3b) {
                        actor.P_GraphicsOffset = 0x03002370;
                        actor.P_PaletteIndex = 0x0000017D;
                    }
                    if (actor.P_GraphicsIndex == 0x3c) {
                        actor.P_GraphicsOffset = 0x030023CC;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x3d) {
                        actor.P_GraphicsOffset = 0x030023F0;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x37) {
                        actor.P_GraphicsOffset = 0x03002904;
                        actor.P_PaletteIndex = 0x192;
                    }
                    if (actor.P_GraphicsIndex == 0x38) {
                        actor.P_GraphicsOffset = 0x03004FF0;
                        actor.P_PaletteIndex = 0x0000017B;
                    }
                    if (actor.P_GraphicsIndex == 0x39) {
                        actor.P_GraphicsOffset = 0x03002908;
                        actor.P_Field14 = 0;
                        actor.P_PaletteIndex = 0x14e;
                    }
                    if (actor.P_GraphicsIndex == 0x31) {
                        actor.P_GraphicsOffset = 0x03005188;
                        actor.P_PaletteIndex = 0x2a8;
                    }
                    if (actor.P_GraphicsIndex == 0x3e) {
                        actor.P_GraphicsOffset = 0x03002300;
                        actor.P_PaletteIndex = 0x000002A6;
                    }
                    #endregion
                    break;
                case 2:
                    // 2 is always a big thing. We'll check that later
                    #region World 2

                    if (actor.P_GraphicsIndex == 3) {
                        actor.P_FunctionPointer = 0x08037B9D;
                    }
                    if (actor.P_GraphicsIndex == 4) {
                        actor.P_FunctionPointer = 0x08037CA1;
                    }
                    if (actor.P_GraphicsIndex == 5) {
                        actor.P_GraphicsOffset = 0x03005158;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 6) {
                        actor.P_FunctionPointer = 0x0804DC65;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 0x18) {
                        actor.P_PaletteIndex = 0x0000033B;
                        actor.P_Field0E = 0;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        actor.P_SpriteWidth = 0xff;
                        //*(undefined4*)0x03005370 = 4;
                    }
                    if (actor.P_GraphicsIndex == 0x34) {
                        actor.P_GraphicsOffset = 0x03002324;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x35) {
                        actor.P_GraphicsOffset = 0x03004264;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 8) {
                        actor.P_GraphicsOffset = 0x0300401C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 9) {
                        actor.P_GraphicsOffset = 0x0300248C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 10) {
                        actor.P_GraphicsOffset = 0x03002374;
                        actor.P_PaletteIndex = 0x13a;
                    }
                    if (actor.P_GraphicsIndex == 0xb) {
                        actor.P_GraphicsOffset = 0x03005064;
                        actor.P_PaletteIndex = 0x13c;
                    }
                    if (actor.P_GraphicsIndex == 0x28) {
                        actor.P_GraphicsOffset = 0x03002ED0;
                        actor.P_PaletteIndex = 0x00000201;
                        actor.P_SpriteHeight = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x29) {
                        actor.P_GraphicsOffset = 0x030022E0;
                        actor.P_PaletteIndex = 0x00000207;
                        actor.P_SpriteHeight = 1;
                        actor.P_Field0E = 0x65;
                    }
                    if (actor.P_GraphicsIndex == 0x2a) {
                        actor.P_GraphicsOffset = 0x03004F94;
                        actor.P_PaletteIndex = 0x00000216;
                        actor.P_SpriteHeight = 2;
                    }
                    if (actor.P_GraphicsIndex == 0x2b) {
                        actor.P_GraphicsOffset = 0x030025CC;
                        actor.P_PaletteIndex = 0x210;
                        actor.P_SpriteHeight = 3;
                    }
                    if (actor.P_GraphicsIndex == 0x2c) {
                        actor.P_GraphicsOffset = 0x03002464;
                        actor.P_PaletteIndex = 0x21c;
                        actor.P_SpriteHeight = 4;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x30) {
                        actor.P_GraphicsOffset = 0x03005140;
                        actor.P_PaletteIndex = 0x00000177;
                    }
                    if (actor.P_GraphicsIndex == 0x36) {
                        actor.P_GraphicsOffset = 0x03004FE0;
                        actor.P_PaletteIndex = 0x00000179;
                    }
                    if (actor.P_GraphicsIndex == 0x3b) {
                        actor.P_GraphicsOffset = 0x0300292C;
                        actor.P_PaletteIndex = 0x000002B9;
                    }
                    if (actor.P_GraphicsIndex == 0x3c) {
                        actor.P_GraphicsOffset = 0x030024B8;
                        actor.P_PaletteIndex = 0x30c;
                    }
                    if (actor.P_GraphicsIndex == 0x3d) {
                        actor.P_GraphicsOffset = 0x03004F6C;
                        actor.P_PaletteIndex = 0x0000017F;
                    }
                    if (actor.P_GraphicsIndex == 0x3e) {
                        actor.P_GraphicsOffset = 0x03005220;
                        actor.P_PaletteIndex = 0x2c0;
                    }
                    if (actor.P_GraphicsIndex == 0x40) {
                        actor.P_GraphicsOffset = 0x030030B4;
                        actor.P_PaletteIndex = 0x000002C9;
                    }
                    if (actor.P_GraphicsIndex == 0x41) {
                        actor.P_GraphicsOffset = 0x03003088;
                        actor.P_PaletteIndex = 0x30c;
                    }
                    if (actor.P_GraphicsIndex == 0x42) {
                        actor.P_GraphicsOffset = 0x030025B8;
                        actor.P_PaletteIndex = 0x30c;
                    }
                    if (actor.P_GraphicsIndex == 0x31) {
                        actor.P_GraphicsOffset = 0x0300435C;
                        actor.P_PaletteIndex = 0x000002A2;
                    }
                    if (actor.P_GraphicsIndex == 0x32) {
                        actor.P_GraphicsOffset = 0x0300245C;
                        actor.P_PaletteIndex = 0x2a4;
                    }
                    if (actor.P_GraphicsIndex == 0x43) {
                        actor.P_GraphicsOffset = 0x030030C8;
                        actor.P_PaletteIndex = 0x000002A1;
                    }
                    #endregion
                    break;
                case 3:
                    // 2 is always a big thing. We'll check that later
                    #region World 3

                    if (actor.P_GraphicsIndex == 3) {
                        actor.P_FunctionPointer = 0x08037B9D;
                    }
                    if (actor.P_GraphicsIndex == 4) {
                        actor.P_FunctionPointer = 0x08037CA1;
                    }
                    if (actor.P_GraphicsIndex == 5) {
                        actor.P_GraphicsOffset = 0x03005158;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 6) {
                        actor.P_FunctionPointer = 0x0804DC65;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 8) {
                        actor.P_GraphicsOffset = 0x0300401C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 9) {
                        actor.P_GraphicsOffset = 0x0300248C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 10) {
                        actor.P_GraphicsOffset = 0x03002374;
                        actor.P_PaletteIndex = 0x13a;
                    }
                    if (actor.P_GraphicsIndex == 0xb) {
                        actor.P_GraphicsOffset = 0x03005064;
                        actor.P_PaletteIndex = 0x13c;
                    }
                    if (actor.P_GraphicsIndex == 0x18) {
                        actor.P_PaletteIndex = 0x0000033B;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        actor.P_SpriteHeight = 0;
                        //actor.P_Field0E = actor.RuntimeXPosition << 2;
                        actor.P_Field2E = (short)0x00000341;
                        actor.P_AnimIndex = 0xb;
                        actor.P_RuntimeAnimFrame = 1;
                        actor.P_Field10 = 1;
                        actor.P_Field14 = 200;
                    }
                    if (actor.P_GraphicsIndex == 0x28) {
                        actor.P_GraphicsOffset = 0x03005190;
                        actor.P_PaletteIndex = 0x00000223;
                        actor.P_SpriteHeight = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x29) {
                        actor.P_GraphicsOffset = 0x030023B0;
                        actor.P_PaletteIndex = 0x00000246;
                        actor.P_SpriteHeight = 1;
                    }
                    if (actor.P_GraphicsIndex == 0x2a) {
                        actor.P_GraphicsOffset = 0x03002498;
                        actor.P_PaletteIndex = 0x230;
                        actor.P_SpriteHeight = 4;
                    }
                    if (actor.P_GraphicsIndex == 0x2b) {
                        actor.P_GraphicsOffset = 0x03003F74;
                        actor.P_PaletteIndex = 0x238;
                        actor.P_SpriteHeight = 2;
                    }
                    if (actor.P_GraphicsIndex == 0x2c) {
                        actor.P_GraphicsOffset = 0x030028E8;
                        actor.P_PaletteIndex = 0x240;
                        actor.P_SpriteHeight = 3;
                    }
                    if (actor.P_GraphicsIndex == 0x2d) {
                        actor.P_GraphicsOffset = 0x03002378;
                        actor.P_PaletteIndex = 0x1be;
                        actor.P_SpriteHeight = 5;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x38) {
                        actor.P_GraphicsOffset = 0x03004354;
                        actor.P_PaletteIndex = 0x00000271;
                    }
                    if (actor.P_GraphicsIndex == 0x39) {
                        actor.P_GraphicsOffset = 0x03003F54;
                        actor.P_PaletteIndex = 0x00000273;
                    }
                    if (actor.P_GraphicsIndex == 0x32) {
                        actor.P_GraphicsOffset = 0x03003FC8;
                        actor.P_PaletteIndex = 0x280;
                    }
                    if (actor.P_GraphicsIndex == 0x33) {
                        actor.P_GraphicsOffset = 0x03005200;
                        actor.P_PaletteIndex = 0x18e;
                    }
                    if (actor.P_GraphicsIndex == 0x34) {
                        actor.P_GraphicsOffset = 0x03002324;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x35) {
                        actor.P_GraphicsOffset = 0x03004264;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x36) {
                        actor.P_GraphicsOffset = 0x03002444;
                        actor.P_PaletteIndex = 0x152;
                    }
                    if (actor.P_GraphicsIndex == 0x37) {
                        actor.P_GraphicsOffset = 0x030050A0;
                        actor.P_PaletteIndex = 0x154;
                    }
                    if (actor.P_GraphicsIndex == 0x3b) {
                        actor.P_GraphicsOffset = 0x03003720;
                        actor.P_PaletteIndex = 0x15a;
                    }
                    if (actor.P_GraphicsIndex == 0x30) {
                        actor.P_GraphicsOffset = 0x0300523C;
                        actor.P_PaletteIndex = 0x18c;
                    }
                    if (actor.P_GraphicsIndex == 0x3c) {
                        actor.P_GraphicsOffset = 0x03002334;
                        actor.P_PaletteIndex = 0x0000026F;
                    }
                    if (actor.P_GraphicsIndex == 0x3d) {
                        actor.P_GraphicsOffset = 0x03004F6C;
                        actor.P_PaletteIndex = 0x0000017F;
                    }
                    if (actor.P_GraphicsIndex == 0x3e) {
                        actor.P_GraphicsOffset = 0x030042EC;
                        actor.P_PaletteIndex = 0x000002B1;
                    }
                    if (actor.P_GraphicsIndex == 0x40) {
                        actor.P_GraphicsOffset = 0x03004040;
                        actor.P_PaletteIndex = 0x000002C2;
                    }
                    if (actor.P_GraphicsIndex == 0x41) {
                        actor.P_GraphicsOffset = 0x030028DC;
                        actor.P_PaletteIndex = 0x000002CB;
                    }
                    if (actor.P_GraphicsIndex == 0x42) {
                        actor.P_GraphicsOffset = 0x03002ED4;
                        actor.P_PaletteIndex = 0x156;
                    }
                    if (actor.P_GraphicsIndex == 0x31) {
                        actor.P_GraphicsOffset = 0x03005188;
                        actor.P_PaletteIndex = 0x2a8;
                    }
                    if (actor.P_GraphicsIndex == 0x43) {
                        actor.P_GraphicsOffset = 0x03003F5C;
                        actor.P_PaletteIndex = 0x000002AA;
                    }
                    #endregion
                    break;
                case 4:
                    // 2 is always a big thing. We'll check that later
                    #region World 4

                    if (actor.P_GraphicsIndex == 3) {
                        actor.P_FunctionPointer = 0x08037B9D;
                    }
                    if (actor.P_GraphicsIndex == 4) {
                        actor.P_FunctionPointer = 0x08037CA1;
                    }
                    if (actor.P_GraphicsIndex == 5) {
                        actor.P_GraphicsOffset = 0x03005158;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 6) {
                        actor.P_FunctionPointer = 0x0804DC65;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 8) {
                        actor.P_GraphicsOffset = 0x0300401C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 9) {
                        actor.P_GraphicsOffset = 0x0300248C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 10) {
                        actor.P_GraphicsOffset = 0x03002374;
                        actor.P_PaletteIndex = 0x13a;
                    }
                    if (actor.P_GraphicsIndex == 0xb) {
                        actor.P_GraphicsOffset = 0x03005064;
                        actor.P_PaletteIndex = 0x13c;
                    }
                    if (actor.P_GraphicsIndex == 0x18) {
                        actor.P_PaletteIndex = 0x0000033B;
                        actor.P_Field0E = 0;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 0x28) {
                        if (level == 0) {
                            actor.P_GraphicsOffset = 0x030051B8;
                            actor.P_AnimIndex = 0xd;
                            actor.P_PaletteIndex = 0x00000282;
                        } else {
                            if (level == 0x18) {
                                actor.P_Field34 = actor.P_Field34 | 0x200;
                            } else {
                                actor.P_GraphicsOffset = 0x03002ED0;
                                actor.P_PaletteIndex = 0x00000201;
                                actor.P_SpriteHeight = 4;
                            }
                        }
                    }
                    if (actor.P_GraphicsIndex == 0x29) {
                        actor.P_GraphicsOffset = 0x0300501C;
                        actor.P_PaletteIndex = 0x0000024D;
                        actor.P_SpriteHeight = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x2a) {
                        if (level == 0) {
                            actor.P_GraphicsOffset = 0x03004338;
                            actor.P_AnimIndex = 0x1d;
                            actor.P_PaletteIndex = 0x288;
                        } else {
                            if (level == 0x18) {
                                actor.P_Field34 = actor.P_Field34 | 0x200;
                            } else {
                                actor.P_GraphicsOffset = 0x03002464;
                                actor.P_PaletteIndex = 0x21c;
                                actor.P_SpriteHeight = 2;
                            }
                        }
                    }
                    if (actor.P_GraphicsIndex == 0x2b) {
                        if (level == 0) {
                            actor.P_GraphicsOffset = 0x03004330;
                            actor.P_AnimIndex = 6;
                            actor.P_PaletteIndex = 0x284;
                        } else {
                            if (level == 0x18) {
                                actor.P_Field34 = actor.P_Field34 | 0x200;
                            } else {
                                actor.P_GraphicsOffset = 0x03002DE0;
                                actor.P_PaletteIndex = 0x0000025B;
                                actor.P_SpriteHeight = 1;
                            }
                        }
                    }
                    if (actor.P_GraphicsIndex == 0x2c) {
                        if (level == 0) {
                            actor.P_GraphicsOffset = 0x03002580;
                            actor.P_AnimIndex = 6;
                            actor.P_PaletteIndex = 0x00000286;
                        } else {
                            if (level == 0x18) {
                                actor.P_Field34 = actor.P_Field34 | 0x200;
                            } else {
                                actor.P_GraphicsOffset = 0x030024F0;
                                actor.P_PaletteIndex = 0x00000253;
                                actor.P_SpriteHeight = 3;
                            }
                        }
                    }
                    if (actor.P_GraphicsIndex == 0x2e) {
                        actor.P_GraphicsOffset = 0x03002F0C;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 0x00000195;
                        actor.P_Field12 = 6;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        //*(Actor**)0x03004278 = actor;
                    }
                    if (actor.P_GraphicsIndex == 0x30) {
                        actor.P_GraphicsOffset = 0x03002514;
                        actor.P_PaletteIndex = 0x00000175;
                    }
                    if (actor.P_GraphicsIndex == 0x31) {
                        switch (actor.P_Field0E) {
                            case 0: actor.P_GraphicsOffset = 0x03005114; break;
                            case 1: actor.P_GraphicsOffset = 0x03005090; break;
                            case 2: actor.P_GraphicsOffset = 0x030042f4; break;
                            case 3: actor.P_GraphicsOffset = 0x03004034; break;
                            case 4: actor.P_GraphicsOffset = 0x030023a4; break;
                            case 5: actor.P_GraphicsOffset = 0x03002500; break;
                        }
                        actor.P_PaletteIndex = 0x00000279;
                    }
                    if (actor.P_GraphicsIndex == 0x32) {
                        actor.P_GraphicsOffset = 0x03003FCC;
                        actor.P_PaletteIndex = 400;
                    }
                    if (actor.P_GraphicsIndex == 0x33) {
                        actor.P_GraphicsOffset = 0x03004F9C;
                        actor.P_PaletteIndex = 0x00000262;
                        if (actor.P_Field12 == 0x96) {
                            if (level == 0) {
                                actor.P_Field34 = actor.P_Field34 | 0x200;
                            } else {
                                actor.P_AnimIndex = 1;
                            }
                        }
                    }
                    if (actor.P_GraphicsIndex == 0x34) {
                        actor.P_GraphicsOffset = 0x03002324;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x35) {
                        actor.P_GraphicsOffset = 0x03004264;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x36) {
                        actor.P_GraphicsOffset = 0x0300439C;
                        actor.P_PaletteIndex = 0x000002DD;
                    }
                    if (actor.P_GraphicsIndex == 0x37) {
                        actor.P_GraphicsOffset = 0x030050B0;
                        actor.P_PaletteIndex = 0x000002DD;
                    }
                    if (actor.P_GraphicsIndex == 0x38) {
                        actor.P_GraphicsOffset = 0x03004340;
                        actor.P_Field18 = 0x03004010;
                        actor.P_PaletteIndex = 0x2ac;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x39) {
                        actor.P_GraphicsOffset = 0x030025AC;
                        actor.P_PaletteIndex = 0x000002AF;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x3a) {
                        actor.P_GraphicsOffset = 0x03005080;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 0x3b) {
                        actor.P_GraphicsOffset = 0x0300292C;
                        actor.P_PaletteIndex = 0x2b4;
                    }
                    if (actor.P_GraphicsIndex == 0x3c) {
                        actor.P_GraphicsOffset = 0x030022B8;
                        actor.P_Field18 = 0x0300234C;
                        actor.P_Field14 = 0x03004020;
                        actor.P_Field0E = 4;
                        actor.P_Field10 = 0x40;
                        actor.P_Field20 = 100;
                        actor.P_PaletteIndex = 0x148;
                    }
                    if (actor.P_GraphicsIndex == 0x3d) {
                        actor.P_GraphicsOffset = 0x03002E24;
                        actor.P_PaletteIndex = 4;
                    }
                    if (actor.P_GraphicsIndex == 0x3e) {
                        actor.P_GraphicsOffset = 0x03002544;
                        actor.P_PaletteIndex = 0x260;
                    }
                    if (actor.P_GraphicsIndex == 0x3f) {
                        actor.P_GraphicsOffset = 0x030023B4;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x40) {
                        actor.P_GraphicsOffset = 0x030050EC;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x41) {
                        actor.P_GraphicsOffset = 0x03004308;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x42) {
                        actor.P_GraphicsOffset = 0x0300403C;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x44) {
                        actor.P_GraphicsOffset = 0x03004390;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x45) {
                        actor.P_GraphicsOffset = 0x03004FFC;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x27) {
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        actor.P_AnimIndex = 5;
                        actor.P_Field2E = 0x32c;
                        actor.P_PaletteIndex = 0x328;
                        actor.P_Field34 = actor.P_Field34 | 1;
                        //*(undefined4*)0x03005370 = 3;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x47) {
                        actor.P_GraphicsOffset = 0x03005088;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 0x48) {
                        actor.P_GraphicsOffset = 0x03005228;
                        actor.P_PaletteIndex = 0x000002BE;
                    }
                    if (actor.P_GraphicsIndex == 0x4c) {
                        actor.P_GraphicsOffset = 0x0300516C;
                        actor.P_PaletteIndex = 0x338;
                        actor.P_Field12 = (uint)actor.RuntimeXPosition;
                        actor.P_Field0E = (uint)actor.RuntimeYPosition;
                    }
                    if (actor.P_GraphicsIndex == 0x4d) {
                        actor.P_GraphicsOffset = 0x03002534;
                        actor.P_PaletteIndex = 0x338;
                        actor.P_Field12 = (uint)actor.RuntimeXPosition;
                        actor.P_Field0E = (uint)actor.RuntimeYPosition;
                    }
                    if (actor.P_GraphicsIndex == 0x4e) {
                        actor.P_GraphicsOffset = 0x03003728;
                        actor.P_PaletteIndex = 0x00000262;
                    }
                    if (actor.P_GraphicsIndex == 0x4f) {
                        actor.P_GraphicsOffset = 0x03004F6C;
                        actor.P_PaletteIndex = 0x0000017F;
                    }
                    if (actor.P_GraphicsIndex == 0x4a) {
                        actor.P_GraphicsOffset = 0x0300232C;
                        actor.P_PaletteIndex = 0x293;
                    }
                    if (actor.P_GraphicsIndex == 0x4b) {
                        actor.P_GraphicsOffset = 0x03002E58;
                        actor.P_PaletteIndex = 0x0000029E;
                    }
                    #endregion
                    break;
                case 5:
                    // 2 is always a big thing. We'll check that later
                    #region World 5

                    if (actor.P_GraphicsIndex == 3) {
                        actor.P_FunctionPointer = 0x08037b9d;
                    }
                    if (actor.P_GraphicsIndex == 4) {
                        actor.P_FunctionPointer = 0x08037ca1;
                    }
                    if (actor.P_GraphicsIndex == 5) {
                        actor.P_GraphicsOffset = 0x03005158;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 6) {
                        actor.P_FunctionPointer = 0x0804dc65;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 8) {
                        actor.P_GraphicsOffset = 0x0300401C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 9) {
                        actor.P_GraphicsOffset = 0x0300248C;
                        actor.P_PaletteIndex = 2;
                    }
                    if (actor.P_GraphicsIndex == 10) {
                        actor.P_GraphicsOffset = 0x03002374;
                        actor.P_PaletteIndex = 0x13a;
                    }
                    if (actor.P_GraphicsIndex == 0xb) {
                        actor.P_GraphicsOffset = 0x03005064;
                        actor.P_PaletteIndex = 0x13c;
                    }
                    if (actor.P_GraphicsIndex == 0x10) {
                        actor.P_GraphicsOffset = 0x030028B4;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x11) {
                        actor.P_GraphicsOffset = 0x03002DEC;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x12) {
                        actor.P_GraphicsOffset = 0x03002E5C;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x13) {
                        actor.P_GraphicsOffset = 0x03004FC8;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x28) {
                        actor.P_GraphicsOffset = 0x030024E8;
                        actor.P_PaletteIndex = 0x0000036A;
                    }
                    if (actor.P_GraphicsIndex == 0x29) {
                        actor.P_GraphicsOffset = 0x030022F0;
                        actor.P_PaletteIndex = 0x0000036A;
                    }
                    if (actor.P_GraphicsIndex == 0x2a) {
                        actor.P_GraphicsOffset = 0x03002528;
                        actor.P_PaletteIndex = 0x0000036A;
                    }
                    if (actor.P_GraphicsIndex == 0x2b) {
                        actor.P_GraphicsOffset = 0x03003F7C;
                        actor.P_PaletteIndex = 0x0000036A;
                    }
                    if (actor.P_GraphicsIndex == 0x2c) {
                        actor.P_GraphicsOffset = 0x03002EF0;
                        actor.P_PaletteIndex = 0x00000372;
                    }
                    if (actor.P_GraphicsIndex == 0x30) {
                        actor.P_GraphicsOffset = 0x03004270;
                        actor.P_PaletteIndex = 0x0000017B;
                    }
                    if (actor.P_GraphicsIndex == 0x2e) {
                        actor.P_GraphicsOffset = 0x03002F0C;
                        actor.P_AnimIndex = 10;
                        actor.P_PaletteIndex = 0x19c;
                        actor.P_Field12 = 6;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                        //0x03004278 = actor;
                    }
                    if (actor.P_GraphicsIndex == 0x34) {
                        actor.P_GraphicsOffset = 0x03002324;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    if (actor.P_GraphicsIndex == 0x35) {
                        actor.P_GraphicsOffset = 0x03004264;
                        actor.P_AnimIndex = 8;
                        actor.P_PaletteIndex = 2;
                        actor.P_Field12 = 0;
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x3a) {
                        actor.P_GraphicsOffset = 0x03004F6C;
                        actor.P_PaletteIndex = 0x0000017F;
                    }
                    if (actor.P_GraphicsIndex == 0x3b) {
                        actor.P_GraphicsOffset = 0x03002370;
                        actor.P_PaletteIndex = 0x0000017D;
                    }
                    if (actor.P_GraphicsIndex == 0x3c) {
                        actor.P_GraphicsOffset = 0x030023CC;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x3d) {
                        actor.P_GraphicsOffset = 0x030023F0;
                        actor.P_PaletteIndex = 0x264;
                    }
                    if (actor.P_GraphicsIndex == 0x38) {
                        actor.P_GraphicsOffset = 0x03004FF0;
                        actor.P_PaletteIndex = 0x0000017B;
                    }
                    if (actor.P_GraphicsIndex == 0x39) {
                        actor.P_GraphicsOffset = 0x03002908;
                        actor.P_Field14 = 0;
                        actor.P_PaletteIndex = 0x14e;
                    }
                    if (actor.P_GraphicsIndex == 0x3e) {
                        actor.P_GraphicsOffset = 0x03004248;
                        actor.P_PaletteIndex = 0x00000275;
                    }
                    if (actor.P_GraphicsIndex == 0xf) {
                        actor.P_GraphicsOffset = 0x030022F4;
                        actor.P_PaletteIndex = 0x00000277;
                        /*iVar12 = GetSomeIndex(0x16);
                        if (iVar12 != 0) {
                            actor.P_AnimIndex = 1;
                        }*/
                    }
                    actor.P_GraphicsIndex = actor.P_GraphicsIndex;
                    if (actor.P_GraphicsIndex == 0x37) {
                        actor.P_GraphicsOffset = 0x0300518C;
                        actor.P_PaletteIndex = 0x15c;
                    }
                    if (actor.P_GraphicsIndex == 0x3f) {
                        actor.P_GraphicsOffset = 0x03002E94;
                        actor.P_PaletteIndex = 0x15e;
                    }
                    if (actor.P_GraphicsIndex == 0x40) {
                        actor.P_GraphicsOffset = 0x03005100;
                        actor.P_PaletteIndex = 0x160;
                    }
                    if (actor.P_GraphicsIndex == 0x27) {
                        actor.P_GraphicsOffset = 0x03002EB4;
                        actor.P_PaletteIndex = 0x0000016F;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 0x36) {
                        actor.P_GraphicsOffset = 0x03002F1C;
                        actor.P_PaletteIndex = 0x0000016F;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    if (actor.P_GraphicsIndex == 0x41) {
                        actor.P_GraphicsOffset = 0x03004260;
                        actor.P_PaletteIndex = 0x0000016F;
                        actor.P_Field34 = actor.P_Field34 | 0x200;
                    }
                    #endregion
                    break;
            }
        }
    }
}