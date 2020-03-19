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
        /// Exports all vignette textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        public void ExportVignetteTextures(GameSettings settings, string outputDir)
        {
            // TODO: This only exports the some .xxx files and not yet the .r16 (raw-16) ones

            // TODO: Get file paths from methods

            var baseDir = Path.Combine(outputDir, "FND");

            Directory.CreateDirectory(baseDir);

            foreach (var filePath in Directory.GetFiles(Path.Combine(settings.GameDirectory, "RAY", "IMA", "FND"), "*.xxx", SearchOption.TopDirectoryOnly))
            {
                // TODO: Replace all of this with a serializable file once the PS1 pointer system is finished

                byte[] file = File.ReadAllBytes(filePath);

                var pointerCount = BitConverter.ToInt32(file, 0);
                var pointer1 = BitConverter.ToInt32(file, 4);
                var pointer2 = BitConverter.ToInt32(file, 8);
                var fileSize = BitConverter.ToInt32(file, 12);

                int length = (pointer2 - pointer1) / 2;
                int height = BitConverter.ToInt16(file, 20);
                int blockSize = height * 64;
                int blockCount = length / blockSize;
                int width = blockCount * 64;

                var tex = new Texture2D(width, height);

                for (int block = 0; block < blockCount; block++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < 64; x++)
                        {
                            int x16 = (x * 2) + 24;
                            int y16 = (y * 2);

                            ushort color16 = BitConverter.ToUInt16(file, x16 + (y16 * 64) + ((block * blockSize) * 2));

                            var red = ((BitHelpers.ExtractBits(color16, 5, 0) / 31f));
                            var green = ((BitHelpers.ExtractBits(color16, 5, 5) / 31f));
                            var blue = ((BitHelpers.ExtractBits(color16, 5, 10) / 31f));

                            tex.SetPixel((x + (block * 64)), tex.height - y - 1, new Color(red, green, blue));
                        }
                    }
                }

                tex.Apply();

                File.WriteAllBytes(Path.Combine(baseDir, $"{Path.GetFileNameWithoutExtension(filePath)}.png"), tex.EncodeToPNG());
            }
        }

        /// <summary>
        /// Exports all sprite textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        public void ExportSpriteTextures(GameSettings settings, string outputDir) => throw new NotImplementedException();

        /// <summary>
        /// Exports all animation frames to the specified directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The directory to export to</param>
        public void ExportAnimationFrames(GameSettings settings, string outputDir) => throw new NotImplementedException();

        /// <summary>
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The tile set</returns>
        public abstract Common_Tileset ReadTileSet(GameSettings settings);

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) { }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public async Task<Common_Lev> LoadLevelAsync(GameSettings settings, List<Common_Design> eventDesigns)
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

            // TODO: Load events

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading tile set";

            Common_Tileset tileSet = ReadTileSet(settings);
            c.TileSet[0] = tileSet;

            await Controller.WaitIfNecessary();

            // Set the tiles
            c.Tiles = new Common_Tile[levelData.Width * levelData.Height];

            int tileIndex = 0;
            for (int y = 0; y < (levelData.Height); y++)
            {
                for (int x = 0; x < (levelData.Width); x++)
                {
                    var graphicX = levelData.Tiles[tileIndex].TileMapX;
                    var graphicY = levelData.Tiles[tileIndex].TileMapY;

                    Common_Tile newTile = new Common_Tile
                    {
                        PaletteIndex = 1,
                        XPosition = x,
                        YPosition = y,
                        CollisionType = levelData.Tiles[tileIndex].CollisionType,
                        TileSetGraphicIndex = (CellSize * graphicY) + graphicX
                    };

                    c.Tiles[tileIndex] = newTile;

                    tileIndex++;
                }
            }

            return c;
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

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public Common_EditorEventInfo GetEditorEventInfo(GameSettings settings, Common_Event e) => null;

        /// <summary>
        /// Gets the animation info for an event
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="e">The event</param>
        /// <returns>The animation info</returns>
        public Common_AnimationInfo GetAnimationInfo(GameSettings settings, Common_Event e) => new Common_AnimationInfo(-1, -1);

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The names of the available events to add</returns>
        public string[] GetEvents(GameSettings settings) => new string[0];

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventController">The event controller to add to</param>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns></returns>
        public Common_Event AddEvent(GameSettings settings, LevelEventController eventController, int index, uint xPos, uint yPos) => throw new NotImplementedException();

        #endregion
    }
}