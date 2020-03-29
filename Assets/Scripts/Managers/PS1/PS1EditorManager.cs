using System;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for PS1
    /// </summary>
    public class PS1EditorManager : BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        public PS1EditorManager(Common_Lev level, Context context, PS1_Manager manager, Common_Design[] designs) : base(level, context)
        {
            // Set properties
            Manager = manager;
            Designs = designs;
        }

        /// <summary>
        /// The manager
        /// </summary>
        public PS1_Manager Manager { get; }

        /// <summary>
        /// The common design
        /// </summary>
        public Common_Design[] Designs { get; }

        /// <summary>
        /// Updates the state
        /// </summary>
        public override Common_EventState GetEventState(Common_Event e)
        {
            if (!(Manager is PS1_BaseXXX_Manager xxx))
                return null;

            // TODO: Change this - we don't want the ETA index to be an event index, instead we want it to be taken from some list...? But how to get it to dynamically update based on Etat and SubEtat?

            // TODO: We can change this by referencing the array of states & substates somewhere in the event object.
            // This can be the global array for the PC version, but for PS1 we'll need to do it per event as this is how the game sees it.
            return FileFactory.Read<PS1_R1_LevFile>(xxx.GetLevelFilePath(Settings), Context).EventData.Events[e.ETA].EventState;
        }

        /// <summary>
        /// Gets the common design for the event based on the DES index
        /// </summary>
        /// <param name="e">The event to get the design for</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public override Common_Design GetCommonDesign(Common_Event e, int desIndex)
        {
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
            // TODO: Implement
            return null;
        }

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <returns>The names of the available events to add</returns>
        public override string[] GetEvents()
        {
            // TODO: Implement
            return new string[0];
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
            throw new NotImplementedException();
        }
    }
}