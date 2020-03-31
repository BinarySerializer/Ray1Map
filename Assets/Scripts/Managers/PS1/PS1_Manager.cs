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
            var eventETA = new List<KeyValuePair<Pointer, Common_EventState[][]>>();
            var commonEvents = new List<Common_Event>();

            // TODO: Clean up
            if (worldFile != null)
            {
                // Get the v-ram
                PS1_VRAM vram = FillVRAM(allfixFile, worldFile, levelTextureBlock);

                var index = 0;

                // Add every event
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

                        // Get every sprite
                        foreach (PS1_R1_ImageDescriptor i in e.ImageDescriptors)
                        {
                            Texture2D tex = GetSpriteTexture(i, vram);

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

                    // Attempt to find existing ETA
                    var etaIndex = eventETA.FindIndex(x => x.Key == e.ETAPointer);

                    // Add if not found
                    if (etaIndex == -1)
                    {
                        // Add to the ETA
                        eventETA.Add(new KeyValuePair<Pointer, Common_EventState[][]>(e.ETAPointer, e.EventStates));

                        // Set the index
                        etaIndex = eventETA.Count - 1;
                    }

                    // Instantiate event prefab using LevelEventController
                    var ee = Controller.obj.levelEventController.AddEvent(e.Type, e.Etat, e.SubEtat, e.XPosition, e.YPosition, desIndex, etaIndex, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.Hitpoints, e.Layer, e.HitSprite, e.FollowEnabled, e.LabelOffsets, e.Commands, events.EventLinkingTable[index]);

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
            return new PS1EditorManager(c, context, this, eventDesigns.Select(x => x.Value).ToArray(), eventETA.Select(x => x.Value).ToArray());
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

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public virtual async Task LoadFilesAsync(Context context) {
            // PS1 loads files in order. We can't really load anything here
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="s">The image descriptor</param>
        /// <param name="vram">The loaded v-ram</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(PS1_R1_ImageDescriptor s, PS1_VRAM vram) 
        {
            // Get the image properties
            var width = s.OuterWidth;
            var height = s.OuterHeight;
            var texturePageInfo = s.TexturePageInfo;
            var paletteInfo = s.PaletteInfo;

            // see http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
            int pageX = BitHelpers.ExtractBits(texturePageInfo, 4, 0);
            int pageY = BitHelpers.ExtractBits(texturePageInfo, 1, 4);
            int abr   = BitHelpers.ExtractBits(texturePageInfo, 2, 5);
            int tp    = BitHelpers.ExtractBits(texturePageInfo, 2, 7); // 0: 4-bit, 1: 8-bit, 2: 15-bit direct

            if (pageX < 5) 
                return null;

            // Get palette coordinates
            int paletteX = BitHelpers.ExtractBits(paletteInfo, 6, 0);
            int paletteY = BitHelpers.ExtractBits(paletteInfo, 10, 6);

            // Get the palette size
            var palette = tp == 0 ? new ARGB1555Color[16] : new ARGB1555Color[256];

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
                        var paletteIndex = vram.GetPixel8(pageX, pageY, s.ImageOffsetInPageX + x, s.ImageOffsetInPageY + y);

                        // Get the color from the palette
                        if (palette[paletteIndex] == null) {
                            palette[paletteIndex] = vram.GetColor1555(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        }
                        /*var palettedByte0 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        var palettedByte1 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex + 1, paletteY);
                        var color = palette[paletteIndex];*/

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteIndex].GetColor());
                    }
                }
            } else if (tp == 0) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
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
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="allFix">The allfix file</param>
        /// <param name="world">The world file</param>
        /// <param name="levelTextureBlock">The level texture block</param>
        /// <returns>The filled v-ram</returns>
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

            // Palettes start at y = 256 + 234 (= 490), so page 1 and y=234
            int paletteY = 234;
            /*vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
            vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            paletteY += 13 - world.TilePalettes.Length;

            foreach (var p in world.TilePalettes)
                vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

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

        #endregion
    }
}