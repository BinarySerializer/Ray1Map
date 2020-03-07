using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PC
    /// </summary>
    public abstract class PC_Manager : IGameManager
    {
        #region Values and paths

        // TODO: Move this?
        public static Dictionary<uint, Common_Animation[]> SpriteAnimationCache { get; } = new Dictionary<uint, Common_Animation[]>();

        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <param name="basePath">The game base path</param>
        /// <returns>The data path</returns>
        public string GetDataPath(string basePath) => Path.Combine(basePath, "PCMAP");

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public string GetWorldName(World world)
        {
            switch (world)
            {
                case World.Jungle:
                    return "JUNGLE";
                case World.Music:
                    return "MUSIC";
                case World.Mountain:
                    return "MOUNTAIN";
                case World.Image:
                    return "IMAGE";
                case World.Cave:
                    return "CAVE";
                case World.Cake:
                    return "CAKE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Gets the short name for the world
        /// </summary>
        /// <returns>The short world name</returns>
        public string GetShortWorldName(World world)
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
                    return "IMA";
                case World.Cave:
                    return "CAV";
                case World.Cake:
                    return "CAK";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public abstract string GetLevelFilePath(GameSettings settings);

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath(GameSettings settings) => Path.Combine(GetDataPath(settings.GameDirectory), $"ALLFIX.DAT");

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public abstract string GetWorldFilePath(GameSettings settings);

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public abstract bool Has3Palettes { get; }

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public abstract int[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public virtual string[] GetEduVolumes(GameSettings settings) => new string[0];

        #endregion

        #region Manager Methods

        /// <summary>
        /// Exports all sprite textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        public void ExportSpriteTextures(GameSettings settings, string outputDir)
        {
            // Read the allfix file
            var allfix = FileFactory.Read<PC_WorldFile>(GetAllfixFilePath(settings), settings);

            // Export the sprite textures
            ExportSpriteTextures(settings, allfix, Path.Combine(outputDir, "Allfix"), 0);

            // Enumerate every world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Set the world
                settings.World = world;

                // Get the world file path
                var worldPath = GetWorldFilePath(settings);

                if (!File.Exists(worldPath))
                    continue;

                // Read the world file
                var worldFile = FileFactory.Read<PC_WorldFile>(worldPath, settings);

                // Export the sprite textures
                ExportSpriteTextures(settings, worldFile, Path.Combine(outputDir, world.ToString()), allfix.DesItemCount);
            }
        }

        /// <summary>
        /// Exports all sprite textures from the world file to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="worldFile">The world file</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="desOffset">The amount of textures in the allfix to use as the DES offset</param>
        public void ExportSpriteTextures(GameSettings settings, PC_WorldFile worldFile, string outputDir, int desOffset)
        {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<PC_LevFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(settings))
            {
                // Set the level number
                settings.Level = i;

                // Load the level
                levels.Add(FileFactory.Read<PC_LevFile>(GetLevelFilePath(settings), settings));
            }

            // Enumerate each sprite group
            for (int i = 0; i < worldFile.DesItems.Length; i++)
            {
                // Get the sprite group
                var desItem = worldFile.DesItems[i];

                // Enumerate each image
                for (int j = 0; j < desItem.ImageDescriptors.Length; j++)
                {
                    Texture2D tex;

                    try
                    {
                        // TODO: This isn't really working for finding the correct palette

                        // Default to the first level
                        var lvl = levels.First();

                        // Find a matching animation descriptor
                        var animDesc = desItem.AnimationDescriptors.FindItemIndex(x => x.Layers.Any(y => y.ImageIndex == j));

                        bool foundCorrectPalette = false;

                        if (animDesc != -1)
                        {
                            // Attempt to find the ETA where it appears
                            var eta = worldFile.Eta.SelectMany(x => x).SelectMany(x => x).FindItem(x => x.AnimationIndex == animDesc);

                            if (eta != null)
                            {
                                // Attempt to find the level where it appears
                                var lvlMatch = levels.Find(x => x.Events.Any(y => y.DES == desOffset + 1 + i && y.Etat == eta.Etat && y.SubEtat == eta.SubEtat && y.ETA == worldFile.Eta.FindItemIndex(z => z.SelectMany(h => h).Contains(eta))));

                                if (lvlMatch != null)
                                {
                                    lvl = lvlMatch;
                                    foundCorrectPalette = true;
                                }
                            }
                        }

                        // Check background DES
                        if (!foundCorrectPalette)
                        {
                            var lvlMatch = levels.FindLast(x => x.BackgroundSpritesDES == desOffset + 1 + i);

                            if (lvlMatch != null)
                                lvl = lvlMatch;
                        }

                        // Get the image descriptor
                        var imgDescriptor = desItem.ImageDescriptors[j];

                        // Get the image properties
                        var width = imgDescriptor.OuterWidth;
                        var height = imgDescriptor.OuterHeight;
                        var offset = imgDescriptor.ImageOffset;

                        // Create the texture
                        tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
                        {
                            filterMode = FilterMode.Point
                        };

                        // Set every pixel
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                // Get the pixel offset
                                var pixelOffset = y * width + x + offset;

                                // Get the pixel and decrypt it
                                var pixel = desItem.ImageData[pixelOffset] ^ 143;

                                // Get the color from the palette
                                var color = pixel > 159 ? new ARGBColor(0, 0, 0, 0) : lvl.ColorPalettes[0][pixel];

                                // Set the pixel
                                tex.SetPixel(x, height - y - 1, color.GetColor());
                            }
                        }

                        // Apply the changes
                        tex.Apply();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting sprite {i}-{j}: {ex.Message}");

                        continue;
                    }

                    // Write the texture
                    File.WriteAllBytes(Path.Combine(outputDir, $"{i.ToString().PadLeft(2, '0')}{j.ToString().PadLeft(2, '0')}.png"), tex.EncodeToPNG());
                }
            }
        }

        /// <summary>
        /// Gets the frames for a sprite
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="desItem">The sprite group</param>
        /// <param name="animationDescriptor">The animation descriptor</param>
        /// <returns>The texture</returns>
        public Common_Animation GetSpriteFrames(GameSettings settings, PC_DesItem desItem, PC_AnimationDescriptor animationDescriptor)
        {
            // Create the animation
            var animation = new Common_Animation
            {
                Frames = new Common_AnimationPart[animationDescriptor.FrameCount, animationDescriptor.LayersPerFrame]
            };

            // Load the level to get the palette
            var lvl = FileFactory.Read<PC_LevFile>(GetLevelFilePath(settings), settings);

            // The layer index
            var layer = 0;

            // Create each frame
            for (int i = 0; i < animationDescriptor.FrameCount; i++)
            {
                //// Get the frame
                //var frame = animationDescriptor.Frames[i];
                
                // Create each layer
                for (var layerIndex = 0; layerIndex < animationDescriptor.LayersPerFrame; layerIndex++)
                {
                    var animationLayer = animationDescriptor.Layers[layer];
                    layer++;

                    // Get the sprite
                    var sprite = desItem.ImageDescriptors[animationLayer.ImageIndex];

                    // Get the image properties
                    var width = sprite.OuterWidth;
                    var height = sprite.OuterHeight;
                    var offset = sprite.ImageOffset;

                    // Create the texture
                    Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                        filterMode = FilterMode.Point
                    };

                    // Default to fully transparent
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                        }
                    }

                    // Set every pixel
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Get the pixel offset
                            var pixelOffset = y * width + x + offset;

                            // Get the pixel and decrypt it
                            var pixel = desItem.ImageData[pixelOffset] ^ 143;

                            // Get the color from the palette
                            var color = lvl.ColorPalettes[0][pixel];

                            // Make sure the color isn't transparent (i.e. uses the event palette)
                            if (pixel > 159) 
                                continue;

                            // Get the x pixel position based on if it's flipper or not
                            var pixelX = (animationLayer.IsFlipped ? (width - 1 - x) : x);

                            // Set the pixel
                            tex.SetPixel(pixelX, - (y + 1), color.GetColor());
                        }
                    }

                    // Apply the changes
                    tex.Apply();

                    // Create the animation part
                    var part = new Common_AnimationPart
                    {
                        Sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16,
                            20),
                        X = animationLayer.XPosition,
                        Y = animationLayer.YPosition
                    };

                    // Add the texture
                    animation.Frames[i, layerIndex] = part;
                }
            }

            // Return the texture
            return animation;
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventInfoData">The loaded event info data</param>
        /// <returns>The level</returns>
        public async Task<Common_Lev> LoadLevelAsync(GameSettings settings, EventInfoData[] eventInfoData)
        {
            Controller.status = $"Loading map data for {settings.World} {settings.Level}";

            // Read the level data
            var levelData = FileFactory.Read<PC_LevFile>(GetLevelFilePath(settings), settings);

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
                // Set the dimensions
                Width = levelData.Width,
                Height = levelData.Height,

                // Create the events list
                Events = new List<Common_Event>(),

                // Create the tile arrays
                TileSet = new Common_Tileset[4],
                Tiles = new Common_Tile[levelData.Width * levelData.Height]
            };

            Controller.status = $"Loading allfix";

            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(GetAllfixFilePath(settings), settings);

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading world";

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(settings), settings);

            await Controller.WaitIfNecessary();

            // Get the DES and ETA
            var des = allfix.DesItems.Concat(worldData.DesItems).ToArray();
            var eta = allfix.Eta.Concat(worldData.Eta).ToArray();

            // Add the events
            commonLev.Events = new List<Common_Event>();

            var index = 0;

            foreach (var e in levelData.Events)
            {
                Controller.status = $"Loading event {index}/{levelData.EventCount}";

                await Controller.WaitIfNecessary();

                // Instantiate event prefab using LevelEventController
                var ee = Controller.obj.levelEventController.AddEvent(
                    eventInfoData.FindItem(y => y.GetEventID() == e.GetEventID()),
                    e.XPosition,
                    e.YPosition,
                    e.OffsetBX,
                    e.OffsetBY,
                    levelData.EventLinkingTable[index]);

                try
                {
                    // Get the DES item
                    var desItem = des[e.DES - 1];

                    // Find the ETA item
                    var etaItem = eta[e.ETA].SelectMany(x => x).FindItem(x => x.Etat == e.Etat && x.SubEtat == e.SubEtat);

                    // Find the animation index
                    var animIndex = etaItem.AnimationIndex;
                    
                    // Get the animation item
                    var animItem = desItem.AnimationDescriptors[animIndex];

                    Common_Animation animation = GetSpriteFrames(settings, desItem, animItem);

                    ee.CommonAnimation = animation;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Reading sprite animation failed for DES {e.DES}: {ex.Message}");
                }

                // Add the event
                commonLev.Events.Add(ee);

                index++;
            }

            Controller.status = $"Loading tile set";

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            commonLev.TileSet[1] = tileSets[0];
            commonLev.TileSet[2] = tileSets[1];
            commonLev.TileSet[3] = tileSets[2];

            // Get the palette changers
            var paletteXChangers = levelData.Events.Where(x => x.Type == 158 && x.SubEtat < 6).ToDictionary(x => x.XPosition, x => (PC_PaletteChangerMode)x.SubEtat);
            var paletteYChangers = levelData.Events.Where(x => x.Type == 158 && x.SubEtat >= 6).ToDictionary(x => x.YPosition, x => (PC_PaletteChangerMode)x.SubEtat);

            // Make sure we don't have both horizontal and vertical palette changers as they would conflict
            if (paletteXChangers.Any() && paletteYChangers.Any())
                throw new Exception("Horizontal and vertical palette changers can't both appear in the same level");

            // Check which type of palette changer we have
            bool isPaletteHorizontal = paletteXChangers.Any();

            // Keep track of the default palette
            int defaultPalette = 1;

            // Get the default palette
            if (isPaletteHorizontal && paletteXChangers.Any())
            {
                switch (paletteXChangers.OrderBy(x => x.Key).First().Value)
                {
                    case PC_PaletteChangerMode.Left1toRight2:
                    case PC_PaletteChangerMode.Left1toRight3:
                        defaultPalette = 1;
                        break;
                    case PC_PaletteChangerMode.Left2toRight1:
                    case PC_PaletteChangerMode.Left2toRight3:
                        defaultPalette = 2;
                        break;
                    case PC_PaletteChangerMode.Left3toRight1:
                    case PC_PaletteChangerMode.Left3toRight2:
                        defaultPalette = 3;
                        break;
                }
            }
            else if (!isPaletteHorizontal && paletteYChangers.Any())
            {
                switch (paletteYChangers.OrderByDescending(x => x.Key).First().Value)
                {
                    case PC_PaletteChangerMode.Top1tobottom2:
                    case PC_PaletteChangerMode.Top1tobottom3:
                        defaultPalette = 1;
                        break;
                    case PC_PaletteChangerMode.Top2tobottom1:
                    case PC_PaletteChangerMode.Top2tobottom3:
                        defaultPalette = 2;
                        break;
                    case PC_PaletteChangerMode.Top3tobottom1:
                    case PC_PaletteChangerMode.Top3tobottom2:
                        defaultPalette = 3;
                        break;
                }
            }

            // Keep track of the current palette
            int currentPalette = defaultPalette;

            // Enumerate each cell
            for (int cellY = 0; cellY < levelData.Height; cellY++)
            {
                // Reset the palette on each row if we have a horizontal changer
                if (isPaletteHorizontal)
                    currentPalette = defaultPalette;
                // Otherwise check the y position
                else
                {
                    // Check every pixel 16 steps forward
                    for (int y = 0; y < CellSize; y++)
                    {
                        // Attempt to find a matching palette changer on this pixel
                        var py = paletteYChangers.TryGetValue((uint)(CellSize * cellY + y), out PC_PaletteChangerMode pm) ? (PC_PaletteChangerMode?)pm : null;

                        // If one was found, change the palette based on type
                        if (py != null)
                        {
                            switch (py)
                            {
                                case PC_PaletteChangerMode.Top2tobottom1:
                                case PC_PaletteChangerMode.Top3tobottom1:
                                    currentPalette = 1;
                                    break;
                                case PC_PaletteChangerMode.Top1tobottom2:
                                case PC_PaletteChangerMode.Top3tobottom2:
                                    currentPalette = 2;
                                    break;
                                case PC_PaletteChangerMode.Top1tobottom3:
                                case PC_PaletteChangerMode.Top2tobottom3:
                                    currentPalette = 3;
                                    break;
                            }
                        }
                    }
                }

                for (int cellX = 0; cellX < levelData.Width; cellX++)
                {
                    // Get the cell
                    var cell = levelData.Tiles[cellY * levelData.Width + cellX];

                    // Check the x position for palette changing
                    if (isPaletteHorizontal)
                    {
                        // Check every pixel 16 steps forward
                        for (int x = 0; x < CellSize; x++)
                        {
                            // Attempt to find a matching palette changer on this pixel
                            var px = paletteXChangers.TryGetValue((uint)(CellSize * cellX + x), out PC_PaletteChangerMode pm) ? (PC_PaletteChangerMode?)pm : null;

                            // If one was found, change the palette based on type
                            if (px != null)
                            {
                                switch (px)
                                {
                                    case PC_PaletteChangerMode.Left3toRight1:
                                    case PC_PaletteChangerMode.Left2toRight1:
                                        currentPalette = 1;
                                        break;
                                    case PC_PaletteChangerMode.Left1toRight2:
                                    case PC_PaletteChangerMode.Left3toRight2:
                                        currentPalette = 2;
                                        break;
                                    case PC_PaletteChangerMode.Left1toRight3:
                                    case PC_PaletteChangerMode.Left2toRight3:
                                        currentPalette = 3;
                                        break;
                                }
                            }
                        }
                    }

                    // Get the texture index, default to -1 for fully transparent (no texture)
                    var textureIndex = -1;

                    // Ignore if fully transparent
                    if (cell.TransparencyMode != PC_MapTileTransparencyMode.FullyTransparent)
                    {
                        // Get the offset for the texture
                        var texOffset = levelData.TexturesOffsetTable[cell.TextureIndex];

                        // Get the texture
                        var texture = cell.TransparencyMode == PC_MapTileTransparencyMode.NoTransparency ? levelData.NonTransparentTextures.FindItem(x => x.Offset == texOffset) : levelData.TransparentTextures.FindItem(x => x.Offset == texOffset);

                        // Get the index
                        textureIndex = levelData.NonTransparentTextures.Concat(levelData.TransparentTextures).FindItemIndex(x => x == texture);
                    }

                    // Set the common tile
                    commonLev.Tiles[cellY * levelData.Width + cellX] = new Common_Tile()
                    {
                        TileSetGraphicIndex = textureIndex,
                        CollisionType = cell.CollisionType,
                        PaletteIndex = currentPalette,
                        XPosition = cellX,
                        YPosition = cellY
                    };
                }
            }

            // Return the common level data
            return commonLev;
        }

        /// <summary>
        /// Reads 3 tile-sets, one for each palette
        /// </summary>
        /// <param name="levData">The level data to get the tile-set for</param>
        /// <returns>The 3 tile-sets</returns>
        public Common_Tileset[] ReadTileSets(PC_LevFile levData)
        {
            // Create the output array
            var output = new Common_Tileset[]
            {
                new Common_Tileset(new Tile[levData.TexturesCount]),
                new Common_Tileset(new Tile[levData.TexturesCount]),
                new Common_Tileset(new Tile[levData.TexturesCount]),
            };

            // Keep track of the tile index
            int index = 0;

            // Enumerate every texture
            foreach (var texture in levData.NonTransparentTextures.Concat(levData.TransparentTextures))
            {
                // Enumerate every palette
                for (int i = 0; i < levData.ColorPalettes.Length; i++)
                {
                    // Create the texture to use for the tile
                    var tileTexture = new Texture2D(CellSize, CellSize, TextureFormat.RGBA32, false)
                    {
                        filterMode = FilterMode.Point
                    };

                    // Write each pixel to the texture
                    for (int y = 0; y < CellSize; y++)
                    {
                        for (int x = 0; x < CellSize; x++)
                        {
                            // Get the index
                            var cellIndex = CellSize * y + x;

                            // Get the color from the current palette
                            var c = levData.ColorPalettes[i][255 - texture.ColorIndexes[cellIndex]].GetColor();

                            // If the texture is transparent, add the alpha channel
                            if (texture is PC_TransparentTileTexture tt)
                                c.a = (float)tt.Alpha[cellIndex] / Byte.MaxValue;

                            // Set the pixel
                            tileTexture.SetPixel(x, y, c);
                        }
                    }

                    // Apply the pixels to the texture
                    tileTexture.Apply();

                    // Create and set up the tile
                    output[i].SetTile(tileTexture, CellSize, index);
                }

                index++;
            }

            return output;
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(GameSettings settings, Common_Lev commonLevelData)
        {
            // Get the level file path
            var lvlPath = GetLevelFilePath(settings);

            // Get the level data
            var lvlData = FileFactory.Read<PC_LevFile>(lvlPath, settings);

            // Update the tiles
            for (int y = 0; y < lvlData.Height; y++)
            {
                for (int x = 0; x < lvlData.Width; x++)
                {
                    // Get the tiles
                    var tile = lvlData.Tiles[y * lvlData.Width + x];
                    var commonTile = commonLevelData.Tiles[y * lvlData.Width + x];

                    // Update the tile
                    tile.CollisionType = commonTile.CollisionType;

                    if (commonTile.TileSetGraphicIndex == -1)
                    {
                        tile.TextureIndex = 0;
                        tile.TransparencyMode = PC_MapTileTransparencyMode.FullyTransparent;
                    }
                    else if (commonTile.TileSetGraphicIndex < lvlData.NonTransparentTexturesCount)
                    {
                        tile.TextureIndex = (ushort)lvlData.TexturesOffsetTable.FindItemIndex(z => z == lvlData.NonTransparentTextures[commonTile.TileSetGraphicIndex].Offset);
                        tile.TransparencyMode = PC_MapTileTransparencyMode.NoTransparency;
                    }
                    else
                    {
                        tile.TextureIndex = (ushort)lvlData.TexturesOffsetTable.FindItemIndex(z => z == lvlData.TransparentTextures[(commonTile.TileSetGraphicIndex - lvlData.NonTransparentTexturesCount)].Offset);
                        tile.TransparencyMode = PC_MapTileTransparencyMode.PartiallyTransparent;
                    }
                }
            }

            // Temporary event lists
            var events = new List<PC_Event>();
            var eventCommands = new List<PC_EventCommand>();
            var eventLinkingTable = new List<ushort>();

            // Set events
            foreach (var e in commonLevelData.Events)
            {
                // Get the event
                var r1Event = e.EventInfoData.PC_R1_Info.ToEvent(settings.World);

                // Set position
                r1Event.XPosition = e.XPosition;
                r1Event.YPosition = e.YPosition;

                // Set type values
                r1Event.Type = (uint)e.EventInfoData.Type;
                r1Event.Etat = (byte)e.EventInfoData.Etat;
                r1Event.SubEtat = (byte)e.EventInfoData.SubEtat;

                // Add the event
                events.Add(r1Event);

                // Add the event commands
                eventCommands.Add(new PC_EventCommand()
                {
                    CodeCount = (ushort)e.EventInfoData.PC_R1_Info.Commands.Length,
                    EventCode = e.EventInfoData.PC_R1_Info.Commands,
                    LabelOffsetCount = (ushort)e.EventInfoData.PC_R1_Info.LabelOffsets.Length,
                    LabelOffsetTable = e.EventInfoData.PC_R1_Info.LabelOffsets
                });

                // Add the event links
                eventLinkingTable.Add((ushort)e.LinkIndex);
            }

            // Update event values
            lvlData.EventCount = (ushort)events.Count;
            lvlData.Events = events.ToArray();
            lvlData.EventCommands = eventCommands.ToArray();
            lvlData.EventLinkingTable = eventLinkingTable.ToArray();

            // Save the file
            FileFactory.Write(lvlPath, settings);
        }

        #endregion
    }
}