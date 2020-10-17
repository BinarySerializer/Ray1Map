using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1)
    /// </summary>
    public abstract class R1_PS1_Manager : R1_PS1BaseXXXManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

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
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings), false);
            FileFactory.Read<R1_PS1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.DetailedState = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings), false);
            FileFactory.Read<R1_PS1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.DetailedState = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings), true);
            var level = FileFactory.Read<R1_PS1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the level
            return await LoadAsync(context, level.MapData, level.EventData.Events, level.EventData.EventLinkingTable.Select(x => (ushort)x).ToArray(), loadTextures, 
                // TODO: Include bg block once we parse the palette correctly
                null);
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="lvl">The level</param>
        public override UniTask SaveLevelAsync(Context context, Unity_Level lvl)
        {
            // Get the level file path
            var lvlPath = GetLevelFilePath(context.Settings);

            // Get the level data
            var lvlData = context.GetMainFileObject<R1_PS1_LevFile>(lvlPath);

            // Get the object manager
            var objManager = (Unity_ObjectManager_R1)lvl.ObjManager;

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++)
            {
                for (int x = 0; x < lvlData.MapData.Width; x++)
                {
                    // Set the tile
                    lvlData.MapData.Tiles[y * lvlData.MapData.Width + x] = lvl.Maps[0].MapTiles[y * lvlData.MapData.Width + x].Data;
                }
            }

            var newEvents = lvl.EventData.Cast<Unity_Object_R1>().Select(e =>
            {
                var ed = e.EventData;

                if (ed.PS1Demo_Unk1 == null)
                    ed.PS1Demo_Unk1 = new byte[40];

                if (ed.Unk_98 == null)
                    ed.Unk_98 = new byte[5];

                // TODO: Do this in the Unity_Object instead
                ed.ImageDescriptorCount = (ushort)objManager.DES[e.DESIndex].Data.ImageDescriptors.Length;
                ed.AnimDescriptorCount = (byte)objManager.DES[e.DESIndex].Data.Graphics.Animations.Count;

                // TODO: Get from DESData in obj manager instead?
                ed.ImageDescriptors = FileFactory.Read<ObjectArray<R1_ImageDescriptor>>(ed.ImageDescriptorsPointer, context, (s, o) => o.Length = ed.ImageDescriptorCount).Value;
                ed.AnimDescriptors = FileFactory.Read<ObjectArray<R1_PS1_AnimationDescriptor>>(ed.AnimDescriptorsPointer, context, (s, o) => o.Length = ed.AnimDescriptorCount).Value;
                ed.ETA = context.Cache.FromOffset<R1_PS1_ETA>(ed.ETAPointer);
                
                // TODO: Update this
                //ed.ImageBuffer = des.ImageBuffer;

                return ed;
            }).ToArray();

            var newEventLinkTable = objManager.LinkTable.Select(x => (byte)x).ToArray();

            // Create the edited block which we append to the file
            lvlData.EditedBlock = new R1_PS1_EditedLevelBlock();

            lvlData.EditedBlock.UpdateAndFillDataBlock(lvlData.Offset + lvlData.FileSize, lvlData.EventData, newEvents, newEventLinkTable, context.Settings);

            // TODO: When writing make sure that ONLY the level file gets recreated - do not touch the other files (ignore DoAt if the file needs to be switched based on some setting?)
            // Save the file
            FileFactory.Write<R1_PS1_LevFile>(lvlPath, context);

            // Create ISO for the modified data
            CreateISO(context);

            return UniTask.CompletedTask;
        }

        protected void CreateISO(Context context)
        {
            // Get the xml file
            const string xmlFilePath = "disc.xml";

            // Close the context so the files can be accessed by other processes
            context.Close();

            // Create a temporary file for the LBA log
            using (var lbaLogFile = new TempFile())
            {
                // Recalculate the LBA for the files on the disc
                ProcessHelpers.RunProcess(Settings.Tool_mkpsxiso_filePath, new string[]
                {
                    "-lba", ProcessHelpers.GetStringAsPathArg(lbaLogFile.TempPath), // Specify LBA log path
                    "-noisogen", // Don't generate an ISO now
                    xmlFilePath // The xml path
                }, workingDir: context.BasePath);

                // Read the LBA log
                using (var lbaLogStream = lbaLogFile.OpenRead())
                {
                    using (var reader = new StreamReader(lbaLogStream))
                    {
                        // Skip initial lines
                        for (int i = 0; i < 8; i++)
                            reader.ReadLine();

                        var logEntries = new List<LBALogEntry>();
                        var currentDirs = new List<string>();

                        // Read all log entries
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();

                            if (line == null)
                                break;

                            var words = line.Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();

                            if (!words.Any())
                                continue;

                            var entry = new LBALogEntry(words, currentDirs);

                            logEntries.Add(entry);

                            if (entry.EntryType == LBALogEntry.Type.Dir)
                                currentDirs.Add(entry.Name);

                            if (entry.EntryType == LBALogEntry.Type.End)
                                currentDirs.RemoveAt(currentDirs.Count - 1);
                        }

                        // Read the game exe
                        var exe = FileFactory.Read<R1_PS1_Executable>(ExeFilePath, context);

                        // Update every file path in the file table
                        foreach (var fileEntry in exe.FileTable)
                        {
                            // Get the matching entry
                            var entry = logEntries.FirstOrDefault(x => x.FullPath == fileEntry.FilePath);

                            if (entry == null)
                            {
                                if (!String.IsNullOrWhiteSpace(fileEntry.FilePath))
                                    Debug.Log($"LBA not updated for {fileEntry.FilePath}");

                                continue;
                            }

                            // Update the LBA and size
                            fileEntry.LBA = entry.LBA;
                            fileEntry.FileSize = (uint)entry.Bytes;
                        }

                        // Write the game exe
                        FileFactory.Write<R1_PS1_Executable>(ExeFilePath, context);
                    }
                }
            }

            // Close context again so the exe can be accessed
            context.Close();

            // Create a new ISO
            ProcessHelpers.RunProcess(Settings.Tool_mkpsxiso_filePath, new string[]
            {
                "-y", // Set to always overwrite
                xmlFilePath // The xml path
            }, workingDir: context.BasePath, logInfo: false);
        }

        private class LBALogEntry
        {
            public LBALogEntry(IReadOnlyList<string> words, IReadOnlyList<string> dirs)
            {
                EntryType = (Type)Enum.Parse(typeof(Type), words[0], true);
                Name = words[1];

                if (EntryType == Type.End)
                    return;

                Length = Int32.Parse(words[2]);
                LBA = Int32.Parse(words[3]);
                TimeCode = words[4];
                Bytes = Int32.Parse(words[5]);
                
                if (EntryType != Type.Dir)
                    SourceFile = words[6];

                FullPath = $"\\{String.Join("\\", dirs.Append(Name))}";
            }

            public Type EntryType { get; }
            public string Name { get; }
            public int Length { get; }
            public int LBA { get; }
            public string TimeCode { get; }
            public int Bytes { get; }
            public string SourceFile { get; }

            public string FullPath { get; }

            public enum Type
            {
                File,
                STR,
                XA,
                CDDA,
                Dir,
                End
            }
        }
    }
}