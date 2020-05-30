using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        /// Gets the file path for the .grx bundle
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The .grx bundle file path</returns>
        public string GetGRXFilePath(GameSettings settings) => $"{settings.EduVolume}.GRX";

        /// <summary>
        /// Gets the name for the file to use in the main .grx files
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The name</returns>
        public string GetGRXName(GameSettings settings) => $"{settings.EduVolume.Substring(0, 1)}W{((int)settings.World) + 1}L{settings.Level}";

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

                    foreach (var grxFile in grx.Files)
                        Util.ByteArrayToFile(Path.Combine(outputDir, grxFilePath, grxFile.FileName), grx.GetFileBytes(context.Deserializer, grxFile.FileName));
                }
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
            // TODO: Remove once we parse world files!
            loadTextures = false;

            Controller.status = $"Loading map data for {context.Settings.EduVolume}: {context.Settings.World} {context.Settings.Level}";

            // Load the level
            var levelData = FileFactory.Read<PS1_EDU_LevFile>(GetLevelFilePath(context.Settings), context);

            await Controller.WaitIfNecessary();

            // Get the .grx file name to use
            var grpName = GetGRXName(context.Settings);

            // Load the .grx bundle
            var grx = FileFactory.Read<PS1_EDU_GRX>(GetGRXFilePath(context.Settings), context);

            // Get the .gsp and .tex files
            var gsp = grx.GetFileBytes(context.Deserializer, grpName + ".GSP");
            var tex = grx.GetFileBytes(context.Deserializer, grpName + ".TEX");

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

            // TODO: Just for testing...
            FileFactory.Read<PS1_EDU_AllfixFile>(GetAllfixFilePath(context.Settings), context);
            FileFactory.Read<PS1_EDU_WorldFile>(GetWorldFilePath(context.Settings), context);

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context, levelData.ColorPalettes.First()) : new Common_Design[0];

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

            // Load the .grx file
            var grx = GetGRXFilePath(context.Settings);

            await FileSystem.PrepareFile(context.BasePath + grx);
            context.AddFile(GetFile(context, grx));
        }

        #endregion
    }
}