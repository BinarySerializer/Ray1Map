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
    /// The game manager for Rayman Educational (PS1)
    /// </summary>
    public class PS1_EDU_Manager : PC_EDU_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) =>
            $"{GetVolumePath(settings)}{GetShortWorldName(settings.World)}/{GetShortWorldName(settings.World)}{Math.Ceiling(settings.Level / 19d):00}/{GetShortWorldName(settings.World)}{settings.Level:00}.NEW";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetVolumePath(settings) + $"RAY{((int)settings.World + 1):00}.NEW";

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public override string GetAllfixFilePath(GameSettings settings) => GetVolumePath(settings) + $"ALLFIX.NEW";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetVolumePath(settings) + $"BIGRAY.DAT";

        /// <summary>
        /// Gets the file path for the .grx bundle for allfix
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The .grx bundle file path</returns>
        public string GetGRXFixFilePath(GameSettings settings) => $"FIX{settings.EduVolume.Substring(2, 1)}.GRX";

        /// <summary>
        /// Gets the file path for the .grx bundle for the levels
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The .grx bundle file path</returns>
        public string GetGRXLevelFilePath(GameSettings settings) => $"{settings.EduVolume}.GRX";

        /// <summary>
        /// Gets the name for the file to use in the .grx files for BigRay
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXBigRayName(GameSettings settings) => $"{settings.EduVolume.Substring(0, 2).ToLower()}_br";

        /// <summary>
        /// Gets the name for the file to use in the .grx files for allfix
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXFixName(GameSettings settings) => $"{settings.EduVolume.Substring(0, 2).ToLower()}_fix";
        
        /// <summary>
        /// Gets the name for the file to use in the .grx files for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXLevelName(GameSettings settings) => $"{settings.EduVolume.Substring(0, 1)}W{((int)settings.World) + 1}L{settings.Level}";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetVolumePath(settings), $"{GetShortWorldName(w)}??.NEW", SearchOption.AllDirectories)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray();

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override ArchiveFile[] GetArchiveFiles(GameSettings settings)
        {
            return GetEduVolumes(settings).SelectMany(x => new ArchiveFile[]
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
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignette(settings, GetVignetteFilePath(settings), output)),
                new GameAction("Export Archives", false, true, (input, output) => ExtractArchives(output)),
                new GameAction("Export GRX", false, true, (i, o) => ExportGRX(settings, o)), 
            };
        }

        #endregion

        #region Manager Methods

        /// <summary>
        /// Exports the .grx files
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputDir">The output directory to export to</param>
        public void ExportGRX(GameSettings settings, string outputDir)
        {
            // Create the context
            using (var context = new Context(settings))
            {
                foreach (var grxFilePath in Directory.GetFiles(settings.GameDirectory, "*.grx", SearchOption.TopDirectoryOnly).Select(Path.GetFileName))
                {
                    context.AddFile(new LinearSerializedFile(context)
                    {
                        filePath = grxFilePath
                    });

                    var grx = FileFactory.Read<PS1_EDU_GRX>(grxFilePath, context);

                    foreach (var grxFile in grx.Files) {
                        Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, grxFile.FileName), grx.GetFileBytes(context.Deserializer, grxFile.FileName));
                        if (grxFile.FileName.ToLower().EndsWith(".tex")) {
                            string baseName = grxFile.FileName.Substring(0, grxFile.FileName.Length - 4);
                            Texture2D[] tex = GetSpriteTextures(context, grx, baseName).ToArray();
                            for (int i = 0; i < tex.Length; i++) {
                                Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, baseName, $"{i}.png"), tex[i].EncodeToPNG());
                            }
                            /*Texture2D[] tex = GetPageTextures(context, grx, baseName).ToArray();
                            for (int i = 0; i < Math.Min(100, tex.Length); i++) {
                                Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, baseName, $"{i}.png"), tex[i].EncodeToPNG());
                            }*/
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
        public async Task<Common_Design[]> LoadSpritesAsync(Context context)
        {
            Controller.status = $"Loading sprites";
            await Controller.WaitIfNecessary();

            // Create the output list
            List<Common_Design> eventDesigns = new List<Common_Design>();

            // Load the world files
            var allfix = FileFactory.Read<PS1_EDU_AllfixFile>(GetAllfixFilePath(context.Settings), context);
            var world = FileFactory.Read<PS1_EDU_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Get all DES
            var des = allfix.DESData.Concat(world.DESData);

            // Load the .grx bundles
            var fixGrx = FileFactory.Read<PS1_EDU_GRX>(GetGRXFixFilePath(context.Settings), context);
            var worldGrx = FileFactory.Read<PS1_EDU_GRX>(GetGRXLevelFilePath(context.Settings), context);

            // Get the textures from allfix and world
            var textures = GetSpriteTextures(context, fixGrx, GetGRXFixName(context.Settings)).Concat(GetSpriteTextures(context, worldGrx, GetGRXLevelName(context.Settings)));

            // TODO: Fix this
            // Enumerate every DES
            foreach (var d in des)
            {
                var sprites = textures.Take(d.ImageDescriptorsCount).Select(x => Sprite.Create(x, new Rect(0, 0, x.width, x.height), new Vector2(0f, 1f), 16, 20)).ToList();
                var animations = new List<Common_Animation>();

                eventDesigns.Add(new Common_Design()
                {
                    Animations = animations,
                    Sprites = sprites
                });
            }

            // Return the sprites
            return eventDesigns.ToArray();
        }

        /// <summary>
        /// Gets the sprites from a .grx file
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="grx">The .grx file</param>
        /// <param name="fileName">The file name, without the extension</param>
        /// <returns>The sprites</returns>
        public IEnumerable<Texture2D> GetSpriteTextures(Context context, PS1_EDU_GRX grx, string fileName)
        {
            // Get the .gsp and .tex data
            var s = context.Deserializer;

            PS1_EDU_TEX tex = null;
            ushort[] gspIndices = null;

            s.DoAt(grx.BaseOffset + grx.GetFile(fileName + ".TEX").FileOffset, () => tex = s.SerializeObject<PS1_EDU_TEX>(default, name: nameof(tex)));
            s.DoAt(grx.BaseOffset + grx.GetFile(fileName + ".GSP").FileOffset, () => gspIndices = s.SerializeObject<PS1_EDU_GSP>(default, name: nameof(gspIndices)).Indices);

            // Parse the sprites from the texture pages
            for(int i = 0; i < tex.Descriptors.Length; i++) {
                var d = tex.Descriptors[i];
                ObjectArray<ARGB1555Color> palette = null;
                if (tex.Palettes.Length == tex.Descriptors.Length) {
                    palette = tex.Palettes[i];
                }
                // Get the texture page
                var page = tex.TexturePages[d.PageIndex];

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

                        if (palette == null) {
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
                            ARGB1555Color col = null;
                            switch (tex.BitDepth) {
                                case 4: col = palette.Value[paletteIndex]; break;
                                case 8: col = palette.Value[paletteIndex]; break;
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
        /// Gets the sprites from a .grx file
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="grx">The .grx file</param>
        /// <param name="fileName">The file name, without the extension</param>
        /// <returns>The sprites</returns>
        public IEnumerable<Texture2D> GetPageTextures(Context context, PS1_EDU_GRX grx, string fileName) {
            // Get the .gsp and .tex data
            var s = context.Deserializer;

            PS1_EDU_TEX tex = null;

            s.DoAt(grx.BaseOffset + grx.GetFile(fileName + ".TEX").FileOffset, () => tex = s.SerializeObject<PS1_EDU_TEX>(default, name: nameof(tex)));
            
            for(int i = 0; i < tex.TexturePages.Length; i++) {
                //var page = tex.TexturePages[i];
                // Create the texture
                int actualW = (int)tex.Width * (int)tex.BitDepth / 8;
                Texture2D sprite = new Texture2D(actualW, (int)tex.Height, TextureFormat.RGBA32, false) {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                // Default to fully transparent
                sprite.SetPixels(new Color[sprite.height * sprite.width]);

                for (int y = 0; y < sprite.height; y++) {
                    for (int x = 0; x < sprite.width; x++) {
                        // TODO: Fix this
                        var color = tex.GetPagePixel(i, x, y);
                        //var paletteIndex = page.GetPixel8(0, 0, d.XInPage + x, d.YInPage + y);

                        // TODO: Fix this
                        // Set the pixel
                        switch (tex.BitDepth) {
                            case 4: color <<= 4; break;
                            case 8: break;
                            case 16: color >>= 8; break;
                        }
                        sprite.SetPixel(
                            x,
                            sprite.height - 1 - y,
                            new Color(color / 255f, color / 255f, color / 255f));
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
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading map data for {context.Settings.EduVolume}: {context.Settings.World} {context.Settings.Level}";

            // Load the level
            var levelData = FileFactory.Read<PS1_EDU_LevFile>(GetLevelFilePath(context.Settings), context);

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
                        Width = levelData.Width,
                        Height = levelData.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        Tiles = new Common_Tile[levelData.Width * levelData.Height],
                    }
                },

                // Create the events list
                EventData = new List<Common_EventData>(),
            };

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context) : new Common_Design[0];

            var index = 0;

            foreach (PC_Event e in levelData.Events)
            {
                // Get the file keys
                var desKey = e.DES.ToString();
                var etaKey = e.ETA.ToString();

                // Add the event
                commonLev.EventData.Add(new Common_EventData
                {
                    Type = e.Type,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    DESKey = desKey,
                    ETAKey = etaKey,
                    OffsetBX = e.OffsetBX,
                    OffsetBY = e.OffsetBY,
                    OffsetHY = e.OffsetHY,
                    FollowSprite = e.FollowSprite,
                    HitPoints = e.HitPoints,
                    Layer = e.Layer,
                    HitSprite = e.HitSprite,
                    FollowEnabled = e.FollowEnabled,
                    LabelOffsets = levelData.EventCommands[index].LabelOffsetTable,
                    CommandCollection = levelData.EventCommands[index].Commands,
                    LinkIndex = levelData.EventLinkTable[index],
                    DebugText = $"Flags: {String.Join(", ", e.Flags.GetFlags())}{Environment.NewLine}"
                });

                index++;
            }

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading tile set";

            // Read the 3 tile sets (one for each palette)
            var tileSets = ReadTileSets(levelData);

            // Set the tile sets
            commonLev.Maps[0].TileSet[0] = tileSets[0];
            commonLev.Maps[0].TileSet[1] = tileSets[1];
            commonLev.Maps[0].TileSet[2] = tileSets[2];

            // Enumerate each cell
            for (int cellY = 0; cellY < levelData.Height; cellY++)
            {
                for (int cellX = 0; cellX < levelData.Width; cellX++)
                {
                    // Get the cell
                    var cell = levelData.MapTiles[cellY * levelData.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * levelData.Width + cellX] = new Common_Tile()
                    {
                        TileSetGraphicIndex = cell.TextureIndex,
                        CollisionType = cell.CollisionType,
                        PaletteIndex = 1,
                        XPosition = cellX,
                        YPosition = cellY
                    };
                }
            }

            // Return an editor manager
            return GetEditorManager(commonLev, context, eventDesigns);
        }

        /// <summary>
        /// Reads 3 tile-sets, one for each palette
        /// </summary>
        /// <param name="levData">The level data to get the tile-set for</param>
        /// <returns>The 3 tile-sets</returns>
        public Common_Tileset[] ReadTileSets(PS1_EDU_LevFile levData)
        {
            // Create the output array
            var output = new Common_Tileset[levData.ColorPalettes.Length];

            // Enumerate every palette
            for (int i = 0; i < levData.ColorPalettes.Length; i++)
                output[i] = new Common_Tileset(levData.TileTextures.Select(x => x == 0 ? new RGB666Color(0, 0, 0, 0) : levData.ColorPalettes[i][x]).ToArray(), 512 / Settings.CellSize, Settings.CellSize);

            return output;
        }

        /// <summary>
        /// Gets the event states for the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The event states</returns>
        public override IEnumerable<PC_ETA> GetCurrentEventStates(Context context)
        {
            // TODO: Read ETA from allfix + world
            return new PC_ETA[0];
        }

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public override async Task LoadFilesAsync(Context context)
        {
            // Load base files
            await base.LoadFilesAsync(context);

            // Load the .grx files
            await LoadFileAsync(GetGRXFixFilePath(context.Settings));
            await LoadFileAsync(GetGRXLevelFilePath(context.Settings));

            async Task LoadFileAsync(string filePath)
            {
                await FileSystem.PrepareFile(context.BasePath + filePath);
                context.AddFile(GetFile(context, filePath));
            }
        }

        #endregion
    }
}