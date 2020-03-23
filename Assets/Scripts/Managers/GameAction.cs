namespace R1Engine
{
    /// <summary>
    /// A game action
    /// </summary>
    public class GameAction
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="displayName">The action display name</param>
        /// <param name="requiresInputDir">Indicates if the action requires an input directory</param>
        /// <param name="requiresOutputDir">Indicates if the action requires an output directory</param>
        public GameAction(string displayName, bool requiresInputDir, bool requiresOutputDir)
        {
            DisplayName = displayName;
            RequiresInputDir = requiresInputDir;
            RequiresOutputDir = requiresOutputDir;
        }

        /// <summary>
        /// The action display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Indicates if the action requires an input directory
        /// </summary>
        public bool RequiresInputDir { get; }

        /// <summary>
        /// Indicates if the action requires an output directory
        /// </summary>
        public bool RequiresOutputDir { get; }
    }
}