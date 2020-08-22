using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Educational (PS1)
    /// </summary>
    public class R1_PS1Edu_Manager : R1_PCEdu_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => Directory.EnumerateFiles(settings.GameDirectory + $"{GetVolumePath(settings.EduVolume)}{GetShortWorldName(settings.R1_World)}", $"{GetShortWorldName(settings.R1_World)}{settings.Level:00}.NEW", SearchOption.AllDirectories).First().Substring(settings.GameDirectory.Length).Replace('\\', '/');

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"RAY{settings.World:00}.NEW";

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public override string GetAllfixFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"ALLFIX.NEW";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + $"BIGRAY.DAT";

        /// <summary>
        /// Gets the file paths for the .grx bundles
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The .grx bundle file paths</returns>
        public string[] GetAllGRX(GameSettings settings) => Directory.GetFiles(settings.GameDirectory, "*.grx", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToArray();

        /// <summary>
        /// Gets the name for the file to use in the .grx files for BigRay
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXBigRayName(GameSettings settings) => $"{GetShortVolName(settings).ToLower()}_br";

        /// <summary>
        /// Gets the short volume name for the volume
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The short volume name</returns>
        public string GetShortVolName(GameSettings settings)
        {
            var v = settings.EduVolume.Substring(0, 2).ToUpper();

            switch (v)
            {
                case "US":
                    return "US";

                case "FG":
                    return "FR";

                case "IG":
                    return "IT";

                case "EG":
                    return "SP";

                case "DG":
                    return "GM";

                case "GB":
                    return "EN";

                case "CS":
                    return "SP";

                default:
                    return v;
            }
        }

        /// <summary>
        /// Gets the name for the file to use in the .grx files for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXLevelName(GameSettings settings) => $"{GetShortVolName(settings).Substring(0, 1)}W{settings.World}L{settings.Level}";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => Directory.GetDirectories(settings.GameDirectory + "/" + GetDataPath(), "???", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).Select(vol => new GameInfo_Volume(vol, WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetVolumePath(vol), $"{GetShortWorldName(w)}??.NEW", SearchOption.AllDirectories)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray())).ToArray();

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override ArchiveFile[] GetArchiveFiles(GameSettings settings)
        {
            return GetLevels(settings).Select(x => x.Name).SelectMany(x => new ArchiveFile[]
            {
                new ArchiveFile($"PCMAP/{x}/COMMON.DAT"),
                new ArchiveFile($"PCMAP/{x}/SPECIAL.DAT"),
                new ArchiveFile($"PCMAP/{x}/VIGNET.DAT", ".pcx"),
            }).ToArray();
        }

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Export Sprites (GRX)", false, true, (input, output) => ExportGRX(settings, output, true)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignette(settings, GetVignetteFilePath(settings), output)),
                new GameAction("Export Archives", false, true, (input, output) => ExtractArchives(output)),
                new GameAction("Export GRX", false, true, (input, output) => ExportGRX(settings, output, false)), 
                new GameAction("Log Archive Files", false, false, (input, output) => LogArchives(settings)),
            };
        }

        #endregion

        #region Manager Methods

        /// <summary>
        /// Exports the .grx files
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory to export to</param>
        /// <param name="exportSprites">True if sprites should be exported, false if the files should be exported</param>
        public void ExportGRX(GameSettings settings, string outputDir, bool exportSprites)
        {
            // Create the context
            using (var context = new Context(settings))
            {
                // Get the big ray palette if exporting sprites
                IList<ARGBColor> brPal = null;

                var s = context.Deserializer;

                if (exportSprites)
                    brPal = GetBigRayPalette(context);

                foreach (var grxFilePath in GetAllGRX(settings))
                {
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = grxFilePath
                    });

                    var grx = FileFactory.Read<R1_PS1Edu_GRX>(grxFilePath, context);

                    foreach (var grxFile in grx.Files) 
                    {
                        if (exportSprites)
                        {
                            if (grxFile.FileName.ToLower().EndsWith(".tex"))
                            {
                                string baseName = grxFile.FileName.Substring(0, grxFile.FileName.Length - 4);

                                R1_PS1Edu_TEX texFile = null;

                                s.DoAt(grx.BaseOffset + grxFile.FileOffset, () => texFile = s.SerializeObject<R1_PS1Edu_TEX>(default, name: nameof(texFile)));

                                Texture2D[] tex = GetSpriteTextures(texFile, 
                                    // Use BigRay palette for BigRay sprites
                                    grxFile.FileName.Substring(3, 2) == "br" ? brPal : null).ToArray();

                                for (int i = 0; i < tex.Length; i++)
                                    Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, baseName, $"{i}.png"), tex[i].EncodeToPNG());
                            }
                        }
                        else
                        {
                            Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, grxFile.FileName), grx.GetFileBytes(context.Deserializer, grxFile.FileName));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the sprites for the level
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The common event designs</returns>
        public async UniTask<Unity_ObjGraphics[]> LoadSpritesAsync(Context context)
        {
            Controller.DetailedState = $"Loading sprites";
            await Controller.WaitIfNecessary();

            // Load the world files
            var allfix = FileFactory.Read<R1_PS1Edu_WorldFile>(GetAllfixFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.Allfix);
            var world = FileFactory.Read<R1_PS1Edu_WorldFile>(GetWorldFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.World);
            var level = FileFactory.Read<R1_PS1Edu_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the .grx bundles
            var grx = GetAllGRX(context.Settings).Select(x => FileFactory.Read<R1_PS1Edu_GRX>(x, context)).ToArray();

            // Helper method to get grx file pointer
            Pointer GetFilePointer(string fileName)
            {
                // Get the file
                var file = grx.SelectMany(x => x.Files).FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception($"No matching file was found for name {fileName}");

                // Get the grx it belongs to
                var g = grx.First(x => x.Files.Contains(file));

                // Return the pointer
                return g.BaseOffset + file.FileOffset;
            }

            var s = context.Deserializer;

            // Load .grx files (.tex and .gsp)
            R1_PS1Edu_TEX levelTex = s.DoAt(GetFilePointer(GetGRXLevelName(context.Settings) + ".TEX"), () => s.SerializeObject<R1_PS1Edu_TEX>(default, name: nameof(levelTex)));
            ushort[] levelIndices = s.DoAt(GetFilePointer(GetGRXLevelName(context.Settings) + ".GSP"), () => s.SerializeObject<R1_PS1Edu_GSP>(default, name: nameof(levelIndices)).Indices);
            Texture2D[] textures = GetSpriteTextures(levelTex).ToArray();

            int gsp_index = 0;
            Unity_ObjGraphics[] des = new Unity_ObjGraphics[allfix.DESCount + world.DESCount];
            R1_ImageDescriptor[][] imageDescriptors = new R1_ImageDescriptor[allfix.DESCount + world.DESCount][];
            for (int i = 0; i < des.Length; i++) {
                R1_PS1Edu_DESData d = null;
                R1_PS1Edu_AnimationDescriptor[] anims = null;
                if (i < allfix.DESCount) {
                    d = allfix.DESData[i];
                    anims = allfix.AnimationDescriptors[i];
                    imageDescriptors[i] = allfix.ImageDescriptors[i];
                } else {
                    d = world.DESData[i - allfix.DESCount];
                    anims = world.AnimationDescriptors[i - allfix.DESCount];
                    imageDescriptors[i] = world.ImageDescriptors[i - allfix.DESCount];
                }
                des[i] = new Unity_ObjGraphics();
                des[i].Sprites = new Sprite[d.ImageDescriptorsCount].ToList();
                des[i].Animations = anims.Select(x => x.ToCommonAnimation()).ToList();
            }
            foreach (R1_EventData e in level.Events) {
                for (int i = 0; i < e.ImageDescriptorCount; i++) {
                    ushort currentTexture = levelIndices[gsp_index];
                    var tex = textures[currentTexture];
                    if (imageDescriptors[e.PC_ImageDescriptorsIndex][i].Index != 0) {
                        des[e.PC_ImageDescriptorsIndex].Sprites[i] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), Settings.PixelsPerUnit, 20);
                    }
                    gsp_index++;
                }
            }

            // Return the sprites
            return des;
        }

        /// <summary>
        /// Gets the sprites from a .grx file
        /// </summary>
        /// <param name="tex">The .tex file data</param>
        /// <param name="palette">Optional palette to use</param>
        /// <returns>The sprites</returns>
        public IEnumerable<Texture2D> GetSpriteTextures(R1_PS1Edu_TEX tex, IList<ARGBColor> palette = null)
        {
            // Parse the sprites from the texture pages
            for(int i = 0; i < tex.Descriptors.Length; i++) {
                var d = tex.Descriptors[i];
                IList<ARGBColor> p = palette;

                if (p == null && tex.Palettes.Length == tex.Descriptors.Length)
                    p = tex.Palettes[i].Value;

                // Create the texture
                Texture2D sprite = new Texture2D(d.Width, d.Height, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                // Default to fully transparent
                sprite.SetPixels(new Color[sprite.height * sprite.width]);

                for (int y = 0; y < d.Height; y++)
                {
                    for (int x = 0; x < d.Width; x++)
                    {
                        var paletteIndex = tex.GetPagePixel(d.PageIndex, d.XInPage + x, d.YInPage + y);

                        if (p == null) {
                            switch (tex.BitDepth) {
                                case 4: paletteIndex <<= 4; break;
                                case 8: break;
                                case 16: paletteIndex >>= 8; break;
                            }
                            sprite.SetPixel(
                                x,
                                d.Height - 1 - y,
                                new Color(paletteIndex / 255f, paletteIndex / 255f, paletteIndex / 255f));
                        } else {
                            ARGBColor col = null;
                            switch (tex.BitDepth) {
                                case 4: col = p[paletteIndex]; break;
                                case 8: col = p[paletteIndex]; break;
                                case 16: col = ARGB1555Color.From1555(paletteIndex); break;
                            }
                            Color c = col.GetColor();
                            if (paletteIndex != 0)
                                c = new Color(c.r, c.g, c.b, 1f);
                            else
                                c = new Color(0, 0, 0, 0f);
                            sprite.SetPixel(
                                x,
                                d.Height - 1 - y,
                                c);
                        }
                    }
                }

                // Apply the changes
                sprite.Apply();

                yield return sprite;
            }
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public override async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading map data for {context.Settings.EduVolume}: {context.Settings.World} {context.Settings.Level}";

            // Load the level
            var levelData = FileFactory.Read<R1_PS1Edu_LevFile>(GetLevelFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level
            {
                // Create the map
                Maps = new Unity_Map[]
                {
                    new Unity_Map()
                    {
                        // Set the dimensions
                        Width = levelData.Width,
                        Height = levelData.Height,

                        // Create the tile arrays
                        TileSet = new Unity_MapTileMap[3],
                        MapTiles = levelData.MapTiles.Select(x => new Unity_Tile(x)).ToArray(),
                        TileSetWidth = 1
                    }
                },

                // Create the events list
                EventData = new List<Unity_Obj>(),
            };

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context) : new Unity_ObjGraphics[0];

            var index = 0;

            foreach (R1_EventData e in levelData.Events)
            {
                // Get the file keys
                var desKey = e.PC_ImageDescriptorsIndex.ToString();
                var etaKey = e.PC_ETAIndex.ToString();

                // Add the event
                level.EventData.Add(new Unity_Obj(e)
                {
                    Type = e.Type,
                    DESKey = desKey,
                    ETAKey = etaKey,
                    LabelOffsets = levelData.EventCommands[index].LabelOffsetTable,
                    CommandCollection = levelData.EventCommands[index].Commands,
                    LinkIndex = levelData.EventLinkTable[index],
                    DebugText = $"Flags: {String.Join(", ", e.PC_Flags.GetFlags())}{Environment.NewLine}"
                });

                index++;
            }

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading tile set";

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            level.Maps[0].TileSet[0] = tileSets[0];
            level.Maps[0].TileSet[1] = tileSets[1];
            level.Maps[0].TileSet[2] = tileSets[2];

            // Load localization
            LoadLocalization(context, level);

            // Return an editor manager
            return GetEditorManager(level, context, eventDesigns);
        }

        /// <summary>
        /// Reads 3 tile-sets, one for each palette
        /// </summary>
        /// <param name="levData">The level data to get the tile-set for</param>
        /// <returns>The 3 tile-sets</returns>
        public Unity_MapTileMap[] ReadTileSets(R1_PS1Edu_LevFile levData)
        {
            // Create the output array
            var output = new Unity_MapTileMap[levData.ColorPalettes.Length];

            // Enumerate every palette
            for (int i = 0; i < levData.ColorPalettes.Length; i++)
                output[i] = new Unity_MapTileMap(levData.TileTextures.Select(x => x == 0 ? new RGB666Color(0, 0, 0, 0) : levData.ColorPalettes[i][x]).ToArray(), 512 / Settings.CellSize, Settings.CellSize);

            return output;
        }

        /// <summary>
        /// Gets the event states for the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The event states</returns>
        public override IEnumerable<R1_PC_ETA> GetCurrentEventStates(Context context)
        {
            // Load the world files
            var allfix = FileFactory.Read<R1_PS1Edu_WorldFile>(GetAllfixFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.Allfix);
            var world = FileFactory.Read<R1_PS1Edu_WorldFile>(GetWorldFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.World);

            // Return the ETA
            return allfix.ETA.Concat(world.ETA).Select(x => new R1_PC_ETA()
            {
                States = x
            });
        }

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public override async UniTask LoadFilesAsync(Context context)
        {
            // Load base files
            await base.LoadFilesAsync(context);

            // Load the .grx files
            foreach (var g in GetAllGRX(context.Settings))
                await LoadFileAsync(g);

            async UniTask LoadFileAsync(string filePath)
            {
                await FileSystem.PrepareFile(context.BasePath + filePath);
                context.AddFile(GetFile(context, filePath));
            }
        }

        #endregion
    }
}