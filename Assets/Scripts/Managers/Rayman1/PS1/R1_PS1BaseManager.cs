using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PS1
    /// </summary>
    public abstract class R1_PS1BaseManager : IGameManager
    {
        #region Values and paths

        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public abstract int TileSetWidth { get; }

        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected abstract Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings);

        protected virtual PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.DevPointerXOR;

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public virtual string GetWorldName(R1_World world)
        {
            switch (world)
            {
                case R1_World.Jungle:
                    return "JUN";
                case R1_World.Music:
                    return "MUS";
                case R1_World.Mountain:
                    return "MON";
                case R1_World.Image:
                    return "IMG";
                case R1_World.Cave:
                    return "CAV";
                case R1_World.Cake:
                    return "CAK";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public abstract GameInfo_Volume[] GetLevels(GameSettings settings);

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public virtual GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Export Sprites", false, true, (input, output) => ExportAllSpritesAsync(settings, output)),
                new GameAction("Export Animation Frames", false, true, (input, output) => ExportAllAnimationFramesAsync(settings, output)),
                new GameAction("Export Vignette", false, true, (input, output) => ExportVignetteTextures(settings, output)),
                new GameAction("Export Menu Sprites", false, true, (input, output) => ExportMenuSpritesAsync(settings, output, false)),
                new GameAction("Export Menu Animation Frames", false, true, (input, output) => ExportMenuSpritesAsync(settings, output, true)),
            };
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public abstract Unity_MapTileMap GetTileSet(Context context);

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected abstract void FillVRAM(Context context, VRAMMode mode);

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="imgBuffer">The image buffer, if available</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public virtual Texture2D GetSpriteTexture(Context context, byte[] imgBuffer, R1_ImageDescriptor s)
        {
            // Get the loaded v-ram
            PS1_VRAM vram = context.GetStoredObject<PS1_VRAM>("vram");

            // Get the image properties
            var width = s.OuterWidth;
            var height = s.OuterHeight;
            var texturePageInfo = s.TexturePageInfo;
            var paletteInfo = s.PaletteInfo;

            // see http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
            int pageX = BitHelpers.ExtractBits(texturePageInfo, 4, 0);
            int pageY = BitHelpers.ExtractBits(texturePageInfo, 1, 4);
            int abr = BitHelpers.ExtractBits(texturePageInfo, 2, 5);
            int tp = BitHelpers.ExtractBits(texturePageInfo, 2, 7); // 0: 4-bit, 1: 8-bit, 2: 15-bit direct

            if (pageX < 5)
                return null;

            // Get palette coordinates
            int paletteX = BitHelpers.ExtractBits(paletteInfo, 6, 0);
            int paletteY = BitHelpers.ExtractBits(paletteInfo, 10, 6);

            //Debug.Log(paletteX + " - " + paletteY + " - " + pageX + " - " + pageY + " - " + tp);

            // Get the palette size
            var palette = tp == 0 ? new ARGB1555Color[16] : new ARGB1555Color[256];

            // Create the texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            // Default to fully transparent
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            //try {
            // Set every pixel
            if (tp == 1)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var paletteIndex = vram.GetPixel8(pageX, pageY, s.ImageOffsetInPageX + x, s.ImageOffsetInPageY + y);

                        // Get the color from the palette
                        if (palette[paletteIndex] == null)
                        {
                            palette[paletteIndex] = vram.GetColor1555(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        }
                        /*var palettedByte0 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        var palettedByte1 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex + 1, paletteY);
                        var color = palette[paletteIndex];*/

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteIndex].GetColor());
                    }
                }
            }
            else if (tp == 0)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int actualX = (s.ImageOffsetInPageX + x) / 2;
                        var paletteIndex = vram.GetPixel8(pageX, pageY, actualX, s.ImageOffsetInPageY + y);
                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);


                        // Get the color from the palette
                        if (palette[paletteIndex] == null)
                            palette[paletteIndex] = vram.GetColor1555(0, 0, paletteX * 16 + paletteIndex, paletteY);

                        /*var palettedByte0 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        var palettedByte1 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex + 1, paletteY);*/

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteIndex].GetColor());
                    }
                }
            }
            /*} catch (Exception ex) {
                Debug.LogWarning($"Couldn't load sprite for DES: " + s.Offset + $" {ex.Message}");

                return null;
            }*/

            // Apply the changes
            tex.Apply();

            // Return the texture
            return tex;
        }

        public virtual async UniTask LoadExtraFile(Context context, string path) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = GetFileInfo(context.Settings);
            PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, fileInfo[path].BaseAddress, InvalidPointerMode) {
                filePath = path,
                Length = fileInfo[path].Size
            };
            context.AddFile(file);
        }

        /// <summary>
        /// Loads the specified level for the editor from the specified blocks
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="map">The map data</param>
        /// <param name="events">The events</param>
        /// <param name="eventLinkingTable">The event linking table</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <param name="bg">The background block data if available</param>
        /// <returns>The editor manager</returns>
        public async UniTask<BaseEditorManager> LoadAsync(Context context, MapData map, R1_EventData[] events, ushort[] eventLinkingTable, bool loadTextures, R1_PS1_BackgroundBlock bg = null)
        {
            Unity_MapTileMap tileSet = GetTileSet(context);

            var eventDesigns = new Dictionary<Pointer, Unity_ObjGraphics>();
            var eventETA = new Dictionary<Pointer, R1_EventState[][]>();
            var commonEvents = new List<Unity_Obj>();

            // Only load the v-ram if we're loading textures
            if (loadTextures)
                // Get the v-ram
                FillVRAM(context, VRAMMode.Level);

            // Load background sprites
            if (bg != null && loadTextures)
            {
                Unity_ObjGraphics finalDesign = new Unity_ObjGraphics
                {
                    Sprites = new List<Sprite>(),
                    Animations = new List<Unity_ObjAnimation>(),
                    FilePath = bg.Offset.file.filePath
                };

                // Get every sprite
                foreach (R1_ImageDescriptor i in bg.BackgroundLayerInfos)
                {
                    // Get the texture for the sprite, or null if not loading textures
                    Texture2D tex = GetSpriteTexture(context, null, i);

                    // Add it to the array
                    finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), Settings.PixelsPerUnit, 20));
                }

                // Add to the designs
                eventDesigns.Add(bg.Offset, finalDesign);
            }

            var index = 0;

            // Add every event
            foreach (R1_EventData e in events ?? (events = new R1_EventData[0]))
            {
                Controller.status = $"Loading DES {index}/{events.Length}";

                await Controller.WaitIfNecessary();

                // Add if not found
                if (e.ImageDescriptorsPointer != null && !eventDesigns.ContainsKey(e.ImageDescriptorsPointer))
                {
                    Unity_ObjGraphics finalDesign = new Unity_ObjGraphics
                    {
                        Sprites = new List<Sprite>(),
                        Animations = new List<Unity_ObjAnimation>(),
                        FilePath = e.ImageDescriptorsPointer.file.filePath
                    };

                    // Get every sprite
                    foreach (R1_ImageDescriptor i in e.ImageDescriptors)
                    {
                        // Get the texture for the sprite, or null if not loading textures
                        Texture2D tex = loadTextures ? GetSpriteTexture(context, e.ImageBuffer, i) : null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), Settings.PixelsPerUnit, 20));
                    }

                    // Add animations
                    finalDesign.Animations.AddRange(e.AnimDescriptors.Select(x => x.ToCommonAnimation()));

                    // Add to the designs
                    eventDesigns.Add(e.ImageDescriptorsPointer, finalDesign);
                }

                // Add if not found
                if (e.ETAPointer != null && !eventETA.ContainsKey(e.ETAPointer))
                    // Add to the ETA
                    eventETA.Add(e.ETAPointer, e.ETA.EventStates);

                // Add the event
                commonEvents.Add(new Unity_Obj(e)
                {
                    Type = e.Type,
                    DESKey = e.ImageDescriptorsPointer?.ToString() ?? String.Empty,
                    ETAKey = e.ETAPointer?.ToString() ?? String.Empty,
                    LabelOffsets = e.LabelOffsets,
                    CommandCollection = e.Commands,
                    LinkIndex = eventLinkingTable[index]
                });

                index++;
            }

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Unity_Level c = new Unity_Level
            {
                // Create the map
                Maps = new Unity_Map[]
                {
                    new Unity_Map()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile array
                        TileSet = new Unity_MapTileMap[1],
                        TileSetWidth = TileSetWidth
                    }
                },

                // Create the events list
                EventData = new List<Unity_Obj>(),

            };
            c.Maps[0].TileSet[0] = tileSet;

            // Add the events
            c.EventData = commonEvents;

            await Controller.WaitIfNecessary();

            // Set the tiles
            c.Maps[0].MapTiles = map.Tiles.Select(x => new Unity_Tile(x)).ToArray();

            // Load localization
            LoadLocalization(context, c);

            // Return an editor manager
            return new R1_PS1_EditorManager(c, context, eventDesigns, eventETA, events);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public abstract UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures);

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="editorManager">The editor manager</param>
        public virtual void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public virtual async UniTask LoadFilesAsync(Context context) {
            // PS1 loads files in order. We can't really load anything here
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Gets the base directory name for exporting a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="des">The design to export</param>
        /// <returns>The base directory name</returns>
        protected abstract string GetExportDirName(GameSettings settings, Unity_ObjGraphics des);

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public async UniTask ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // TODO: Extract BigRay from INI

            // Keep track of the hash for every DES
            var hashList = new List<string>();

            // Keep track of the DES index for each file
            var desIndexes = new Dictionary<string, int>();

            // Enumerate every world
            foreach (var world in GetLevels(baseGameSettings).First().Worlds)
            {
                baseGameSettings.World = world.Index;

                // Enumerate every level
                foreach (var lvl in world.Maps)
                {
                    baseGameSettings.Level = lvl;

                    // Create the context
                    using (var context = new Context(baseGameSettings))
                    {
                        // Load the editor manager
                        var editorManager = await LoadAsync(context, true);

                        // Set up animations
                        editorManager.InitializeRayAnim();

                        // Enumerate every design
                        foreach (var des in editorManager.DES.Values)
                        {
                            // Get the export dir name
                            var exportDirName = GetExportDirName(baseGameSettings, des);

                            if (!desIndexes.ContainsKey(exportDirName))
                                desIndexes.Add(exportDirName, 0);

                            var spriteIndex = -1;

                            // Enumerate every sprite
                            foreach (var sprite in des.Sprites.Where(x => x != null).Select(x => x.texture))
                            {
                                spriteIndex++;

                                // Get the png encoded data
                                var encodedData = sprite.EncodeToPNG();

                                // Check the hash
                                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                                {
                                    // Get the hash
                                    var hash = Convert.ToBase64String(sha1.ComputeHash(encodedData));

                                    // Check if it's been used before
                                    if (hashList.Contains(hash))
                                        continue;

                                    // Add to the hash list
                                    hashList.Add(hash);
                                }

                                // Export it
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"{exportDirName}{desIndexes[exportDirName]} - {spriteIndex}.png"), encodedData);
                            }

                            desIndexes[exportDirName]++;
                        }
                    }

                    // Unload textures
                    await Resources.UnloadUnusedAssets();
                }
            }
        }

        /// <summary>
        /// Exports every animation frame from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public async UniTask ExportAllAnimationFramesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Keep track of the hash for every DES
            var hashList = new List<string>();

            // Keep track of the DES index for each file
            var desIndexes = new Dictionary<string, int>();

            // Enumerate every world
            foreach (var world in GetLevels(baseGameSettings).First().Worlds)
            {
                baseGameSettings.World = world.Index;

                // Enumerate every level
                foreach (var lvl in world.Maps)
                {
                    baseGameSettings.Level = lvl;

                    // If Rayman 2, only include first map (since all 4 have same events)
                    if (baseGameSettings.EngineVersion == EngineVersion.R2_PS1 && lvl != 0)
                        continue;

                    // Create the context
                    using (var context = new Context(baseGameSettings))
                    {
                        // Load the editor manager
                        var editorManager = await LoadAsync(context, true);

                        // Set up animations
                        editorManager.InitializeRayAnim();

                        // Enumerate every design
                        foreach (var des in editorManager.DES)
                        {
                            // Check the hash
                            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                            {
                                // Get the hash
                                var hash = Convert.ToBase64String(sha1.ComputeHash(des.Value.Sprites.SelectMany(x => x?.texture?.GetRawTextureData() ?? new byte[0]).Append((byte)des.Value.Animations.Count).ToArray()));

                                // Check if it's been used before
                                if (hashList.Contains(hash))
                                    continue;

                                // Add to the hash list
                                hashList.Add(hash);
                            }

                            // Get the export dir name
                            var exportDirName = GetExportDirName(baseGameSettings, des.Value);

                            if (!desIndexes.ContainsKey(exportDirName))
                                desIndexes.Add(exportDirName, 0);

                            await ExportAnimationFramesAsync(baseGameSettings, editorManager, des, Path.Combine(outputDir, $"{exportDirName}{desIndexes[exportDirName]}"));

                            desIndexes[exportDirName]++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Exports the animation frames from a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="editorManager">The current editor manager</param>
        /// <param name="desValuePair">The common design and its key</param>
        /// <param name="outputDir">The output directory to export to</param>
        /// <returns>The task</returns>
        public async UniTask ExportAnimationFramesAsync(GameSettings settings, BaseEditorManager editorManager, KeyValuePair<string, Unity_ObjGraphics> desValuePair, string outputDir)
        {
            // Find all events where this DES is used
            var matchingEvents = editorManager.Level.EventData.Where(x => x.DESKey == desValuePair.Key);

            // Find matching ETA for this DES from the level events
            var matchingStates = matchingEvents.SelectMany(lvlEvent => editorManager.ETA[lvlEvent.ETAKey].SelectMany(x => x)).ToArray();

            // Correct Rayman's ETA for Rayman 2
            if (settings.EngineVersion == EngineVersion.R2_PS1 && !matchingStates.Any())
                matchingStates = editorManager.ETA.Last().Value.SelectMany(x => x).ToArray();

            // Get the animations
            var spriteAnim = desValuePair.Value.Animations;

            // Get the textures
            var textures = desValuePair.Value.Sprites?.Select(x => x?.texture).ToArray() ?? new Texture2D[0];

            // Enumerate the animations
            for (var j = 0; j < spriteAnim.Count; j++)
            {
                // Get the animation descriptor
                var anim = spriteAnim[j];

                // Get the speed
                var speed = String.Join("-", matchingStates.Where(x => x.AnimationIndex == j).Select(x => x.AnimationSpeed).Distinct());

                // Get the folder
                var animFolderPath = Path.Combine(outputDir, $"{j}-{speed}");

                int? frameWidth = null;
                int? frameHeight = null;

                var layersPerFrame = anim.Frames.First().Layers.Length;
                var frameCount = anim.Frames.Length;

                for (int dummyFrame = 0; dummyFrame < frameCount; dummyFrame++)
                {
                    for (int dummyLayer = 0; dummyLayer < layersPerFrame; dummyLayer++)
                    {
                        var l = anim.Frames[dummyFrame].Layers[dummyLayer];

                        if (l.ImageIndex < textures.Length)
                        {
                            var s = textures[l.ImageIndex];

                            if (s != null)
                            {
                                var w = s.width + l.XPosition;
                                var h = s.height + l.YPosition;

                                if (frameWidth == null || frameWidth < w)
                                    frameWidth = w;

                                if (frameHeight == null || frameHeight < h)
                                    frameHeight = h;
                            }
                        }
                    }
                }

                // Create each animation frame
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    Texture2D tex = new Texture2D(frameWidth ?? 1, frameHeight ?? 1, TextureFormat.RGBA32, false)
                    {
                        filterMode = FilterMode.Point,
                        wrapMode = TextureWrapMode.Clamp
                    };

                    // Default to fully transparent
                    tex.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), tex.width * tex.height).ToArray());

                    bool hasLayers = false;

                    // Write each layer
                    for (var layerIndex = 0; layerIndex < layersPerFrame; layerIndex++)
                    {
                        var animationLayer = anim.Frames[frameIndex].Layers[layerIndex];

                        if (animationLayer.ImageIndex >= textures.Length)
                            continue;

                        // Get the sprite
                        var sprite = textures[animationLayer.ImageIndex];

                        if (sprite == null)
                            continue;

                        // Set every pixel
                        for (int y = 0; y < sprite.height; y++)
                        {
                            for (int x = 0; x < sprite.width; x++)
                            {
                                var c = sprite.GetPixel(x, sprite.height - y - 1);

                                var xPosition = (animationLayer.IsFlippedHorizontally ? (sprite.width - 1 - x) : x) + animationLayer.XPosition;
                                var yPosition = y + animationLayer.YPosition;

                                if (xPosition >= tex.width)
                                    throw new Exception("Horizontal overflow!");

                                if (c.a != 0)
                                    tex.SetPixel(xPosition, tex.height - 1 - yPosition, c);
                            }
                        }

                        hasLayers = true;
                    }

                    tex.Apply();

                    if (!hasLayers)
                        continue;

                    // Save the file
                    Util.ByteArrayToFile(Path.Combine(animFolderPath, $"{frameIndex}.png"), tex.EncodeToPNG());
                }
            }

            // Unload textures
            await Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected abstract PS1VignetteFileInfo[] GetVignetteInfo();

        /// <summary>
        /// Exports all vignette textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        public virtual void ExportVignetteTextures(GameSettings settings, string outputDir)
        {
            // Create the context
            using (var context = new Context(settings))
            {
                // Enumerate every file
                foreach (var fileInfo in GetVignetteInfo().Where(x => File.Exists(settings.GameDirectory + x.FilePath)))
                {
                    // Add the file to the context
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = fileInfo.FilePath
                    });

                    // Get the textures
                    var textures = new List<Texture2D>();

                    if (fileInfo.FileType == VignetteFileType.Raw16)
                    {
                        // Read the raw data
                        var rawData = FileFactory.Read<ObjectArray<ARGB1555Color>>(fileInfo.FilePath, context, onPreSerialize: (s, x) => x.Length = s.CurrentLength / 2);

                        // Create the texture
                        textures.Add(new Texture2D(fileInfo.Width, (int)(rawData.Length / fileInfo.Width)));

                        // Set the pixels
                        for (int y = 0; y < textures.First().height; y++)
                        {
                            for (int x = 0; x < textures.First().width; x++)
                            {
                                var c = rawData.Value[y * textures.First().width + x];
                                c.Alpha = Byte.MaxValue;
                                textures.First().SetPixel(x, textures.First().height - y - 1, c.GetColor());
                            }
                        }
                    }
                    else if (fileInfo.FileType == VignetteFileType.MultiXXX)
                    {
                        // Read the data
                        var multiData = FileFactory.Read<R1_PS1_MultiVignetteFile>(fileInfo.FilePath, context);

                        // Get the textures
                        for (int i = 0; i < multiData.ImageBlocks.Length; i++)
                        {
                            // Create the texture
                            var tex = new Texture2D(fileInfo.Widths[i], (int)(multiData.ImageBlocks[i].Length / fileInfo.Widths[i]));

                            // Set the pixels
                            for (int y = 0; y < tex.height; y++)
                            {
                                for (int x = 0; x < tex.width; x++)
                                {
                                    var c = multiData.ImageBlocks[i].Value[y * tex.width + x];
                                    c.Alpha = Byte.MaxValue;
                                    tex.SetPixel(x, tex.height - y - 1, c.GetColor());
                                }
                            }

                            // Add the texture
                            textures.Add(tex);
                        }
                    }
                    else
                    {
                        R1_PS1_VignetteBlockGroup imageBlock;

                        // Get the block
                        if (fileInfo.FileType == VignetteFileType.BlockedXXX)
                            imageBlock = FileFactory.Read<R1_PS1_BackgroundVignetteFile>(fileInfo.FilePath, context).ImageBlock;
                        else
                            imageBlock = FileFactory.Read<R1_PS1_VignetteBlockGroup>(fileInfo.FilePath, context, onPreSerialize: (s, x) => x.BlockGroupSize = (int)(s.CurrentLength / 2));

                        // Create the texture
                        textures.Add(new Texture2D(imageBlock.Width, imageBlock.Height));

                        // Get the block width
                        var blockWdith = imageBlock.GetBlockWidth(context.Settings.EngineVersion);

                        // Write each block
                        for (int blockIndex = 0; blockIndex < imageBlock.ImageBlocks.Length; blockIndex++)
                        {
                            // Get the block data
                            var blockData = imageBlock.ImageBlocks[blockIndex];

                            // Write the block
                            for (int y = 0; y < imageBlock.Height; y++)
                            {
                                for (int x = 0; x < blockWdith; x++)
                                {
                                    // Get the color
                                    var c = blockData[x + (y * blockWdith)];

                                    c.Alpha = Byte.MaxValue;

                                    // Set the pixel
                                    textures.First().SetPixel((x + (blockIndex * blockWdith)), textures.First().height - y - 1, c.GetColor());
                                }
                            }
                        }
                    }

                    // Apply the pixels
                    textures.ForEach(x => x.Apply());

                    // Write the textures
                    if (textures.Count == 1)
                    {
                        // Get the output file path
                        var outputPath = Path.Combine(outputDir, FileSystem.ChangeFilePathExtension(fileInfo.FilePath, ".png"));

                        // Create the directory
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                        // Write the texture
                        File.WriteAllBytes(outputPath, textures.First().EncodeToPNG());
                    }
                    else
                    {
                        var index = 0;

                        foreach (var tex in textures)
                        {
                            // Get the output file path
                            var outputPath = Path.Combine(outputDir, FileSystem.ChangeFilePathExtension(fileInfo.FilePath, $" - {index}.png"));

                            // Create the directory
                            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                            // Write the texture
                            File.WriteAllBytes(outputPath, tex.EncodeToPNG());

                            index++;
                        }
                    }
                }
            }
        }

        protected virtual void LoadLocalization(Context context, Unity_Level level) { }

        public abstract UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames);

        protected async UniTask ExportMenuSpritesAsync(Context menuContext, Context bigRayContext, string outputPath, bool exportAnimFrames, R1_PS1_FontData[] fontData, R1_EventData[] fixEvents, R1_PS1_BigRayBlock bigRay)
        {
            // Fill the v-ram for each context
            FillVRAM(menuContext, VRAMMode.Menu);

            if (bigRayContext != null)
                FillVRAM(bigRayContext, VRAMMode.BigRay);

            // Export each font DES
            if (!exportAnimFrames)
            {
                for (int fontIndex = 0; fontIndex < fontData.Length; fontIndex++)
                {
                    // Export every sprite
                    for (int spriteIndex = 0; spriteIndex < fontData[fontIndex].ImageDescriptorsCount; spriteIndex++)
                    {
                        // Get the sprite texture
                        var tex = GetSpriteTexture(menuContext, fontData[fontIndex].ImageBuffer, fontData[fontIndex].ImageDescriptors[spriteIndex]);

                        // Make sure it's not null
                        if (tex == null)
                            continue;

                        // Export the font sprite
                        Util.ByteArrayToFile(Path.Combine(outputPath, "Font", $"{fontIndex} - {spriteIndex}.png"), tex.EncodeToPNG());
                    }
                }
            }

            // Export menu sprites from allfix
            var exportedImgDescr = new List<Pointer>();
            var index = 0;

            foreach (R1_EventData t in fixEvents)
            {
                if (exportedImgDescr.Contains(t.ImageDescriptorsPointer))
                    continue;

                exportedImgDescr.Add(t.ImageDescriptorsPointer);

                await ExportEventSpritesAsync(menuContext, t, Path.Combine(outputPath, "Menu"), index);

                index++;
            }

            // Export BigRay
            if (bigRay != null)
                await ExportEventSpritesAsync(bigRayContext, bigRay.BigRay, Path.Combine(outputPath, "BigRay"), 0);

            async UniTask ExportEventSpritesAsync(Context context, R1_EventData e, string eventOutputDir, int desIndex)
            {
                var sprites = e.ImageDescriptors.Select(x => GetSpriteTexture(context, e.ImageBuffer, x)).ToArray();

                if (!exportAnimFrames)
                {
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        if (sprites[i] == null)
                            continue;

                        Util.ByteArrayToFile(Path.Combine(eventOutputDir, $"{desIndex} - {i}.png"), sprites[i].EncodeToPNG());
                    }
                }
                else
                {
                    // Enumerate the animations
                    for (var j = 0; j < e.AnimDescriptors.Length; j++)
                    {
                        // Get the animation descriptor
                        var anim = e.AnimDescriptors[j];

                        // Get the speed
                        var speed = String.Join("-", e.ETA.EventStates.SelectMany(x => x).Where(x => x.AnimationIndex == j).Select(x => x.AnimationSpeed).Distinct());

                        // Get the folder
                        var animFolderPath = Path.Combine(eventOutputDir, desIndex.ToString(), $"{j}-{speed}");

                        int? frameWidth = null;
                        int? frameHeight = null;

                        for (int dummyFrame = 0; dummyFrame < anim.FrameCount; dummyFrame++)
                        {
                            for (int dummyLayer = 0; dummyLayer < anim.LayersPerFrame; dummyLayer++)
                            {
                                var l = anim.Layers[dummyFrame * anim.LayersPerFrame + dummyLayer];

                                if (l.ImageIndex < sprites.Length)
                                {
                                    var s = sprites[l.ImageIndex];

                                    if (s != null)
                                    {
                                        var w = s.width + l.XPosition;
                                        var h = s.height + l.YPosition;

                                        if (frameWidth == null || frameWidth < w)
                                            frameWidth = w;

                                        if (frameHeight == null || frameHeight < h)
                                            frameHeight = h;
                                    }
                                }
                            }
                        }

                        // Create each animation frame
                        for (int frameIndex = 0; frameIndex < anim.FrameCount; frameIndex++)
                        {
                            Texture2D tex = new Texture2D(frameWidth ?? 1, frameHeight ?? 1, TextureFormat.RGBA32, false)
                            {
                                filterMode = FilterMode.Point,
                                wrapMode = TextureWrapMode.Clamp
                            };

                            // Default to fully transparent
                            tex.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), tex.width * tex.height).ToArray());

                            bool hasLayers = false;

                            // Write each layer
                            for (var layerIndex = 0; layerIndex < anim.LayersPerFrame; layerIndex++)
                            {
                                var animationLayer = anim.Layers[frameIndex * anim.LayersPerFrame + layerIndex];

                                if (animationLayer.ImageIndex >= sprites.Length)
                                    continue;

                                // Get the sprite
                                var sprite = sprites[animationLayer.ImageIndex];

                                if (sprite == null)
                                    continue;

                                // Set every pixel
                                for (int y = 0; y < sprite.height; y++)
                                {
                                    for (int x = 0; x < sprite.width; x++)
                                    {
                                        var c = sprite.GetPixel(x, sprite.height - y - 1);

                                        var xPosition = (animationLayer.IsFlippedHorizontally ? (sprite.width - 1 - x) : x) + animationLayer.XPosition;
                                        var yPosition = y + animationLayer.YPosition;

                                        if (xPosition >= tex.width)
                                            throw new Exception("Horizontal overflow!");

                                        if (c.a != 0)
                                            tex.SetPixel(xPosition, tex.height - 1 - yPosition, c);
                                    }
                                }

                                hasLayers = true;
                            }

                            tex.Apply();

                            if (!hasLayers)
                                continue;

                            // Save the file
                            Util.ByteArrayToFile(Path.Combine(animFolderPath, $"{frameIndex}.png"), tex.EncodeToPNG());
                        }
                    }
                }

                // Unload textures
                await Resources.UnloadUnusedAssets();
            }
        }

        #endregion

        #region Value Types

        protected class PS1VignetteFileInfo
        {
            public PS1VignetteFileInfo(string filePath, int width = 0)
            {
                FilePath = filePath;
                Width = width;

                if (width != 0)
                    FileType = VignetteFileType.Raw16;
                else if (filePath.EndsWith(".XXX", StringComparison.InvariantCultureIgnoreCase))
                    FileType = VignetteFileType.BlockedXXX;
                else
                    FileType = VignetteFileType.Blocked;
            }

            public PS1VignetteFileInfo(string filePath, params int[] widths)
            {
                FilePath = filePath;
                Widths = widths;
                FileType = VignetteFileType.MultiXXX;
            }

            public VignetteFileType FileType { get; }

            public string FilePath { get; }

            public int Width { get; }

            public int[] Widths { get; }
        }

        /// <summary>
        /// The available vignette file types
        /// </summary>
        protected enum VignetteFileType
        {
            Raw16,
            Blocked,
            BlockedXXX,
            MultiXXX
        }

        protected enum VRAMMode
        {
            Level,
            Menu,
            BigRay
        }

        #endregion
    }
}