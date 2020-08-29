using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

using ImageMagick;
using JetBrains.Annotations;

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
        public virtual string GetGameCubeManifestFilePath => $"gba.nfo";

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
            new GameAction("Export Sprites", false, true, (input, output) => ExportSpritesAsync(settings, output, false)),
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportSpritesAsync(settings, output, true)),
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
                var file = context.GetFile(GetROMFilePath);

                // Get the deserialize
                var s = context.Deserializer;

                // Keep track of blocks
                var blocks = new List<Tuple<long, long, int>>();

                s.Goto(file.StartPointer);

                // Enumerate every fourth byte (compressed blocks are always aligned to 4)
                for (int i = 0; i < s.CurrentLength; i += 4)
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

        public async UniTask ExportSpritesAsync(GameSettings settings, string outputDir, bool exportAnimFrames)
        {
            var exported = new HashSet<Pointer>();

            // Export menu sprites
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the data block
                var data = LoadDataBlock(context);

                // Enumerate every menu sprite group
                foreach (var menuSprite in AdditionalSprites4bpp)
                {
                    var s = context.Deserializer;
                    await ExportSpriteGroup(s.DoAt(data.UiOffsetTable.GetPointer(menuSprite), () => s.SerializeObject<GBA_SpriteGroup>(default)), false, menuSprite);
                }
                await Controller.WaitIfNecessary();

                foreach (var menuSprite in AdditionalSprites8bpp)
                {
                    var s = context.Deserializer;
                    await ExportSpriteGroup(s.DoAt(data.UiOffsetTable.GetPointer(menuSprite), () => s.SerializeObject<GBA_SpriteGroup>(default)), true, menuSprite);
                }
                await Controller.WaitIfNecessary();
            }

            // Enumerate every level
            for (int lev = 0; lev < LevelCount; lev++)
            {
                Debug.Log($"Exporting level {lev + 1}/{LevelCount}");

                settings.Level = lev;

                using (var context = new Context(settings))
                {
                    // Load the ROM
                    await LoadFilesAsync(context);

                    // Read the level
                    var data = LoadDataBlock(context);
                    GBA_Scene lvl = data.Scene;

                    // Enumerate every graphic group
                    await UniTask.WaitForEndOfFrame();

                    foreach (var spr in lvl.Actors.Select(x => x.GraphicData.SpriteGroup).Distinct())
                        await ExportSpriteGroup(spr, false, -1);
                }
            }

            Debug.Log("Finished export");

            async UniTask ExportSpriteGroup(GBA_SpriteGroup spr, bool is8bit, int uioffset)
            {
                if (exported.Contains(spr.Offset))
                    return;
                exported.Add(spr.Offset);

                if (exportAnimFrames) {
                    await ExportAnimations(spr, Path.Combine(outputDir, $"0x{spr.Offset.AbsoluteOffset:X8}"), is8bit);
                } else {
                    ExportSpriteTileSet(spr, outputDir, is8bit, uioffset);
                }
            }
        }

        protected void ExportSpriteTileSet(GBA_SpriteGroup spr, string outputDir, bool is8bit, int uioffset)
        {
            try
            {
                var paletteCount = is8bit ? 1 : spr.Palette.Palette.Length / 16;

                for (int palIndex = 0; palIndex < paletteCount; palIndex++)
                {
                    var numTiles = spr.TileMap.TileMapLength / (is8bit ? 2 : 1);
                    const int wrap = 16;
                    int tileDataSize = (CellSize * CellSize) / (is8bit ? 1 : 2);

                    int tilesX = Math.Min(numTiles, wrap);
                    int tilesY = Mathf.CeilToInt(numTiles / (float)wrap);

                    // Create a texture for the tileset
                    var tex = TextureHelpers.CreateTexture2D(tilesX * CellSize, tilesY * CellSize, true);

                    // Add each tile
                    for (int i = 0; i < numTiles; i++)
                    {
                        int tileY = tilesY - 1 - (i / wrap);
                        int tileX = i % wrap;

                        for (int y = 0; y < CellSize; y++)
                        {
                            for (int x = 0; x < CellSize; x++)
                            {
                                int index = (i * tileDataSize) + ((y * CellSize + x) / (is8bit ? 1 : 2));

                                var v = is8bit ? spr.TileMap.TileMap[index] : BitHelpers.ExtractBits(spr.TileMap.TileMap[index], 4, x % 2 == 0 ? 0 : 4);

                                Color c = spr.Palette.Palette[palIndex * 16 + v].GetColor();

                                if (v != 0)
                                    c = new Color(c.r, c.g, c.b, 1f);

                                tex.SetPixel(tileX * CellSize + x, tileY * CellSize + (CellSize - y - 1), c);
                            }
                        }
                    }

                    tex.Apply();

                    var fileName = $"{(uioffset != -1 ? $"MenuSprite{uioffset}_" : "Sprites_")}{spr.Offset.AbsoluteOffset:X8}_Pal{palIndex}.png";

                    Util.ByteArrayToFile(Path.Combine(outputDir, fileName), tex.EncodeToPNG());
                }
            }
            catch (Exception ex)
            {
                if (uioffset != -1)
                    Debug.Log($"Error for UI offset {uioffset}");

                Debug.LogError($"Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
            }
        }

        protected async UniTask ExportAnimations(GBA_SpriteGroup spr, string outputDir, bool is8bit)
        {
            MagickImage[] sprites = null;

            try
            {
                var commonDesign = GetCommonDesign(spr, is8bit);

                // Convert Texture2D to MagickImage
                sprites = commonDesign.Sprites.Select(x => GetMagickImage(x.texture.GetPixels())).ToArray();

                MagickImage GetMagickImage(IList<Color> pixels)
                {
                    var bytes = new byte[pixels.Count * 4];

                    for (int i = 0; i < pixels.Count; i++)
                    {
                        bytes[i * 4 + 0] = (byte)(pixels[i].a * 255);
                        bytes[i * 4 + 1] = (byte)(pixels[i].b * 255);
                        bytes[i * 4 + 2] = (byte)(pixels[i].g * 255);
                        bytes[i * 4 + 3] = (byte)(pixels[i].r * 255);
                    }
                    
                    var img = new MagickImage(bytes, new PixelReadSettings(8, 8, StorageType.Char, PixelMapping.ABGR));
                    img.Flip();
                    return img;
                }

                var animIndex = 0;

                // Export every animation
                foreach (var anim in commonDesign.Animations)
                {
                    await Controller.WaitIfNecessary();
                    var frameIndex = 0;
                    var animDir = Path.Combine(outputDir, $"{animIndex}-{anim.AnimSpeed}");
                    Directory.CreateDirectory(animDir);
                    if (anim.Frames == null || anim.Frames.Length == 0) continue;


                    /*var shiftX = anim.Frames.Min(f => f.Layers.Select(x => Mathf.Min(0, x.XPosition)).DefaultIfEmpty().Min()) * -1;
                    var shiftY = anim.Frames.Min(f => f.Layers.Select(x => Mathf.Min(0, x.YPosition)).DefaultIfEmpty().Min()) * -1;

                    var maxX = anim.Frames.Max(f => f.Layers.Select(x => x.XPosition).DefaultIfEmpty().Max()) + 8 + shiftX;
                    var maxY = anim.Frames.Max(f => f.Layers.Select(x => x.YPosition).DefaultIfEmpty().Max()) + 8 + shiftY;*/

                    Vector2Int min = new Vector2Int();
                    Vector2Int max = new Vector2Int();
                    foreach (var frame in anim.Frames) {
                        foreach (var layer in frame.Layers) {
                            Vector2 size = new Vector2Int(8, 8);
                            Vector2 pos = new Vector2(layer.XPosition, layer.YPosition);
                            if ((layer.Scale.HasValue && layer.Scale.Value != Vector2.one) || (layer.Rotation.HasValue && layer.Rotation.Value != 0f)) {
                                Vector2 transformOrigin = new Vector2(layer.TransformOriginX, layer.TransformOriginY);
                                Vector2 relativePos = pos - transformOrigin; // Center relative to transform origin
                                
                                // Scale first
                                Vector2 scale = Vector2.one;
                                if (layer.Scale.HasValue && layer.Scale.Value != Vector2.one) {
                                    scale = layer.Scale.Value;
                                    float scaleX = layer.Scale.Value.x;
                                    float scaleY = layer.Scale.Value.y;
                                    if (scaleX == 0f || scaleY == 0f) continue;
                                    if (scaleX > 0f) {
                                        scaleX = Mathf.Ceil(size.x * scaleX) / (size.x);
                                    } else {
                                        scaleX = -Mathf.Ceil(size.x * -scaleX) / (size.x);
                                    }
                                    if (scaleY > 0f) {
                                        scaleY = Mathf.Ceil(size.y * scaleY) / (size.y);
                                    } else {
                                        scaleY = -Mathf.Ceil(size.y * -scaleY) / (size.y);
                                    }
                                    scale = new Vector2(scaleX, scaleY);

                                    relativePos = Vector2.Scale(relativePos, layer.Scale.Value);
                                    size = Vector2.Scale(size, scale);
                                }
                                // Then rotate
                                float rotation = 0f;
                                if (layer.Rotation.HasValue && layer.Rotation.Value != 0) {
                                    rotation = -layer.Rotation.Value;
                                    relativePos = Quaternion.Euler(0f, 0f, rotation) * relativePos;
                                    //size = Quaternion.Euler(0f, 0f, rotation) * size;
                                    // Calculate new bounding box
                                    var newY = Mathf.Abs(size.x * Mathf.Sin(Mathf.Deg2Rad * rotation)) + Mathf.Abs(size.y * Mathf.Cos(Mathf.Deg2Rad * rotation));
                                    var newX = Mathf.Abs(size.x * Mathf.Cos(Mathf.Deg2Rad * rotation)) + Mathf.Abs(size.y * Mathf.Sin(Mathf.Deg2Rad * rotation));
                                    size = new Vector2(newX, newY);
                                }
                                pos = transformOrigin + relativePos;
                            }
                            int x = Mathf.FloorToInt(pos.x);
                            int y = Mathf.FloorToInt(pos.y);
                            if (x < min.x) min.x = x;
                            if (y < min.y) min.y = y;
                            int maxX = Mathf.CeilToInt(pos.x + size.x);
                            int maxY = Mathf.CeilToInt(pos.y + size.y);
                            if (maxX > max.x) max.x = maxX;
                            if (maxY > max.y) max.y = maxY;
                        }
                    }
                    Vector2Int frameImgSize = max - min;
                    if (frameImgSize.x == 0 || frameImgSize.y == 0) continue;

                    foreach (var frame in anim.Frames)
                    {

                        using (var frameImg = new MagickImage(new byte[frameImgSize.x * frameImgSize.y * 4], new PixelReadSettings(frameImgSize.x, frameImgSize.y, StorageType.Char, PixelMapping.ABGR))) {
                            frameImg.FilterType = FilterType.Point;
                            frameImg.Interpolate = PixelInterpolateMethod.Nearest;
                            int layerIndex = 0;
                            foreach (var layer in frame.Layers)
                            {
                                MagickImage img = (MagickImage)sprites[layer.ImageIndex].Clone();
                                Vector2 size = new Vector2(img.Width, img.Height);
                                img.FilterType = FilterType.Point;
                                img.Interpolate = PixelInterpolateMethod.Nearest;
                                img.BackgroundColor = MagickColors.Transparent;
                                if (layer.IsFlippedHorizontally)
                                    img.Flop();

                                if (layer.IsFlippedVertically)
                                    img.Flip();
                                Vector2 pos = new Vector2(layer.XPosition, layer.YPosition);
                                if ((layer.Scale.HasValue && layer.Scale.Value != Vector2.one) || (layer.Rotation.HasValue && layer.Rotation.Value != 0f)) {
                                    pos += size / 2f;
                                    Vector2 transformOrigin = new Vector2(layer.TransformOriginX, layer.TransformOriginY);
                                    Vector2 relativePos = pos - transformOrigin; // Center relative to transform origin
                                    Vector2Int canvas = Vector2Int.one * 128;
                                    img.Extent(canvas.x, canvas.y, Gravity.Center); // 2x max size

                                    // Scale first
                                    Vector2 scale = Vector2.one;
                                    if (layer.Scale.HasValue && layer.Scale.Value != Vector2.one) {
                                        scale = layer.Scale.Value;
                                        float scaleX = layer.Scale.Value.x;
                                        float scaleY = layer.Scale.Value.y;
                                        if (scaleX == 0f || scaleY == 0f) continue;
                                        if (scaleX > 0f) {
                                            scaleX = Mathf.Ceil(size.x * scaleX) / (size.x);
                                        } else {
                                            scaleX = -Mathf.Ceil(size.x * -scaleX) / (size.x);
                                        }
                                        if (scaleY > 0f) {
                                            scaleY = Mathf.Ceil(size.y * scaleY) / (size.y);
                                        } else {
                                            scaleY = -Mathf.Ceil(size.y * -scaleY) / (size.y);
                                        }
                                        scale = new Vector2(scaleX, scaleY);

                                        relativePos = Vector2.Scale(relativePos, layer.Scale.Value);
                                        size = Vector2.Scale(size, layer.Scale.Value);
                                    }
                                    // Then rotate
                                    float rotation = 0f;
                                    if (layer.Rotation.HasValue && layer.Rotation.Value != 0) {
                                        rotation = -layer.Rotation.Value;
                                        relativePos = Quaternion.Euler(0f, 0f, rotation) * relativePos;
                                        size = Quaternion.Euler(0f, 0f, rotation) * size;
                                        // Calculate new bounding box
                                        /*var a = Mathf.Abs(x * Mathf.Sin(o)) + Mathf.Abs(y * Mathf.Cos(o));
                                        var b = Mathf.Abs(x * Mathf.Cos(o)) + Mathf.Abs(y * Mathf.Sin(o));*/


                                    }
                                    if (scale.x < 0f) {
                                        img.Flop();
                                        scale.x = -scale.x;
                                    }
                                    if (scale.y < 0f) {
                                        img.Flip();
                                        scale.y = -scale.y;
                                    }
                                    img.Distort(DistortMethod.ScaleRotateTranslate, new double[] { (canvas.x / 2), (canvas.y / 2), scale.x, scale.y, rotation });
                                    //img.Write(Path.Combine(animDir, $"{frameIndex}___{layerIndex}.png"), MagickFormat.Png);
                                    frameImg.Composite(img,
                                        Mathf.RoundToInt(transformOrigin.x + relativePos.x) - (canvas.x / 2) - min.x,
                                        Mathf.RoundToInt(transformOrigin.y + relativePos.y) - (canvas.y / 2) - min.y, CompositeOperator.Over);
                                } else {
                                    frameImg.Composite(img,
                                        Mathf.RoundToInt(pos.x) - min.x,
                                        Mathf.RoundToInt(pos.y) - min.y, CompositeOperator.Over);
                                }
                                layerIndex++;
                            }

                            frameImg.Write(Path.Combine(animDir, $"{frameIndex}.png"), MagickFormat.Png);
                        }

                        frameIndex++;
                    }

                    animIndex++;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
            }
            finally
            {
                if (sprites != null && sprites.Length > 0)
                    foreach (var s in sprites)
                        s?.Dispose();
            }
        }

        public virtual GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_ROM>(GetROMFilePath, context).Data;
        public virtual GBA_LocLanguageTable LoadLocalization(Context context) => FileFactory.Read<GBA_ROM>(GetROMFilePath, context).Localization;

        public virtual async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            var lvlType = GetLevelType(context.Settings.World);

            GBA_PlayField playField;
            GBA_Scene scene;

            if (lvlType == LevelType.DLC)
            {
                var s = context.Deserializer;

                // Uncomment to load the manifest
                //context.AddFile(new LinearSerializedFile(context)
                //{
                //    filePath = GetGameCubeManifestFilePath
                //});

                //s.DoAt(context.GetFile(GetGameCubeManifestFilePath).StartPointer, () => s.SerializeObject<GBA_GameCubeMapManifest>(default, name: "GameCubeManifest"));

                var mapFilePath = $"map.{context.Settings.Level:000}";

                context.AddFile(new LinearSerializedFile(context)
                {
                    filePath = mapFilePath
                });

                var gcnMap = s.DoAt(context.GetFile(mapFilePath).StartPointer, () => s.SerializeObject<GBA_GameCubeMap>(default, name: "GameCubeMap"));

                playField = gcnMap.PlayField;
                scene = gcnMap.Scene;
            }
            else
            {
                // Load the data block
                var dataBlock = LoadDataBlock(context);

                // Log unused data blocks in offset tables
                var notParsedBlocks = GBA_OffsetTable.OffsetTables.Skip(1).Where(x => x.UsedOffsets.Any(y => !y)).ToArray();
                if (notParsedBlocks.Any())
                    Debug.Log($"The following blocks were never parsed:{Environment.NewLine}" + String.Join(Environment.NewLine, notParsedBlocks.Select(y => $"[{y.Offset}]:" + String.Join(", ", y.UsedOffsets.Select((o, i) => new
                    {
                        Obj = o,
                        Index = i
                    }).Where(o => !o.Obj).Select(o => o.Index.ToString())))));

                if (lvlType == LevelType.Game)
                {
                    scene = dataBlock.Scene;
                    playField = dataBlock.Scene.PlayField;
                }
                else
                {
                    scene = null;
                    playField = dataBlock.MenuLevelPlayfield;
                }
            }

            // Get the map layers, skipping the text layers
            var mapLayers = playField.Layers.Where(x => x.StructType != GBA_TileLayer.TileLayerStructTypes.TextLayerMode7).ToArray();

            // Create a dummy layer for Mode7 background
            if (playField.IsMode7)
            {
                mapLayers = new GBA_TileLayer[]
                {
                    new GBA_TileLayer()
                    {
                        StructType = GBA_TileLayer.TileLayerStructTypes.Layer2D,
                        MapData = playField.Mode7Tiles,
                        Width = 64,
                        Height = 64,
                        ColorMode = GBA_ColorMode.Color8bpp,
                        UsesTileKitDirectly = false
                    }
                }.Concat(mapLayers).ToArray();
            }

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level
            {
                // Create the map array
                Maps = new Unity_Map[mapLayers.Length],

                // Create the events list
                EventData = new List<Unity_Obj>(),
            };

            var tilePalettesCount = context.Settings.EngineVersion == EngineVersion.GBA_BatmanVengeance ? 1 : playField.TileKits[0].PaletteCount;

            var mapDatas = new MapTile[mapLayers.Length][];

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
                    if (map.StructType == GBA_TileLayer.TileLayerStructTypes.RotscaleLayerMode7) {
                        mapData = map.Mode7Data?.Select(x => new MapTile() { TileMapY = playField.BGTileTable.Indices8bpp[x > 0 ? x - 1 : 0] }).ToArray();
                    } else if (!map.UsesTileKitDirectly
                        && context.Settings.EngineVersion != EngineVersion.GBA_SplinterCell_NGage
                        && context.Settings.EngineVersion != EngineVersion.GBA_BatmanVengeance) {
                        //Controller.print(map.MapData?.Max(m => BitHelpers.ExtractBits(m.TileMapY, 10, 0)) + " - " + mapData.Length + " - " + playField.BGTileTable.Data1.Length + " - " + playField.BGTileTable.Data2.Length);
                        //Controller.print(map.MapData?.Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.Data1.Length + " - " + playField.BGTileTable.Data2.Length);
                        //Controller.print(map.MapData?.Where(m=>m.IsFirstBlock).Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount8bpp);
                        //Controller.print(map.MapData?.Where(m => !m.IsFirstBlock).Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount8bpp);
                        //if(map.ColorMode == GBA_ColorMode.Color4bpp) Controller.print(map.LayerID + ": Min:" + map.MapData?.Where(m =>m.TileMapY != 0).Min(m => m.TileMapY) + " - Max:" +map.MapData?.Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount4bpp);

                        GBA_BGTileTable tbl = map.Unk_0E == 1 ? playField.FGTileTable : playField.BGTileTable;

                        mapData = map.MapData?.Select(x => {
                            int index = x.TileMapY;
                            MapTile newt = x.CloneObj();
                            if (map.ColorMode == GBA_ColorMode.Color8bpp) {
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
                                if (index < 0 || index >= tbl.IndicesCount8bpp) {
                                    newt.TileMapY = 0;
                                } else {
                                    newt.TileMapY = tbl.Indices8bpp[index];
                                }
                            } else {
                                index -= 2;
                                if (context.Settings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia
                                && context.Settings.EngineVersion < EngineVersion.GBA_StarWarsTrilogy
                                && playField.BGTileTable != null) {
                                    int numTiles = playField.BGTileTable.IndicesCount8bpp % 128;
                                    index -= numTiles * 2;
                                }
                                if (map.Unk_0E == 1 && playField.BGTileTable != null) {
                                    index -= playField.BGTileTable.IndicesCount4bpp;
                                }
                                if (index < 0 || index >= tbl.IndicesCount4bpp) {
                                    newt.TileMapY = 0;
                                } else {
                                    //Controller.print(index);
                                    newt.TileMapY = tbl.Indices4bpp[index];
                                }
                            }
                            return newt;
                        }).ToArray();
                    }

                    mapDatas[layer] = mapData;

                    level.Maps[layer] = new Unity_Map
                    {
                        Width = map.Width,
                        Height = map.Height,
                        TileSetWidth = 1,
                        MapTiles = mapData.Select(x => new Unity_Tile(x)).ToArray(),
                        IsForeground = map.LayerID == 3
                    };
                    if (map.ShouldSetBGAlphaBlending) {
                        level.Maps[layer].Alpha = map.AlphaBlending_Coeff / 16f;
                    }

                    level.DefaultMap = layer;
                }
            }

            // Cache loaded tilesets
            Dictionary<byte[], Unity_MapTileMap[]> tilesetCache = new Dictionary<byte[], Unity_MapTileMap[]>();

            // Get tileset info for every map
            var tilesetInfos = mapLayers.Select(x => x.StructType == GBA_TileLayer.TileLayerStructTypes.Collision ? new TilesetInfo(null, false, null, null) : GetTilesetInfo(context, playField, x)).ToArray();

            // Load tilesets
            var tilesetIndex = 0;
            for (int layer = 0; layer < mapLayers.Length; layer++)
            {
                var map = mapLayers[layer];

                // Load empty tileset for collision layer
                if (map.StructType == GBA_TileLayer.TileLayerStructTypes.Collision)
                {
                    level.Maps[layer].TileSet = Enumerable.Repeat(new Unity_MapTileMap(new Unity_TileTexture[]
                    {
                        TextureHelpers.CreateTexture2D(CellSize, CellSize, clear: true, applyClear: true).CreateTile()
                    }), tilePalettesCount).ToArray();
                }
                else
                {
                    // If not cached we load the tileset
                    if (!tilesetCache.ContainsKey(tilesetInfos[layer].Tileset))
                    {
                        // Load the tileset and pass in all map data which use it
                        tilesetCache[tilesetInfos[layer].Tileset] = await LoadTilesetsAsync(context, map, tilesetInfos.Select((x, i) => new
                        {
                            Data = x,
                            Index = i
                        }).Where(x => x.Data.Tileset == tilesetInfos[layer].Tileset).SelectMany(x => mapDatas[x.Index]).ToArray(), tilesetInfos[layer], tilesetIndex);

                        tilesetIndex++;
                    }

                    // Se the tileset
                    level.Maps[layer].TileSet = tilesetCache[tilesetInfos[layer].Tileset];
                }
            }

            Controller.DetailedState = $"Loading actors";
            await Controller.WaitIfNecessary();

            level.EventData = new List<Unity_Obj>();

            var des = new Dictionary<int, Unity_ObjGraphics>();

            var eta = new Dictionary<string, R1_EventState[][]>();

            // Add actors
            if (scene != null)
            {
                var actorIndex = 0;

                foreach (var actor in scene.Actors)
                {
                    Controller.DetailedState = $"Loading actor {actorIndex + 1}/{scene.Actors.Length}";
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
                        ForceAlways = actorIndex < scene.AlwaysActorsCount,
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
                                    $"State_Flags: {String.Join(", ", actor.GraphicData?.States.ElementAtOrDefault(actor.StateIndex)?.Flags.GetFlags() ?? new Enum[0])}{Environment.NewLine}" +
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

        public virtual Unity_ObjGraphics GetCommonDesign(GBA_ActorGraphicData graphics) => GetCommonDesign(graphics.SpriteGroup, false);
        public Unity_ObjGraphics GetCommonDesign(GBA_SpriteGroup spr, bool is8bit)
        {
            // Create the design
            var des = new Unity_ObjGraphics
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };

            if (spr == null)
                return des;

            var length = spr.TileMap.TileMapLength / (is8bit ? 2 : 1);
            var tileMap = spr.TileMap;
            var pal = spr.Palette.Palette;
            const int tileWidth = 8;
            int tileSize = (tileWidth * tileWidth) / (is8bit ? 1 : 2);
            var numPalettes = is8bit ? 1 : spr.Palette.Palette.Length / 16;

            // Add sprites for each palette
            for (int palIndex = 0; palIndex < numPalettes; palIndex++)
            {
                for (int i = 0; i < length; i++)
                {
                    var tex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                    for (int y = 0; y < tileWidth; y++)
                    {
                        for (int x = 0; x < tileWidth; x++)
                        {
                            int index = (i * tileSize) + ((y * tileWidth + x) / (is8bit ? 1 : 2));

                            var b = tileMap.TileMap[index];
                            var v = is8bit ? b : BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                            Color c = pal[palIndex * 16 + v].GetColor();

                            if (v != 0)
                                c = new Color(c.r, c.g, c.b, 1f);

                            tex.SetPixel(x, (tileWidth - 1 - y), c);
                        }
                    }

                    tex.Apply();
                    des.Sprites.Add(tex.CreateSprite());
                }
            }
            // Add boxes
            /*int AddBox(Color c) {
                int count = des.Sprites.Count;
                var tex = TextureHelpers.CreateTexture2D(1,1);
                tex.SetPixel(0, 0, c);
                //tex.SetPixels(Enumerable.Repeat(c, width * height).ToArray());

                tex.Apply();
                des.Sprites.Add(tex.CreateSprite());
                return count;
            }
            int boxIndex = des.Sprites.Count;
            AddBox(new Color(1f, 0f, 0f, 0.5f));
            AddBox(new Color(0f, 1f, 0f, 0.5f));*/


            Unity_ObjAnimationPart[] GetPartsForLayer(GBA_SpriteGroup s, GBA_Animation a, int frame, GBA_AnimationChannel l) {
                /*if (l.ChannelType == GBA_AnimationChannel.Type.AttackBox) {
                    return new Unity_ObjAnimationPart[1] {
                        new Unity_ObjAnimationPart() {
                            ImageIndex = boxIndex,
                            XPosition = l.BoxX,
                            YPosition = l.BoxY,
                            TransformOriginX = l.BoxX,
                            TransformOriginY = l.BoxY,
                            Scale = new Vector2(l.BoxX2 - l.BoxX, l.BoxY2 - l.BoxY)
                        }
                    };
                }
                if (l.ChannelType == GBA_AnimationChannel.Type.VulnerabilityBox) {
                    return new Unity_ObjAnimationPart[1] {
                        new Unity_ObjAnimationPart() {
                            ImageIndex = boxIndex+1,
                            XPosition = l.BoxX,
                            YPosition = l.BoxY,
                            TransformOriginX = l.BoxX,
                            TransformOriginY = l.BoxY,
                            Scale = new Vector2(l.BoxX2 - l.BoxX, l.BoxY2 - l.BoxY)
                        }
                    };
                }*/
                if (l.RenderMode == GBA_AnimationChannel.GfxMode.Window
                    || l.RenderMode == GBA_AnimationChannel.GfxMode.Regular
                   // || l.ChannelType == GBA_AnimationChannel.Type.Null
                    || l.ChannelType != GBA_AnimationChannel.Type.Sprite) return new Unity_ObjAnimationPart[0];
                if (l.Color == GBA_ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Unity_ObjAnimationPart[0];
                }
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[l.XSize * l.YSize];

                var imgIndex = l.ImageIndex / (is8bit ? 2 : 1);

                if (imgIndex > length) {
                    Controller.print($"Image index {imgIndex} too high (length {length}) @ : " + spr.Offset + " - " + l.Offset);
                }
                if (l.PaletteIndex > spr.Palette.Palette.Length / 16) {
                    Controller.print("Palette index too high: " + spr.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (spr.Palette.Palette.Length / 16));
                }
                float rot = l.GetRotation(a, s, frame);
                Vector2 scl = l.GetScale(a, s, frame);
                for (int y = 0; y < l.YSize; y++) {
                    for (int x = 0; x < l.XSize; x++) {
                        parts[y * l.XSize + x] = new Unity_ObjAnimationPart {
                            ImageIndex = length * l.PaletteIndex + (imgIndex + y * l.XSize + x),
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

            // Add animations
            foreach (var a in spr.Animations) {
                var unityAnim = new Unity_ObjAnimation();
                unityAnim.AnimSpeed = (byte)(1 + (a.Flags & 0xF));
                var frames = new List<Unity_ObjAnimationFrame>();
                for (int i = 0; i < a.FrameCount; i++) {
                    frames.Add(new Unity_ObjAnimationFrame() {
                        Layers = a.Layers[i].OrderByDescending(l => l.Priority).OrderByDescending(l => l.ChannelType).SelectMany(l => GetPartsForLayer(spr, a, i, l)).Reverse().ToArray()
                    });
                }
                unityAnim.Frames = frames.ToArray();
                des.Animations.Add(unityAnim);
            }

            return des;
        }



        public virtual R1_EventState[][] GetCommonEventStates(GBA_ActorGraphicData graphicData) {
            // Create the states
            if (graphicData == null) return new R1_EventState[0][];
            var eta = new R1_EventState[1][];
            eta[0] = graphicData.States.Select(s => new R1_EventState() {
                AnimationIndex = s.AnimationIndex,
                AnimationSpeed = (byte)(1 + (graphicData.SpriteGroup.Animations[s.AnimationIndex].Flags & 0xF)),
                IsFlippedHorizontally = s.Flags.HasFlag(GBA_ActorState.ActorStateFlags.HorizontalFlip),
                IsFlippedVertically = s.Flags.HasFlag(GBA_ActorState.ActorStateFlags.VerticalFlip)
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

        protected TilesetInfo GetTilesetInfo(Context context, GBA_PlayField playField, GBA_TileLayer map)
        {
            // Get the tileset to use
            byte[] tileset;
            bool is8bpp;
            GBA_Palette[] tilePalettes;
            GBA_AnimatedTileKit[] animatedTilekits = null;
            if (context.Settings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
            {
                is8bpp = map.TileKit.Is8bpp;
                tileset = is8bpp ? map.TileKit.TileSet8bpp : map.TileKit.TileSet4bpp;
                tilePalettes = new GBA_Palette[]
                {
                    playField.TilePalette
                };
            }
            else
            {
                is8bpp = map.ColorMode == GBA_ColorMode.Color8bpp;
                tileset = is8bpp ? playField.TileKits[map.Unk_0E].TileSet8bpp : playField.TileKits[map.Unk_0E].TileSet4bpp;
                tilePalettes = playField.TileKits[map.Unk_0E].Palettes.Distinct().ToArray();
                animatedTilekits = playField.TileKits[map.Unk_0E].AnimatedTileKits?.Where(atk => atk.Is8Bpp == (map.ColorMode == GBA_ColorMode.Color8bpp)).ToArray();
            }

            return new TilesetInfo(tileset, is8bpp, tilePalettes, animatedTilekits);
        }
        protected async UniTask<Unity_MapTileMap[]> LoadTilesetsAsync(Context context, GBA_TileLayer map, MapTile[] mapData, TilesetInfo info, int tilesetIndex)
        {
            Controller.DetailedState = $"Loading tileset {tilesetIndex + 1}";
            await Controller.WaitIfNecessary();

            int tileSize = (info.Is8bpp ? (CellSize * CellSize) : (CellSize * CellSize) / 2);
            int tilesetLength = (info.Tileset.Length / (info.Is8bpp ? (CellSize*CellSize) : (CellSize * CellSize)/2)) + 1;

            Unity_AnimatedTile[] animatedTiles = null;
            if (info.AnimatedTilekits != null) {
                int[] GetIndicesFrom(int start, int step, int count) {
                    int[] indices = new int[count];
                    for (int i = 0; i < count; i++) {
                        indices[i] = start + step * i;
                    }
                    return indices;
                }

                animatedTiles = info.AnimatedTilekits.SelectMany(atk => atk.TileIndices.Where(atkt => atkt != 0).Select(atkt => new Unity_AnimatedTile() {
                    AnimationSpeed = atk.AnimationSpeed / 2f,
                    TileIndices = GetIndicesFrom(atkt, atk.TilesStep, atk.NumFrames)
                })).ToArray();
            }

            const int paletteSize = 16;

            var output = new Unity_MapTileMap[info.TilePalettes.Length];

            var wrap = 4096 / CellSize;

            int tilesX = Math.Min(tilesetLength, wrap);
            int tilesY = Mathf.CeilToInt(tilesetLength / (float)wrap);

            var tileSetTex = TextureHelpers.CreateTexture2D(tilesX * CellSize, tilesY * CellSize * info.TilePalettes.Length);
            Unity_TileTexture empty = TextureHelpers.CreateTexture2D(CellSize, CellSize, clear: true, applyClear: true).CreateTile();
            Unity_TileTexture[][] tiles = new Unity_TileTexture[info.TilePalettes.Length][];
            for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                tiles[tilePal] = new Unity_TileTexture[tilesetLength];
                tiles[tilePal][0] = empty;
            }
            
            for (int i = 1; i < tilesetLength; i++)
            {
                Controller.DetailedState = $"Loading tileset {tilesetIndex + 1} (tile {i-1}/{tilesetLength-1})";
                if(i % 64 == 1) await Controller.WaitIfNecessary();
                int tileY = ((i / wrap)) * CellSize;
                int tileX = (i % wrap) * CellSize;

                // Get the palette to use
                var pals = mapData.Where(x => x.TileMapY == i).Select(x => x.PaletteIndex).Distinct().ToArray();

                if (pals.Length > 1)
                    Debug.LogWarning($"Tile {i} has several possible palettes: {String.Join(", ", pals)}");

                int p = pals.FirstOrDefault();

                for (int y = 0; y < CellSize; y++)
                {
                    for (int x = 0; x < CellSize; x++)
                    {
                        Color c;

                        int index = ((i - 1) * tileSize) + ((y * CellSize + x) / (info.Is8bpp ? 1 : 2));

                        if (info.Is8bpp)
                        {
                            var b = info.Tileset[index];
                            for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                                c = info.TilePalettes[tilePal].Palette[b].GetColor();

                                if (b != 0)
                                    c = new Color(c.r, c.g, c.b, 1f);

                                tileSetTex.SetPixel(tileX + x, tileY + y + (tilePal * tilesY * CellSize), c);
                            }
                        }
                        else
                        {
                            var b = info.Tileset[index];
                            var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);
                            for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                                c = info.TilePalettes[tilePal].Palette[p * paletteSize + v].GetColor();

                                if (v != 0)
                                    c = new Color(c.r, c.g, c.b, 1f);

                                tileSetTex.SetPixel(tileX + x, tileY + y + (tilePal * tilesY * CellSize), c);
                            }
                        }


                        for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                            // Create a tile
                            tiles[tilePal][i] = tileSetTex.CreateTile(new Rect(tileX, tileY + (tilePal * tilesY * CellSize), CellSize, CellSize));
                        }
                    }
                }
            }

            for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                output[tilePal] = new Unity_MapTileMap(tiles[tilePal])
                {
                    AnimatedTiles = animatedTiles
                };
            }

            tileSetTex.Apply();

            return output;
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

        protected class TilesetInfo
        {
            public TilesetInfo(byte[] tileset, bool is8Bpp, GBA_Palette[] tilePalettes, GBA_AnimatedTileKit[] animatedTilekits)
            {
                Tileset = tileset;
                Is8bpp = is8Bpp;
                TilePalettes = tilePalettes;
                AnimatedTilekits = animatedTilekits;
            }

            public byte[] Tileset { get; }
            public bool Is8bpp { get; }
            public GBA_Palette[] TilePalettes { get; }
            public GBA_AnimatedTileKit[] AnimatedTilekits { get; }
        }
    }
}