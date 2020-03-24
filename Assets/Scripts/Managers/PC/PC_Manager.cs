using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PC
    /// </summary>
    public abstract class PC_Manager : IGameManager {
        #region Values and paths

        /// <summary>
        /// The loaded PC  info cache
        /// </summary>
        protected static Dictionary<string, GeneralPCEventInfoData[]> EventCache { get; } = new Dictionary<string, GeneralPCEventInfoData[]>();

        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public virtual string GetDataPath() => "PCMAP/";

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public string GetWorldName(World world) {
            switch (world) {
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
        public virtual string GetShortWorldName(World world) {
            switch (world) {
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
        public virtual string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"ALLFIX.DAT";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public virtual string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"BIGRAY.DAT";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public abstract string GetVignetteFilePath(GameSettings settings);

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
        /// Gets the level count for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level count</returns>
        public abstract int[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public virtual string[] GetEduVolumes(GameSettings settings) => new string[0];

        /// <summary>
        /// Gets the DES file names, in order, for the world
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The DES file names</returns>
        public virtual IEnumerable<string> GetDESNames(Context context) => new string[0];

        /// <summary>
        /// Gets the ETA file names, in order, for the world
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The ETA file names</returns>
        public virtual IEnumerable<string> GetETANames(Context context) => new string[0];

        #endregion

        #region Texture Methods

        /// <summary>
        /// Extracts all found .pcx from an xor encrypted file
        /// </summary>
        /// <param name="filePath">The path of the file to extract from</param>
        /// <param name="outputDir">The directory to output the files to</param>
        public void ExtractEncryptedPCX(string filePath, string outputDir)
        {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            // Read the file bytes
            var originalBytes = File.ReadAllBytes(filePath);

            // Enumerate every possible xor key
            for (int i = 0; i < 255; i++)
            {
                // Create a buffer
                var buffer = new byte[originalBytes.Length];

                // Decrypt the bytes to the buffer
                for (int j = 0; j < buffer.Length; j++)
                    buffer[j] = (byte)(originalBytes[j] ^ i);

                // Enumerate every byte
                for (int j = 0; j < buffer.Length - 100; j++)
                {
                    // Check if a valid PCX header is found
                    if (buffer[j + 0] != 0x0A || buffer[j + 1] != 0x05 || buffer[j + 2] != 0x01 ||
                        buffer[j + 3] != 0x08 || buffer[j + 4] != 0x00 || buffer[j + 5] != 0x00 ||
                        buffer[j + 6] != 0x00 || buffer[j + 7] != 0x00)
                        continue;

                    // Attempt to read the PCX file
                    try
                    {
                        // Serialize the data
                        using (var stream = new MemoryStream(buffer.Skip<byte>(j).ToArray<byte>())) {
                            using (Context c = new Context(Settings.GetGameSettings)) {
                                c.AddFile(new StreamFile("pcx", stream, c));
                                var pcx = FileFactory.Read<PCX>("pcx", c);

                                // Convert to a texture
                                var tex = pcx.ToTexture();

                                // Flip the texture
                                var flippedTex = new Texture2D(tex.width, tex.height);

                                for (int x = 0; x < tex.width; x++) {
                                    for (int y = 0; y < tex.height; y++) {
                                        flippedTex.SetPixel(x, tex.height - y - 1, tex.GetPixel(x, y));
                                    }
                                }

                                // Apply the pixels
                                flippedTex.Apply();

                                // Save the file
                                File.WriteAllBytes(Path.Combine(outputDir, $"{i} - {j}.png"), flippedTex.EncodeToPNG());

                                Debug.Log("Exported PCX");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to create PCX: {ex.Message}");
                    }
                }
            }

        }

        /// <summary>
        /// Exports all sprite textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="exportAnimFrames">Indicates if the textures should be exported as animation frames</param>
        public void ExportSpriteTextures(GameSettings settings, string outputDir, bool exportAnimFrames) 
        {
            // Create the context
            using (Context context = new Context(settings)) {
                // Add all files
                AddAllFiles(context);

                // Get the DES names for every world
                var desNames = EnumHelpers.GetValues<World>().ToDictionary<World, World, string[]>(x => x, world => {
                    // Set the world
                    context.Settings.World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        return null;

                    var a = GetDESNames(context).ToArray<string>();

                    return a.Any<string>() ? a : null;
                });

                // Helper method for exporting textures
                PC_WorldFile ExportTextures(string filePath, PC_WorldFile.Type type, string name, int desOffset, string[] desFileNames) {
                    // Read the file
                    var file = FileFactory.Read<PC_WorldFile>(filePath, context,
                        onPreSerialize: data => data.FileType = type);

                    // Export the sprite textures
                    if (exportAnimFrames)
                        ExportAnimationFrames(context, file, Path.Combine(outputDir, name), desOffset, desFileNames);
                    else
                        ExportSpriteTextures(context, file, Path.Combine(outputDir, name), desOffset, desFileNames);

                    return file;
                }

                // Export big ray
                ExportTextures(GetBigRayFilePath(context.Settings), PC_WorldFile.Type.BigRay, "Bigray", 0, null);

                // Export allfix
                var allfix = ExportTextures(GetAllfixFilePath(context.Settings), PC_WorldFile.Type.AllFix, "Allfix", 0, desNames.Values.FirstOrDefault<string[]>());

                // Enumerate every world
                foreach (World world in EnumHelpers.GetValues<World>()) {
                    // Set the world
                    context.Settings.World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        continue;

                    // Export big ray
                    ExportTextures(worldPath, PC_WorldFile.Type.World, world.ToString(), allfix.DesItemCount, desNames.TryGetValue(world, out var d) ? d : null);
                }
            }
        }

        /// <summary>
        /// Exports all sprite textures from the world file to the specified output directory
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="worldFile">The world file</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="desOffset">The amount of textures in the allfix to use as the DES offset if a world texture</param>
        /// <param name="desNames">The DES names, if available</param>
        /// <param name="palette">Optional palette to use</param>
        public void ExportSpriteTextures(Context context, PC_WorldFile worldFile, string outputDir, int desOffset, string[] desNames, IList<ARGBColor> palette = null) {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<PC_LevFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.Settings)) {
                // Set the level number
                context.Settings.Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.Settings);

                // Load the level
                levels.Add(FileFactory.Read<PC_LevFile>(lvlPath, context));
            }

            // Enumerate each sprite group
            for (int i = 0; i < worldFile.DesItems.Length; i++)
            {
                int index = -1;

                // Enumerate each image
                foreach (var tex in GetSpriteTextures(context.Settings, levels, worldFile.DesItems[i], worldFile, desOffset + 1 + i, palette))
                {
                    index++;

                    // Skip if null
                    if (tex == null)
                        continue;

                    // Get the DES name
                    var desName = desNames != null ? $" ({desNames[desOffset + i]})" : String.Empty;

                    // Write the texture
                    File.WriteAllBytes(Path.Combine(outputDir, $"{i.ToString()}{desName} - {index}.png"), tex.EncodeToPNG());
                }
            }
        }

        /// <summary>
        /// Gets all sprite textures for a DES item
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="levels">The levels in the world to check for the palette</param>
        /// <param name="desItem">The DES item</param>
        /// <param name="worldFile">The world data</param>
        /// <param name="desIndex">The DES index</param>
        /// <param name="palette">Optional palette to use</param>
        /// <returns>The sprite textures</returns>
        public Texture2D[] GetSpriteTextures(GameSettings settings, List<PC_LevFile> levels, PC_DES desItem, PC_WorldFile worldFile, int desIndex, IList<ARGBColor> palette = null)
        {
            // Create the output array
            var output = new Texture2D[desItem.ImageDescriptors.Length];

            bool foundForSpriteGroup = false;
            var defaultPalette = levels.First<PC_LevFile>();

            Beginning:

            // Process the image data
            var processedImageData = ProcessImageData(desItem.ImageData, desItem.RequiresBackgroundClearing);

            // Enumerate each image
            for (int i = 0; i < desItem.ImageDescriptors.Length; i++)
            {
                // Get the image descriptor
                var imgDescriptor = desItem.ImageDescriptors[i];

                // Ignore garbage sprites
                if (imgDescriptor.InnerHeight == 0 || imgDescriptor.InnerWidth == 0)
                    continue;

                // Default to the first level
                var lvl = defaultPalette;

                bool foundCorrectPalette = false;

                // Check all matching animation descriptor
                foreach (var animDesc in desItem.AnimationDescriptors.Where<PC_AnimationDescriptor>(x => x.Layers.Any<PC_AnimationLayer>(y => y.ImageIndex == i)).Select<PC_AnimationDescriptor, int>(x => desItem.AnimationDescriptors.FindItemIndex<PC_AnimationDescriptor>(y => y == x)))
                {
                    // Check all ETA's where it appears
                    foreach (var eta in worldFile.Eta.SelectMany<PC_ETA, PC_EventState[]>(x => x.States).SelectMany<PC_EventState[], PC_EventState>(x => x).Where<PC_EventState>(x => x.AnimationIndex == animDesc))
                    {
                        // TODO: Use new state indexing system
                        // Attempt to find the level where it appears
                        var lvlMatch = levels.FindLast(x => x.Events.Any<PC_Event>(y =>
                            y.DES == desIndex &&
                            y.Etat == eta.Etat &&
                            y.SubEtat == eta.SubEtat &&
                            y.ETA == worldFile.Eta.FindItemIndex<PC_ETA>(z => z.States.SelectMany<PC_EventState[], PC_EventState>(h => h).Contains<PC_EventState>(eta))));

                        if (lvlMatch != null)
                        {
                            lvl = lvlMatch;
                            foundCorrectPalette = true;

                            if (!foundForSpriteGroup)
                            {
                                foundForSpriteGroup = true;
                                defaultPalette = lvlMatch;
                                goto Beginning;
                            }

                            break;
                        }
                    }
                }

                // Check background DES
                if (!foundCorrectPalette)
                {
                    var lvlMatch = levels.FindLast(x => x.BackgroundSpritesDES == desIndex);

                    if (lvlMatch != null)
                        lvl = lvlMatch;
                }

                // Hard-code palette for certain DES groups
                if (settings.GameMode == GameMode.RayPC && settings.World == World.Music && desIndex == 16)
                    lvl = levels[11];
                else if (settings.GameMode == GameMode.RayPC && settings.World == World.Image && desIndex == 19)
                    lvl = levels[10];

                // Get the texture
                Texture2D tex = GetSpriteTexture(imgDescriptor, palette ?? lvl.ColorPalettes.First<RGB666Color[]>(), processedImageData);

                // Set the texture
                output[i] = tex;
            }

            // Return the output
            return output;
        }

        /// <summary>
        /// Exports the animation frames
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="worldFile">The world file to export from</param>
        /// <param name="outputDir">The directory to export to</param>
        /// <param name="desOffset">The amount of textures in the allfix to use as the DES offset if a world texture</param>
        /// <param name="desNames">The DES names, if available</param>
        /// <param name="palette">Optional palette to use</param>
        public void ExportAnimationFrames(Context context, PC_WorldFile worldFile, string outputDir, int desOffset, string[] desNames)
        {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<PC_LevFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.Settings))
            {
                // Set the level number
                context.Settings.Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.Settings);

                // Load the level
                levels.Add(FileFactory.Read<PC_LevFile>(lvlPath, context));
            }

            // Enumerate each sprite group
            for (int i = 0; i < worldFile.DesItems.Length; i++)
            {
                // Get the DES item
                var des = worldFile.DesItems[i];

                // Get the DES index
                var desIndex = desOffset + 1 + i;

                // Get the textures
                var textures = GetSpriteTextures(context.Settings, levels, des, worldFile, desIndex);

                // Get the DES name
                var desName = desNames != null ? $" ({desNames[desIndex - 1]})" : String.Empty;

                // Get the folder
                var desFolderPath = Path.Combine(outputDir, $"{i}{desName}");

                byte prevSpeed = 4;

                // Enumerate the animations
                for (var j = 0; j < des.AnimationDescriptors.Length; j++)
                {
                    // Get the animation descriptor
                    var anim = des.AnimationDescriptors[j];

                    byte? speed = null;

                    IEnumerable<PC_EventState> GetEtaMatches(int etaIndex) => worldFile.Eta[etaIndex].States.SelectMany<PC_EventState[], PC_EventState>(x => x).Where<PC_EventState>(x => x.AnimationIndex == j);

                    // TODO: Redo this to use new ETA indexing system
                    // Attempt to find a perfect match
                    for (int etaIndex = 0; etaIndex < worldFile.Eta.Length; etaIndex++)
                    {
                        if (speed != null)
                            break;

                        foreach (var etaMatch in GetEtaMatches(etaIndex))
                        {
                            // Check if it's a perfect match
                            if (levels.SelectMany<PC_LevFile, PC_Event>(x => x.Events).Any<PC_Event>(x => x.DES == desIndex &&
                                                                                                          x.ETA == etaIndex &&
                                                                                                          x.Etat == etaMatch.Etat &&
                                                                                                          x.SubEtat == etaMatch.SubEtat))
                            {
                                speed = etaMatch.AnimationSpeed;
                                break;
                            }
                        }
                    }

                    // If still null, find semi-perfect match
                    if (speed == null)
                    {
                        // Attempt to find a semi-perfect match
                        for (int etaIndex = 0; etaIndex < worldFile.Eta.Length; etaIndex++)
                        {
                            if (speed != null)
                                break;

                            foreach (var etaMatch in GetEtaMatches(etaIndex))
                            {
                                // Check if it's a semi-perfect match
                                if (levels.SelectMany<PC_LevFile, PC_Event>(x => x.Events).Any<PC_Event>(x => x.DES == desIndex && x.ETA == etaIndex))
                                {
                                    speed = etaMatch.AnimationSpeed;
                                    break;
                                }
                            }
                        }

                        // If still null, use previous eta
                        if (speed == null)
                            speed = prevSpeed;
                    }

                    // Set the previous eta
                    prevSpeed = speed.Value;

                    // Get the folder
                    var animFolderPath = Path.Combine(desFolderPath, $"{j}-{speed}");

                    // The layer index
                    var layer = 0;

                    var tempLayer = layer;

                    int? frameWidth = null;
                    int? frameHeight = null;

                    for (var dummy = 0; dummy < anim.LayersPerFrame * anim.FrameCount; dummy++)
                    {
                        var l = anim.Layers[tempLayer];

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

                        tempLayer++;
                    }

                    // Create each animation frame
                    for (int frameIndex = 0; frameIndex < anim.FrameCount; frameIndex++)
                    {
                        Texture2D tex = new Texture2D(frameWidth ?? 1, frameHeight ?? 1, TextureFormat.RGBA32, false)
                        {
                            filterMode = FilterMode.Point
                        };

                        // Default to fully transparent
                        for (int y = 0; y < tex.height; y++)
                        {
                            for (int x = 0; x < tex.width; x++)
                            {
                                tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                            }
                        }

                        bool hasLayers = false;

                        // Write each layer
                        for (var layerIndex = 0; layerIndex < anim.LayersPerFrame; layerIndex++)
                        {
                            var animationLayer = anim.Layers[layer];

                            layer++;

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

                                    var xPosition = (animationLayer.IsFlipped ? (sprite.width - 1 - x) : x) + animationLayer.XPosition;
                                    var yPosition = -(y + animationLayer.YPosition + 1);

                                    if (xPosition >= tex.width)
                                        throw new Exception("Horizontal overflow!");

                                    if (c.a != 0)
                                        tex.SetPixel(xPosition, yPosition, c);
                                }
                            }

                            hasLayers = true;
                        }

                        tex.Apply();

                        if (!hasLayers)
                            continue;

                        // Create the directory
                        Directory.CreateDirectory(animFolderPath);

                        // Save the file
                        File.WriteAllBytes(Path.Combine(animFolderPath, $"{frameIndex}.png"), tex.EncodeToPNG());
                    }
                }
            }
        }

        /// <summary>
        /// Processes the image data
        /// </summary>
        /// <param name="imageData">The image data to process</param>
        /// <param name="requiresBackgroundClearing">Indicates if the data requires background clearing</param>
        /// <returns>The processed image data</returns>
        public byte[] ProcessImageData(byte[] imageData, bool requiresBackgroundClearing)
        {
            // Create the output array
            var processedData = new byte[imageData.Length];

            int flag = -1;

            for (int i = imageData.Length - 1; i >= 0; i--)
            {
                // Decrypt the byte
                processedData[i] = (byte)(imageData[i] ^ 143);

                // Continue to next if we don't need to do background clearing
                if (!requiresBackgroundClearing)
                    continue;

                int num6 = (flag < 255) ? (flag + 1) : 255;

                if (processedData[i] == 161 || processedData[i] == 250)
                {
                    flag = processedData[i];
                    processedData[i] = 0;
                }
                else if (flag != -1)
                {
                    if (processedData[i] == num6)
                    {
                        processedData[i] = 0;
                        flag = num6;
                    }
                    else
                    {
                        flag = -1;
                    }
                }
            }

            return processedData;
        }

        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="s">The image descriptor</param>
        /// <param name="palette">The palette to use</param>
        /// <param name="processedImageData">The processed image data to use</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(PC_ImageDescriptor s, IList<ARGBColor> palette, byte[] processedImageData)
        {
            // Get the image properties
            var width = s.OuterWidth;
            var height = s.OuterHeight;
            var offset = s.ImageOffset;

            // Create the texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };

            // Default to fully transparent
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            try
            {
                // Set every pixel
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Get the pixel offset
                        var pixelOffset = y * width + x + offset;

                        // Get the palette index
                        var pixel = processedImageData[pixelOffset];

                        // Ignore if 0
                        if (pixel == 0)
                            continue;

                        // Get the color from the palette
                        var color = palette[pixel];

                        // Set the pixel
                        tex.SetPixel(x, -(y + 1), color.GetColor());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Couldn't load sprite for DES: {ex.Message}");

                return null;
            }

            // Apply the changes
            tex.Apply();

            // Return the texture
            return tex;
        }

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
                new GameAction("Export Sprites", false, true),
                new GameAction("Export Animation Frames", false, true),
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
                    ExportSpriteTextures(settings, outputDir, false);
                    break;

                case 1:
                    ExportSpriteTextures(settings, outputDir, true);
                    break;

                case 2:
                    ExtractEncryptedPCX(settings.GameDirectory + GetVignetteFilePath(settings), outputDir);
                    break;
            }
        }

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level)
        {
            // Get the palette changers
            var paletteXChangers = level.Events.Where<Common_Event>(x => x.Type == 158 && x.SubEtat < 6).ToDictionary<Common_Event, uint, PC_PaletteChangerMode>(x => x.XPosition, x => (PC_PaletteChangerMode)x.SubEtat);
            var paletteYChangers = level.Events.Where<Common_Event>(x => x.Type == 158 && x.SubEtat >= 6).ToDictionary<Common_Event, uint, PC_PaletteChangerMode>(x => x.YPosition, x => (PC_PaletteChangerMode)x.SubEtat);

            // TODO: The auto system won't always work since it just checks one type of palette swapper and doesn't take into account that the palette swappers only trigger when on-screen, rather than based on the axis. Because of this some levels, like Music 5, won't work. More are messed up in the EDU games. There is sadly no solution to this since it depends on the players movement.
            // Check which type of palette changer we have
            bool isPaletteHorizontal = paletteXChangers.Any<KeyValuePair<uint, PC_PaletteChangerMode>>();

            // Keep track of the default palette
            int defaultPalette = 1;

            // Get the default palette
            if (isPaletteHorizontal && paletteXChangers.Any<KeyValuePair<uint, PC_PaletteChangerMode>>())
            {
                switch (paletteXChangers.OrderBy<KeyValuePair<uint, PC_PaletteChangerMode>, uint>(x => x.Key).First<KeyValuePair<uint, PC_PaletteChangerMode>>().Value)
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
            else if (!isPaletteHorizontal && paletteYChangers.Any<KeyValuePair<uint, PC_PaletteChangerMode>>())
            {
                switch (paletteYChangers.OrderByDescending<KeyValuePair<uint, PC_PaletteChangerMode>, uint>(x => x.Key).First<KeyValuePair<uint, PC_PaletteChangerMode>>().Value)
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
            for (int cellY = 0; cellY < level.Height; cellY++)
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

                for (int cellX = 0; cellX < level.Width; cellX++)
                {
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

                    // Set the common tile
                    level.Tiles[cellY * level.Width + cellX].PaletteIndex = currentPalette;
                }
            }
        }

        /// <summary>
        /// Loads the sprites for the level
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="palette">The palette to use</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        public async Task LoadSpritesAsync(Context context, IList<ARGBColor> palette, List<Common_Design> eventDesigns)
        {
            Controller.status = $"Loading allfix";

            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(GetAllfixFilePath(context.Settings), context,
                onPreSerialize: data => data.FileType = PC_WorldFile.Type.AllFix);

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading world";

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context,
                onPreSerialize: data => data.FileType = PC_WorldFile.Type.World);

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading big ray";

            // NOTE: This is not loaded into normal levels and is purely loaded here so the animation can be viewed!
            // Read the big ray data
            var bigRayData = FileFactory.Read<PC_WorldFile>(GetBigRayFilePath(context.Settings), context,
                onPreSerialize: data => data.FileType = PC_WorldFile.Type.BigRay);

            await Controller.WaitIfNecessary();

            // Get the DES and ETA
            var des = allfix.DesItems.Concat<PC_DES>(worldData.DesItems).Concat<PC_DES>(bigRayData.DesItems).ToArray<PC_DES>();

            int desIndex = 0;

            // Read every DES item
            foreach (var d in des)
            {
                Controller.status = $"Loading DES {desIndex}/{des.Length}";

                await Controller.WaitIfNecessary();

                Common_Design finalDesign = new Common_Design
                {
                    Sprites = new List<Sprite>(),
                    Animations = new List<Common_Animation>()
                };

                // Process the image data
                var processedImageData = ProcessImageData(d.ImageData, d.RequiresBackgroundClearing);

                // Sprites
                foreach (var s in d.ImageDescriptors)
                {
                    // Ignore garbage sprites
                    var isGarbage = s.InnerHeight == 0 || s.InnerWidth == 0;

                    // Get the texture
                    Texture2D tex = isGarbage ? null : GetSpriteTexture(s, palette, processedImageData);

                    // Add it to the array
                    finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                }

                // Animations
                foreach (var a in d.AnimationDescriptors)
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
                eventDesigns.Add(finalDesign);
                desIndex++;
            }
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public virtual async Task<Common_Lev> LoadLevelAsync(Context context, List<Common_Design> eventDesigns) {
            Controller.status = $"Loading map data for {context.Settings.World} {context.Settings.Level}";

            // Read the level data
            var levelData = FileFactory.Read<PC_LevFile>(GetLevelFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev {
                // Set the dimensions
                Width = levelData.Width,
                Height = levelData.Height,

                // Create the events list
                Events = new List<Common_Event>(),

                // Create the tile arrays
                TileSet = new Common_Tileset[3],
                Tiles = new Common_Tile[levelData.Width * levelData.Height],
            };

            // Load the sprites
            await LoadSpritesAsync(context, levelData.ColorPalettes.First<RGB666Color[]>(), eventDesigns);

            // Add the events
            commonLev.Events = new List<Common_Event>();

            var index = 0;

            foreach (PC_Event e in levelData.Events) {
                Controller.status = $"Loading event {index}/{levelData.EventCount}";

                await Controller.WaitIfNecessary();

                // Instantiate event prefab using LevelEventController
                var ee = Controller.obj.levelEventController.AddEvent((int)e.Type, e.Etat, e.SubEtat, e.XPosition, e.YPosition, (int)e.DES, (int)e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, levelData.EventCommands[index].LabelOffsetTable, levelData.EventCommands[index].Commands, levelData.EventLinkingTable[index]);

                // Add the event
                commonLev.Events.Add(ee);

                index++;
            }

            Controller.status = $"Loading tile set";

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            commonLev.TileSet[0] = tileSets[0];
            commonLev.TileSet[1] = tileSets[1];
            commonLev.TileSet[2] = tileSets[2];

            // Enumerate each cell
            for (int cellY = 0; cellY < levelData.Height; cellY++) 
            {
                for (int cellX = 0; cellX < levelData.Width; cellX++) 
                {
                    // Get the cell
                    var cell = levelData.Tiles[cellY * levelData.Width + cellX];

                    // Get the texture index, default to 0 for fully transparent (no texture)
                    var textureIndex = 0;

                    // Ignore if fully transparent
                    if (cell.TransparencyMode != PC_MapTileTransparencyMode.FullyTransparent) {
                        // Get the offset for the texture
                        var texOffset = levelData.TexturesOffsetTable[cell.TextureIndex];

                        // Get the texture
                        var texture = cell.TransparencyMode == PC_MapTileTransparencyMode.NoTransparency ? levelData.NonTransparentTextures.FindItem<PC_TileTexture>(x => x.TextureOffset == texOffset) : levelData.TransparentTextures.FindItem<PC_TransparentTileTexture>(x => x.TextureOffset == texOffset);

                        // Get the index
                        textureIndex = levelData.NonTransparentTextures.Concat<PC_TileTexture>(levelData.TransparentTextures).FindItemIndex<PC_TileTexture>(x => x == texture);
                    }

                    // Set the common tile
                    commonLev.Tiles[cellY * levelData.Width + cellX] = new Common_Tile() 
                    {
                        TileSetGraphicIndex = textureIndex,
                        CollisionType = cell.CollisionType,
                        PaletteIndex = 1,
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
        public Common_Tileset[] ReadTileSets(PC_LevFile levData) {
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
            foreach (var texture in levData.NonTransparentTextures.Concat<PC_TileTexture>(levData.TransparentTextures)) {
                // Enumerate every palette
                for (int i = 0; i < levData.ColorPalettes.Length; i++) {
                    // Create the texture to use for the tile
                    var tileTexture = new Texture2D(CellSize, CellSize, TextureFormat.RGBA32, false) {
                        filterMode = FilterMode.Point
                    };

                    // Write each pixel to the texture
                    for (int y = 0; y < CellSize; y++) {
                        for (int x = 0; x < CellSize; x++) {
                            // Get the index
                            var cellIndex = CellSize * y + x;

                            // Get the color from the current palette (or default to fully transparent if it's the first tile)
                            var c = index == 0 ? new Color(0, 0, 0, 0) : levData.ColorPalettes[i][255 - texture.ColorIndexes[cellIndex]].GetColor();

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
        /// <param name="context">The serialization context</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(Context context, Common_Lev commonLevelData) {
            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<PC_LevFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.Height; y++) {
                for (int x = 0; x < lvlData.Width; x++) {
                    // Get the tiles
                    var tile = lvlData.Tiles[y * lvlData.Width + x];
                    var commonTile = commonLevelData.Tiles[y * lvlData.Width + x];

                    // Update the tile
                    tile.CollisionType = commonTile.CollisionType;

                    if (commonTile.TileSetGraphicIndex == 0) {
                        tile.TextureIndex = 0;
                        tile.TransparencyMode = PC_MapTileTransparencyMode.FullyTransparent;
                    }
                    else if (commonTile.TileSetGraphicIndex < lvlData.NonTransparentTexturesCount) {
                        tile.TextureIndex = (ushort)lvlData.TexturesOffsetTable.FindItemIndex<uint>(z => z == lvlData.NonTransparentTextures[commonTile.TileSetGraphicIndex].TextureOffset);
                        tile.TransparencyMode = PC_MapTileTransparencyMode.NoTransparency;
                    }
                    else {
                        tile.TextureIndex = (ushort)lvlData.TexturesOffsetTable.FindItemIndex<uint>(z => z == lvlData.TransparentTextures[(commonTile.TileSetGraphicIndex - lvlData.NonTransparentTexturesCount)].TextureOffset);
                        tile.TransparencyMode = PC_MapTileTransparencyMode.PartiallyTransparent;
                    }
                }
            }

            // Temporary event lists
            var events = new List<PC_Event>();
            var eventCommands = new List<PC_EventCommand>();
            var eventLinkingTable = new List<ushort>();

            // Set events
            Controller.obj.levelEventController.CalculateLinkIndexes();

            foreach (var e in commonLevelData.Events) 
            {
                // Create the event
                var r1Event = new PC_Event
                {
                    DES = (uint)e.DES,
                    DES2 = (uint)e.DES,
                    DES3 = (uint)e.DES,
                    ETA = (uint)e.ETA,
                    Unk1 = new uint[6],
                    Unk3 = 0,
                    Unk4 = new ushort[22],
                    Unk5 = new byte[5],
                    Unk6 = 0,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    Type = (ushort)e.Type,
                    OffsetBX = (byte)e.OffsetBX,
                    OffsetBY = (byte)e.OffsetBY,
                    Unk7 = 0,
                    SubEtat = (byte)e.SubEtat,
                    Etat = (byte)e.Etat,
                    Unk8 = 0,
                    Unk9 = 0,
                    OffsetHY = (byte)e.OffsetHY,
                    FollowSprite = (byte)e.FollowSprite,
                    HitPoints = (byte)e.HitPoints,
                    UnkGroup = 0,
                    HitSprite = (byte)e.HitSprite,
                    Unk10 = new byte[6],
                    Unk11 = 0,
                    FollowEnabled = e.FollowEnabled,
                    Unk12 = 0
                };

                // Add the event
                events.Add(r1Event);

                // Add the event commands
                eventCommands.Add(new PC_EventCommand()
                {
                    CommandLength = (ushort)(e.CommandCollection.Commands.Select(x => x.Length).Sum()),
                    Commands = e.CommandCollection,
                    LabelOffsetCount = (ushort)e.LabelOffsets.Length,
                    LabelOffsetTable = e.LabelOffsets
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
            FileFactory.Write<PC_LevFile>(lvlPath, context);
        }

        public virtual async Task LoadFilesAsync(Context context) 
        {
            Dictionary<string, string> paths = new Dictionary<string, string>
            {
                ["allfix"] = GetAllfixFilePath(context.Settings),
                ["world"] = GetWorldFilePath(context.Settings),
                ["level"] = GetLevelFilePath(context.Settings),
                ["bigray"] = GetBigRayFilePath(context.Settings)
            };

            foreach (string pathKey in paths.Keys) {
                await FileSystem.PrepareFile(context.BasePath + paths[pathKey]);

                context.AddFile(GetFile(context, paths[pathKey]));
            }
        }

        /// <summary>
        /// Adds all files to the context, to be used for export operations
        /// </summary>
        /// <param name="context">The context to add to</param>
        public virtual void AddAllFiles(Context context)
        {
            // Add big ray file
            context.AddFile(GetFile(context, GetBigRayFilePath(context.Settings)));
            
            // Add allfix file
            context.AddFile(GetFile(context, GetAllfixFilePath(context.Settings)));

            // Add for every world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Set the world
                context.Settings.World = world;

                // Add world file
                context.AddFile(GetFile(context, GetWorldFilePath(context.Settings)));

                // Add every level
                foreach (var lvl in GetLevels(context.Settings))
                {
                    // Set the level
                    context.Settings.Level = lvl;

                    // Add level file
                    context.AddFile(GetFile(context, GetLevelFilePath(context.Settings)));
                }
            }
        }

        /// <summary>
        /// Gets a binary file to add to the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filePath">The file path</param>
        /// <returns>The binary file</returns>
        protected virtual BinaryFile GetFile(Context context, string filePath) => new LinearSerializedFile(context)
        {
            filePath = filePath
        };

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public virtual Common_EditorEventInfo GetEditorEventInfo(GameSettings settings, Common_Event e)
        {
            // TODO: Convert these to command bytes!
            var cmds = new byte[0];

            // Find match
            var match = GetPCEventInfo(settings.GameModeSelection, settings.World, e.Type, e.Etat, e.SubEtat, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, cmds);

            // Return the editor info
            return new Common_EditorEventInfo(match?.Name, match?.Flag);
        }

        /// <summary>
        /// Gets the animation info for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <returns>The animation info</returns>
        public Common_AnimationInfo GetAnimationInfo(Context context, Common_Event e)
        {
            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(GetAllfixFilePath(context.Settings), context);

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Read the big ray data
            var bigRayData = FileFactory.Read<PC_WorldFile>(GetBigRayFilePath(context.Settings), context);

            // Get the eta items
            var eta = allfix.Eta.Concat<PC_ETA>(worldData.Eta).Concat<PC_ETA>(bigRayData.Eta);

            // Get animation index from the matching ETA item
            //var etaItem = eta.ElementAt(e.ETA).SelectMany(x => x).FindItem(x => x.Etat == e.Etat && x.SubEtat == e.SubEtat);
            var etaItem = eta.ElementAt<PC_ETA>(e.ETA).States.ElementAtOrDefault<PC_EventState[]>(e.Etat)?.ElementAtOrDefault<PC_EventState>(e.SubEtat);

            // Return the index
            return new Common_AnimationInfo(etaItem?.AnimationIndex ?? -1, etaItem?.AnimationSpeed ?? -1);
        }

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The names of the available events to add</returns>
        public virtual string[] GetEvents(GameSettings settings)
        {
            var w = settings.World.ToEventWorld();

            return LoadPCEventInfo(settings.GameModeSelection)?.Where<GeneralPCEventInfoData>(x => x.World == EventWorld.All || x.World == w).Select<GeneralPCEventInfoData, string>(x => x.Name).ToArray<string>() ?? new string[0];
        }

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventController">The event controller to add to</param>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns></returns>
        public virtual Common_Event AddEvent(GameSettings settings, LevelEventController eventController, int index, uint xPos, uint yPos)
        {
            var w = settings.World.ToEventWorld();

            // Get the event
            var e = LoadPCEventInfo(settings.GameModeSelection).Where<GeneralPCEventInfoData>(x => x.World == EventWorld.All || x.World == w).ElementAt<GeneralPCEventInfoData>(index);

            // TODO: Before Designer is merged we need to find the "used" commands
            var cmds = Common_EventCommandCollection.FromBytes(e.Commands.Any<byte>() ? e.Commands : e.LocalCommands);

            // Add and return the event
            return eventController.AddEvent(e.Type, e.Etat, e.SubEtat, xPos, yPos, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, cmds, 0);
        }

        /// <summary>
        /// Loads the PC event info
        /// </summary>
        /// <param name="mode">The game mode to get the info for</param>
        /// <returns>The loaded event info</returns>
        public GeneralPCEventInfoData[] LoadPCEventInfo(GameModeSelection mode)
        {
            // Get the file name
            string fileName;

            switch (mode)
            {
                case GameModeSelection.RaymanPC:
                    fileName = "RayPC.csv";
                    break;

                case GameModeSelection.RaymanDesignerPC:
                case GameModeSelection.MapperPC:
                    fileName = "RayKit.csv";
                    break;

                default:
                    fileName = null;
                    break;
            }

            // Return empty collection if no file was found
            if (fileName == null)
                return new GeneralPCEventInfoData[0];

            // Load the file if not already loaded
            if (!EventCache.ContainsKey(fileName))
                EventCache.Add(fileName, LoadPCEventInfo(fileName));

            // Return the loaded datas
            return EventCache[fileName];
        }

        /// <summary>
        /// Loads the PC event info from the specified file
        /// </summary>
        /// <param name="filePath">The file to load from</param>
        /// <returns>The loaded info data</returns>
        private static GeneralPCEventInfoData[] LoadPCEventInfo(string filePath)
        {
            // Open the file
            using (var fileStream = File.OpenRead(filePath))
            {
                // Use a reader
                using (var reader = new StreamReader(fileStream))
                {
                    // Create the output
                    var output = new List<GeneralPCEventInfoData>();

                    // Skip header
                    reader.ReadLine();

                    // Read every line
                    while (!reader.EndOfStream)
                    {
                        // Read the line
                        var line = reader.ReadLine()?.Split(',');

                        // Make sure we read something
                        if (line == null)
                            break;

                        // Keep track of the value index
                        var index = 0;

                        try
                        {
                            // Helper methods for parsing values
                            string nextValue() => line[index++];
                            bool nextBoolValue() => Boolean.Parse(line[index++]);
                            int nextIntValue() => Int32.Parse(nextValue());
                            T? nextEnumValue<T>() where T : struct => Enum.TryParse<T>(nextValue(), out T parsedEnum) ? (T?)parsedEnum : null;
                            ushort[] next16ArrayValue() => nextValue().Split('_').Where<string>(x => !String.IsNullOrWhiteSpace(x)).Select<string, ushort>(UInt16.Parse).ToArray<ushort>();
                            byte[] next8ArrayValue() => nextValue().Split('_').Where<string>(x => !String.IsNullOrWhiteSpace(x)).Select<string, byte>(Byte.Parse).ToArray<byte>();
                            string[] nextStringArrayValue() => nextValue().Split('/').Where<string>(x => !String.IsNullOrWhiteSpace(x)).ToArray<string>();

                            // Add the item to the output
                            output.Add(new GeneralPCEventInfoData(nextValue(), nextValue(), nextEnumValue<EventWorld>(), nextIntValue(), nextIntValue(), nextIntValue(), nextEnumValue<EventFlag>(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextBoolValue(), nextStringArrayValue(), next16ArrayValue(), next8ArrayValue(), next8ArrayValue()));
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to parse event info. Index: {index}, items: {String.Join(" - ", line)} , exception: {ex.Message}");
                            throw;
                        }
                    }

                    // Return the output
                    return output.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the event info data which matches the specified values for a PC event
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="world"></param>
        /// <param name="type"></param>
        /// <param name="etat"></param>
        /// <param name="subEtat"></param>
        /// <param name="des"></param>
        /// <param name="eta"></param>
        /// <param name="offsetBx"></param>
        /// <param name="offsetBy"></param>
        /// <param name="offsetHy"></param>
        /// <param name="followSprite"></param>
        /// <param name="hitPoints"></param>
        /// <param name="hitSprite"></param>
        /// <param name="followEnabled"></param>
        /// <param name="labelOffsets"></param>
        /// <param name="commands"></param>
        /// <returns>The item which matches the values</returns>
        public GeneralPCEventInfoData GetPCEventInfo(GameModeSelection mode, World world, int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands)
        {
            // Load the event info
            var allInfo = LoadPCEventInfo(mode);

            EventWorld eventWorld = world.ToEventWorld();

            // Find a matching item
            var match = allInfo.FindItem<GeneralPCEventInfoData>(x => (x.World == eventWorld || x.World == EventWorld.All) &&
                                                                      x.Type == type &&
                                                                      x.Etat == etat &&
                                                                      x.SubEtat == subEtat &&
                                                                      x.DES == des &&
                                                                      x.ETA == eta &&
                                                                      x.OffsetBX == offsetBx &&
                                                                      x.OffsetBY == offsetBy &&
                                                                      x.OffsetHY == offsetHy &&
                                                                      x.FollowSprite == followSprite &&
                                                                      x.HitPoints == hitPoints &&
                                                                      x.HitSprite == hitSprite &&
                                                                      x.FollowEnabled == followEnabled &&
                                                                      x.LabelOffsets.SequenceEqual<ushort>(labelOffsets) &&
                                                                      x.Commands.SequenceEqual<byte>(commands));

            // Create dummy item if not found
            if (match == null && allInfo.Any<GeneralPCEventInfoData>())
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat}");

            // Return the item
            return match;
        }

        #endregion
    }
}