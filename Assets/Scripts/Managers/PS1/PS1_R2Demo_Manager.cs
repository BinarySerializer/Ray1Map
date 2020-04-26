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
        public string GetSubMapPalettePath(GameSettings s) => $"JUNGLE/{GetMapName(s.Level)}.PAL";
        public string GetSubMapPath(GameSettings s) => $"JUNGLE/{GetMapName(s.Level)}.MPU";


        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoR2PS1;

        // TODO: Is this needed?
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
            var palettePath = GetSubMapPalettePath(context.Settings);
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
            var palettePath = GetSubMapPalettePath(context.Settings);
            var mapPath = GetSubMapPath(context.Settings);


            baseAddress += await LoadFile(context, fixDTAPath, baseAddress);
            baseAddress -= 0x5E; // FIX.DTA header size
            Pointer fixDTAHeader = new Pointer(baseAddress, context.FilePointer(fixDTAPath).file);
            context.Deserializer.DoAt(fixDTAHeader, () => {
                // TODO: Read header here (0x5E bytes). Should be done now because these bytes will be overwritten

            });
            await LoadFile(context, fixGRPPath, 0);
            await LoadFile(context, sprPLSPath, 0);
            baseAddress += await LoadFile(context, levelSPRPath, baseAddress);
            baseAddress += await LoadFile(context, levelDTAPath, baseAddress);
            await LoadFile(context, levelGRPPath, 0);
            await LoadFile(context, tileSetPath, 0);
            await LoadFile(context, palettePath, 0);
            await LoadFile(context, mapPath, 0); // TODO: Load all maps for this level

            // Read the level data
            var lvlData = FileFactory.Read<PS1_R2Demo_LevDataFile>(levelDTAPath, context);

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);

            // Test export
            /*if (loadTextures) {
                // Get the v-ram
                FillVRAM(context);
                for (int i = 0; i < lvlData.FixImageDescriptors.Length; i++) {
                    Texture2D t = GetSpriteTexture(context, null, lvlData.FixImageDescriptors[i]);
                    Util.ByteArrayToFile(context.Settings.GameDirectory + "textures/fix_" + i + ".png", t.EncodeToPNG());
                }
                ObjectArray<Common_ImageDescriptor> test_img = FileFactory.Read<ObjectArray<Common_ImageDescriptor>>(levelSPRPath, context, onPreSerialize: (s, a) => a.Length = s.CurrentLength / 0xC);
                for (int i = 0; i < test_img.Length; i++) {
                    Texture2D t = GetSpriteTexture(context, null, test_img.Value[i]);
                    Util.ByteArrayToFile(context.Settings.GameDirectory + "textures/level_" + i + ".png", t.EncodeToPNG());
                }

            }*/

            var globalDESKey = lvlData.FixImageDescriptorsPointer;

            // Get the tile set
            Common_Tileset tileSet = GetTileSet(context);

            var animGroups = new List<PS1_R2Demo_EventAnimGroup>();
            var commonEvents = new List<Common_EventData>();

            if (loadTextures)
                // Get the v-ram
                FillVRAM(context);

            var animBaseIndex = 0;

            var index = 0;

            var events = lvlData.Events.ToArray();

            // Add every event
            foreach (var e in events)
            {
                Controller.status = $"Loading event {index}/{events.Length}";

                await Controller.WaitIfNecessary();

                // Add if not found
                if (!animGroups.Contains(e.AnimGroup) && e.AnimGroup != null)
                {
                    // Add to the list
                    animGroups.Add(e.AnimGroup);

                    if (e.AnimGroup?.EventStates != null)
                    {
                        // TODO: Clean up this hack
                        foreach (var ee in e.AnimGroup.EventStates.SelectMany(x => x))
                            ee.AnimationIndex = (byte)(animBaseIndex + ee.AnimationIndex);
                    }

                    animBaseIndex += (int)e.AnimGroup?.AnimationDescriptorCount;
                }

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
                    LinkIndex = lvlData.EventLinkTable[index],
                    DebugText = $"Unk1: {String.Join("-", e.Unk1)}{Environment.NewLine}" +
                                $"Pos: ({e.XPosition},{e.YPosition}){Environment.NewLine}" +
                                $"EventType: {String.Join("-", e.EventType)}{Environment.NewLine}" +
                                $"Etat: {e.Etat}{Environment.NewLine}" +
                                $"SubEtat: {e.SubEtat}{Environment.NewLine}" +
                                $"UnkStateRelatedValue: {e.UnkStateRelatedValue}{Environment.NewLine}" +
                                $"Unk3: {String.Join("-", e.Unk3)}{Environment.NewLine}" +
                                $"Unk4: {String.Join("-", e.Unk4)}{Environment.NewLine}" +
                                $"Unk5: {String.Join("-", e.Unk5)}{Environment.NewLine}"
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

            // Add every animations
            foreach (var a in animGroups.SelectMany(x => x?.AnimationDecriptors ?? new PS1_R2Demo_AnimationDecriptor[0]))
            {
                // Create the animation
                var animation = new Common_Animation
                {
                    Frames = new Common_AnimationPart[a.FrameCount, a.LayersPerFrame],

                    // TODO: Set these values correctly
                    DefaultFrameXPosition = 20,
                    DefaultFrameYPosition = 20,
                    DefaultFrameWidth = 20,
                    DefaultFrameHeight = 20,
                };

                // The layer index
                var layer = 0;

                // Create each frame
                for (int i = 0; i < a.FrameCount; i++)
                {
                    // Create each layer
                    for (var layerIndex = 0; layerIndex < a.LayersPerFrame; layerIndex++)
                    {
                        var animationLayer = a.Layers[layer];
                        layer++;

                        // Create the animation part
                        var part = new Common_AnimationPart
                        {
                            SpriteIndex = animationLayer.ImageIndex,
                            X = animationLayer.XPosition,
                            Y = animationLayer.YPosition,
                        };

                        // Add the texture
                        animation.Frames[i, layerIndex] = part;
                    }
                }

                // Add the animation to list
                globalDesign.Animations.Add(animation);
            }

            await Controller.WaitIfNecessary();

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev
            {
                // Set the dimensions
                Width = map.Width,
                Height = map.Height,

                // Create the events list
                EventData = new List<Common_EventData>(),

                // Create the tile array
                TileSet = new Common_Tileset[4]
            };
            c.TileSet[0] = tileSet;

            // Add the events
            c.EventData = commonEvents;

            await Controller.WaitIfNecessary();

            // Set the tiles
            c.Tiles = new Common_Tile[map.Width * map.Height];

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

                    c.Tiles[tileIndex] = newTile;

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
            }, animGroups.Where(x => x != null).ToDictionary(x => x.ETAPointer, x => x.EventStates));
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