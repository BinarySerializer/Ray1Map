using BinarySerializer;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var loader = Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Process each BIN file
                loader.ProcessBINFiles(entry, blockIndex, (cmd, i) =>
                {
                    var ext = IDXLoadCommand.FileExtensions[cmd.FILE_Type];

                    if (unpack)
                    {
                        var type = cmd.FILE_Type;
                        var archiveDepth = IDXLoadCommand.ArchiveDepths[type];

                        if (archiveDepth > 0)
                        {
                            // Be lazy and hard-code instead of making some recursive loop
                            if (archiveDepth == 1)
                            {
                                var archive = loader.Load_BINFile<RawData_ArchiveFile>(cmd, blockIndex, i);

                                for (int j = 0; j < archive.Files.Length; j++)
                                {
                                    var file = archive.Files[j];

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"{j}{ext}"), file.Data);
                                }
                            }
                            else if (archiveDepth == 2)
                            {
                                var archives = loader.Load_BINFile<ArchiveFile<RawData_ArchiveFile>>(cmd, blockIndex, i);

                                for (int a = 0; a < archives.Files.Length; a++)
                                {
                                    var archive = archives.Files[a];

                                    for (int j = 0; j < archive.Files.Length; j++)
                                    {
                                        var file = archive.Files[j];

                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"{a}_{j}{ext}"), file.Data);
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

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({ext.Substring(1)})", $"DATA.{ext}"), data);
                });
            }
        }

        public async UniTask Extract_TIMAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            await LoadFilesAsync(context);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Process each BIN file
                loader.ProcessBINFiles(entry, blockIndex, (cmd, i) =>
                {
                    // Check the file type
                    if (cmd.FILE_Type == IDXLoadCommand.FileType.Archive_TIM)
                    {
                        // Read the data
                        TIM_ArchiveFile timFiles = loader.Load_BINFile<TIM_ArchiveFile>(cmd, blockIndex, i);

                        var index = 0;

                        foreach (var tim in timFiles.Files)
                        {
                            try
                            {
                                var tex = GetTexture(tim);

                                if (tex != null)
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} - {index}.png"),
                                        tex.EncodeToPNG());
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Error exporting with ex: {ex}");
                            }

                            index++;
                        }
                    }
                    else if (cmd.FILE_Type == IDXLoadCommand.FileType.Archive_SpritePack)
                    {
                        // Read the data
                        LevelSpritePack_ArchiveFile spritePack = loader.Load_BINFile<LevelSpritePack_ArchiveFile>(cmd, blockIndex, i);

                        var exported = new HashSet<PlayerSprite_File>();

                        var index = 0;

                        var pal = spritePack.PlayerSprites.Files.FirstOrDefault(x => x?.TIM?.Clut != null)?.TIM.Clut.Palette.Select(x => x.GetColor()).ToArray();

                        foreach (var file in spritePack.PlayerSprites.Files)
                        {
                            if (file != null && !exported.Contains(file))
                            {
                                exported.Add(file);

                                try
                                {
                                    Texture2D tex;

                                    if (file.TIM != null)
                                        tex = GetTexture(file.TIM, palette: pal);
                                    else
                                        tex = GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height, PS1_TIM.TIM_ColorFormat.BPP_8);

                                    if (tex != null)
                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} - {index} (SpritePack).png"),
                                            tex.EncodeToPNG());
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogWarning($"Error exporting with ex: {ex}");
                                }
                            }

                            index++;
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

            var loader = Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                // Load the BIN
                loader.Load_BIN(entry, blockIndex);

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

            var loader = Loader.Create(context);

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                var vram = new PS1_VRAM();

                // Process each BIN file
                loader.ProcessBINFiles(entry, blockIndex, (cmd, i) =>
                {
                    try
                    {
                        // Check the file type
                        if (cmd.FILE_Type != IDXLoadCommand.FileType.Archive_BackgroundPack)
                            return;

                        // Read the data
                        var bg = loader.Load_BINFile<BackgroundPack_ArchiveFile>(cmd, blockIndex, i);

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

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
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
            var loader = Loader.Create(context);

            // Load fixed block first
            loader.Load_BIN(idxData.Entries[0], 0);

            // Load the BIN
            loader.Load_BIN(idxData.Entries[lev], lev);

            var obj = CreateGameObject(loader.LevelPack.Sectors[sector].LevelModel, loader);
            var levelDimensions = GetDimensions(loader.LevelPack.Sectors[sector].LevelModel);
            obj.transform.position = new Vector3(0, 0, -levelDimensions.y / 8f);

            var layers = new List<Unity_Layer>();
            var parent3d = Controller.obj.levelController.editor.layerTiles.transform;
            layers.Add(new Unity_Layer_GameObject(true)
            {
                Name = "Map",
                ShortName = "MAP",
                Graphics = obj,
                Collision = null,
                Dimensions = levelDimensions * 2,
                DisableGraphicsWhenCollisionIsActive = true
            });
            obj.transform.SetParent(parent3d);

            return new Unity_Level(
                layers: layers.ToArray(),
                cellSize: 1,
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
                    ViewAngle = Quaternion.Euler(0, 0, 0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * 8
                });
        }

        public GameObject CreateGameObject(PS1_TMD tmd, Loader loader)
        {
            var textureCache = new Dictionary<int, Texture2D>();

            GameObject gaoParent = new GameObject("Map");
            gaoParent.transform.position = Vector3.zero;

            foreach (var obj in tmd.Objects)
            {
                float scale = 8f;
                Vector3 toVertex(PS1_TMD_Vertex v) => new Vector3(v.X / scale, v.Z / scale, v.Y / scale);
                Vector2 toUV(PS1_TMD_UV uv) => new Vector2(uv.U, uv.V);

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
                            // lower left triangle
                            0, 2, 1,
                            // upper right triangle
                            2, 3, 1
                        };
                    }
                    else
                    {
                        triangles = new int[]
                        {
                            0, 1, 2,
                            0, 2, 1
                        };
                    }

                    unityMesh.SetTriangles(triangles, 0);

                    var colors = packet.RGB.Select(x => x.Color.GetColor()).ToArray();

                    if (colors.Length == 1)
                        colors = Enumerable.Repeat(colors[0], packet.Mode.IsQuad ? 4 : 3).ToArray();

                    unityMesh.SetColors(colors);
                    unityMesh.SetUVs(0, packet.UV.Select(toUV).ToArray());

                    unityMesh.RecalculateNormals();

                    GameObject gao = new GameObject($"Packet_{packet.Offset}");

                    MeshCollider mc = gao.AddComponent<MeshCollider>();
                    Mesh colMesh = new Mesh();
                    colMesh.SetVertices(vertices);
                    colMesh.SetTriangles(triangles.Where((x, i) => i % 6 >= 3).ToArray(), 0);
                    colMesh.RecalculateNormals();
                    mc.sharedMesh = colMesh;


                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                    gao.layer = LayerMask.NameToLayer("3D Collision");
                    gao.transform.SetParent(gaoParent.transform);
                    gao.transform.localScale = Vector3.one;
                    gao.transform.localPosition = Vector3.zero;
                    mf.mesh = unityMesh;
                    mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;

                    if (packet.Mode.TME)
                    {
                        var key = packet.CBA.ClutX | packet.CBA.ClutY << 6 | packet.TSB.TX << 16 | packet.TSB.TY << 24;

                        if (!textureCache.ContainsKey(key))
                        {
                            var tex = TextureHelpers.CreateTexture2D(256, 256);

                            FillTextureFromVRAM(
                                tex: tex,
                                vram: loader.VRAM,
                                width: 256,
                                height: 256,
                                colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4, // TODO: Can be 8-bit - use packet.TSB
                                texX: 0,
                                texY: 0,
                                clutX: packet.CBA.ClutX,
                                clutY: packet.CBA.ClutY,
                                texturePageX: packet.TSB.TX,
                                texturePageY: packet.TSB.TY,
                                texturePageOriginX: 0,
                                texturePageOriginY: 0,
                                texturePageOffsetX: 0,
                                texturePageOffsetY: 0);

                            tex.Apply();

                            textureCache.Add(key, tex);
                        }

                        var t = textureCache[key];

                        t.wrapMode = TextureWrapMode.Repeat;
                        mr.material.SetTexture("_MainTex", t);
                    }

                }
            }

            return gaoParent;
        }

        public Vector2 GetDimensions(PS1_TMD tmd)
        {
            var height = tmd.Objects.SelectMany(x => x.Vertices).Max(v => v.Y);
            var width = tmd.Objects.SelectMany(x => x.Vertices).Max(v => v.X);
            return new Vector2(width, height);
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
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY, texturePageOriginX + texturePageOffsetX + x, texturePageOriginY + texturePageOffsetY + y);
                    }
                    else if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_4)
                    {
                        int actualX = texturePageOriginX + (texturePageOffsetX + x) / 2;
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY, actualX, texturePageOriginY + texturePageOffsetY + y);

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