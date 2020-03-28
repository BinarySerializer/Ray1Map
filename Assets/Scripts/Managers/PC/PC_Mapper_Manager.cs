using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Mapper (PC)
    /// </summary>
    public class PC_Mapper_Manager : PC_RD_Manager {
        #region Values and paths

        /// <summary>
        /// Gets the folder path for the specified level
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The level file path</returns>
        public async Task<string> GetLevelFolderPath(Context context) {
            var custom = GetWorldName(context.Settings.World) + "/" + $"MAP{context.Settings.Level}" + "/";
            await FileSystem.CheckDirectory(context.BasePath + custom); // check this and await, since it's a request in WebGL
            if (FileSystem.DirectoryExists(context.BasePath + custom))
                return custom;

            return GetWorldName(context.Settings.World) + "/" + $"MAP_{context.Settings.Level}" + "/";
        }

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(GameSettings settings) => GetWorldName(settings.World) + "/";

        /// <summary>
        /// Gets the file path for the PCX tile map
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The PCX tile map file path</returns>
        public string GetPCXFilePath(GameSettings settings) => GetWorldFolderPath(settings) + $"{GetShortWorldName(settings.World)}.PCX";

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) {
            return Directory.EnumerateDirectories(settings.GameDirectory + GetWorldFolderPath(settings), "MAP???", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(x => x.Length < 7)
                .Select(x => Int32.Parse(x.Replace("_", String.Empty).Substring(3)))
                .ToArray();
        }

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the localization files for each event, with the language tag as the key
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <returns>The localization files</returns>
        public Dictionary<string, PC_Mapper_EventLocFile[]> GetEventLocFiles(string basePath)
        {
            var pcDataDir = Path.Combine(basePath, GetDataPath());

            var output = new Dictionary<string, PC_Mapper_EventLocFile[]>();

            foreach (var langDir in Directory.GetDirectories(pcDataDir, "???", SearchOption.TopDirectoryOnly))
            {
                string[] locFiles = Directory.GetFiles(langDir, "*.wld", SearchOption.TopDirectoryOnly);
                output.Add(Path.GetFileName(langDir), locFiles.Select(locFile => {
                    var locFileRelative = locFile.Substring(basePath.Length);
                    using (Context c = new Context(new GameSettings(GameModeSelection.MapperPC, basePath))) {
                        c.AddFile(GetFile(c, locFileRelative));
                        return FileFactory.Read<PC_Mapper_EventLocFile>(locFileRelative, c);
                    }
                }).ToArray());
            }

            return output;
        }

        public async Task LoadExtraFile(Context context, string path) {
            await FileSystem.PrepareFile(context.BasePath + path);
            context.AddFile(GetFile(context, path));
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            Controller.status = $"Loading Mapper map data for {context.Settings.World} {context.Settings.Level}";

            // Get the level folder path
            var basePath = await GetLevelFolderPath(context);

            // Load new files
            Dictionary<string, string> paths = new Dictionary<string, string>
            {
                ["event.map"] = basePath + "EVENT.MAP",
                ["ray.lev"] = basePath + "RAY.LEV",
                ["pcx"] = GetPCXFilePath(context.Settings)
            };
            foreach (KeyValuePair<string, string> path in paths) {
                await LoadExtraFile(context, path.Value);
            }

            // Read the map data
            var mapData = FileFactory.Read<Mapper_Map>(paths["event.map"], context);

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
                // Set the dimensions
                Width = mapData.Width,
                Height = mapData.Height,

                // Create the events list
                Events = new List<Common_Event>(),

                // Create the tile arrays
                TileSet = new Common_Tileset[4],
                Tiles = new Common_Tile[mapData.Width * mapData.Height],
            };

            Controller.status = $"Loading Mapper files";

            // Read the DES CMD manifest
            var desCmdManifest = FileFactory.ReadMapper<Mapper_RayLev>(paths["ray.lev"], context).DESManifest;

            // Read the CMD files
            Dictionary<string, Mapper_EventCMD> cmd = new Dictionary<string, Mapper_EventCMD>();
            foreach (KeyValuePair<string, string> item in desCmdManifest.Skip<KeyValuePair<string, string>>(1)) {
                await Controller.WaitIfNecessary();
                string path = basePath + item.Value;
                await LoadExtraFile(context, path);
                cmd.Add(item.Key, FileFactory.ReadMapper<Mapper_EventCMD>(path, context));
            }

            await Controller.WaitIfNecessary();

            // Get the palette from the PCX file
            var vgaPalette = FileFactory.Read<PCX>(paths["pcx"], context).VGAPalette;

            var palette = new List<ARGBColor>();
            for (var i = 0; i < vgaPalette.Length; i += 3)
                palette.Add(new ARGBColor(vgaPalette[i + 0], vgaPalette[i + 1], vgaPalette[i + 2]));

            // Load the sprites
            var eventDesigns = await LoadSpritesAsync(context, palette);

            // Add the events
            commonLev.Events = new List<Common_Event>();

            var index = 0;

            // Get the event count
            var eventCount = cmd.SelectMany<KeyValuePair<string, Mapper_EventCMD>, PC_Mapper_EventCMDItem>(x => x.Value.Items).Count<PC_Mapper_EventCMDItem>();

            // Get the Designer DES and ETA names
            var kitDESNames = GetDESNames(context).ToArray<string>();
            var kitETANames = GetETANames(context).ToArray<string>();

            // Handle each event
            foreach (var c in cmd)
            {
                foreach (var e in c.Value.Items)
                {
                    Controller.status = $"Loading event {index}/{eventCount}";

                    await Controller.WaitIfNecessary();

                    // Get the DES index
                    var desIndex = kitDESNames.FindItemIndex<string>(x => x == c.Key);

                    // Get the ETA index
                    var etaIndex = kitETANames.FindItemIndex<string>(x => x == e.ETAFile);

                    if (desIndex != -1)
                        desIndex += 1;

                    var ee = Controller.obj.levelEventController.AddEvent((EventType)(Int32.TryParse(e.Obj_type, out var r1) ? r1 : -1), (int)e.Etat, Int32.TryParse(e.SubEtat, out var r2) ? r2 : -1, (uint)e.XPosition, (uint)e.YPosition, desIndex, etaIndex, (int)e.Offset_BX, (int)e.Offset_BY, (int)e.Offset_HY, (int)e.Follow_sprite, (int)e.Hitpoints, (int)e.Hit_sprite, e.Follow_enabled > 0, new ushort[0], Common_EventCommandCollection.FromBytes(e.EventCommands.Select(x => (byte)x).ToArray()), 

                        // TODO: Update this
                        index);

                    // Add the event
                    commonLev.Events.Add(ee);

                    index++;
                }
            }

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading tile set";

            // Read the .pcx file and get the texture
            var pcxtex = FileFactory.Read<PCX>(paths["pcx"], context).ToTexture();

            var tileSetWidth = pcxtex.width / CellSize;
            var tileSetHeight = pcxtex.height / CellSize;

            // Create the tile array
            var tiles = new Tile[tileSetWidth * tileSetHeight];

            // Get the transparency color
            var transparencyColor = pcxtex.GetPixel(0, 0);
            
            // Replace the transparency color with true transparency
            for (int y = 0; y < pcxtex.height; y++)
            {
                for (int x = 0; x < pcxtex.width; x++)
                {
                    if (pcxtex.GetPixel(x, y) == transparencyColor) 
                        pcxtex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            pcxtex.Apply();

            // Split the .pcx into a tile-set
            for (int ty = 0; ty < tileSetHeight; ty++)
            {
                for (int tx = 0; tx < tileSetWidth; tx++)
                {
                    // Create a tile
                    Tile t = ScriptableObject.CreateInstance<Tile>();
                    t.sprite = Sprite.Create(pcxtex, new Rect(tx * CellSize, ty * CellSize, CellSize, CellSize), new Vector2(0.5f, 0.5f), CellSize, 20);

                    // Set the tile
                    tiles[ty * tileSetWidth + tx] = t;
                }
            }

            // Set the tile-set
            commonLev.TileSet[0] = new Common_Tileset(tiles);

            // Set the tiles
            commonLev.Tiles = new Common_Tile[mapData.Width * mapData.Height];

            int tileIndex = 0;
            for (int ty = 0; ty < (mapData.Height); ty++)
            {
                for (int tx = 0; tx < (mapData.Width); tx++)
                {
                    Common_Tile newTile = new Common_Tile
                    {
                        PaletteIndex = 1,
                        XPosition = tx,
                        YPosition = ty,
                        CollisionType = mapData.Tiles[tileIndex].CollisionType,
                        TileSetGraphicIndex = mapData.Tiles[tileIndex].TileIndex
                    };

                    commonLev.Tiles[tileIndex] = newTile;

                    tileIndex++;
                }
            }

            // Return an editor manager
            return new MapperEditorManager(commonLev, context, this, eventDesigns);
        }

        #endregion
    }
}