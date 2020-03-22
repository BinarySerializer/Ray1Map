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
    public class PC_Mapper_Manager : PC_RD_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings)
        {
            var custom = settings.GameDirectory + GetWorldName(settings.World) + "/" + $"MAP{settings.Level}" + "/";

            if(FileSystem.DirectoryExists(custom))
                return custom;

            return settings.GameDirectory + GetWorldName(settings.World) + "/" + $"MAP_{settings.Level}" + "/";
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
        public override int[] GetLevels(GameSettings settings) => Directory.EnumerateDirectories(settings.GameDirectory + GetWorldFolderPath(settings), "MAP???", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).Where(x => x.Length < 7).Select(x => Int32.Parse(x.Replace("_", String.Empty).Substring(3))).ToArray();

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
                output.Add(Path.GetFileName(langDir), Directory.GetFiles(langDir, "*.wld", SearchOption.TopDirectoryOnly).Select(locFile => FileFactory.Read<PC_Mapper_EventLocFile>(locFile, new Context(new GameSettings(GameModeSelection.MapperPC, basePath)))).ToArray());
            }

            return output;
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public override async Task<Common_Lev> LoadLevelAsync(Context context, List<Common_Design> eventDesigns)
        {
            Controller.status = $"Loading Mapper map data for {context.Settings.World} {context.Settings.Level}";

            // Get the level folder path
            var basePath = GetLevelFilePath(context.Settings);

            // Read the map data
            var mapData = FileFactory.Read<Mapper_Map>(Path.Combine(basePath, $"EVENT.MAP"), context);

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
            var desCmdManifest = FileFactory.ReadMapper<Mapper_RayLev>(Path.Combine(basePath, $"RAY.LEV"), context).DESManifest;

            // Read the CMD files
            var cmd = desCmdManifest.Skip(1).ToDictionary(item => item.Key, 
                item => FileFactory.ReadMapper<Mapper_EventCMD>(Path.Combine(basePath, item.Value), context));

            await Controller.WaitIfNecessary();

            // Get the palette from the PCX file
            var vgaPalette = FileFactory.Read<PCX>(GetPCXFilePath(context.Settings), context).VGAPalette;

            var palette = new List<ARGBColor>();
            for (var i = 0; i < vgaPalette.Length; i += 3)
                palette.Add(new ARGBColor(vgaPalette[i + 0], vgaPalette[i + 1], vgaPalette[i + 2]));

            // Load the sprites
            var eta = await LoadSpritesAsync(context, palette, eventDesigns);

            // Add the events
            commonLev.Events = new List<Common_Event>();

            var index = 0;

            // Get the event count
            var eventCount = cmd.SelectMany(x => x.Value.Items).Count();

            // Get the Designer DES and ETA names
            var kitDESNames = GetDESNames(context).ToArray();
            var kitETANames = GetETANames(context).ToArray();

            // Handle each event
            foreach (var c in cmd)
            {
                foreach (var e in c.Value.Items)
                {
                    Controller.status = $"Loading event {index}/{eventCount}";

                    await Controller.WaitIfNecessary();

                    // Get the DES index
                    var desIndex = kitDESNames.FindItemIndex(x => x == c.Key);

                    // Get the ETA index
                    var etaIndex = kitETANames.FindItemIndex(x => x == e.ETAFile);

                    if (desIndex != -1)
                        desIndex += 1;

                    var ee = Controller.obj.levelEventController.AddEvent(Int32.TryParse(e.Obj_type, out var r1) ? r1 : -1, (int)e.Etat, Int32.TryParse(e.SubEtat, out var r2) ? r2 : -1, (uint)e.XPosition, (uint)e.YPosition, desIndex, etaIndex, (int)e.Offset_BX, (int)e.Offset_BY, (int)e.Offset_HY, (int)e.Follow_sprite, (int)e.Hitpoints, (int)e.Hit_sprite, e.Follow_enabled > 0, new ushort[0], e.EventCommands.Select(x => (byte)x).ToArray(),

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
            var pcxtex = FileFactory.Read<PCX>(GetPCXFilePath(context.Settings), context).ToTexture();

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

            // Return the common level data
            return commonLev;
        }

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public override Common_EditorEventInfo GetEditorEventInfo(GameSettings settings, Common_Event e)
        {
            // Find match
            var match = GetMapperEventInfo(settings.GameModeSelection, settings.World, e.Type, e.Etat, e.SubEtat, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.Commands);

            // Return the editor info
            return new Common_EditorEventInfo(match?.Name, match?.Flag);
        }

        // TODO: Until Designer is merged we need to only add supported events
        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The names of the available events to add</returns>
        public override string[] GetEvents(GameSettings settings)
        {
            var w = settings.World.ToEventWorld();

            return LoadPCEventInfo(settings.GameModeSelection)?.Where(x => x.MapperID != null).Where(x => x.World == EventWorld.All || x.World == w).Select(x => x.Name).ToArray() ?? new string[0];
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
        public override Common_Event AddEvent(GameSettings settings, LevelEventController eventController, int index, uint xPos, uint yPos)
        {
            var w = settings.World.ToEventWorld();

            // Get the event
            var e = LoadPCEventInfo(settings.GameModeSelection).Where(x => x.World == EventWorld.All || x.World == w).ElementAt(index);

            // Add and return the event
            return eventController.AddEvent(e.Type, e.Etat, e.SubEtat, xPos, yPos, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, e.LocalCommands, 0);
        }

        /// <summary>
        /// Gets the event info data which matches the specified values for a Mapper event
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
        /// <param name="localCommands"></param>
        /// <returns>The item which matches the values</returns>
        public GeneralPCEventInfoData GetMapperEventInfo(GameModeSelection mode, World world, int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, byte[] localCommands)
        {
            // Load the event info
            var allInfo = LoadPCEventInfo(mode);

            EventWorld eventWorld = world.ToEventWorld();

            // Find a matching item
            var match = allInfo.FindItem(x => (x.World == eventWorld || x.World == EventWorld.All) &&
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
                                  //x.MapperID == mapperID &&
                                  x.LocalCommands.SequenceEqual(localCommands));

            // Create dummy item if not found
            if (match == null && allInfo.Any())
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat}");

            // Return the item
            return match;
        }

        #endregion
    }
}