using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PC
    /// </summary>
    public abstract class R1_PCBaseManager : IGameManager {
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
        public string GetWorldName(R1_World world) {
            switch (world) {
                case R1_World.Jungle:
                    return "JUNGLE";
                case R1_World.Music:
                    return "MUSIC";
                case R1_World.Mountain:
                    return "MOUNTAIN";
                case R1_World.Image:
                    return "IMAGE";
                case R1_World.Cave:
                    return "CAVE";
                case R1_World.Cake:
                    return "CAKE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Gets the short name for the world
        /// </summary>
        /// <returns>The short world name</returns>
        public virtual string GetShortWorldName(R1_World world) {
            switch (world) {
                case R1_World.Jungle:
                    return "JUN";
                case R1_World.Music:
                    return "MUS";
                case R1_World.Mountain:
                    return "MON";
                case R1_World.Image:
                    return "IMA";
                case R1_World.Cave:
                    return "CAV";
                case R1_World.Cake:
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
        public abstract GameInfo_Volume[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        /// <param name="settings">The game settings</param>
        public abstract Archive[] GetArchiveFiles(GameSettings settings);

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public abstract AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings);

        public abstract bool IsDESMultiColored(Context context, int desIndex, GeneralEventInfoData[] generalEvents);

        #endregion

        #region Texture Methods

        /// <summary>
        /// Extracts a the vignette files
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <param name="vigPath">The vignette file path</param>
        /// <param name="outputDir">The output directory</param>
        public virtual void ExtractVignette(GameSettings settings, string vigPath, string outputDir)
        {
            // Create a new context
            using (var context = new Context(settings))
            {
                context.AddFile(new LinearSerializedFile(context)
                {
                    filePath = vigPath
                });

                // Read the archive
                var archive = FileFactory.Read<R1_PC_EncryptedFileArchive>(vigPath, context);

                // Extract every .pcx file
                for (int i = 0; i < archive.Entries.Length; i++)
                {
                    // Read the data
                    var pcx = archive.ReadFile<PCX>(context, i);

                    // Convert to a texture
                    var tex = pcx.ToTexture(true);

                    // Write the bytes
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{archive.Entries[i].FileName ?? i.ToString()}.png"), tex.EncodeToPNG());
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
            for (int i = 0; i <= 255; i++)
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
                                var tex = pcx.ToTexture(true);

                                // Add the file
                                foundPCX.Add($"{i}-{j}", tex.EncodeToPNG());
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
                FileOffset = long.Parse(x.Key.Split('-')[1]),
                Data = x.Value
            }).OrderBy(x => x.FileOffset))
            {
                File.WriteAllBytes(Path.Combine(outputDir, $"{index}. [{pcx.XORKey}] ({string.Format("{0:X8}", pcx.FileOffset)}).png"), pcx.Data);

                index++;
            }
        }

        /// <summary>
        /// Exports all sprite textures to the specified output directory
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="exportAnimFrames">Indicates if the textures should be exported as animation frames</param>
        public async UniTask ExportSpriteTexturesAsync(GameSettings settings, string outputDir, bool exportAnimFrames) 
        {
            // Create the context
            using (Context context = new Context(settings)) {
                // Add all files
                AddAllFiles(context);

                // Load the event info data
                var eventInfo = LevelEditorData.EventInfoData;

                // Get the DES names for every world
                var desNames = WorldHelpers.GetR1Worlds().ToDictionary(x => x, world => {
                    // Set the world
                    context.Settings.R1_World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        return null;

                    // TODO: Update this to not include extensions
                    var a = FileFactory.Read<R1_PC_WorldFile>(worldPath, context).DESFileNames?.Skip(1).ToArray();

                    return a?.Any() == true ? a : null;
                });

                // Get the ETA names for every world
                var etaNames = WorldHelpers.GetR1Worlds().ToDictionary(x => x, world => {
                    // Set the world
                    context.Settings.R1_World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        return null;

                    var a = FileFactory.Read<R1_PC_WorldFile>(worldPath, context).ETAFileNames?.ToArray();

                    return a?.Any() == true ? a : null;
                });

                context.Settings.R1_World = R1_World.Jungle;

                // Keep track of Rayman's anim
                R1_PC_AnimationDescriptor[] rayAnim = null;

                // Helper method for exporting textures
                async UniTask<Wld> ExportTexturesAsync<Wld>(string filePath, string name, int desOffset, IEnumerable<R1_PC_ETA> baseEta, string[] desFileNames, string[] etaFileNames, IList<ARGBColor> palette = null)
                    where Wld : R1_PC_BaseWorldFile, new()
                {
                    // Read the file
                    var file = FileFactory.Read<Wld>(filePath, context);

                    if (rayAnim == null && file is R1_PC_AllfixFile)
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
                await ExportTexturesAsync<R1_PC_BigRayFile>(GetBigRayFilePath(context.Settings), "Bigray", 0, new R1_PC_ETA[0], null, null, GetBigRayPalette(context));

                // Export allfix
                var allfix = await ExportTexturesAsync<R1_PC_AllfixFile>(GetAllfixFilePath(context.Settings), "Allfix", 0, new R1_PC_ETA[0], desNames.Values.FirstOrDefault(), etaNames.Values.FirstOrDefault());

                // Enumerate every world
                foreach (R1_World world in WorldHelpers.GetR1Worlds()) {
                    // Set the world
                    context.Settings.R1_World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.Settings);

                    if (!FileSystem.FileExists(context.BasePath + worldPath))
                        continue;

                    // Export world
                    await ExportTexturesAsync<R1_PC_WorldFile>(worldPath, world.ToString(), allfix.DesItems.Length, allfix.Eta, desNames.TryGetItem(world), etaNames.TryGetItem(world));
                }
            }
        }

        /// <summary>
        /// Gets the big ray palette if available
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The big ray palette</returns>
        public virtual IList<ARGBColor> GetBigRayPalette(Context context) => null;

        /// <summary>
        /// Exports all sprite textures from the world file to the specified output directory
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="worldFile">The world file</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="desOffset">The amount of textures in the allfix to use as the DES offset if a world texture</param>
        /// <param name="desNames">The DES names, if available</param>
        /// <param name="palette">Optional palette to use</param>
        public void ExportSpriteTextures(Context context, R1_PC_BaseWorldFile worldFile, string outputDir, int desOffset, string[] desNames, IList<ARGBColor> palette = null) {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<R1_PC_LevFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.Settings).First(x => x.Name == context.Settings.EduVolume || x.Name == null).Worlds.First(x => x.Index == context.Settings.World).Maps.OrderBy(x => x)) {
                // Set the level number
                context.Settings.Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.Settings);

                // Load the level
                levels.Add(FileFactory.Read<R1_PC_LevFile>(lvlPath, context));
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
        public Texture2D[] GetSpriteTextures(List<R1_PC_LevFile> levels, R1_PC_DES desItem, int desIndex, IList<ARGBColor> palette = null)
        {
            // Create the output array
            var output = new Texture2D[desItem.ImageDescriptors.Length];

            // Process the image data
            var processedImageData = desItem.RequiresBackgroundClearing ? ProcessImageData(desItem.ImageData) : desItem.ImageData;

            // Find the level with the correct palette
            var lvl = levels.FindLast(x => x.BackgroundSpritesDES == desIndex || x.EventData.Events.Any(y => y.PC_ImageDescriptorsIndex == desIndex)) ?? levels.First();

            // Enumerate each image
            for (int i = 0; i < desItem.ImageDescriptors.Length; i++)
            {
                // Get the image descriptor
                var imgDescriptor = desItem.ImageDescriptors[i];

                // Ignore dummy sprites
                if (imgDescriptor.IsDummySprite())
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
        public async UniTask ExportAnimationFramesAsync(Context context, R1_PC_BaseWorldFile worldFile, string outputDir, int desOffset, R1_PC_ETA[] eta, string[] desNames, string[] etaNames, IList<GeneralEventInfoData> eventInfo, R1_PC_AnimationDescriptor[] rayAnim, IList<ARGBColor> palette = null)
        {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<R1_PC_LevFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.Settings).First(x => x.Name == context.Settings.EduVolume).Worlds.FindItem(x => x.Index == context.Settings.World).Maps.OrderBy(x => x))
            {
                // Set the level number
                context.Settings.Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.Settings);

                // Load the level
                levels.Add(FileFactory.Read<R1_PC_LevFile>(lvlPath, context));
            }

            // Get special DES
            int? smallRayDES = null;
            int? darkRayDES = null;

            // Get the small Rayman DES if allfix
            if (worldFile is R1_PC_AllfixFile)
            {
                var ei = eventInfo.FindItem(x => x.Type == (int)R1_EventType.TYPE_DEMI_RAYMAN);

                if (context.Settings.EngineVersion == EngineVersion.R1_PC)
                    smallRayDES = ei.DesR1[R1_World.Jungle];
                else if (context.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
                    smallRayDES = desNames.FindItemIndex(x => ei.DesKit[R1_World.Jungle] == x.Substring(0, x.Length - 4)) + 1;
                else
                    throw new NotImplementedException();
            }

            // Get the Dark Rayman DES if Cake
            if (worldFile is R1_PC_WorldFile && context.Settings.World == (int)R1_World.Cake)
            {
                var ei = eventInfo.FindItem(x => x.Type == (int)R1_EventType.TYPE_BLACK_RAY);

                if (context.Settings.EngineVersion == EngineVersion.R1_PC)
                    darkRayDES = ei.DesR1[R1_World.Cake];
                else if (context.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
                    darkRayDES = desNames.FindItemIndex(x => ei.DesKit[R1_World.Cake] == x.Substring(0, x.Length - 4)) + 1;
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
                List<R1_EventState> matchingStates = new List<R1_EventState>();

                if (!(worldFile is R1_PC_BigRayFile))
                {
                    // Search level events
                    foreach (var lvlEvent in levels.SelectMany(x => x.EventData.Events).Where(x => x.PC_ImageDescriptorsIndex == desIndex))
                        matchingStates.AddRange(eta[lvlEvent.PC_ETAIndex].States.SelectMany(x => x).Where(x => !matchingStates.Contains(x)));

                    // Search event info
                    foreach (var ei in eventInfo)
                    {
                        R1_PC_ETA matchingEta = null;

                        if (context.Settings.EngineVersion == EngineVersion.R1_PC)
                        {
                            if (ei.DesR1.TryGetValue(context.Settings.R1_World, out int? desR1) && desR1 == desIndex)
                                matchingEta = eta[ei.EtaR1[context.Settings.R1_World].Value];
                        }
                        else if (context.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
                        {
                            if (ei.DesKit.TryGetValue(context.Settings.R1_World, out string desKit) && desKit == desName.Substring(0, desName.Length - 4))
                                matchingEta = eta[etaNames.FindItemIndex(x => x == ei.EtaKit[context.Settings.R1_World])];
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
                    if (worldFile is R1_PC_BigRayFile)
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
                        Texture2D tex = TextureHelpers.CreateTexture2D(frameWidth ?? 1, frameHeight ?? 1, clear: true);

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
        public byte[] ProcessImageData(byte[] imageData)
        {
            // Create the output array
            var processedData = new byte[imageData.Length];

            int flag = -1;

            for (int i = imageData.Length - 1; i >= 0; i--)
            {
                // Get the byte
                var b = imageData[i];

                if (b == 161 || b == 250)
                {
                    flag = b;
                    b = 0;
                }
                else if (flag != -1)
                {
                    int num6 = (flag < 0xFF) ? (flag + 1) : 0xFF;

                    if (b == num6)
                    {
                        b = 0;
                        flag = num6;
                    }
                    else
                    {
                        flag = -1;
                    }
                }

                // Set the byte
                processedData[i] = b;
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
        public Texture2D GetSpriteTexture(R1_ImageDescriptor s, IList<ARGBColor> palette, byte[] processedImageData)
        {
            // Ignore dummy sprites
            if (s.IsDummySprite())
                return null;

            // Get the image properties
            var width = s.Width;
            var height = s.Height;
            var offset = s.ImageBufferOffset;

            // Create the texture
            Texture2D tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

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
        public virtual Unity_ObjGraphics GetCommonDesign(Context context, R1_PC_DES des, IList<ARGBColor> palette, int desIndex)
        {
            // Check if the DES is used for multi-colored events
            var isMultiColored = IsDESMultiColored(context, desIndex + 1, LevelEditorData.EventInfoData);

            // Create the common design
            Unity_ObjGraphics graphics = new Unity_ObjGraphics
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>()
            };

            // Process the image data
            var processedImageData = des.RequiresBackgroundClearing ? ProcessImageData(des.ImageData) : des.ImageData;

            if (!isMultiColored)
            {
                // Sprites
                foreach (var s in des.ImageDescriptors)
                {
                    // Get the texture
                    Texture2D tex = GetSpriteTexture(s, palette, processedImageData);

                    // Add it to the array
                    graphics.Sprites.Add(tex == null ? null : tex.CreateSprite());
                }
            }
            else
            {
                // Add sprites for each color
                for (int i = 0; i < 6; i++)
                {
                    // Hack to get correct colors
                    var p = palette.Skip(i * 8 + 1).ToList();

                    p.Insert(0, new ARGBColor(0, 0, 0));

                    if (i % 2 != 0)
                        p[8] = palette[i * 8];

                    // Sprites
                    foreach (var s in des.ImageDescriptors)
                    {
                        // Get the texture
                        Texture2D tex = GetSpriteTexture(s, p, processedImageData);

                        // Add it to the array
                        graphics.Sprites.Add(tex == null ? null : tex.CreateSprite());
                    }
                }
            }

            // Animations
            foreach (var a in des.AnimationDescriptors)
                // Add the animation to list
                graphics.Animations.Add(a.ToCommonAnimation());

            return graphics;
        }

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the sound groups
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The available sound groups</returns>
        public async UniTask<IEnumerable<SoundGroup>> GetSoundGroupsAsync(Context context)
        {
            var output = new List<SoundGroup>();
            
            // Get common sound files
            string soundFile = GetSoundFilePath();
            string soundManifestFile = GetSoundManifestFilePath();

            await AddFile(context, soundFile);
            await AddFile(context, soundManifestFile);

            // Extract the archives
            var soundArchive = FileFactory.Read<R1_PC_EncryptedFileArchive>(soundFile, context);
            var soundManifestArchive = FileFactory.Read<R1_PC_EncryptedFileArchive>(soundManifestFile, context);

            var index = 0;

            // Handle every sound group
            foreach (var soundArchiveEntry in soundArchive.Entries)
            {
                var manifestArchiveEntry = soundManifestArchive.Entries[index];

                // Read the manifest data
                var manifestData = soundManifestArchive.ReadFile<R1_PC_SoundManifest>(context, index, file => file.Length = manifestArchiveEntry.FileSize / (4 * 4));

                var groupName = manifestArchiveEntry.FileName ?? index.ToString();

                // Create the group
                var group = new SoundGroup()
                {
                    GroupName = groupName
                };

                var groupEntries = new List<SoundGroup.SoundGroupEntry>();

                // Handle every sound file entry
                for (int j = 0; j < manifestData.SoundFileEntries.Length; j++)
                {
                    // Get the entry
                    var entry = manifestData.SoundFileEntries[j];

                    // Make sure it contains any data
                    if (entry.FileSize == 0)
                        continue;

                    // Get the bytes
                    var s = context.Deserializer;
                    s.DoXOR(soundArchiveEntry.XORKey, () =>
                    {
                        var soundEntryBytes = s.DoAt(soundArchive.Offset + soundArchiveEntry.FileOffset + entry.FileOffset, () => s.SerializeArray<byte>(default, entry.FileSize));

                        groupEntries.Add(new SoundGroup.SoundGroupEntry()
                        {
                            FileName = $"{groupName}_{j}",
                            RawSoundData = soundEntryBytes
                        });
                    });
                }

                group.Entries = groupEntries.ToArray();

                // Add the group
                output.Add(group);

                index++;
            }

            // Handle the additional archives
            foreach (var archiveData in GetAdditionalSoundArchives(context.Settings))
            {
                if (!File.Exists(context.BasePath + archiveData.ArchiveFile))
                    continue;

                await AddFile(context, archiveData.ArchiveFile);

                // Extract the archive
                var archive = FileFactory.Read<R1_PC_EncryptedFileArchive>(archiveData.ArchiveFile, context);

                // Create and add the group
                output.Add(new SoundGroup()
                {
                    GroupName = archiveData.Name,
                    Entries = archive.Entries.Take(archive.Entries.Length - 1).Select((x, i) => new SoundGroup.SoundGroupEntry()
                    {
                        FileName = x.FileName ?? i.ToString(),
                        RawSoundData = archive.ReadFileBytes(context, i)
                    }).ToArray(),
                    BitsPerSample = archiveData.BitsPerSample
                });
            }

            return output;
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
                new GameAction("Export Sound", false, true, (input, output) => ExtractSoundAsync(settings, output)),
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
                new GameAction("Log Archive Files", false, false, (input, output) => LogArchives(settings)),
                new GameAction("Export ETA Info", false, true, (input, output) => ExportETAInfo(settings, output, false)),
                new GameAction("Export ETA Info (extended)", false, true, (input, output) => ExportETAInfo(settings, output, true)),
            };
        }

        /// <summary>
        /// Extracts the sound data
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The output path</param>
        public async UniTask ExtractSoundAsync(GameSettings settings, string outputPath)
        {
            // Create a new context
            using (var context = new Context(Settings.GetGameSettings))
            {
                // Handle every sound group
                foreach (var soundGroup in await GetSoundGroupsAsync(context))
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
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = archiveFile.FilePath
                    });

                    // Get the output directory
                    var output = Path.Combine(outputPath, Path.GetDirectoryName(archiveFile.FilePath), Path.GetFileNameWithoutExtension(archiveFile.FilePath));

                    // Read archive
                    var archive = FileFactory.Read<R1_PC_EncryptedFileArchive>(archiveFile.FilePath, context);

                    // Extract every file
                    for (int i = 0; i < archive.Entries.Length; i++)
                        // Write the bytes
                        Util.ByteArrayToFile(Path.Combine(output, (archive.Entries[i].FileName ?? i.ToString())), archive.ReadFileBytes(context, i));
                }
            }
        }

        public void ExportPaletteImage(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                var pal = new List<RGB666Color[]>();

                // Enumerate every world
                foreach (var world in GetLevels(settings).First().Worlds)
                {
                    settings.World = world.Index;

                    // Enumerate every level
                    foreach (var lvl in world.Maps)
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
                        var lvlData = FileFactory.Read<R1_PC_LevFile>(path, context);

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
        public void ImportRawImageData(R1_PC_DES des, IEnumerable<KeyValuePair<int, byte[]>> rawImageData)
        {
            // TODO: Clean this up

            // Import every image data
            foreach (var data in rawImageData)
            {
                // Get the descriptor
                var imgDesc = des.ImageDescriptors[data.Key];

                // Add every byte and encrypt it
                for (int i = 0; i < data.Value.Length; i++)
                    des.ImageData[imgDesc.ImageBufferOffset + i] = data.Value[i];
            }

            // TODO: Move the reverse image processing to its own method
            int flag = -1;

            // Process every byte
            for (int i = des.ImageData.Length - 1; i >= 0; i--)
            {
                // Get the decrypted value
                var val = des.ImageData[i];

                // Check if it should be transparent
                if (val == 0)
                {
                    if (flag == -1)
                        flag = 0xA1;
                    else
                        flag++;

                    if (flag > 0xFF)
                        flag = 0xFF;

                    des.ImageData[i] = (byte)flag;
                }
                else
                {
                    flag = -1;
                }
            }
        }

        /// <summary>
        /// Loads the sprites for the level
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="palette">The palette to use</param>
        /// <returns>The common event designs</returns>
        public async UniTask<Unity_ObjectManager_R1.DESData[]> LoadSpritesAsync(Context context, IList<ARGBColor> palette)
        {
            // Create the output list
            List<Unity_ObjectManager_R1.DESData> eventDesigns = new List<Unity_ObjectManager_R1.DESData>();

            Controller.DetailedState = $"Loading allfix";

            // Read the fixed data
            var allfix = FileFactory.Read<R1_PC_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading world";

            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading big ray";

            // NOTE: This is not loaded into normal levels and is purely loaded here so the animation can be viewed!
            // Read the big ray data
            var bigRayData = FileFactory.Read<R1_PC_BigRayFile>(GetBigRayFilePath(context.Settings), context);

            // Get the big ray palette
            var bigRayPalette = GetBigRayPalette(context);

            await Controller.WaitIfNecessary();

            // Get the DES and ETA
            var des = allfix.DesItems.Concat(worldData.DesItems).Concat(bigRayData.DesItems).ToArray();

            int desIndex = 0;

            // Add dummy DES to index 0
            eventDesigns.Add(new Unity_ObjectManager_R1.DESData(new Unity_ObjGraphics(), null));

            // Read every DES item
            foreach (R1_PC_DES d in des)
            {
                Controller.DetailedState = $"Loading DES {desIndex}/{des.Length}";

                await Controller.WaitIfNecessary();

                // Use big ray palette for last one
                var p = desIndex == des.Length - 1 && bigRayPalette != null ? bigRayPalette : palette;

                // Add to the designs
                eventDesigns.Add(new Unity_ObjectManager_R1.DESData(GetCommonDesign(context, d, p, desIndex), d.ImageDescriptors));

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
        /// <returns>The level</returns>
        public virtual async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading map data";

            // Read the level data
            var levelData = FileFactory.Read<R1_PC_LevFile>(GetLevelFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context, levelData.MapData.ColorPalettes.First()) : new Unity_ObjectManager_R1.DESData[0];

            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            var bigRayName = Path.GetFileNameWithoutExtension(GetBigRayFilePath(context.Settings));

            var des = eventDesigns.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<Unity_ObjectManager_R1.DESData>(x, i, i == eventDesigns.Length - 1 ? bigRayName : worldData.DESFileNames?.ElementAtOrDefault(i))).ToArray();
            var allEta = GetCurrentEventStates(context).ToArray();
            var eta = allEta.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<R1_EventState[][]>(x.States, i, i == allEta.Length - 1 ? bigRayName : worldData.ETAFileNames?.ElementAtOrDefault(i))).ToArray();

            // Load ZDC (collision) and event flags
            var typeZDCBytes = GetTypeZDCBytes;
            var zdcTableBytes = GetZDCTableBytes;
            var eventFlagsBytes = GetEventFlagsBytes;

            R1_ZDCEntry[] typeZDC = context.Deserializer.SerializeFromBytes<ObjectArray<R1_ZDCEntry>>(typeZDCBytes, "TypeZDC", x => x.Length = (uint)(typeZDCBytes.Length / 2), name: "TypeZDC").Value;
            R1_ZDCData[] zdcData = context.Deserializer.SerializeFromBytes<ObjectArray<R1_ZDCData>>(zdcTableBytes, "ZDCTable", x => x.Length = (uint)(zdcTableBytes.Length / 8), name: "ZDCTable").Value;
            R1_EventFlags[] eventFlags = context.Deserializer.SerializeFromBytes<Array<R1_EventFlags>>(eventFlagsBytes, "EventFlags", x => x.Length = (uint)(eventFlagsBytes.Length / 4), name: "EventFlags").Value;

            // Create the object manager
            var objManager = new Unity_ObjectManager_R1(context, des, eta, levelData.EventData.EventLinkingTable, 
                usesPointers: false, 
                typeZDC: typeZDC, 
                zdcData: zdcData,
                eventFlags: eventFlags);

            // Create the maps
            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    // Set the dimensions
                    Width = levelData.MapData.Width,
                    Height = levelData.MapData.Height,

                    // Create the tile arrays
                    TileSet = new Unity_MapTileMap[3],
                    MapTiles = levelData.MapData.Tiles.Select(x => new Unity_Tile(x)).ToArray(),
                    TileSetWidth = 1,

                    TileSetTransparencyModes = levelData.TileTextureData.TexturesOffsetTable.Select(x => levelData.TileTextureData.NonTransparentTextures.Concat(levelData.TileTextureData.TransparentTextures).FirstOrDefault(t => t.Offset == x)).Select(x =>
                    {
                        if (x == null)
                            return R1_PC_MapTileTransparencyMode.FullyTransparent;

                        if (x.TransparencyMode == 0xAAAAAAAA)
                            return R1_PC_MapTileTransparencyMode.FullyTransparent;

                        if (x.TransparencyMode == 0x55555555)
                            return R1_PC_MapTileTransparencyMode.NoTransparency;

                        return R1_PC_MapTileTransparencyMode.PartiallyTransparent;
                    }).ToArray(),
                    PCTileOffsetTable = levelData.TileTextureData.TexturesOffsetTable
                }
            };

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            // Load the localization
            var loc = await LoadLocalizationAsync(context);

            Controller.DetailedState = $"Loading events";
            await Controller.WaitIfNecessary();

            // Load Rayman
            var rayman = new Unity_Object_R1(R1_EventData.GetRayman(levelData.EventData.Events.FirstOrDefault(x => x.Type == R1_EventType.TYPE_RAY_POS)), objManager);

            // Load background vignette textures
            var bg = await LoadBackgroundVignetteAsync(context, worldData, levelData, false);
            var bg2 = await LoadBackgroundVignetteAsync(context, worldData, levelData, true);

            // Create a level object
            Unity_Level level = new Unity_Level(
                maps: maps, 
                objManager: objManager, 
                rayman: rayman, 
                localization: loc,
                background: bg,
                parallaxBackground: bg2);

            for (var i = 0; i < levelData.EventData.Events.Length; i++)
            {
                R1_EventData e = levelData.EventData.Events[i];

                e.Commands = levelData.EventData.EventCommands[i].Commands;
                e.LabelOffsets = levelData.EventData.EventCommands[i].LabelOffsetTable;

                // Add the event
                level.EventData.Add(new Unity_Object_R1(e, objManager));
            }

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading tile set";

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            level.Maps[0].TileSet[0] = tileSets[0];
            level.Maps[0].TileSet[1] = tileSets[1];
            level.Maps[0].TileSet[2] = tileSets[2];

            // Enumerate each cell
            for (int cellY = 0; cellY < levelData.MapData.Height; cellY++) 
            {
                for (int cellX = 0; cellX < levelData.MapData.Width; cellX++) 
                {
                    // Get the cell
                    var cell = levelData.MapData.Tiles[cellY * levelData.MapData.Width + cellX];

                    // Set the common tile
                    level.Maps[0].MapTiles[cellY * levelData.MapData.Width + cellX] = new Unity_Tile(cell);
                }
            }

            // Return the level
            return level;
        }

        public abstract byte[] GetTypeZDCBytes { get; }
        public abstract byte[] GetZDCTableBytes { get; }
        public abstract byte[] GetEventFlagsBytes { get; }

        public abstract UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, R1_PC_WorldFile world, R1_PC_LevFile level, bool parallax);

        protected abstract UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context);

        /// <summary>
        /// Reads 3 tile-sets, one for each palette
        /// </summary>
        /// <param name="levData">The level data to get the tile-set for</param>
        /// <returns>The 3 tile-sets</returns>
        public Unity_MapTileMap[] ReadTileSets(R1_PC_LevFile levData) {
            // Create the output array
            var output = new Unity_MapTileMap[]
            {
                new Unity_MapTileMap(new Unity_TileTexture[levData.TileTextureData.TexturesOffsetTable.Length]),
                new Unity_MapTileMap(new Unity_TileTexture[levData.TileTextureData.TexturesOffsetTable.Length]),
                new Unity_MapTileMap(new Unity_TileTexture[levData.TileTextureData.TexturesOffsetTable.Length])
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
                    var tileTexture = TextureHelpers.CreateTexture2D(Settings.CellSize, Settings.CellSize);

                    // Keep track if all pixels are red (transparent tile in RayKit)
                    bool allRed = true;

                    // Write each pixel to the texture
                    for (int y = 0; y < Settings.CellSize; y++)
                    {
                        for (int x = 0; x < Settings.CellSize; x++)
                        {
                            // Get the index
                            var cellIndex = Settings.CellSize * y + x;

                            // Get the color from the current palette (or default to fully transparent if a valid tile texture was not found or it has the transparency flag)
                            var c = tileTex == null || index == 0 ? new Color(0, 0, 0, 0) : levData.MapData.ColorPalettes[i][255 - tileTex.ColorIndexes[cellIndex]].GetColor();

                            if (tileTex != null && tileTex.ColorIndexes[cellIndex] != 242)
                                allRed = false;

                            // If the texture is transparent, add the alpha channel
                            if (tileTex is R1_PC_TransparentTileTexture tt)
                                c.a = (float)tt.Alpha[cellIndex] / Byte.MaxValue;

                            // Set the pixel
                            tileTexture.SetPixel(x, y, c);
                        }
                    }

                    // If all red, make it transparent
                    if (allRed)
                        tileTexture.SetPixels(Enumerable.Repeat(new Color(), Settings.CellSize * Settings.CellSize).ToArray());

                    // Apply the pixels to the texture
                    tileTexture.Apply();

                    // Create and set up the tile
                    output[i].Tiles[index] = tileTexture.CreateTile();
                }

                index++;
            }

            return output;
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="level">The level</param>
        public void SaveLevel(Context context, Unity_Level level) 
        {
            // Get the object manager
            var objManager = (Unity_ObjectManager_R1)level.ObjManager;

            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<R1_PC_LevFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++) {
                for (int x = 0; x < lvlData.MapData.Width; x++) {
                    // Set the tiles
                    lvlData.MapData.Tiles[y * lvlData.MapData.Width + x] = level.Maps[0].MapTiles[y * lvlData.MapData.Width + x].Data;
                }
            }

            // Temporary event lists
            var events = new List<R1_EventData>();
            var eventCommands = new List<R1_PC_EventCommand>();

            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Get file names if available
            var desNames = worldData.DESFileNames ?? new string[0];
            var etaNames = worldData.ETAFileNames ?? new string[0];

            foreach (var e in level.EventData.Cast<Unity_Object_R1>())
            {
                var r1Event = e.EventData;

                if (r1Event.PS1Demo_Unk1 == null)
                    r1Event.PS1Demo_Unk1 = new byte[40];

                if (r1Event.Unk_98 == null)
                    r1Event.Unk_98 = new byte[5];

                r1Event.ImageDescriptorCount = (ushort)objManager.DES[e.DESIndex].Data.ImageDescriptors.Length;
                r1Event.AnimDescriptorCount = (byte)objManager.DES[e.DESIndex].Data.Graphics.Animations.Count;

                // Add the event
                events.Add(r1Event);

                // Add the event commands
                eventCommands.Add(new R1_PC_EventCommand()
                {
                    CommandLength = (ushort)(e.EventData.Commands.Commands.Select(x => x.Length).Sum()),
                    Commands = e.EventData.Commands,
                    LabelOffsetCount = (ushort)e.EventData.LabelOffsets.Length,
                    LabelOffsetTable = e.EventData.LabelOffsets
                });
            }

            // Update event values
            lvlData.EventData.EventCount = (ushort)events.Count;
            lvlData.EventData.Events = events.ToArray();
            lvlData.EventData.EventCommands = eventCommands.ToArray();

            // Save the file
            FileFactory.Write<R1_PC_LevFile>(lvlPath, context);
        }

        public virtual async UniTask LoadFilesAsync(Context context)
        {
            // Allfix
            await AddFile(context, GetAllfixFilePath(context.Settings));

            // World
            await AddFile(context, GetWorldFilePath(context.Settings));

            // Level
            await AddFile(context, GetLevelFilePath(context.Settings));

            // BigRay
            await AddFile(context, GetBigRayFilePath(context.Settings));

            // Vignette
            await AddFile(context, GetVignetteFilePath(context.Settings));
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
            for (int world = 1; world < 7; world++)
            {
                // Set the world
                context.Settings.World = world;

                // Add world file
                context.AddFile(GetFile(context, GetWorldFilePath(context.Settings)));

                // Add every level
                foreach (var lvl in GetLevels(context.Settings).First(x => x.Name == context.Settings.EduVolume || x.Name == null).Worlds.FirstOrDefault(x => x.Index == world)?.Maps ?? new int[0])
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

        public async UniTask AddFile(Context context, string filePath, bool isBigFile = false)
        {
            if (isBigFile)
                await FileSystem.PrepareBigFile(context.BasePath + filePath, 8);
            else
                await FileSystem.PrepareFile(context.BasePath + filePath);

            if (!FileSystem.FileExists(context.BasePath + filePath))
                return;

            context.AddFile(GetFile(context, filePath));
        }

        /// <summary>
        /// Gets the event states for the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The event states</returns>
        public virtual IEnumerable<R1_PC_ETA> GetCurrentEventStates(Context context)
        {
            // Read the fixed data
            var allfix = FileFactory.Read<R1_PC_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Read the big ray data
            var bigRayData = FileFactory.Read<R1_PC_BigRayFile>(GetBigRayFilePath(context.Settings), context);

            // Get the eta items
            return allfix.Eta.Concat(worldData.Eta).Concat(bigRayData.Eta);
        }

        public void LogArchives(GameSettings settings)
        {
            using (var context = new Context(settings))
            {
                // Load every archive
                var archives = GetArchiveFiles(settings).
                    Where(x => File.Exists(context.BasePath + x.FilePath)).
                    Select(archive =>
                    {
                        context.AddFile(new LinearSerializedFile(context)
                        {
                            filePath = archive.FilePath
                        });
                        return new
                        {
                            Archive = FileFactory.Read<R1_PC_EncryptedFileArchive>(archive.FilePath, context),
                            Volume = archive.Volume
                        };
                    }).
                    ToArray();

                // Helper methods for loading an archive file
                void LogFile<T>(R1_PC_ArchiveFileName fileName)
                    where T : R1Serializable, new()
                {
                    // Log each file
                    foreach (var archive in archives)
                    {
                        var index = archive.Archive.Entries.FindItemIndex(x => x.FileName == fileName.ToString());
                        context.Settings.EduVolume = archive.Volume;
                        archive.Archive.ReadFile<T>(context, index);
                    }
                }

                // Read all known files
                LogFile<R1_PC_VersionFile>(R1_PC_ArchiveFileName.VERSION);
                //LogFile<>("SCRIPT"); // TODO: Serialize script
                LogFile<R1_PC_GeneralFile>(R1_PC_ArchiveFileName.GENERAL);
                LogFile<R1_PC_GeneralFile>(R1_PC_ArchiveFileName.GENERAL0);
                LogFile<R1_PCEdu_MOTFile>(R1_PC_ArchiveFileName.MOT);
                LogFile<R1_PC_SampleNamesFile>(R1_PC_ArchiveFileName.SMPNAMES);
                LogFile<R1_PC_LocFile>(R1_PC_ArchiveFileName.TEXT);
                LogFile<R1_PC_WorldMap>(R1_PC_ArchiveFileName.WLDMAP01);
            }
        }

        public void ExportETAInfo(GameSettings settings, string outputDir, bool includeStates)
        {
            /*
            using (var context = new Context(settings))
            {
                AddAllFiles(context);

                var output = new List<KeyValuePair<string, ETAInfo[]>>();
                var events = LevelEditorData.EventInfoData;
                BaseEditorManager editor;

                if (this is R1_PC_Manager)
                    editor = new R1_PC_EditorManager(new Unity_Level(), context, this, new Unity_ObjGraphics[0]);
                else if (this is R1_Kit_Manager rd)
                    editor = new R1_Kit_EditorManager(new Unity_Level(), context, rd, new Unity_ObjGraphics[0]);
                else if (this is R1_PCEdu_Manager)
                    editor = new R1_EDU_EditorManager(new Unity_Level(), context, this, new Unity_ObjGraphics[0]);
                else
                    throw new Exception("PC version is not supported for this operation");

                void AddToOutput(string name, R1_PC_BaseWorldFile world, int baseIndex)
                {
                    // Read the world data and get the ETA file names
                    var fileNames = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context).ETAFileNames;

                    var availableEvents = events.Where(x => editor.IsAvailableInWorld(x)).ToArray();

                    output.Add(new KeyValuePair<string, ETAInfo[]>(name, world.Eta.Select((x, i) =>
                    {
                        var index = baseIndex + i;
                        var fileName = fileNames?.ElementAtOrDefault(index) ?? String.Empty;
                        var key = String.IsNullOrWhiteSpace(fileName) ? index.ToString() : fileName;

                        return new ETAInfo()
                        {
                            GlobalIndex = index,
                            FileName = fileName,
                            SubEtatLengths = x.States.Select(s => s.Length).ToArray(),
                            Events = String.Join(", ", availableEvents.Where(e => editor.GetEtaKey(e) == key).Select(e => e.Name)),
                            EventStates = !includeStates ? null : x.States.Select(s => s.Select(se => new ETAInfo.StateInfo
                            {
                                RightSpeed = se.RightSpeed,
                                LeftSpeed = se.LeftSpeed,
                                AnimationIndex = se.AnimationIndex,
                                AnimationSpeed = se.AnimationSpeed,
                                LinkedEtat = se.LinkedEtat,
                                LinkedSubEtat = se.LinkedSubEtat,
                                SoundIndex = se.SoundIndex,
                                InteractionType = se.InteractionType
                            }).ToArray()).ToArray()
                        };
                    }).ToArray()));
                }

                var allfix = FileFactory.Read<R1_PC_AllfixFile>(GetAllfixFilePath(context.Settings), context);
                AddToOutput("Allfix", allfix, 0);

                for (int w = 1; w < 7; w++)
                {
                    context.Settings.World = w;
                    AddToOutput(w.ToString(), FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context), allfix.Eta.Length);
                }

                JsonHelpers.SerializeToFile(output, Path.Combine(outputDir, $"ETA{(includeStates ? "ex" : String.Empty)} - {context.Settings.GameModeSelection}.json"), NullValueHandling.Ignore);
            }*/
        }

        public T LoadArchiveFile<T>(Context context, string archivePath, string fileName)
            where T : R1Serializable, new()
        {
            if (context.MemoryMap.Files.All(x => x.filePath != archivePath))
                return null;

            return FileFactory.Read<R1_PC_EncryptedFileArchive>(archivePath, context).ReadFile<T>(context, fileName);
        }

        public T LoadArchiveFile<T>(Context context, string archivePath, R1_PC_ArchiveFileName fileName)
            where T : R1Serializable, new() => LoadArchiveFile<T>(context, archivePath, fileName.ToString());
        public T LoadArchiveFile<T>(Context context, string archivePath, int fileIndex)
            where T : R1Serializable, new()
        {
            if (context.MemoryMap.Files.All(x => x.filePath != archivePath))
                return null;

            return FileFactory.Read<R1_PC_EncryptedFileArchive>(archivePath, context).ReadFile<T>(context, fileIndex);
        }

        #endregion

        #region Classes

        protected class ETAInfo
        {
            public string FileName { get; set; }

            public int GlobalIndex { get; set; }

            public string Events { get; set; }

            public int[] SubEtatLengths { get; set; }

            public StateInfo[][] EventStates { get; set; }

            public class StateInfo
            {
                public sbyte RightSpeed { get; set; }
                public sbyte LeftSpeed { get; set; }

                public byte AnimationIndex { get; set; }
                public byte AnimationSpeed { get; set; }

                public byte LinkedEtat { get; set; }
                public byte LinkedSubEtat { get; set; }

                public byte SoundIndex { get; set; }
                public byte InteractionType { get; set; }
            }
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
            public AdditionalSoundArchive(string name, string archiveFile, int bitsPerSample = 8)
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
            public string ArchiveFile { get; }

            /// <summary>
            /// The bits per sample
            /// </summary>
            public int BitsPerSample { get; }
        }

        public class Archive
        {
            public Archive(string filePath, string volume = null)
            {
                FilePath = filePath;
                Volume = volume;
            }

            public string FilePath { get; }
            public string Volume { get; }
        }

        public enum R1_PC_ArchiveFileName
        {
            VERSION,
            SCRIPT,
            GENERAL,
            GENERAL0, // French KIT only
            MOT,
            SMPNAMES,
            TEXT,
            WLDMAP01
        }

        #endregion
    }
}