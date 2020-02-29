using System;
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
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(string basePath, World world)
        {
            // Helper method for getting the folder name for the world
            string GetWorldFolderName()
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

            return Path.Combine(basePath, "PCMAP", GetWorldFolderName());
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
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <returns>The level</returns>
        public Common_Lev LoadLevel(string basePath, World world, int level)
        {
            // Read the level data
            var levelData = FileFactory.Read<PC_R1_LevFile>(GetLevelFilePath(basePath, world, level));

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev
            {
                Width = levelData.MapWidth,
                Height = levelData.MapHeight,
                Events = levelData.Events.Select(x => new Event()
                {
                    pos = new PxlVec((ushort) x.XPosition, (ushort) x.YPosition),
                    type = (EventType)x.Type
                }).ToArray(),
                TileSet = new Common_Tileset[4],
                Tiles = new Common_Tile[levelData.MapWidth * levelData.MapHeight]
            };

            // TODO: Clean up by making a common event class

            var sets = ReadTileSets(levelData);

            c.TileSet[1] = sets[0];
            c.TileSet[2] = sets[1];
            c.TileSet[3] = sets[2];

            // Get the palette changers
            var paletteXChangers = levelData.Events.Where(x => x.Type == 158 && x.SubEtat < 6).ToDictionary(x => x.XPosition, x => (PC_R1_PaletteChangerMode)x.SubEtat);                  
            var paletteYChangers = levelData.Events.Where(x => x.Type == 158 && x.SubEtat >= 6).ToDictionary(x => x.YPosition, x => (PC_R1_PaletteChangerMode)x.SubEtat);

            if (paletteXChangers.Any() && paletteYChangers.Any())
                throw new Exception("Horizontal and vertical palette changers can't both appear in the same level");

            bool isPaletteHorizontal = paletteXChangers.Any();
            int currentPalette = 0;

            // Enumerate each cell
            for (int cellY = 0; cellY < levelData.MapHeight; cellY++)
            {
                if (isPaletteHorizontal)
                    currentPalette = 0;
                else
                {
                    for (int y = 0; y < CellSize; y++)
                    {
                        var py = paletteYChangers.TryGetValue((uint)(CellSize * cellY + y), out PC_R1_PaletteChangerMode pm) ? (PC_R1_PaletteChangerMode?)pm : null;

                        if (py != null)
                        {
                            switch (py)
                            {
                                case PC_R1_PaletteChangerMode.Top2tobottom1:
                                case PC_R1_PaletteChangerMode.Top3tobottom1:
                                    currentPalette = 0;
                                    break;
                                case PC_R1_PaletteChangerMode.Top1tobottom2:
                                case PC_R1_PaletteChangerMode.Top3tobottom2:
                                    currentPalette = 1;
                                    break;
                                case PC_R1_PaletteChangerMode.Top1tobottom3:
                                case PC_R1_PaletteChangerMode.Top2tobottom3:
                                    currentPalette = 2;
                                    break;
                            }
                        }
                    }
                }

                for (int cellX = 0; cellX < levelData.MapWidth; cellX++)
                {
                    // Get the cell
                    var cell = levelData.Tiles[cellY * levelData.MapWidth + cellX];

                    if (isPaletteHorizontal)
                    {
                        for (int x = 0; x < CellSize; x++)
                        {
                            var px = paletteXChangers.TryGetValue((uint)(CellSize * cellX + x), out PC_R1_PaletteChangerMode pm) ? (PC_R1_PaletteChangerMode?)pm : null;

                            if (px != null)
                            {
                                switch (px)
                                {
                                    case PC_R1_PaletteChangerMode.Left3toRight1:
                                    case PC_R1_PaletteChangerMode.Left2toRight1:
                                        currentPalette = 0;
                                        break;
                                    case PC_R1_PaletteChangerMode.Left1toRight2:
                                    case PC_R1_PaletteChangerMode.Left3toRight2:
                                        currentPalette = 1;
                                        break;
                                    case PC_R1_PaletteChangerMode.Left1toRight3:
                                    case PC_R1_PaletteChangerMode.Left2toRight3:
                                        currentPalette = 2;
                                        break;
                                }
                            }
                        }
                    }

                    // Ignore if fully transparent
                    if (cell.TransparencyMode == PC_R1_MapTile.PC_R1_MapTileTransparencyMode.FullyTransparent)
                    {
                        c.Tiles[cellY * levelData.MapWidth + cellX] = new Common_Tile()
                        {
                            gIndex = 0,
                            cType = cell.CollisionType,
                            palette = currentPalette + 1,
                            x = cellX,
                            y = cellY
                        };

                        continue;
                    }

                    // Get the offset for the texture
                    var texOffset = levelData.TexturesOffsetTable[cell.TextureIndex];

                    // Get the texture
                    var texture = cell.TransparencyMode == PC_R1_MapTile.PC_R1_MapTileTransparencyMode.NoTransparency ? levelData.NonTransparentTextures.FindItem(x => x.Offset == texOffset) : levelData.TransparentTextures.FindItem(x => x.Offset == texOffset);

                    // Get the index
                    var index = levelData.NonTransparentTextures.FindItemIndex(x => x == texture);

                    if (index == -1)
                        index = levelData.TransparentTextures.FindItemIndex(x => x == texture) + levelData.NonTransparentTextures.Length;

                    c.Tiles[cellY * levelData.MapWidth + cellX] = new Common_Tile()
                    {
                        gIndex = index,
                        cType = cell.CollisionType,
                        palette = currentPalette + 1,
                        x = cellX,
                        y = cellY
                    };
                }
            }

            return c;
        }

        public Common_Tileset[] ReadTileSets(PC_R1_LevFile levData) 
        {
            var output = new Common_Tileset[]
            {
                new Common_Tileset(new Tile[levData.TexturesCount]),
                new Common_Tileset(new Tile[levData.TexturesCount]),
                new Common_Tileset(new Tile[levData.TexturesCount]),
            };

            int index = 0;

            foreach (var texture in levData.NonTransparentTextures.Concat(levData.TransparentTextures))
            {
                for (int i = 0; i < 3; i++)
                {
                    var tileTexture = new Texture2D(CellSize, CellSize, TextureFormat.RGBA32, false)
                    {
                        filterMode = FilterMode.Point
                    };

                    // Write each pixel for the texture
                    for (int x = 0; x < CellSize; x++)
                    {
                        for (int y = 0; y < CellSize; y++)
                        {
                            // Get the color
                            var c = levData.ColorPalettes[i][texture.ColorIndexes[x, y]].GetColor();

                            // If the texture is transparent, replace the color with one with the alpha channel
                            if (texture is PC_R1_TransparentTileTexture tt)
                                c.a = (float)tt.Alpha[x, y] / Byte.MaxValue;

                            // Set the pixel
                            tileTexture.SetPixel(x, y, c);
                        }
                    }

                    tileTexture.Apply();

                    output[i].Tiles[index] = ScriptableObject.CreateInstance<Tile>();
                    output[i].Tiles[index].sprite = Sprite.Create(tileTexture, new Rect(0, 0, CellSize, CellSize), new Vector2(0.5f, 0.5f), CellSize, 20);
                }

                index++;
            }

            return output;
        }
    }
}