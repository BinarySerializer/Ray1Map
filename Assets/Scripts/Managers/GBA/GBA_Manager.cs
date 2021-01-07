using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBA_Manager : IGameManager
    {
        public int CellSize { get; set; } = 8;

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

        public LevelType GetLevelType(Context context)
        {
            var worlds = WorldLevels.Length;

            if (context.Settings.World == worlds && MenuLevels.Any())
                return LevelType.Menu;

            if (context.Settings.World == (worlds + 1) && DLCLevelCount > 0)
                return LevelType.DLC;

            return LevelType.Game;
        }

        public virtual string GetROMFilePath(Context context) => $"ROM.gba";
        public virtual string GetGameCubeManifestFilePath => $"gba.nfo";

        public virtual long ActorTypeTableLength => 0;
        public abstract IEnumerable<int>[] WorldLevels { get; }
        public int LevelCount => WorldLevels.Select(x => x.Count()).Sum();
        public abstract int[] MenuLevels { get; }
        public abstract int DLCLevelCount { get; }
        public abstract int[] AdditionalSprites4bpp { get; }
        public abstract int[] AdditionalSprites8bpp { get; }

        public virtual GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
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
                var file = context.GetFile(GetROMFilePath(context));

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
                    await ExportSpriteGroup(s.DoAt(data.UiOffsetTable.GetPointer(menuSprite), () => s.SerializeObject<GBA_Puppet>(default)), false, menuSprite);
                }
                await Controller.WaitIfNecessary();

                foreach (var menuSprite in AdditionalSprites8bpp)
                {
                    var s = context.Deserializer;
                    await ExportSpriteGroup(s.DoAt(data.UiOffsetTable.GetPointer(menuSprite), () => s.SerializeObject<GBA_Puppet>(default)), true, menuSprite);
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

                    foreach (var spr in lvl.GetAllActors(settings).Select(x => x.ActorModel.Puppet).Distinct())
                        await ExportSpriteGroup(spr, false, -1);
                }
            }

            Debug.Log("Finished export");

            async UniTask ExportSpriteGroup(GBA_Puppet spr, bool is8bit, int uioffset)
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

        protected void ExportSpriteTileSet(GBA_Puppet spr, string outputDir, bool is8bit, int uioffset)
        {
            try
            {
                var paletteCount = is8bit ? 1 : spr.Palette.Palette.Length / 16;

                for (int palIndex = 0; palIndex < paletteCount; palIndex++)
                {
                    var numTiles = spr.TileSet.TileSetLength / (is8bit ? 2 : 1);
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

                                var v = is8bit ? spr.TileSet.TileSet[index] : BitHelpers.ExtractBits(spr.TileSet.TileSet[index], 4, x % 2 == 0 ? 0 : 4);

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

        protected async UniTask ExportAnimations(GBA_Puppet spr, string outputDir, bool is8bit)
        {
            MagickImage[] sprites = null;

            try
            {
                var commonDesign = GetCommonDesign(spr, is8bit);

                // Convert Texture2D to MagickImage
                sprites = commonDesign.Sprites.Select(x => x.texture.ToMagickImage()).ToArray();

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
                        foreach (var layer in frame.SpriteLayers) {
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
                            foreach (var layer in frame.SpriteLayers)
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

        public virtual GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_ROM>(GetROMFilePath(context), context).Data;
        public virtual GBA_LocLanguageTable LoadLocalizationTable(Context context) => FileFactory.Read<GBA_ROM>(GetROMFilePath(context), context).Localization;
        public virtual Dictionary<string, string[]> LoadLocalization(Context context)
        {
            var strings = LoadLocalizationTable(context)?.StringGroups;
            Dictionary<string, string[]> loc = null;

            if (strings != null)
            {
                loc = new Dictionary<string, string[]>();

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
                    loc.Add(languages[i], strings[i].LocStrings.SelectMany(x => x.Strings).ToArray());
            }

            return loc;
        }

        public virtual async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            var lvlType = GetLevelType(context);

            GBA_PlayField playField;
            GBA_Scene scene;

            // Load the data block
            var dataBlock = lvlType == LevelType.DLC ? null : LoadDataBlock(context);

            if (lvlType == LevelType.DLC)
            {
                var s = context.Deserializer;

                // Uncomment to load the manifest
                //context.AddFile(new LinearSerializedFile(context)
                //{
                //    filePath = GetGameCubeManifestFilePath
                //});

                //s.DoAt(context.GetFile(GetGameCubeManifestFilePath).StartPointer, () => s.SerializeObject<GBA_GameCubeMapManifest>(default, name: "GameCubeManifest"));

                var gcnFile = await context.AddLinearSerializedFileAsync($"map.{context.Settings.Level:000}");

                var gcnMap = s.DoAt(gcnFile.StartPointer, () => s.SerializeObject<GBA_GameCubeMap>(default, name: "GameCubeMap"));

                playField = gcnMap.PlayField;
                scene = gcnMap.Scene;
            }
            else
            {
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
                    if (context.Settings.GBA_IsCommon)
                    {
                        scene = dataBlock.Scene;
                        playField = dataBlock.Scene.PlayField;
                    }
                    else if (context.Settings.EngineVersion == EngineVersion.GBA_R3_MadTrax)
                    {
                        scene = null;

                        // Create a dummy playfield to store the layers in
                        playField = new GBA_PlayField()
                        {
                            Layers = new GBA_PlayField[]
                            {
                                dataBlock.MadTraxPlayField_BG,
                                dataBlock.MadTraxPlayField_FG,
                            }.SelectMany(x => x.Layers).ToArray(),
                        };
                    }
                    else if (context.Settings.GBA_IsMilan)
                    {
                        scene = null;
                        playField = dataBlock.Milan_SceneList.Scene.PlayField;
                    }
                    else // Shanghai
                    {
                        scene = null;
                        playField = dataBlock.Shanghai_Scene.CombinedPlayField;
                    }
                }
                else
                {
                    scene = null;
                    playField = dataBlock.MenuLevelPlayfield;
                }
            }

            await Controller.WaitIfNecessary();

            // Get the map layers, skipping the text layers
            var mapLayers = playField.Layers.Where(x => x.StructType != GBA_TileLayer.Type.TextLayerMode7).ToArray();

            // Create layers for Mode7 background
            if (playField.StructType == GBA_PlayField.Type.PlayFieldMode7)
            {
                // Get Mode7 text layers
                var mode7Layers = playField.Layers.Where(x => x.StructType == GBA_TileLayer.Type.TextLayerMode7).ToArray();

                int currentY = 0;
                int currentBlock = 0;
                foreach (GBA_TileLayer l in mode7Layers)
                {
                    MapTile[] t = new MapTile[l.Width * l.Height];
                    const int screenBlockWidth = 32;
                    for (int y = 0; y < l.Height; y++) {
                        for (int x = 0; x < l.Width; x++) {
                            int screenblock = (currentBlock + (x / screenBlockWidth)) * 1024;
                            t[y * l.Width + x] = playField.Mode7Tiles[(currentY + y) * screenBlockWidth + (x % screenBlockWidth) + screenblock];
                        }
                    }
                    currentY += l.Height;
                    if (currentY >= 32) {
                        currentY -= 32;
                        currentBlock += 2;
                    }
                    l.MapData = t;
                }
                mapLayers = mapLayers.Concat(mode7Layers).ToArray();
            }

            // Create a normal 8x8 map array for Shanghai games with 32x8 maps
            foreach (var l in mapLayers.Where(x => x.Cluster?.Shanghai_MapTileSize == 1 || x.Cluster?.Shanghai_MapTileSize == 2))
            {
                var indexArray = l.Cluster.Shanghai_MapTileSize == 1 ? l.Shanghai_MapIndices_8.Select(x => (ushort)x).ToArray() : l.Shanghai_MapIndices_16;

                l.MapData = new MapTile[l.Width * l.Height]; //new MapTile[Mathf.CeilToInt(l.Width / 4f) * 4 * l.Height];
                int indexWidth = Mathf.CeilToInt(l.Width / 4f) * 4;
                for (int y = 0; y < l.Height; y++) {
                    for (int x = 0; x < l.Width; x++) {
                        int i = y * indexWidth + x;
                        var mapTile = indexArray[i / 4];

                        if (context.Settings.GBA_IsMilan)
                            mapTile = (ushort)BitHelpers.ExtractBits(mapTile, 12, 0);

                        var offX = i % 4;
                        var tile = l.Shanghai_MapTiles[mapTile * 4 + offX];

                        l.MapData[y * l.Width + x] = new MapTile() {
                            TileMapY = tile.TileMapY,
                            HorizontalFlip = tile.HorizontalFlip,
                            VerticalFlip = tile.VerticalFlip,
                            PaletteIndex = tile.PaletteIndex
                        };
                    }
                }
            }

            // Create a normal 8x8 map array for Milan games with 16x16 maps
            if (context.Settings.GBA_IsMilan)
            {
                foreach (var l in mapLayers.Where(x => x.StructType == GBA_TileLayer.Type.Layer2D))
                {
                    var indexArray = l.Shanghai_MapIndices_16;

                    var indexArrayWidth = l.Width / 2;
                    var indexArrayHeight = l.Height / 2;

                    l.MapData = new MapTile[l.Width * l.Height];

                    for (int y = 0; y < indexArrayHeight; y++)
                    {
                        for (int x = 0; x < indexArrayWidth; x++)
                        {
                            var actualX = x * 2;
                            var actualY = y * 2;

                            var mapTile = BitHelpers.ExtractBits(indexArray[y * indexArrayWidth + x], 12, 0);

                            setTileAt(0, 0, l.Shanghai_MapTiles[mapTile * 4 + 0]);
                            setTileAt(1, 0, l.Shanghai_MapTiles[mapTile * 4 + 1]);
                            setTileAt(0, 1, l.Shanghai_MapTiles[mapTile * 4 + 2]);
                            setTileAt(1, 1, l.Shanghai_MapTiles[mapTile * 4 + 3]);

                            void setTileAt(int offX, int offY, MapTile tile)
                            {
                                var outputX = actualX + offX;
                                var outputY = actualY + offY;

                                l.MapData[outputY * l.Width + outputX] = new MapTile()
                                {
                                    TileMapY = tile.TileMapY,
                                    HorizontalFlip = tile.HorizontalFlip,
                                    VerticalFlip = tile.VerticalFlip,
                                    PaletteIndex = tile.PaletteIndex
                                };
                            }
                        }
                    }
                }
            }

            Controller.DetailedState = $"Loading actor models & puppets";
            await Controller.WaitIfNecessary();

            var objManager = GetObjectManager(context, scene, dataBlock);

            var mapDatas = new MapTile[mapLayers.Length][];
            var maps = new Unity_Map[mapLayers.Length];

            // Add every map
            for (int layer = 0; layer < mapLayers.Length; layer++)
            {
                Controller.DetailedState = $"Loading map {layer + 1}/{mapLayers.Length}";
                await Controller.WaitIfNecessary();

                var map = mapLayers[layer];

                if (map.StructType == GBA_TileLayer.Type.Collision)
                {
                    maps[layer] = new Unity_Map {
                        Type = Unity_Map.MapType.Collision,
                        Width = map.Width,
                        Height = map.Height,
                        MapTiles = map.CollisionData.Select((x, i) => new Unity_Tile(new MapTile()
                        {
                            CollisionType = (byte)x
                        })).ToArray(),
                    };
                }
                else
                {
                    MapTile[] mapData = map.MapData;
                    //MapTile[] bgData = playField.BGTileTable.Indices1.Concat(playField.BGTileTable.Indices2).ToArray();
                    if (map.StructType == GBA_TileLayer.Type.RotscaleLayerMode7) {
                        mapData = map.Mode7Data?.Select(x => new MapTile() {
                            TileMapY = playField.BGTileTable.Indices8bpp[x > 0 ? x - 1 : 0]
                        }).ToArray();
                    } else if (!map.UsesTileKitDirectly
                        && context.Settings.EngineVersion != EngineVersion.GBA_SplinterCell_NGage
                        && context.Settings.EngineVersion > EngineVersion.GBA_BatmanVengeance) {
                        //Controller.print(map.MapData?.Max(m => BitHelpers.ExtractBits(m.TileMapY, 10, 0)) + " - " + mapData.Length + " - " + playField.BGTileTable.Data1.Length + " - " + playField.BGTileTable.Data2.Length);
                        //Controller.print(map.MapData?.Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.Data1.Length + " - " + playField.BGTileTable.Data2.Length);
                        //Controller.print(map.MapData?.Where(m=>m.IsFirstBlock).Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount8bpp);
                        //Controller.print(map.MapData?.Where(m => !m.IsFirstBlock).Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount8bpp);
                        //if(map.ColorMode == GBA_ColorMode.Color4bpp) Controller.print(map.LayerID + ": Min:" + map.MapData?.Where(m =>m.TileMapY != 0).Min(m => m.TileMapY) + " - Max:" +map.MapData?.Max(m => m.TileMapY) + " - " + mapData.Length + " - " + playField.BGTileTable.IndicesCount4bpp);

                        GBA_BGTileTable tbl = map.TileKitIndex == 1 ? playField.FGTileTable : playField.BGTileTable;

                        mapData = map.MapData?.Select(x => {
                            int index = x.TileMapY;
                            MapTile newt = x.CloneObj();
                            if (map.ColorMode == GBA_ColorMode.Color8bpp) {
                                if (x.IsFirstBlock) {
                                    //Controller.print(i + " - " + index);
                                    //relativeIndex = index;
                                } else {
                                    if (index > 0) {
                                        if (x.GBATileType == MapTile.GBA_TileType.Mode7Tile) {
                                            index += 256;
                                        } else {
                                            index += 384; // 3 * 128, 24 * 16?
                                        }
                                        index -= 1;
                                        //index += relativeIndex; // 383; // seems hardcoded
                                    } else {
                                        index = -1;
                                    }
                                }
                                if (x.GBATileType == MapTile.GBA_TileType.Mode7Tile) {
                                    index -= 1;
                                }
                                if (index < 0 || index >= tbl.IndicesCount8bpp) {
                                    //Controller.print(map.LayerID + " - " + index);
                                    newt.TileMapY = 0;
                                } else {
                                    newt.TileMapY = tbl.Indices8bpp[index];
                                }
                            } else {
                                index -= 2;
                                if (context.Settings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia
                                //&& context.Settings.EngineVersion < EngineVersion.GBA_StarWarsTrilogy
                                && playField.BGTileTable != null
                                && playField.BGTileTable.IndicesCount8bpp > 383) {
                                    int numTiles = playField.BGTileTable.IndicesCount8bpp % 384;
                                    index -= numTiles * 2;
                                }
                                if (map.TileKitIndex == 1 && playField.BGTileTable != null) {
                                    index -= playField.BGTileTable.IndicesCount4bpp;
                                }
                                if (index < 0 || index >= tbl.IndicesCount4bpp) {
                                    //Controller.print(map.LayerID + " - " + index);
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

                    maps[layer] = new Unity_Map {
                        Type = Unity_Map.MapType.Graphics,
                        Width = map.Width,
                        Height = map.Height,
                        MapTiles = mapData.Select(x => new Unity_Tile(x)).ToArray(),
                        Layer = (map.LayerID == 3 && map.StructType != GBA_TileLayer.Type.TextLayerMode7) ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle
                    };
                    if (map.ShouldSetBGAlphaBlending) {
                        maps[layer].Alpha = map.AlphaBlending_Coeff / 16f;
                    }
                }
            }

            // Cache loaded tilesets
            Dictionary<byte[], Unity_TileSet[]> tilesetCache = new Dictionary<byte[], Unity_TileSet[]>();

            // Get tileset info for every map
            var tilesetInfos = mapLayers.Select(x => x.StructType == GBA_TileLayer.Type.Collision ? new TilesetInfo(null, false, null, null) : GetTilesetInfo(context, playField, x, dataBlock)).ToArray();

            // Load tilesets
            var tilesetIndex = 0;
            for (int layer = 0; layer < mapLayers.Length; layer++)
            {
                var map = mapLayers[layer];

                // Load empty tileset for collision layer
                if (map.StructType == GBA_TileLayer.Type.Collision)
                {
                    maps[layer].TileSet = new Unity_TileSet[] {
                        new Unity_TileSet(new Unity_TileTexture[] {
                            TextureHelpers.CreateTexture2D(CellSize, CellSize, clear: true, applyClear: true).CreateTile()
                        })
                    };
                }
                else
                {
                    // If not cached we load the tileset
                    if (!tilesetCache.ContainsKey(tilesetInfos[layer].Tileset))
                    {
                        // Load the tileset and pass in all map data which use it
                        tilesetCache[tilesetInfos[layer].Tileset] = await LoadTilesetsAsync(tilesetInfos.Select((x, i) => new
                        {
                            Data = x,
                            Index = i
                        }).Where(x => x.Data.Tileset == tilesetInfos[layer].Tileset).SelectMany(x => mapDatas[x.Index]).ToArray(), tilesetInfos[layer], tilesetIndex);

                        tilesetIndex++;
                    }

                    // Se the tileset
                    maps[layer].TileSet = tilesetCache[tilesetInfos[layer].Tileset];
                }
            }

            Controller.DetailedState = $"Loading actors";
            await Controller.WaitIfNecessary();

            // Get actors
            var actors = new List<Unity_Object>(GetObjects(context, scene, objManager, dataBlock));

            return new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: actors,
                defaultCollisionMap: mapLayers.FindItemIndex(x => x.StructType == GBA_TileLayer.Type.Collision),
                localization: LoadLocalization(context),
                cellSize: CellSize,
                getCollisionTypeNameFunc: x => playField.StructType == GBA_PlayField.Type.PlayFieldMode7 ? ((GBA_Mode7TileCollisionType)x).ToString() : ((GBA_TileCollisionType)x).ToString(),
                getCollisionTypeGraphicFunc: x => playField.StructType == GBA_PlayField.Type.PlayFieldMode7 ? ((GBA_Mode7TileCollisionType)x).GetCollisionTypeGraphic() : ((GBA_TileCollisionType)x).GetCollisionTypeGraphic(context.Settings.EngineVersion),
                sectors: GetSectors(scene, dataBlock));
        }

        public virtual Unity_ObjGraphics GetCommonDesign(GBA_ActorModel graphics, GBA_Data data) => GetCommonDesign(graphics.Puppet, false);
        public virtual Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager_GBA(context, LoadActorModels(context, scene?.GetAllActors(context.Settings) ?? new GBA_Actor[0], data));
        public virtual IEnumerable<Unity_Object> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => scene?.GetAllActors(context.Settings).Select(a => new Unity_Object_GBA(a, (Unity_ObjectManager_GBA)objManager)) ?? new Unity_Object_GBA[0];
        public virtual Unity_Sector[] GetSectors(GBA_Scene scene, GBA_Data data) => scene?.Knots.Select(x => new Unity_Sector(x.ActorIndices.Concat(x.CaptorIndices ?? new byte[0]).Select(y => (int)y).ToList())).ToArray();
        public Unity_ObjGraphics GetCommonDesign(GBA_Puppet puppet, bool is8bit)
        {
            // Create the design
            var des = new Unity_ObjGraphics
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };

            if (puppet == null)
                return des;

            var tileSetLength = puppet.Context.Settings.GBA_IsMilan ? (is8bit ? puppet.Milan_TileKit.TileSet8bppSize : puppet.Milan_TileKit.TileSet4bppSize) : puppet.TileSet.TileSetLength;

            var length = tileSetLength / (is8bit ? 2 : 1);
            var tileSet = puppet.Context.Settings.GBA_IsMilan ? (is8bit ? puppet.Milan_TileKit.TileSet8bpp : puppet.Milan_TileKit.TileSet4bpp) : puppet.TileSet.TileSet;
            var pal = puppet.Palette.Palette;
            const int tileWidth = 8;
            int tileSize = (tileWidth * tileWidth) / (is8bit ? 1 : 2);
            var numPalettes = is8bit ? 1 : puppet.Palette.Palette.Length / 16;

            // Add sprites for each palette
            var pal_split = Util.ConvertAndSplitGBAPalette((RGBA5551Color[])pal);
            for (int palIndex = 0; palIndex < numPalettes; palIndex++)
            {
                var tileSetTex = Util.ToTileSetTexture(tileSet, pal_split[palIndex], Util.TileEncoding.Linear_4bpp, CellSize, false, flipTileY: true);

                // Extract every sprite
                for (int i = 0; i < tileSetLength; i++) {
                    int x = i % 32;
                    int y = i / 32;
                    des.Sprites.Add(tileSetTex.CreateSprite(rect: new Rect(x * CellSize, y * CellSize, CellSize, CellSize)));
                }
            }

            Unity_ObjAnimationPart[] GetPartsForLayer(GBA_Puppet s, GBA_Animation a, int frame, GBA_AnimationChannel l) {
                if (l.RenderMode == GBA_AnimationChannel.GfxMode.Window
                    || l.RenderMode == GBA_AnimationChannel.GfxMode.Regular
                    || l.ChannelType != GBA_AnimationChannel.Type.Sprite) 
                    return new Unity_ObjAnimationPart[0];

                if (l.Color == GBA_ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Unity_ObjAnimationPart[0];
                }
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[l.XSize * l.YSize];

                var imgIndex = l.ImageIndex / (is8bit ? 2 : 1);

                if (imgIndex > length) {
                    Controller.print($"Image index {imgIndex} too high (length {length}) @ : " + puppet.Offset + " - " + l.Offset);
                }
                if (l.PaletteIndex > puppet.Palette.Palette.Length / 16) {
                    Controller.print("Palette index too high: " + puppet.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (puppet.Palette.Palette.Length / 16));
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

            Unity_ObjAnimationCollisionPart[] GetCollisionPartsForLayer(GBA_Puppet s, GBA_Animation a, GBA_AnimationChannel l) 
            {
                if (l.ChannelType != GBA_AnimationChannel.Type.AttackBox && l.ChannelType != GBA_AnimationChannel.Type.VulnerabilityBox) 
                    return new Unity_ObjAnimationCollisionPart[0];

                return new Unity_ObjAnimationCollisionPart[]
                {
                    new Unity_ObjAnimationCollisionPart() {

                        XPosition = l.Box_MinX,
                        YPosition = l.Box_MinY,
                        Width = l.Box_MaxX - l.Box_MinX,
                        Height = l.Box_MaxY - l.Box_MinY,
                        Type = l.ChannelType == GBA_AnimationChannel.Type.AttackBox ? Unity_ObjAnimationCollisionPart.CollisionType.AttackBox : Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox
                    }
                };
            }

            // Add animations
            foreach (var a in puppet.Animations) 
            {
                var unityAnim = new Unity_ObjAnimation
                {
                    AnimSpeed = (byte) (1 + (a.Flags & 0xF))
                };

                var frames = new List<Unity_ObjAnimationFrame>();

                for (int i = 0; i < a.FrameCount; i++) {
                    frames.Add(new Unity_ObjAnimationFrame(
                        a.Layers[i].OrderByDescending(l => l.Priority).OrderByDescending(l => l.ChannelType).SelectMany(l => GetPartsForLayer(puppet, a, i, l)).Reverse().ToArray(),
                        a.Layers[i].OrderByDescending(l => l.Priority).OrderByDescending(l => l.ChannelType).SelectMany(l => GetCollisionPartsForLayer(puppet, a, l)).Reverse().ToArray()
                        ));
                }

                unityAnim.Frames = frames.ToArray();
                des.Animations.Add(unityAnim);
            }

            return des;
        }

        public Unity_ObjectManager_GBA.ModelData[] LoadActorModels(Context context, IEnumerable<GBA_Actor> actors, GBA_Data data)
        {
            var graphicsData = new List<Unity_ObjectManager_GBA.ModelData>();
            var cachedPuppets = new Dictionary<GBA_Puppet, Unity_ObjGraphics>();

            foreach (var actor in actors)
            {
                // Make sure the model hasn't already been loaded
                if (graphicsData.Any(x => x.Index == actor.Index_ActorModel))
                    continue;

                // Make sure there is an actor model
                if (actor.ActorModel == null)
                    continue;

                // Get the puppets
                var modelPuppets = actor.ActorModel.GetPuppets;
                var loadedPuppets = new Unity_ObjGraphics[modelPuppets.Length];

                for (int i = 0; i < modelPuppets.Length; i++)
                    loadedPuppets[i] = cachedPuppets.ContainsKey(modelPuppets[i]) ? cachedPuppets[modelPuppets[i]] : cachedPuppets[modelPuppets[i]] = GetCommonDesign(modelPuppets[i], context.Settings.GBA_IsMilan && modelPuppets[i].Milan_TileKit.Is8bpp);

                graphicsData.Add(new Unity_ObjectManager_GBA.ModelData(actor.Index_ActorModel, context.Settings.GBA_IsMilan ? actor.ActorModel.Milan_Actions.Blocks.Select(y => y.Action).ToArray() : actor.ActorModel.Actions, loadedPuppets, actor.ActorModel.Milan_ActorID));
            }

            return graphicsData.ToArray();
        }

        protected TilesetInfo GetTilesetInfo(Context context, GBA_PlayField playField, GBA_TileLayer map, GBA_Data data)
        {
            // Get the tileset to use
            byte[] tileset;
            bool is8bpp;
            GBA_Palette[] tilePalettes;
            GBA_AnimatedTileKit[] animatedTilekits = null;
            if (context.Settings.EngineVersion <= EngineVersion.GBA_BatmanVengeance)
            {
                is8bpp = map.TileKit.Is8bpp;
                tileset = is8bpp ? map.TileKit.TileSet8bpp : map.TileKit.TileSet4bpp;

                GBA_Palette pal;

                if (context.Settings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
                    pal = playField.TilePalette;
                else if (context.Settings.EngineVersion == EngineVersion.GBA_R3_MadTrax)
                    pal = data.MadTraxPalette;
                else if (context.Settings.GBA_IsMilan)
                    pal = map.TileKit.Palettes[0];
                else
                    pal = data.Shanghai_Scene.TilePal;

                tilePalettes = new GBA_Palette[]
                {
                    pal
                };
            }
            else
            {
                is8bpp = map.ColorMode == GBA_ColorMode.Color8bpp;
                tileset = is8bpp ? playField.TileKits[map.TileKitIndex].TileSet8bpp : playField.TileKits[map.TileKitIndex].TileSet4bpp;
                tilePalettes = playField.TileKits[map.TileKitIndex].Palettes.Distinct().ToArray();
                animatedTilekits = playField.TileKits[map.TileKitIndex].AnimatedTileKits?.Where(atk => atk.Is8Bpp == (map.ColorMode == GBA_ColorMode.Color8bpp)).ToArray();
            }

            return new TilesetInfo(tileset, is8bpp, tilePalettes, animatedTilekits);
        }
        protected async UniTask<Unity_TileSet[]> LoadTilesetsAsync(MapTile[] mapData, TilesetInfo info, int tilesetIndex)
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

            var output = new Unity_TileSet[info.TilePalettes.Length];

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
            Dictionary<int, Dictionary<int, Color>> paletteCache4 = new Dictionary<int, Dictionary<int, Color>>();
            Dictionary<int, Dictionary<int, Color>> paletteCache8 = new Dictionary<int, Dictionary<int, Color>>();

            Color GetColor(bool is8Bit, int palette, int index, int pal4bpp = 0) {
                Color c;
                if (is8Bit) {
                    if (paletteCache8.ContainsKey(palette) && paletteCache8[palette].ContainsKey(index))
                        return paletteCache8[palette][index];
                } else {
                    if (paletteCache4.ContainsKey(palette) && paletteCache4[palette].ContainsKey(pal4bpp * paletteSize + index))
                        return paletteCache4[palette][pal4bpp * paletteSize + index];
                }
                if (is8Bit) {
                    c = info.TilePalettes[palette].Palette[index].GetColor();
                } else {
                    c = info.TilePalettes[palette].Palette[pal4bpp * paletteSize + index].GetColor();
                }
                if (index != 0)
                    c = new Color(c.r, c.g, c.b, 1f);

                if (is8Bit) {
                    if (!paletteCache8.ContainsKey(palette))
                        paletteCache8[palette] = new Dictionary<int, Color>();
                    paletteCache8[palette][index] = c;
                } else {
                    if (!paletteCache4.ContainsKey(palette))
                        paletteCache4[palette] = new Dictionary<int, Color>();
                    paletteCache4[palette][pal4bpp * paletteSize + index] = c;
                }
                return c;
            }
            Dictionary<int, int> tilesetPalettes = new Dictionary<int, int>();
            for (int i = 0; i < mapData.Length; i++) {
                var m = mapData[i];
                if (!tilesetPalettes.ContainsKey(m.TileMapY)) {
                    tilesetPalettes[m.TileMapY] = m.PaletteIndex;
                } else if(tilesetPalettes[m.TileMapY] != m.PaletteIndex) {
                    Debug.LogWarning($"Tile {m.TileMapY} has several possible palettes: {tilesetPalettes[m.TileMapY]} - {m.PaletteIndex}");
                }
            }
            for (int i = 1; i < tilesetLength; i++)
            {
                Controller.DetailedState = $"Loading tileset {tilesetIndex + 1} (tile {i-1}/{tilesetLength-1})";
                if(i % 64 == 1) await Controller.WaitIfNecessary();
                int tileY = ((i / wrap)) * CellSize;
                int tileX = (i % wrap) * CellSize;

                // Get the palette to use
                int p = 0;
                if (!tilesetPalettes.ContainsKey(i)) {
                    if (animatedTiles != null) {
                        Unity_AnimatedTile animT = animatedTiles.FirstOrDefault(at => at.TileIndices.Contains(i));
                        if (animT != null) {
                            var pals = mapData.Where(x => x.TileMapY == animT.TileIndices[0]).Select(x => x.PaletteIndex).Distinct().ToArray();
                            if (pals.Length > 1) {
                                Debug.LogWarning($"Tile {i} has several possible palettes: {String.Join(", ", pals)}");
                            } else if (pals.Length == 0) {
                                continue; // Tile isn't referenced. Don't have to fill this in
                            } else {
                                p = pals[0];
                            }
                        } else continue;
                    } else {
                        continue;
                    }
                } else {
                    p = tilesetPalettes[i];
                }

                // Fill in tile pixels
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
                                c = GetColor(true, tilePal, b);

                                tileSetTex.SetPixel(tileX + x, tileY + y + (tilePal * tilesY * CellSize), c);
                            }
                        }
                        else
                        {
                            var b = info.Tileset[index];
                            var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);
                            for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                                c = GetColor(false, tilePal, v, pal4bpp: p);

                                tileSetTex.SetPixel(tileX + x, tileY + y + (tilePal * tilesY * CellSize), c);
                            }
                        }
                    }
                }
                for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                    // Create a tile
                    tiles[tilePal][i] = tileSetTex.CreateTile(new Rect(tileX, tileY + (tilePal * tilesY * CellSize), CellSize, CellSize));
                }
            }

            for (int tilePal = 0; tilePal < info.TilePalettes.Length; tilePal++) {
                output[tilePal] = new Unity_TileSet(tiles[tilePal])
                {
                    AnimatedTiles = animatedTiles
                };
            }

            tileSetTex.Apply();

            return output;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath(context), GBA_ROMBase.Address_ROM);

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