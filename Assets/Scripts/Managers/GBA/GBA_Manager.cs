using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public abstract class GBA_Manager : IGameManager
    {
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            new KeyValuePair<World, int[]>(World.Jungle, Enumerable.Range(0, LevelCount).ToArray()), 
        };

        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        public virtual string GetROMFilePath => $"ROM.gba";

        public abstract int LevelCount { get; }

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Compressed Blocks", false, true, (input, output) => ExportAllCompressedBlocksAsync(settings, output)),
            new GameAction("Log Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, false)),
            //new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, true)),
            new GameAction("Export Sprites", false, true, (input, output) => ExportSpriteSetsAsync(settings, output)),
            new GameAction("Export Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
        };

        // TODO: Find the way the game gets the vignette offsets and find remaining vignettes
        public abstract UniTask ExtractVignetteAsync(GameSettings settings, string outputDir);

        public async UniTask ExportAllCompressedBlocksAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Get the file
                var file = (GBAMemoryMappedFile)context.GetFile(GetROMFilePath);

                // Get the deserialize
                var s = context.Deserializer;

                // Keep track of blocks
                var blocks = new List<Tuple<long, long, int>>();

                // Enumerate every fourth byte (compressed blocks are always aligned to 4)
                for (int i = 0; i < file.Length; i += 4)
                {
                    // Go to the offset
                    s.Goto(file.StartPointer + i);

                    // Check for compression header
                    if (s.Serialize<byte>(default) == 0x10)
                    {
                        // Get the decompressed size
                        var decompressedSizeValue = s.SerializeArray<byte>(default, 3);
                        Array.Resize(ref decompressedSizeValue, 4);
                        var decompressedSize = BitConverter.ToUInt32(decompressedSizeValue, 0);

                        // Skip if the decompressed size is too low
                        if (decompressedSize <= 32) 
                            continue;
                        
                        // Go back to the offset
                        s.Goto(file.StartPointer + i);

                        // Attempt to decompress
                        try
                        {
                            byte[] data = null;

                            s.DoEncoded(new LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength));

                            // Make sure we got some data
                            if (data != null && data.Length > 32)
                            {
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"Block_0x{(file.StartPointer + i).AbsoluteOffset:X8}.dat"), data);

                                blocks.Add(new Tuple<long, long, int>((file.StartPointer + i).AbsoluteOffset, s.CurrentPointer - (file.StartPointer + i), data.Length));
                            }
                        }
                        catch
                        {
                            // Ignore exceptions...
                        }
                    }
                }

                var log = new List<string>();

                for (int i = 0; i < blocks.Count; i++)
                {
                    var (offset, compressedSize, size) = blocks[i];

                    var end = offset + compressedSize;

                    log.Add($"0x{offset:X8} - 0x{end:X8} (0x{compressedSize:X8} - 0x{size:X8}) - ");

                    if (i != blocks.Count - 1)
                    {
                        var dif = blocks[i + 1].Item1 - end;

                        if (dif >= 4)
                            log.Add($"0x{end:X8} - 0x{end + dif:X8} (0x{dif:X8})              - ");
                    }
                }

                File.WriteAllLines(Path.Combine(outputDir, "blocks_log.txt"), log);
            }
        }

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputDir, bool export)
        {
            // TODO: Allow blocks to be read and exported as files
            if (export)
                throw new NotImplementedException();

            using (var context = new Context(settings))
            {
                // Get the deserializer
                var s = context.Deserializer;

                var references = new Dictionary<Pointer, HashSet<Pointer>>();

                using (var logFile = File.Create(Path.Combine(outputDir, "GBA_Blocks_Log-Map.txt")))
                {
                    using (var writer = new StreamWriter(logFile))
                    {
                        // Load the ROM
                        await LoadFilesAsync(context);

                        // Read the rom
                        var rom = FileFactory.Read<GBA_R3_ROM>(GetROMFilePath, context);

                        var indentLevel = 0;

                        void ExportBlocks(Pointer blockPointer, int blockSize, int index, GBA_OffsetTable table)
                        {
                            indentLevel++;

                            writer.WriteLine($"{blockPointer}:{new string(' ', indentLevel * 2)}[{index}] Offsets: {table.OffsetsCount} - BlockSize: {blockSize}");

                            // Handle every block offset in the table
                            for (int i = 0; i < table.OffsetsCount; i++)
                            {
                                var pointer = table.GetPointer(i);
                                var length = s.DoAt(pointer - 4, () => s.Serialize<int>(default));
                                
                                var newOffsetTable = s.DoAt(pointer + length, () => 
                                {
                                    // Align
                                    s.Align();

                                    // Serialize the offset table
                                    return s.SerializeObject<GBA_OffsetTable>(default);
                                });

                                if (!references.ContainsKey(pointer))
                                    references[pointer] = new HashSet<Pointer>();

                                references[pointer].Add(blockPointer);

                                // Export
                                ExportBlocks(pointer, length, i, newOffsetTable);
                            }

                            indentLevel--;
                        }

                        // Recursively export the blocks
                        ExportBlocks(rom.Data.UiOffsetTable.Offset, 0, 0, rom.Data.UiOffsetTable);
                    }
                }

                // Log references
                using (var logFile = File.Create(Path.Combine(outputDir, "GBA_Blocks_Log-References.txt")))
                {
                    using (var writer = new StreamWriter(logFile))
                    {
                        foreach (var r in references.OrderBy(x => x.Key))
                        {
                            writer.WriteLine($"{r.Key}: {String.Join(", ", r.Value.Select(x => $"{x.AbsoluteOffset:X8}"))}");
                        }
                    }
                }
            }
        }

        public async UniTask ExportSpriteSetsAsync(GameSettings settings, string outputDir)
        {
            var exported = new HashSet<Pointer>();

            // Enumerate every level
            for (int lev = 0; lev < LevelCount; lev++)
            {
                settings.Level = lev;

                using (var context = new Context(settings))
                {
                    // Load the ROM
                    await LoadFilesAsync(context);

                    GBA_LevelBlock lvl;

                    try
                    {
                        // Read the level
                        lvl = LoadLevelBlock(context);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error loading level {lev}: {ex.Message}");
                        continue;
                    }

                    // Enumerate every graphic group
                    foreach (var spr in lvl.Actors.Select(x => x.GraphicData.SpriteGroup).Distinct())
                    {
                        if (exported.Contains(spr.Offset))
                            return;

                        exported.Add(spr.Offset);

                        var length = spr.TileMap.TileMapLength;
                        const int wrap = 16;
                        const int tileWidth = 8;
                        const int tileSize = (tileWidth * tileWidth) / 2;

                        // Create a texture for the tileset
                        var tex = new Texture2D(Mathf.Min(length, wrap) * tileWidth, Mathf.CeilToInt(length / (float)wrap) * tileWidth)
                        {
                            filterMode = FilterMode.Point,
                        };

                        // Default to transparent
                        tex.SetPixels(Enumerable.Repeat(Color.clear, tex.width * tex.height).ToArray());

                        // Add each tile
                        for (int i = 0; i < length; i++)
                        {
                            int mainY = tex.height - 1 - (i / wrap);
                            int mainX = i % wrap;

                            for (int y = 0; y < tileWidth; y++)
                            {
                                for (int x = 0; x < tileWidth; x++)
                                {
                                    int index = (i * tileSize) + ((y * tileWidth + x) / 2);
                                    var v = BitHelpers.ExtractBits(spr.TileMap.TileMap[index], 4, x % 2 == 0 ? 0 : 4);

                                    Color c = spr.Palette.Palette[v].GetColor();

                                    if (v != 0)
                                        c = new Color(c.r, c.g, c.b, 1f);

                                    tex.SetPixel(mainX * tileWidth + x, mainY * tileWidth + (tileWidth - y - 1), c);
                                }
                            }
                        }

                        tex.Apply();

                        Util.ByteArrayToFile(Path.Combine(outputDir, $"Sprites_{spr.Offset.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());
                    }
                }
            }
        }

        public virtual GBA_LevelBlock LoadLevelBlock(Context context) => FileFactory.Read<GBA_R3_ROM>(GetROMFilePath, context).Data.LevelBlock;

        public virtual async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Load the level block
            var levelBlock = LoadLevelBlock(context);

            // Get the current play field
            var playField = levelBlock.PlayField;

            // Get the map
            GBA_TileLayer map = playField.IsMode7
                ? playField.Layers.First(x => x.StructType == GBA_TileLayer.TileLayerStructTypes.Mode7)
                : playField.Layers.FirstOrDefault(x => x.LayerID == 2) ?? playField.Layers.First(x => !x.Is8bpp);

            // Get the map data to use
            var mapData = !playField.IsMode7 ? map.MapData : map.Mode7Data?.Select(x => playField.UnkBGData.Data[x - 1]).ToArray();

            // Get the collision data
            GBA_TileLayer cMap = playField.Layers.First(x => x.StructType == GBA_TileLayer.TileLayerStructTypes.Collision);

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[1],
                        MapTiles = mapData.Select((x, i) =>
                        {
                            x.CollisionType = (byte)cMap.CollisionData[i];
                            return new Editor_MapTile(x);
                        }).ToArray(),
                        TileSetWidth = 1
                    }
                },

                // Create the events list
                EventData = new List<Editor_EventData>(),
            };

            Controller.status = $"Loading actors";
            await Controller.WaitIfNecessary();

            commonLev.EventData = new List<Editor_EventData>();

            var des = new Dictionary<int, Common_Design>();

            // Add actors
            foreach (var actor in levelBlock.Actors)
            {
                if (!des.ContainsKey(actor.GraphicsDataIndex))
                    des.Add(actor.GraphicsDataIndex, GetCommonDesign(actor.GraphicData));

                commonLev.EventData.Add(new Editor_EventData(new EventData()
                {
                    XPosition = actor.XPos * 2,
                    YPosition = actor.YPos * 2
                })
                {
                    Type = actor.ActorID,
                    DESKey = actor.GraphicsDataIndex.ToString(),
                    ETAKey = String.Empty,
                    DebugText = $"{nameof(GBA_Actor.Int_08)}: {actor.Int_08}{Environment.NewLine}" +
                                $"{nameof(GBA_Actor.Byte_04)}: {actor.Byte_04}{Environment.NewLine}" +
                                $"{nameof(GBA_Actor.ActorID)}: {actor.ActorID}{Environment.NewLine}" +
                                $"{nameof(GBA_Actor.GraphicsDataIndex)}: {actor.GraphicsDataIndex}{Environment.NewLine}" +
                                $"{nameof(GBA_Actor.Byte_07)}: {actor.Byte_07}{Environment.NewLine}"
                });
            }

            Controller.status = $"Loading tilemap";
            await Controller.WaitIfNecessary();

            // Set tile set
            commonLev.Maps[0].TileSet[0] = LoadTileset(context, playField, map, mapData);

            return new GBA_EditorManager(commonLev, context, des);
        }

        public Common_Design GetCommonDesign(GBA_ActorGraphicData graphicData)
        {
            // Create the design
            var des = new Common_Design
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Common_Animation>(),
            };

            var tileMap = graphicData.SpriteGroup.TileMap;
            var pal = graphicData.SpriteGroup.Palette.Palette;
            const int tileWidth = 8;
            const int tileSize = (tileWidth * tileWidth) / 2;

            // Add sprites
            for (int i = 0; i < tileMap.TileMapLength; i++)
            {
                var tex = new Texture2D(Settings.CellSize, Settings.CellSize)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                for (int y = 0; y < tileWidth; y++)
                {
                    for (int x = 0; x < tileWidth; x++)
                    {
                        int index = (i * tileSize) + ((y * tileWidth + x) / 2);

                        var b = tileMap.TileMap[index];
                        var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                        Color c = pal[v].GetColor();

                        if (v != 0)
                            c = new Color(c.r, c.g, c.b, 1f);

                        // Upscale to 16x16 for now...
                        tex.SetPixel(x * 2, y * 2, c);
                        tex.SetPixel(x * 2 + 1, y * 2, c);
                        tex.SetPixel(x * 2 + 1, y * 2 + 1, c);
                        tex.SetPixel(x * 2, y * 2 + 1, c);
                    }
                }

                tex.Apply();

                des.Sprites.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
            }

            // Add first animation for now
            des.Animations.Add(new Common_Animation()
            {
                Frames = new Common_AnimFrame[]
                {
                    new Common_AnimFrame
                    {
                        Layers = graphicData.SpriteGroup.Animations[0].Layers.Take(graphicData.SpriteGroup.Animations[0].Header[5]).Select(x => new Common_AnimationPart
                        {
                            ImageIndex = x.Data[2],
                            XPosition = x.Data[4] * 2,
                            YPosition = x.Data[5] * 2,
                        }).ToArray()
                    }
                }
            });

            return des;
        }

        public Common_Tileset LoadTileset(Context context, GBA_PlayField playField, GBA_TileLayer map, MapTile[] mapData)
        {
            // Get the tilemap to use
            byte[] tileMap;
            bool is8bpp;
            GBA_Palette tilePalette;
            if (context.Settings.EngineVersion == EngineVersion.BatmanVengeanceGBA)
            {
                is8bpp = map.Tilemap.Is8bpp;
                tileMap = is8bpp ? map.Tilemap.TileMap8bpp : map.Tilemap.TileMap4bpp;
                tilePalette = playField.TilePalette;
            }
            else
            {
                is8bpp = map.Is8bpp;
                tileMap = is8bpp ? playField.Tilemap.TileMap8bpp : playField.Tilemap.TileMap4bpp;
                tilePalette = playField.Tilemap.TilePalette;
            }

            int tilemapLength = (tileMap.Length / (is8bpp ? 64 : 32)) + 1;


            const int paletteSize = 16;
            const int tileWidth = 8;
            int tileSize = is8bpp ? (tileWidth * tileWidth) : (tileWidth * tileWidth) / 2;

            var tiles = new Tile[tilemapLength];

            // Create empty tile
            var emptyTileTex = new Texture2D(Settings.CellSize, Settings.CellSize)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            emptyTileTex.SetPixels(Enumerable.Repeat(Color.clear, Settings.CellSize * Settings.CellSize).ToArray());
            emptyTileTex.Apply();
            Tile emptyTile = ScriptableObject.CreateInstance<Tile>();
            emptyTile.sprite = Sprite.Create(emptyTileTex, new Rect(0, 0, Settings.CellSize, Settings.CellSize), new Vector2(0.5f, 0.5f), Settings.CellSize, 20);

            tiles[0] = emptyTile;

            for (int i = 1; i < tilemapLength; i++)
            {
                // Get the palette to use
                var pals = mapData.Where(x => x.TileMapY == i).Select(x => x.PaletteIndex).Distinct().ToArray();

                if (pals.Length > 1)
                    Debug.LogWarning($"Tile {i} has several possible palettes!");

                var p = pals.FirstOrDefault();

                var tex = new Texture2D(Settings.CellSize, Settings.CellSize)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                for (int y = 0; y < tileWidth; y++)
                {
                    for (int x = 0; x < tileWidth; x++)
                    {
                        Color c;

                        int index = ((i - 1) * tileSize) + ((y * tileWidth + x) / (is8bpp ? 1 : 2));

                        if (is8bpp)
                        {
                            var b = tileMap[index];

                            c = tilePalette.Palette[b].GetColor();

                            if (b != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                        }
                        else
                        {
                            var b = tileMap[index];
                            var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                            c = tilePalette.Palette[p * paletteSize + v].GetColor();

                            if (v != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                        }

                        // Upscale to 16x16 for now...
                        tex.SetPixel(x * 2, y * 2, c);
                        tex.SetPixel(x * 2 + 1, y * 2, c);
                        tex.SetPixel(x * 2 + 1, y * 2 + 1, c);
                        tex.SetPixel(x * 2, y * 2 + 1, c);
                    }
                }

                tex.Apply();

                // Create a tile
                Tile t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = Sprite.Create(tex, new Rect(0, 0, Settings.CellSize, Settings.CellSize), new Vector2(0.5f, 0.5f), Settings.CellSize, 20);

                tiles[i] = t;
            }

            return new Common_Tileset(tiles);
        }

        public void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new GBAMemoryMappedFile(context, 0x08000000)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}