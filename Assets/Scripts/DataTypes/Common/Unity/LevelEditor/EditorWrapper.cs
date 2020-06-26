namespace R1Engine
{
    /// <summary>
    /// Wrapper for editor objects
    /// </summary>
    /// <typeparam name="T">The native object type</typeparam>
    public abstract class EditorWrapper<T>
        where T : R1Serializable
    {
        protected EditorWrapper(T data)
        {
            Data = data;
        }

        /// <summary>
        /// The native data object
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Optional debug text
        /// </summary>
        public string DebugText { get; set; }

        /// <summary>
        /// Indicates if the native object has been modified and should be written to memory
        /// </summary>
        public bool HasPendingEdits { get; set; }
    }
}