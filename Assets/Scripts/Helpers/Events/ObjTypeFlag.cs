namespace R1Engine
{
    /// <summary>
    /// The event type
    /// </summary>
    public enum ObjTypeFlag
    {
        /// <summary>
        /// Normal event - appears in-game
        /// </summary>
        Normal,

        /// <summary>
        /// An always event - works together with a normal event
        /// </summary>
        Always,

        /// <summary>
        /// Editor event - does not appear in-game
        /// </summary>
        Editor
    }
}