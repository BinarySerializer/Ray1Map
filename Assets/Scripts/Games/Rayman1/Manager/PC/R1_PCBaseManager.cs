using BinarySerializer;
using BinarySerializer.Audio;
using BinarySerializer.Image;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PC;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Sprite = BinarySerializer.Ray1.Sprite;
using Animation = BinarySerializer.Ray1.Animation;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// Base game manager for PC
    /// </summary>
    public abstract class R1_PCBaseManager : R1_BaseMultiplatformManager
    {
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

        public virtual string[] GetMoviePaths => new string[]
        {
            "INTRO.DAT",
            "CONCLU.DAT",
        };

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public abstract string GetWorldFilePath(GameSettings settings);

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

        public bool IsDESMultiColored(Context context, int desIndex, GeneralEventInfoData[] generalEvents)
        {
            // Hacky fix for French with Rayman
            if ((context.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu || context.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_Edu) &&
                (context.GetR1Settings().EduVolume[1] == 'F' || context.GetR1Settings().EduVolume[1] == 'f') && 
                (context.GetR1Settings().R1_World == World.Jungle || context.GetR1Settings().R1_World == World.Cake))
                desIndex -= 4;

            var name = GetDESNameTable(context).ElementAtOrDefault(desIndex);

            return generalEvents.Any(x => (x.DES == name && x.Worlds.Contains(context.GetR1Settings().R1_World)) && ((ObjType)x.Type).IsMultiColored());
        }

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
            using (var context = new Ray1MapContext(settings))
            {
                context.AddFile(new LinearFile(context, vigPath));

                // Read the archive
                var archive = FileFactory.Read<FileArchive>(context, vigPath);

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
                            using (Context c = new Ray1MapContext(Settings.GetGameSettings)) {
                                c.AddFile(new StreamFile(c, "pcx", stream));
                                var pcx = FileFactory.Read<PCX>(c, "pcx");

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
            using (Context context = new Ray1MapContext(settings)) {
                // Add all files
                AddAllFiles(context);

                // Load the event info data
                var eventInfo = LevelEditorData.EventInfoData;

                // Get the DES names for every world
                var desNames = WorldHelpers.EnumerateWorlds().ToDictionary(x => x, world => {
                    // Set the world
                    context.GetR1Settings().R1_World = world;
                    context.GetRequiredSettings<Ray1Settings>().World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.GetR1Settings());

                    if (!FileSystem.FileExists(context.GetAbsoluteFilePath(worldPath)))
                        return null;

                    // TODO: Update this to not include extensions
                    var a = FileFactory.Read<WorldFile>(context, worldPath).DESFileNames?.Skip(1).ToArray();

                    return a?.Any() == true ? a : null;
                });

                // Get the ETA names for every world
                var etaNames = WorldHelpers.EnumerateWorlds().ToDictionary(x => x, world => {
                    // Set the world
                    context.GetR1Settings().R1_World = world;
                    context.GetRequiredSettings<Ray1Settings>().World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.GetR1Settings());

                    if (!FileSystem.FileExists(context.GetAbsoluteFilePath(worldPath)))
                        return null;

                    var a = FileFactory.Read<WorldFile>(context, worldPath).ETAFileNames?.ToArray();

                    return a?.Any() == true ? a : null;
                });

                context.GetR1Settings().R1_World = World.Jungle;
                context.GetRequiredSettings<Ray1Settings>().World = World.Jungle;

                // Keep track of Rayman's anim
                Animation[] rayAnim = null;

                // Helper method for exporting textures
                async UniTask<Wld> ExportTexturesAsync<Wld>(string filePath, string name, int desOffset, IEnumerable<States> baseEta, string[] desFileNames, string[] etaFileNames, IList<SerializableColor> palette = null)
                    where Wld : BaseWorldFile, new()
                {
                    // Read the file
                    var file = FileFactory.Read<Wld>(context, filePath);

                    if (rayAnim == null && file is AllfixFile)
                        // Rayman is always the first DES
                        rayAnim = file.DesItems.First().Animations;

                    // Export the sprite textures
                    if (exportAnimFrames)
                        await ExportAnimationFramesAsync(context, file, Path.Combine(outputDir, name), desOffset, baseEta.Concat(file.Eta).ToArray(), desFileNames, etaFileNames, eventInfo, rayAnim, palette);
                    else
                        ExportSpriteTextures(context, file, Path.Combine(outputDir, name), desOffset, desFileNames, palette);

                    return file;
                }

                // Export big ray
                await ExportTexturesAsync<BigRayFile>(GetBigRayFilePath(context.GetR1Settings()), "Bigray", 0, new States[0], null, null, GetBigRayPalette(context));

                // Export allfix
                var allfix = await ExportTexturesAsync<AllfixFile>(GetAllfixFilePath(context.GetR1Settings()), "Allfix", 0, new States[0], desNames.Values.FirstOrDefault(), etaNames.Values.FirstOrDefault());

                // Enumerate every world
                foreach (World world in WorldHelpers.EnumerateWorlds()) {
                    // Set the world
                    context.GetR1Settings().R1_World = world;
                    context.GetRequiredSettings<Ray1Settings>().World = world;

                    // Get the world file path
                    var worldPath = GetWorldFilePath(context.GetR1Settings());

                    if (!FileSystem.FileExists(context.GetAbsoluteFilePath(worldPath)))
                        continue;

                    // Export world
                    await ExportTexturesAsync<WorldFile>(worldPath, world.ToString(), allfix.DesItems.Length, allfix.Eta, desNames.TryGetItem(world), etaNames.TryGetItem(world));
                }
            }
        }

        /// <summary>
        /// Gets the big ray palette if available
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The big ray palette</returns>
        public virtual IList<SerializableColor> GetBigRayPalette(Context context) => null;

        /// <summary>
        /// Exports all sprite textures from the world file to the specified output directory
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="worldFile">The world file</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="desOffset">The amount of textures in the allfix to use as the DES offset if a world texture</param>
        /// <param name="desNames">The DES names, if available</param>
        /// <param name="palette">Optional palette to use</param>
        public void ExportSpriteTextures(Context context, BaseWorldFile worldFile, string outputDir, int desOffset, string[] desNames, IList<SerializableColor> palette = null) {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<LevelFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.GetR1Settings()).First(x => x.Name == context.GetR1Settings().EduVolume || x.Name == null).Worlds.First(x => x.Index == context.GetR1Settings().World).Maps.OrderBy(x => x)) {
                // Set the level number
                context.GetR1Settings().Level = i;
                context.GetRequiredSettings<Ray1Settings>().Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.GetR1Settings());

                // Load the level
                levels.Add(FileFactory.Read<LevelFile>(context, lvlPath));
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
        public Texture2D[] GetSpriteTextures(List<LevelFile> levels, Design desItem, int desIndex, IList<SerializableColor> palette = null)
        {
            // Create the output array
            var output = new Texture2D[desItem.Sprites.Length];

            // Process the image data
            var processedImageData = desItem.IsAnimatedSprite ? Design.ProcessImageData(desItem.ImageData) : desItem.ImageData;

            // Find the level with the correct palette
            var lvl = levels.FindLast(x => x.ScrollDiffSprites == desIndex || x.ObjData.Objects.Any(y => y.PCPacked_SpritesIndex == desIndex)) ?? levels.First();

            // Enumerate each image
            for (int i = 0; i < desItem.Sprites.Length; i++)
            {
                // Get the image descriptor
                var imgDescriptor = desItem.Sprites[i];

                // Ignore dummy sprites
                if (imgDescriptor.IsDummySprite())
                    continue;

                // Get the texture
                Texture2D tex = GetSpriteTexture(imgDescriptor, palette ?? lvl.MapInfo.Palettes.First(), processedImageData);

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
        public async UniTask ExportAnimationFramesAsync(Context context, BaseWorldFile worldFile, string outputDir, int desOffset, States[] eta, string[] desNames, string[] etaNames, IList<GeneralEventInfoData> eventInfo, Animation[] rayAnim, IList<SerializableColor> palette = null)
        {
            // Create the directory
            Directory.CreateDirectory(outputDir);

            var levels = new List<LevelFile>();

            // Load the levels to get the palettes
            foreach (var i in GetLevels(context.GetR1Settings()).First(x => x.Name == context.GetR1Settings().EduVolume).Worlds.FindItem(x => x.Index == context.GetR1Settings().World).Maps.OrderBy(x => x))
            {
                // Set the level number
                context.GetR1Settings().Level = i;
                context.GetRequiredSettings<Ray1Settings>().Level = i;

                // Get the level file path
                var lvlPath = GetLevelFilePath(context.GetR1Settings());

                // Load the level
                levels.Add(FileFactory.Read<LevelFile>(context, lvlPath));
            }

            // Get special DES
            int? smallRayDES = null;
            int? darkRayDES = null;

            // Get the small Rayman DES if allfix
            if (worldFile is AllfixFile)
            {
                var ei = eventInfo.FindItem(x => x.Type == (int)ObjType.TYPE_DEMI_RAYMAN);

                if (context.GetR1Settings().EngineVersion == EngineVersion.R1_PC)
                    smallRayDES = LevelEditorData.NameTable_R1PCDES[0].FindItemIndex(x => x == ei.DES);
                else if (context.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit)
                    smallRayDES = desNames.FindItemIndex(x => ei.DES == x.Substring(0, x.Length - 4)) + 1;
                else
                    throw new NotImplementedException();
            }

            // Get the Dark Rayman DES if Cake
            if (worldFile is WorldFile && context.GetR1Settings().World == (int)World.Cake)
            {
                var ei = eventInfo.FindItem(x => x.Type == (int)ObjType.TYPE_BLACK_RAY);

                if (context.GetR1Settings().EngineVersion == EngineVersion.R1_PC)
                    darkRayDES = LevelEditorData.NameTable_R1PCDES[6 - 1].FindItemIndex(x => x == ei.DES);
                else if (context.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit)
                    darkRayDES = desNames.FindItemIndex(x => ei.DES == x.Substring(0, x.Length - 4)) + 1;
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
                List<ObjState> matchingStates = new List<ObjState>();

                if (!(worldFile is BigRayFile))
                {
                    // Search level events
                    foreach (var lvlEvent in levels.SelectMany(x => x.ObjData.Objects).Where(x => x.PCPacked_SpritesIndex == desIndex))
                        matchingStates.AddRange(eta[lvlEvent.PCPacked_ETAIndex].ObjectStates.SelectMany(x => x).Where(x => !matchingStates.Contains(x)));

                    var desNameTable = GetDESNameTable(context);
                    var etaNameTable = GetETANameTable(context);

                    // Search event info
                    foreach (var ei in eventInfo)
                    {
                        States matchingEta = null;

                        if (ei.DES == desNameTable[desIndex])
                            matchingEta = eta[etaNameTable.FindItemIndex(x => x == ei.ETA)];

                        if (matchingEta != null)
                            matchingStates.AddRange(matchingEta.ObjectStates.SelectMany(x => x).Where(x => !matchingStates.Contains(x)));
                    }
                }

                // Get the textures
                var textures = GetSpriteTextures(levels, des, desIndex, palette);

                // Get the folder
                var desFolderPath = Path.Combine(outputDir, $"{i}{desName}");

                // Get the animations
                var spriteAnim = des.Animations;

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
                    if (worldFile is BigRayFile)
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

                    for (var dummy = 0; dummy < anim.LayersCount * anim.FramesCount; dummy++)
                    {
                        var l = anim.Layers[tempLayer];

                        if (l.SpriteIndex < textures.Length)
                        {
                            var s = textures[l.SpriteIndex];

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
                    for (int frameIndex = 0; frameIndex < anim.FramesCount; frameIndex++)
                    {
                        Texture2D tex = TextureHelpers.CreateTexture2D(frameWidth ?? 1, frameHeight ?? 1, clear: true);

                        bool hasLayers = false;

                        // Write each layer
                        for (var layerIndex = 0; layerIndex < anim.LayersCount; layerIndex++)
                        {
                            var animationLayer = anim.Layers[layer];

                            layer++;

                            if (animationLayer.SpriteIndex >= textures.Length)
                                continue;

                            // Get the sprite
                            var sprite = textures[animationLayer.SpriteIndex];

                            if (sprite == null)
                                continue;
                            
                            // Set every pixel
                            for (int y = 0; y < sprite.height; y++)
                            {
                                for (int x = 0; x < sprite.width; x++)
                                {
                                    var c = sprite.GetPixel(x, sprite.height - y - 1);
                                    
                                    var xPosition = (animationLayer.FlipX ? (sprite.width - 1 - x) : x) + (desIndex == smallRayDES ? animationLayer.XPosition / 2 : animationLayer.XPosition);
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
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="s">The image descriptor</param>
        /// <param name="palette">The palette to use</param>
        /// <param name="processedImageData">The processed image data to use</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(Sprite s, IList<SerializableColor> palette, byte[] processedImageData)
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
        public virtual Unity_ObjGraphics GetCommonDesign(Context context, Design des, IList<SerializableColor> palette, int desIndex)
        {
            // Check if the DES is used for multi-colored events
            var isMultiColored = IsDESMultiColored(context, desIndex + 1, LevelEditorData.EventInfoData);

            // Create the common design
            Unity_ObjGraphics graphics = new Unity_ObjGraphics
            {
                Sprites = new List<UnityEngine.Sprite>(),
                Animations = new List<Unity_ObjAnimation>()
            };

            // Process the image data
            var processedImageData = des.IsAnimatedSprite ? Design.ProcessImageData(des.ImageData) : des.ImageData;

            if (!isMultiColored)
            {
                // Sprites
                foreach (var s in des.Sprites)
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

                    p.Insert(0, SerializableColor.Black);

                    if (i % 2 != 0)
                        p[8] = palette[i * 8];

                    // Sprites
                    foreach (var s in des.Sprites)
                    {
                        // Get the texture
                        Texture2D tex = GetSpriteTexture(s, p, processedImageData);

                        // Add it to the array
                        graphics.Sprites.Add(tex == null ? null : tex.CreateSprite());
                    }
                }
            }

            // Animations
            foreach (var a in des.Animations)
                // Add the animation to list
                graphics.Animations.Add(AnimationHelpers.ToCommonAnimation(a.Layers, a.LayersCount, a.FramesCount));

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
            var soundArchive = FileFactory.Read<FileArchive>(context, soundFile);
            var soundManifestArchive = FileFactory.Read<FileArchive>(context, soundManifestFile);

            var index = 0;

            // Handle every sound group
            foreach (var soundArchiveEntry in soundArchive.Entries)
            {
                var manifestArchiveEntry = soundManifestArchive.Entries[index];

                // Read the manifest data
                var manifestData = soundManifestArchive.ReadFile<SoundManifest>(context, index, file => file.Pre_Length = manifestArchiveEntry.FileSize / (4 * 4));

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
                    s.DoProcessed(new Xor8Processor(soundArchiveEntry.XORKey), () =>
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
            foreach (var archiveData in GetAdditionalSoundArchives(context.GetR1Settings()))
            {
                if (!File.Exists(context.GetAbsoluteFilePath(archiveData.ArchiveFile)))
                    continue;

                await AddFile(context, archiveData.ArchiveFile);

                // Extract the archive
                var archive = FileFactory.Read<FileArchive>(context, archiveData.ArchiveFile);

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
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new("Export Sprites", false, true, (input, output) => ExportSpriteTexturesAsync(settings, output, false)),
                new("Export Animation Frames", false, true, (input, output) => ExportSpriteTexturesAsync(settings, output, true)),
                new("Export Vignette", false, true, (input, output) => ExtractVignette(settings, GetVignetteFilePath(settings), output)),
                new("Export Archives", false, true, (input, output) => ExtractArchives(output)),
                new("Export Sound", false, true, (input, output) => ExtractSoundAsync(settings, output)),
                new("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
                new("Log Archive Files", false, false, (input, output) => LogArchives(settings)),
                new("Export ETA Info", false, true, (input, output) => ExportETAInfo(settings, output, false)),
                new("Export ETA Info (extended)", false, true, (input, output) => ExportETAInfo(settings, output, true)),
                new("Export Movies", false, true, (input, output) => ExportMoviesAsync(settings, output)),
                new("Export Movie Frames", false, true, (input, output) => ExportMovieFramesAsync(settings, output)),
            }).ToArray();
        }

        /// <summary>
        /// Extracts the sound data
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The output path</param>
        public async UniTask ExtractSoundAsync(GameSettings settings, string outputPath)
        {
            // Create a new context
            using (var context = new Ray1MapContext(Settings.GetGameSettings))
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
                        var wav = new WAV();
                        var fmt = wav.Format;
                        fmt.FormatType = 1;
                        fmt.ChannelCount = 1;
                        fmt.SampleRate = 11025;
                        fmt.BitsPerSample = (ushort)soundGroup.BitsPerSample;
                        wav.Data.Data = soundGroupEntry.RawSoundData;

                        fmt.ByteRate = (fmt.SampleRate * fmt.BitsPerSample * fmt.ChannelCount) / 8;
                        fmt.BlockAlign = (ushort)((fmt.BitsPerSample * fmt.ChannelCount) / 8);

                        // Get the output path
                        var outputFilePath = Path.Combine(groupOutputDir, soundGroupEntry.FileName + ".wav");

                        // Create and open the output file
                        using (var outputStream = File.Create(outputFilePath))
                        {
                            // Create a context
                            using (var wavContext = new Ray1MapContext(settings))
                            {
                                // Create a key
                                const string wavKey = "wav";

                                // Add the file to the context
                                wavContext.AddFile(new StreamFile(wavContext, wavKey, outputStream));

                                // Write the data
                                FileFactory.Write<WAV>(wavContext, wavKey, wav);
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
            using (var context = new Ray1MapContext(Settings.GetGameSettings))
            {
                // Extract every archive file
                foreach (var archiveFile in GetArchiveFiles(context.GetR1Settings()).Where(x => File.Exists(context.GetAbsoluteFilePath(x.FilePath))))
                {
                    context.AddFile(new LinearFile(context, archiveFile.FilePath));

                    // Get the output directory
                    var output = Path.Combine(outputPath, Path.GetDirectoryName(archiveFile.FilePath), Path.GetFileNameWithoutExtension(archiveFile.FilePath));

                    // Read archive
                    var archive = FileFactory.Read<FileArchive>(context, archiveFile.FilePath);

                    // Extract every file
                    for (int i = 0; i < archive.Entries.Length; i++)
                        // Write the bytes
                        Util.ByteArrayToFile(Path.Combine(output, (archive.Entries[i].FileName ?? i.ToString())), archive.ReadFileBytes(context, i));
                }
            }
        }

        public void ExportPaletteImage(GameSettings settings, string outputPath)
        {
            using (var context = new Ray1MapContext(settings))
            {
                var pal = new List<SerializableColor[]>();

                // Enumerate every world
                foreach (var world in GetLevels(settings).First().Worlds)
                {
                    settings.World = world.Index;
                    context.GetRequiredSettings<Ray1Settings>().World = (World)world.Index;

                    // Enumerate every level
                    foreach (var lvl in world.Maps)
                    {
                        settings.Level = lvl;
                        context.GetRequiredSettings<Ray1Settings>().Level = lvl;

                        // Get the file path
                        var path = GetLevelFilePath(settings);

                        // Load the level
                        context.AddFile(new LinearFile(context, path));

                        // Read the level
                        var lvlData = FileFactory.Read<LevelFile>(context, path);

                        // Add the palettes
                        foreach (var mapPal in lvlData.MapInfo.Palettes)
                            if (!pal.Any(x => x.SequenceEqual(mapPal)))
                                pal.Add(mapPal);
                    }
                }

                // Export
                PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), pal.SelectMany(x => x).ToArray(), optionalWrap: 256);
            }
        }

        public async UniTask ExportMoviesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                foreach (var moviePath in GetMoviePaths)
                {
                    await context.AddLinearFileAsync(moviePath);

                    var flc = FileFactory.Read<FLIC>(context, moviePath);

                    flc.Speed = 83;

                    // Export
                    using var collection = flc.ToMagickImageCollection();
                    collection.Write(Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(moviePath)}.gif"));
                }
            }
        }

        public async UniTask ExportMovieFramesAsync(GameSettings settings, string outputDir)
        {
            using var context = new Ray1MapContext(settings);
            
            foreach (var moviePath in GetMoviePaths)
            {
                await context.AddLinearFileAsync(moviePath);

                var flc = FileFactory.Read<FLIC>(context, moviePath);

                flc.Speed = 83;

                int i = 0;
                foreach (Texture2D frame in flc.EnumerateFrames())
                {
                    string filePath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(moviePath), $"{i}.png");
                    Util.ByteArrayToFile(filePath, frame.EncodeToPNG());

                    i++;
                }
            }
        }

        /// <summary>
        /// Imports raw image data into a DES
        /// </summary>
        /// <param name="des">The DES item</param>
        /// <param name="rawImageData">The raw image data, categorized by image descriptor</param>
        public void ImportRawImageData(Design des, IEnumerable<KeyValuePair<int, byte[]>> rawImageData)
        {
            // TODO: Clean this up

            // Import every image data
            foreach (var data in rawImageData)
            {
                // Get the descriptor
                var imgDesc = des.Sprites[data.Key];

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
        public async UniTask<Unity_ObjectManager_R1.DESData[]> LoadSpritesAsync(Context context, IList<SerializableColor> palette)
        {
            // Create the output list
            List<Unity_ObjectManager_R1.DESData> eventDesigns = new List<Unity_ObjectManager_R1.DESData>();

            Controller.DetailedState = $"Loading allfix";

            // Read the fixed data
            var allfix = FileFactory.Read<AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings()));

            await Controller.WaitIfNecessary();

            IList<SerializableColor> bigRayPalette;
            Design[] des;

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                Controller.DetailedState = $"Loading world";

                // Read the world data
                var worldData = FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

                await Controller.WaitIfNecessary();

                Controller.DetailedState = $"Loading big ray";

                // NOTE: This is not loaded into normal levels and is purely loaded here so the animation can be viewed!
                // Read the big ray data
                var bigRayData = FileFactory.Read<BigRayFile>(context, GetBigRayFilePath(context.GetR1Settings()));

                // Get the big ray palette
                bigRayPalette = GetBigRayPalette(context);

                await Controller.WaitIfNecessary();

                // Get the DES
                des = allfix.DesItems.Concat(worldData.DesItems).Concat(bigRayData.DesItems).ToArray();
            }
            else
            {
                bigRayPalette = null;

                // Get the DES
                des = allfix.DesItems;
            }

            int desIndex = 0;

            // Add dummy DES to index 0
            eventDesigns.Add(new Unity_ObjectManager_R1.DESData(new Unity_ObjGraphics(), null));

            // Read every DES item
            foreach (Design d in des)
            {
                Controller.DetailedState = $"Loading DES {desIndex}/{des.Length}";

                await Controller.WaitIfNecessary();

                // Use big ray palette for last one
                var p = desIndex == des.Length - 1 && bigRayPalette != null ? bigRayPalette : palette;

                // Add to the designs
                eventDesigns.Add(new Unity_ObjectManager_R1.DESData(GetCommonDesign(context, d, p, desIndex), d.Sprites));

                desIndex++;
            }

            // Return the sprites
            return eventDesigns.ToArray();
        }

        public abstract string[] GetDESNameTable(Context context);
        public abstract string[] GetETANameTable(Context context);

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = $"Loading map data";

            MapInfo mapData;
            LevelObjects objData;
            SerializableColor[][] palettes;
            NormalBlockTextures tileTextureData = null;
            Texture2D bg;
            Texture2D bg2;
            WorldInfo[] worldInfos = null;

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                // Read the level data
                var levelData = FileFactory.Read<LevelFile>(context, GetLevelFilePath(context.GetR1Settings()));

                // Read the world data
                var worldData = FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

                mapData = levelData.MapInfo;
                objData = levelData.ObjData;
                palettes = mapData.Palettes;
                tileTextureData = levelData.NormalBlockTextures;

                // Load background vignette textures
                bg = await LoadBackgroundVignetteAsync(context, worldData, levelData, false);
                bg2 = await LoadBackgroundVignetteAsync(context, worldData, levelData, true);
            }
            else
            {
                // Load the vignette
                var vig = await GetWorldMapVigAsync(context);
                bg = vig.ToTexture(true);
                bg2 = null;
                palettes = new SerializableColor[][] { vig.VGAPalette };

                // Set empty map data
                var width = (ushort)(vig.ImageWidth / Settings.CellSize);
                var height = (ushort)(vig.ImageHeight / Settings.CellSize);

                mapData = new MapInfo()
                {
                    Width = width,
                    Height = height,
                    Blocks = Enumerable.Repeat(new Block(), width * height).ToArray()
                };

                // Set event data
                worldInfos = GetWorldMapInfos(context);
                var events = worldInfos.Select((x, i) => ObjData.CreateMapObj(context, x.XPosition, x.YPosition, i, context.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu || context.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_Edu || context.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit ? x.Type : WorldInfo.PelletType.Normal)).ToArray();
                objData = new LevelObjects()
                {
                    Objects = events,
                    ObjLinkingTable = Enumerable.Range(0, events.Length).Select(x => (ushort)x).ToArray()
                };
            }

            await Controller.WaitIfNecessary();

            // Load the sprites
            var eventDesigns = await LoadSpritesAsync(context, palettes.First());

            var bigRayName = Path.GetFileNameWithoutExtension(GetBigRayFilePath(context.GetR1Settings()));

            var desNameTable = GetDESNameTable(context);
            var etaNameTable = GetETANameTable(context);

            var des = eventDesigns.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<Unity_ObjectManager_R1.DESData>(x, i, i == eventDesigns.Length - 1 ? bigRayName : desNameTable?.ElementAtOrDefault(i))).ToArray();
            var allEta = GetCurrentEventStates(context).ToArray();
            var eta = allEta.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<ObjState[][]>(x.ObjectStates, i, i == allEta.Length - 1 ? bigRayName : etaNameTable?.ElementAtOrDefault(i))).ToArray();

            // Load ZDC (collision) and event flags
            var typeZDCBytes = GetTypeZDCBytes;
            var zdcTableBytes = GetZDCTableBytes;
            var eventFlagsBytes = GetEventFlagsBytes;

            ZDCReference[] typeZDC = context.Deserializer.SerializeFromBytes<ObjectArray<ZDCReference>>(typeZDCBytes, "TypeZDC", x => x.Pre_Length = (uint)(typeZDCBytes.Length / 2), name: "TypeZDC").Value;
            ZDCBox[] zdcData = context.Deserializer.SerializeFromBytes<ObjectArray<ZDCBox>>(zdcTableBytes, "ZDCTable", x => x.Pre_Length = (uint)(zdcTableBytes.Length / 8), name: "ZDCTable").Value;
            ObjTypeFlags[] eventFlags = context.Deserializer.SerializeFromBytes<ObjectArray<ObjTypeFlags>>(eventFlagsBytes, "EventFlags", x => x.Pre_Length = (uint)(eventFlagsBytes.Length / 4), name: "EventFlags").Value;

            // Read the world data
            var allfix = FileFactory.Read<AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings()));

            // Create the object manager
            var objManager = new Unity_ObjectManager_R1(context, des, eta, objData.ObjLinkingTable, 
                usesPointers: false, 
                typeZDC: typeZDC, 
                zdcData: zdcData,
                eventFlags: eventFlags,
                hasDefinedDesEtaNames: true,
                eventTemplates: new Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData>()
                {
                    // TODO: Match ETA using the property in the DES
                    [Unity_ObjectManager_R1.WldObjType.Ray] = createEventDataTemplate(allfix.DESIndex_Ray, 0),
                    [Unity_ObjectManager_R1.WldObjType.RayLittle] = createEventDataTemplate(allfix.DESIndex_RayLittle, 0),
                    [Unity_ObjectManager_R1.WldObjType.MapObj] = createEventDataTemplate(allfix.DESIndex_MapObj, 2),
                    [Unity_ObjectManager_R1.WldObjType.ClockObj] = createEventDataTemplate(allfix.DESIndex_ClockObj, 3),
                    [Unity_ObjectManager_R1.WldObjType.DivObj] = createEventDataTemplate(allfix.DESIndex_DivObj, 1),
                });

            ObjData createEventDataTemplate(uint desIndex, uint etaIndex) => new ObjData()
            {
                PCPacked_SpritesIndex = desIndex,
                PCPacked_ImageBufferIndex = desIndex,
                PCPacked_AnimationsIndex = desIndex,
                PCPacked_ETAIndex = etaIndex
            };

            // Create the maps
            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,

                    // Set the dimensions
                    Width = mapData.Width,
                    Height = mapData.Height,

                    // Create the tile arrays
                    TileSet = new Unity_TileSet[palettes.Length],
                    MapTiles = mapData.Blocks.Select(x => new Unity_Tile(MapTile.FromR1MapTile(x))).ToArray(),

                    TileSetTransparencyModes = tileTextureData?.NormalBlockTexturesOffsetTable.Select(x => tileTextureData.OpaqueTextures.Concat(tileTextureData.TransparentTextures).FirstOrDefault(t => t.Offset == x)).Select(x =>
                    {
                        if (x == null)
                            return Block.BlockRenderMode.FullyTransparent;

                        if (x.TransparencyMode == 0xAAAAAAAA)
                            return Block.BlockRenderMode.FullyTransparent;

                        if (x.TransparencyMode == 0x55555555)
                            return Block.BlockRenderMode.Opaque;

                        return Block.BlockRenderMode.Transparent;
                    }).ToArray(),
                    PCTileOffsetTable = tileTextureData?.NormalBlockTexturesOffsetTable
                }
            };

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            // Load the localization
            var loc = await LoadLocalizationAsync(context);

            Controller.DetailedState = "Loading events";
            await Controller.WaitIfNecessary();

            // Load Rayman
            var rayman = new Unity_Object_R1(ObjData.CreateRayman(context, objData.Objects.FirstOrDefault(x => x.Type == ObjType.TYPE_RAY_POS)), objManager);

            // Create a level object
            Unity_Level level = new Unity_Level()
            {
                Maps = maps, 
                ObjManager = objManager, 
                Rayman = rayman, 
                Localization = loc,
                Background = bg,
                ParallaxBackground = bg2
            };

            for (var i = 0; i < objData.Objects.Length; i++)
            {
                ObjData e = objData.Objects[i];

                if (context.GetR1Settings().R1_World != World.Menu)
                {
                    e.Commands = objData.ObjCommands[i].Commands;
                    e.LabelOffsets = objData.ObjCommands[i].LabelOffsetTable;
                }

                // Add the event
                level.EventData.Add(new Unity_Object_R1(e, objManager, worldInfo: worldInfos?[i]));
            }

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading tile set";

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                // Read the 3 tile sets (one for each palette)
                var tileSets = ReadTileSets(FileFactory.Read<LevelFile>(context, GetLevelFilePath(context.GetR1Settings())));

                // Set the tile sets
                for (int i = 0; i < level.Maps[0].TileSet.Length; i++)
                    level.Maps[0].TileSet[i] = tileSets[i];
            }
            else
            {
                level.Maps[0].TileSet[0] = new Unity_TileSet(Settings.CellSize);
            }

            // Return the level
            return level;
        }

        public abstract byte[] GetTypeZDCBytes { get; }
        public abstract byte[] GetZDCTableBytes { get; }
        public abstract byte[] GetEventFlagsBytes { get; }
        public abstract WorldInfo[] GetWorldMapInfos(Context context);

        public abstract UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, WorldFile world, LevelFile level, bool parallax);

        protected abstract UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context);

        /// <summary>
        /// Reads 3 tile-sets, one for each palette
        /// </summary>
        /// <param name="levData">The level data to get the tile-set for</param>
        /// <returns>The 3 tile-sets</returns>
        public Unity_TileSet[] ReadTileSets(LevelFile levData) {
            // Create the output array
            var output = new Unity_TileSet[]
            {
                new Unity_TileSet(new Unity_TileTexture[levData.NormalBlockTextures.NormalBlockTexturesOffsetTable.Length]),
                new Unity_TileSet(new Unity_TileTexture[levData.NormalBlockTextures.NormalBlockTexturesOffsetTable.Length]),
                new Unity_TileSet(new Unity_TileTexture[levData.NormalBlockTextures.NormalBlockTexturesOffsetTable.Length])
            };

            // Keep track of the tile index
            int index = 0;

            // Get all tile textures
            var allTex = levData.NormalBlockTextures.OpaqueTextures.Concat(levData.NormalBlockTextures.TransparentTextures).ToArray();

            // Enumerate every texture
            foreach (var offset in levData.NormalBlockTextures.NormalBlockTexturesOffsetTable)
            {
                // Find matching tile texture
                var tileTex = allTex.FirstOrDefault(x => x.Offset == offset);

                // Enumerate every palette
                for (int i = 0; i < levData.MapInfo.Palettes.Length; i++)
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
                            var c = tileTex == null || index == 0 ? new Color(0, 0, 0, 0) : levData.MapInfo.Palettes[i][255 - tileTex.ImgData[cellIndex]].GetColor();

                            if (tileTex != null && tileTex.ImgData[cellIndex] != 242)
                                allRed = false;

                            // If the texture is transparent, add the alpha channel
                            if (tileTex is TransparentBlockTexture tt)
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
        public override UniTask SaveLevelAsync(Context context, Unity_Level level) 
        {
            // Menu levels can't be saved
            if (context.GetR1Settings().R1_World == World.Menu)
                return UniTask.CompletedTask;

            // Get the object manager
            var objManager = (Unity_ObjectManager_R1)level.ObjManager;

            // Get the level file path
            var lvlPath = GetLevelFilePath(context.GetR1Settings());

            // Get the level data
            var lvlData = context.GetMainFileObject<LevelFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.MapInfo.Height; y++) {
                for (int x = 0; x < lvlData.MapInfo.Width; x++) {
                    // Set the tiles
                    lvlData.MapInfo.Blocks[y * lvlData.MapInfo.Width + x] = level.Maps[0].MapTiles[y * lvlData.MapInfo.Width + x].Data.ToR1MapTile();
                }
            }

            // Update link table
            lvlData.ObjData.ObjLinkingTable = objManager.LinkTable;

            // Temporary event lists
            var events = new List<ObjData>();
            var eventCommands = new List<ObjCommandsData>();

            // Read the world data
            foreach (var e in level.EventData.Cast<Unity_Object_R1>())
            {
                var r1Event = e.EventData;

                r1Event.BlockTypes ??= new BlockType[5];
                r1Event.CommandContexts ??= new[] { new CommandContext() };

                r1Event.SpritesCount = (ushort)objManager.DES[e.DESIndex].Data.ImageDescriptors.Length;
                r1Event.AnimationsCount = (byte)objManager.DES[e.DESIndex].Data.Graphics.Animations.Count;

                // Add the event
                events.Add(r1Event);

                var cmds = e.EventData.Commands ?? new ObjCommands()
                {
                    Commands = new Command[0]
                };
                var labelOffsets = e.EventData.LabelOffsets ?? new ushort[0];

                // Add the event commands
                eventCommands.Add(new ObjCommandsData()
                {
                    Commands = cmds,
                    LabelOffsetTable = labelOffsets
                });
            }

            // Update event values
            lvlData.ObjData.ObjectsCount = (ushort)events.Count;
            lvlData.ObjData.Objects = events.ToArray();
            lvlData.ObjData.ObjCommands = eventCommands.ToArray();

            // Save the file
            FileFactory.Write<LevelFile>(context, lvlPath);

            return UniTask.CompletedTask;
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            // Allfix
            await AddFile(context, GetAllfixFilePath(context.GetR1Settings()));

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                // World
                await AddFile(context, GetWorldFilePath(context.GetR1Settings()));

                // Level
                await AddFile(context, GetLevelFilePath(context.GetR1Settings()));

                // BigRay
                await AddFile(context, GetBigRayFilePath(context.GetR1Settings()));
            }

            // Vignette
            await AddFile(context, GetVignetteFilePath(context.GetR1Settings()));
        }

        /// <summary>
        /// Adds all files to the context, to be used for export operations
        /// </summary>
        /// <param name="context">The context to add to</param>
        public virtual void AddAllFiles(Context context)
        {
            // Add big ray file
            context.AddFile(GetFile(context, GetBigRayFilePath(context.GetR1Settings())));
            
            // Add allfix file
            context.AddFile(GetFile(context, GetAllfixFilePath(context.GetR1Settings())));

            // Add for every world
            for (int world = 1; world < 7; world++)
            {
                // Set the world
                context.GetR1Settings().World = world;
                context.GetRequiredSettings<Ray1Settings>().World = (World)world;

                // Add world file
                context.AddFile(GetFile(context, GetWorldFilePath(context.GetR1Settings())));

                // Add every level
                foreach (var lvl in GetLevels(context.GetR1Settings()).First(x => x.Name == context.GetR1Settings().EduVolume || x.Name == null).Worlds.FirstOrDefault(x => x.Index == world)?.Maps ?? new int[0])
                {
                    // Set the level
                    context.GetR1Settings().Level = lvl;
                    context.GetRequiredSettings<Ray1Settings>().Level = lvl;

                    // Add level file
                    context.AddFile(GetFile(context, GetLevelFilePath(context.GetR1Settings())));
                }
            }
        }

        /// <summary>
        /// Gets a binary file to add to the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filePath">The file path</param>
        /// <param name="endianness">The endianness to use</param>
        /// <returns>The binary file</returns>
        protected virtual BinaryFile GetFile(Context context, string filePath, Endian endianness = Endian.Little) => new LinearFile(context, filePath, endianness);

        public async UniTask AddFile(Context context, string filePath, bool isBigFile = false, Endian endianness = Endian.Little)
        {
            if (context.FileExists(filePath))
                return;

            if (isBigFile)
                await FileSystem.PrepareBigFile(context.GetAbsoluteFilePath(filePath), 8);
            else
                await FileSystem.PrepareFile(context.GetAbsoluteFilePath(filePath));

            if (!FileSystem.FileExists(context.GetAbsoluteFilePath(filePath)))
                return;

            var file = GetFile(context, filePath, endianness);

            context.AddFile(file);
        }

        /// <summary>
        /// Gets the event states for the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The event states</returns>
        public virtual IEnumerable<States> GetCurrentEventStates(Context context)
        {
            // Read the fixed data
            var allfix = FileFactory.Read<AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings()));

            if (context.GetR1Settings().R1_World == World.Menu)
                return allfix.Eta;

            // Read the world data
            var worldData = FileFactory.Read<WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

            // Read the big ray data
            var bigRayData = FileFactory.Read<BigRayFile>(context, GetBigRayFilePath(context.GetR1Settings()));

            // Get the eta items
            return allfix.Eta.Concat(worldData.Eta).Concat(bigRayData.Eta);
        }

        public void LogArchives(GameSettings settings)
        {
            using (var context = new Ray1MapContext(settings))
            {
                // Load every archive
                var archives = GetArchiveFiles(settings).
                    Where(x => File.Exists(context.GetAbsoluteFilePath(x.FilePath))).
                    Select(archive =>
                    {
                        context.AddFile(new LinearFile(context, archive.FilePath));
                        return new
                        {
                            Archive = FileFactory.Read<FileArchive>(context, archive.FilePath),
                            Volume = archive.Volume
                        };
                    }).
                    ToArray();

                // Helper methods for loading an archive file
                void LogFile<T>(R1_PC_ArchiveFileName fileName)
                    where T : BinarySerializable, new()
                {
                    // Log each file
                    foreach (var archive in archives)
                    {
                        var index = archive.Archive.Entries.FindItemIndex(x => x.FileName == fileName.ToString());
                        context.GetR1Settings().EduVolume = archive.Volume;
                        context.GetRequiredSettings<Ray1Settings>().Volume = archive.Volume;
                        archive.Archive.ReadFile<T>(context, index);
                    }
                }

                // Read all known files
                LogFile<VersionScript>(R1_PC_ArchiveFileName.VERSION);
                //LogFile<>("SCRIPT"); // TODO: Serialize script! Contains data for the in-game menus.
                LogFile<GeneralScript>(R1_PC_ArchiveFileName.GENERAL);
                LogFile<GeneralScript>(R1_PC_ArchiveFileName.GENERAL0);
                LogFile<WordsScript>(R1_PC_ArchiveFileName.MOT);
                LogFile<SampleNamesScript>(R1_PC_ArchiveFileName.SMPNAMES);
                LogFile<TextScript>(R1_PC_ArchiveFileName.TEXT);
                LogFile<WorldMapScript>(R1_PC_ArchiveFileName.WLDMAP01);
            }
        }

        public void ExportETAInfo(GameSettings settings, string outputDir, bool includeStates)
        {
            /*
            using (var context = new R1Context(settings))
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
                    var fileNames = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.GetR1Settings()), context).ETAFileNames;

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

                var allfix = FileFactory.Read<R1_PC_AllfixFile>(GetAllfixFilePath(context.GetR1Settings()), context);
                AddToOutput("Allfix", allfix, 0);

                for (int w = 1; w < 7; w++)
                {
                    context.GetR1Settings().World = w;
                    AddToOutput(w.ToString(), FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.GetR1Settings()), context), allfix.Eta.Length);
                }

                JsonHelpers.SerializeToFile(output, Path.Combine(outputDir, $"ETA{(includeStates ? "ex" : String.Empty)} - {context.GetR1Settings().GameModeSelection}.json"), NullValueHandling.Ignore);
            }*/
        }

        public T LoadArchiveFile<T>(Context context, string archivePath, string fileName)
            where T : BinarySerializable, new()
        {
            if (context.MemoryMap.Files.All(x => x.FilePath != archivePath))
                return null;

            return FileFactory.Read<FileArchive>(context, archivePath).ReadFile<T>(context, fileName);
        }

        public T LoadArchiveFile<T>(Context context, string archivePath, R1_PC_ArchiveFileName fileName)
            where T : BinarySerializable, new() => LoadArchiveFile<T>(context, archivePath, fileName.ToString());
        public T LoadArchiveFile<T>(Context context, string archivePath, int fileIndex)
            where T : BinarySerializable, new()
        {
            if (context.MemoryMap.Files.All(x => x.FilePath != archivePath))
                return null;

            return FileFactory.Read<FileArchive>(context, archivePath).ReadFile<T>(context, fileIndex);
        }

        public abstract UniTask<PCX> GetWorldMapVigAsync(Context context);

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
