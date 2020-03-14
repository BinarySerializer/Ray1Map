using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event info manager class
    /// </summary>
    public static class EventInfoManager
    {
        // TODO: Clean up this method - have each manager handle generating its own info? Right now it only works for PC.
        public static void GenerateCSVFiles(string outputDir)
        {
            var modes = new Dictionary<GameMode, GameModeSelection[]>()
            {
                {
                    GameMode.RayPC,
                    new GameModeSelection[]
                    {
                        GameModeSelection.RaymanPC,
                    }
                },
                {
                    GameMode.RayKit,
                    new GameModeSelection[]
                    {
                        GameModeSelection.RaymanDesignerPC,
                        GameModeSelection.RaymanByHisFansPC,
                        GameModeSelection.Rayman60LevelsPC,
                    }
                },
                {
                    GameMode.RayEduPC,
                    new GameModeSelection[]
                    {
                        GameModeSelection.RaymanEducationalPC,
                    }
                },
            };

            foreach (var mode in modes)
            {
                // Get the attribute data
                var attr = mode.Value.First().GetAttribute<GameModeAttribute>();

                // Get the manager
                var manager = (PC_Manager)Activator.CreateInstance(attr.ManagerType);

                using (var file = File.Create(Path.Combine(outputDir, $"{mode.Key.ToString()}.csv")))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        void WriteLine(params object[] values)
                        {
                            foreach (var value in values)
                            {
                                var toWrite = value?.ToString();

                                if (value is IEnumerable enu && !(enu is string))
                                {
                                    toWrite = enu.Cast<object>().Aggregate(String.Empty, (current, o) =>
                                    {
                                        var s = o is string ? "/" : "_";

                                        return current + $"{s}{o}";
                                    });

                                    if (toWrite.Length > 1)
                                        toWrite = toWrite.Remove(0, 1);
                                }

                                writer.Write($"{toWrite},");
                            }

                            writer.Flush();

                            file.Position--;
                            
                            writer.Write(Environment.NewLine);
                        }

                        WriteLine("Name", "MapperID", "World", "Type", "Etat", "SubEtat", "Flag", "DES", "ETA", "OffsetBX", "OffsetBY", "OffsetHY", "FollowSprite", "HitPoints", "HitSprite", "FollowEnabled", "ConnectedEvents", "LabelOffsets", "Commands");

                        var events = new List<GeneralEventInfoData>();

                        foreach (var modeSelection in mode.Value)
                        {
                            // Get the settings
                            var s = new GameSettings(modeSelection, Settings.GameDirectories[modeSelection])
                            {
                                EduVolume = Settings.EduVolume
                            };

                            var allfixDesCount = FileFactory.Read<PC_WorldFile>(manager.GetAllfixFilePath(s), s).DesItemCount;

                            // Enumerate each PC world
                            foreach (World world in EnumHelpers.GetValues<World>())
                            {
                                s.World = world;

                                // Enumerate each level
                                foreach (var i in manager.GetLevels(s))
                                {
                                    // Set the level
                                    s.Level = i;

                                    // Get the level file path
                                    var lvlFilePath = manager.GetLevelFilePath(s);

                                    // Read the level
                                    var lvl = FileFactory.Read<PC_LevFile>(lvlFilePath, s);

                                    var eventIndex = 0;
                                    
                                    // Add every event
                                    foreach (var e in lvl.Events)
                                    {
                                        EventWorld world2;

                                        if (e.DES <= allfixDesCount)
                                            world2 = EventWorld.All;
                                        else
                                        {
                                            switch (world)
                                            {
                                                case World.Jungle:
                                                    world2 = EventWorld.Jungle;
                                                    break;
                                                case World.Music:
                                                    world2 = EventWorld.Music;
                                                    break;
                                                case World.Mountain:
                                                    world2 = EventWorld.Mountain;
                                                    break;
                                                case World.Image:
                                                    world2 = EventWorld.Image;
                                                    break;
                                                case World.Cave:
                                                    world2 = EventWorld.Cave;
                                                    break;
                                                case World.Cake:
                                                    world2 = EventWorld.Cake;
                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException();
                                            }
                                        }

                                        // Create the event info data
                                        GeneralEventInfoData eventData = new GeneralEventInfoData(String.Empty, null, world2, (int)e.Type, e.Etat, e.SubEtat, null, (int)e.DES, (int)e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, null, lvl.EventCommands[eventIndex].LabelOffsetTable, lvl.EventCommands[eventIndex].EventCode);

                                        eventIndex++;

                                        if (!events.Any(x => x.Equals(eventData)))
                                            events.Add(eventData);
                                    }
                                }

                                // Read the event manifest if Designer
                                if (modeSelection == GameModeSelection.RaymanDesignerPC)
                                {
                                    // Get the manager
                                    var mapperManager = new PC_Mapper_Manager();

                                    // Read the event loc files and get the English items
                                    var eventLoc = mapperManager.GetEventLocFiles(s.GameDirectory)["USA"].SelectMany(x => x.LocItems).ToArray();

                                    // Read the event manifest
                                    var manifest = FileFactory.Read<PC_Mapper_EventManifestFile>(Path.Combine(s.GameDirectory, mapperManager.GetWorldName(world), "eve.mlt"), s);

                                    // Enumerate each item
                                    foreach (var e in manifest.Items)
                                    {
                                        // Get the DES index
                                        var des = -1;

                                        // Get the type
                                        var type = Int32.TryParse(e.Obj_type, out var t) ? t : -1;

                                        // Get the sub-etat
                                        var subEtat = Int32.TryParse(e.SubEtat, out var se) ? se : -1;

                                        // Get the ETA index
                                        var eta = -1;

                                        // Get the localized name
                                        var locName = eventLoc.FindItem(x => x.LocKey == e.Name)?.Name;

                                        // Create the event info data
                                        GeneralEventInfoData eventData = new GeneralEventInfoData(locName, e.Name, null, type, (int)e.Etat, subEtat, e.DesignerGroup == -1 ? (EventFlag?)EventFlag.Always : null, des, eta, (int)e.Offset_BX, (int)e.Offset_BY, (int)e.Offset_HY, (int)e.Follow_sprite, (int)e.Hitpoints, (int)e.Hit_sprite, e.Follow_enabled != 0, e.IfCommand, new ushort[0], e.EventCommands.Select(x => (byte)(sbyte)x).ToArray());

                                        if (!events.Any(x => x.Flag != EventFlag.Always && !String.IsNullOrWhiteSpace(x.MapperID) && x.MapperID == eventData.MapperID))
                                            events.Add(eventData);
                                    }
                                }
                            }
                        }
                        foreach (var e in events.OrderBy(x => x.Type).ThenBy(x => x.Etat).ThenBy(x => x.SubEtat))
                        {
                            WriteLine(e.Name, e.MapperID, e.World, e.Type, e.Etat, e.SubEtat, e.Flag, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.ConnectedEvents, e.LabelOffsets, e.Commands);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the event info
        /// </summary>
        /// <param name="mode">The game mode to get the info for</param>
        /// <returns>The loaded event info</returns>
        public static GeneralEventInfoData[] LoadEventInfo(GameModeSelection mode)
        {
            // Get the file name
            string fileName;

            switch (mode)
            {
                case GameModeSelection.RaymanPC:
                    fileName = "RayPC.csv";
                    break;
                
                case GameModeSelection.RaymanDesignerPC:
                case GameModeSelection.MapperPC: 
                    fileName = "RayKit.csv";
                    break;

                default:
                    fileName = null;
                    break;
            }

            // Return empty collection if no file was found
            if (fileName == null)
                return new GeneralEventInfoData[0];

            // Load the file if not already loaded
            if (!Cache.ContainsKey(fileName))
                Cache.Add(fileName, LoadEventInfo(fileName));

            // Return the loaded datas
            return Cache[fileName];
        }

        /// <summary>
        /// Loads the event info from the specified file
        /// </summary>
        /// <param name="filePath">The file to load from</param>
        /// <returns>The loaded info data</returns>
        private static GeneralEventInfoData[] LoadEventInfo(string filePath)
        {
            // Open the file
            using (var fileStream = File.OpenRead(filePath))
            {
                // Use a reader
                using (var reader = new StreamReader(fileStream))
                {
                    // Create the output
                    var output = new List<GeneralEventInfoData>();

                    // Skip header
                    reader.ReadLine();

                    // Read every line
                    while (!reader.EndOfStream)
                    {
                        // Read the line
                        var line = reader.ReadLine()?.Split(',');

                        // Make sure we read something
                        if (line == null)
                            break;

                        // Keep track of the value index
                        var index = 0;

                        // Helper methods for parsing values
                        string nextValue() => line[index++];
                        bool nextBoolValue() => Boolean.Parse(line[index++]);
                        int nextIntValue() => Int32.Parse(nextValue());
                        T? nextEnumValue<T>() where T : struct => Enum.TryParse(nextValue(), out T parsedEnum) ? (T?)parsedEnum : null;
                        ushort[] next16ArrayValue() => nextValue().Split('_').Where(x => !String.IsNullOrWhiteSpace(x)).Select(UInt16.Parse).ToArray();
                        byte[] next8ArrayValue() => nextValue().Split('_').Where(x => !String.IsNullOrWhiteSpace(x)).Select(Byte.Parse).ToArray();
                        string[] nextStringArrayValue() => nextValue().Split('/').Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();


                        // Add the item to the output
                        output.Add(new GeneralEventInfoData(nextValue(), nextValue(), nextEnumValue<EventWorld>(), nextIntValue(), nextIntValue(), nextIntValue(), nextEnumValue<EventFlag>(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextBoolValue(), nextStringArrayValue(), next16ArrayValue(), next8ArrayValue()));
                    }

                    // Return the output
                    return output.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the event info data which matches the specified values
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="world"></param>
        /// <param name="type"></param>
        /// <param name="etat"></param>
        /// <param name="subEtat"></param>
        /// <param name="des"></param>
        /// <param name="eta"></param>
        /// <param name="offsetBx"></param>
        /// <param name="offsetBy"></param>
        /// <param name="offsetHy"></param>
        /// <param name="followSprite"></param>
        /// <param name="hitPoints"></param>
        /// <param name="hitSprite"></param>
        /// <param name="followEnabled"></param>
        /// <param name="labelOffsets"></param>
        /// <param name="commands"></param>
        /// <returns>The item which matches the values</returns>
        public static GeneralEventInfoData GetEventInfo(GameModeSelection mode, World world, int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands)
        {
            // Load the event info
            var allInfo = LoadEventInfo(mode);

            EventWorld eventWorld;

            switch (world)
            {
                case World.Jungle:
                    eventWorld = EventWorld.Jungle;
                    break;
                case World.Music:
                    eventWorld = EventWorld.Music;
                    break;
                case World.Mountain:
                    eventWorld = EventWorld.Mountain;
                    break;
                case World.Image:
                    eventWorld = EventWorld.Image;
                    break;
                case World.Cave:
                    eventWorld = EventWorld.Cave;
                    break;
                case World.Cake:
                    eventWorld = EventWorld.Cake;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }

            // Find a matching item
            var match = allInfo.FindItem(x => (x.World == eventWorld || x.World == EventWorld.All) &&
                                  x.Type == type &&
                                  x.Etat == etat &&
                                  x.SubEtat == subEtat &&
                                  x.DES == des &&
                                  x.ETA == eta &&
                                  x.OffsetBX == offsetBx &&
                                  x.OffsetBY == offsetBy &&
                                  x.OffsetHY == offsetHy &&
                                  x.FollowSprite == followSprite &&
                                  x.HitPoints == hitPoints &&
                                  x.HitSprite == hitSprite &&
                                  x.FollowEnabled == followEnabled &&
                                  x.LabelOffsets.SequenceEqual(labelOffsets) &&
                                  x.Commands.SequenceEqual(commands));

            // Create dummy item if not found
            if (match == null)
            {
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat}");
                match = new GeneralEventInfoData(null, null, eventWorld, type, etat, subEtat, null, des, eta, offsetBx, offsetBy, offsetHy, followSprite, hitPoints, hitSprite, followEnabled, null, labelOffsets, commands);
            }

            // Return the item
            return match;
        }

        /// <summary>
        /// Gets the event info data which matches the Mapper ID
        /// </summary>
        /// <param name="mode">The game mode</param>
        /// <param name="mapperId">The Mapper ID</param>
        /// <returns>The item which matches the ID</returns>
        public static GeneralEventInfoData GetEventInfo(GameModeSelection mode, string mapperId)
        {
            // Load the event info
            var allInfo = LoadEventInfo(mode);

            // Find and return a matching item
            return allInfo.FindItem(x => x.MapperID == mapperId);
        }

        /// <summary>
        /// Parses a command
        /// </summary>
        /// <param name="cmds">The command bytes</param>
        /// <param name="labelOffsets">The label offsets</param>
        /// <returns>The parsed command</returns>
        public static string[] ParseCommands(byte[] cmds, ushort[] labelOffsets)
        {
            // Create the output
            var output = new List<string>();

            // Handle every command byte
            for (int i = 0; i < cmds.Length;)
            {
                var command = cmds[i++];
                byte ReadArg() => cmds[i++];
                sbyte ReadSArg() => (sbyte)cmds[i++];

                // Handle the commands
                switch (command)
                {
                    case 0:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;
                    
                    case 1:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;
                    
                    case 2:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;
                    
                    case 3:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;

                    case 4:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;

                    case 5:
                        output.Add($"Set SubEtat to {ReadArg()}");
                        break;

                    case 6:
                        output.Add($"Skip {ReadArg()} commands");
                        break;

                    case 7:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;

                    case 8:
                        output.Add($"Set Etat to {ReadArg()}");
                        break;

                    case 9:
                        output.Add($"Prepare loop {ReadArg()}");
                        break;

                    case 10:
                        output.Add($"Do loop");
                        break;

                    case 11:
                        output.Add($"Set label {ReadArg()}");
                        break;

                    case 12:
                        output.Add($"Go to label {ReadArg()}");
                        break;

                    case 13:
                        output.Add($"Enter sub-function with label {ReadArg()}");
                        break;

                    case 14:
                        output.Add($"Exit sub-function");
                        break;

                    case 15:
                        output.Add($"Go to label {ReadArg()} if flag is true");
                        break;

                    case 16:
                        output.Add($"Go to label {ReadArg()} if flag is false");
                        break;

                    case 17:
                        var arg1 = ReadArg();

                        output.Add(arg1 <= 4 ? $"Test {arg1} {ReadArg()}" : $"Test {arg1}");

                        break;

                    case 18:
                        output.Add($"Set test {ReadArg()}");
                        break;

                    case 19:
                        output.Add($"Wait {ReadArg()} seconds");
                        break;

                    // TODO: Might differ between commands
                    case 20:
                        output.Add($"Move for {ReadArg()} seconds, speedX {ReadSArg()} and speedY {ReadSArg()}");
                        break;

                    case 21:
                        output.Add($"Set X to {ReadArg().ToString() + ReadArg().ToString()}");
                        break;

                    case 22:
                        output.Add($"Set Y to {ReadArg().ToString() + ReadArg().ToString()}");
                        break;

                    case 23:
                        output.Add($"Skip/go to offset {labelOffsets[ReadArg()]}");
                        break;

                    case 24:
                        output.Add($"Skip/go to offset {labelOffsets[ReadArg()]}");
                        break;

                    case 25:
                        output.Add($"Enter sub-function with label {labelOffsets[ReadArg()]}");
                        break;

                    case 26:
                        output.Add($"Go to offset {labelOffsets[ReadArg()]} if flag is true");
                        break;

                    case 27:
                        output.Add($"Go to offset {labelOffsets[ReadArg()]} if flag is false");
                        break;

                    case 28:
                        output.Add($"Skip to offset {labelOffsets[ReadArg()]} if flag is true");
                        break;

                    case 29:
                        output.Add($"Skip to offset {labelOffsets[ReadArg()]} if flag is false");
                        break;

                    case 30:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;

                    case 31:
                        output.Add($"Skip {ReadArg()} commands if flag is true");
                        break;

                    case 32:
                        output.Add($"Skip {ReadArg()} commands if flag is false");
                        break;

                    case 33:
                        output.Add($"End ({ReadArg()})");
                        break;
                }
            }

            // Return the output
            return output.ToArray();
        }

        /// <summary>
        /// The loaded event info cache
        /// </summary>
        private static Dictionary<string, GeneralEventInfoData[]> Cache { get; } = new Dictionary<string, GeneralEventInfoData[]>();
    }
}