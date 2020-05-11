using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// Base game manager for PC
    /// </summary>
    public class GBA_R1_Manager : IGameManager {
        #region Values and paths

        /// <summary>
        /// The size of one cell
        /// </summary>
        public const int CellSize = 16;

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            new KeyValuePair<World, int[]>(World.Jungle, new int[]
            {
                1
            }), 
        };

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];
        
        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        /// <param name="level">The level to auto-apply the palette to</param>
        public void AutoApplyPalette(Common_Lev level) {}

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public virtual async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            // TODO: Parse the ROM - find out where the compressed data is stored and how it's compressed

            // Load the rom
            var romPath = "ROM.gba";

            // Load the rom starting from the rom address
            var romFile = new GBAMemoryMappedFile(context, 0x08000000)
            {
                filePath = romPath
            };
            context.AddFile(romFile);

            // For now we're reading memory dump files from the WRAM section
            var memoryPath = "Jungle2.gba";

            // Load the file starting from the WRAM address
            var memoryFile = new GBAMemoryMappedFile(context, 0x02000000)
            {
                filePath = memoryPath
            };
            context.AddFile(memoryFile);

            // Deserialize the data
            var s = context.Deserializer;

            GBA_R1_Map map = null;
            PC_Event[] events = null;
            ushort[] linkTable = null;

            var eventCount = 146;

            s.DoAt(new Pointer(0x02002230, memoryFile), () => map = s.SerializeObject<GBA_R1_Map>(map, name: nameof(map)));
            s.DoAt(new Pointer(0x020226B0, memoryFile), () => events = s.SerializeObjectArray<PC_Event>(events, eventCount, name: nameof(map)));
            s.DoAt(new Pointer(0x0202BB00, memoryFile), () => linkTable = s.SerializeArray<ushort>(linkTable, eventCount, name: nameof(linkTable)));


            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev 
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        Tiles = new Common_Tile[map.Width * map.Height]
                    }
                },

                // Create the events list
                EventData = new List<Common_EventData>(),
            };

            // Load a dummy tile for now
            commonLev.Maps[0].TileSet[0] = new Common_Tileset(Enumerable.Repeat(new ARGBColor(0, 0, 0, 0), 16*16).ToArray(), 1, 16);

            var index = 0;

            // Load the events
            foreach (PC_Event e in events)
            {
                // Add the event
                commonLev.EventData.Add(new Common_EventData
                {
                    Type = e.Type,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,

                    // TODO: Fix once we load sprites and ETA
                    DESKey = "N/A",
                    ETAKey = "N/A",

                    OffsetBX = e.OffsetBX,
                    OffsetBY = e.OffsetBY,
                    OffsetHY = e.OffsetHY,
                    FollowSprite = e.FollowSprite,
                    HitPoints = e.HitPoints,
                    Layer = e.Layer,
                    HitSprite = e.HitSprite,
                    FollowEnabled = e.FollowEnabled,
                    CommandCollection = e.Commands_GBA,
                    LinkIndex = linkTable[index]
                });

                index++;
            }

            await Controller.WaitIfNecessary();

            // Enumerate each cell
            for (int cellY = 0; cellY < map.Height; cellY++) 
            {
                for (int cellX = 0; cellX < map.Width; cellX++) 
                {
                    // Get the cell
                    var cell = map.Tiles[cellY * map.Width + cellX];

                    // Set the common tile
                    commonLev.Maps[0].Tiles[cellY * map.Width + cellX] = new Common_Tile() 
                    {
                        // TODO: Fix once we load tile graphics
                        //TileSetGraphicIndex = textureIndex,
                        CollisionType = cell.CollisionType,
                        PaletteIndex = 1,
                        XPosition = cellX,
                        YPosition = cellY
                    };
                }
            }

            return new GBA_R1_EditorManager(commonLev, context);
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(Context context, Common_Lev commonLevelData) => throw new NotImplementedException();

        public virtual Task LoadFilesAsync(Context context) => Task.CompletedTask;

        #endregion
    }
}