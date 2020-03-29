using R1Engine.Serialize;

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
            Level = level;
            Context = context;
        }

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
        public abstract Common_EventState GetEventState(Common_Event e);

        /// <summary>
        /// Gets the common design for the event based on the DES index
        /// </summary>
        /// <param name="e">The event to get the design for</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public abstract Common_Design GetCommonDesign(Common_Event e, int desIndex);

        /// <summary>
        /// Gets the common editor event info for an event
        /// </summary>
        /// <param name="e">The event</param>
        /// <returns>The common editor event info</returns>
        public abstract Common_EditorEventInfo GetEditorEventInfo(Common_Event e);

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <returns>The names of the available events to add</returns>
        public abstract string[] GetEvents();

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="eventController">The event controller to add to</param>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns></returns>
        public abstract Common_Event AddEvent(LevelEventController eventController, int index, uint xPos, uint yPos);
    }
}