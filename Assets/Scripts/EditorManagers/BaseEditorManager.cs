using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A base editor manager
    /// </summary>
    public abstract class BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="des">The event designs</param>
        /// <param name="eta">The event states</param>
        protected BaseEditorManager(Common_Lev level, Context context, IReadOnlyDictionary<string, Common_Design> des, IReadOnlyDictionary<string, Common_EventState[][]> eta)
        {
            // Set properties
            Level = level;
            Context = context;
            DES = des;
            ETA = eta;

            // Helper method for getting the names from an event type enum
            string[] getEventTypeNames<T>()
                where T : Enum
            {
                // Get the values
                var values = EnumHelpers.GetValues<T>();

                // Get the max value
                var max = values.Cast<ushort>().Max();

                // Create the array
                var names = new string[max + 1];

                // Add every value
                for (int i = 0; i < names.Length; i++)
                    names[i] = Enum.GetName(typeof(T), i) ?? $"Type_{i}";

                return names;
            }

            // TODO: We should limit the types further (such as for Designer, EDU etc.). We could do this by tagging each event enum value with the platforms it's available on, or have a max value for each platform.
            // Set the available event types based on game
            if (context.Settings.Game == Game.Rayman2)
                EventTypes = getEventTypeNames<PS1_R2Demo_EventType>();
            else
                EventTypes = getEventTypeNames<EventType>();


            // Load the event info data
            using (var csvFile = File.OpenRead("Events.csv"))
                EventInfoData = GeneralEventInfoData.ReadCSV(csvFile).Where(IsAvailableInWorld).ToArray();
        }

        /// <summary>
        /// The loaded event info
        /// </summary>
        protected GeneralEventInfoData[] EventInfoData { get; }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected abstract bool UsesLocalCommands { get; }

        /// <summary>
        /// The event designs
        /// </summary>
        public IReadOnlyDictionary<string, Common_Design> DES { get; }

        /// <summary>
        /// The event states
        /// </summary>
        public IReadOnlyDictionary<string, Common_EventState[][]> ETA { get; }

        /// <summary>
        /// The available event type names
        /// </summary>
        public string[] EventTypes { get; }

        /// <summary>
        /// The common level
        /// </summary>
        public Common_Lev Level { get; }

        /// <summary>
        /// The context
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// The game settings
        /// </summary>
        public GameSettings Settings => Context.Settings;

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <returns>The names of the available events to add</returns>
        public string[] GetEvents() => EventInfoData.Select(x => x.Name).ToArray();

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns>The event</returns>
        public Common_EventData AddEvent(int index, uint xPos, uint yPos)
        {
            // Get the event
            var e = EventInfoData[index];

            // Get the commands and label offsets
            Common_EventCommandCollection cmds;
            ushort[] labelOffsets;

            // If local (non-compiled) commands are used, attempt to get them from the event info or decompile the compiled ones
            if (UsesLocalCommands)
            {
                cmds = e.LocalCommands.Any() 
                    ? Common_EventCommandCollection.FromBytes(e.LocalCommands, Settings) 
                    : EventCommandCompiler.Decompile(new EventCommandCompiler.CompiledEventCommandData(Common_EventCommandCollection.FromBytes(e.Commands, Settings), e.LabelOffsets), e.Commands);

                // Local commands don't use label offsets
                labelOffsets = new ushort[0];
            }
            else
            {
                if (e.Commands.Any())
                {
                    cmds = Common_EventCommandCollection.FromBytes(e.Commands, Settings);
                    labelOffsets = e.LabelOffsets;
                }
                else
                {
                    var cmdData = EventCommandCompiler.Compile(Common_EventCommandCollection.FromBytes(e.LocalCommands, Settings), e.LocalCommands);
                    cmds = cmdData.Events;
                    labelOffsets = cmdData.LabelOffsets;
                }
            }

            // Return the event
            return new Common_EventData
            {
                Type = (EventType)e.Type,
                Etat = e.Etat,
                SubEtat = e.SubEtat,
                XPosition = xPos,
                YPosition = yPos,
                DESKey = GetDesKey(e),
                ETAKey = GetEtaKey(e),
                OffsetBX = e.OffsetBX,
                OffsetBY = e.OffsetBY,
                OffsetHY = e.OffsetHY,
                FollowSprite = e.FollowSprite,
                HitPoints = e.HitPoints,
                Layer = 0,
                HitSprite = e.HitSprite,
                FollowEnabled = e.FollowEnabled,
                LabelOffsets = labelOffsets,
                CommandCollection = cmds,
                LinkIndex = 0
            };
        }

        /// <summary>
        /// Gets the general event info data which matches the specified values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="etat"></param>
        /// <param name="subEtat"></param>
        /// <param name="desKey"></param>
        /// <param name="etaKey"></param>
        /// <param name="offsetBx"></param>
        /// <param name="offsetBy"></param>
        /// <param name="offsetHy"></param>
        /// <param name="followSprite"></param>
        /// <param name="hitPoints"></param>
        /// <param name="hitSprite"></param>
        /// <param name="followEnabled"></param>
        /// <param name="labelOffsets"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public GeneralEventInfoData GetGeneralEventInfo(int type, int etat, int subEtat, string desKey, string etaKey, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands)
        {
            // Helper method for comparing the commands
            bool compareCommands(GeneralEventInfoData e)
            {
                if (UsesLocalCommands)
                    return e.LocalCommands.SequenceEqual(commands);
                else
                    return e.LabelOffsets.SequenceEqual(labelOffsets) &&
                           e.Commands.SequenceEqual(commands);
            }

            // Find a matching item
            var match = EventInfoData.FindItem(x => IsAvailableInWorld(x) &&
                                                    x.Type == type &&
                                                    x.Etat == etat &&
                                                    x.SubEtat == subEtat &&
                                                    GetDesKey(x) == desKey &&
                                                    GetEtaKey(x) == etaKey &&
                                                    x.OffsetBX == offsetBx &&
                                                    x.OffsetBY == offsetBy &&
                                                    x.OffsetHY == offsetHy &&
                                                    x.FollowSprite == followSprite &&
                                                    x.HitPoints == hitPoints &&
                                                    x.HitSprite == hitSprite &&
                                                    x.FollowEnabled == followEnabled &&
                                                    compareCommands(x));

            // Create dummy item if not found
            if (match == null && EventInfoData.Any())
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat}");

            // Return the item
            return match;
        }

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public Common_EditorEventInfo GetEditorEventInfo(Common_EventData e)
        {
            // Get the command bytes
            var cmds = e.CommandCollection?.ToBytes(Settings);

            // Find match
            var match = GetGeneralEventInfo((ushort)(object)e.Type, e.Etat, e.SubEtat, e.DESKey, e.ETAKey, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, cmds);

            // Return the editor info
            return new Common_EditorEventInfo(match?.Name);
        }

        /// <summary>
        /// Gets the DES key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES key</returns>
        public abstract string GetDesKey(GeneralEventInfoData eventInfoData);

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public abstract string GetEtaKey(GeneralEventInfoData eventInfoData);
        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public abstract bool IsAvailableInWorld(GeneralEventInfoData eventInfoData);
    }
}