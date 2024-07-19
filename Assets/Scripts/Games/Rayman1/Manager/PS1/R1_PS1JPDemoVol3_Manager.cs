using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ray1.PS1;
using UnityEngine;
using Animation = BinarySerializer.Ray1.Animation;
using Context = BinarySerializer.Context;
using Sprite = BinarySerializer.Ray1.Sprite;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class R1_PS1JPDemoVol3_Manager : R1_PS1BaseManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 40;

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath() => $"RAY.FXS";

        public string GetPalettePath(GameSettings settings, int i) => $"RAY{i}_{(settings.R1_World == World.Jungle ? 1 : 2)}.PAL";

        /// <summary>
        /// Gets the file path for the world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => $"RAY.WL{(settings.R1_World == World.Jungle ? 1 : 2)}";

        /// <summary>
        /// Gets the file path for the level file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => $"RAY.LV{(settings.R1_World == World.Jungle ? 1 : 2)}";
        
        /// <summary>
        /// Gets the file path for the level tile set file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level tile set file path</returns>
        public virtual string GetTileSetFilePath(GameSettings settings) => $"_{GetWorldName(settings.R1_World)}_01.R16";

        /// <summary>
        /// Gets the file path for the level map file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level map file path</returns>
        public virtual string GetMapFilePath(GameSettings settings) => $"_{GetWorldName(settings.R1_World)}_{settings.Level:00}.MAP";

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public override string GetWorldName(World world) => base.GetWorldName(world).Substring(1);

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[]
        {
            new GameInfo_Volume(null, new GameInfo_World[]
            {
                new GameInfo_World(1, "Jungle", new[] { 1, 2, 3, 4, 5, 6 }),
                new GameInfo_World(3, "Mountain", new[] { 1 }),
            })
        };

        public override string ExeFilePath => "RAY.EXE";
        public override uint? ExeBaseAddress => 0x80180000 - 0x800;
        protected override ExecutableConfig GetExecutableConfig => ExecutableConfig.PS1_JPDemoVol3;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_TileSet GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetTileSetFilePath(context.GetR1Settings());

            // Read the file
            var tileSet = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, filename, (s, x) => x.Pre_Length = s.CurrentLength / 2);

            // Return the tile set
            return new Unity_TileSet(tileSet.Value, TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, PS1VramHelpers.VRAMMode mode)
        {
            // We don't need to emulate the v-ram for this version
            return;// null;
        }

        /// <summary>
        /// Gets the sprite texture for an event
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="imgBuffer">The image buffer, if available</param>
        /// <param name="s">The image descriptor to use</param>
        /// <returns>The texture</returns>
        public override Texture2D GetSpriteTexture(Context context, byte[] imgBuffer, Sprite s, int palOffset = 0)
        {
            // Ignore dummy sprites
            if (s.IsDummySprite())
                return null;

            // Get the image properties
            var width = s.Width;
            var height = s.Height;
            var offset = s.ImageBufferOffset;

            var pal4 = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, GetPalettePath(context.GetR1Settings(), 4), (y, x) => x.Pre_Length = 256);
            var pal8 = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, GetPalettePath(context.GetR1Settings(), 8), (y, x) => x.Pre_Length = 256);

            // Select correct palette
            var palette = s.Depth == SpriteDepth.BPP_8 ? pal8.Value : pal4.Value;
            var paletteOffset = 16 * (s.SubPaletteIndex + palOffset);

            // Create the texture
            Texture2D tex = TextureHelpers.CreateTexture2D(width, height);

            // Set every pixel
            if (s.Depth == SpriteDepth.BPP_8)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var paletteIndex = imgBuffer[offset + width * y + x];

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteIndex].GetColor());
                    }
                }
            }
            else if (s.Depth == SpriteDepth.BPP_4)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int actualX = (s.ImageOffsetInPageX + x) / 2;
                        var paletteIndex = imgBuffer[offset + (width * y + x) / 2];
                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);

                        // Set the pixel
                        tex.SetPixel(x, height - 1 - y, palette[paletteOffset + paletteIndex].GetColor());
                    }
                }
            }

            // Apply the changes
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get the file paths
            var levelPath = GetLevelFilePath(context.GetR1Settings());
            var mapPath = GetMapFilePath(context.GetR1Settings());

            // Do this to include unused world graphics data
            if (context.GetR1Settings().R1_World == World.Mountain)
                ReadAllWorldData(context);

            // Read the level
            // The game hard-codes these addresses in a table, so we should ideally read that. But this works for now.
            int lvlOffset = 0x8000 * (context.GetR1Settings().Level - 1);
            var lvl = FileFactory.Read<LevelData>(context, context.FilePointer(levelPath) + lvlOffset);

            // Read the map
            MapData map;

            if (context.FileExists(mapPath))
            {
                map = FileFactory.Read<MapData>(context, mapPath);
            }
            else
            {
                ObjData maxObjX = lvl.Objects.OrderBy(x => x.XPosition + x.OffsetBX).Last();
                ObjData maxObjY = lvl.Objects.OrderBy(x => x.YPosition + x.OffsetBY).Last();
                int width = (maxObjX.XPosition + maxObjX.OffsetBX) / 16 + 4;
                int height = (maxObjY.YPosition + maxObjY.OffsetBY) / 16 + 2;

                map = new MapData(width, height);
            }

            // Load the level
            return await LoadAsync(context, map, lvl.Objects, lvl.ObjectLinkingTable.Select(x => (ushort)x).ToArray());
        }

        private void ReadAllWorldData(Context context)
        {
            // Hacky, but this also gets unreferenced data for which there is some in Mountain
            //
            // Data is always stored like:
            //  Animation[]
            //  ImgBuffer
            //  Sprite[]
            //  
            //  Repeat:
            //  AnimationLayer[]
            //  AnimationFrame[]
            //

            string worldPath = GetWorldFilePath(context.GetR1Settings());
            BinaryFile worldFile = context.GetRequiredFile(worldPath);
            BinaryDeserializer s = context.Deserializer;
            s.DoAt(worldFile.StartPointer, () =>
            {
                List<DES> des = new();
                int i = 0;

                while (s.CurrentFileOffset < worldFile.Length)
                {
                    int animsCount = 0;

                    while (s.SerializePointer(default, allowInvalid: true, name: "FirstAnimValue") != null)
                    {
                        animsCount++;
                        s.Goto(s.CurrentPointer + (12 - 4));
                    }

                    s.Goto(s.CurrentPointer - (12 * animsCount) - 4);

                    Animation[] animations = s.SerializeObjectArray<Animation>(default, animsCount, name: "Animations");

                    Pointer imgBufferPointer = s.CurrentPointer;

                    long spritesAndImgBufferLength = animations[0].LayersPointer.FileOffset - 4 - s.CurrentPointer.FileOffset;
                    int currentSpritesLength = 0;
                    int maxImgBufferLength = 0;
                    int spritesCount = 0;

                    s.Goto(animations[0].LayersPointer - 4 - 24);

                    Pointer spritesPointer = null;

                    // Read sprites in reverse
                    while (maxImgBufferLength + currentSpritesLength < spritesAndImgBufferLength)
                    {
                        currentSpritesLength += 24;
                        spritesCount++;

                        spritesPointer = s.CurrentPointer;
                        Sprite sprite = s.SerializeObject<Sprite>(default, name: "Sprite");

                        int imgBufferLength = sprite.ImageBufferOffset;

                        if (sprite.Depth == SpriteDepth.BPP_8)
                            imgBufferLength += sprite.Width * sprite.Height;
                        else
                            imgBufferLength += sprite.Width * sprite.Height / 2;

                        if (imgBufferLength > maxImgBufferLength)
                            maxImgBufferLength = imgBufferLength;

                        s.Log($"{maxImgBufferLength + currentSpritesLength} < {spritesAndImgBufferLength}");
                        s.Goto(s.CurrentPointer - 24 * 2);
                    }

                    des.Add(new DES
                    {
                        ImageDescriptorsPointer = spritesPointer,
                        AnimationDescriptorsPointer = animations.First().Offset,
                        ImageBufferPointer = imgBufferPointer,
                        ImageDescriptorCount = (ushort)spritesCount,
                        AnimationDescriptorCount = (byte)animations.Length,
                        ImageBufferLength = (uint?)maxImgBufferLength,
                        Name = $"Unused {i}",
                        EventData = null
                    });

                    s.Goto(animations.Last().Frames.Last().Offset + 4);
                    i++;
                }

                context.StoreObject("DES", des);
            });
        }

        protected override IEnumerable<DES> GetLevelDES(Context context, IEnumerable<ObjData> events)
        {
            // Hacky way to add graphics
            return base.GetLevelDES(context, events).Concat((IEnumerable<DES>)context.GetStoredObject<List<DES>>("DES") ?? Array.Empty<DES>());
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            await base.LoadFilesAsync(context);

            // Get the file paths
            var allfixPath = GetAllfixFilePath();
            var worldPath = GetWorldFilePath(context.GetR1Settings());
            var levelPath = GetLevelFilePath(context.GetR1Settings());
            var mapPath = GetMapFilePath(context.GetR1Settings());
            var tileSetPath = GetTileSetFilePath(context.GetR1Settings());
            var pal4Path = GetPalettePath(context.GetR1Settings(), 4);
            var pal8Path = GetPalettePath(context.GetR1Settings(), 8);

            // Load the files
            await LoadExtraFile(context, allfixPath, false);
            await LoadExtraFile(context, worldPath, false);
            await LoadExtraFile(context, levelPath, true);
            await LoadExtraFile(context, mapPath, true);
            await LoadExtraFile(context, tileSetPath, true);
            await LoadExtraFile(context, pal4Path, true);
            await LoadExtraFile(context, pal8Path, true);
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("JUN_F02.R16"), 
            new PS1VignetteFileInfo("MON_F2W.R16"), 
            new PS1VignetteFileInfo("VIG_0P.R16", 260), 
            new PS1VignetteFileInfo("VIG_1P.R16", 320), 
            new PS1VignetteFileInfo("VIG_02P.R16", 257), 
            new PS1VignetteFileInfo("VIG_7P.R16", 320), 
            new PS1VignetteFileInfo("WORLD.R16", 320), 
        };

        /// <summary>
        /// Gets the base directory name for exporting a common design
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="des">The design to export</param>
        /// <returns>The base directory name</returns>
        protected override string GetExportDirName(GameSettings settings, Unity_ObjGraphics des)
        {
            // Get the file path
            var path = des.FilePath;

            if (path == null)
                throw new Exception("Path can not be null");

            if (path == GetAllfixFilePath())
                return $"Allfix/";
            else if (path == GetWorldFilePath(settings))
                return $"{(World)settings.World}/{(World)settings.World} - ";
            else if (path == GetLevelFilePath(settings))
                return $"{(World)settings.World}/{(World)settings.World}{settings.Level} - ";

            return $"Unknown/";
        }

        public override async UniTask ExportMenuSpritesAsync(GameSettings settings, string outputPath, bool exportAnimFrames)
        {
            using (var context = new Ray1MapContext(settings))
            {
                // Load files
                await LoadFilesAsync(context);

                // Read level file
                var level = FileFactory.Read<LevelData>(context, GetLevelFilePath(context.GetR1Settings()));

                // Export
                await ExportMenuSpritesAsync(context, null, outputPath, exportAnimFrames, new Alpha[]
                {
                    level.Alpha
                }, new ObjData[]
                {
                    level.Ray
                }, null);
            }
        }

        public override Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData> GetEventTemplates(Context context)
        {
            var level = FileFactory.Read<LevelData>(context, GetLevelFilePath(context.GetR1Settings()));

            return new Dictionary<Unity_ObjectManager_R1.WldObjType, ObjData>()
            {
                [Unity_ObjectManager_R1.WldObjType.Ray] = level.Ray,
            };
        }

        public override async UniTask<Texture2D> LoadLevelBackgroundAsync(Context context)
        {
            var exe = LoadEXE(context);

            var bgIndex = context.GetR1Settings().R1_World == World.Jungle ? 0 : 2;
            var fndStartIndex = exe.GetFileTypeIndex(GetExecutableConfig, FileType.fnd_file);

            if (fndStartIndex == -1)
                return null;

            var bgFilePath = exe.PS1_FileTable[fndStartIndex + bgIndex].ProcessedFilePath;

            await LoadExtraFile(context, bgFilePath, true);

            var bg = FileFactory.Read<Fond>(context, bgFilePath);

            return bg.ToTexture(context);
        }

        /*

        World info table: (x, y, up, down, left, right)
        36 00 B5 00 02 00 01 00 01 00 02 00 // Jungle
        5E 00 7E 00 02 00 01 00 07 00 03 00 // Music
        A3 00 5D 00 03 00 03 00 02 00 05 00 // Mountain
        1C 01 A2 00 06 00 04 00 05 00 06 00 // Image
        E6 00 73 00 03 00 04 00 03 00 04 00 // Cave
        2D 01 55 00 06 00 04 00 04 00 06 00 // Cake
        23 00 5F 00 07 00 02 00 07 00 02 00 // Present (Breakout?)

        */
    }
}