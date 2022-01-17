﻿using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace Ray1Map.Rayman1
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
        public override Unity_TileSet GetTileSet(Context context)
        {
            if (context.GetR1Settings().R1_World == World.Menu)
                return new Unity_TileSet(Settings.CellSize);

            // Get the file name
            var filename = GetWorldFilePath(context.GetR1Settings());

            // Read the file
            var worldFile = FileFactory.Read<PS1_WorldFile>(context, filename);

            int tileCount = worldFile.TilePaletteIndexTable.Length;
            int width = TileSetWidth * Settings.CellSize;
            int height = (worldFile.PalettedTiles.Length) / width;

            var pixels = new RGBA5551Color[width * height];

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
                    pixels[pixel] = new RGBA5551Color();
                }
                else
                {
                    byte tileIndex1 = worldFile.TilePaletteIndexTable[tile];
                    byte tileIndex2 = worldFile.PalettedTiles[pixel];
                    pixels[pixel] = worldFile.TilePalettes[tileIndex1][tileIndex2];
                }
            }

            return new Unity_TileSet(pixels, TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, PS1VramHelpers.VRAMMode mode)
        {
            // Read the files
            var allFix = mode != PS1VramHelpers.VRAMMode.BigRay ? FileFactory.Read<PS1_AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings())) : null;
            var world = mode == PS1VramHelpers.VRAMMode.Level ? FileFactory.Read<PS1_WorldFile>(context, GetWorldFilePath(context.GetR1Settings())) : null;
            var lev = mode == PS1VramHelpers.VRAMMode.Level ? FileFactory.Read<PS1_LevFile>(context, GetLevelFilePath(context.GetR1Settings())) : null;
            var bigRay = mode == PS1VramHelpers.VRAMMode.BigRay ? FileFactory.Read<PS1_BigRayFile>(context, GetBigRayFilePath(context.GetR1Settings())) : null;
            var font = mode == PS1VramHelpers.VRAMMode.Menu ? FileFactory.Read<Array<byte>>(context, GetFontFilePath(context.GetR1Settings()), (s, o) => o.Pre_Length = s.CurrentLength) : null;

            var vram = PS1VramHelpers.PS1_FillVRAM(mode, allFix, world, bigRay, lev, font?.Value, context.GetR1Settings().GameModeSelection == GameModeSelection.RaymanPS1US);

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.GetR1Settings()), false);
            FileFactory.Read<PS1_AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings()));

            PS1_ObjBlock objBlock = null;
            MapData mapData;

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                Controller.DetailedState = $"Loading world file";

                await Controller.WaitIfNecessary();

                // Read the world file
                await LoadExtraFile(context, GetWorldFilePath(context.GetR1Settings()), false);
                FileFactory.Read<PS1_WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

                Controller.DetailedState = $"Loading map data";

                // Read the level data
                await LoadExtraFile(context, GetLevelFilePath(context.GetR1Settings()), true);
                var level = FileFactory.Read<PS1_LevFile>(context, GetLevelFilePath(context.GetR1Settings()));

                objBlock = level.ObjData;
                mapData = level.MapData;
            }
            else
            {
                await LoadExtraFile(context, GetFontFilePath(context.GetR1Settings()), false);

                mapData = MapData.GetEmptyMapData(384 / Settings.CellSize, 288 / Settings.CellSize);
            }

            // Load the level
            return await LoadAsync(context, mapData, objBlock?.Objects, objBlock?.ObjectLinkingTable.Select(x => (ushort)x).ToArray(), 
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
            // Menu levels can't be saved
            if (context.GetR1Settings().R1_World == World.Menu)
                return UniTask.CompletedTask;

            // Get the level file path
            var lvlPath = GetLevelFilePath(context.GetR1Settings());

            // Get the level data
            var lvlData = context.GetMainFileObject<PS1_LevFile>(lvlPath);

            // Get the object manager
            var objManager = (Unity_ObjectManager_R1)lvl.ObjManager;

            // Update the tiles
            for (int y = 0; y < lvlData.MapData.Height; y++)
            {
                for (int x = 0; x < lvlData.MapData.Width; x++)
                {
                    // Set the tile
                    lvlData.MapData.Tiles[y * lvlData.MapData.Width + x] = lvl.Maps[0].MapTiles[y * lvlData.MapData.Width + x].Data.ToR1MapTile();
                }
            }

            var newEvents = lvl.EventData.Cast<Unity_Object_R1>().Select(e =>
            {
                var ed = e.EventData;

                if (ed.PS1Demo_Unk1 == null)
                    ed.PS1Demo_Unk1 = new byte[40];

                if (ed.CollisionTypes == null)
                    ed.CollisionTypes = new TileCollisionType[5];

                if (ed.CommandContexts == null)
                    ed.CommandContexts = new ObjData.CommandContext[]
                    {
                        new ObjData.CommandContext()
                    };

                // TODO: Do this in the Unity_Object instead
                ed.SpritesCount = (ushort)objManager.DES[e.DESIndex].Data.ImageDescriptors.Length;
                ed.AnimationsCount = (byte)objManager.DES[e.DESIndex].Data.Graphics.Animations.Count;

                // TODO: Get from DESData in obj manager instead?
                ed.SpriteCollection = FileFactory.Read<SpriteCollection>(context, ed.SpritesPointer, (s, o) => o.Pre_SpritesCount = ed.SpritesCount);
                ed.AnimationCollection = FileFactory.Read<AnimationCollection>(context, ed.AnimationsPointer, (s, o) => o.Pre_AnimationsCount = ed.AnimationsCount);
                ed.ETA = context.Cache.FromOffset<BinarySerializer.Ray1.ETA>(ed.ETAPointer);
                
                // TODO: Update this
                //ed.ImageBuffer = des.ImageBuffer;

                return ed;
            }).ToArray();

            var newEventLinkTable = objManager.LinkTable.Select(x => (byte)x).ToArray();

            // Relocate pointers to a new block of data we append to the level file
            UpdateAndFillDataBlock(lvlData.Offset + lvlData.FileSize, lvlData.ObjData, newEvents, newEventLinkTable, context.GetR1Settings());

            // TODO: When writing make sure that ONLY the level file gets recreated - do not touch the other files (ignore DoAt if the file needs to be switched based on some setting?)
            // Save the file
            FileFactory.Write<PS1_LevFile>(context, lvlPath);

            // Create ISO for the modified data
            CreateISO(context);

            return UniTask.CompletedTask;
        }

        public void UpdateAndFillDataBlock(Pointer offset, PS1_ObjBlock originalBlock, ObjData[] events, byte[] eventLinkingTable, GameSettings settings)
        {
            long currentOffset = 0;
            Pointer getCurrentBlockPointer()
            {
                // Align by 4
                if (currentOffset % 4 != 0)
                    currentOffset += 4 - currentOffset % 4;

                return offset + (1 * 4) + currentOffset;
            }

            originalBlock.ObjectsCount = (byte)events.Length;
            originalBlock.ObjectsPointer = getCurrentBlockPointer();
            originalBlock.Objects = events;

            currentOffset += events.Length * 112;

            originalBlock.ObjectLinksCount = (byte)eventLinkingTable.Length;
            originalBlock.ObjectLinksPointer = getCurrentBlockPointer();
            originalBlock.ObjectLinkingTable = eventLinkingTable;

            currentOffset += eventLinkingTable.Length;

            foreach (var e in events)
            {
                if (e.Commands != null)
                {
                    e.CommandsPointer = getCurrentBlockPointer();
                    currentOffset += e.Commands.ToBytes(() => new Ray1MapContext(settings)).Length;
                }
                else
                {
                    e.CommandsPointer = null;
                }

                if (e.LabelOffsets != null)
                {
                    e.LabelOffsetsPointer = getCurrentBlockPointer();
                    currentOffset += e.LabelOffsets.Length * 2;
                }
                else
                {
                    e.LabelOffsetsPointer = null;
                }
            }
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
                        var exe = LoadEXE(context);

                        // Update every file path in the file table
                        foreach (var fileEntry in exe.PS1_FileTable)
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
                        FileFactory.Write<PS1_Executable>(context, ExeFilePath);
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