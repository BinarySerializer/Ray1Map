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
    /// The game manager for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Manager : IGameManager
    {
        #region Values and paths

        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public string GetLevelFilePath(GameSettings settings)
        {
            return Path.Combine(GetWorldFolderPath(settings), $"{GetWorldName(settings.World)}{settings.Level:00}.XXX");
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
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(GameSettings settings)
        {

            return Path.Combine(settings.GameDirectory, "RAY", GetWorldName(settings.World));
        }

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public int[] GetLevels(GameSettings settings)
        {
            var worldPath = GetWorldFolderPath(settings);

            return Enumerable.Range(1, Directory.EnumerateFiles(worldPath, "*.XXX", SearchOption.TopDirectoryOnly).Count(x => Path.GetFileNameWithoutExtension(x)?.Length == 5)).ToArray();
        }

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public virtual string[] GetEduVolumes(GameSettings settings) => new string[0];

        #endregion

        #region Manager Methods

        /// <summary>
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The tile set</returns>
        public Common_Tileset ReadTileSet(GameSettings settings)
        {
            // Get the file name
            var fileName = Path.Combine(GetWorldFolderPath(settings), $"{GetWorldName(settings.World)}.XXX");

            // Read the file
            var worldFile = FileFactory.Read<PS1_R1_WorldFile>(fileName, settings);

            int tile = 0;
            int tileCount = worldFile.TilePaletteIndexTable.Length;
            const int width = 256;
            int height = (worldFile.TilesIndexTable.Length) / width;
            Color[] pixels = new Color[width * height];

            for (int yB = 0; yB < height; yB += 16)
                for (int xB = 0; xB < width; xB += 16, tile++)
                    for (int y = 0; y < CellSize; y++)
                        for (int x = 0; x < CellSize; x++)
                        {
                            if (tile >= tileCount)
                                goto End;

                            int pixel = x + xB + (y + yB) * width;

                            pixels[pixel] = worldFile.TileColorPalettes[worldFile.TilePaletteIndexTable[tile]][worldFile.TilesIndexTable[pixel]].GetColor();
                        }
            End:
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            tex.SetPixels(pixels);
            tex.Apply();

            var tiles = new Tile[tex.width * tex.height];

            // Loop through all the 16x16 cells in the tileset Texture2D and generate tiles out of it
            int tileIndex = 0;
            for (int yy = 0; yy < (tex.height / CellSize); yy++)
            {
                for (int xx = 0; xx < (tex.width / CellSize); xx++)
                {
                    // Create a tile
                    Tile t = ScriptableObject.CreateInstance<Tile>();
                    t.sprite = Sprite.Create(tex, new Rect(xx * CellSize, yy * CellSize, CellSize, CellSize),
                        new Vector2(0.5f, 0.5f), CellSize, 20);
                    tiles[tileIndex] = t;
                    tileIndex++;
                }
            }

            return new Common_Tileset(tiles);
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventInfoData">The loaded event info data</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public async Task<Common_Lev> LoadLevelAsync(GameSettings settings, EventInfoData[] eventInfoData, List<Common_Design> eventDesigns)
        {
            Controller.status = $"Loading map data for {settings.World} {settings.Level}";

            // Read the level
            var levelData = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(settings), settings);

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev
            {
                // Set the dimensions
                Width = levelData.Width,
                Height = levelData.Height,

                // Create the events list
                Events = new List<Common_Event>(),

                // Create the tile array
                TileSet = new Common_Tileset[4]
            };

            var index = 0;

            // Add the events
            foreach (var e in levelData.Events)
            {
                Controller.status = $"Loading event {index}/{levelData.EventCount}";
                /*
                // Instantiate event prefab using LevelEventController
                c.Events.Add(Controller.obj.levelEventController.AddEvent(
                    eventInfoData.FindItem(y => y.GetEventID() == e.GetEventID()),
                    e.XPosition,
                    e.YPosition,
                    e.OffsetBX,
                    e.OffsetBY,
                    levelData.EventLinkingTable[index]));
                    */
                index++;
            }

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading tile set";

            Common_Tileset tileSet = ReadTileSet(settings);
            c.TileSet[1] = tileSet;

            await Controller.WaitIfNecessary();

            c.Tiles = ConvertTilesToCommon(levelData.Tiles, levelData.Width, levelData.Height);

            return c;
        }

        /// <summary>
        /// Converts a PS1_R1_Tile array to Common_Tile array
        /// </summary>
        /// <param name="tiles">Array of PS1 tiles</param>
        /// <param name="w">Level width</param>
        /// <param name="h">Level height</param>
        /// <returns>Common_Tile array</returns>
        public Common_Tile[] ConvertTilesToCommon(PS1_R1_MapTile[] tiles, ushort w, ushort h)
        {
            Common_Tile[] finalTiles = new Common_Tile[w * h];

            int tileIndex = 0;
            for (int ty = 0; ty < (h); ty++)
            {
                for (int tx = 0; tx < (w); tx++)
                {
                    var graphicX = tiles[tileIndex].TileMapX;
                    var graphicY = tiles[tileIndex].TileMapY;

                    Common_Tile newTile = new Common_Tile
                    {
                        PaletteIndex = 1,
                        XPosition = tx,
                        YPosition = ty,
                        CollisionType = tiles[tileIndex].CollisionType,
                        TileSetGraphicIndex = (CellSize * graphicY) + graphicX
                    };

                    finalTiles[tileIndex] = newTile;

                    tileIndex++;
                }
            }

            return finalTiles;
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
            var lvlData = FileFactory.Read<PS1_R1_LevFile>(lvlPath, settings);

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
                    tile.TileMapY = (int)Math.Floor(commonTile.TileSetGraphicIndex / 16d);
                    tile.TileMapX = commonTile.TileSetGraphicIndex - (CellSize * tile.TileMapY);
                }
            }

            // Set events
            // TODO: Implement

            // Save the file
            FileFactory.Write(lvlPath, settings);
        }

        #endregion
    }
}