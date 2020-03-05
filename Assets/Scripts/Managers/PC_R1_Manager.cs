using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_Manager : IGameManager
    {
        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <returns>The level file path</returns>
        public string GetLevelFilePath(string basePath, World world, int level)
        {
            return Path.Combine(GetWorldFolderPath(basePath, world), $"RAY{level}.LEV");
        }

        /// <summary>
        /// Gets the file path for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The world file path</returns>
        public string GetWorldFilePath(string basePath, World world)
        {
            return Path.Combine(basePath, "PCMAP", $"RAY{(int)world + 1}.WLD");
        }

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
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(string basePath, World world)
        {
            return Path.Combine(basePath, "PCMAP", GetWorldName(world));
        }

        /// <summary>
        /// Gets the level count for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The level count</returns>
        public int GetLevelCount(string basePath, World world)
        {
            var worldPath = GetWorldFolderPath(basePath, world);

            return Directory.EnumerateFiles(worldPath, "RAY??.LEV", SearchOption.TopDirectoryOnly).Count();
        }

        /// <summary>
        /// Exports all sprite textures to the specified output directory
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="outputDir">The output directory</param>
        public void ExportSpriteTextures(string basePath, string outputDir)
        {
            // Enumerate every world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Read the world file
                var worldFile = FileFactory.Read<PC_R1_WorldFile>(GetWorldFilePath(basePath, world));

                // Enumerate each sprite group
                for (int i = 0; i < worldFile.SpriteGroups.Length; i++)
                {
                    // Get the sprite group
                    var sprite = worldFile.SpriteGroups[i];

                    // Enumerate each image
                    for (int j = 0; j < sprite.ImageDescriptors.Length; j++)
                    {
                        Texture2D tex;

                        try
                        {
                            // Get the texture
                            tex = GetSpriteTexture(basePath, world, 1, sprite, sprite.ImageDescriptors[j]);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Error exporting sprite {i}-{j} in {world}: {ex.Message}");

                            continue;
                        }

                        // Get the directory
                        var dir = Path.Combine(outputDir, world.ToString());

                        // Create the directory
                        Directory.CreateDirectory(dir);

                        // Write the texture
                        File.WriteAllBytes(Path.Combine(dir, $"{i.ToString().PadLeft(2, '0')}{j.ToString().PadLeft(2, '0')}.png"), tex.EncodeToPNG());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="spriteGroup">The sprite group</param>
        /// <param name="imgDescriptor">The image descriptor</param>
        /// <returns>The texture</returns>
        public Texture2D GetSpriteTexture(string basePath, World world, int level, PC_R1_SpriteGroup spriteGroup, PC_R1_ImageDescriptor imgDescriptor)
        {
            // Load the level to get the palette
            var lvl = FileFactory.Read<PC_R1_LevFile>(GetLevelFilePath(basePath, world, level));

            // Get the image properties
            var width = imgDescriptor.OuterWidth;
            var height = imgDescriptor.OuterHeight;
            var offset = imgDescriptor.ImageOffset;

            // Create the texture
            var tex = new Texture2D(width, height);

            // Set every pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get the pixel offset
                    var pixelOffset = y * width + x + offset;

                    // Get the pixel and decrypt it
                    var pixel = spriteGroup.ImageData[pixelOffset] ^ 112;

                    // Get the color from the palette
                    var color = pixel < 96 ? new ARGBColor(0, 0, 0, 0) : lvl.ColorPalettes[0][pixel];

                    // Set the pixel
                    tex.SetPixel(x, height - y - 1, color.GetColor());
                }
            }

            // Apply the changes
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="eventInfoData">The loaded event info data</param>
        /// <returns>The level</returns>
        public Common_Lev LoadLevel(string basePath, World world, int level, EventInfoData[] eventInfoData)
        {
            // Read the level data
            var levelData = FileFactory.Read<PC_R1_LevFile>(GetLevelFilePath(basePath, world, level));

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

            var index = 0;

            // Read the world data
            var worldData = FileFactory.Read<PC_R1_WorldFile>(GetWorldFilePath(basePath, world));

            // Add the events
            commonLev.Events = new List<Common_Event>();
            foreach (var e in levelData.Events)
            {
                // Instantiate event prefab using LevelEventController
                var ee = Controller.obj.levelEventController.AddEvent(
                    eventInfoData.FindItem(y => y.GetEventID() == e.GetEventID()),
                    e.XPosition,
                    e.YPosition,
                    e.OffsetBX,
                    e.OffsetBY,
                    levelData.EventLinkingTable[index]);

                // Set the event sprite
                ee.SetSprite(GetSpriteTexture(basePath, world, level, worldData.SpriteGroups[4], worldData.SpriteGroups[4].ImageDescriptors[31]));

                // Add the event
                commonLev.Events.Add(ee);

                index++;
            }

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            commonLev.TileSet[1] = tileSets[0];
            commonLev.TileSet[2] = tileSets[1];
            commonLev.TileSet[3] = tileSets[2];

            // Get the palette changers
            var paletteXChangers = levelData.Events.Where(x => x.Type == 158 && x.SubEtat < 6).ToDictionary(x => x.XPosition, x => (PC_R1_PaletteChangerMode)x.SubEtat);                  
            var paletteYChangers = levelData.Events.Where(x => x.Type == 158 && x.SubEtat >= 6).ToDictionary(x => x.YPosition, x => (PC_R1_PaletteChangerMode)x.SubEtat);

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
                    case PC_R1_PaletteChangerMode.Left1toRight2:
                    case PC_R1_PaletteChangerMode.Left1toRight3:
                        defaultPalette = 1;
                        break;
                    case PC_R1_PaletteChangerMode.Left2toRight1:
                    case PC_R1_PaletteChangerMode.Left2toRight3:
                        defaultPalette = 2;
                        break;
                    case PC_R1_PaletteChangerMode.Left3toRight1:
                    case PC_R1_PaletteChangerMode.Left3toRight2:
                        defaultPalette = 3;
                        break;
                }
            }
            else if (!isPaletteHorizontal && paletteYChangers.Any())
            {
                switch (paletteYChangers.OrderByDescending(x => x.Key).First().Value)
                {
                    case PC_R1_PaletteChangerMode.Top1tobottom2:
                    case PC_R1_PaletteChangerMode.Top1tobottom3:
                        defaultPalette = 1;
                        break;
                    case PC_R1_PaletteChangerMode.Top2tobottom1:
                    case PC_R1_PaletteChangerMode.Top2tobottom3:
                        defaultPalette = 2;
                        break;
                    case PC_R1_PaletteChangerMode.Top3tobottom1:
                    case PC_R1_PaletteChangerMode.Top3tobottom2:
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
                        var py = paletteYChangers.TryGetValue((uint)(CellSize * cellY + y), out PC_R1_PaletteChangerMode pm) ? (PC_R1_PaletteChangerMode?)pm : null;

                        // If one was found, change the palette based on type
                        if (py != null)
                        {
                            switch (py)
                            {
                                case PC_R1_PaletteChangerMode.Top2tobottom1:
                                case PC_R1_PaletteChangerMode.Top3tobottom1:
                                    currentPalette = 1;
                                    break;
                                case PC_R1_PaletteChangerMode.Top1tobottom2:
                                case PC_R1_PaletteChangerMode.Top3tobottom2:
                                    currentPalette = 2;
                                    break;
                                case PC_R1_PaletteChangerMode.Top1tobottom3:
                                case PC_R1_PaletteChangerMode.Top2tobottom3:
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
                            var px = paletteXChangers.TryGetValue((uint)(CellSize * cellX + x), out PC_R1_PaletteChangerMode pm) ? (PC_R1_PaletteChangerMode?)pm : null;

                            // If one was found, change the palette based on type
                            if (px != null)
                            {
                                switch (px)
                                {
                                    case PC_R1_PaletteChangerMode.Left3toRight1:
                                    case PC_R1_PaletteChangerMode.Left2toRight1:
                                        currentPalette = 1;
                                        break;
                                    case PC_R1_PaletteChangerMode.Left1toRight2:
                                    case PC_R1_PaletteChangerMode.Left3toRight2:
                                        currentPalette = 2;
                                        break;
                                    case PC_R1_PaletteChangerMode.Left1toRight3:
                                    case PC_R1_PaletteChangerMode.Left2toRight3:
                                        currentPalette = 3;
                                        break;
                                }
                            }
                        }
                    }

                    // Get the texture index, default to -1 for fully transparent (no texture)
                    var textureIndex = -1;

                    // Ignore if fully transparent
                    if (cell.TransparencyMode != PC_R1_MapTileTransparencyMode.FullyTransparent)
                    {
                        // Get the offset for the texture
                        var texOffset = levelData.TexturesOffsetTable[cell.TextureIndex];

                        // Get the texture
                        var texture = cell.TransparencyMode == PC_R1_MapTileTransparencyMode.NoTransparency ? levelData.NonTransparentTextures.FindItem(x => x.Offset == texOffset) : levelData.TransparentTextures.FindItem(x => x.Offset == texOffset);

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
        public Common_Tileset[] ReadTileSets(PC_R1_LevFile levData) 
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
                for (int i = 0; i < 3; i++)
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
                            var c = levData.ColorPalettes[i][texture.ColorIndexes[cellIndex]].GetColor();

                            // If the texture is transparent, add the alpha channel
                            if (texture is PC_R1_TransparentTileTexture tt)
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
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(string basePath, World world, int level, Common_Lev commonLevelData)
        {
            // Get the level file path
            var lvlPath = GetLevelFilePath(basePath, world, level);

            // Get the level data
            var lvlData = FileFactory.Read<PC_R1_LevFile>(lvlPath);

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

                    if (commonTile.TileSetGraphicIndex == -1) {
                        tile.TextureIndex = 0;
                        tile.TransparencyMode = PC_R1_MapTileTransparencyMode.FullyTransparent;
                    }
                    else if (commonTile.TileSetGraphicIndex < lvlData.NonTransparentTexturesCount) {
                        tile.TextureIndex = (ushort)lvlData.TexturesOffsetTable.FindItemIndex(z => z == lvlData.NonTransparentTextures[commonTile.TileSetGraphicIndex].Offset);
                        tile.TransparencyMode = PC_R1_MapTileTransparencyMode.NoTransparency;
                    }
                    else {
                        tile.TextureIndex = (ushort)lvlData.TexturesOffsetTable.FindItemIndex(z => z == lvlData.TransparentTextures[(commonTile.TileSetGraphicIndex - lvlData.NonTransparentTexturesCount)].Offset);
                        tile.TransparencyMode = PC_R1_MapTileTransparencyMode.PartiallyTransparent;
                    }
                }
            }

            // Temporary event lists
            var events = new List<PC_R1_Event>();
            var eventCommands = new List<PC_R1_EventCommand>();
            var eventLinkingTable = new List<ushort>();

            // Set events
            foreach (var e in commonLevelData.Events)
            {
                // Get the event
                var r1Event = e.EventInfoData.PC_R1_Info.ToEvent(world);

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
                eventCommands.Add(new PC_R1_EventCommand()
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
            FileFactory.Write(lvlPath);
        }
    }
}