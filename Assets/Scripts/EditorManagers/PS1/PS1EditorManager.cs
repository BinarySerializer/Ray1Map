using System.Collections.Generic;
using R1Engine.Serialize;
using System.Linq;

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
        /// Indicates if the local commands should be used
        /// </summary>
        protected override bool UsesLocalCommands => false;

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
        public override int GetMaxEtat(int eta) => ETA.ElementAtOrDefault(eta)?.Length - 1 ?? 0;

        /// <summary>
        /// Gets the maximum allowed SubEtat value
        /// </summary>
        /// <param name="eta">The ETA value</param>
        /// <param name="etat">The etat value</param>
        public override int GetMaxSubEtat(int eta, int etat) => ETA.ElementAtOrDefault(eta)?.ElementAtOrDefault(etat)?.Length - 1 ?? 0;

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
        /// Gets the event states
        /// </summary>
        public override Common_EventState[] GetEventStates(Common_EventData e)
        {
            var etat = e.Etat;
            var subEtat = e.SubEtat;
            Common_EventState state;
            var states = new List<Common_EventState>();

            // Helper method for adding a state
            Common_EventState Add(Common_EventState s)
            {
                states.Add(s);
                return s;
            }

            // Get all linked states
            while ((state = Add(ETA[e.ETA].ElementAtOrDefault(etat)?.ElementAtOrDefault(subEtat))) != null && ((state.LinkedEtat != e.Etat || state.LinkedSubEtat != e.SubEtat) && (state.LinkedEtat != etat || state.LinkedSubEtat != subEtat)))
            {
                // Set the state values for the next state
                etat = state.LinkedEtat;
                subEtat = state.LinkedSubEtat;
            }

            return states.ToArray();
        }

        /// <summary>
        /// Gets the common design for the event based on the DES index
        /// </summary>
        /// <param name="e">The event to get the design for</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public override Common_Design GetCommonDesign(Common_EventData e, int desIndex)
        {
            // Return the common design
            return Designs.ElementAtOrDefault(desIndex);
        }

        /// <summary>
        /// Gets the DES index for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES index</returns>
        public override int? GetDesIndex(GeneralEventInfoData eventInfoData)
        {
            return null;
        }

        /// <summary>
        /// Gets the ETA index for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA index</returns>
        public override int? GetEtaIndex(GeneralEventInfoData eventInfoData)
        {
            return null;
        }

        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData)
        {
            return false;
        }
    }
}