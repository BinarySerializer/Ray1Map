using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.PS1;
using BinarySerializer.Ray1.PS1;
using UnityEngine;
using Sprite = BinarySerializer.Ray1.Sprite;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_PS1R2_Manager : R1_PS1BaseManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        /// <summary>
        /// The amount of available maps for the demo level
        /// </summary>
        public const int MapCount = 4;

        public string FixDataPath => $"RAY.DTA";
        public string FixGraphicsPath => "RAY.GRP";
        public string SpritePalettesPath => "SPR.PLS";
        public string GetLevelGraphicsPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.R1_World)}01.GRP";
        public string GetLevelImageDescriptorsPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.R1_World)}01.SPR";
        public string GetLevelDataPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.R1_World)}01.DTA";

        public string GetSubMapTilesetPath(int level) => $"JUNGLE/{GetMapName(level)}.RAW";
        public string GetSubMapPalettePath(int level) => $"JUNGLE/{GetMapName(level)}.PAL";
        public string GetSubMapPath(int level) => $"JUNGLE/{GetMapName(level)}.MPU";

        public static Dictionary<string, uint> FileSizes { get; } = new Dictionary<string, uint>() 
        {
            ["LOGO_UBI.TIM"] = 0x25818,
            ["../VIDEO/15FPS.STR"] = 0x3BD8E0,
            ["RAY.INF"] = 0xA5,
            ["RAY.DTA"] = 0x459E6,
            ["RAY.GRP"] = 0x15100,
            ["SPR.PLS"] = 0x1000,
            ["JUNGLE/JUN01.DTA"] = 0x25CC8,
            ["JUNGLE/JUN01.SPR"] = 0x10B0,
            ["JUNGLE/JUN01.GRP"] = 0x53B00,
            ["JUNGLE/PL1.RAW"] = 0x19000,
            ["JUNGLE/PL2.RAW"] = 0x15000,
            ["JUNGLE/FD1.RAW"] = 0x10000,
            ["JUNGLE/NEWFND16.TIM"] = 0x1F414,
            ["JUNGLE/JUN!01.PLT"] = 0x22D,
            ["JUNGLE/FD1.PAL"] = 0x200,
            ["JUNGLE/FD2.PAL"] = 0x200,
            ["JUNGLE/PL1.MPU"] = 0x81E8,
            ["JUNGLE/PL2.MPU"] = 0x2C9E,
            ["JUNGLE/FD1.MPU"] = 0x3F4,
            ["JUNGLE/FD2.MPU"] = 0x164,
            ["RAY.BBX"] = 0x41520,
            ["JUNGLE/JUN.BBX"] = 0
        };

        protected override MemoryMappedPS1File.InvalidPointerMode InvalidPointerMode => MemoryMappedPS1File.InvalidPointerMode.Allow;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
            {
                new GameInfo_World(1, new []
                {
                    0
                }), 
            }).ToArray();

        public override string ExeFilePath => null;
        public override uint? ExeBaseAddress => null;

        /// <summary>
        /// Gets the name for the specified map
        /// </summary>
        /// <param name="map">The map</param>
        /// <returns>The map name</returns>
        public virtual string GetMapName(int map)
        {
            switch (map)
            {
                case 0:
                    return "FD2";

                case 1:
                    return "FD1";

                case 2:
                    return "PL2";

                case 3:
                    return "PL1";

                default:
                    throw new ArgumentOutOfRangeException(nameof(map));
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="map">The map</param>
        /// <returns>The tile set to use</returns>
        public Unity_TileSet GetTileSet(Context context, int map)
        {
            string tileSetPath = GetSubMapTilesetPath(map);
            string palettePath = GetSubMapPalettePath(map);
            
            var tileSet = FileFactory.Read<Array<byte>>(context, tileSetPath, (s, x) => x.Pre_Length = s.CurrentLength);
            var palette = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, palettePath, (s, x) => x.Pre_Length = s.CurrentLength / 2);

            return new Unity_TileSet(tileSet, new Unity_Palette(palette.Value), Unity_TextureFormat.Indexed_8, TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_TileSet GetTileSet(Context context) => throw new NotImplementedException();

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, PS1VramHelpers.VRAMMode mode)
        {
            // Read the files
            var fixGraphics = FileFactory.Read<Array<byte>>(context, FixGraphicsPath, onPreSerialize: (s, a) => a.Pre_Length = s.CurrentLength);
            var lvlGraphics = FileFactory.Read<Array<byte>>(context, GetLevelGraphicsPath(context.GetR1Settings()), onPreSerialize: (s, a) => a.Pre_Length = s.CurrentLength);
            var palettes = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, SpritePalettesPath, onPreSerialize: (s, a) => a.Pre_Length = s.CurrentLength / 2);

            var tilePalettes = new RGBA5551Color[4][];
            for (int i = 0; i < MapCount; i++)
                tilePalettes[i] = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, GetSubMapPalettePath(i), onPreSerialize: (s, a) => a.Pre_Length = s.CurrentLength / 2).Value;

            var vram = PS1VramHelpers.PS1_R2_FillVRAM(fixGraphics.Value, lvlGraphics.Value, palettes.Value, tilePalettes);

            context.StoreObject("vram", vram);
        }

        public async UniTask<uint> LoadFile(Context context, string path, uint baseAddress) {
            await FileSystem.PrepareFile(context.GetAbsoluteFilePath(path));

            if (baseAddress != 0) {
                MemoryMappedPS1File file = new MemoryMappedPS1File(context, path, baseAddress, InvalidPointerMode, fileLength: FileSizes[path]);
                context.AddFile(file);

                return FileSizes[path];
            } else {
                LinearFile file = new LinearFile(context, path, fileLength: FileSizes.ContainsKey(path) ? FileSizes[path] : (long?)null);
                context.AddFile(file);
                return 0;
            }
        }

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading files";

            uint baseAddress = 0x80018000;

            var fixDTAPath = FixDataPath;
            var fixGRPPath = FixGraphicsPath;
            var sprPLSPath = SpritePalettesPath;
            var levelDTAPath = GetLevelDataPath(context.GetR1Settings());
            var levelSPRPath = GetLevelImageDescriptorsPath(context.GetR1Settings());
            var levelGRPPath = GetLevelGraphicsPath(context.GetR1Settings());

            baseAddress += await LoadFile(context, fixDTAPath, baseAddress);
            baseAddress -= 94; // FIX.DTA header size
            Pointer fixDTAHeader = new Pointer(baseAddress, context.FilePointer(fixDTAPath).File);

            R2_AllfixData footer = null;

            context.Deserializer.DoAt(fixDTAHeader, () => footer = context.Deserializer.SerializeObject<R2_AllfixData>(null, name: "AllfixFooter"));
            await LoadFile(context, fixGRPPath, 0);
            await LoadFile(context, sprPLSPath, 0);
            baseAddress += await LoadFile(context, levelSPRPath, baseAddress);
            baseAddress += await LoadFile(context, levelDTAPath, baseAddress);
            await LoadFile(context, levelGRPPath, 0);

            // Load every map
            for (int i = 0; i < 4; i++)
            {
                await LoadFile(context, GetSubMapPalettePath(i), 0);
                await LoadFile(context, GetSubMapTilesetPath(i), 0);
                await LoadFile(context, GetSubMapPath(i), 0);
            }

            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading level data";

            // Read the level data
            var lvlData = FileFactory.Read<R2_LevelData>(context, levelDTAPath);

            // Read the map blocks
            var maps = Enumerable.Range(0, MapCount).Select(x => FileFactory.Read<MapData>(context, GetSubMapPath(x))).ToArray();

            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading sprite data";

            var commonEvents = new List<Unity_SpriteObject>();

            // Get the v-ram
            FillVRAM(context, PS1VramHelpers.VRAMMode.Level);


            // Create the global design list

            var lvlImgDescriptors = FileFactory.Read<ObjectArray<Sprite>>(context, levelSPRPath, onPreSerialize: (s, a) => a.Pre_Length = s.CurrentLength / 0xC).Value;

            var imgDescriptors = lvlData.Level.FixSprites.Concat(lvlImgDescriptors).ToArray();

            // Get every sprite
            var globalDesigns = imgDescriptors.Select(img => GetSpriteTexture(context, null, img)).Select(tex => tex == null ? null : tex.CreateSprite()).ToArray();

            // Get the events
            var events = lvlData.Level.R2_Objects.Concat(lvlData.Level.AlwaysObjects).ToArray();

            Controller.DetailedState = $"Loading animations";
            await Controller.WaitIfNecessary();

            // Get the animation groups
            var r2AnimGroups = events.Select(x => x.AnimSet).Append(footer.RaymanAnimSet).Where(x => x != null).Distinct().ToArray();
            Unity_ObjectManager_R2.AnimGroup[] animGroups = new Unity_ObjectManager_R2.AnimGroup[r2AnimGroups.Length];
            for (int i = 0; i < animGroups.Length; i++) {
                animGroups[i] = await getGroupAsync(r2AnimGroups[i]);
                await Controller.WaitIfNecessary();
            }

            async UniTask<Unity_ObjectManager_R2.AnimGroup> getGroupAsync(AnimationSet animGroup) 
            {
                await UniTask.CompletedTask;

                // Add DES and ETA
                return new Unity_ObjectManager_R2.AnimGroup(
                    pointer: animGroup?.Offset, eta: animGroup?.ETA.States ?? new ObjState[0][], 
                    animations: animGroup?.Animations?.Select(x => AnimationHelpers.ToCommonAnimation(x.R2_Layers.Select(x => x.Value).SelectMany(l => l).ToArray(), x.LayersCount, x.FramesCount)).ToArray(), 
                    filePath: animGroup?.AnimationsPointer?.File.FilePath);
            }

            var objManager = new Unity_ObjectManager_R2(
                context: context, 
                linkTable: lvlData.ObjectLinkTable, 
                animGroups: animGroups, 
                sprites: globalDesigns, 
                imageDescriptors: imgDescriptors,
                levData: lvlData);

            Controller.DetailedState = $"Loading events";
            await Controller.WaitIfNecessary();

            // Add every event
            foreach (var e in events)
            {
                // Add the event
                commonEvents.Add(new Unity_Object_R2(e, objManager)
                {
                    IsAlwaysEvent = lvlData.Level.AlwaysObjects.Contains(e)
                });
            }

            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading tiles";

            var levelMaps = maps.Select((x, i) => new Unity_Map() {
                Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                Layer = i < 3 ? Unity_Map.MapLayer.Back : Unity_Map.MapLayer.Middle,

                // Set the dimensions
                Width = x.Width,
                Height = x.Height,

                // Create the tile array
                TileSet = new Unity_TileSet[1],
                TileSetWidth = TileSetWidth
            }).ToArray();

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level()
            {
                Maps = levelMaps, 
                ObjManager = objManager, 
                EventData = commonEvents,
                Rayman = new Unity_Object_R2(R2_ObjData.GetRayman(events.FirstOrDefault(x => x.Type == R2_ObjType.TYPE_RAY_POS), footer), objManager),
                GetCollisionTypeNameFunc = x => ((R2_BlockType)x).ToString(),
                GetCollisionTypeGraphicFunc = x => ((R2_BlockType)x).GetCollisionTypeGraphic(),
            };

            await Controller.WaitIfNecessary();

            // Load maps
            for (int i = 0; i < MapCount; i++)
            {
                // Get the tile set
                Unity_TileSet tileSet = GetTileSet(context, i);

                level.Maps[i].TileSet[0] = tileSet;

                // Set the tiles
                level.Maps[i].MapTiles = maps[i].Blocks.Select(x => new Unity_Tile(MapTile.FromR1MapTile(x, isR2: true))).ToArray();
            }

            // Return the level
            return level;
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("LOGO_UBI.TIM", 320),
            new PS1VignetteFileInfo("JUNGLE/NEWFND16.TIM", 320),
        };

        /// <summary>
        /// Gets the base directory name for exporting a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="des">The design to export</param>
        /// <returns>The base directory name</returns>
        protected override string GetExportDirName(GameSettings settings, Unity_ObjGraphics des)
        {
            // Since all paths are in allfix, we return an empty path
            return String.Empty;
        }

        public override UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames) => throw new NotSupportedException("Rayman 2 does not have menu sprites");
        protected override ExecutableConfig GetExecutableConfig => null;

        /// <summary>
        /// Exports every animation frame from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <returns>The task</returns>
        public override async UniTask ExportAllAnimationFramesAsync(GameSettings baseGameSettings, string outputDir)
        {
            // Create the context
            using (var context = new Ray1MapContext(baseGameSettings))
            {
                // Load the level
                var level = await LoadAsync(context);

                var objManager = (Unity_ObjectManager_R2)level.ObjManager;
                var sprites = objManager.Sprites.Select(x => x?.texture).ToArray();

                var index = 0;

                // Enumerate every animation group
                for (var i = 0; i < objManager.AnimGroups.Length; i++)
                {
                    var des = objManager.AnimGroups[i];

                    if (des?.Animations?.Any() != true)
                        continue;

                    // Find all events where this DES is used
                    var matchingEvents = level.EventData.Append(level.Rayman).Cast<Unity_Object_R2>().Where(x => x.AnimGroupIndex == i);

                    // Find matching ETA for this DES from the level events
                    var matchingStates = matchingEvents.SelectMany(lvlEvent => objManager.AnimGroups[lvlEvent.AnimGroupIndex].ETA.SelectMany(x => x)).Distinct().ToArray();

                    ExportAnimationFrames(sprites, des.Animations, Path.Combine(outputDir, "Jungle", index.ToString()), matchingStates);

                    index++;
                }

                // Unload textures
                await Resources.UnloadUnusedAssets();
            }
        }
    }
}