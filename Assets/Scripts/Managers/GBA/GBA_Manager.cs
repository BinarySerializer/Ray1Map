using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public abstract class GBA_Manager : IGameManager
    {
        public KeyValuePair<int, int[]>[] GetLevels(GameSettings settings)
        {
            var output = new List<KeyValuePair<int, int[]>>();

            // Add normal levels
            output.AddRange(WorldLevels.Select((x, i) => new KeyValuePair<int, int[]>(i, x.ToArray())));

            // Add menu maps
            output.Add(new KeyValuePair<int, int[]>(output.Count, MenuLevels));

            // Add DLC maps if available
            if (DLCLevelCount > 0)
                output.Add(new KeyValuePair<int, int[]>(output.Count, Enumerable.Range(0, DLCLevelCount).ToArray()));

            return output.ToArray();
        }

        public LevelType GetLevelType(int world)
        {
            var worlds = WorldLevels.Length;

            if (world == worlds)
                return LevelType.Menu;

            if (world == (worlds + 1))
                return LevelType.DLC;

            return LevelType.Game;
        }

        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        public virtual string GetROMFilePath => $"ROM.gba";

        public abstract IEnumerable<int>[] WorldLevels { get; }
        public int LevelCount => WorldLevels.Select(x => x.Count()).Sum();
        public abstract int[] MenuLevels { get; }
        public abstract int DLCLevelCount { get; }

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Compressed Blocks", false, true, (input, output) => ExportAllCompressedBlocksAsync(settings, output)),
            new GameAction("Log Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, false)),
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, true)),
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

                        // Load the data block
                        var dataBlock = LoadDataBlock(context);

                        var indentLevel = 0;
                        GBA_OffsetTable offsetTable = dataBlock.UiOffsetTable;
                        GBA_DummyBlock[] blocks = new GBA_DummyBlock[offsetTable.OffsetsCount];

                        for (int i = 0; i < blocks.Length; i++) {
                            s.DoAt(offsetTable.GetPointer(i), () => {
                                blocks[i] = s.SerializeObject<GBA_DummyBlock>(blocks[i], name: $"{nameof(blocks)}[{i}]");
                            });
                        }

                        void ExportBlocks(GBA_DummyBlock block, int index, string path)
                        {
                            indentLevel++;

                            if (export) {
                                Util.ByteArrayToFile(outputDir + "/blocks/" + path + "/" + block.Offset.StringFileOffset + ".bin", block.Data);
                            }

                            writer.WriteLine($"{block.Offset}:{new string(' ', indentLevel * 2)}[{index}] Offsets: {block.OffsetTable.OffsetsCount} - BlockSize: {block.BlockSize}");

                            // Handle every block offset in the table
                            for (int i = 0; i < block.SubBlocks.Length; i++)
                            {

                                if (!references.ContainsKey(block.SubBlocks[i].Offset))
                                    references[block.SubBlocks[i].Offset] = new HashSet<Pointer>();

                                references[block.SubBlocks[i].Offset].Add(block.Offset);

                                // Export
                                ExportBlocks(block.SubBlocks[i], i, path + "/" + (i + " - " + block.SubBlocks[i].Offset.StringFileOffset));
                            }

                            indentLevel--;
                        }

                        for (int i = 0; i < blocks.Length; i++) {
                            await UniTask.WaitForEndOfFrame();
                            ExportBlocks(blocks[i], i, (i + " - " + blocks[i].Offset.StringFileOffset));
                        }
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
                    Pointer baseOffset;

                    try
                    {
                        // Read the level
                        var data = LoadDataBlock(context);
                        lvl = data.LevelBlock;
                        baseOffset = data.UiOffsetTable.Offset;
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

                        var paletteCount = spr.Palette.Palette.Length / 16;

                        for (int palIndex = 0; palIndex < paletteCount; palIndex++)
                        {
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

                                        Color c = spr.Palette.Palette[palIndex * 16 + v].GetColor();

                                        if (v != 0)
                                            c = new Color(c.r, c.g, c.b, 1f);

                                        tex.SetPixel(mainX * tileWidth + x, mainY * tileWidth + (tileWidth - y - 1), c);
                                    }
                                }
                            }

                            tex.Apply();

                            var fileName = $"Sprites_{(spr.Offset.AbsoluteOffset - baseOffset.AbsoluteOffset):X8}_Pal{palIndex}.png";

                            Util.ByteArrayToFile(Path.Combine(outputDir, fileName), tex.EncodeToPNG());
                        }
                    }
                }
            }
        }

        public virtual GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_R3_ROM>(GetROMFilePath, context).Data;

        public virtual async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            var lvlType = GetLevelType(context.Settings.World);

            // Load the data block
            var dataBlock = LoadDataBlock(context);

            // Get the current play field
            GBA_PlayField playField;

            switch (lvlType)
            {
                case LevelType.Game:
                    playField = dataBlock.LevelBlock.PlayField;
                    break;

                case LevelType.Menu:
                    playField = dataBlock.MenuLevelPlayfield;
                    break;
                
                case LevelType.DLC:
                default:
                    throw new NotImplementedException();
            }

            // Get the map layers, skipping unknown ones
            var mapLayers = playField.Layers.Where(x => x.StructType == GBA_TileLayer.TileLayerStructTypes.Map2D || x.StructType == GBA_TileLayer.TileLayerStructTypes.Mode7 || x.StructType == GBA_TileLayer.TileLayerStructTypes.Collision).ToArray();

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
                // Create the map array
                Maps = new Common_LevelMap[mapLayers.Length],

                // Create the events list
                EventData = new List<Editor_EventData>(),
            };

            // Add every map
            for (int layer = 0; layer < mapLayers.Length; layer++)
            {
                Controller.status = $"Loading map {layer + 1}/{mapLayers.Length}";
                await Controller.WaitIfNecessary();

                var map = mapLayers[layer];

                if (map.StructType == GBA_TileLayer.TileLayerStructTypes.Collision)
                {
                    commonLev.Maps[layer] = new Common_LevelMap
                    {
                        Width = map.Width,
                        Height = map.Height,
                        TileSetWidth = 1,
                        TileSet = new Common_Tileset[]
                        {
                            new Common_Tileset(new Tile[]
                            {
                                ScriptableObject.CreateInstance<Tile>(),
                            }), 
                        },
                        MapTiles = map.CollisionData.Select((x, i) => new Editor_MapTile(new MapTile()
                        {
                            CollisionType = (byte)x
                        })).ToArray()
                    };

                    commonLev.DefaultCollisionMap = layer;
                }
                else
                {
                    MapTile[] mapData = map.MapData;
                    MapTile[] bgData = playField.BGTileTable.Data1.Concat(playField.BGTileTable.Data2).ToArray();
                    if (map.StructType == GBA_TileLayer.TileLayerStructTypes.Mode7)
                        mapData = map.Mode7Data?.Select(x => bgData[x - 1].CloneObj()).ToArray();
                    else if (map.Unk_0C == 0) {
                        mapData = map.MapData?.Select(x => {
                            int index = BitHelpers.ExtractBits(x.TileMapY, 9, 0);
                            bool isData1 = BitHelpers.ExtractBits(x.TileMapY, 1, 9) == 1;
                            if (isData1) {
                                return playField.BGTileTable.Data1[index].CloneObj();
                            } else {
                                index -= 2;
                                if (index < 0) {
                                    return new MapTile() { PC_TransparencyMode = PC_MapTileTransparencyMode.FullyTransparent };
                                }
                                return playField.BGTileTable.Data2[index].CloneObj();
                            }
                        }).ToArray();
            // TODO: Avoid having to clamp here - why are the values too big?
            //mapData = map.MapData?.Select(x => bgData[Mathf.Clamp(x.TileMapY - 1, 0, bgData.Length - 1)].CloneObj()).ToArray();
                    }

                    commonLev.Maps[layer] = new Common_LevelMap
                    {
                        Width = map.Width,
                        Height = map.Height,
                        TileSetWidth = 1,
                        TileSet = new Common_Tileset[]
                        {
                            LoadTileset(context, playField, map, mapData)
                        },
                        MapTiles = mapData.Select((x, i) => new Editor_MapTile(x)).ToArray()
                    };

                    commonLev.DefaultMap = layer;
                }
            }

            Controller.status = $"Loading actors";
            await Controller.WaitIfNecessary();

            commonLev.EventData = new List<Editor_EventData>();

            var des = new Dictionary<int, Common_Design>();

            var eta = new Dictionary<string, Common_EventState[][]>();

            // Add actors
            if (lvlType != LevelType.Menu)
            {
                var actorIndex = 0;

                foreach (var actor in dataBlock.LevelBlock.Actors)
                {
                    Controller.status = $"Loading actor {actorIndex + 1}/{dataBlock.LevelBlock.Actors.Length}";
                    await Controller.WaitIfNecessary();

                    if (!des.ContainsKey(actor.GraphicsDataIndex))
                        des.Add(actor.GraphicsDataIndex, GetCommonDesign(actor.GraphicData));

                    if (!eta.ContainsKey(actor.GraphicsDataIndex.ToString()))
                        eta.Add(actor.GraphicsDataIndex.ToString(), GetCommonEventStates(actor.GraphicData));

                    commonLev.EventData.Add(new Editor_EventData(new EventData()
                    {
                        XPosition = actor.XPos * 2,
                        YPosition = actor.YPos * 2,
                        Etat = 0,
                        SubEtat = actor.StateIndex,
                        RuntimeSubEtat = actor.StateIndex
                    })
                    {
                        Type = actor.ActorID,
                        LinkIndex = actor.Link_0,
                        ForceAlways = actorIndex < dataBlock.LevelBlock.AlwaysActorsCount,
                        DESKey = actor.GraphicsDataIndex.ToString(),
                        ETAKey = actor.GraphicsDataIndex.ToString(),
                        DebugText = $"{nameof(GBA_Actor.Link_0)}: {actor.Link_0}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.Link_1)}: {actor.Link_1}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.Link_2)}: {actor.Link_2}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.Link_3)}: {actor.Link_3}{Environment.NewLine}" +
                                    $"Index: {actorIndex}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.Byte_04)}: {actor.Byte_04}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.ActorID)}: {actor.ActorID}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.GraphicsDataIndex)}: {actor.GraphicsDataIndex}{Environment.NewLine}" +
                                    $"{nameof(GBA_Actor.StateIndex)}: {actor.StateIndex}{Environment.NewLine}"
                    });

                    actorIndex++;
                }
            }

            return new GBA_EditorManager(commonLev, context, des, eta);
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
            var numPalettes = graphicData.SpriteGroup.Palette.Palette.Length / 16;

            // Add sprites for each palette
            for (int palIndex = 0; palIndex < numPalettes; palIndex++)
            {
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

                            Color c = pal[palIndex * 16 + v].GetColor();

                            if (v != 0)
                                c = new Color(c.r, c.g, c.b, 1f);

                            // Upscale to 16x16 for now...
                            tex.SetPixel(x * 2, (tileWidth - 1 - y) * 2, c);
                            tex.SetPixel(x * 2 + 1, (tileWidth - 1 - y) * 2, c);
                            tex.SetPixel(x * 2 + 1, (tileWidth - 1 - y) * 2 + 1, c);
                            tex.SetPixel(x * 2, (tileWidth - 1 - y) * 2 + 1, c);
                        }
                    }

                    tex.Apply();
                    des.Sprites.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                }
            }

            Common_AnimationPart[] GetPartsForLayer(GBA_SpriteGroup s, GBA_Animation a, GBA_AnimationLayer l) {
                if (l.TransformMode == GBA_AnimationLayer.AffineObjectMode.Hide
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Window
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Regular
                    || l.Mosaic) return new Common_AnimationPart[0];
                if (l.Color == GBA_AnimationLayer.ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Common_AnimationPart[0];
                }
                Common_AnimationPart[] parts = new Common_AnimationPart[l.XSize * l.YSize];
                if (l.ImageIndex > graphicData.SpriteGroup.TileMap.TileMapLength) {
                    Controller.print("Image index too high: " + graphicData.Offset + " - " + l.Offset);
                }
                if (l.PaletteIndex > graphicData.SpriteGroup.Palette.Palette.Length / 16) {
                    Controller.print("Palette index too high: " + graphicData.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (graphicData.SpriteGroup.Palette.Palette.Length / 16));
                }
                float rot = l.GetRotation(a, s);
                Vector2 scl = l.GetScale(a, s);
                for (int y = 0; y < l.YSize; y++) {
                    for (int x = 0; x < l.XSize; x++) {
                        parts[y * l.XSize + x] = new Common_AnimationPart {
                            ImageIndex = tileMap.TileMapLength * l.PaletteIndex + (l.ImageIndex + y * l.XSize + x),
                            IsFlippedHorizontally = l.IsFlippedHorizontally,
                            IsFlippedVertically = l.IsFlippedVertically,
                            XPosition = (l.XPosition + (l.IsFlippedHorizontally ? (l.XSize - 1 - x) : x) * 8) * 2,
                            YPosition = (l.YPosition + (l.IsFlippedVertically ? (l.YSize - 1 - y) : y) * 8) * 2,
                            Rotation = rot,
                            Scale = scl,
                            TransformOriginX = (l.XPosition + l.XSize * 8f / 2f) * 2,
                            TransformOriginY = (l.YPosition + l.YSize * 8f / 2f) * 2
                        };
                    }
                }
                return parts;
            }

            // Add first animation for now
            des.Animations.AddRange(graphicData.SpriteGroup.Animations.Select(a => new Common_Animation() {
                Frames = a.Layers.Select(f => new Common_AnimFrame {
                    Layers = f.OrderByDescending(l => l.Priority).SelectMany(l => GetPartsForLayer(graphicData.SpriteGroup, a, l)).Reverse().ToArray()
                }).ToArray()
            }));

            return des;
        }



        public Common_EventState[][] GetCommonEventStates(GBA_ActorGraphicData graphicData) {
            // Create the states
            var eta = new Common_EventState[1][];
            eta[0] = graphicData.States.Select(s => new Common_EventState() {
                AnimationIndex = s.AnimationIndex,
                AnimationSpeed = (byte)(1 + (graphicData.SpriteGroup.Animations[s.AnimationIndex].Flags & 0xF)),
                IsFlipped = s.Flags.HasFlag(GBA_ActorState.ActorStateFlags.IsFlipped)
            }).ToArray();
            int numAnims = graphicData.SpriteGroup.Animations.Length;
            if (eta[0].Length == 0 && numAnims > 0) {
                eta[0] = Enumerable.Range(0, numAnims).Select(i => new Common_EventState() {
                    AnimationIndex = (byte)i,
                    AnimationSpeed = (byte)(1 + (graphicData.SpriteGroup.Animations[i].Flags & 0xF)),
                }).ToArray();
            }

            return eta;
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
                tileMap = is8bpp ? playField.TileKit.TileMap8bpp : playField.TileKit.TileMap4bpp;
                tilePalette = playField.TileKit.TilePalette;
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

        public enum LevelType
        {
            Game,
            Menu,
            DLC
        }
    }
}