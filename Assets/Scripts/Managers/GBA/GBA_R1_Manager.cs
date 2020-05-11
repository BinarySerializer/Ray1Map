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
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="e">The event</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public virtual Texture2D GetSpriteTexture(Context context, PC_Event e, Common_ImageDescriptor s)
        {
            if (s.Index == 0)
                return null;

            // Create the texture
            Texture2D tex = new Texture2D(s.OuterWidth, s.OuterHeight, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    // TODO: Fix and load palettes

                    var value = e.ImageBuffer[s.ImageBufferOffset + (y * tex.width + x)];

                    if (value == 0)
                        tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                    else
                        tex.SetPixel(x, y, new Color(value / 255f, value / 255f, value / 255f));
                }
            }

            tex.Apply();

            return tex;
        }

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

            GBA_R1_Level[] levels = null;
            GBA_R1_Map map = null;
            PC_Event[] events = null;
            ushort[] linkTable = null;

            var eventCount = 146;

            // Parse rom
            s.DoAt(new Pointer(0x085485B4, romFile), () => levels = s.SerializeObjectArray<GBA_R1_Level>(levels, 22+18+13+13+12+4+6, name: nameof(levels)));

            // Parse memory files
            s.DoAt(new Pointer(0x02002230, memoryFile), () => map = s.SerializeObject<GBA_R1_Map>(map, name: nameof(map)));
            s.DoAt(new Pointer(0x020226B0, memoryFile), () => events = s.SerializeObjectArray<PC_Event>(events, eventCount, name: nameof(map)));

            // Doesn't seem correct
            s.DoAt(new Pointer(0x0202BB00, memoryFile), () => linkTable = s.SerializeArray<ushort>(linkTable, eventCount, name: nameof(linkTable)));

            //Util.ExportPointerArray(s, @"C:\Users\RayCarrot\Downloads\Pointer_00.txt", levels.Select(x => x.Pointer_00));
            //Util.ExportPointerArray(s, @"C:\Users\RayCarrot\Downloads\Pointer_04.txt", levels.Select(x => x.Pointer_04));
            //Util.ExportPointerArray(s, @"C:\Users\RayCarrot\Downloads\Pointer_08.txt", levels.Select(x => x.Pointer_08));
            //Util.ExportPointerArray(s, @"C:\Users\RayCarrot\Downloads\Pointer_0B.txt", levels.Select(x => x.Pointer_0B));
            //Util.ExportPointerArray(s, @"C:\Users\RayCarrot\Downloads\WorldPointer.txt", levels.Select(x => x.WorldPointer));

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

            var eventDesigns = new Dictionary<Pointer, Common_Design>();
            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();

            var index = 0;

            // Load the events
            foreach (PC_Event e in events)
            {
                // Add if not found
                if (e.ImageDescriptorsPointer_GBA != null && !eventDesigns.ContainsKey(e.ImageDescriptorsPointer_GBA))
                {
                    Common_Design finalDesign = new Common_Design
                    {
                        Sprites = new List<Sprite>(),
                        Animations = new List<Common_Animation>(),
                        FilePath = e.ImageDescriptorsPointer_GBA.file.filePath
                    };

                    // Get every sprite
                    foreach (Common_ImageDescriptor i in e.ImageDescriptors)
                    {
                        // Get the texture for the sprite, or null if not loading textures
                        Texture2D tex = loadTextures ? GetSpriteTexture(context, e, i) : null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                    }

                    // TODO: Clean this up - maybe inherit from the PS1 manager?
                    // Add animations
                    finalDesign.Animations.AddRange(e.AnimDescriptors.Select(x => new PS1_R1_Manager().GetCommonAnimation(x)));

                    // Add to the designs
                    eventDesigns.Add(e.ImageDescriptorsPointer_GBA, finalDesign);
                }

                // Add if not found
                if (e.ETAPointer_GBA != null && !eventETA.ContainsKey(e.ETAPointer_GBA))
                    // Add to the ETA
                    eventETA.Add(e.ETAPointer_GBA, e.ETA_GBA);

                // Add the event
                commonLev.EventData.Add(new Common_EventData
                {
                    Type = e.Type,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    DESKey = e.ImageDescriptorsPointer_GBA?.ToString() ?? String.Empty,
                    ETAKey = e.ETAPointer_GBA?.ToString() ?? String.Empty,
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

            return new PS1EditorManager(commonLev, context, eventDesigns, eventETA);
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