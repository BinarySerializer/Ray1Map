using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for PC
    /// </summary>
    public class MapperEditorManager : PCEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public MapperEditorManager(Common_Lev level, Context context, PC_Manager manager, Common_Design[] designs) : base(level, context, manager, designs)
        { }

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public override Common_EditorEventInfo GetEditorEventInfo(Common_Event e)
        {
            // Get the command bytes
            var cmd = e.CommandCollection.ToBytes();

            // Find match
            var match = GetMapperEventInfo(Settings.GameModeSelection, Settings.World, (int)e.Type, e.Etat, e.SubEtat, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, cmd);

            // Return the editor info
            return new Common_EditorEventInfo(match?.Name, match?.Flag);
        }

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <returns>The names of the available events to add</returns>
        public override string[] GetEvents()
        {
            // TODO: Find valid ones before we merge Designer/Mapper events

            // Get the event world
            var w = Settings.World.ToEventWorld();

            return LoadPCEventInfo(Settings.GameModeSelection)?.Where(x => x.MapperID != null).Where(x => x.World == EventWorld.All || x.World == w).Select(x => x.Name).ToArray() ?? new string[0];
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

            // Add and return the event
            return eventController.AddEvent((EventType)e.Type, e.Etat, e.SubEtat, xPos, yPos, e.DES, e.ETA, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.LabelOffsets, Common_EventCommandCollection.FromBytes(e.LocalCommands), 0);
        }

        /// <summary>
        /// Gets the event info data which matches the specified values for a Mapper event
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
        /// <param name="localCommands"></param>
        /// <returns>The item which matches the values</returns>
        public GeneralPCEventInfoData GetMapperEventInfo(GameModeSelection mode, World world, int type, int etat, int subEtat, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, byte[] localCommands)
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
                                              x.LocalCommands.SequenceEqual<byte>(localCommands));

            // Create dummy item if not found
            if (match == null && allInfo.Any())
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat}");

            // Return the item
            return match;
        }
    }
}