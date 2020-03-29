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
        /// <param name="eta">The available event states</param>
        public PS1EditorManager(Common_Lev level, Context context, PS1_Manager manager, Common_Design[] designs, Common_EventState[][][] eta) : base(level, context)
        {
            // Set properties
            Manager = manager;
            Designs = designs;
            ETA = eta;
        }

        /// <summary>
        /// Gets the maximum allowed DES value
        /// </summary>
        public override int GetMaxDES => Designs.Length - 1;

        /// <summary>
        /// Gets the maximum allowed ETA value
        /// </summary>
        public override int GetMaxETA => ETA.Length - 1;

        /// <summary>
        /// Gets the maximum allowed Etat value
        /// </summary>
        /// <param name="eta">The ETA value</param>
        public override int GetMaxEtat(int eta) => ETA[eta].Length - 1;

        /// <summary>
        /// Gets the maximum allowed SubEtat value
        /// </summary>
        /// <param name="eta">The ETA value</param>
        /// <param name="etat">The etat value</param>
        public override int GetMaxSubEtat(int eta, int etat) => ETA[eta][etat].Length - 1;

        /// <summary>
        /// The manager
        /// </summary>
        public PS1_Manager Manager { get; }

        /// <summary>
        /// The common design
        /// </summary>
        public Common_Design[] Designs { get; }

        /// <summary>
        /// The available event states
        /// </summary>
        public Common_EventState[][][] ETA { get; }

        /// <summary>
        /// Updates the state
        /// </summary>
        public override Common_EventState GetEventState(Common_Event e)
        {
            return ETA[e.ETA].ElementAtOrDefault(e.Etat)?.ElementAtOrDefault(e.SubEtat);
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