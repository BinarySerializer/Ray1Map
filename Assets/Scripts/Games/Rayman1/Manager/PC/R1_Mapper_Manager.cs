using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Image;
using BinarySerializer.Ray1;
using UnityEngine;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman Mapper (PC)
    /// </summary>
    public class R1_Mapper_Manager : R1_Kit_Manager 
    {
        #region Values and paths

        /// <summary>
        /// Gets the folder path for the specified level
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The level file path</returns>
        public async UniTask<string> GetLevelFolderPath(Context context) {
            var custom = GetWorldName(context.GetR1Settings().R1_World) + "/" + $"MAP{context.GetR1Settings().Level}" + "/";
            await FileSystem.CheckDirectory(context.GetAbsoluteFilePath(custom)); // check this and await, since it's a request in WebGL
            if (FileSystem.DirectoryExists(context.GetAbsoluteFilePath(custom)))
                return custom;

            return GetWorldName(context.GetR1Settings().R1_World) + "/" + $"MAP_{context.GetR1Settings().Level}" + "/";
        }

        public string GetMapFilePath(string levelDir) => levelDir + "EVENT.MAP";
        public string GetMapEventsFilePath(string levelDir) => levelDir + "EVENT.MEV";
        public string GetSaveEventsFilePath(string levelDir) => levelDir + "EVENT.SEV";

        public string GetEventManfiestFilePath(World world) => GetWorldFolderPath(world) + $"EVE.MLT";

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(World world) => GetWorldName(world) + "/";

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
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.EnumerateWorlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateDirectories(settings.GameDirectory + GetWorldFolderPath(w), "MAP*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(x => x.Length < 7)
            .Select(x => Int32.Parse(x.Replace("_", String.Empty).Substring(3)))
            .ToArray())).ToArray());

        #endregion

        #region Manager Methods

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get the level folder path
            var basePath = await GetLevelFolderPath(context);

            // TODO: Parse .ini file to get background etc.
            // Get file paths
            var mapPath = GetMapFilePath(basePath);
            var mapEventsPath = GetMapEventsFilePath(basePath);
            var saveEventsPath = GetSaveEventsFilePath(basePath);
            var pcxPath = GetPCXFilePath(context.GetR1Settings());
            var eventsCsvPath = GetEventManfiestFilePath(context.GetR1Settings().R1_World);

            // Load files to context
            await AddFile(context, mapPath);
            await AddFile(context, mapEventsPath, endianness: Endian.Big); // Big endian just like on Jaguar
            await AddFile(context, pcxPath);

            // Read the files

            Controller.DetailedState = $"Loading map data";
            await Controller.WaitIfNecessary();

            var mapData = FileFactory.Read<Mapper_MapData>(context, mapPath);

            Controller.DetailedState = $"Loading event data";
            await Controller.WaitIfNecessary();

            var mapEvents = FileFactory.Read<MapEvents>(context, mapEventsPath);
            var saveEvents = Ray1TextFileFactory.ReadText<Mapper_SaveEvents>(saveEventsPath, context);
            var csv = Ray1TextFileFactory.ReadText<Mapper_EventManifest>(eventsCsvPath, context);

            Controller.DetailedState = $"Loading tileset";
            await Controller.WaitIfNecessary();

            var pcx = FileFactory.Read<PCX>(context, pcxPath);

            // Get the palette from the PCX file
            var vgaPalette = pcx.VGAPalette;

            // Load the sprites
            var eventDesigns = await LoadSpritesAsync(context, vgaPalette);

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,

                    // Set the dimensions
                    Width = mapData.Width,
                    Height = mapData.Height,

                    // Create the tile arrays
                    TileSet = new Unity_TileSet[1],
                    MapTiles = mapData.Tiles.Select(x => new Unity_Tile(new MapTile()
                    {
                        TileMapY = x.TileIndex,
                        CollisionType = x.BlockType
                    })).ToArray(),
                }
            };

            var allEventInstances = saveEvents.SaveEventInstances.SelectMany(x => x).ToArray();

            // Create a linking table
            var linkTable = new ushort[allEventInstances.Length];

            // Handle each event link group
            foreach (var linkedEvents in allEventInstances.Select((x, i) => new
            {
                Index = i,
                Data = x
            }).GroupBy(x => x.Data.LinkID))
            {
                // Get the group
                var group = linkedEvents.ToArray();

                // Handle every event
                for (int i = 0; i < group.Length; i++)
                {
                    // Get the item
                    var item = group[i];

                    if (group.Length == i + 1)
                        linkTable[item.Index] = (ushort)group[0].Index;
                    else
                        linkTable[item.Index] = (ushort)group[i + 1].Index;
                }
            }

            // Create the object manager
            var objManager = new Unity_ObjectManager_R1(
                context: context, 
                des: eventDesigns.Select((x, i) => new Unity_ObjectManager_R1.DataContainer<Unity_ObjectManager_R1.DESData>(x, i, worldData.DESFileNames?.ElementAtOrDefault(i))).ToArray(), 
                eta: GetCurrentEventStates(context).Select((x, i) => new Unity_ObjectManager_R1.DataContainer<ObjState[][]>(x.States, i, worldData.ETAFileNames?.ElementAtOrDefault(i))).ToArray(), 
                linkTable: linkTable, 
                usesPointers: false,
                hasDefinedDesEtaNames: true);

            Controller.DetailedState = $"Loading events";
            await Controller.WaitIfNecessary();

            var levelEvents = new List<Unity_SpriteObject>();

            // Create events
            for (var i = 0; i < saveEvents.SaveEventInstances.Length; i++)
            {
                // Get the map base position, based on the event map
                var mapPos = mapEvents.EventIndexMap.FindItemIndex(z => z == i + 1);

                // Get the x and y positions
                var mapY = (uint) Math.Floor(mapPos / (double) (mapEvents.Width));
                var mapX = (uint) (mapPos - (mapY * mapEvents.Width));

                // Calculate the actual position on the map
                mapX *= 4 * (uint) Settings.CellSize;
                mapY *= 4 * (uint) Settings.CellSize;

                // Add every instance
                foreach (var instance in saveEvents.SaveEventInstances[i])
                {
                    // Get the definition
                    var def = csv.EventDefinitions.FirstOrDefault(x => x.Name == instance.EventDefinitionKey);

                    if (def == null)
                        throw new Exception($"No matching event definition found for {instance.EventDefinitionKey}");

                    var ed = new ObjData()
                    {
                        Type = def.Type,
                        MainEtat = def.Etat,
                        SubEtat = def.SubEtat,
                        XPosition = (short)(mapX + instance.OffsetX),
                        YPosition = (short)(mapY + instance.OffsetY),
                        OffsetBX = def.OffsetBX,
                        OffsetBY = def.OffsetBY,
                        OffsetHY = def.OffsetHY,
                        FollowSprite = def.FollowSprite,
                        ActualHitPoints = (uint)instance.HitPoints,
                        InitFlag = instance.InitFlag,
                        HitSprite = def.HitSprite,

                        BlockTypes = new BlockType[5],

                        CommandContexts = new CommandContext[]
                        {
                            new CommandContext()
                        },

                        LabelOffsets = new ushort[0],
                        Commands = CommandCollection.FromBytes(def.EventCommands, () => new Ray1MapContext(context.GetR1Settings())),
                    };

                    ed.FollowEnabled = def.FollowEnabled > 0;

                    // Add the event
                    levelEvents.Add(new Unity_Object_R1(
                        eventData: ed, 
                        objManager: objManager, 
                        ETAIndex: worldData.ETAFileNames.FindItemIndex(x => x == def.ETAFile))
                    {
                        DESIndex = worldData.DESFileNames.FindItemIndex(x => x.Length > 4 && x.Substring(0, x.Length - 4) == def.DESFile)
                    });
                }
            }

            // Convert levelData to common level format
            var level = new Unity_Level()
            {
                Maps = maps,
                ObjManager = objManager,
                EventData = levelEvents,
                Rayman = new Unity_Object_R1(ObjData.CreateRayman(context, levelEvents.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == ObjType.TYPE_RAY_POS)?.EventData), objManager)
            };

            Controller.DetailedState = $"Creating tileset";
            await Controller.WaitIfNecessary();

            // Load the tile set
            level.Maps[0].TileSet[0] = LoadTileSet(pcx);

            // Return the level
            return level;
        }

        public Unity_TileSet LoadTileSet(PCX pcx)
        {
            // Get the tilemap texture
            var tileMap = pcx.ToTexture();

            var tileSetWidth = tileMap.width / Settings.CellSize;
            var tileSetHeight = tileMap.height / Settings.CellSize;

            // Create the tile array
            var tiles = new Unity_TileTexture[tileSetWidth * tileSetHeight];

            // Get the transparency color
            var transparencyColor = tileMap.GetPixel(0, 0);

            // Replace the transparency color with true transparency
            for (int y = 0; y < tileMap.height; y++)
            {
                for (int x = 0; x < tileMap.width; x++)
                {
                    if (tileMap.GetPixel(x, y) == transparencyColor)
                        tileMap.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            tileMap.Apply();

            // Split the .pcx into a tile-set
            for (int ty = 0; ty < tileSetHeight; ty++)
            {
                for (int tx = 0; tx < tileSetWidth; tx++)
                {
                    // Create a tile
                    tiles[ty * tileSetWidth + tx] = tileMap.CreateTile(new RectInt(tx * Settings.CellSize, ty * Settings.CellSize, Settings.CellSize, Settings.CellSize));
                }
            }

            return new Unity_TileSet(tiles);
        }

        #endregion
    }
}
