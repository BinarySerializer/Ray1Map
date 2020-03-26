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
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings) + $"{GetWorldName(settings.World)}{settings.Level:00}.XXX";

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"RAY.XXX";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public virtual string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"INI.XXX";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => GetWorldFolderPath(settings) + $"{GetWorldName(settings.World)}.XXX";

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
        public string GetWorldFolderPath(GameSettings settings) => GetDataPath() + GetWorldName(settings.World) + "/";

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public string GetDataPath() => "RAY/";

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
            var worldPath = settings.GameDirectory + GetWorldFolderPath(settings);

            return Enumerable.Range(1, Directory.EnumerateFiles(worldPath, "*.XXX", SearchOption.TopDirectoryOnly).Count<string>(x => Path.GetFileNameWithoutExtension(x)?.Length == 5)).ToArray<int>();
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
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The tile set</returns>
        public abstract Common_Tileset ReadTileSet(Context context);

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) { }

        public async Task LoadExtraFile(Context context, string path) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = PS1FileInfo.fileInfoUS;
            PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, fileInfo[path].BaseAddress) {
                filePath = path,
                Length = fileInfo[path].Length
            };
            context.AddFile(file);
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public async Task<Common_Lev> LoadLevelAsync(Context context, List<Common_Design> eventDesigns) {
            Controller.status = $"Loading world file";
            await Controller.WaitIfNecessary();

            // Read the allfix file
            var allfixFileName = GetAllfixFilePath(context.Settings);
            var allfixFile = FileFactory.Read<PS1_R1_AllfixFile>(allfixFileName, context);

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            Common_Tileset tileSet = ReadTileSet(context);
            var worldFileName = GetWorldFilePath(context.Settings);
            var worldFile = FileFactory.Read<PS1_R1_WorldFile>(worldFileName, context);

            Controller.status = $"Loading map data for {context.Settings.World} {context.Settings.Level}";

            // Read the level
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var levelData = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            PS1_VRAM vram = FillVRAM(allfixFile, worldFile, levelData);

            foreach (PS1_R1_Event e in levelData.Events) {
                foreach (PS1_R1_ImageDescriptor i in e.ImageDescriptors) {
                    if (i.Offset.file.filePath == worldFileName) {
                        Texture2D tex = GetSpriteTexture("world", i, worldFile.EventPalette1, vram);
                    } else if (i.Offset.file.filePath == allfixFileName) {
                        Texture2D tex = GetSpriteTexture("allfix", i, allfixFile.Palette2, vram);
                    } else if (i.Offset.file.filePath == GetLevelFilePath(context.Settings)) {
                        Texture2D tex = GetSpriteTexture("level", i, worldFile.EventPalette1, vram);
                    }
                }
            }
            /*ExportTexturePage("Allfix_1", allfixFile.Palette1, allfixFile.TextureBlock);
            ExportTexturePage("Allfix_2", allfixFile.Palette2, allfixFile.TextureBlock);
            ExportTexturePage("Allfix_3", allfixFile.Palette3, allfixFile.TextureBlock);
            ExportTexturePage("Allfix_4", allfixFile.Palette4, allfixFile.TextureBlock);
            ExportTexturePage("Allfix_5", allfixFile.Palette5, allfixFile.TextureBlock);
            ExportTexturePage("Allfix_6", allfixFile.Palette6, allfixFile.TextureBlock);
            ExportTexturePage("World_1", worldFile.EventPalette1, worldFile.TextureBlock);
            ExportTexturePage("World_2", worldFile.EventPalette2, worldFile.TextureBlock);
            ExportTexturePage("Level_1", worldFile.EventPalette1, levelData.TextureBlock);
            ExportTexturePage("Level_2", worldFile.EventPalette2, levelData.TextureBlock);*/

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
            c.TileSet[0] = tileSet;

            // TODO: Load events

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
        /// <param name="context">The serialization context</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(Context context, Common_Lev commonLevelData)
        {
            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<PS1_R1_LevFile>(lvlPath);

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
            FileFactory.Write<PS1_R1_LevFile>(lvlPath, context);
        }

        public async Task LoadFilesAsync(Context context) {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            paths["allfix"] = GetAllfixFilePath(context.Settings);
            /*paths["world"] = GetWorldFilePath(context.Settings);
            paths["level"] = GetLevelFilePath(context.Settings);
            paths["bigray"] = GetBigRayFilePath(context.Settings);*/
            Dictionary<string, PS1FileInfo> fileInfo = PS1FileInfo.fileInfoUS;
            foreach (string pathKey in paths.Keys) {
                await FileSystem.PrepareFile(context.BasePath + paths[pathKey]);
                if (!fileInfo.ContainsKey(paths[pathKey])) {
                    throw new Exception("File base address wasn't defined for path " + paths[pathKey]);
                }
                PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, fileInfo[paths[pathKey]].BaseAddress) {
                    filePath = paths[pathKey],
                    Length = fileInfo[paths[pathKey]].Length
                };
                context.AddFile(file);
            }
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
                filterMode = FilterMode.Point
            };

            // Default to fully transparent
            for (int y = 0; y < tex.height; y++) {
                for (int x = 0; x < tex.width; x++) {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            //try {
                // Set every pixel

                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var pixel = vram.GetPixel(pageX, pageY, s.ImageOffsetInPageX + x, s.ImageOffsetInPageY + y);

                        // Get the color from the palette
                        var color = palette[pixel];

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, color.GetColor());
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

        public PS1_VRAM FillVRAM(PS1_R1_AllfixFile allFix, PS1_R1_WorldFile world, PS1_R1_LevFile level) {
            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.skippedPagesX = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * 256 * 2];
            Array.Copy(allFix.TextureBlock, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[allFix.TextureBlock.Length - cageSprites.Length];
            Array.Copy(allFix.TextureBlock, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);

            vram.AddData(cageSprites, 128, 256 * 2);
            vram.AddData(allFixSprites, 256, allFixSprites.Length / 256);

            vram.AddData(world.TextureBlock, 256, world.TextureBlock.Length / 256);
            vram.AddData(level.TextureBlock, 256, level.TextureBlock.Length / 256);

            return vram;
        }


        public void ExportTexturePage(string name, IList<ARGBColor> palette, byte[] processedImageData) {
            // Get the image properties
            var width = 256;
            var height = processedImageData.Length / 256 + (processedImageData.Length % 256 == 0 ? 0 : 1);

            // Create the texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Point
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
        public Common_AnimationInfo GetAnimationInfo(Context context, Common_Event e) => new Common_AnimationInfo(-1, -1);

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