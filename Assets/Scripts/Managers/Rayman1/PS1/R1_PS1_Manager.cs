using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_Manager : R1_PS1BaseXXXManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        public virtual string GetLanguageFilePath(string langCode) => GetDataPath() + $"IMA/CRD/RAY{langCode}.TXT";

        /// <summary>
        /// Gets the file info to use
        /// </summary>
        /// <param name="settings">The game settings</param>
        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => settings.GameModeSelection == GameModeSelection.RaymanPS1US ? PS1FileInfo.fileInfoUS : PS1FileInfo.fileInfoPAL;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_MapTileMap GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldFile = FileFactory.Read<R1_PS1_WorldFile>(filename, context);

            int tileCount = worldFile.TilePaletteIndexTable.Length;
            int width = TileSetWidth * Settings.CellSize;
            int height = (worldFile.PalettedTiles.Length) / width;

            var pixels = new ARGB1555Color[width * height];

            int tile = 0;

            for (int yB = 0; yB < height; yB += Settings.CellSize)
            for (int xB = 0; xB < width; xB += Settings.CellSize, tile++)
            for (int y = 0; y < Settings.CellSize; y++)
            for (int x = 0; x < Settings.CellSize; x++)
            {
                int pixel = x + xB + (y + yB) * width;
                
                if (tile >= tileCount)
                {
                    // Set dummy data
                    pixels[pixel] = new ARGB1555Color();
                }
                else
                {
                    byte tileIndex1 = worldFile.TilePaletteIndexTable[tile];
                    byte tileIndex2 = worldFile.PalettedTiles[pixel];
                    pixels[pixel] = worldFile.TilePalettes[tileIndex1][tileIndex2];
                }
            }

            return new Unity_MapTileMap(pixels, TileSetWidth, Settings.CellSize);
        }

        // TODO: Fix & support for JP version
        public string GetLevelBackgroundFilePath(GameSettings settings, bool returnNullIfNoParallax)
        {
            return null;

            // Commented because of unreachable code warnings, uncomment when we fix this
            /*var index = -1;

            // TODO: Add bonus levels
            if (settings.World == World.Jungle)
            {
                switch (settings.Level)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 10:
                    case 11:
                    case 12:
                    case 14:
                    case 15:
                    case 17:
                        index = returnNullIfNoParallax ? -1 : 1;
                        break;

                    case 1:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 13:
                        index = 2;
                        break;

                    case 9:
                        index = returnNullIfNoParallax ? -1 : 3;
                        break;

                    case 16:
                        index = returnNullIfNoParallax ? -1 : 4;
                        break;
                }
            }
            else if (settings.World == World.Music)
            {
                switch (settings.Level)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                        index = 2;
                        break;

                    case 4:
                        index = returnNullIfNoParallax ? -1 : 3;
                        break;
                }
            }

            return index == -1 ? null : $"RAY/IMA/FND/{GetWorldName(settings.World)}F{index}.XXX";*/
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, VRAMMode mode)
        {
            // TODO: Support BigRay + font for US version

            // Read the files
            var allFix = mode != VRAMMode.BigRay ? FileFactory.Read<R1_PS1_AllfixFile>(GetAllfixFilePath(context.Settings), context) : null;
            var world = mode == VRAMMode.Level ? FileFactory.Read<R1_PS1_WorldFile>(GetWorldFilePath(context.Settings), context) : null;
            var levelTextureBlock = mode == VRAMMode.Level ? FileFactory.Read<R1_PS1_LevFile>(GetLevelFilePath(context.Settings), context).TextureBlock : null;
            var bigRay = mode == VRAMMode.BigRay ? FileFactory.Read<R1_PS1_BigRayFile>(GetBigRayFilePath(context.Settings), context) : null;
            var font = mode == VRAMMode.Menu ? FileFactory.Read<Array<byte>>(GetFontFilePath(context.Settings), context, (s, o) => o.Length = s.CurrentLength) : null;

            //var bgPath = GetLevelBackgroundFilePath(context.Settings, true);
            //ARGB1555Color[][] bgPalette = new ARGB1555Color[0][];
            //if (bgPath != null)
            //    bgPalette = FileFactory.Read<PS1_R1_BackgroundVignetteFile>(bgPath, context).ParallaxPalettes;

            PS1_VRAM vram = new PS1_VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.currentXPage = 5;

            if (mode != VRAMMode.BigRay) {
                // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
                // storing the first bit of sprites we load as 1x2
                byte[] cageSprites = new byte[128 * (256 * 2 - 8)];
                Array.Copy(allFix.TextureBlock, 0, cageSprites, 0, cageSprites.Length);
                byte[] allFixSprites = new byte[allFix.TextureBlock.Length - cageSprites.Length];
                Array.Copy(allFix.TextureBlock, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
                byte[] unknown = new byte[128 * 8];
                vram.AddData(unknown, 128);
                vram.AddData(cageSprites, 128);
                vram.AddData(allFixSprites, 256);
            }

            if (mode == VRAMMode.Level) {
                vram.AddData(world.TextureBlock, 256);
                vram.AddData(levelTextureBlock, 256);
            } 
            else if (mode == VRAMMode.Menu) 
            {
                if (context.Settings.GameModeSelection == GameModeSelection.RaymanPS1US)
                    vram.AddDataAt(10, 1, 0, 80, font.Value, 256);
                else 
                    vram.AddDataAt(10, 0, 0, 226, font.Value, 256);
            } else if (mode == VRAMMode.BigRay) {
                vram.AddDataAt(10, 0, 0, 0, bigRay.TextureBlock, 256);
            }

            // Palettes start at y = 256 + 234 (= 490), so page 1 and y=234
            int paletteY = 234;
            if (mode != VRAMMode.BigRay) {
                /*vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
                if (mode == VRAMMode.Level) {
                    vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                    vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                } else {
                    vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                    vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                }
                vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

                // TODO: How are these aligned? Seems different for every background...
                //// Add background parallax palettes
                //foreach (var p in bgPalette.Reverse())
                //    vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

                if (mode == VRAMMode.Level) {
                    paletteY += 13 - world.TilePalettes.Length;

                    // Add tile palettes
                    foreach (var p in world.TilePalettes)
                        vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                }
            } else {
                // BigRay
                vram.AddDataAt(12, 1, 0, paletteY++, bigRay.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(12, 1, 0, paletteY++, bigRay.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            }

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            FileFactory.Read<R1_PS1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.DetailedState = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            FileFactory.Read<R1_PS1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.DetailedState = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<R1_PS1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the background
            var bgPath = GetLevelBackgroundFilePath(context.Settings, true);
            if (bgPath != null)
                await LoadExtraFile(context, bgPath);

            // Load the level
            return await LoadAsync(context, level.MapData, level.EventData.Events, level.EventData.EventLinkingTable.Select(x => (ushort)x).ToArray(), loadTextures, bgPath == null ? null : level.BackgroundData);
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="lvl">The level</param>
        public override void SaveLevel(Context context, Unity_Level lvl)
        {
            /*
            var em = (R1_PS1_EditorManager)level;
            var commonLevelData = level.Level;

            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<R1_PS1_LevFile>(lvlPath);

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++)
            {
                for (int x = 0; x < lvlData.MapData.Width; x++)
                {
                    // Set the tile
                    lvlData.MapData.Tiles[y * lvlData.MapData.Width + x] = commonLevelData.Maps[0].MapTiles[y * lvlData.MapData.Width + x].Data;
                }
            }

            var newEvents = commonLevelData.EventData.Select(e =>
            {
                // Get the DES data
                var des = em.DESCollection[e.DESKey];
                var eta = em.ETACollection[e.ETAKey];

                var ed = e.Data;

                if (ed.PS1Demo_Unk1 == null)
                    ed.PS1Demo_Unk1 = new byte[40];

                if (ed.Unk_98 == null)
                    ed.Unk_98 = new byte[5];

                ed.ImageDescriptorsPointer = des.ImageDescriptorsPointer;
                ed.AnimDescriptorsPointer = des.AnimDescriptorsPointer;
                ed.ImageBufferPointer = des.ImageBufferPointer;
                ed.ETAPointer = eta.ETAPointer;

                ed.ImageDescriptorCount = (ushort)des.ImageDescriptors.Length;
                ed.AnimDescriptorCount = (byte)des.AnimDescriptors.Length;

                ed.ImageDescriptors = des.ImageDescriptors;
                ed.AnimDescriptors = des.AnimDescriptors;
                ed.Commands = e.CommandCollection;
                ed.LabelOffsets = e.LabelOffsets;
                ed.ETA = eta.ETA;
                ed.ImageBuffer = des.ImageBuffer;

                return ed;
            }).ToArray();

            var newEventLinkTable = commonLevelData.EventData.Select(x => (byte)x.LinkIndex).ToArray();

            // Create the edited block which we append to the file
            lvlData.EditedBlock = new R1_PS1_EditedLevelBlock();

            lvlData.EditedBlock.UpdateAndFillDataBlock(lvlData.Offset + lvlData.FileSize, lvlData.EventData, newEvents, newEventLinkTable, context.Settings);

            // TODO: When writing make sure that ONLY the level file gets recreated - do not touch the other files (ignore DoAt if the file needs to be switched based on some setting?)
            // Save the file
            FileFactory.Write<R1_PS1_LevFile>(lvlPath, context);*/
        }

        protected override IReadOnlyDictionary<string, string[]> LoadLocalization(Context context)
        {
            // The localization is compiled in the US/JP releases
            if (context.Settings.GameModeSelection != GameModeSelection.RaymanPS1EU)
                return null;

            var langs = new[]
            {
                new
                {
                    LangCode = "US",
                    Language = "English"
                },
                new
                {
                    LangCode = "FR",
                    Language = "French"
                },
                new
                {
                    LangCode = "GR",
                    Language = "German"
                },
            };

            // Create the dictionary
            var loc = new Dictionary<string, string[]>();

            // Add each language
            foreach (var lang in langs)
            {
                var langFile = FileFactory.ReadText<R1_TextLocFile>(GetLanguageFilePath(lang.LangCode), context);
                loc.Add(lang.Language, langFile.Strings);
            }

            return loc;
        }

        public void LogEventFlags(GameSettings settings, string outputFilePath)
        {
            if (settings.GameModeSelection != GameModeSelection.RaymanPS1US)
                throw new NotSupportedException();

            using (var context = new Context(settings))
            {
                var file = new LinearSerializedFile(context)
                {
                    filePath = "SLUS-000.05"
                };
                context.AddFile(file);
                var s = context.Deserializer;

                // flag & 8 checks if the event hurts Rayman on contact
                // flag & 4 checks if the event has collision - what do the rest do?
                var flags = s.DoAt(file.StartPointer + 0x9CA94, () => s.SerializeArray<uint>(default, 256));

                using (var log = File.Create(outputFilePath))
                {
                    using (var logWriter = new StreamWriter(log))
                    {
                        for (int i = 0; i < flags.Length; i++)
                        {
                            var type = (R1_EventType)i;
                            var attrFlag = type.GetAttribute<ObjTypeInfoAttribute>()?.Flag ?? ObjTypeFlag.Normal;

                            logWriter.WriteLine($"{Convert.ToString(flags[i], 2).PadLeft(32, '0')} - {type}{(attrFlag != ObjTypeFlag.Normal ? $" ({attrFlag})" : String.Empty)}");
                        }
                    }
                }
            }
        }
    }
}