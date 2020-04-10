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
        protected BaseEditorManager(Common_Lev level, Context context)
        {
            // Set properties
            Level = level;
            Context = context;

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
        /// Gets the maximum allowed DES value
        /// </summary>
        public abstract int GetMaxDES { get; }

        /// <summary>
        /// Gets the maximum allowed ETA value
        /// </summary>
        public abstract int GetMaxETA { get; }

        /// <summary>
        /// Gets the maximum allowed Etat value
        /// </summary>
        /// <param name="eta">The ETA value</param>
        public abstract int GetMaxEtat(int eta);

        /// <summary>
        /// Gets the maximum allowed SubEtat value
        /// </summary>
        /// <param name="eta">The ETA value</param>
        /// <param name="etat">The etat value</param>
        public abstract int GetMaxSubEtat(int eta, int etat);

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
        /// Updates the state
        /// </summary>
        public abstract Common_EventState GetEventState(Common_EventData e);

        /// <summary>
        /// Gets the common design for the event based on the DES index
        /// </summary>
        /// <param name="e">The event to get the design for</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public abstract Common_Design GetCommonDesign(Common_EventData e, int desIndex);

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

            // Get the commands from the bytes
            var cmds = Common_EventCommandCollection.FromBytes(UsesLocalCommands ? e.LocalCommands : e.Commands);

            // Return the event
            return new Common_EventData
            {
                Type = (EventType)e.Type,
                Etat = e.Etat,
                SubEtat = e.SubEtat,
                XPosition = xPos,
                YPosition = yPos,
                DES = GetDesIndex(e) ?? -1,
                ETA = GetEtaIndex(e) ?? -1,
                OffsetBX = e.OffsetBX,
                OffsetBY = e.OffsetBY,
                OffsetHY = e.OffsetHY,
                FollowSprite = e.FollowSprite,
                HitPoints = e.HitPoints,
                Layer = 0,
                HitSprite = e.HitSprite,
                FollowEnabled = e.FollowEnabled,
                LabelOffsets = UsesLocalCommands ? new ushort[0] : e.LabelOffsets,
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
        /// <returns></returns>
        public GeneralEventInfoData GetGeneralEventInfo(int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands)
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
                                                    GetDesIndex(x) == des &&
                                                    GetEtaIndex(x) == eta &&
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
            var cmds = e.CommandCollection?.ToBytes();

            // Find match
            var match = GetGeneralEventInfo((int)e.Type, e.Etat, e.SubEtat, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, cmds);

            // Return the editor info
            return new Common_EditorEventInfo(match?.Name);
        }

        /// <summary>
        /// Gets the DES index for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES index</returns>
        public abstract int? GetDesIndex(GeneralEventInfoData eventInfoData);

        /// <summary>
        /// Gets the ETA index for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA index</returns>
        public abstract int? GetEtaIndex(GeneralEventInfoData eventInfoData);

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public abstract bool IsAvailableInWorld(GeneralEventInfoData eventInfoData);
    }
}