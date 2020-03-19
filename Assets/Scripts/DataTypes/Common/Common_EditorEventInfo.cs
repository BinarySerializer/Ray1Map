namespace R1Engine
{
    /// <summary>
    /// Common event info for the editor
    /// </summary>
    public class Common_EditorEventInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="displayName">The display name</param>
        /// <param name="flag">The flag, if available</param>
        public Common_EditorEventInfo(string displayName, EventFlag? flag)
        {
            DisplayName = displayName;
            Flag = flag;
        }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }
        
        /// <summary>
        /// The flag, if available
        /// </summary>
        public EventFlag? Flag { get; }
    }
}