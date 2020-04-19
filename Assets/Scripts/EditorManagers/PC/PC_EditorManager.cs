using System.Collections.Generic;
using R1Engine.Serialize;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The base editor manager for PC
    /// </summary>
    public abstract class PC_EditorManager : BaseEditorManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="manager">The manager</param>
        /// <param name="designs">The common design</param>
        protected PC_EditorManager(Common_Lev level, Context context, PC_Manager manager, Common_Design[] designs) : base(level, context)
        {
            // Set properties
            Manager = manager;
            Designs = designs;

            // Read the fixed data
            var allfix = FileFactory.Read<PC_WorldFile>(manager.GetAllfixFilePath(Settings), Context, (s, x) => x.FileType = PC_WorldFile.Type.AllFix);

            // Read the world data
            var worldData = FileFactory.Read<PC_WorldFile>(manager.GetWorldFilePath(Settings), Context, (s, x) => x.FileType = PC_WorldFile.Type.World);

            // Read the big ray data
            var bigRayData = FileFactory.Read<PC_WorldFile>(manager.GetBigRayFilePath(Settings), Context, (s, x) => x.FileType = PC_WorldFile.Type.BigRay);

            // Get the eta items
            ETA = allfix.Eta.Concat(worldData.Eta).Concat(bigRayData.Eta).ToArray();
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
        public override int GetMaxEtat(int eta) => ETA.ElementAtOrDefault(eta)?.States.Length - 1 ?? 0;

        /// <summary>
        /// Gets the maximum allowed SubEtat value
        /// </summary>
        /// <param name="eta">The ETA value</param>
        /// <param name="etat">The etat value</param>
        public override int GetMaxSubEtat(int eta, int etat) => ETA.ElementAtOrDefault(eta)?.States.ElementAtOrDefault(etat)?.Length - 1 ?? 0;

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
            while ((state = Add(ETA[e.ETA].States.ElementAtOrDefault(etat)?.ElementAtOrDefault(subEtat))) != null && ((state.LinkedEtat != e.Etat || state.LinkedSubEtat != e.SubEtat) && (state.LinkedEtat != etat || state.LinkedSubEtat != subEtat)))
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
    }
}