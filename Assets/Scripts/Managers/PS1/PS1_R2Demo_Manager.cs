using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Manager : PS1_Manager
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
        public string GetLevelGraphicsPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.World)}01.GRP";
        public string GetLevelImageDescriptorsPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.World)}01.SPR";
        public string GetLevelDataPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.World)}01.DTA";

        public string GetSubMapTilesetPath(int level) => $"JUNGLE/{GetMapName(level)}.RAW";
        public string GetSubMapPalettePath(int level) => $"JUNGLE/{GetMapName(level)}.PAL";
        public string GetSubMapPath(int level) => $"JUNGLE/{GetMapName(level)}.MPU";


        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoR2PS1;

        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Enumerable.Range(0, w == World.Jungle ? MapCount : 0).ToArray())).ToArray();

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
                    return "PL1";

                case 1:
                    return "PL2";

                case 2:
                    return "FD1";

                case 3:
                    return "FD2";

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
        public Common_Tileset GetTileSet(Context context, int map) {
            var tileSetPath = GetSubMapTilesetPath(map);
            var palettePath = GetSubMapPalettePath(map);
            var tileSet = FileFactory.Read<Array<byte>>(tileSetPath, context, (s, x) => x.Length = s.CurrentLength);
            var palette = FileFactory.Read<ObjectArray<ARGB1555Color>>(palettePath, context, (s, x) => x.Length = s.CurrentLength / 2);

            return new Common_Tileset(tileSet.Value.Select(ind => palette.Value[ind]).ToArray(), TileSetWidth, CellSize);
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Common_Tileset GetTileSet(Context context) => throw new NotImplementedException();

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override void FillVRAM(Context context) {
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
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[0].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[1].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[1].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[1].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[1].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[2].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[3].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, tilePalettes[3].Value.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
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

        /// <summary>
        /// Gets a common animation
        /// </summary>
        /// <param name="animationDescriptor">The animation descriptor</param>
        /// <returns>The common animation</returns>
        public virtual Common_Animation GetCommonR2Animation(PS1_R2Demo_AnimationDecriptor animationDescriptor)
        {
            // Create the animation
            var animation = new Common_Animation
            {
                Frames = new Common_AnimFrame[animationDescriptor.FrameCount],
            };

            // Create each frame
            for (int i = 0; i < animationDescriptor.FrameCount; i++)
            {
                // Create the frame
                var frame = new Common_AnimFrame()
                {
                    FrameData = animationDescriptor.Frames[i],
                    Layers = new Common_AnimationPart[animationDescriptor.LayersPerFrame]
                };

                // Create each layer
                for (var layerIndex = 0; layerIndex < animationDescriptor.LayersPerFrame; layerIndex++)
                {
                    var animationLayer = animationDescriptor.Layers[i][layerIndex];

                    // Create the animation part
                    var part = new Common_AnimationPart
                    {
                        ImageIndex = animationLayer.ImageIndex,
                        XPosition = animationLayer.XPosition,
                        YPosition = animationLayer.YPosition,
                        IsFlippedHorizontally = animationLayer.IsFlippedHorizontally,
                        IsFlippedVertically = animationLayer.IsFlippedVertically
                    };

                    // Add the part
                    frame.Layers[layerIndex] = part;
                }

                // Set the frame
                animation.Frames[i] = frame;
            }

            return animation;
        }

        public async Task<uint> LoadFile(Context context, string path, uint baseAddress) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = FileInfo;
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
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            await Controller.WaitIfNecessary();
            Controller.status = $"Loading files";

            uint baseAddress = 0x80018000;

            // TODO: Move these to methods to avoid hard-coding
            var fixDTAPath = FixDataPath;
            var fixGRPPath = FixGraphicsPath;
            var sprPLSPath = SpritePalettesPath;
            var levelDTAPath = GetLevelDataPath(context.Settings);
            var levelSPRPath = GetLevelImageDescriptorsPath(context.Settings); // SPRites?
            var levelGRPPath = GetLevelGraphicsPath(context.Settings); // GRaPhics/graphismes

            baseAddress += await LoadFile(context, fixDTAPath, baseAddress);
            baseAddress -= 94; // FIX.DTA header size
            Pointer fixDTAHeader = new Pointer(baseAddress, context.FilePointer(fixDTAPath).file);

            PS1_R2Demo_AllfixFooter footer = null;

            context.Deserializer.DoAt(fixDTAHeader, () => footer = context.Deserializer.SerializeObject<PS1_R2Demo_AllfixFooter>(null, name: "AllfixFooter"));
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
            Controller.status = $"Loading level data";

            // Read the level data
            var lvlData = FileFactory.Read<PS1_R2Demo_LevDataFile>(levelDTAPath, context);

            // Read the map blocks
            var maps = Enumerable.Range(0, MapCount).Select(x => FileFactory.Read<PS1_R1_MapBlock>(GetSubMapPath(x), context)).ToArray();

            await Controller.WaitIfNecessary();
            Controller.status = $"Loading sprite data";

            var globalDESKey = lvlData.FixImageDescriptorsPointer;

            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();
            var commonEvents = new List<Common_EventData>();
            var eventDES = new Dictionary<Pointer, Common_Design>();

            if (loadTextures)
                // Get the v-ram
                FillVRAM(context);

            // Create the global design list
            var globalDesigns = new List<Sprite>();

            // Add every sprite
            foreach (var img in lvlData.FixImageDescriptors.Concat(FileFactory.Read<ObjectArray<Common_ImageDescriptor>>(levelSPRPath, context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 0xC).Value))
            {
                Texture2D tex = GetSpriteTexture(context, null, img);

                // Add it to the array
                globalDesigns.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
            }

            // Get the events
            var events = lvlData.Events.Concat(lvlData.AlwaysEvents).ToArray();

            Controller.status = $"Loading animations";
            await Controller.WaitIfNecessary();

            // Get the ETA and DES
            foreach (var animGroup in events.Select(x => x.AnimGroup).Append(footer.RaymanAnimGroup))
            {
                // Add the ETA
                if (animGroup?.ETAPointer != null && !eventETA.ContainsKey(animGroup.ETAPointer))
                    eventETA.Add(animGroup.ETAPointer, animGroup.ETA.EventStates);

                // Add the DES
                if (animGroup?.AnimationDescriptorsPointer != null && !eventDES.ContainsKey(animGroup.AnimationDescriptorsPointer))
                {
                    // Create the DES
                    var des = new Common_Design()
                    {
                        Sprites = globalDesigns,
                        Animations = new List<Common_Animation>(),
                        FilePath = animGroup.AnimationDescriptorsPointer.file.filePath
                    };

                    // Add animations
                    des.Animations.AddRange(animGroup.AnimationDecriptors.Select(GetCommonR2Animation));

                    // Add the DES
                    eventDES.Add(animGroup.AnimationDescriptorsPointer, des);
                }
            }

            Controller.status = $"Loading events";
            await Controller.WaitIfNecessary();

            var index = 0;

            // Add every event
            foreach (var e in events)
            {
                // Add the event
                commonEvents.Add(new Common_EventData
                {
                    Type = e.EventType,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    DESKey = e.AnimGroup?.AnimationDescriptorsPointer?.ToString() ?? "NULL",
                    ETAKey = e.AnimGroup?.ETAPointer?.ToString() ?? "NULL",
                    OffsetBX = e.CollisionData?.OffsetBX ?? 0,
                    OffsetBY = e.CollisionData?.OffsetBY ?? 0,
                    OffsetHY = e.CollisionData?.OffsetHY ?? 0,
                    MapLayer = e.MapLayer,
                    //FollowSprite = e.FollowSprite,
                    //HitPoints = e.Hitpoints,
                    Layer = e.Layer,
                    //HitSprite = e.HitSprite,
                    //FollowEnabled = e.GetFollowEnabled(context.Settings),
                    FlipHorizontally = e.IsFlippedHorizontally,
                    //LabelOffsets = e.LabelOffsets,
                    //CommandCollection = e.Commands,
                    LinkIndex = lvlData.EventLinkTable.Length > index ? lvlData.EventLinkTable[index] : index,
                    DebugText = $"UShort_00: {e.UShort_00}{Environment.NewLine}" +
                                $"UShort_02: {e.UShort_02}{Environment.NewLine}" +
                                $"UShort_04: {e.UShort_04}{Environment.NewLine}" +
                                $"UShort_06: {e.UShort_06}{Environment.NewLine}" +
                                $"UShort_08: {e.UShort_08}{Environment.NewLine}" +
                                $"UShort_0A: {e.UShort_0A}{Environment.NewLine}" +
                                $"UnkStateRelatedValue: {e.UnkStateRelatedValue}{Environment.NewLine}" +
                                $"Unk_22: {e.Unk_22}{Environment.NewLine}" +
                                $"MapLayer: {e.MapLayer}{Environment.NewLine}" +
                                $"Unk1: {e.Unk1}{Environment.NewLine}" +
                                $"Unk2: {String.Join("-", e.Unk2)}{Environment.NewLine}" +
                                $"RuntimeUnk1: {e.RuntimeUnk1}{Environment.NewLine}" +
                                $"EventType: {e.EventType}{Environment.NewLine}" +
                                $"RuntimeOffset1: {e.RuntimeOffset1}{Environment.NewLine}" +
                                $"RuntimeOffset2: {e.RuntimeOffset2}{Environment.NewLine}" +
                                $"RuntimeBytes1: {String.Join("-", e.RuntimeBytes1)}{Environment.NewLine}" +
                                $"Unk_58: {e.Unk_58}{Environment.NewLine}" +
                                $"Unk3: {String.Join("-", e.Unk3)}{Environment.NewLine}" +
                                $"Unk4: {String.Join("-", e.Unk4)}{Environment.NewLine}" +
                                $"Flags: {String.Join(", ", e.Flags.GetFlags())}{Environment.NewLine}" +
                                $"Unk5: {String.Join("-", e.Unk5)}{Environment.NewLine}" +
                                $"CollisionDataValues 1: {String.Join("-", e.CollisionData?.Unk1 ?? new byte[0])}{Environment.NewLine}" +
                                $"CollisionDataValues 2: {String.Join("-", e.CollisionData?.Unk2 ?? new byte[0])}{Environment.NewLine}"
                });

                index++;
            }

            await Controller.WaitIfNecessary();
            Controller.status = $"Loading tiles";

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev
            {
                // Create the maps
                Maps = maps.Select((x, i) => new Common_LevelMap()
                {
                    // Set the dimensions
                    Width = x.Width,
                    Height = x.Height,

                    // TODO: Correct this - scale backgrounds too
                    ScaleFactor = i == 1 ? 0.5f : 1,

                    // Create the tile array
                    TileSet = new Common_Tileset[1]
                }).ToArray(),

                // Create the events list
                EventData = new List<Common_EventData>(),

                DefaultMap = context.Settings.Level
            };

            // Add the events
            c.EventData = commonEvents;

            await Controller.WaitIfNecessary();

            // Load maps
            for (int i = 0; i < MapCount; i++)
            {
                // Get the tile set
                Common_Tileset tileSet = GetTileSet(context, i);

                c.Maps[i].TileSet[0] = tileSet;

                // Set the tiles
                c.Maps[i].Tiles = new Common_Tile[maps[i].Width * maps[i].Height];

                int tileIndex = 0;
                for (int y = 0; y < maps[i].Height; y++)
                {
                    for (int x = 0; x < maps[i].Width; x++)
                    {
                        var graphicX = maps[i].Tiles[tileIndex].TileMapX;
                        var graphicY = maps[i].Tiles[tileIndex].TileMapY;

                        Common_Tile newTile = new Common_Tile
                        {
                            PaletteIndex = 1,
                            XPosition = x,
                            YPosition = y,
                            CollisionType = maps[i].Tiles[tileIndex].CollisionType,
                            TileSetGraphicIndex = (TileSetWidth * graphicY) + graphicX
                        };

                        c.Maps[i].Tiles[tileIndex] = newTile;

                        tileIndex++;
                    }
                }
            }

            // Return an editor manager
            return new PS1EditorManager(c, context, eventDES, eventETA);
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
        protected override string GetExportDirName(GameSettings settings, Common_Design des)
        {
            // Since all paths are in allfix, we return an empty path
            return String.Empty;
        }
    }
}