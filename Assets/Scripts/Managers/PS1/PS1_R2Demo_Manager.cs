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

        public string FixDataPath => $"RAY.DTA";
        public string FixGraphicsPath => "RAY.GRP";
        public string SpritePalettesPath => "SPR.PLS";
        public string GetLevelGraphicsPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.World)}01.GRP";
        public string GetLevelImageDescriptorsPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.World)}01.SPR";
        public string GetLevelDataPath(GameSettings s) => $"JUNGLE/{GetWorldName(s.World)}01.DTA";

        public string GetSubMapTilesetPath(GameSettings s) => $"JUNGLE/{GetMapName(s.Level)}.RAW";
        public string GetSubMapPalettePath(GameSettings s, int level) => $"JUNGLE/{GetMapName(level)}.PAL";
        public string GetSubMapPath(GameSettings s) => $"JUNGLE/{GetMapName(s.Level)}.MPU";


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
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Enumerable.Range(1, w == World.Jungle ? 4 : 0).ToArray())).ToArray();

        /// <summary>
        /// Gets the name for the specified map
        /// </summary>
        /// <param name="map">The map</param>
        /// <returns>The map name</returns>
        public virtual string GetMapName(int map)
        {
            switch (map)
            {
                case 1:
                    return "PL1";

                case 2:
                    return "PL2";

                case 3:
                    return "FD1";

                case 4:
                    return "FD2";

                default:
                    throw new ArgumentOutOfRangeException(nameof(map));
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Common_Tileset GetTileSet(Context context) {
            var tileSetPath = GetSubMapTilesetPath(context.Settings);
            var palettePath = GetSubMapPalettePath(context.Settings, context.Settings.Level);
            var tileSet = FileFactory.Read<Array<byte>>(tileSetPath, context, (s, x) => x.Length = s.CurrentLength);
            var palette = FileFactory.Read<ObjectArray<ARGB1555Color>>(palettePath, context, (s, x) => x.Length = s.CurrentLength / 2);

            return new Common_Tileset(tileSet.Value.Select(ind => palette.Value[ind]).ToArray(), TileSetWidth, CellSize);
        }

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
            for (int i = 0; i < 4; i++) {
                tilePalettes[i] = FileFactory.Read<ObjectArray<ARGB1555Color>>(GetSubMapPalettePath(context.Settings, i+1), context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 2);
            }
            
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
            // TODO: Load submaps based on levelDTA file
            var tileSetPath = GetSubMapTilesetPath(context.Settings);
            //var palettePath = GetSubMapPalettePath(context.Settings);
            var mapPath = GetSubMapPath(context.Settings);

            for (int i = 0; i < 4; i++)
                await LoadFile(context, GetSubMapPalettePath(context.Settings, i+1), 0);

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
            await LoadFile(context, tileSetPath, 0);
            await LoadFile(context, mapPath, 0); // TODO: Load all maps for this level

            await Controller.WaitIfNecessary();
            Controller.status = $"Loading level data";

            // Read the level data
            var lvlData = FileFactory.Read<PS1_R2Demo_LevDataFile>(levelDTAPath, context);

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);

            await Controller.WaitIfNecessary();
            Controller.status = $"Loading sprite data";

            var globalDESKey = lvlData.FixImageDescriptorsPointer;

            // Get the tile set
            Common_Tileset tileSet = GetTileSet(context);

            var eventETA = new Dictionary<Pointer, Common_EventState[][]>();
            var commonEvents = new List<Common_EventData>();

            if (loadTextures)
                // Get the v-ram
                FillVRAM(context);

            // Get the events
            var events = lvlData.Events.Concat(lvlData.AlwaysEvents).ToArray();

            // Get the animations
            var anim = events.Where(x => x.AnimGroup?.AnimationDecriptors != null).
                SelectMany(x => x.AnimGroup.AnimationDecriptors).
                // Add Rayman's animations
                Concat(footer.RaymanAnimGroup.AnimationDecriptors).
                Where(x => x != null).
                Distinct().
                ToArray();

            // Get the ETA
            foreach (var animGroup in events.Select(x => x.AnimGroup).Append(footer.RaymanAnimGroup))
            {
                if (animGroup?.ETAPointer == null || eventETA.ContainsKey(animGroup.ETAPointer))
                    continue;

                // Add the ETA
                eventETA.Add(animGroup.ETAPointer, animGroup.EventStates);

                // TODO: Find a better way of doing this - we don't want to edit the state like this, especially since the anim index is a byte - might be better creating one DES for each animation group
                // Change animation index to target global array
                if (animGroup.AnimationDecriptors != null)
                {
                    foreach (var state in animGroup.EventStates.SelectMany(x => x))
                    {
                        var a = animGroup.AnimationDecriptors.ElementAtOrDefault(state.AnimationIndex);

                        if (a == null)
                        {
                            Debug.LogWarning($"Animation descriptor not found of index {state.AnimationIndex} with length {animGroup.AnimationDecriptors.Length}");

                            // For now we default to 255 as that's an invalid index
                            state.AnimationIndex = 255;
                        }
                        else
                        {
                            state.AnimationIndex = (byte)(anim.FindItemIndex(x => x == a));
                        }
                    }
                }
            }

            Controller.status = $"Loading events";

            Debug.Log($"Loading {anim.Length} animations");

            await Controller.WaitIfNecessary();

            var index = 0;

            // Add every event
            foreach (var e in events)
            {
                // Add the event
                commonEvents.Add(new Common_EventData
                {
                    //Type = e.Type,
                    Etat = e.Etat,
                    SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    DESKey = globalDESKey.ToString(),
                    ETAKey = e.AnimGroup?.ETAPointer?.ToString() ?? "NULL",
                    //OffsetBX = e.OffsetBX,
                    //OffsetBY = e.OffsetBY,
                    //OffsetHY = e.OffsetHY,
                    //FollowSprite = e.FollowSprite,
                    //HitPoints = e.Hitpoints,
                    Layer = e.Layer,
                    //HitSprite = e.HitSprite,
                    //FollowEnabled = e.GetFollowEnabled(context.Settings),
                    //LabelOffsets = e.LabelOffsets,
                    //CommandCollection = e.Commands,
                    LinkIndex = lvlData.EventLinkTable.Length > index ? lvlData.EventLinkTable[index] : index,
                    DebugText = $"Unk1: {String.Join("-", e.Unk1)}{Environment.NewLine}" +
                                $"Pos: ({e.XPosition},{e.YPosition}){Environment.NewLine}" +
                                $"EventType: {String.Join("-", e.EventType)}{Environment.NewLine}" +
                                $"Etat: {e.Etat}{Environment.NewLine}" +
                                $"SubEtat: {e.SubEtat}{Environment.NewLine}" +
                                $"UnkStateRelatedValue: {e.UnkStateRelatedValue}{Environment.NewLine}" +
                                $"Unk3: {String.Join("-", e.Unk3)}{Environment.NewLine}" +
                                $"Unk4: {String.Join("-", e.Unk4)}{Environment.NewLine}" +
                                $"Unk5: {String.Join("-", e.Unk5)}{Environment.NewLine}" +
                                $"Animations: {e.AnimGroup?.AnimationDescriptorCount ?? -1}{Environment.NewLine}"
                });

                index++;
            }

            // Create the global design
            Common_Design globalDesign = new Common_Design
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Common_Animation>()
            };

            // Add every sprite
            foreach (var img in lvlData.FixImageDescriptors.Concat(FileFactory.Read<ObjectArray<Common_ImageDescriptor>>(levelSPRPath, context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 0xC).Value))
            {
                Texture2D tex = GetSpriteTexture(context, null, img);

                // Add it to the array
                globalDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
            }

            await Controller.WaitIfNecessary();
            Controller.status = $"Loading animations";

            // Add every animations
            foreach (var a in anim)
            {
                // Create the animation
                var animation = new Common_Animation
                {
                    Frames = new Common_AnimationPart[a.FrameCount, a.LayersPerFrame],
                    DefaultFrameXPosition = a.Frames.FirstOrDefault()?.XPosition ?? -1,
                    DefaultFrameYPosition = a.Frames.FirstOrDefault()?.YPosition ?? -1,
                    DefaultFrameWidth = a.Frames.FirstOrDefault()?.Width ?? -1,
                    DefaultFrameHeight = a.Frames.FirstOrDefault()?.Height ?? -1,
                };

                // Create each frame
                for (int i = 0; i < a.Layers.Length; i++)
                {
                    // Get the layers for the frame
                    var layers = a.Layers[i];

                    // Create each layer
                    for (var j = 0; j < layers.Length; j++)
                    {
                        var animationLayer = layers[j];

                        // Create the animation part
                        var part = new Common_AnimationPart
                        {
                            SpriteIndex = animationLayer.ImageIndex,
                            X = animationLayer.XPosition,
                            Y = animationLayer.YPosition,
                            Flipped = animationLayer.IsFlippedHorizontally
                        };

                        // Add the texture
                        animation.Frames[i, j] = part;
                    }
                }

                // Add the animation to list
                globalDesign.Animations.Add(animation);
            }

            await Controller.WaitIfNecessary();
            Controller.status = $"Loading tiles";

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev
            {
                // Create the maps
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile array
                        TileSet = new Common_Tileset[1]
                    }
                },

                // Create the events list
                EventData = new List<Common_EventData>(),

            };
            c.Maps[0].TileSet[0] = tileSet;

            // Add the events
            c.EventData = commonEvents;

            await Controller.WaitIfNecessary();

            // Set the tiles
            c.Maps[0].Tiles = new Common_Tile[map.Width * map.Height];

            int tileIndex = 0;
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var graphicX = map.Tiles[tileIndex].TileMapX;
                    var graphicY = map.Tiles[tileIndex].TileMapY;

                    Common_Tile newTile = new Common_Tile
                    {
                        PaletteIndex = 1,
                        XPosition = x,
                        YPosition = y,
                        CollisionType = map.Tiles[tileIndex].CollisionType,
                        TileSetGraphicIndex = (TileSetWidth * graphicY) + graphicX
                    };

                    c.Maps[0].Tiles[tileIndex] = newTile;

                    tileIndex++;
                }
            }

            // Return an editor manager
            return new PS1EditorManager(c, context, new Dictionary<Pointer, Common_Design>()
            {
                {
                    globalDESKey,
                    globalDesign
                }
            }, eventETA);
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
    }
}