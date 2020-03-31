namespace R1Engine
{
    // TODO: Remove class and only use string?
    /// <summary>
    /// Common event info for the editor
    /// </summary>
    public class Common_EditorEventInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="displayName">The display name</param>
        public Common_EditorEventInfo(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }
    }
}