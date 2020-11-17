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
        public override string GetLevelFilePath(GameSettings settings)
        {
            var worldDir = $"{GetVolumePath(settings.EduVolume)}{GetShortWorldName(settings.R1_World)}";

            // US version has the world split into two parts
            if (settings.EduVolume.Contains("US"))
            {
                return $"{worldDir}/{GetShortWorldName(settings.R1_World)}{(settings.Level < 20 ? 1 : 2):00}/{GetShortWorldName(settings.R1_World)}{settings.Level:00}.NEW";
            }
            else
            {
                return $"{worldDir}/{GetShortWorldName(settings.R1_World)}{settings.Level:00}.NEW";
            }
        }

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

        public override string GetCommonArchiveFilePath(GameSettings settings) => GetVolumePath(settings.EduVolume) + "COMMON.DAT";

        /// <summary>
        /// Gets the name for the file to use in the .grx files for BigRay
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXBigRayName(GameSettings settings) => $"{GetLangCode(settings).ToLower()}_br";

        /// <summary>
        /// Gets the language for the volume
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The language code</returns>
        public string GetLangCode(GameSettings settings)
        {
            var v = settings.EduVolume.Substring(0, 2).ToUpper();

            switch (v)
            {
                // English (US)
                case "US":
                    return "US";

                // French > English
                case "FG":
                // French
                case "FR":
                    return "FR";

                // Italian > English
                case "IG":
                // Italian
                case "IT":
                    return "IT";

                // Spanish > English
                case "EG":
                // Spanish
                case "CS":
                    return "SP";

                // German > English
                case "DG":
                // German
                case "AL":
                    return "GM";
                
                // English (UK)
                case "GB":
                    return "EN";

                default:
                    return v;
            }
        }

        /// <summary>
        /// Gets the name for the file to use in the .grx files for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXLevelName(GameSettings settings) => $"{GetLangCode(settings).Substring(0, 1)}W{settings.World}L{settings.Level}";

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
        public override Archive[] GetArchiveFiles(GameSettings settings)
        {
            return new Archive[]
            {
                new Archive(GetCommonArchiveFilePath(settings)), 
            }.Concat(GetLevels(settings).Select(x => x.Name).SelectMany(x => new Archive[]
            {
                new Archive(GetSpecialArchiveFilePath(x), x), 
                new Archive(GetVignetteFilePath(x), x), 
            })).ToArray();
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
                new GameAction("Export Sprites (GRX)", false, true, (input, output) => ExportGRXAsync(settings, output, true)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignette(settings, GetVignetteFilePath(settings), output)),
                new GameAction("Export Archives", false, true, (input, output) => ExtractArchives(output)),
                new GameAction("Export GRX", false, true, (input, output) => ExportGRXAsync(settings, output, false)), 
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
        public async UniTask ExportGRXAsync(GameSettings settings, string outputDir, bool exportSprites)
        {
            // Create the context
            using (var context = new Context(settings))
            {
                // Get the big ray palette if exporting sprites
                IList<ARGBColor> brPal = null;

                var s = context.Deserializer;

                if (exportSprites)
                    brPal = GetBigRayPalette(context);

                foreach (var grxFilePath in Directory.GetFiles(settings.GameDirectory, "*.grx", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName))
                {
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = grxFilePath
                    });

                    var grx = await LoadGRXAsync(context, grxFilePath);

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
                            Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, grxFile.FileName), await grx.GetFileBytesAsync(context.Deserializer, grxFile.FileName));
                        }
                    }
                }
            }
        }

        public async UniTask<R1_PS1Edu_GRX> LoadGRXAsync(Context context, string grxFilePath)
        {
            var s = context.Deserializer;
            s.Goto(context.GetFile(grxFilePath).StartPointer);

            var grx = new R1_PS1Edu_GRX();
            await grx.SerializeHeaderAsync(s);

            return grx;
        }

        public async UniTask<T> LoadGRXFileAsync<T>(Context context, IList<R1_PS1Edu_GRX> grx, string fileName, string name)
            where T : R1Serializable, new()
        {
            // Get the file
            var file = grx.SelectMany(x => x.Files).FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));

            if (file == null)
                return null;

            // Get the grx it belongs to
            var g = grx.First(x => x.Files.Contains(file));

            // Get the pointer
            var pointer = g.BaseOffset + file.FileOffset;

            var s = context.Deserializer;

            // Go to the pointer
            s.Goto(pointer);

            await s.FillCacheForRead((int)file.FileSize);

            return s.SerializeObject<T>(default, name: nameof(name));
        }

        /// <summary>
        /// Loads the sprites for the level
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The common event designs</returns>
        public async UniTask<Unity_ObjectManager_R1.DESData[]> LoadSpritesAsync(Context context)
        {
            Controller.DetailedState = $"Loading sprites";
            await Controller.WaitIfNecessary();

            // Load the world files
            var allfix = FileFactory.Read<R1_PS1Edu_WorldFile>(GetAllfixFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.Allfix);
            var world = FileFactory.Read<R1_PS1Edu_WorldFile>(GetWorldFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.World);
            var level = FileFactory.Read<R1_PS1Edu_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the .grx bundles
            var grx = new List<R1_PS1Edu_GRX>();

            foreach (var grxFile in context.MemoryMap.Files.Where(x => x.filePath.Contains(".GRX")).Select(x => x.filePath))
                grx.Add(await LoadGRXAsync(context, grxFile));

            var s = context.Deserializer;

            // Load .grx files (.tex and .gsp)
            R1_PS1Edu_TEX levelTex = await LoadGRXFileAsync<R1_PS1Edu_TEX>(context, grx, GetGRXLevelName(context.Settings) + ".TEX", "LevelTex");
            ushort[] levelIndices = (await LoadGRXFileAsync<R1_PS1Edu_GSP>(context, grx, GetGRXLevelName(context.Settings) + ".GSP", "LevelIndices"))?.Indices;

            if (levelTex == null || levelIndices == null)
                return new Unity_ObjectManager_R1.DESData[0];

            Unity_ObjectManager_R1.DESData[] des = new Unity_ObjectManager_R1.DESData[allfix.DESCount + world.DESCount];
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

                // Check if it's multi-colored
                var isMultiColored = IsDESMultiColored(context, i, LevelEditorData.EventInfoData);

                des[i] = new Unity_ObjectManager_R1.DESData(new Unity_ObjGraphics
                {
                    Sprites = new Sprite[d.ImageDescriptorsCount * (isMultiColored ? 6 : 1)].ToList(),
                    Animations = anims.Select(x => x.ToCommonAnimation()).ToList()
                }, imageDescriptors[i]);
            }

            int globalGspIndex = 0;

            // Keep track of already loaded DES
            var loadedDES = new List<int>();

            // Enumerate every event
            foreach (R1_EventData e in level.Events)
            {
                // Get event DES index
                var desIndex = (int)e.PC_ImageDescriptorsIndex;

                if (loadedDES.Contains(desIndex))
                {
                    globalGspIndex += e.ImageDescriptorCount;
                    continue;
                }

                await Controller.WaitIfNecessary();
                loadedDES.Add(desIndex);

                // Check if it's multi-colored
                var isMultiColored = IsDESMultiColored(context, desIndex, LevelEditorData.EventInfoData);

                // Enumerate every color
                for (int color = 0; color < (isMultiColored ? 6 : 1); color++)
                {
                    var localGspIndex = globalGspIndex;

                    // Enumerate every image descriptor in the event DES
                    for (int i = 0; i < e.ImageDescriptorCount; i++)
                    {
                        ushort texIndex = levelIndices[localGspIndex];
                        var d = levelTex.Descriptors[texIndex];

                        IList<ARGBColor> p = null;

                        if (levelTex.Palettes.Length == levelTex.Descriptors.Length)
                            p = levelTex.Palettes[texIndex].Value;

                        //if (color > 0) Debug.Log(color + " - " + texIndex);
                        if (isMultiColored && p != null && color > 0)
                        {
                            // Hack to get correct colors
                            /*var newPal = p.Skip(color * 8 + 1).ToList();

                            newPal.Insert(0, new ARGBColor(0, 0, 0));

                            if (color % 2 != 0)
                                newPal[8] = p[color * 8];

                            p = newPal;*/
                            List<ARGBColor> newPal = new List<ARGBColor>();
                            for (int c = 0; c < p.Count; c++) {
                                if (c == 0) {
                                    newPal.Add(p[c]);
                                } else {
                                    bool added = false;
                                    for (int oc = 0; oc < 8; oc++) {
                                        if ((level.ColorPalettes[0][oc].Red >> 3) == (p[c].Red >> 3) &&
                                            (level.ColorPalettes[0][oc].Green >> 3) == (p[c].Green >> 3) &&
                                            (level.ColorPalettes[0][oc].Blue >> 3) == (p[c].Blue >> 3)) {
                                            newPal.Add(level.ColorPalettes[0][oc + color * 8]);
                                            added = true;
                                            break;
                                        }
                                    }
                                    if (!added) newPal.Add(p[c]);
                                }
                            }
                            p = newPal;
                        }

                        if (!imageDescriptors[e.PC_ImageDescriptorsIndex][i].IsDummySprite())
                            des[desIndex].Graphics.Sprites[color * e.ImageDescriptorCount + i] = GetSpriteTexture(levelTex, d, p).CreateSprite();

                        localGspIndex++;
                    }
                }

                globalGspIndex += e.ImageDescriptorCount;
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
            for (int i = 0; i < tex.Descriptors.Length; i++)
            {
                var d = tex.Descriptors[i];
                IList<ARGBColor> p = palette;

                if (p == null && tex.Palettes.Length == tex.Descriptors.Length)
                    p = tex.Palettes[i].Value;

                yield return GetSpriteTexture(tex, d, p);
            }
        }

        public Texture2D GetSpriteTexture(R1_PS1Edu_TEX tex, R1_PS1Edu_TEXDescriptor d, IList<ARGBColor> p)
        {
            // Create the texture
            Texture2D sprite = TextureHelpers.CreateTexture2D(d.Width, d.Height, clear: true);

            for (int y = 0; y < d.Height; y++)
            {
                for (int x = 0; x < d.Width; x++)
                {
                    var paletteIndex = tex.GetPagePixel(d.PageIndex, d.XInPage + x, d.YInPage + y);

                    if (p == null)
                    {
                        switch (tex.BitDepth)
                        {
                            case 4: paletteIndex <<= 4; break;
                            case 8: break;
                            case 16: paletteIndex >>= 8; break;
                        }
                        sprite.SetPixel(x, d.Height - 1 - y, new Color(paletteIndex / 255f, paletteIndex / 255f, paletteIndex / 255f));
                    }
                    else
                    {
                        ARGBColor col = null;

                        switch (tex.BitDepth)
                        {
                            case 4: col = p[paletteIndex]; break;
                            case 8: col = p[paletteIndex]; break;
                            case 16: col = ARGB1555Color.From1555(paletteIndex); break;
                        }

                        Color c = col.GetColor();

                        c = paletteIndex != 0 ? new Color(c.r, c.g, c.b, 1f) : new Color(0, 0, 0, 0f);

                        sprite.SetPixel(x, d.Height - 1 - y, c);
                    }
                }
            }

            // Apply the changes
            sprite.Apply();

            return sprite;
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading map data";

            // Load the level
            var levelData = FileFactory.Read<R1_PS1Edu_LevFile>(GetLevelFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context) : new Unity_ObjectManager_R1.DESData[0];

            var bigRayName = Path.GetFileNameWithoutExtension(GetBigRayFilePath(context.Settings));

            var des = eventDesigns.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<Unity_ObjectManager_R1.DESData>(x, i)).ToArray();
            var allEta = GetCurrentEventStates(context).ToArray();
            var eta = allEta.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<R1_EventState[][]>(x.States, i)).ToArray();

            // Create the object manager
            var objManager = new Unity_ObjectManager_R1(
                context: context, 
                des: des, 
                eta: eta, 
                linkTable: levelData.EventLinkTable, 
                usesPointers: false,
                hasDefinedDesEtaNames: true);

            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,

                    // Set the dimensions
                    Width = levelData.Width,
                    Height = levelData.Height,

                    // Create the tile arrays
                    TileSet = new Unity_MapTileMap[3],
                    MapTiles = levelData.MapTiles.Select(x => new Unity_Tile(x)).ToArray(),
                }
            };

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            // Load the localization
            var loc = await LoadLocalizationAsync(context);

            Controller.DetailedState = $"Loading events";
            await Controller.WaitIfNecessary();

            // Load Rayman
            var rayman = new Unity_Object_R1(R1_EventData.GetRayman(context, levelData.Events.FirstOrDefault(x => x.Type == R1_EventType.TYPE_RAY_POS)), objManager);

            var world = FileFactory.Read<R1_PS1Edu_WorldFile>(GetWorldFilePath(context.Settings), context, (ss, o) => o.FileType = R1_PS1Edu_WorldFile.Type.World);

            var bg = LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings), world.Plan0NumPcxFiles[levelData.LevelDefines.BG_0])?.ToTexture(true); 

            Unity_Level level = new Unity_Level(maps, objManager, rayman: rayman, localization: loc, background: bg);

            // Add the events
            for (var index = 0; index < levelData.Events.Length; index++)
            {
                var e = levelData.Events[index];

                e.Commands = levelData.EventCommands[index].Commands;
                e.LabelOffsets = levelData.EventCommands[index].LabelOffsetTable;

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

            // Return the level
            return level;
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

            var langCode = GetLangCode(context.Settings);

            // Load the .grx files
            if (context.Settings.GameModeSelection == GameModeSelection.RaymanEducationalPS1)
            {
                var volLevel = context.Settings.EduVolume.Substring(2, 1);

                await AddFile(context, $"FIX{volLevel}.GRX", true);

                for (int i = 1; i < 3 + 1; i++)
                    await AddFile(context, $"{langCode}{i}.GRX", true);
            }
            else if (context.Settings.GameModeSelection == GameModeSelection.RaymanQuizPS1)
            {
                await AddFile(context, $"LFIX.GRX", true);
                await AddFile(context, $"LE_{langCode}.GRX", true);
            }
        }

        // EDU PS1 doesn't use ZDC tables
        public override byte[] GetTypeZDCBytes => null;
        public override byte[] GetZDCTableBytes => null;

        #endregion
    }
}