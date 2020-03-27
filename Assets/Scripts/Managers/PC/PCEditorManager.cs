using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for PC
    /// </summary>
    public class PCEditorManager : BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public PCEditorManager(Common_Lev level, Context context, PC_Manager manager, Common_Design[] designs) : base(level, context)
        {
            // Set properties
            Manager = manager;
            Designs = designs;
            EventCache = new Dictionary<string, GeneralPCEventInfoData[]>();

            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(manager.GetAllfixFilePath(Settings), Context);

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(manager.GetWorldFilePath(Settings), Context);

            // Read the big ray data
            var bigRayData = FileFactory.Read<PC_WorldFile>(manager.GetBigRayFilePath(Settings), Context);

            // Get the eta items
            ETA = allfix.Eta.Concat(worldData.Eta).Concat(bigRayData.Eta).ToArray();

        }

        /// <summary>
        /// The manager
        /// </summary>
        public PC_Manager Manager { get; }

        /// <summary>
        /// The available ETA
        /// </summary>
        public PC_ETA[] ETA { get; }

        /// <summary>
        /// The common design
        /// </summary>
        public Common_Design[] Designs { get; }

        /// <summary>
        /// The loaded PC info cache
        /// </summary>
        protected Dictionary<string, GeneralPCEventInfoData[]> EventCache { get; }

        /// <summary>
        /// Updates the state
        /// </summary>
        public override Common_EventState GetEventState(Common_Event e)
        {
            // Find the matching item based on ETA, Etat and SubEtat
            return ETA[e.ETA].States.ElementAtOrDefault(e.Etat)?.ElementAtOrDefault(e.SubEtat);
        }

        /// <summary>
        /// Gets the common design for the event based on the DES index
        /// </summary>
        /// <param name="e">The event to get the design for</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public override Common_Design GetCommonDesign(Common_Event e, int desIndex)
        {
            // TODO: Correct this if EDU letter?

            // Return the common design
            return Designs.ElementAtOrDefault(desIndex);
        }

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public override Common_EditorEventInfo GetEditorEventInfo(Common_Event e)
        {
            // Get the command bytes
            var cmds = e.CommandCollection.ToBytes();

            // Find match
            var match = GetPCEventInfo(Settings.GameModeSelection, Settings.World, (int)e.Type, e.Etat, e.SubEtat, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, cmds);

            // Return the editor info
            return new Common_EditorEventInfo(match?.Name, match?.Flag);
        }

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <returns>The names of the available events to add</returns>
        public override string[] GetEvents()
        {
            // Get the event world
            var w = Settings.World.ToEventWorld();

            // Get the events for the current world
            return LoadPCEventInfo(Settings.GameModeSelection)?.Where(x => x.World == EventWorld.All || x.World == w).Select(x => x.Name).ToArray() ?? new string[0];
        }

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="eventController">The event controller to add to</param>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns></returns>
        public override Common_Event AddEvent(LevelEventController eventController, int index, uint xPos, uint yPos)
        {
            // Get the event world
            var w = Settings.World.ToEventWorld();

            // Get the event
            var e = LoadPCEventInfo(Settings.GameModeSelection).Where(x => x.World == EventWorld.All || x.World == w).ElementAt(index);

            // TODO: Before Designer is merged we need to find the "used" commands
            var cmds = Common_EventCommandCollection.FromBytes(e.Commands.Any() ? e.Commands : e.LocalCommands);

            // Add and return the event
            return eventController.AddEvent((EventType)e.Type, e.Etat, e.SubEtat, xPos, yPos, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, cmds, 0);
        }

        /// <summary>
        /// Loads the PC event info
        /// </summary>
        /// <param name="mode">The game mode to get the info for</param>
        /// <returns>The loaded event info</returns>
        public GeneralPCEventInfoData[] LoadPCEventInfo(GameModeSelection mode)
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
                return new GeneralPCEventInfoData[0];

            // Load the file if not already loaded
            if (!EventCache.ContainsKey(fileName))
                EventCache.Add(fileName, LoadPCEventInfo(fileName));

            // Return the loaded datas
            return EventCache[fileName];
        }

        /// <summary>
        /// Loads the PC event info from the specified file
        /// </summary>
        /// <param name="filePath">The file to load from</param>
        /// <returns>The loaded info data</returns>
        private static GeneralPCEventInfoData[] LoadPCEventInfo(string filePath)
        {
            // Open the file
            using (var fileStream = File.OpenRead(filePath))
            {
                // Use a reader
                using (var reader = new StreamReader(fileStream))
                {
                    // Create the output
                    var output = new List<GeneralPCEventInfoData>();

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

                        try
                        {
                            // Helper methods for parsing values
                            string nextValue() => line[index++];
                            bool nextBoolValue() => Boolean.Parse(line[index++]);
                            int nextIntValue() => Int32.Parse(nextValue());
                            T? nextEnumValue<T>() where T : struct => Enum.TryParse<T>(nextValue(), out T parsedEnum) ? (T?)parsedEnum : null;
                            ushort[] next16ArrayValue() => nextValue().Split('_').Where<string>(x => !String.IsNullOrWhiteSpace(x)).Select<string, ushort>(UInt16.Parse).ToArray<ushort>();
                            byte[] next8ArrayValue() => nextValue().Split('_').Where<string>(x => !String.IsNullOrWhiteSpace(x)).Select<string, byte>(Byte.Parse).ToArray<byte>();
                            string[] nextStringArrayValue() => nextValue().Split('/').Where<string>(x => !String.IsNullOrWhiteSpace(x)).ToArray<string>();

                            // Add the item to the output
                            output.Add(new GeneralPCEventInfoData(nextValue(), nextValue(), nextEnumValue<EventWorld>(), nextIntValue(), nextIntValue(), nextIntValue(), nextEnumValue<EventFlag>(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextBoolValue(), nextStringArrayValue(), next16ArrayValue(), next8ArrayValue(), next8ArrayValue()));
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to parse event info. Index: {index}, items: {String.Join(" - ", line)} , exception: {ex.Message}");
                            throw;
                        }
                    }

                    // Return the output
                    return output.OrderBy(x => x.Name).ThenBy(x => x.Type).ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the event info data which matches the specified values for a PC event
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
        public GeneralPCEventInfoData GetPCEventInfo(GameModeSelection mode, World world, int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands)
        {
            // Load the event info
            var allInfo = LoadPCEventInfo(mode);

            EventWorld eventWorld = world.ToEventWorld();

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
                                              x.LabelOffsets.SequenceEqual<ushort>(labelOffsets) &&
                                              x.Commands.SequenceEqual<byte>(commands));

            // Create dummy item if not found
            if (match == null && allInfo.Any<GeneralPCEventInfoData>())
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat}");

            // Return the item
            return match;
        }
    }
}