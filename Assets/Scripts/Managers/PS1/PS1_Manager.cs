using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                new GameAction("Export Vignette", false, true),
            };
        }

        /// <summary>
        /// Runs the specified game action
        /// </summary>
        /// <param name="actionIndex">The action index</param>
        /// <param name="inputDir">The input directory</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="settings">The game settings</param>
        public virtual void RunAction(int actionIndex, string inputDir, string outputDir, GameSettings settings)
        {
            switch (actionIndex)
            {
                case 0:
                    ExportVignetteTextures(settings, outputDir);
                    break;
            }
        }

        /// <summary>
        /// Exports all vignette textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        public void ExportVignetteTextures(GameSettings settings, string outputDir)
        {
            // TODO: Get file paths from methods
            // TODO: Export all vignette files, not just FND

            // Create the context
            using (var context = new Context(settings)) {

                // Get the base output directory
                var baseOutputDir = Path.Combine(outputDir, "FND");

                // Create it
                Directory.CreateDirectory(baseOutputDir);

                // Extract every file
                foreach (var filePath in Directory.GetFiles(context.BasePath + "RAY/IMA/FND/", "*.XXX", SearchOption.TopDirectoryOnly)) {
                    // Get the relative path
                    var relativePath = filePath.Substring(context.BasePath.Length);

                    // Add the file to the context
                    context.AddFile(new LinearSerializedFile(context) {
                        filePath = relativePath
                    });

                    // Read the file
                    var vig = FileFactory.Read<PS1_R1_VignetteFile>(relativePath, context);

                    // Create the texture
                    var tex = new Texture2D(vig.Width, vig.Height);

                    // Write each block
                    for (int blockIndex = 0; blockIndex < vig.ImageBlocks.Length; blockIndex++) {
                        // Get the block data
                        var blockData = vig.ImageBlocks[blockIndex];

                        // Write the block
                        for (int y = 0; y < vig.Height; y++) {
                            for (int x = 0; x < 64; x++) {
                                // Get the color
                                var c = blockData[x + (y * 64)].GetColor();

                                // Set the pixel
                                tex.SetPixel((x + (blockIndex * 64)), tex.height - y - 1, c);
                            }
                        }
                    }

                    // Apply the pixels
                    tex.Apply();

                    // Write the texture
                    File.WriteAllBytes(Path.Combine(baseOutputDir, $"{Path.GetFileNameWithoutExtension(filePath)}.png"), tex.EncodeToPNG());
                }
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public abstract IList<ARGBColor> GetTileSet(Context context);

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) { }

        public virtual async Task LoadExtraFile(Context context, string path) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = FileInfo;
            PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, fileInfo[path].BaseAddress) {
                filePath = path,
                Length = fileInfo[path].Size
            };
            context.AddFile(file);
        }

        /// <summary>
        /// Loads the specified level for the editor from the specified blocks
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="allfixFile">The allfix file</param>
        /// <param name="worldFile">The world file</param>
        /// <param name="map">The map data</param>
        /// <param name="events">The event data</param>
        /// <param name="levelTextureBlock">The level texture data</param>
        /// <returns>The editor manager</returns>
        public async Task<BaseEditorManager> LoadAsync(Context context,
            
            // TODO: Replace these two with blocks like below - we can never reference the xxx files themselves since JP and demos pack them differently
            PS1_R1_AllfixFile allfixFile, PS1_R1_WorldFile worldFile, 
            
            PS1_R1_MapBlock map, PS1_R1_EventBlock events, byte[] levelTextureBlock)
        {
            Common_Tileset tileSet = new Common_Tileset(GetTileSet(context), TileSetWidth, CellSize);

            var eventDesigns = new List<KeyValuePair<Pointer, Common_Design>>();
            var commonEvents = new List<Common_Event>();

            // TODO: Clean up
            if (this is PS1_BaseXXX_Manager xxx && worldFile != null)
            {
                var allfixFileName = xxx.GetAllfixFilePath(context.Settings);
                var worldFileName = xxx.GetWorldFilePath(context.Settings);

                PS1_VRAM vram = FillVRAM(allfixFile, worldFile, levelTextureBlock);

                var index = 0;

                foreach (PS1_R1_Event e in events.Events)
                {
                    Controller.status = $"Loading DES {index}/{events.Events.Length}";

                    await Controller.WaitIfNecessary();

                    // Attempt to find existing DES
                    var desIndex = eventDesigns.FindIndex(x => x.Key == e.ImageDescriptorsPointer);

                    // Add if not found
                    if (desIndex == -1)
                    {
                        Common_Design finalDesign = new Common_Design
                        {
                            Sprites = new List<Sprite>(),
                            Animations = new List<Common_Animation>()
                        };

                        foreach (PS1_R1_ImageDescriptor i in e.ImageDescriptors)
                        {
                            Texture2D tex = null;

                            if (i.Offset.file.filePath == worldFileName)
                            {
                                tex = GetSpriteTexture("world", i, worldFile.EventPalette1, vram);
                            }
                            else if (i.Offset.file.filePath == allfixFileName)
                            {
                                tex = GetSpriteTexture("allfix", i, allfixFile.Palette2, vram);
                            }
                            else if (i.Offset.file.filePath == xxx.GetLevelFilePath(context.Settings))
                            {
                                tex = GetSpriteTexture("level", i, worldFile.EventPalette1, vram);
                            }

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
                        eventDesigns.Add(new KeyValuePair<Pointer, Common_Design>(e.ImageDescriptorsPointer, finalDesign));

                        // Set the index
                        desIndex = eventDesigns.Count - 1;
                    }

                    // Instantiate event prefab using LevelEventController
                    var ee = Controller.obj.levelEventController.AddEvent(e.Type, e.Etat, e.SubEtat, e.XPosition, e.YPosition, desIndex, index, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.Hitpoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, e.Commands, events.EventLinkingTable[index]);

                    // Add the event
                    commonEvents.Add(ee);

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
                Events = new List<Common_Event>(),

                // Create the tile array
                TileSet = new Common_Tileset[4]
            };
            c.TileSet[0] = tileSet;

            // Add the events
            c.Events = commonEvents;

            await Controller.WaitIfNecessary();

            // Set the tiles
            c.Tiles = new Common_Tile[map.Width * map.Height];

            int tileIndex = 0;
            for (int y = 0; y < (map.Height); y++)
            {
                for (int x = 0; x < (map.Width); x++)
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
            return new PS1EditorManager(c, context, this, eventDesigns.Select(x => x.Value).ToArray());
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public abstract Task<BaseEditorManager> LoadAsync(Context context);

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

        public virtual async Task LoadFilesAsync(Context context) {
            // PS1 loads files in order. We can't really load anything here
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="s">The image descriptor</param>
        /// <param name="palette">The palette to use</param>
        /// <param name="processedImageData">The processed image data to use</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(string name, PS1_R1_ImageDescriptor s, IList<ARGBColor> palette, PS1_VRAM vram) {
            // Get the image properties
            var width = s.OuterWidth;
            var height = s.OuterHeight;
            var texturePageInfo = s.TexturePageInfo;

            // see http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
            int pageX = BitHelpers.ExtractBits(texturePageInfo, 4, 0);
            int pageY = BitHelpers.ExtractBits(texturePageInfo, 1, 4);
            int abr   = BitHelpers.ExtractBits(texturePageInfo, 2, 5);
            int tp    = BitHelpers.ExtractBits(texturePageInfo, 2, 7); // 0: 4-bit, 1: 8-bit, 2: 15-bit direct
            //UnityEngine.Debug.Log(string.Format("{0:X4}", texturePageInfo));
            if (pageX < 5) return null;
            // Create the texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            // Default to fully transparent
            for (int y = 0; y < tex.height; y++) {
                for (int x = 0; x < tex.width; x++) {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            //try {
            // Set every pixel
            if (tp == 1) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var pixel = vram.GetPixel(pageX, pageY, s.ImageOffsetInPageX + x, s.ImageOffsetInPageY + y);

                        // Get the color from the palette
                        var color = palette[pixel];

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, color.GetColor());
                    }
                }
            } else if (tp == 0) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        int actualX = (s.ImageOffsetInPageX + x) / 2;
                        var pixel = vram.GetPixel(pageX, pageY, actualX, s.ImageOffsetInPageY + y);
                        if (x % 2 == 0) {
                            pixel = (byte)BitHelpers.ExtractBits(pixel, 4, 0);
                        } else {
                            pixel = (byte)BitHelpers.ExtractBits(pixel, 4, 4);
                        }

                        // Get the color from the palette
                        var color = palette[pixel];

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, color.GetColor());
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

        public PS1_VRAM FillVRAM(PS1_R1_AllfixFile allFix, PS1_R1_WorldFile world, byte[] levelTextureBlock) {
            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.skippedPagesX = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * (256 * 2 - 8)];
            Array.Copy(allFix.TextureBlock, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[allFix.TextureBlock.Length - cageSprites.Length];
            Array.Copy(allFix.TextureBlock, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            byte[] unknown = new byte[128 * 8];
            vram.AddData(unknown, 128);
            vram.AddData(cageSprites, 128);
            vram.AddData(allFixSprites, 256);

            vram.AddData(world.TextureBlock, 256);
            vram.AddData(levelTextureBlock, 256);

            /*Texture2D vramTex = new Texture2D(7 * 128, 2 * 256);
            for (int x = 0; x < 7 * 128; x++) {
                for (int y = 0; y < 2 * 256; y++) {
                    byte val = vram.GetPixel(5, y / 256, x, y % 256);
                    vramTex.SetPixel(x, y, new Color(val / 255f, val / 255f, val / 255f));
                }
            }
            vramTex.Apply();*/

            return vram;
        }

        public void ExportTexturePage(string name, IList<ARGBColor> palette, byte[] processedImageData) {
            // Get the image properties
            var width = 256;
            var height = processedImageData.Length / 256 + (processedImageData.Length % 256 == 0 ? 0 : 1);

            // Create the texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            // Default to fully transparent
            for (int y = 0; y < tex.height; y++) {
                for (int x = 0; x < tex.width; x++) {
                    var pixelOffset = y * width + x;
                    if (pixelOffset >= processedImageData.Length) continue;
                    var palIndex = processedImageData[pixelOffset];
                    var color = palette[palIndex];
                    tex.SetPixel(x, height - 1 - y, color.GetColor());
                }
            }
            tex.Apply();
        }

        #endregion
    }
}