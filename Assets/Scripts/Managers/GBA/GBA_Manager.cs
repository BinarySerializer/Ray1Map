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
        public virtual int PixelsPerUnit { get; set; } = 16;
        public virtual int CellSize { get; set; } = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings)
        {
            var output = new List<GameInfo_World>();

            // Add normal levels
            output.AddRange(WorldLevels.Select((x, i) => new GameInfo_World(i, x.ToArray())));

            // Add menu maps
            output.Add(new GameInfo_World(output.Count, MenuLevels));

            // Add DLC maps if available
            if (DLCLevelCount > 0)
                output.Add(new GameInfo_World(output.Count, Enumerable.Range(0, DLCLevelCount).ToArray()));

            return GameInfo_Volume.SingleVolume(output.ToArray());
        }

        public LevelType GetLevelType(int world)
        {
            var worlds = WorldLevels.Length;

            if (world == worlds && MenuLevels.Any())
                return LevelType.Menu;

            if (world == (worlds + 1) && DLCLevelCount > 0)
                return LevelType.DLC;

            return LevelType.Game;
        }

        public virtual string GetROMFilePath => $"ROM.gba";

        public abstract IEnumerable<int>[] WorldLevels { get; }
        public int LevelCount => WorldLevels.Select(x => x.Count()).Sum();
        public abstract int[] MenuLevels { get; }
        public abstract int DLCLevelCount { get; }
        public abstract int[] AdditionalSprites4bpp { get; }
        public abstract int[] AdditionalSprites8bpp { get; }

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

                            s.DoEncoded(new GBA_LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength));

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
                        Controller.print(blocks.Length);

                        for (int i = 0; i < blocks.Length; i++) {
                            try {
                                s.DoAt(offsetTable.GetPointer(i), () => {
                                    blocks[i] = s.SerializeObject<GBA_DummyBlock>(blocks[i], name: $"{nameof(blocks)}[{i}]");
                                });
                            } catch (Exception e) {
                                Debug.LogError(e);
                            }
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

            Debug.Log("Finished logging blocks");
        }

        public async UniTask ExportSpriteSetsAsync(GameSettings settings, string outputDir)
        {
            var exported = new HashSet<Pointer>();
            Pointer baseOffset;

            // Export menu sprites
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the data block
                var data = LoadDataBlock(context);

                // Set the base offset
                baseOffset = data.UiOffsetTable.Offset;

                // Enumerate every menu sprite group
                foreach (var menuSprite in AdditionalSprites4bpp)
                {
                    var s = context.Deserializer;
                    ExportSpriteGroup(s.DoAt(data.UiOffsetTable.GetPointer(menuSprite), () => s.SerializeObject<GBA_SpriteGroup>(default)), false);
                }

                foreach (var menuSprite in AdditionalSprites8bpp)
                {
                    var s = context.Deserializer;
                    ExportSpriteGroup(s.DoAt(data.UiOffsetTable.GetPointer(menuSprite), () => s.SerializeObject<GBA_SpriteGroup>(default)), true);
                }
            }

            // Enumerate every level
            for (int lev = 0; lev < LevelCount; lev++)
            {
                settings.Level = lev;

                using (var context = new Context(settings))
                {
                    // Load the ROM
                    await LoadFilesAsync(context);

                    // Read the level
                    var data = LoadDataBlock(context);
                    GBA_LevelBlock lvl = data.LevelBlock;

                    // Enumerate every graphic group
                    foreach (var spr in lvl.Actors.Select(x => x.GraphicData.SpriteGroup).Distinct())
                        ExportSpriteGroup(spr, false);
                }
            }

            void ExportSpriteGroup(GBA_SpriteGroup spr, bool is8bit)
            {
                if (exported.Contains(spr.Offset))
                    return;

                exported.Add(spr.Offset);
                var paletteCount = is8bit ? 1 : spr.Palette.Palette.Length / 16;

                for (int palIndex = 0; palIndex < paletteCount; palIndex++)
                {
                    var length = spr.TileMap.TileMapLength / (is8bit ? 2 : 1);
                    const int wrap = 16;
                    const int tileWidth = 8;
                    int tileSize = (tileWidth * tileWidth) / (is8bit ? 1 : 2);

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
                                int index = (i * tileSize) + ((y * tileWidth + x) / (is8bit ? 1 : 2));

                                var v = is8bit ? spr.TileMap.TileMap[index] : BitHelpers.ExtractBits(spr.TileMap.TileMap[index], 4, x % 2 == 0 ? 0 : 4);

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

        public virtual GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_ROM>(GetROMFilePath, context).Data;
        public virtual GBA_LocLanguageTable LoadLocalization(Context context) => FileFactory.Read<GBA_ROM>(GetROMFilePath, context).Localization;

        public virtual async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            var lvlType = GetLevelType(context.Settings.World);

            // Load the data block
            var dataBlock = LoadDataBlock(context);

            // Log unused data blocks in offset tables
            Debug.Log($"The following blocks were never parsed:{Environment.NewLine}" + String.Join(Environment.NewLine, GBA_OffsetTable.OffsetTables.Where(x => x.UsedOffsets.Any(y => !y)).Select(y => $"[{y.Offset}]:" + String.Join(", ", y.UsedOffsets.Select((o, i) => new
            {
                Obj = o,
                Index = i
            }).Where(o => !o.Obj).Select(o => o.Index.ToString())))));


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
            Unity_Level level = new Unity_Level
            {
                // Create the map array
                Maps = new Unity_Map[mapLayers.Length],

                // Create the events list
                EventData = new List<Unity_Obj>(),
            };

            // Add every map
            for (int layer = 0; layer < mapLayers.Length; layer++)
            {
                Controller.DetailedState = $"Loading map {layer + 1}/{mapLayers.Length}";
                await Controller.WaitIfNecessary();

                var map = mapLayers[layer];

                if (map.StructType == GBA_TileLayer.TileLayerStructTypes.Collision)
                {
                    level.Maps[layer] = new Unity_Map
                    {
                        Width = map.Width,
                        Height = map.Height,
                        TileSetWidth = 1,
                        TileSet = new Unity_MapTileMap[]
                        {
                            new Unity_MapTileMap(new Tile[]
                            {
                                ScriptableObject.CreateInstance<Tile>(),
                            }), 
                        },
                        MapTiles = map.CollisionData.Select((x, i) => new Unity_Tile(new MapTile()
                        {
                            CollisionType = (byte)x
                        })).ToArray(),
                    };

                    level.DefaultCollisionMap = layer;
                }
                else
                {
                    MapTile[] mapData = map.MapData;
                    //MapTile[] bgData = playField.BGTileTable.Indices1.Concat(playField.BGTileTable.Indices2).ToArray();
                    if (map.StructType == GBA_TileLayer.TileLayerStructTypes.Mode7) {
                        mapData = map.Mode7Data?.Select(x => new MapTile() { TileMapY = playField.BGTileTable.Indices8bpp[x > 0 ? x - 1 : 0] }).ToArray();
                    } else if (!map.IsForegroundTileLayer && context.Settings.EngineVersion != EngineVersion.GBA_SplinterCell_NGage) {
                        //Controller.print(map.MapData?.Max(m => BitHelpers.ExtractBits(m.TileMapY, 10, 0)) + " - " + mapData.Length + " - " + playField.BGTileTable.Data1.Length + " - " + playField.BGTileTable.Data2.Length);
                        //Controller.print(map.MapData?.Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.Data1.Length + " - " + playField.BGTileTable.Data2.Length);
                        //Controller.print(map.MapData?.Where(m=>m.IsFirstBlock).Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount8bpp);
                        //Controller.print(map.MapData?.Where(m => !m.IsFirstBlock).Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount8bpp);
                        int i = 0;
                        mapData = map.MapData?.Select(x => {
                            int index = x.TileMapY;
                            bool is8bpp = map.Is8bpp;
                            MapTile newt = x.CloneObj();
                            if (is8bpp) {
                                if (x.IsFirstBlock) {
                                    //Controller.print(i + " - " + index);
                                    //relativeIndex = index;
                                } else {
                                    if (index > 0) {
                                        index += 384; // 3 * 128, 24 * 16?
                                        index -= 1;
                                        //index += relativeIndex; // 383; // seems hardcoded
                                    } else {
                                        index = -1;
                                    }
                                }
                                if (index < 0 || index >= playField.BGTileTable.IndicesCount8bpp) {
                                    newt.TileMapY = 0;
                                    newt.PC_TransparencyMode = R1_PC_MapTileTransparencyMode.FullyTransparent;
                                } else {
                                    newt.TileMapY = playField.BGTileTable.Indices8bpp[index];
                                }
                            } else {
                                index -= 2;
                                if (index < 0 || index >= playField.BGTileTable.IndicesCount4bpp) {
                                    newt.TileMapY = 0;
                                    newt.PC_TransparencyMode = R1_PC_MapTileTransparencyMode.FullyTransparent;
                                } else {
                                    //Controller.print(index);
                                    newt.TileMapY = playField.BGTileTable.Indices4bpp[index];
                                }
                            }
                            i++;
                            return newt;
                        }).ToArray();
                    }

                    level.Maps[layer] = new Unity_Map
                    {
                        Width = map.Width,
                        Height = map.Height,
                        TileSetWidth = 1,
                        TileSet = new Unity_MapTileMap[]
                        {
                            LoadTileset(context, playField, map, mapData)
                        },
                        MapTiles = mapData.Select((x, i) => new Unity_Tile(x)).ToArray(),
                        IsForeground = map.LayerID == 3
                    };

                    level.DefaultMap = layer;
                }
            }

            Controller.DetailedState = $"Loading actors";
            await Controller.WaitIfNecessary();

            level.EventData = new List<Unity_Obj>();

            var des = new Dictionary<int, Unity_ObjGraphics>();

            var eta = new Dictionary<string, R1_EventState[][]>();

            // Add actors
            if (lvlType != LevelType.Menu)
            {
                var actorIndex = 0;

                foreach (var actor in dataBlock.LevelBlock.Actors)
                {
                    Controller.DetailedState = $"Loading actor {actorIndex + 1}/{dataBlock.LevelBlock.Actors.Length}";
                    await Controller.WaitIfNecessary();

                    if (!des.ContainsKey(actor.GraphicsDataIndex))
                        des.Add(actor.GraphicsDataIndex, GetCommonDesign(actor.GraphicData));

                    if (!eta.ContainsKey(actor.GraphicsDataIndex.ToString()))
                        eta.Add(actor.GraphicsDataIndex.ToString(), GetCommonEventStates(actor.GraphicData));

                    level.EventData.Add(new Unity_Obj(new R1_EventData()
                    {
                        XPosition = actor.XPos,
                        YPosition = actor.YPos,
                        Etat = 0,
                        SubEtat = actor.StateIndex,
                        RuntimeSubEtat = actor.StateIndex
                    })
                    {
                        Type = actor.ActorID,
                        GBALinks = new int[]
                        {
                            actor.Link_0, 
                            actor.Link_1, 
                            actor.Link_2, 
                            actor.Link_3, 
                        },
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
                                    $"{nameof(GBA_Actor.StateIndex)}: {actor.StateIndex}{Environment.NewLine}" +
                                    $"State_UnkOffsetIndexType: {actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.StateDataType}{Environment.NewLine}" + 
                                    $"State_UnkOffsetIndex: {actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.StateDataOffsetIndex}{Environment.NewLine}" +
                                    $"State_Byte_00: {actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.Byte_00}{Environment.NewLine}" +
                                    $"State_Byte_01: {actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.Byte_01}{Environment.NewLine}" +
                                    $"State_Byte_02: {actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.Byte_02}{Environment.NewLine}" +
                                    $"State_Byte_03: {actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.Byte_03}{Environment.NewLine}" +
                                    $"State_Data: {String.Join("-", actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.StateData?.Data ?? new byte[0])}{Environment.NewLine}"
                    });

                    actorIndex++;
                }
            }

            var strings = LoadLocalization(context)?.StringGroups;

            if (strings != null)
            {
                level.Localization = new Dictionary<string, string[]>();

                // TODO: Don't hard-code languages as they differ between games and releases
                var languages = new string[]
                {
                    "English",
                    "French",
                    "Spanish",
                    "German",
                    "Italian",
                    "Dutch",
                    "Swedish",
                    "Finnish",
                    "Norwegian",
                    "Danish"
                };

                for (int i = 0; i < strings.Length; i++)
                    level.Localization.Add(languages[i], strings[i].LocStrings.SelectMany(x => x.Strings).ToArray());
            }

            return new GBA_EditorManager(level, context, des, eta);
        }

        public Unity_ObjGraphics GetCommonDesign(GBA_ActorGraphicData graphicData)
        {
            // Create the design
            var des = new Unity_ObjGraphics
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };
            if (graphicData == null) return des;

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
                    var tex = new Texture2D(CellSize, CellSize)
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

                            tex.SetPixel(x, (tileWidth - 1 - y), c);
                        }
                    }

                    tex.Apply();
                    des.Sprites.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), PixelsPerUnit, 20));
                }
            }

            Unity_ObjAnimationPart[] GetPartsForLayer(GBA_SpriteGroup s, GBA_Animation a, int frame, GBA_AnimationLayer l) {
                if (l.TransformMode == GBA_AnimationLayer.AffineObjectMode.Hide
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Window
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Regular
                    || l.Mosaic) return new Unity_ObjAnimationPart[0];
                if (l.Color == GBA_AnimationLayer.ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Unity_ObjAnimationPart[0];
                }
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[l.XSize * l.YSize];
                if (l.ImageIndex > graphicData.SpriteGroup.TileMap.TileMapLength) {
                    Controller.print("Image index too high: " + graphicData.Offset + " - " + l.Offset);
                }
                if (l.PaletteIndex > graphicData.SpriteGroup.Palette.Palette.Length / 16) {
                    Controller.print("Palette index too high: " + graphicData.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (graphicData.SpriteGroup.Palette.Palette.Length / 16));
                }
                float rot = l.GetRotation(a, s, frame);
                Vector2 scl = l.GetScale(a, s, frame);
                for (int y = 0; y < l.YSize; y++) {
                    for (int x = 0; x < l.XSize; x++) {
                        parts[y * l.XSize + x] = new Unity_ObjAnimationPart {
                            ImageIndex = tileMap.TileMapLength * l.PaletteIndex + (l.ImageIndex + y * l.XSize + x),
                            IsFlippedHorizontally = l.IsFlippedHorizontally,
                            IsFlippedVertically = l.IsFlippedVertically,
                            XPosition = (l.XPosition + (l.IsFlippedHorizontally ? (l.XSize - 1 - x) : x) * CellSize),
                            YPosition = (l.YPosition + (l.IsFlippedVertically ? (l.YSize - 1 - y) : y) * CellSize),
                            Rotation = rot,
                            Scale = scl,
                            TransformOriginX = (l.XPosition + l.XSize * CellSize / 2f),
                            TransformOriginY = (l.YPosition + l.YSize * CellSize / 2f)
                        };
                    }
                }
                return parts;
            }

            // Add first animation for now
            foreach (var a in graphicData.SpriteGroup.Animations) {
                var unityAnim = new Unity_ObjAnimation();
                var frames = new List<Unity_ObjAnimationFrame>();
                for (int i = 0; i < a.FrameCount; i++) {
                    frames.Add(new Unity_ObjAnimationFrame() {
                        Layers = a.Layers[i].OrderByDescending(l => l.Priority).SelectMany(l => GetPartsForLayer(graphicData.SpriteGroup, a, i, l)).Reverse().ToArray()
                    });
                }
                unityAnim.Frames = frames.ToArray();
                des.Animations.Add(unityAnim);
            }

            return des;
        }



        public R1_EventState[][] GetCommonEventStates(GBA_ActorGraphicData graphicData) {
            // Create the states
            if (graphicData == null) return new R1_EventState[0][];
            var eta = new R1_EventState[1][];
            eta[0] = graphicData.States.Select(s => new R1_EventState() {
                AnimationIndex = s.AnimationIndex,
                AnimationSpeed = (byte)(1 + (graphicData.SpriteGroup.Animations[s.AnimationIndex].Flags & 0xF)),
                IsFlipped = s.Flags.HasFlag(GBA_ActorState.ActorStateFlags.IsFlipped)
            }).ToArray();
            int numAnims = graphicData.SpriteGroup.Animations.Length;
            if (eta[0].Length == 0 && numAnims > 0) {
                eta[0] = Enumerable.Range(0, numAnims).Select(i => new R1_EventState() {
                    AnimationIndex = (byte)i,
                    AnimationSpeed = (byte)(1 + (graphicData.SpriteGroup.Animations[i].Flags & 0xF)),
                }).ToArray();
            }

            return eta;
        }

        public Unity_MapTileMap LoadTileset(Context context, GBA_PlayField playField, GBA_TileLayer map, MapTile[] mapData)
        {
            // Get the tileset to use
            byte[] tileset;
            bool is8bpp;
            GBA_Palette tilePalette;
            if (context.Settings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
            {
                is8bpp = map.TileKit.Is8bpp;
                tileset = is8bpp ? map.TileKit.TileSet8bpp : map.TileKit.TileSet4bpp;
                tilePalette = playField.TilePalette;
            }
            else
            {
                is8bpp = map.Is8bpp;
                tileset = is8bpp ? playField.TileKit.TileSet8bpp : playField.TileKit.TileSet4bpp;
                tilePalette = playField.TileKit.TilePalette;
            }

            int tileSize = (is8bpp ? (CellSize * CellSize) : (CellSize * CellSize) / 2);
            int tilesetLength = (tileset.Length / (is8bpp ? (CellSize*CellSize) : (CellSize * CellSize)/2)) + 1;


            const int paletteSize = 16;

            var tiles = new Tile[tilesetLength];

            // Create empty tile
            var emptyTileTex = new Texture2D(CellSize, CellSize)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            emptyTileTex.SetPixels(Enumerable.Repeat(Color.clear, CellSize * CellSize).ToArray());
            emptyTileTex.Apply();
            Tile emptyTile = ScriptableObject.CreateInstance<Tile>();
            emptyTile.sprite = Sprite.Create(emptyTileTex, new Rect(0, 0, CellSize, CellSize), new Vector2(0.5f, 0.5f), PixelsPerUnit, 20);

            tiles[0] = emptyTile;

            for (int i = 1; i < tilesetLength; i++)
            {
                // Get the palette to use
                var pals = mapData.Where(x => x.TileMapY == i).Select(x => x.PaletteIndex).Distinct().ToArray();

                if (pals.Length > 1)
                    Debug.LogWarning($"Tile {i} has several possible palettes!");

                int p = pals.FirstOrDefault();
                if (context.Settings.EngineVersion == EngineVersion.GBA_SplinterCell && map.IsForegroundTileLayer) {
                    //p = ((p + 8) % (tilePalette.Palette.Length / paletteSize));
                    p += 8;
                }

                var tex = new Texture2D(CellSize, CellSize)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                for (int y = 0; y < CellSize; y++)
                {
                    for (int x = 0; x < CellSize; x++)
                    {
                        Color c;

                        int index = ((i - 1) * tileSize) + ((y * CellSize + x) / (is8bpp ? 1 : 2));

                        if (is8bpp)
                        {
                            var b = tileset[index];

                            c = tilePalette.Palette[b].GetColor();

                            if (b != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                        }
                        else
                        {
                            var b = tileset[index];
                            var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                            c = tilePalette.Palette[p * paletteSize + v].GetColor();

                            if (v != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                        }

                        tex.SetPixel(x, y, c);
                    }
                }

                tex.Apply();

                // Create a tile
                Tile t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = Sprite.Create(tex, new Rect(0, 0, CellSize, CellSize), new Vector2(0.5f, 0.5f), PixelsPerUnit, 20);

                tiles[i] = t;
            }

            return new Unity_MapTileMap(tiles);
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