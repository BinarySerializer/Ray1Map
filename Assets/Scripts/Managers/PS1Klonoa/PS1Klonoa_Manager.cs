using BinarySerializer;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BinarySerializer.KlonoaDTP;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class PS1Klonoa_Manager : BaseGameManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Levels.Select((x, i) => new GameInfo_World(i, x.Item1, Enumerable.Range(0, x.Item2).ToArray())).ToArray());

        public virtual (string, int)[] Levels => new (string, int)[]
        {
            ("FIX", 0),
            ("MENU", 0),
            ("CODE", 0),

            ("Vision 1-1", 3),
            ("Vision 1-2", 5),
            ("Rongo Lango", 2),

            ("Vision 2-1", 4),
            ("Vision 2-2", 6),
            ("Pamela", 2),

            ("Vision 3-1", 5),
            ("Vision 3-2", 10),
            ("Gelg Bolm", 1),

            ("Vision 4-1", 3),
            ("Vision 4-2", 8),
            ("Baladium", 2),

            ("Vision 5-1", 7),
            ("Vision 5-2", 9),
            ("Joka", 1),

            ("Vision 6-1", 8),
            ("Vision 6-2", 8),
            ("", 2),
            ("", 2),
            ("", 3),
            ("", 3),
            ("", 9),
        };

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Extract BIN", false, true, (input, output) => Extract_BINAsync(settings, output, false)),
                new GameAction("Extract BIN (unpack archives)", false, true, (input, output) => Extract_BINAsync(settings, output, true)),
                new GameAction("Extract TIM", false, true, (input, output) => Extract_TIMAsync(settings, output)),
                new GameAction("Extract Backgrounds", false, true, (input, output) => Extract_BackgroundsAsync(settings, output)),
                new GameAction("Extract Sprites", false, true, (input, output) => Extract_SpriteFramesAsync(settings, output)),
                new GameAction("Extract ULZ blocks", false, true, (input, output) => Extract_ULZAsync(settings, output)),
            };
        }

        public async UniTask Extract_BINAsync(GameSettings settings, string outputPath, bool unpack)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var s = context.Deserializer;

            var loader = Loader.Create(context, idxData, GetLoaderConfig(settings));

            var archiveDepths = new Dictionary<IDXLoadCommand.FileType, int>()
            {
                [IDXLoadCommand.FileType.Unknown] = 0,

                [IDXLoadCommand.FileType.Archive_TIM_Generic] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SongsText] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SaveText] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SpriteSheets] = 1,

                [IDXLoadCommand.FileType.OA05] = 0,
                [IDXLoadCommand.FileType.SEQ] = 0,

                [IDXLoadCommand.FileType.Archive_BackgroundPack] = 2,

                [IDXLoadCommand.FileType.FixedSprites] = 1,
                [IDXLoadCommand.FileType.Archive_SpritePack] = 1,
                
                [IDXLoadCommand.FileType.Archive_LevelPack] = 1,
                
                [IDXLoadCommand.FileType.Archive_Unk0] = 1,
                [IDXLoadCommand.FileType.Archive_Unk4] = 2,

                [IDXLoadCommand.FileType.Code] = 0,
            };

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    var type = cmd.FILE_Type;

                    if (unpack)
                    {
                        var archiveDepth = archiveDepths[type];

                        if (archiveDepth > 0)
                        {
                            // Be lazy and hard-code instead of making some recursive loop
                            if (archiveDepth == 1)
                            {
                                var archive = loader.LoadBINFile<RawData_ArchiveFile>(i);

                                for (int j = 0; j < archive.Files.Length; j++)
                                {
                                    var file = archive.Files[j];

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"{j}.bin"), file.Data);
                                }
                            }
                            else if (archiveDepth == 2)
                            {
                                var archives = loader.LoadBINFile<ArchiveFile<RawData_ArchiveFile>>(i);

                                for (int a = 0; a < archives.Files.Length; a++)
                                {
                                    var archive = archives.Files[a];

                                    for (int j = 0; j < archive.Files.Length; j++)
                                    {
                                        var file = archive.Files[j];

                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"{a}_{j}.bin"), file.Data);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception($"Unsupported archive depth");
                            }

                            return;
                        }
                    }

                    // Read the raw data
                    var data = s.SerializeArray<byte>(null, cmd.FILE_Length);

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"Data.bin"), data);
                });
            }
        }

        public async UniTask Extract_TIMAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, GetLoaderConfig(settings));

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    var index = 0;

                    switch (cmd.FILE_Type)
                    {
                        case IDXLoadCommand.FileType.Archive_TIM_Generic:
                        case IDXLoadCommand.FileType.Archive_TIM_SongsText:
                        case IDXLoadCommand.FileType.Archive_TIM_SaveText:
                        case IDXLoadCommand.FileType.Archive_TIM_SpriteSheets:

                            // Read the data
                            TIM_ArchiveFile timFiles = loader.LoadBINFile<TIM_ArchiveFile>(i);

                            foreach (var tim in timFiles.Files)
                            {
                                export(() => GetTexture(tim));
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_BackgroundPack:

                            // Read the data
                            var bgPack = loader.LoadBINFile<BackgroundPack_ArchiveFile>(i);

                            foreach (var tim in bgPack.TIMFiles.Files)
                            {
                                export(() => GetTexture(tim));
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_SpritePack:

                            // Read the data
                            LevelSpritePack_ArchiveFile spritePack = loader.LoadBINFile<LevelSpritePack_ArchiveFile>(i);

                            var exported = new HashSet<PlayerSprite_File>();

                            var pal = spritePack.PlayerSprites.Files.FirstOrDefault(x => x?.TIM?.Clut != null)?.TIM.Clut.Palette.Select(x => x.GetColor()).ToArray();

                            foreach (var file in spritePack.PlayerSprites.Files)
                            {
                                if (file != null && !exported.Contains(file))
                                {
                                    exported.Add(file);

                                    export(() =>
                                    {
                                        if (file.TIM != null)
                                            return GetTexture(file.TIM, palette: pal);
                                        else
                                            return GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height, PS1_TIM.TIM_ColorFormat.BPP_8);
                                    });
                                }

                                index++;
                            }

                            break;
                    }

                    void export(Func<Texture2D> getTex)
                    {
                        try
                        {
                            var tex = getTex();

                            if (tex != null)
                                Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} - {index}.png"),
                                    tex.EncodeToPNG());
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Error exporting with ex: {ex}");
                        }
                    }
                });
            }
        }

        public async UniTask Extract_SpriteFramesAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, GetLoaderConfig(settings));

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                loader.SwitchBlocks(blockIndex);

                // Load the BIN
                loader.LoadAndProcessBINBlock();

                // Enumerate every set of frames
                for (int framesSet = 0; framesSet < loader.SpriteFrames.Length; framesSet++)
                {
                    var spriteFrames = loader.SpriteFrames[framesSet];

                    if (spriteFrames == null)
                        continue;

                    // Enumerate every frame
                    for (int frameIndex = 0; frameIndex < spriteFrames.Files.Length; frameIndex++)
                    {
                        try
                        {
                            var sprites = spriteFrames.Files[frameIndex].Textures;

                            foreach (var s in sprites)
                            {
                                if (s.FlipX)
                                    s.XPos = (short)(s.XPos - s.Width - 1);
                                if (s.FlipY)
                                    s.YPos = (short)(s.YPos - s.Height - 1);
                            }

                            var minX = sprites.Min(x => x.XPos);
                            var minY = sprites.Min(x => x.YPos);
                            var maxX = sprites.Max(x => x.XPos + x.Width);
                            var maxY = sprites.Max(x => x.YPos + x.Height);

                            var width = maxX - minX;
                            var height = maxY - minY;

                            var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                            foreach (var sprite in sprites)
                            {
                                var texPage = sprite.TexturePage;

                                FillTextureFromVRAM(
                                    tex: tex,
                                    vram: loader.VRAM,
                                    width: sprite.Width,
                                    height: sprite.Height,
                                    colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4,
                                    texX: sprite.XPos - minX,
                                    texY: sprite.YPos - minY,
                                    clutX: sprite.PalOffsetX,
                                    clutY: 500 + sprite.PalOffsetY,
                                    texturePageOriginX: 64 * (texPage % 16),
                                    texturePageOriginY: 128 * (texPage / 16), // TODO: Fix this
                                    texturePageOffsetX: sprite.TexturePageOffsetX,
                                    texturePageOffsetY: sprite.TexturePageOffsetY,
                                    flipX: sprite.FlipX,
                                    flipY: sprite.FlipY);
                            }

                            tex.Apply();

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{framesSet} - {frameIndex}.png"), tex.EncodeToPNG());
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Error exporting sprite frame: {ex}");
                        }
                    }
                }

                try
                {
                    PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), loader.VRAM);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting VRAM: {ex}");
                }
            }
        }

        public async UniTask Extract_BackgroundsAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, GetLoaderConfig(settings));

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var vram = new PS1_VRAM();

                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    try
                    {
                        // Check the file type
                        if (cmd.FILE_Type != IDXLoadCommand.FileType.Archive_BackgroundPack)
                            return;

                        // Read the data
                        var bg = loader.LoadBINFile<BackgroundPack_ArchiveFile>(i);

                        // TODO: Some maps use different textures! How do we find the index? For now export all variants
                        for (int tileSetIndex = 0; tileSetIndex < bg.TIMFiles.Files.Length; tileSetIndex++)
                        {
                            var tim = bg.TIMFiles.Files[tileSetIndex];
                            var cel = bg.CELFiles.Files[tileSetIndex];

                            // The game hard-codes this
                            if (tileSetIndex == 1)
                            {
                                tim.XPos = 0x1C0;
                                tim.YPos = 0x100;
                                tim.Width = 0x40;
                                tim.Height = 0x100;

                                tim.Clut.XPos = 0x120;
                                tim.Clut.YPos = 0x1F0;
                                tim.Clut.Width = 0x10;
                                tim.Clut.Height = 0x10;
                            }

                            loader.AddToVRAM(tim);

                            for (int j = 0; j < bg.BGDFiles.Files.Length; j++)
                            {
                                var map = bg.BGDFiles.Files[j];

                                var tex = TextureHelpers.CreateTexture2D(map.MapWidth * map.CellWidth, map.MapHeight * map.CellHeight, clear: true);

                                for (int mapY = 0; mapY < map.MapHeight; mapY++)
                                {
                                    for (int mapX = 0; mapX < map.MapWidth; mapX++)
                                    {
                                        var cellIndex = map.Map[mapY * map.MapWidth + mapX];

                                        if (cellIndex == 0xFF)
                                            continue;

                                        var cell = cel.Cells[cellIndex];

                                        FillTextureFromVRAM(
                                            tex: tex, 
                                            vram: vram, 
                                            width: map.CellWidth, 
                                            height: map.CellHeight, 
                                            colorFormat: tim.ColorFormat, 
                                            texX: mapX * map.CellWidth, 
                                            texY: mapY * map.CellHeight, 
                                            clutX: cell.ClutX * 16, 
                                            clutY: cell.ClutY, 
                                            texturePageOriginX: tim.XPos, 
                                            texturePageOriginY: tim.YPos, 
                                            texturePageOffsetX: cell.XOffset, 
                                            texturePageOffsetY: cell.YOffset);
                                    }
                                }

                                tex.Apply();

                                Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - {j}_{tileSetIndex}.png"), tex.EncodeToPNG());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting with ex: {ex}");
                    }
                });

                PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), vram);
            }
        }

        public async UniTask Extract_ULZAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);

            await LoadFilesAsync(context);

            var s = context.Deserializer;

            s.Goto(context.GetFile(Loader.FilePath_BIN).StartPointer);

            while (s.CurrentFileOffset < s.CurrentLength)
            {
                var v = s.Serialize<int>(default);

                if (v != 0x1A7A6C55)
                    continue;

                var offset = s.CurrentPointer - 4;

                s.DoAt(offset, () =>
                {
                    try
                    {
                        var bytes = s.DoEncoded(new ULZEncoder(), () => s.SerializeArray<byte>(default, s.CurrentLength));

                        Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{offset.AbsoluteOffset:X8}.bin"), bytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error decompressing at {offset} with ex: {ex}");
                    }
                });
            }
        }

        public LoaderConfiguration GetLoaderConfig(GameSettings settings)
        {
            // TODO: Support other versions
            return new LoaderConfiguration_DTP_US();
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get settings
            var settings = context.GetR1Settings();
            var lev = settings.World;
            var sector = settings.Level;

            Controller.DetailedState = "Loading IDX";
            await Controller.WaitIfNecessary();

            // Load the IDX
            var idxData = Load_IDX(context);

            Controller.DetailedState = "Loading BIN";
            await Controller.WaitIfNecessary();

            // Create the loader
            var loader = Loader.Create(context, idxData, GetLoaderConfig(settings));

            var logAction = new Func<string, Task>(async x =>
            {
                Controller.DetailedState = x;
                await Controller.WaitIfNecessary();
            });

            // Load the fixed BIN
            loader.SwitchBlocks(loader.Config.BLOCK_Fix);
            await loader.LoadAndProcessBINBlockAsync(logAction);

            // Load the level BIN
            loader.SwitchBlocks(lev);
            await loader.LoadAndProcessBINBlockAsync(logAction);

            Controller.DetailedState = "Loading code level data";
            await Controller.WaitIfNecessary();

            // Load code level data
            loader.ProcessCodeLevelData();

            // Load the layers
            var layers = await Load_LayersAsync(loader, sector);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            // TODO: Load objects

            return new Unity_Level(
                layers: layers,
                cellSize: 16,
                objManager: new Unity_ObjectManager(context),
                eventData: new List<Unity_Object>(),
                isometricData: new Unity_IsometricData
                {
                    CollisionWidth = 0,
                    CollisionHeight = 0,
                    TilesWidth = 0,
                    TilesHeight = 0,
                    Collision = null,
                    Scale = Vector3.one,
                    ViewAngle = Quaternion.Euler(90, 0, 0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * 8
                });
        }

        public async UniTask<Unity_Layer[]> Load_LayersAsync(Loader loader, int sector)
        {
            var layers = new List<Unity_Layer>();

            const float scale = 16f;

            Controller.DetailedState = "Loading backgrounds";
            await Controller.WaitIfNecessary();

            // TODO: Load backgrounds - easiest to convert to textures instead of using tilemaps, unless it's easier for animations?

            Controller.DetailedState = "Loading level geometry";
            await Controller.WaitIfNecessary();

            var obj = CreateGameObject(loader.LevelPack.Sectors[sector].LevelModel, loader, scale);
            var levelDimensions = GetDimensions(loader.LevelPack.Sectors[sector].LevelModel) / scale;
            obj.transform.position = new Vector3(0, 0, 0);

            var parent3d = Controller.obj.levelController.editor.layerTiles.transform;
            layers.Add(new Unity_Layer_GameObject(true)
            {
                Name = "Map",
                ShortName = "MAP",
                Graphics = obj,
                Collision = null,
                Dimensions = levelDimensions,
                DisableGraphicsWhenCollisionIsActive = true
            });
            obj.transform.SetParent(parent3d);

            Controller.DetailedState = "Loading collision";
            await Controller.WaitIfNecessary();

            // TODO: Load collision

            return layers.ToArray();
        }

        public GameObject CreateGameObject(PS1_TMD tmd, Loader loader, float scale)
        {
            var textureCache = new Dictionary<int, Texture2D>();

            GameObject gaoParent = new GameObject("Map");
            gaoParent.transform.position = Vector3.zero;

            foreach (var obj in tmd.Objects)
            {
                Vector3 toVertex(PS1_TMD_Vertex v) => new Vector3(v.X / scale, -v.Y / scale, v.Z / scale);
                Vector2 toUV(PS1_TMD_UV uv) => new Vector2(uv.U / 255f, uv.V / 255f);

                // TODO: Implement scale
                if (obj.Scale != 0)
                    Debug.LogWarning($"TMD object is scaled at {obj.Scale}");

                // TODO: Implement normals
                if (obj.NormalsCount != 0)
                    Debug.LogWarning($"TMD object has {obj.NormalsCount} normals");

                foreach (var packet in obj.Primitives)
                {
                    // TODO: Implement other types
                    if (packet.Mode.Code != PS1_TMD_PacketMode.PacketModeCODE.Polygon)
                    {
                        Debug.LogWarning($"Skipped packet with code {packet.Mode.Code}");
                        continue;
                    }

                    Mesh unityMesh = new Mesh();

                    var vertices = packet.Vertices.Select(x => toVertex(obj.Vertices[x])).ToArray();
                    int[] triangles;

                    // Set vertices
                    unityMesh.SetVertices(vertices);

                    if (packet.Mode.IsQuad)
                    {
                        triangles = new int[]
                        {
                            // Lower left triangle
                            0, 1, 2,
                            // Upper right triangle
                            3, 2, 1
                        };
                    }
                    else
                    {
                        triangles = new int[]
                        {
                            0, 1, 2,
                        };
                    }

                    unityMesh.SetTriangles(triangles, 0);

                    var colors = packet.RGB.Select(x => x.Color.GetColor()).ToArray();

                    if (colors.Length == 1)
                        colors = Enumerable.Repeat(colors[0], packet.Mode.IsQuad ? 4 : 3).ToArray();

                    unityMesh.SetColors(colors);

                    if (packet.UV != null) 
                        unityMesh.SetUVs(0, packet.UV.Select(toUV).ToArray());

                    unityMesh.RecalculateNormals();

                    GameObject gao = new GameObject($"Packet_{packet.Offset}");

                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                    gao.layer = LayerMask.NameToLayer("3D Collision");
                    gao.transform.SetParent(gaoParent.transform);
                    gao.transform.localScale = Vector3.one;
                    gao.transform.localPosition = Vector3.zero;
                    mf.mesh = unityMesh;
                    mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial;

                    // Add texture
                    if (packet.Mode.TME)
                    {
                        var key = packet.CBA.ClutX | packet.CBA.ClutY << 6 | packet.TSB.TX << 16 | packet.TSB.TY << 24;

                        if (!textureCache.ContainsKey(key))
                            textureCache.Add(key, GetTexture(packet, loader.VRAM));

                        var t = textureCache[key];

                        t.wrapMode = TextureWrapMode.Repeat;
                        mr.material.SetTexture("_MainTex", t);
                    }
                }
            }

            return gaoParent;
        }

        public Vector3 GetDimensions(PS1_TMD tmd)
        {
            var verts = tmd.Objects.SelectMany(x => x.Vertices).ToArray();
            var width = verts.Max(v => v.X) - verts.Min(v => v.X);
            var height = verts.Max(v => v.Y) - verts.Min(v => v.Y);
            var depth = verts.Max(v => v.Z) - verts.Min(v => v.Z);
            return new Vector3(width, height, depth);
        }

        public IDX Load_IDX(Context context)
        {
            return FileFactory.Read<IDX>(Loader.FilePath_IDX, context);
        }

        public void FillTextureFromVRAM(
            Texture2D tex,
            PS1_VRAM vram,
            int width, int height,
            PS1_TIM.TIM_ColorFormat colorFormat,
            int texX, int texY,
            int clutX, int clutY,
            int texturePageOriginX, int texturePageOriginY,
            int texturePageOffsetX, int texturePageOffsetY,
            int texturePageX = 0, int texturePageY = 0,
            bool flipX = false, bool flipY = false,
            bool useDummyPal = false)
        {
            var dummyPal = useDummyPal ? Util.CreateDummyPalette(colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8 ? 256 : 16) : null;

            texturePageOriginX *= 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte paletteIndex;

                    if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + texturePageOffsetX + x,
                            texturePageOriginY + texturePageOffsetY + y);
                    }
                    else if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_4)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + (texturePageOffsetX + x) / 2,
                            texturePageOriginY + texturePageOffsetY + y);

                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);
                    }
                    else
                    {
                        throw new Exception($"Non-supported color format");
                    }

                    // Get the color from the palette
                    var c = useDummyPal ? dummyPal[paletteIndex] : vram.GetColor1555(0, 0, clutX + paletteIndex, clutY);

                    if (c.Alpha == 0)
                        continue;

                    var texOffsetX = flipX ? width - x - 1 : x;
                    var texOffsetY = flipY ? height - y - 1 : y;

                    // Set the pixel
                    tex.SetPixel(texX + texOffsetX, tex.height - (texY + texOffsetY) - 1, c.GetColor());
                }
            }
        }

        public Texture2D GetTexture(PS1_TIM tim, bool flipTextureY = true, Color[] palette = null)
        {
            if (tim.XPos == 0 && tim.YPos == 0)
                return null;

            var pal = palette ?? tim.Clut?.Palette?.Select(x => x.GetColor()).ToArray();

            return GetTexture(tim.ImgData, pal, tim.Width, tim.Height, tim.ColorFormat, flipTextureY);
        }

        public Texture2D GetTexture(PS1_TMD_Packet packet, PS1_VRAM vram)
        {
            if (!packet.Mode.TME)
                throw new Exception($"Packet has no texture");

            PS1_TIM.TIM_ColorFormat colFormat = packet.TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => PS1_TIM.TIM_ColorFormat.BPP_4,
                PS1_TSB.TexturePageTP.CLUT_8Bit => PS1_TIM.TIM_ColorFormat.BPP_8,
                PS1_TSB.TexturePageTP.Direct_15Bit => PS1_TIM.TIM_ColorFormat.BPP_16,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {packet.TSB.TP}")
            };
            int width = packet.TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => 256,
                PS1_TSB.TexturePageTP.CLUT_8Bit => 128,
                PS1_TSB.TexturePageTP.Direct_15Bit => 64,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {packet.TSB.TP}")
            };

            var tex = TextureHelpers.CreateTexture2D(width, 256, clear: true);

            FillTextureFromVRAM(
                tex: tex,
                vram: vram,
                width: width,
                height: 256,
                colorFormat: colFormat,
                texX: 0,
                texY: 0,
                clutX: packet.CBA.ClutX * 16,
                clutY: packet.CBA.ClutY,
                texturePageX: packet.TSB.TX,
                texturePageY: packet.TSB.TY,
                texturePageOriginX: 0,
                texturePageOriginY: 0,
                texturePageOffsetX: 0,
                texturePageOffsetY: 0,
                flipY: true);

            tex.Apply();

            return tex;
        }

        public Texture2D GetTexture(byte[] imgData, Color[] pal, int width, int height, PS1_TIM.TIM_ColorFormat colorFormat, bool flipTextureY = true)
        {
            Util.TileEncoding encoding;

            int palLength;

            switch (colorFormat)
            {
                case PS1_TIM.TIM_ColorFormat.BPP_4:
                    width *= 2 * 2;
                    encoding = Util.TileEncoding.Linear_4bpp;
                    palLength = 16;
                    break;

                case PS1_TIM.TIM_ColorFormat.BPP_8:
                    width *= 2;
                    encoding = Util.TileEncoding.Linear_8bpp;
                    palLength = 256;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            pal ??= Util.CreateDummyPalette(palLength).Select(x => x.GetColor()).ToArray();

            var tex = TextureHelpers.CreateTexture2D(width, height);

            tex.FillRegion(
                imgData: imgData,
                imgDataOffset: 0,
                pal: pal,
                encoding: encoding,
                regionX: 0,
                regionY: 0,
                regionWidth: tex.width,
                regionHeight: tex.height,
                flipTextureY: flipTextureY);

            return tex;
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            // The game only loads portions of the BIN at a time
            await context.AddLinearSerializedFileAsync(Loader.FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(Loader.FilePath_IDX, 0x80010000);
        }
    }
}