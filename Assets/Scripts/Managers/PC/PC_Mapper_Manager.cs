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

        // TODO: Only works for custom levels - not original ones
        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => Path.Combine(settings.GameDirectory, GetWorldName(settings.World), $"MAP{settings.Level}");

        /// <summary>
        /// Gets the file path for the PCX tile map
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The PCX tile map file path</returns>
        public string GetPCXFilePath(GameSettings settings) => Path.Combine(settings.GameDirectory, GetWorldName(settings.World), $"{GetShortWorldName(settings.World)}.PCX");

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) => Directory.EnumerateDirectories(Path.Combine(settings.GameDirectory, GetWorldName(settings.World)), "MAP???", SearchOption.TopDirectoryOnly).Where(x => !x.Contains('_')).Select(x => Int32.Parse(Path.GetFileName(x).Substring(3))).ToArray();

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the localization files for each event, with the language tag as the key
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <returns>The localization files</returns>
        public Dictionary<string, PC_Mapper_EventLocFile[]> GetEventLocFiles(string basePath)
        {
            var pcDataDir = Path.Combine(basePath, "PCMAP");

            var output = new Dictionary<string, PC_Mapper_EventLocFile[]>();

            foreach (var langDir in Directory.GetDirectories(pcDataDir, "???", SearchOption.TopDirectoryOnly))
            {
                output.Add(Path.GetFileName(langDir), Directory.GetFiles(langDir, "*.wld", SearchOption.TopDirectoryOnly).Select(locFile => FileFactory.Read<PC_Mapper_EventLocFile>(locFile, new GameSettings(GameModeSelection.MapperPC, basePath))).ToArray());
            }

            return output;
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public override async Task<Common_Lev> LoadLevelAsync(GameSettings settings, List<Common_Design> eventDesigns)
        {
            Controller.status = $"Loading Mapper map data for {settings.World} {settings.Level}";

            // Get the level folder path
            var basePath = GetLevelFilePath(settings);

            // Read the map data
            var mapData = FileFactory.Read<Mapper_Map>(Path.Combine(basePath, $"EVENT.MAP"), settings, FileMode);

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
            var desCmdManifest = FileFactory.Read<Mapper_RayLev>(Path.Combine(basePath, $"RAY.LEV"), settings, FileMode).DESManifest;

            // Read the CMD files
            var cmd = desCmdManifest.Skip(1).ToDictionary(item => item.Key, 
                item => FileFactory.Read<Mapper_EventCMD>(Path.Combine(basePath, item.Value), settings, FileMode));

            await Controller.WaitIfNecessary();

            // TODO: Better way to get this - same as PCX?
            var dummySettings = new GameSettings(GameModeSelection.RaymanDesignerPC, Settings.GameDirectories[GameModeSelection.RaymanDesignerPC], settings.World);
            var palette = FileFactory.Read<PC_LevFile>(new PC_RD_Manager().GetLevelFilePath(dummySettings), dummySettings).ColorPalettes.First();

            // Load the sprites
            var eta = await LoadSpritesAsync(settings, palette, eventDesigns);

            // Add the events
            commonLev.Events = new List<Common_Event>();

            var index = 0;

            // Get the event count
            var eventCount = cmd.SelectMany(x => x.Value.Items).Count();

            // Get the Designer DES and ETA names
            var kitDESNames = GetDESNames(settings).ToArray();
            var kitETANames = GetETANames(settings).ToArray();

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

                    // Find the matching event info item
                    var eventInfo = EventInfoManager.GetMapperEventInfo(settings.GameModeSelection, settings.World, Int32.TryParse(e.Obj_type, out var r1) ? r1 : -1, (int)e.Etat, Int32.TryParse(e.SubEtat, out var r2) ? r2 : -1, desIndex, etaIndex, (int)e.Offset_BX, (int)e.Offset_BY, (int)e.Offset_HY, (int)e.Follow_sprite, (int)e.Hitpoints, (int)e.Hit_sprite, e.Follow_enabled > 0, e.EventCommands.Select(x => (byte)x).ToArray(), e.Name);

                    // Get animation index from the eta item
                    var etaItem = eventInfo.ETA == -1 ? null : eta[eventInfo.ETA].SelectMany(x => x).FindItem(x => x.Etat == e.Etat && x.SubEtat == eventInfo.SubEtat);
                    int animIndex = etaItem?.AnimationIndex ?? 0;
                    int animSpeed = etaItem?.AnimationSpeed ?? 0;

                    // Instantiate event prefab using LevelEventController
                    var ee = Controller.obj.levelEventController.AddEvent(eventInfo,
                        (uint)e.XPosition,
                        (uint)e.YPosition,
                        
                        // TODO: Update this
                        index,

                        animIndex,
                        animSpeed);

                    // Add the event
                    commonLev.Events.Add(ee);

                    index++;
                }
            }

            await Controller.WaitIfNecessary();

            Controller.status = $"Loading tile set";

            // Read the .pcx file and get the texture
            var pcxtex = FileFactory.Read<PCX>(GetPCXFilePath(settings), settings).ToTexture();

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

        #endregion
    }
}