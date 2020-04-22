using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PS1
    /// </summary>
    public abstract class PS1_Manager : IGameManager
    {
        #region Values and paths

        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public abstract int TileSetWidth { get; }

        /// <summary>
        /// The file info to use
        /// </summary>
        protected abstract Dictionary<string, PS1FileInfo> FileInfo { get; }

        protected virtual PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.DevPointerXOR;

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public virtual string GetWorldName(World world)
        {
            switch (world)
            {
                case World.Jungle:
                    return "JUN";
                case World.Music:
                    return "MUS";
                case World.Mountain:
                    return "MON";
                case World.Image:
                    return "IMG";
                case World.Cave:
                    return "CAV";
                case World.Cake:
                    return "CAK";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public abstract KeyValuePair<World, int[]>[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public virtual string[] GetEduVolumes(GameSettings settings) => new string[0];

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
                new GameAction("Export Vignette", false, true, (input, output) => ExportVignetteTextures(settings, output)),
            };
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public abstract Common_Tileset GetTileSet(Context context);

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public abstract void FillVRAM(Context context);

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <param name="vram">The filled v-ram</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public virtual Texture2D GetSpriteTexture(Context context, PS1_R1_Event e, Common_ImageDescriptor s)
        {
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

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) { }

        public virtual async Task LoadExtraFile(Context context, string path) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = FileInfo;
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
        /// <returns>The editor manager</returns>
        public async Task<BaseEditorManager> LoadAsync(Context context, PS1_R1_MapBlock map, PS1_R1_Event[] events, ushort[] eventLinkingTable, bool loadTextures)
        {
            Common_Tileset tileSet = GetTileSet(context);

            var eventDesigns = new Dictionary<Pointer, Common_Design>();
            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();
            var commonEvents = new List<Common_EventData>();

            // TODO: Temp fix so all versions work
            if (events != null)
            {
                if (loadTextures)
                    // Get the v-ram
                    FillVRAM(context);

                var index = 0;

                // Add every event
                foreach (PS1_R1_Event e in events)
                {
                    Controller.status = $"Loading DES {index}/{events.Length}";

                    await Controller.WaitIfNecessary();

                    // Add if not found
                    if (e.ImageDescriptorsPointer != null && !eventDesigns.ContainsKey(e.ImageDescriptorsPointer))
                    {
                        Common_Design finalDesign = new Common_Design
                        {
                            Sprites = new List<Sprite>(),
                            Animations = new List<Common_Animation>()
                        };

                        // Get every sprite
                        foreach (Common_ImageDescriptor i in e.ImageDescriptors)
                        {
                            Texture2D tex = loadTextures ? GetSpriteTexture(context, e, i) : null;

                            // Add it to the array
                            finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                        }

                        // Animations
                        foreach (var a in e.AnimDescriptors)
                        {
                            // Create the animation
                            var animation = new Common_Animation
                            {
                                Frames = new Common_AnimationPart[a.FrameCount, a.LayersPerFrame],
                                DefaultFrameXPosition = a.Frames.FirstOrDefault()?.XPosition ?? -1,
                                DefaultFrameYPosition = a.Frames.FirstOrDefault()?.YPosition ?? -1,
                                DefaultFrameWidth = a.Frames.FirstOrDefault()?.Width ?? -1,
                                DefaultFrameHeight = a.Frames.FirstOrDefault()?.Height ?? -1,
                            };

                            // The layer index
                            var layer = 0;

                            // Create each frame
                            for (int i = 0; i < a.FrameCount; i++)
                            {
                                // Create each layer
                                for (var layerIndex = 0; layerIndex < a.LayersPerFrame; layerIndex++)
                                {
                                    var animationLayer = a.Layers[layer];
                                    layer++;

                                    // Create the animation part
                                    var part = new Common_AnimationPart
                                    {
                                        SpriteIndex = animationLayer.ImageIndex,
                                        X = animationLayer.XPosition,
                                        Y = animationLayer.YPosition,
                                        Flipped = animationLayer.IsFlipped
                                    };

                                    // Add the texture
                                    animation.Frames[i, layerIndex] = part;
                                }
                            }
                            // Add the animation to list
                            finalDesign.Animations.Add(animation);
                        }

                        // Add to the designs
                        eventDesigns.Add(e.ImageDescriptorsPointer, finalDesign);
                    }

                    // Add if not found
                    if (e.ETAPointer != null && !eventETA.ContainsKey(e.ETAPointer))
                        // Add to the ETA
                        eventETA.Add(e.ETAPointer, e.EventStates);

                    // Add the event
                    commonEvents.Add(new Common_EventData
                    {
                        Type = e.Type,
                        Etat = e.Etat,
                        SubEtat = e.SubEtat,
                        XPosition = e.XPosition,
                        YPosition = e.YPosition,
                        DESKey = e.ImageDescriptorsPointer?.ToString() ?? String.Empty,
                        ETAKey = e.ETAPointer?.ToString() ?? String.Empty,
                        OffsetBX = e.OffsetBX,
                        OffsetBY = e.OffsetBY,
                        OffsetHY = e.OffsetHY,
                        FollowSprite = e.FollowSprite,
                        HitPoints = e.Hitpoints,
                        Layer = e.Layer,
                        HitSprite = e.HitSprite,
                        FollowEnabled = e.GetFollowEnabled(context.Settings),
                        LabelOffsets = e.LabelOffsets,
                        CommandCollection = e.Commands,
                        LinkIndex = eventLinkingTable[index]
                    });

                    index++;
                }
            }

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev
            {
                // Set the dimensions
                Width = map.Width,
                Height = map.Height,

                // Create the events list
                EventData = new List<Common_EventData>(),

                // Create the tile array
                TileSet = new Common_Tileset[4]
            };
            c.TileSet[0] = tileSet;

            // Add the events
            c.EventData = commonEvents;

            await Controller.WaitIfNecessary();

            // Set the tiles
            c.Tiles = new Common_Tile[map.Width * map.Height];

            int tileIndex = 0;
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var graphicX = map.Tiles[tileIndex].TileMapX;
                    var graphicY = map.Tiles[tileIndex].TileMapY;

                    Common_Tile newTile = new Common_Tile
                    {
                        PaletteIndex = 1,
                        XPosition = x,
                        YPosition = y,
                        CollisionType = map.Tiles[tileIndex].CollisionType,
                        TileSetGraphicIndex = (TileSetWidth * graphicY) + graphicX
                    };

                    c.Tiles[tileIndex] = newTile;

                    tileIndex++;
                }
            }

            // Return an editor manager
            return new PS1EditorManager(c, context, eventDesigns, eventETA);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public abstract Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures);

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(Context context, Common_Lev commonLevelData)
        {
            if (!(this is PS1_BaseXXX_Manager xxx))
                return;

            // TODO: This currently only works for NTSC

            // Get the level file path
            var lvlPath = xxx.GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<PS1_R1_LevFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++)
            {
                for (int x = 0; x < lvlData.MapData.Width; x++)
                {
                    // Get the tiles
                    var tile = lvlData.MapData.Tiles[y * lvlData.MapData.Width + x];
                    var commonTile = commonLevelData.Tiles[y * lvlData.MapData.Width + x];

                    // Update the tile
                    tile.CollisionType = commonTile.CollisionType;
                    tile.TileMapY = (int)Math.Floor(commonTile.TileSetGraphicIndex / (double)TileSetWidth);
                    tile.TileMapX = commonTile.TileSetGraphicIndex - (CellSize * tile.TileMapY);
                }
            }

            // Set events
            // TODO: Implement

            // Save the file
            FileFactory.Write<PS1_R1_LevFile>(lvlPath, context);
        }

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public virtual async Task LoadFilesAsync(Context context) {
            // PS1 loads files in order. We can't really load anything here
            await Task.CompletedTask;
        }

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public async Task ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Keep track of the hash for every DES
            var hashList = new List<string>();

            // Enumerate every world
            foreach (var world in GetLevels(baseGameSettings))
            {
                baseGameSettings.World = world.Key;

                // Enumerate every level
                foreach (var lvl in world.Value)
                {
                    baseGameSettings.Level = lvl;

                    var dir = Path.Combine(outputDir, world.Key.ToString());

                    Directory.CreateDirectory(dir);

                    // Create the context
                    var context = new Context(baseGameSettings);

                    // Load the editor manager
                    var editorManager = await LoadAsync(context, true);

                    var desIndex = 0;
                    
                    // Enumerate every design
                    foreach (var des in editorManager.DES.Values)
                    {
                        // Get the raw data
                        var rawData = des.Sprites.Where(x => x != null).SelectMany(x => x.texture.GetRawTextureData()).ToArray();

                        // Check the hash
                        using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                        {
                            // Get the hash
                            var hash = Convert.ToBase64String(sha1.ComputeHash(rawData));

                            // Check if it's been used before
                            if (hashList.Contains(hash))
                                continue;

                            hashList.Add(hash);
                        }

                        var spriteIndex = 0;

                        // Enumerate every sprite
                        foreach (var sprite in des.Sprites.Where(x => x != null).Select(x => x.texture))
                        {
                            // Export it
                            File.WriteAllBytes(Path.Combine(dir, $"{lvl} - {desIndex} - {spriteIndex}.png"), sprite.EncodeToPNG());

                            spriteIndex++;
                        }

                        desIndex++;
                    }
                }
            }
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
                        var multiData = FileFactory.Read<PS1_R1_MultiVignetteFile>(fileInfo.FilePath, context);

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
                        PS1_R1_VignetteBlockGroup imageBlock;

                        // Get the block
                        if (fileInfo.FileType == VignetteFileType.BlockedXXX)
                            imageBlock = FileFactory.Read<PS1_R1_BackgroundVignetteFile>(fileInfo.FilePath, context).ImageBlock;
                        else
                            imageBlock = FileFactory.Read<PS1_R1_VignetteBlockGroup>(fileInfo.FilePath, context, onPreSerialize: (s, x) => x.BlockGroupSize = (int)(s.CurrentLength / 2));

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

        #endregion
    }
}