using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asyncoroutine;
using R1Engine.Serialize;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PC
    /// </summary>
    public abstract class PC_Manager : IGameManager {
        #region Values and paths

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
        /// Gets the file path for the primary sound file
        /// </summary>
        /// <returns>The primary sound file path</returns>
        public virtual string GetSoundFilePath() => $"SNDD8B.DAT";

        /// <summary>
        /// Gets the file path for the primary sound manifest file
        /// </summary>
        /// <returns>The primary sound manifest file path</returns>
        public virtual string GetSoundManifestFilePath() => $"SNDH8B.DAT";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public abstract string GetWorldFilePath(GameSettings settings);

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

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        /// <param name="settings">The game settings</param>
        public abstract ArchiveFile[] GetArchiveFiles(GameSettings settings);

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public abstract AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings);

        #endregion

        #region Texture Methods

        /// <summary>
        /// Extracts a the vignette files
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <param name="filePath">The vignette file path</param>
        /// <param name="outputDir">The output directory</param>
        public void ExtractVignette(GameSettings settings, string filePath, string outputDir)
        {
            var archiveVig = GetArchiveFiles(settings).FindItem(x => x.FilePath == filePath);

            if (archiveVig == null)
            {
                ExtractEncryptedPCX(settings.GameDirectory + filePath, outputDir);
                return;
            }

            // Create a new context
            using (var context = new Context(Settings.GetGameSettings))
            {
                // Read the archive
                var archive = ExtractArchive(context, archiveVig);

                var index = 0;

                // Extract every .pcx file
                foreach (var file in archive)
                {
                    // Create the key
                    var key = $"PCX{index}";

                    // Use a memory stream
                    using (var stream = new MemoryStream(file.Data))
                    {
                        // Add to context
                        context.AddFile(new StreamFile(key, stream, context));

                        // Serialize the data
                        var pcx = FileFactory.Read<PCX>(key, context);

                        // Convert to a texture
                        var tex = pcx.ToTexture();

                        // Flip the texture
                        var flippedTex = new Texture2D(tex.width, tex.height);

                        for (int x = 0; x < tex.width; x++)
                        {
                            for (int y = 0; y < tex.height; y++)
                            {
                                flippedTex.SetPixel(x, tex.height - y - 1, tex.GetPixel(x, y));
                            }
                        }

                        // Apply the pixels
                        flippedTex.Apply();

                        // Write the bytes
                        File.WriteAllBytes(Path.Combine(outputDir, $"{index}. {file.FileName}.png"), flippedTex.EncodeToPNG());
                    }

                    index++;
                }
            }
        }

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

            var foundPCX = new Dictionary<string, byte[]>();

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
                        using (var stream = new MemoryStream(buffer.Skip(j).ToArray())) {
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

                                // Add the file
                                foundPCX.Add($"{i}-{j}", flippedTex.EncodeToPNG());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to create PCX: {ex.Message}");
                    }
                }
            }

            var index = 0;

            // Save all the files
            foreach (var pcx in foundPCX.Select(x => new
            {
                XORKey = x.Key.Split('-')[0],
                FileOffset = x.Key.Split('-')[1],
                Data = x.Value
            }).OrderBy(x => x.FileOffset))
            {
                File.WriteAllBytes(Path.Combine(outputDir, $"{index}. [{pcx.XORKey}] ({pcx.FileOffset}).png"), pcx.Data);

                index++;
            }
        }

        /// <summary>
        /// Exports all sprite textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="exportAnimFrames">Indicates if the textures should be exported as animation frames</param>
        public async Task ExportSpriteTexturesAsync(GameSettings settings, string outputDir, bool exportAnimFrames) 
        {
            // Create the context
            using (Context context = new Context(settings)) {
                // Add all files
                AddAllFiles(context);

                // Get the event info
                IList<GeneralEventInfoData> eventInfo;

                // TODO: Generalize this with helper method
                // Load the event info data
                using (var csvFile = File.OpenRead("Events.csv"))
                    eventInfo = GeneralEventInfoData.ReadCSV(csvFile);

                // Get the DES names for every world
                var desNames = EnumHelpers.GetValues<World>().ToDictionary(x => x, world => {
                    // Set the world
                    context.Settings.World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        return null;

                    // TODO: Update this to not include extensions
                    var a = FileFactory.Read<PC_WorldFile>(worldPath, context, (s, o) => o.FileType = PC_WorldFile.Type.World).DESFileNames?.Skip(1).ToArray();

                    return a?.Any() == true ? a : null;
                });

                // Get the ETA names for every world
                var etaNames = EnumHelpers.GetValues<World>().ToDictionary(x => x, world => {
                    // Set the world
                    context.Settings.World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        return null;

                    var a = FileFactory.Read<PC_WorldFile>(worldPath, context, (s, o) => o.FileType = PC_WorldFile.Type.World).ETAFileNames?.ToArray();

                    return a?.Any() == true ? a : null;
                });

                // Keep track of Rayman's anim
                PC_AnimationDescriptor[] rayAnim = null;

                // Helper method for exporting textures
                async Task<PC_WorldFile> ExportTexturesAsync(string filePath, PC_WorldFile.Type type, string name, int desOffset, IEnumerable<PC_ETA> baseEta, string[] desFileNames, string[] etaFileNames, IList<ARGBColor> palette = null) {
                    // Read the file
                    var file = FileFactory.Read<PC_WorldFile>(filePath, context, onPreSerialize: (s, data) => data.FileType = type);

                    if (rayAnim == null && type == PC_WorldFile.Type.AllFix)
                        // Rayman is always the first DES
                        rayAnim = file.DesItems.First().AnimationDescriptors;

                    // Export the sprite textures
                    if (exportAnimFrames)
                        await ExportAnimationFramesAsync(context, file, Path.Combine(outputDir, name), desOffset, baseEta.Concat(file.Eta).ToArray(), desFileNames, etaFileNames, eventInfo, rayAnim, palette);
                    else
                        ExportSpriteTextures(context, file, Path.Combine(outputDir, name), desOffset, desFileNames, palette);

                    return file;
                }

                // Export big ray
                await ExportTexturesAsync(GetBigRayFilePath(context.Settings), PC_WorldFile.Type.BigRay, "Bigray", 0, new PC_ETA[0], null, null, GetBigRayPalette(context));

                // Export allfix
                var allfix = await ExportTexturesAsync(GetAllfixFilePath(context.Settings), PC_WorldFile.Type.AllFix, "Allfix", 0, new PC_ETA[0], desNames.Values.FirstOrDefault(), etaNames.Values.FirstOrDefault());

                // Enumerate every world
                foreach (World world in EnumHelpers.GetValues<World>()) {
                    // Set the world
                    context.Settings.World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        continue;

                    // Export world
                    await ExportTexturesAsync(worldPath, PC_WorldFile.Type.World, world.ToString(), allfix.DesItems.Length, allfix.Eta, desNames.TryGetItem(world), etaNames.TryGetItem(world));
                }
            }
        }

        /// <summary>
        /// Gets the big ray palette if available
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The big ray palette</returns>
        public IList<ARGBColor> GetBigRayPalette(Context context)
        {
            // Attempt to get the vignette archive file
            var vig = GetArchiveFiles(context.Settings).FindItem(x => x.FilePath == GetVignetteFilePath(context.Settings));

            if (vig == null) 
                return null;

            // Extract the archive
            var vigData = ExtractArchive(context, vig);

            // Get the splash screen vignette
            var splashVig = vigData.FindItem(x => context.Settings.EngineVersion == EngineVersion.RayEduPS1 || context.Settings.EngineVersion == EngineVersion.RayEduPC ? x.FileName.StartsWith("FND04") : x.FileName.StartsWith("FND0"));

            if (splashVig == null) 
                return null;

            // Create the key
            const string key = "PCX";

            // Use a memory stream
            using (var stream = new MemoryStream(splashVig.Data))
            {
                // Add to context
                context.AddFile(new StreamFile(key, stream, context));

                // Serialize the data
                var pcx = FileFactory.Read<PCX>(key, context);

                // Get the palette
                var bigRayPalette = new List<ARGBColor>();

                for (int i = 0; i < pcx.VGAPalette.Length; i += 3)
                {
                    bigRayPalette.Add(new ARGBColor(
                        red: pcx.VGAPalette[i + 0],
                        green: pcx.VGAPalette[i + 1],
                        blue: pcx.VGAPalette[i + 2]));
                }

                return bigRayPalette;
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
            foreach (var i in GetLevels(context.Settings).FindItem(x => x.Key == context.Settings.World).Value.OrderBy(x => x)) {
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
                foreach (var tex in GetSpriteTextures(levels, worldFile.DesItems[i], desOffset + 1 + i, palette))
                {
                    index++;

                    // Skip if null
                    if (tex == null)
                        continue;

                    // Get the DES name
                    var desName = desNames != null ? $" ({desNames[desOffset + i]})" : String.Empty;

                    // Write the texture
                    File.WriteAllBytes(Path.Combine(outputDir, $"{i}{desName} - {index}.png"), tex.EncodeToPNG());
                }
            }
        }

        /// <summary>
        /// Gets all sprite textures for a DES item
        /// </summary>
        /// <param name="levels">The levels in the world to check for the palette</param>
        /// <param name="desItem">The DES item</param>
        /// <param name="desIndex">The DES index</param>
        /// <param name="palette">Optional palette to use</param>
        /// <returns>The sprite textures</returns>
        public Texture2D[] GetSpriteTextures(List<PC_LevFile> levels, PC_DES desItem, int desIndex, IList<ARGBColor> palette = null)
        {
            // Create the output array
            var output = new Texture2D[desItem.ImageDescriptors.Length];

            // Process the image data
            var processedImageData = ProcessImageData(desItem.ImageData, desItem.RequiresBackgroundClearing);

            // Find the level with the correct palette
            var lvl = levels.FindLast(x => x.BackgroundSpritesDES == desIndex || x.EventData.Events.Any(y => y.PC_ImageDescriptorsIndex == desIndex)) ?? levels.First();

            // Enumerate each image
            for (int i = 0; i < desItem.ImageDescriptors.Length; i++)
            {
                // Get the image descriptor
                var imgDescriptor = desItem.ImageDescriptors[i];

                // Ignore dummy sprites
                if (imgDescriptor.Index == 0)
                    continue;

                // Get the texture
                Texture2D tex = GetSpriteTexture(imgDescriptor, palette ?? lvl.MapData.ColorPalettes.First(), processedImageData);

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
        /// <param name="eta">The available ETA</param>
        /// <param name="desNames">The DES names, if available</param>
        /// <param name="etaNames">The ETA names, if available</param>
        /// <param name="eventInfo">The event info</param>
        /// <param name="rayAnim">Rayman's animation</param>
        /// <param name="palette">Optional palette to use</param>
        public async Task ExportAnimationFramesAsync(Context context, PC_WorldFile worldFile, string outputDir, int desOffset, PC_ETA[] eta, string[] desNames, string[] etaNames, IList<GeneralEventInfoData> eventInfo, PC_AnimationDescriptor[] rayAnim, IList<ARGBColor> palette = null)
        {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<PC_LevFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.Settings).FindItem(x => x.Key == context.Settings.World).Value.OrderBy(x => x))
            {
                // Set the level number
                context.Settings.Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.Settings);

                // Load the level
                levels.Add(FileFactory.Read<PC_LevFile>(lvlPath, context));
            }

            // Get special DES
            int? smallRayDES = null;
            int? darkRayDES = null;

            // Get the small Rayman DES if allfix
            if (worldFile.FileType == PC_WorldFile.Type.AllFix)
            {
                var ei = eventInfo.FindItem(x => x.Type == (int)EventType.TYPE_DEMI_RAYMAN);

                if (context.Settings.EngineVersion == EngineVersion.RayPC)
                    smallRayDES = ei.DesR1[World.Jungle];
                else if (context.Settings.EngineVersion == EngineVersion.RayKitPC)
                    smallRayDES = desNames.FindItemIndex(x => ei.DesKit[World.Jungle] == x.Substring(0, x.Length - 4)) + 1;
                else
                    throw new NotImplementedException();
            }

            // Get the Dark Rayman DES if Cake
            if (worldFile.FileType == PC_WorldFile.Type.World && context.Settings.World == World.Cake)
            {
                var ei = eventInfo.FindItem(x => x.Type == (int)EventType.TYPE_BLACK_RAY);

                if (context.Settings.EngineVersion == EngineVersion.RayPC)
                    darkRayDES = ei.DesR1[World.Cake];
                else if (context.Settings.EngineVersion == EngineVersion.RayKitPC)
                    darkRayDES = desNames.FindItemIndex(x => ei.DesKit[World.Cake] == x.Substring(0, x.Length - 4)) + 1;
                else
                    throw new NotImplementedException();
            }

            // Enumerate each sprite group
            for (int i = 0; i < worldFile.DesItems.Length; i++)
            {
                // Get the DES item
                var des = worldFile.DesItems[i];

                // Get the DES index
                var desIndex = desOffset + 1 + i;

                // Get the DES name
                var desName = desNames != null ? $" ({desNames[desIndex - 1]})" : String.Empty;

                // Find matching ETA for this DES
                List<Common_EventState> matchingStates = new List<Common_EventState>();

                if (worldFile.FileType != PC_WorldFile.Type.BigRay)
                {
                    // Search level events
                    foreach (var lvlEvent in levels.SelectMany(x => x.EventData.Events).Where(x => x.PC_ImageDescriptorsIndex == desIndex))
                        matchingStates.AddRange(eta[lvlEvent.PC_ETAIndex].States.SelectMany(x => x).Where(x => !matchingStates.Contains(x)));

                    // Search event info
                    foreach (var ei in eventInfo)
                    {
                        PC_ETA matchingEta = null;

                        if (context.Settings.EngineVersion == EngineVersion.RayPC)
                        {
                            if (ei.DesR1.TryGetValue(context.Settings.World, out int? desR1) && desR1 == desIndex)
                                matchingEta = eta[ei.EtaR1[context.Settings.World].Value];
                        }
                        else if (context.Settings.EngineVersion == EngineVersion.RayKitPC)
                        {
                            if (ei.DesKit.TryGetValue(context.Settings.World, out string desKit) && desKit == desName.Substring(0, desName.Length - 4))
                                matchingEta = eta[etaNames.FindItemIndex(x => x == ei.EtaKit[context.Settings.World])];
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        if (matchingEta != null)
                            matchingStates.AddRange(matchingEta.States.SelectMany(x => x).Where(x => !matchingStates.Contains(x)));
                    }
                }

                // Get the textures
                var textures = GetSpriteTextures(levels, des, desIndex, palette);

                // Get the folder
                var desFolderPath = Path.Combine(outputDir, $"{i}{desName}");

                // Get the animations
                var spriteAnim = des.AnimationDescriptors;

                // Use Rayman's animation if a special DES
                if (desIndex == darkRayDES || desIndex == smallRayDES)
                    spriteAnim = rayAnim;

                // Enumerate the animations
                for (var j = 0; j < spriteAnim.Length; j++)
                {
                    // Get the animation descriptor
                    var anim = spriteAnim[j];

                    var matches = matchingStates.Where(x => x.AnimationIndex == j).ToArray();

                    // Get the speeds
                    string speed;

                    // Hard-code for big ray
                    if (worldFile.FileType == PC_WorldFile.Type.BigRay)
                        speed = "1";
                    // Hard-code for clock event
                    else if (desIndex == 7)
                        speed = "4";
                    else
                        speed = String.Join("-", matches.Select(x => x.AnimationSpeed).Distinct());

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
                                var w = s.width + (desIndex == smallRayDES ? l.XPosition / 2 : l.XPosition);
                                var h = s.height + (desIndex == smallRayDES ? l.YPosition / 2 : l.YPosition);

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
                            filterMode = FilterMode.Point,
                            wrapMode = TextureWrapMode.Clamp
                        };

                        // Default to fully transparent
                        tex.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), tex.width * tex.height).ToArray());

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
                                    
                                    var xPosition = (animationLayer.IsFlippedHorizontally ? (sprite.width - 1 - x) : x) + (desIndex == smallRayDES ? animationLayer.XPosition / 2 : animationLayer.XPosition);
                                    var yPosition = (y + (desIndex == smallRayDES ? animationLayer.YPosition / 2 : animationLayer.YPosition));

                                    if (xPosition >= tex.width)
                                        throw new Exception("Horizontal overflow!");

                                    if (c.a != 0)
                                        tex.SetPixel(xPosition, tex.height - 1 - yPosition, c);
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

                // Unload textures
                await Resources.UnloadUnusedAssets();
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

                if (processedData[i] == 161 || processedData[i] == 250)
                {
                    flag = processedData[i];
                    processedData[i] = 0;
                }
                else if (flag != -1)
                {
                    int num6 = (flag < 255) ? (flag + 1) : 255;

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
        public Texture2D GetSpriteTexture(Common_ImageDescriptor s, IList<ARGBColor> palette, byte[] processedImageData)
        {
            // Ignore dummy sprites
            if (s.Index == 0)
                return null;

            // Get the image properties
            var width = s.OuterWidth;
            var height = s.OuterHeight;
            var offset = s.ImageBufferOffset;

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
                        tex.SetPixel(x, height - 1 - y, color.GetColor());
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

        /// <summary>
        /// Gets a common design
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="des">The DES</param>
        /// <param name="palette">The palette to use</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public virtual Common_Design GetCommonDesign(Context context, PC_DES des, IList<ARGBColor> palette, int desIndex)
        {
            // Create the common design
            Common_Design commonDesign = new Common_Design
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Common_Animation>()
            };

            // Process the image data
            var processedImageData = ProcessImageData(des.ImageData, des.RequiresBackgroundClearing);

            // Sprites
            foreach (var s in des.ImageDescriptors)
            {
                // Get the texture
                Texture2D tex = GetSpriteTexture(s, palette, processedImageData);

                // Add it to the array
                commonDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
            }

            // Animations
            foreach (var a in des.AnimationDescriptors)
                // Add the animation to list
                commonDesign.Animations.Add(a.ToCommonAnimation());

            return commonDesign;
        }

        #endregion

        #region Manager Methods

        /// <summary>
        /// Extracts the data from an archive file
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="file">The archive file</param>
        /// <returns>The archive data</returns>
        public virtual IEnumerable<ArchiveData> ExtractArchive(Context context, ArchiveFile file)
        {
            // Add the file to the context
            context.AddFile(new LinearSerializedFile(context)
            {
                filePath = file.FilePath
            });

            // Read the archive
            var data = FileFactory.Read<PC_EncryptedFileArchive>(file.FilePath, context);

            // Return the data
            for (int i = 0; i < data.DecodedFiles.Length; i++)
                yield return new ArchiveData(data.Entries[i].FileNameString, data.DecodedFiles[i]);
        }

        /// <summary>
        /// Gets the sound groups
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The available sound groups</returns>
        public IEnumerable<SoundGroup> GetSoundGroups(Context context)
        {
            // Get common sound files
            string soundFile = GetSoundFilePath();
            string soundManifestFile = GetSoundManifestFilePath();

            // Extract the archives
            var soundArchiveFileData = ExtractArchive(context, new ArchiveFile(soundFile));
            var soundManifestArchiveFileData = ExtractArchive(context, new ArchiveFile(soundManifestFile)).ToArray();

            var index = 0;

            // Handle every sound group
            foreach (var soundArchiveData in soundArchiveFileData)
            {
                // Get the sound manifest data
                var manifestArchiveData = soundManifestArchiveFileData[index];

                // Get the manifest data
                using (var manfiestStream = new MemoryStream(manifestArchiveData.Data))
                {
                    using (var manifestContext = new Context(context.Settings))
                    {
                        // Create a key
                        var key = $"manifest{index}";

                        // Add to context
                        manifestContext.AddFile(new StreamFile(key, manfiestStream, manifestContext));

                        // Serialize the manifest data
                        var manfiestData = FileFactory.Read<PC_SoundManifest>(key, manifestContext, (o, file) => file.Length = o.CurrentLength / (4 * 4));

                        // Get the group name
                        var groupName = manifestArchiveData.FileName;

                        // Create the group
                        var group = new SoundGroup()
                        {
                            GroupName = groupName
                        };

                        var groupEntries = new List<SoundGroup.SoundGroupEntry>();

                        // Handle every sound file entry
                        for (int j = 0; j < manfiestData.SoundFileEntries.Length; j++)
                        {
                            // Get the entry
                            var entry = manfiestData.SoundFileEntries[j];

                            // Make sure it contains any data
                            if (entry.FileSize == 0)
                                continue;

                            // Get the bytes
                            var soundEntryBytes = soundArchiveData.Data.Skip((int)entry.FileOffset).Take((int)entry.FileSize).ToArray();

                            groupEntries.Add(new SoundGroup.SoundGroupEntry()
                            {
                                FileName = $"{groupName}_{j}",
                                RawSoundData = soundEntryBytes
                            });
                        }

                        group.Entries = groupEntries.ToArray();

                        // Return the group
                        yield return group;
                    }
                }

                index++;
            }

            // Handle the additional archives
            foreach (var archiveData in GetAdditionalSoundArchives(context.Settings))
            {
                // Extract the archive
                var archive = ExtractArchive(context, archiveData.ArchiveFile);

                // Create and return the group
                yield return new SoundGroup()
                {
                    GroupName = archiveData.Name,
                    Entries = archive.Select(x => new SoundGroup.SoundGroupEntry()
                    {
                        FileName = x.FileName,
                        RawSoundData = x.Data
                    }).ToArray(),
                    BitsPerSample = archiveData.BitsPerSample
                };
            }
        }

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public virtual GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Export Sprites", false, true, (input, output) => ExportSpriteTexturesAsync(settings, output, false)),
                new GameAction("Export Animation Frames", false, true, (input, output) => ExportSpriteTexturesAsync(settings, output, true)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignette(settings, GetVignetteFilePath(settings), output)),
                new GameAction("Export Archives", false, true, (input, output) => ExtractArchives(output)),
                new GameAction("Export Sound", false, true, (input, output) => ExtractSound(settings, output)),
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
            };
        }

        /// <summary>
        /// Extracts the sound data
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The output path</param>
        public void ExtractSound(GameSettings settings, string outputPath)
        {
            // Create a new context
            using (var context = new Context(Settings.GetGameSettings))
            {
                // Handle every sound group
                foreach (var soundGroup in GetSoundGroups(context))
                {
                    // Get the output directory
                    var groupOutputDir = Path.Combine(outputPath, soundGroup.GroupName);

                    // Create the directory
                    Directory.CreateDirectory(groupOutputDir);

                    // Handle every sound file entry
                    foreach (var soundGroupEntry in soundGroup.Entries)
                    {
                        // Create WAV data
                        var wav = new WAV
                        {
                            Magic = new byte[]
                            {
                                0x52, 0x49, 0x46, 0x46
                            },
                            FileSize = (44 - 8) + (uint)soundGroupEntry.RawSoundData.Length,
                            FileTypeHeader = new byte[]
                            {
                                0x57, 0x41, 0x56, 0x45
                            },
                            FormatChunkMarker = new byte[]
                            {
                                0x66, 0x6D, 0x74, 0x20
                            },
                            FormatDataLength = 0x10,
                            FormatType = 1,
                            ChannelCount = 1,
                            SampleRate = 11025,
                            BitsPerSample = (ushort)soundGroup.BitsPerSample,
                            DataChunkHeader = new byte[]
                            {
                                0x64, 0x61, 0x74, 0x61
                            },
                            DataSize = (uint)soundGroupEntry.RawSoundData.Length,
                            Data = soundGroupEntry.RawSoundData
                        };

                        wav.ByteRate = (wav.SampleRate * wav.BitsPerSample * wav.ChannelCount) / 8;
                        wav.BlockAlign = (ushort)((wav.BitsPerSample * wav.ChannelCount) / 8);

                        // Get the output path
                        var outputFilePath = Path.Combine(groupOutputDir, soundGroupEntry.FileName + ".wav");

                        // Create and open the output file
                        using (var outputStream = File.Create(outputFilePath))
                        {
                            // Create a context
                            using (var wavContext = new Context(settings))
                            {
                                // Create a key
                                const string wavKey = "wav";

                                // Add the file to the context
                                wavContext.AddFile(new StreamFile(wavKey, outputStream, wavContext));

                                // Write the data
                                FileFactory.Write<WAV>(wavKey, wav, wavContext);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extracts all file archives
        /// </summary>
        /// <param name="outputPath">The output path to extract to</param>
        public void ExtractArchives(string outputPath)
        {
            // Create a new context
            using (var context = new Context(Settings.GetGameSettings))
            {
                // Extract every archive file
                foreach (var archiveFile in GetArchiveFiles(context.Settings).Where(x => File.Exists(context.BasePath + x.FilePath)))
                {
                    // Get the output directory
                    var output = Path.Combine(outputPath, Path.GetDirectoryName(archiveFile.FilePath), Path.GetFileNameWithoutExtension(archiveFile.FilePath));

                    // Create the directory
                    Directory.CreateDirectory(output);

                    // Extract every file
                    foreach (var fileData in ExtractArchive(context, archiveFile))
                        // Write the bytes
                        File.WriteAllBytes(Path.Combine(output, fileData.FileName + archiveFile.FileExtension), fileData.Data);
                }
            }
        }

        public void ExportPaletteImage(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                var pal = new List<RGB666Color[]>();

                // Enumerate every world
                foreach (var world in GetLevels(settings))
                {
                    settings.World = world.Key;

                    // Enumerate every level
                    foreach (var lvl in world.Value)
                    {
                        settings.Level = lvl;

                        // Get the file path
                        var path = GetLevelFilePath(settings);

                        // Load the level
                        context.AddFile(new LinearSerializedFile(context)
                        {
                            filePath = path
                        });

                        // Read the level
                        var lvlData = FileFactory.Read<PC_LevFile>(path, context);

                        // Add the palettes
                        foreach (var mapPal in lvlData.MapData.ColorPalettes)
                            if (!pal.Any(x => x.SequenceEqual(mapPal)))
                                pal.Add(mapPal);
                    }
                }

                // Export
                PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), pal.SelectMany(x => x).ToArray(), optionalWrap: 256);
            }
        }

        /// <summary>
        /// Imports raw image data into a DES
        /// </summary>
        /// <param name="des">The DES item</param>
        /// <param name="rawImageData">The raw image data, categorized by image descriptor</param>
        public void ImportRawImageData(PC_DES des, IEnumerable<KeyValuePair<int, byte[]>> rawImageData)
        {
            // TODO: Clean this up

            // Import every image data
            foreach (var data in rawImageData)
            {
                // Get the descriptor
                var imgDesc = des.ImageDescriptors[data.Key];

                // Add every byte and encrypt it
                for (int i = 0; i < data.Value.Length; i++)
                    des.ImageData[imgDesc.ImageBufferOffset + i] = (byte)(data.Value[i] ^ 143);
            }

            // TODO: Move the reverse image processing to its own method
            int flag = -1;

            // Process every byte
            for (int i = des.ImageData.Length - 1; i >= 0; i--)
            {
                // Get the decrypted value
                var val = des.ImageData[i] ^ 143;

                // Check if it should be transparent
                if (val == 0)
                {
                    if (flag == -1)
                        flag = 161;
                    else
                        flag++;

                    if (flag > 255)
                        flag = 255;

                    des.ImageData[i] = (byte)(flag ^ 143);
                }
                else
                {
                    flag = -1;
                }
            }

            // TODO: Checksum should be handled automatically when writing
            var check = new Checksum8Calculator();
            check.AddBytes(des.ImageData);
            des.ImageDataChecksum = check.ChecksumValue;
        }

        /// <summary>
        /// Loads the sprites for the level
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="palette">The palette to use</param>
        /// <returns>The common event designs</returns>
        public async Task<Common_Design[]> LoadSpritesAsync(Context context, IList<ARGBColor> palette)
        {
            // Create the output list
            List<Common_Design> eventDesigns = new List<Common_Design>();

            Controller.status = $"Loading allfix";

            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(GetAllfixFilePath(context.Settings), context,
                onPreSerialize: (s, data) => data.FileType = PC_WorldFile.Type.AllFix);

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading world";

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context,
                onPreSerialize: (s, data) => data.FileType = PC_WorldFile.Type.World);

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading big ray";

            // NOTE: This is not loaded into normal levels and is purely loaded here so the animation can be viewed!
            // Read the big ray data
            var bigRayData = FileFactory.Read<PC_WorldFile>(GetBigRayFilePath(context.Settings), context,
                onPreSerialize: (s, data) => data.FileType = PC_WorldFile.Type.BigRay);

            // Get the big ray palette
            var bigRayPalette = GetBigRayPalette(context);

            await Controller.WaitIfNecessary();

            // Get the DES and ETA
            var des = allfix.DesItems.Concat(worldData.DesItems).Concat(bigRayData.DesItems).ToArray();

            int desIndex = 0;

            // Add dummy DES to index 0
            eventDesigns.Add(new Common_Design());

            // Read every DES item
            foreach (PC_DES d in des)
            {
                Controller.status = $"Loading DES {desIndex}/{des.Length}";

                await Controller.WaitIfNecessary();

                // Use big ray palette for last one
                var p = desIndex == des.Length - 1 && bigRayPalette != null ? bigRayPalette : palette;

                // Add to the designs
                eventDesigns.Add(GetCommonDesign(context, d, p, desIndex));

                desIndex++;
            }

            // Return the sprites
            return eventDesigns.ToArray();
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public virtual async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading map data for {context.Settings.World} {context.Settings.Level}";

            // Read the level data
            var levelData = FileFactory.Read<PC_LevFile>(GetLevelFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev 
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = levelData.MapData.Width,
                        Height = levelData.MapData.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        MapTiles = levelData.MapData.Tiles.Select(x => new Editor_MapTile(x)).ToArray(),
                        TileSetWidth = 1,

                        TileSetTransparencyModes = levelData.TileTextureData.TexturesOffsetTable.Select(x => levelData.TileTextureData.NonTransparentTextures.Concat(levelData.TileTextureData.TransparentTextures).FirstOrDefault(t => t.Offset == x)).Select(x =>
                        {
                            if (x == null)
                                return PC_MapTileTransparencyMode.FullyTransparent;

                            if (x.TransparencyMode == 0xAAAAAAAA)
                                return PC_MapTileTransparencyMode.FullyTransparent;

                            if (x.TransparencyMode == 0x55555555)
                                return PC_MapTileTransparencyMode.NoTransparency;

                            return PC_MapTileTransparencyMode.PartiallyTransparent;
                        }).ToArray(),
                        PCTileOffsetTable = levelData.TileTextureData.TexturesOffsetTable
                    }
                },

                // Create the events list
                EventData = new List<Editor_EventData>(),
            };

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context, levelData.MapData.ColorPalettes.First()) : new Common_Design[0];

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context,
                onPreSerialize: (s, data) => data.FileType = PC_WorldFile.Type.World);

            // Get file names if available
            var desNames = worldData.DESFileNames ?? new string[0];
            var etaNames = worldData.ETAFileNames ?? new string[0];

            var index = 0;

            foreach (EventData e in levelData.EventData.Events)
            {
                // Get the file keys
                var desKey = desNames.Any() ? desNames[e.PC_ImageDescriptorsIndex] : e.PC_ImageDescriptorsIndex.ToString();
                var etaKey = etaNames.Any() ? etaNames[e.PC_ETAIndex] : e.PC_ETAIndex.ToString();

                // Add the event
                commonLev.EventData.Add(new Editor_EventData(e)
                {
                    Type = e.Type,
                    DESKey = desKey,
                    ETAKey = etaKey,
                    LabelOffsets = levelData.EventData.EventCommands[index].LabelOffsetTable,
                    CommandCollection = levelData.EventData.EventCommands[index].Commands,
                    LinkIndex = levelData.EventData.EventLinkingTable[index],
                    DebugText = $"Flags: {String.Join(", ", e.PC_Flags.GetFlags())}{Environment.NewLine}"
                });

                index++;
            }

            // Add Rayman
            commonLev.Rayman = new Editor_EventData(EventData.Rayman)
            {
                Type = EventType.TYPE_RAYMAN,
                DESKey = desNames.Any() ? desNames[1] : "1",
                ETAKey = etaNames.Any() ? etaNames[0] : "0",
            };

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading tile set";

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            commonLev.Maps[0].TileSet[0] = tileSets[0];
            commonLev.Maps[0].TileSet[1] = tileSets[1];
            commonLev.Maps[0].TileSet[2] = tileSets[2];

            // Enumerate each cell
            for (int cellY = 0; cellY < levelData.MapData.Height; cellY++) 
            {
                for (int cellX = 0; cellX < levelData.MapData.Width; cellX++) 
                {
                    // Get the cell
                    var cell = levelData.MapData.Tiles[cellY * levelData.MapData.Width + cellX];

                    // TODO: FIX
                    // Set the common tile
                    commonLev.Maps[0].MapTiles[cellY * levelData.MapData.Width + cellX] = new Editor_MapTile(cell);
                }
            }

            // Return an editor manager
            return GetEditorManager(commonLev, context, eventDesigns);
        }

        /// <summary>
        /// Gets an editor manager from the specified objects
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="designs">The common design</param>
        /// <returns>The editor manager</returns>
        public abstract BaseEditorManager GetEditorManager(Common_Lev level, Context context, Common_Design[] designs);

        /// <summary>
        /// Reads 3 tile-sets, one for each palette
        /// </summary>
        /// <param name="levData">The level data to get the tile-set for</param>
        /// <returns>The 3 tile-sets</returns>
        public Common_Tileset[] ReadTileSets(PC_LevFile levData) {
            // Create the output array
            var output = new Common_Tileset[]
            {
                new Common_Tileset(new Tile[levData.TileTextureData.TexturesOffsetTable.Length]),
                new Common_Tileset(new Tile[levData.TileTextureData.TexturesOffsetTable.Length]),
                new Common_Tileset(new Tile[levData.TileTextureData.TexturesOffsetTable.Length])
            };

            // Keep track of the tile index
            int index = 0;

            // Get all tile textures
            var allTex = levData.TileTextureData.NonTransparentTextures.Concat(levData.TileTextureData.TransparentTextures).ToArray();

            // Enumerate every texture
            foreach (var offset in levData.TileTextureData.TexturesOffsetTable)
            {
                // Find matching tile texture
                var tileTex = allTex.FirstOrDefault(x => x.Offset == offset);

                // Enumerate every palette
                for (int i = 0; i < levData.MapData.ColorPalettes.Length; i++)
                {
                    // Create the texture to use for the tile
                    var tileTexture = new Texture2D(Settings.CellSize, Settings.CellSize, TextureFormat.RGBA32, false)
                    {
                        filterMode = FilterMode.Point,
                        wrapMode = TextureWrapMode.Clamp
                    };

                    // Write each pixel to the texture
                    for (int y = 0; y < Settings.CellSize; y++)
                    {
                        for (int x = 0; x < Settings.CellSize; x++)
                        {
                            // Get the index
                            var cellIndex = Settings.CellSize * y + x;

                            // Get the color from the current palette (or default to fully transparent if a valid tile texture was not found or it's the first one)
                            var c = tileTex == null || index == 0 ? new Color(0, 0, 0, 0) : levData.MapData.ColorPalettes[i][255 - tileTex.ColorIndexes[cellIndex]].GetColor();

                            // If the texture is transparent, add the alpha channel
                            if (tileTex is PC_TransparentTileTexture tt)
                                c.a = (float)tt.Alpha[cellIndex] / Byte.MaxValue;

                            // Set the pixel
                            tileTexture.SetPixel(x, y, c);
                        }
                    }

                    // Apply the pixels to the texture
                    tileTexture.Apply();

                    // Create and set up the tile
                    output[i].SetTile(tileTexture, Settings.CellSize, index);
                }

                index++;
            }

            return output;
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="editorManager">The editor manager</param>
        public void SaveLevel(Context context, BaseEditorManager editorManager) {
            var commonLevelData = editorManager.Level;

            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<PC_LevFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++) {
                for (int x = 0; x < lvlData.MapData.Width; x++) {
                    // Set the tiles
                    lvlData.MapData.Tiles[y * lvlData.MapData.Width + x] = commonLevelData.Maps[0].MapTiles[y * lvlData.MapData.Width + x].Data;
                }
            }

            // Temporary event lists
            var events = new List<EventData>();
            var eventCommands = new List<PC_EventCommand>();
            var eventLinkingTable = new List<ushort>();

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context,
                onPreSerialize: (s, data) => data.FileType = PC_WorldFile.Type.World);

            // Get file names if available
            var desNames = worldData.DESFileNames ?? new string[0];
            var etaNames = worldData.ETAFileNames ?? new string[0];

            foreach (var e in commonLevelData.EventData) 
            {
                // Get the file indexes
                var desIndex = desNames.Any() ? (uint)desNames.FindItemIndex(x => x == e.DESKey) : UInt32.Parse(e.DESKey);
                var etaIndex = etaNames.Any() ? (uint)etaNames.FindItemIndex(x => x == e.ETAKey) : UInt32.Parse(e.ETAKey);

                var r1Event = e.Data;

                if (r1Event.PS1Demo_Unk1 == null)
                    r1Event.PS1Demo_Unk1 = new byte[40];

                if (r1Event.Unk_98 == null)
                    r1Event.Unk_98 = new byte[5];

                r1Event.PC_ImageDescriptorsIndex = desIndex;
                r1Event.PC_AnimationDescriptorsIndex = desIndex;
                r1Event.PC_ImageBufferIndex = desIndex;
                r1Event.PC_ETAIndex = etaIndex;

                r1Event.ImageDescriptorCount = (ushort)editorManager.DES[e.DESKey].Sprites.Count;
                r1Event.AnimDescriptorCount = (byte)editorManager.DES[e.DESKey].Animations.Count;

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
            lvlData.EventData.EventCount = (ushort)events.Count;
            lvlData.EventData.Events = events.ToArray();
            lvlData.EventData.EventCommands = eventCommands.ToArray();
            lvlData.EventData.EventLinkingTable = eventLinkingTable.ToArray();

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
                foreach (var lvl in GetLevels(context.Settings).FindItem(x => x.Key == world).Value)
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
        /// Gets the event states for the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The event states</returns>
        public virtual IEnumerable<PC_ETA> GetCurrentEventStates(Context context)
        {
            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(GetAllfixFilePath(context.Settings), context, (s, x) => x.FileType = PC_WorldFile.Type.AllFix);

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context, (s, x) => x.FileType = PC_WorldFile.Type.World);

            // Read the big ray data
            var bigRayData = FileFactory.Read<PC_WorldFile>(GetBigRayFilePath(context.Settings), context, (s, x) => x.FileType = PC_WorldFile.Type.BigRay);

            // Get the eta items
            return allfix.Eta.Concat(worldData.Eta).Concat(bigRayData.Eta);
        }

        #endregion

        #region Classes

        /// <summary>
        /// Archive file info
        /// </summary>
        public class ArchiveFile
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="filePath">The file path</param>
            /// <param name="fileExtension">The file extension</param>
            public ArchiveFile(string filePath, string fileExtension = ".dat")
            {
                FilePath = filePath;
                FileExtension = fileExtension;
            }

            /// <summary>
            /// The file path
            /// </summary>
            public string FilePath { get; }

            /// <summary>
            /// The file extension
            /// </summary>
            public string FileExtension { get; }
        }

        /// <summary>
        /// Archive data
        /// </summary>
        public class ArchiveData
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="fileName">The file name</param>
            /// <param name="data">The data</param>
            public ArchiveData(string fileName, byte[] data)
            {
                FileName = fileName;
                Data = data;
            }

            /// <summary>
            /// The file name
            /// </summary>
            public string FileName { get; }

            /// <summary>
            /// The data
            /// </summary>
            public byte[] Data { get; }
        }

        /// <summary>
        /// Sound group data
        /// </summary>
        public class SoundGroup
        {
            /// <summary>
            /// The group name
            /// </summary>
            public string GroupName { get; set; }

            /// <summary>
            /// The entries
            /// </summary>
            public SoundGroupEntry[] Entries { get; set; }

            /// <summary>
            /// The bits per sample
            /// </summary>
            public int BitsPerSample { get; set; } = 8;

            /// <summary>
            /// Sound group entry data
            /// </summary>
            public class SoundGroupEntry
            {
                /// <summary>
                /// The file name
                /// </summary>
                public string FileName { get; set; }

                /// <summary>
                /// The raw sound data
                /// </summary>
                public byte[] RawSoundData { get; set; }
            }
        }

        /// <summary>
        /// Additional sound archive data
        /// </summary>
        public class AdditionalSoundArchive
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="name">The name</param>
            /// <param name="archiveFile">The archive file</param>
            /// <param name="bitsPerSample">The bits per sample</param>
            public AdditionalSoundArchive(string name, ArchiveFile archiveFile, int bitsPerSample = 8)
            {
                Name = name;
                ArchiveFile = archiveFile;
                BitsPerSample = bitsPerSample;
            }

            /// <summary>
            /// The name
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// The archive file
            /// </summary>
            public ArchiveFile ArchiveFile { get; }

            /// <summary>
            /// The bits per sample
            /// </summary>
            public int BitsPerSample { get; }
        }

        #endregion
    }
}