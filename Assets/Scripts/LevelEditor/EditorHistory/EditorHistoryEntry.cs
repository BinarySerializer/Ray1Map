namespace R1Engine
{
    /// <summary>
    /// Generic editor history entry
    /// </summary>
    /// <typeparam name="T">The type of data to save in the history</typeparam>
    public class EditorHistoryEntry<T>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="before">The state of the item before the change</param>
        /// <param name="after">The state of the item after the change</param>
        public EditorHistoryEntry(T before, T after)
        {
            Before = before;
            After = after;
        }

        /// <summary>
        /// The state of the item before the change
        /// </summary>
        public T Before { get; }

        /// <summary>
        /// The state of the item after the change
        /// </summary>
        public T After { get; }
    }
}