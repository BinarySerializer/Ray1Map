using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Mapper (PC)
    /// </summary>
    public class R1_Mapper_Manager : R1_Kit_Manager {
        #region Values and paths

        /// <summary>
        /// Gets the folder path for the specified level
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The level file path</returns>
        public async UniTask<string> GetLevelFolderPath(Context context) {
            var custom = GetWorldName(context.Settings.R1_World) + "/" + $"MAP{context.Settings.Level}" + "/";
            await FileSystem.CheckDirectory(context.BasePath + custom); // check this and await, since it's a request in WebGL
            if (FileSystem.DirectoryExists(context.BasePath + custom))
                return custom;

            return GetWorldName(context.Settings.R1_World) + "/" + $"MAP_{context.Settings.Level}" + "/";
        }

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(R1_World world) => GetWorldName(world) + "/";

        /// <summary>
        /// Gets the file path for the PCX tile map
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The PCX tile map file path</returns>
        public string GetPCXFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetShortWorldName(settings.R1_World)}.PCX";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.GetR1Worlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateDirectories(settings.GameDirectory + GetWorldFolderPath(w), "MAP???", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(x => x.Length < 7)
            .Select(x => Int32.Parse(x.Replace("_", String.Empty).Substring(3)))
            .ToArray())).ToArray());

        #endregion

        #region Manager Methods

        public async UniTask LoadExtraFile(Context context, string path) {
            await FileSystem.PrepareFile(context.BasePath + path);
            context.AddFile(GetFile(context, path));
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading Mapper map data for {context.Settings.World} {context.Settings.Level}";

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
            var mapData = FileFactory.Read<MapData>(paths["event.map"], context);

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading Mapper files";

            // Read the DES CMD manifest
            var desCmdManifest = FileFactory.ReadText<R1_Mapper_RayLev>(paths["ray.lev"], context).DESManifest;

            // Read the CMD files
            Dictionary<string, R1_Mapper_EventCMD> cmd = new Dictionary<string, R1_Mapper_EventCMD>();
            foreach (KeyValuePair<string, string> item in desCmdManifest.Skip(1)) {
                await Controller.WaitIfNecessary();
                string path = basePath + item.Value;
                await LoadExtraFile(context, path);
                cmd.Add(item.Key, FileFactory.ReadText<R1_Mapper_EventCMD>(path, context));
            }

            await Controller.WaitIfNecessary();

            // Get the palette from the PCX file
            var vgaPalette = FileFactory.Read<PCX>(paths["pcx"], context).VGAPalette;

            // Load the sprites
            var eventDesigns = loadTextures ? await LoadSpritesAsync(context, vgaPalette) : new Unity_ObjGraphics[0];

            // Read the world data
            var worldData = FileFactory.Read<R1_PC_WorldFile>(GetWorldFilePath(context.Settings), context);

            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    // Set the dimensions
                    Width = mapData.Width,
                    Height = mapData.Height,

                    // Create the tile arrays
                    TileSet = new Unity_MapTileMap[1],
                    MapTiles = mapData.Tiles.Select(x => new Unity_Tile(x)).ToArray(),
                    TileSetWidth = 1
                }
            };

            var index = 0;

            // Get the events
            var events = cmd.SelectMany(x => x.Value.Events.Concat<R1_Mapper_EventDefinition>(x.Value.AlwaysEvents).Select(y => new
            {
                DESFileName = x.Key,
                EventData = y
            })).ToArray();

            // Get the event count
            var eventCount = events.Length;

            // Create a linking table
            var linkTable = new ushort[eventCount];

            // Handle each event link group
            foreach (var linkedEvents in events.Select((x, i) => new
            {
                Index = i,
                Data = x,
                LinkID = x.EventData is R1_Mapper_EventCMDItem ei ? ei.LinkID : -1
            }).GroupBy(x => x.LinkID))
            {
                // Get the group
                var group = linkedEvents.ToArray();

                // Handle every event
                for (int i = 0; i < group.Length; i++)
                {
                    // Get the item
                    var item = group[i];

                    if (item.Data.EventData.Name == "always")
                        linkTable[item.Index] = (ushort)item.Index;
                    else if (group.Length == i + 1)
                        linkTable[item.Index] = (ushort)group[0].Index;
                    else
                        linkTable[item.Index] = (ushort)group[i + 1].Index;
                }
            }

            // Create the object manager
            var objManager = new Unity_ObjectManager_R1(context, eventDesigns.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<Unity_ObjGraphics>(x, i, worldData.DESFileNames?.ElementAtOrDefault(i))).ToArray(), GetCurrentEventStates(context).Select((x, i) => new Unity_ObjectManager_R1.DataContainer<R1_EventState[][]>(x.States, i, worldData.ETAFileNames?.ElementAtOrDefault(i))).ToArray(), linkTable, usesPointers: false);

            // Convert levelData to common level format
            var level = new Unity_Level(maps, objManager);

            // Handle each event
            foreach (var eventData in events)
            {
                Controller.DetailedState = $"Loading event {index}/{eventCount}";

                await Controller.WaitIfNecessary();

                // Get the data
                var e = eventData.EventData;
                
                var ed = new R1_EventData()
                {
                    Type = e.Type,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    OffsetBX = e.OffsetBX,
                    OffsetBY = e.OffsetBY,
                    OffsetHY = e.OffsetHY,
                    FollowSprite = e.FollowSprite,
                    ActualHitPoints = e.HitPoints,
                    Layer = e.DisplayPrio,
                    HitSprite = e.HitSprite,

                    PS1Demo_Unk1 = new byte[40],
                    Unk_98 = new byte[5],

                    LabelOffsets = new ushort[0],
                    Commands = R1_EventCommandCollection.FromBytes(e.EventCommands.Select(x => (byte)x).ToArray(), context.Settings),
                };

                ed.SetFollowEnabled(context.Settings, e.FollowEnabled > 0);

                // Add the event
                level.EventData.Add(new Unity_Object_R1(ed, objManager)
                {
                    DESIndex = worldData.DESFileNames.FindItemIndex(x => x == eventData.DESFileName),
                    ETAIndex = worldData.ETAFileNames.FindItemIndex(x => x == e.ETAFile)
                });

                index++;
            }

            await Controller.WaitIfNecessary();

            Controller.DetailedState = $"Loading tile set";

            // Read the .pcx file and get the texture
            var pcxtex = FileFactory.Read<PCX>(paths["pcx"], context).ToTexture();

            var tileSetWidth = pcxtex.width / Settings.CellSize;
            var tileSetHeight = pcxtex.height / Settings.CellSize;

            // Create the tile array
            var tiles = new Unity_TileTexture[tileSetWidth * tileSetHeight];

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
                    tiles[ty * tileSetWidth + tx] = pcxtex.CreateTile(new Rect(tx * Settings.CellSize, ty * Settings.CellSize, Settings.CellSize, Settings.CellSize));
                }
            }

            // Set the tile-set
            level.Maps[0].TileSet[0] = new Unity_MapTileMap(tiles);

            // Return the level
            return level;
        }

        #endregion
    }
}