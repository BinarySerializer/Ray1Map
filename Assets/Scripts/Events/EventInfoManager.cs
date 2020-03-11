using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                                    toWrite = enu.Cast<object>().Aggregate(String.Empty, (current, o) => current + $"_{o}");

                                    if (toWrite.Length > 1)
                                        toWrite = toWrite.Remove(0, 1);
                                }

                                writer.Write($"{toWrite},");
                            }

                            writer.Flush();

                            file.Position--;
                            
                            writer.Write(Environment.NewLine);
                        }

                        WriteLine("Name", "World", "Type", "Etat", "SubEtat", "Flag", "DES", "ETA", "OffsetBX", "OffsetBY", "OffsetHY", "FollowSprite", "HitPoints", "UnkGroup", "HitSprite", "FollowEnabled", "LabelOffsets", "Commands");

                        var events = new List<GeneralEventInfoData>();

                        foreach (var modeSelection in mode.Value)
                        {
                            // Get the settings
                            var s = new GameSettings(attr.GameMode, Settings.GameDirectories[modeSelection])
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
                                        GeneralEventInfoData eventData = new GeneralEventInfoData(String.Empty, world2, (int)e.Type, e.Etat, e.SubEtat, null, (int)e.DES, (int)e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.UnkGroup, e.HitSprite, e.FollowEnabled, lvl.EventCommands[eventIndex].LabelOffsetTable, lvl.EventCommands[eventIndex].EventCode);

                                        eventIndex++;

                                        if (events.Any(x => x.Equals(eventData)))
                                            continue;

                                        events.Add(eventData);
                                    }
                                }
                            }
                        }

                        foreach (var e in events.OrderBy(x => x.Type).ThenBy(x => x.Etat).ThenBy(x => x.SubEtat))
                        {
                            WriteLine(e.Name, e.World, e.Type, e.Etat, e.SubEtat, e.Flag, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.UnkGroup, e.HitSprite, e.FollowEnabled, e.LabelOffsets, e.Commands);
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
        public static GeneralEventInfoData[] LoadEventInfo(GameMode mode)
        {
            if (!Cache.ContainsKey(mode))
                Cache.Add(mode, LoadEventInfo($"{mode}.csv"));

            return Cache[mode];
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
                        int nextIntValue() => Int32.Parse(nextValue());
                        T? nextEnumValue<T>() where T : struct => Enum.TryParse(nextValue(), out T parsedEnum) ? (T?)parsedEnum : null;
                        ushort[] next16ArrayValue() => nextValue().Split('_').Where(x => !String.IsNullOrWhiteSpace(x)).Select(UInt16.Parse).ToArray();
                        byte[] next8ArrayValue() => nextValue().Split('_').Where(x => !String.IsNullOrWhiteSpace(x)).Select(Byte.Parse).ToArray();

                        // Add the item to the output
                        output.Add(new GeneralEventInfoData(nextValue(), nextEnumValue<EventWorld>(), nextIntValue(), nextIntValue(), nextIntValue(), nextEnumValue<EventFlag>(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), next16ArrayValue(), next8ArrayValue()));
                    }

                    // Return the output
                    return output.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the event info data which matches the specified values, or null if none was found
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
        /// <param name="unkGroup"></param>
        /// <param name="hitSprite"></param>
        /// <param name="followEnabled"></param>
        /// <param name="labelOffsets"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public static GeneralEventInfoData GetEventInfo(GameMode mode, World world, int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int unkGroup, int hitSprite, int followEnabled, ushort[] labelOffsets, byte[] commands)
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
            return allInfo.FindItem(x => (x.World == eventWorld || x.World == EventWorld.All) &&
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
                                  x.UnkGroup == unkGroup &&
                                  x.HitSprite == hitSprite &&
                                  x.FollowEnabled == followEnabled &&
                                  x.LabelOffsets.SequenceEqual(labelOffsets) &&
                                  x.Commands.SequenceEqual(commands));
        }

        /// <summary>
        /// The loaded event info cache
        /// </summary>
        private static Dictionary<GameMode, GeneralEventInfoData[]> Cache { get; } = new Dictionary<GameMode, GeneralEventInfoData[]>();
    }
}