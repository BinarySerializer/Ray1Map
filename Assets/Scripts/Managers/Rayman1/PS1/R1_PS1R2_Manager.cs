using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
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


        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => PS1FileInfo.fileInfoR2PS1;

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

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

        public override string GetExeFilePath => null;

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
        public Unity_MapTileMap GetTileSet(Context context, int map) {
            var tileSetPath = GetSubMapTilesetPath(map);
            var palettePath = GetSubMapPalettePath(map);
            var tileSet = FileFactory.Read<Array<byte>>(tileSetPath, context, (s, x) => x.Length = s.CurrentLength);
            var palette = FileFactory.Read<ObjectArray<ARGB1555Color>>(palettePath, context, (s, x) => x.Length = s.CurrentLength / 2);

            return new Unity_MapTileMap(tileSet.Value.Select(ind => palette.Value[ind]).ToArray(), TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_MapTileMap GetTileSet(Context context) => throw new NotImplementedException();

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, VRAMMode mode)
        {
            // Read the files
            var fixGraphics = FileFactory.Read<Array<byte>>(FixGraphicsPath, context, onPreSerialize: (s,a) => a.Length = s.CurrentLength);
            var lvlGraphics = FileFactory.Read<Array<byte>>(GetLevelGraphicsPath(context.Settings), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength);
            var palettes = FileFactory.Read<ObjectArray<ARGB1555Color>>(SpritePalettesPath, context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 2);

            var tilePalettes = new ObjectArray<ARGB1555Color>[4];
            for (int i = 0; i < MapCount; i++)
                tilePalettes[i] = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetSubMapPalettePath(i), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 2);
            
            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.currentXPage = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * (256 * 2)];
            Array.Copy(fixGraphics.Value, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[fixGraphics.Value.Length - cageSprites.Length];
            Array.Copy(fixGraphics.Value, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            /*byte[] unknown = new byte[128 * 8];
            vram.AddData(unknown, 128);*/
            vram.AddData(cageSprites, 128);
            vram.AddData(allFixSprites, 256);

            vram.AddData(lvlGraphics.Value, 256);

            // Palettes start at y = 256 + 234 (= 490), so page 1 and y=234
            int paletteY = 240;
            vram.AddDataAt(0, 0, 0, paletteY, palettes.Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            paletteY = 248;
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[3].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[2].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[2].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[2].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[2].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[1].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[0].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[0].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            /*vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
            /*vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            paletteY += 13 - world.TilePalettes.Length;

            foreach (var p in world.TilePalettes)
                vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
            vram.AddDataAt(0, 0, 0, paletteY, palettes.Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512); 

            context.StoreObject("vram", vram);
            //PaletteHelpers.ExportVram(context.Settings.GameDirectory + "vram.png", vram);
        }

        public async UniTask<uint> LoadFile(Context context, string path, uint baseAddress) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = GetFileInfo(context.Settings);
            if (baseAddress != 0) {
                PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, baseAddress, InvalidPointerMode) {
                    filePath = path,
                    Length = fileInfo[path].Size
                };
                context.AddFile(file);

                return fileInfo[path].Size;
            } else {
                LinearSerializedFile file = new LinearSerializedFile(context) {
                    filePath = path,
                    length = fileInfo.ContainsKey(path) ? fileInfo[path].Size : 0
                };
                context.AddFile(file);
                return 0;
            }
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading files";

            uint baseAddress = 0x80018000;

            var fixDTAPath = FixDataPath;
            var fixGRPPath = FixGraphicsPath;
            var sprPLSPath = SpritePalettesPath;
            var levelDTAPath = GetLevelDataPath(context.Settings);
            var levelSPRPath = GetLevelImageDescriptorsPath(context.Settings); // SPRites?
            var levelGRPPath = GetLevelGraphicsPath(context.Settings); // GRaPhics/graphismes

            baseAddress += await LoadFile(context, fixDTAPath, baseAddress);
            baseAddress -= 94; // FIX.DTA header size
            Pointer fixDTAHeader = new Pointer(baseAddress, context.FilePointer(fixDTAPath).file);

            R1_R2AllfixFooter footer = null;

            context.Deserializer.DoAt(fixDTAHeader, () => footer = context.Deserializer.SerializeObject<R1_R2AllfixFooter>(null, name: "AllfixFooter"));
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
            var lvlData = FileFactory.Read<R1_R2LevDataFile>(levelDTAPath, context);

            // Read the map blocks
            var maps = Enumerable.Range(0, MapCount).Select(x => FileFactory.Read<MapData>(GetSubMapPath(x), context)).ToArray();

            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading sprite data";

            var commonEvents = new List<Unity_Object>();

            if (loadTextures)
                // Get the v-ram
                FillVRAM(context, VRAMMode.Level);


            // Create the global design list
            var globalDesigns = new List<Sprite>();

            var lvlImgDescriptors = FileFactory.Read<ObjectArray<R1_ImageDescriptor>>(levelSPRPath, context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 0xC).Value;

            var imgDescriptors = lvlData.FixImageDescriptors.Concat(lvlImgDescriptors).ToArray();

            // Add every sprite
            foreach (var img in imgDescriptors)
            {
                Texture2D tex = GetSpriteTexture(context, null, img);

                // Add it to the array
                globalDesigns.Add(tex == null ? null : tex.CreateSprite());
            }

            // Get the events
            var events = lvlData.Events.Concat(lvlData.AlwaysEvents).ToArray();
            Controller.DetailedState = $"Loading animations";
            await Controller.WaitIfNecessary();

            // Get the animation groups
            var r2AnimGroups = events.Select(x => x.AnimGroup).Append(footer.RaymanAnimGroup).Distinct().ToArray();
            Unity_ObjectManager_R2.AnimGroup[] animGroups = new Unity_ObjectManager_R2.AnimGroup[r2AnimGroups.Length];
            for (int i = 0; i < animGroups.Length; i++) {
                animGroups[i] = await GetGroup(r2AnimGroups[i]);
                await Controller.WaitIfNecessary();
            }
            async UniTask<Unity_ObjectManager_R2.AnimGroup> GetGroup(R1_R2EventAnimGroup animGroup) {
                await UniTask.CompletedTask;
                // Create the DES
                var des = new Unity_ObjGraphics()
                {
                    Sprites = globalDesigns,
                    Animations = new List<Unity_ObjAnimation>(),
                    FilePath = animGroup?.AnimationDescriptorsPointer?.file.filePath
                };

                // Add animations
                des.Animations.AddRange(animGroup?.AnimationDecriptors?.Select(x => x.ToCommonAnimation()) ?? new Unity_ObjAnimation[0]);

                // Add DES and ETA
                return new Unity_ObjectManager_R2.AnimGroup(animGroup?.Offset, animGroup?.ETA.EventStates ?? new R1_EventState[0][], des);
            }

            var objManager = new Unity_ObjectManager_R2(context, lvlData.EventLinkTable, animGroups, lvlData.ZDC, imgDescriptors);

            Controller.DetailedState = $"Loading events";
            await Controller.WaitIfNecessary();

            // Add every event
            foreach (var e in events)
            {
                // Add the event
                commonEvents.Add(new Unity_Object_R2(e, objManager)
                {
                    IsAlwaysEvent = lvlData.AlwaysEvents.Contains(e)
                });
            }

            await Controller.WaitIfNecessary();
            Controller.DetailedState = $"Loading tiles";

            var levelMaps = maps.Select((x, i) => new Unity_Map()
            {
                // Set the dimensions
                Width = x.Width,
                Height = x.Height,

                // Create the tile array
                TileSet = new Unity_MapTileMap[1],
                TileSetWidth = TileSetWidth
            }).ToArray();

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level(levelMaps, objManager, commonEvents, rayman: new Unity_Object_R2(R1_R2EventData.GetRayman(events.FirstOrDefault(x => x.EventType == R1_R2EventType.RaymanPosition), footer), objManager), getCollisionTypeGraphicFunc: x => ((R2_TileCollsionType)x).GetCollisionTypeGraphic());

            await Controller.WaitIfNecessary();

            // Load maps
            for (int i = 0; i < MapCount; i++)
            {
                // Get the tile set
                Unity_MapTileMap tileSet = GetTileSet(context, i);

                level.Maps[i].TileSet[0] = tileSet;

                // Set the tiles
                level.Maps[i].MapTiles = maps[i].Tiles.Select(x => new Unity_Tile(x)).ToArray();
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

        public override UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames) => throw new NotImplementedException();
    }
}